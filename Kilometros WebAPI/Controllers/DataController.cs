﻿using Kilometros_WebAPI.Models.ResponseModels;
using Kilometros_WebAPI.Models.RequestModels;
using Kilometros_WebAPI.Security;
using KilometrosDatabase.Helpers;
using KilometrosDatabase.Abstraction;
using KilometrosDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Data.Entity.Validation;
using Kilometros_WebGlobalization.API;
using Kilometros_WebAPI.Exceptions;
using System.Threading;

namespace Kilometros_WebAPI.Controllers {
    /// <summary>
    ///     Permite subir y descargar Datos generados por los Dispositivos KMS, o los Servicios de Terceros
    ///     soportados por la Nube KMS.
    /// </summary>
    [Authorize]
    public class DataController : IKMSController {
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
        ///     Sólo puede solicitarse hasta un año como rango a la vez. No hay límite en cuanto a fechas en el pasado.
        /// </remarks>
        [HttpGet]
        [Route("data/search/{activity}")]
        public IEnumerable<DataDistanceResponse> SearchDistance(string activity, DateTime from, DateTime until) {
            // --- Validar que activity esté soportado ---
            activity
                = activity.ToLowerInvariant();
            string[] activityEnum
                = Enum.GetNames(typeof(DataActivity));
            short activityId
                = 0;

            for ( short i = 1; i < activityEnum.Length; i++ ) {
                if ( activityEnum[0].ToLowerInvariant() == activity )
                    activityId = i;
            }

            if ( activityId == 0 )
                throw new HttpNotFoundException(
                    "301" + ControllerStrings.Warning301_ActivityInvalid
                );

            // --- Establecer datos recibidos como fecha UTC ---
            from
                = DateTime.SpecifyKind(from, DateTimeKind.Utc);
            until
                = DateTime.SpecifyKind(until, DateTimeKind.Utc);

            // --- Validar los campos recibidos ---
            TimeSpan timeSpan = until - from;

            if ( timeSpan.Days < 0 ) // orden correcto
                throw new ArgumentOutOfRangeException();
            if ( timeSpan.Days > 365 ) // menor a 1 año de rango (8000~ registros)
                throw new ArgumentOutOfRangeException();

            // --- Obtener distancias recorridas en el rango especificado ---
            DataActivity dataActivity
                = (DataActivity)activityId;

            IEnumerable<UserDataHourlyDistance> distanceView
                = Database.UserDataHourlyDistance.GetAll(
                    f =>
                        f.User.Guid == CurrentUser.Guid
                        && f.Activity == dataActivity
                        && f.Timestamp > from && f.Timestamp < until,
                    o => o.OrderBy(b => b.Timestamp)
                );

            // --- Preparar y devolver respueta ---
            return (
                from d in distanceView
                select new DataDistanceResponse() {
                    Timestamp
                        = d.Timestamp,

                    Distance
                        = d.Distance,
                    Steps
                        = d.Steps
                }
            );
        }

        /// <summary>
        ///     Devuelve los pasos totales dados y la distancia total recorrida por el Usuario.
        /// </summary>
        /// <returns>
        ///     Los pasos totales dados y las distancias totales recorridas por el Usuario.
        /// </returns>
        [HttpGet]
        [Route("data/total")]
        public DataTotalResponse GetDistanceTotal() {
            // --- Obtener Distancia Total ---
            IEnumerable<UserDataTotalDistance> distanceView
                = Database.UserDataTotalDistance.GetAll(
                    o => o.User.Guid == CurrentUser.Guid
                );
            UserDataTotalDistance distanceRunning
                = (
                    from d in distanceView
                    where d.Activity == DataActivity.Running
                    select d
                ).FirstOrDefault();
            UserDataTotalDistance distanceWalking
                = (
                    from d in distanceView
                    where d.Activity == DataActivity.Walking
                    select d
                ).FirstOrDefault();

            // --- Verificar si se tiene la cabecera {If-Modified-Since} ---
            DateTimeOffset? ifModifiedSince
                = Request.Headers.IfModifiedSince;

            if ( ifModifiedSince.HasValue ) {
                if (
                    ifModifiedSince.Value.DateTime > distanceRunning.Timestamp
                    && ifModifiedSince.Value.DateTime > distanceWalking.Timestamp
                )
                    throw new HttpNotModifiedException();
            }

            // --- Preparar y devolver respuesta ---
            return new DataTotalResponse() {
                RunningTotalDistance
                    = distanceRunning.TotalDistance,
                WalkingTotalDistance
                    = distanceWalking.TotalDistance,
                TotalDistance
                    = distanceRunning.TotalDistance + distanceWalking.TotalDistance,

                RunningTotalSteps
                    = distanceRunning.TotalSteps,
                WalkingTotalSteps
                    = distanceWalking.TotalSteps,
                TotalSteps
                    = distanceRunning.TotalSteps + distanceWalking.TotalSteps,

                LastModified
                    = distanceRunning.Timestamp > distanceWalking.Timestamp
                    ? distanceRunning.Timestamp
                    : distanceWalking.Timestamp
            };
        }

