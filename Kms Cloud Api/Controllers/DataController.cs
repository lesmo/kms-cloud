using Kms.Cloud.Api.Models.ResponseModels;
using Kms.Cloud.Api.Models.RequestModels;
using Kms.Cloud.Api.Security;
using Kms.Cloud.Database.Helpers;
using Kms.Cloud.Database.Abstraction;
using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Data.Entity.Validation;
using Kilometros_WebGlobalization.API;
using Kms.Cloud.Api.Exceptions;
using System.Threading;
using Kms.Cloud.Api.MagicTriggers;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace Kms.Cloud.Api.Controllers {
    /// <summary>
    ///     Subir y bajar Datos generados por los Dispositivos KMS, o los Servicios de Terceros
    ///     soportados por la Nube KMS.
    /// </summary>
    public class DataController : BaseController {
        /// <summary>
        ///     Devuelve los pasos dados y la distancia recorrida por el Usuario divididos por hora,
        ///     únicamente en el rango temporal establecido.
        /// </summary>
        /// <param name="activity">
        ///     Actividad a buscar. Puede ser: "running", "walking" o "sleeping".
        /// </param>
        /// <param name="from">
        ///     Fecha a partir de la cual buscar datos.
        /// </param>
        /// <param name="until">
        ///     Fecha hasta la cual buscar datos.
        /// </param>
        /// <returns></returns>
        /// <remarks>
        ///     Sólo puede solicitarse hasta un año como rango a la vez. Por ahora, no hay límite en cuanto a
        ///     fechas en el pasado pero éste comportamiento podría cambiar.
        /// </remarks>
        [HttpGet]
        [Route("data/search/{activity}")]
        public IEnumerable<DataDistanceResponse> SearchDistance(string activity, DateTime from, DateTime until) {
            // --- Validar que activity esté soportado ---
            activity         = activity.ToUpper(CultureInfo.InvariantCulture);
            var activityEnum = Enum.GetNames(typeof(DataActivity));
            short activityId = 0;

            for ( short i = 0; i < activityEnum.Length; i++ ) {
                if ( activityEnum[i].ToUpper(CultureInfo.InvariantCulture) == activity )
                    activityId = (short)(i + 1);
            }

            if ( activityId == 0 )
                throw new HttpNotFoundException(
                    "301" + ControllerStrings.Warning301_ActivityInvalid
                );

            // --- Establecer datos recibidos como fecha UTC ---
            from  = DateTime.SpecifyKind(from, DateTimeKind.Utc);
            until = DateTime.SpecifyKind(until, DateTimeKind.Utc);

            // --- Validar los campos recibidos ---
            TimeSpan timeSpan = until - from;

            if ( timeSpan.Days < 0 ) // orden correcto
                throw new ArgumentOutOfRangeException("from", "You're joking, right?");
            if ( timeSpan.Days > 365 ) // menor a 1 año de rango (8000~ registros)
                throw new ArgumentOutOfRangeException("until", "Time range is so fucking long.");

            // --- Obtener distancias recorridas en el rango especificado ---
            var dataActivity = (DataActivity)activityId;
            var distanceView = Database.UserDataHourlyDistance.GetAll(
                    filter: f =>
                        f.User.Guid == CurrentUser.Guid
                        && f.Activity == dataActivity
                        && f.Timestamp > from && f.Timestamp < until,
                    orderBy: o =>
                        o.OrderBy(b => b.Timestamp)
                );

            // --- Preparar y devolver respueta ---
            return (
                from d in distanceView
                select new DataDistanceResponse() {
                    Timestamp = d.Timestamp,
                    Distance  = d.Distance,
                    Steps     = d.Steps
                }
            );
        }

        /// <summary>
        ///     Devuelve los pasos totales dados y la distancia total recorrida por el Usuario.
        /// </summary>
        /// <returns>
        ///     Los pasos totales dados y las distancias totales recorridas por el Usuario.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [HttpGet, Route("data/total")]
        public DataTotalResponse GetDistanceTotal() {
            // --- Verificar si se tiene la cabecera {If-Modified-Since} ---
            DateTimeOffset? ifModifiedSince
                = Request.Headers.IfModifiedSince;

            if ( ifModifiedSince.HasValue ) {
                if ( CurrentUser.UserDataTotalDistanceSum.Timestamp != default(DateTime) ) {
                    if ( ifModifiedSince.Value.DateTime < CurrentUser.UserDataTotalDistanceSum.Timestamp )
                        throw new HttpNotModifiedException();
                }
            }

            if ( CurrentUser.UserDataTotalDistanceSum.Timestamp == default(DateTime) ) {
                return new DataTotalResponse {
                    RunningTotalDistance = 0,
                    WalkingTotalDistance = 0,
                    TotalDistance        = 0,

                    RunningTotalSteps = 0,
                    WalkingTotalSteps = 0,
                    TotalSteps        = 0,

                    // TODO: Cambiar con fecha en la que se lanza KMS.
                    LastModified = new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                };
            }

            var distanceRunning = CurrentUser.UserDataTotalDistance.FirstOrDefault(w => w.Activity == DataActivity.Running)
                ?? new UserDataTotalDistance {
                    Timestamp = DateTime.UtcNow,
                    TotalDistance = 0,
                    TotalSteps = 0
                };
            var distanceWalking = CurrentUser.UserDataTotalDistance.FirstOrDefault(w => w.Activity == DataActivity.Walking)
                ?? new UserDataTotalDistance {
                    Timestamp = DateTime.UtcNow,
                    TotalDistance = 0,
                    TotalSteps = 0
                };

            // --- Preparar y devolver respuesta ---
            return new DataTotalResponse {
                RunningTotalDistance = distanceRunning.TotalDistance,
                WalkingTotalDistance = distanceWalking.TotalDistance,
                TotalDistance        = distanceRunning.TotalDistance + distanceWalking.TotalDistance,

                RunningTotalSteps = distanceRunning.TotalSteps,
                WalkingTotalSteps = distanceWalking.TotalSteps,
                TotalSteps        = distanceRunning.TotalSteps + distanceWalking.TotalSteps,

                LastModified = distanceRunning.Timestamp > distanceWalking.Timestamp
                    ? distanceRunning.Timestamp
                    : distanceWalking.Timestamp
            };
        }

        /// <summary>
        ///     Subir Datos generados por el Dispositivo KMS.
        /// </summary>
        /// <param name="dataPost">
        ///     Registros generados por el Dispositivo KMS. Cada registro corresponde a una franja
        ///     temporal de 2 minutos.
        /// </param>
        [HttpPost]
        [Route("data/bulk")]
        public HttpResponseMessage PostDataBulk([FromBody]IEnumerable<DataPost> dataPost) {
            if ( dataPost.Count() < 1 )
                throw new HttpBadRequestException();

            // --- Validar que Usuario actual tenga asociado al menos un Dispositivo ---
            if ( CurrentUser.Device.Count < 1 )
                throw new HttpConflictException("207 " + ControllerStrings.Warning207_NoDevicesLinked);

            // --- Validar que Usuario actual tenga capturado Perfil Físico ---
            if ( CurrentUser.UserBody == null )
                throw new HttpConflictException(
                    "204 " + ControllerStrings.Warning204_PhysicalInfoNotSet
                );

            // --- Determinar la última fecha registrada ---
            var lastData = Database.DataStore.GetFirst(
                filter: f =>
                    f.User.Guid == CurrentUser.Guid,
                orderBy: o =>
                    o.OrderByDescending(b => b.Timestamp)
            );
            var lastDataTimestamp =
                lastData == null
                    ? DateTime.MinValue
                    : lastData.Timestamp;

            // --- Determinar los registros que si se almacenarán en BD ---
            // Especificar que fechas son UTC
            foreach ( DataPost i in dataPost )
                i.Timestamp = DateTime.SpecifyKind(
                    i.Timestamp,
                    DateTimeKind.Utc
                );

            // TODO: Incluir un algoritmo que mejore la solución al problema
            //       de datos replicados y sincronía.
            var finalDataPost = new List<DataPost>();

            foreach ( DataPost i in dataPost ) {
                if ( i.Timestamp > lastDataTimestamp && i.Timestamp < DateTime.UtcNow )
                    finalDataPost.Add(i);
            }

            // --- Almacenar los nuevos Datos ---
            foreach ( DataPost i in dataPost )
                this.PrepareDataInsert(i);

            Database.SaveChanges();

            // --- Ejecutar magia --
            this.MagicTriggers();
            
            return new HttpResponseMessage() {
                RequestMessage = Request,
                StatusCode     = HttpStatusCode.OK
            };
        }

        /// <summary>
        ///     Subir un solo Registro generado por el Dispositivo KMS. El uso de éste recurso está
        ///     desaconsejado para Apps de producción (rápidamente podrías consumir la cuota de peticiones
        ///     que tiene tu API-Key). Está aquí principalmente para debugging en ambas partes (server y Apps).
        /// </summary>
        /// <param name="dataPost">
        ///     Registro del Dispositivo KMS.
        /// </param>
        public HttpResponseMessage PostData([FromBody]DataPost dataPost) {
            // --- Validar que Usuario actual tenga capturado Perfil Físico ---
            if ( CurrentUser.UserBody == null )
                throw new HttpConflictException(
                    "204 " + ControllerStrings.Warning204_PhysicalInfoNotSet
                );

            // --- Determinar la última fecha registrada ---
            Data lastData = Database.DataStore.GetFirst(
                filter: f =>
                    f.User.Guid == CurrentUser.Guid,
                orderBy: o =>
                    o.OrderByDescending(b => b.Timestamp)
            );

            DateTime lastDataTimestamp = lastData == null
                ? DateTime.MinValue
                : lastData.Timestamp;

            // --- Determinar los registros si se almacenarán en BD ---
            // Especificar que fecha es UTC
            dataPost.Timestamp = DateTime.SpecifyKind(
                dataPost.Timestamp,
                DateTimeKind.Utc
            );
            
            if ( dataPost.Timestamp < lastDataTimestamp )
                throw new HttpConflictException(
                    ControllerStrings.Warning302_DataTimestampTooOld
                );
            
            this.PrepareDataInsert(dataPost);
            Database.SaveChanges();

            // --- Ejecutar magia --
            this.MagicTriggers();

            return new HttpResponseMessage() {
                RequestMessage = Request,
                StatusCode     = HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Prepara la inserción de Datos, sin consultar detalles del
        /// Perfil Físico del Usuario aún.
        /// </summary>
        /// <param name="dataPost"></param>
        /// <param name="userBody"></param>
        private void PrepareDataInsert(DataPost dataPost) {
            // --- Validar que Activity esté soportado ---
            var activity     = dataPost.Activity.ToUpper(CultureInfo.InvariantCulture);
            var activityEnum = Enum.GetNames(typeof(DataActivity));
            short activityId = 0;

            for ( short i = 0; i < activityEnum.Length; i++ ) {
                if ( activityEnum[i].ToUpper(CultureInfo.InvariantCulture) == activity ) {
                    activityId = (short)(i + 1);
                    break;
                }
            }

            if ( activityId == 0 )
                throw new HttpBadRequestException(
                    "301 " + ControllerStrings.Warning301_ActivityInvalid
                );

            var dataActivity = (DataActivity)activityId;

            // --- Determinar el largo de zancada ---
            int strideLength;
            switch ( dataActivity ) {
                case DataActivity.Running:
                    strideLength = CurrentUser.UserBody.StrideLengthRunning;
                    break;
                case DataActivity.Walking:
                    strideLength = CurrentUser.UserBody.StrideLengthWalking;
                    break;
                default:
                    strideLength = 0;
                    break;
            }

            // --- Añadir el nuevo registro ---
            Data newData
                = new Data() {
                    User = CurrentUser,

                    Timestamp    = dataPost.Timestamp,
                    Activity     = dataActivity,
                    Steps        = dataPost.Steps,
                    StrideLength = strideLength,

                    EqualsKcal = (Int32)CurrentUser.UserBody.CalculateCaloriesBurned(
                            dataPost.Steps,
                            dataActivity
                        ),
                    EqualsCo2 = (Int32)(
                            // Distancia en la lectura
                            (strideLength * dataPost.Steps).CentimetersToKilometers()
                            // [x] Litros de Gasolina por Kilómetro
                            * 1.25d
                            // [x] Gramos de CO2 por Litro de Gasolina
                            * 2303
                        ),
                    EqualsCash = (Int32)(
                            (
                                // Distancia en la lectura
                                (strideLength * dataPost.Steps).CentimetersToKilometers()
                                // [/] Kilómetros por Litro de Gasoilna
                                / 10d
                            // [x] Precio por Litro de Gasolina en Centavos de Dólar
                            ) * 100.0d
                        )
                };

            Database.DataStore.Add(newData);
        }

        private void MagicTriggers() {
            new Thread(
                new ParameterizedThreadStart((object user) => {
                    new RewardTipTrigger(user as User);
                })
            ).Start(CurrentUser);
        }
    }

}