        /// <summary>
        ///     Subir Datos generados por el Dispositivo KMS.
        /// </summary>
        /// <param name="dataPost">
        ///     Datos del Dispositivo KMS.
        /// </param>
        /// <returns>
        ///     
        /// </returns>
        [HttpPost]
        [Route("data/bulk")]
        public HttpResponseMessage PostDataBulk([FromBody]IEnumerable<DataPost> dataPost) {
            // --- Validar que Usuario actual tenga capturado Perfil Físico ---
            if ( CurrentUser.UserBody == null )
                throw new HttpConflictException(
                    "204 " + ControllerStrings.Warning204_PhysicalInfoNotSet
                );

            // --- Determinar la última fecha registrada ---
            Data lastData
                = Database.DataStore.GetFirst(
                    filter: f =>
                        f.User.Guid == CurrentUser.Guid,
                    orderBy: o =>
                        o.OrderByDescending(b => b.Timestamp)
                );
            DateTime lastDataTimestamp
                = lastData == null
                ? DateTime.MinValue
                : lastData.Timestamp;

            // --- Determinar los registros si se almacenarán en BD ---
            // Especificar que fechas son UTC
            foreach ( DataPost i in dataPost )
                i.Timestamp
                    = DateTime.SpecifyKind(
                        i.Timestamp,
                        DateTimeKind.Utc
                    );

            // TODO: Incluir un algoritmo que mejore la solución al problema
            //       de datos replicados y sincronía.
            HashSet<DataPost> finalDataPost
                = new HashSet<DataPost>();

            foreach ( DataPost i in dataPost ) {
                if ( i.Timestamp > lastDataTimestamp )
                    finalDataPost.Add(i);
            }

            // --- Almacenar los nuevos Datos ---
            foreach ( DataPost i in dataPost )
                this.PrepareDataInsert(i);

            Database.SaveChanges();

            // --- Ejecutar magia --
            this.MagicTriggers();
            
            return new HttpResponseMessage() {
                RequestMessage
                    = Request,

                StatusCode
                    = HttpStatusCode.OK
            };
        }

        /// <summary>
        ///     Subir un solo Dato generado por el Dispositivo KMS.
        /// </summary>
        /// <param name="dataPost">
        ///     Dato del Dispositivo KMS.
        /// </param>
        /// <returns>
        ///     
        /// </returns>
        public HttpResponseMessage PostData([FromBody]DataPost dataPost) {
            // --- Validar que Usuario actual tenga capturado Perfil Físico ---
            if ( CurrentUser.UserBody == null )
                throw new HttpConflictException(
                    "204 " + ControllerStrings.Warning204_PhysicalInfoNotSet
                );

            // --- Determinar la última fecha registrada ---
            Data lastData
                = Database.DataStore.GetFirst(
                    f => f.User.Guid == CurrentUser.Guid,
                    o => o.OrderByDescending(b => b.Timestamp)
                );
            DateTime lastDataTimestamp
                = lastData == null
                ? DateTime.MinValue
                : lastData.Timestamp;

            // --- Determinar los registros si se almacenarán en BD ---
            // Especificar que fecha es UTC
            dataPost.Timestamp
                = DateTime.SpecifyKind(
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
                RequestMessage
                    = Request,

                StatusCode
                    = HttpStatusCode.OK
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
            string activity
                = dataPost.Activity.ToLowerInvariant();
            string[] activityEnum
                = Enum.GetNames(typeof(DataActivity));
            short activityId
                = 0;

            for ( short i = 1; i < activityEnum.Length; i++ ) {
                if ( activityEnum[0].ToLowerInvariant() == activity )
                    activityId = i;
            }

            if ( activityId == 0 )
                throw new HttpBadRequestException(
                    "301" + ControllerStrings.Warning301_ActivityInvalid
                );

            // --- Determinar el largo de zancada ---
            int strideLength;
            switch ( (DataActivity)activityId ) {
                case DataActivity.Running:
                    strideLength
                        = CurrentUser.UserBody.StrideLengthRunning;
                    break;
                case DataActivity.Walking:
                    strideLength
                        = CurrentUser.UserBody.StrideLengthWalking;
                    break;
                default:
                    strideLength
                        = 0;
                    break;
            }

            // --- Añadir el nuevo registro ---
            Data newData
                = new Data() {
                    User
                        = CurrentUser,

                    Timestamp
                        = dataPost.Timestamp,
                    Activity
                        = (DataActivity)activityId,
                    Steps
                        = dataPost.Steps,
                    StrideLength
                        = strideLength,
                    EqualsKcal
                        = (int)CurrentUser.UserBody.CalculateCaloriesBurned(
                            dataPost.Steps,
                            (DataActivity)activityId
                        ),
                    EqualsCo2
                        = (int)(
                            // Distancia en la lectura
                            (strideLength * dataPost.Steps).CentimetersToKilometers()
                            // [x] Litros de Gasolina por Kilómetro
                            * 1.25d
                            // [x] Gramos de CO2 por Litro de Gasolina
                            * 2303
                        ),
                    EqualsCash
                        = (int)(
                            (
                                // Distancia en la lectura
                                (strideLength * dataPost.Steps).CentimetersToKilometers()
                                // [/] Kilómetros por Litro de Gasoilna
                                / 10d
                            // [x] Precio por Litro de Gasolina en Dólares
                            ) * 1.0d
                        )
                };

            Database.DataStore.Add(newData);
        }

        private void MagicTriggers() {
            new Thread(
                new ParameterizedThreadStart((object userGuid) => {
                    new MagicTriggers.AsyncRewardTipTrigger().ForUserGuid((Guid)userGuid);
                })
            ).Start(CurrentUser.Guid);
        }
    }

}
