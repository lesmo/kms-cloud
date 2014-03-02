using Kilometros_WebAPI.Models.HttpGet.DataController;
using Kilometros_WebAPI.Models.HttpPost.DataController;
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

namespace Kilometros_WebAPI.Controllers {
    /// <summary>
    /// Permite obtener y enviar Datos generados por el Podómetro de KMS.
    /// </summary>
    [Authorize]
    public class DataController : ApiController {
        private readonly Dictionary<DataActivity, string> DataActivityString = new Dictionary<DataActivity,string> {
            {DataActivity.Running, "run"},
            {DataActivity.Walking, "walk"},
            {DataActivity.Sleep, "sleep"}
        };

        private WorkUnit Database = new WorkUnit();

        /// <summary>
        /// Obtener los pasos dados por el usuario divididos por hora.
        /// </summary>
        /// <param name="from">Fecha a partir de la cual buscar datos.</param>
        /// <param name="until">Fecha hasta la cual buscar datos.</param>
        /// <returns></returns>
        /// <remarks>
        /// Sólo puede solicitarse hasta un año como rango a la vez. No hay límite en cuanto a fechas en el pasado.
        /// </remarks>
        [HttpGet]
        [Route("data/search/steps")]
        public IEnumerable<DataStepsResponse> SearchSteps(DateTime from, DateTime until) {
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

            // --- Obtener pasos dados en el rango especificado ---
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;

            IEnumerable<UserDataHourlyStepsView> stepsView
                = Database.UserDataHourlyStepsView.GetAll(
                    f => f.User_Guid == user.Guid,
                    o => o.OrderBy(b => b.Timestamp)
                );

            // --- Preparar y devolver respuesta ---
            return
                from s in stepsView
                select new DataStepsResponse() {
                    Timestamp
                        = s.Timestamp,
                    Activity
                        = DataActivityString[s.Activity],
                    Steps
                        = s.TotalSteps
                };
        }

        /// <summary>
        /// Obtener la distancia recorrida por el usuario dividido por hora.
        /// </summary>
        /// <param name="from">Fecha a partir de la cual buscar datos.</param>
        /// <param name="until">Fecha hasta la cual buscar datos.</param>
        /// <returns></returns>
        /// <remarks>
        /// Sólo puede solicitarse hasta un año como rango a la vez. No hay límite en cuanto a fechas en el pasado.
        /// </remarks>
        [HttpGet]
        [Route("data/search/distance")]
        public IEnumerable<DataDistanceResponse> SearchDistance(DateTime from, DateTime until) {
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
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;

            IEnumerable<UserDataHourlyDistanceView> distanceView
                = Database.UserDataHourlyDistanceView.GetAll(
                    f => f.User_Guid == user.Guid && f.Timestamp > from && f.Timestamp < until,
                    o => o.OrderBy(b => b.Timestamp)
                );

            // --- Preparar y devolver respueta ---
            return
                from d in distanceView
                select new DataDistanceResponse() {
                    Timestamp
                        = d.Timestamp,

                    RunningDistance
                        = d.RunningDistance,
                    WalkingDistance
                        = d.WalkingDistance,
                    TotalDistance
                        = d.RunningDistance + d.WalkingDistance
                };
        }

        /// <summary>
        /// Obtener la distancia total recorrida por el Usuario
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("data/total/distance")]
        public DataTotalDistanceResponse GetDistanceTotal() {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;

            // --- Obtener Distancia Total ---
            UserDataTotalDistanceView distanceView
                = Database.UserDataTotalDistanceView.GetFirst(
                    o => o.User_Guid == user.Guid
                );

            // --- Verificar si se tiene la cabecera {If-Modified-Since} ---
            DateTimeOffset? ifModifiedSince
                = Request.Headers.IfModifiedSince;

            if ( ifModifiedSince.HasValue ) {
                if ( ifModifiedSince.Value.DateTime > distanceView.Timestamp )
                    throw new HttpNotModifiedException();
            }

            // --- Preparar y devolver respuesta ---
            return new DataTotalDistanceResponse() {
                RunningTotalDistance
                    = distanceView.RunningDistance,
                WalkingTotalDistance
                    = distanceView.WalkingDistance,
                TotalDistance
                    = distanceView.TotalDistance,

                LastModified
                    = distanceView.Timestamp
            };
        }

        /// <summary>
        /// Obtener el total de pasos dados por el Usuario
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("data/total/steps")]
        public DataTotalStepsResponse GetStepsTotal() {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;

            // --- Obtener Pasos Totales ---
            IEnumerable<UserDataTotalStepsView> stepsView
                = Database.UserDataTotalStepsView.GetAll(
                    o => o.User_Guid == user.Guid
                );
            DateTime stepsMaxTimestamp
                = (
                    from s in stepsView
                    where s.Activity == DataActivity.Running || s.Activity == DataActivity.Walking
                    select s.Timestamp
                ).Max();

            // --- Verificar si se tiene la cabecera {If-Modified-Since} ---
            DateTimeOffset? ifModifiedSince
                = Request.Headers.IfModifiedSince;

            if ( ifModifiedSince.HasValue ) {
                if ( ifModifiedSince.Value.DateTime > stepsMaxTimestamp )
                    throw new HttpNotModifiedException();
            }

            // --- Obtener Pasos Totales Corriendo y Caminando ---
            UserDataTotalStepsView stepsRunning
                = (
                    from s in stepsView
                    where s.Activity == DataActivity.Running
                    select s
                ).FirstOrDefault();
            UserDataTotalStepsView stepsWalking
                = (
                    from s in stepsView
                    where s.Activity == DataActivity.Walking
                    select s
                ).FirstOrDefault();

            // --- Preparar y devolver respuesta ---
            return new DataTotalStepsResponse() {
                RunningTotalSteps
                    = stepsRunning.TotalSteps,
                WalkingTotalSteps
                    = stepsWalking.TotalSteps,
                TotalSteps
                    = stepsRunning.TotalSteps + stepsWalking.TotalSteps,

                LastModified
                    = stepsMaxTimestamp
            };
        }

        /// <summary>
        /// Enviar los Datos generados por el Podómetro KMS.
        /// </summary>
        /// <param name="dataPost">Datos de la Pulsera</param>
        /// <returns></returns>
        [HttpPost]
        [Route("data")]
        public IHttpActionResult PostData([FromBody]IEnumerable<DataPost> dataPost) {
            // --- Determinar la última fecha registrada ---
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;

            Data lastData
                = Database.DataStore.GetFirst(
                    f => f.User == user,
                    o => o.OrderByDescending(b => b.Timestamp)
                );
            DateTime lastDataTimestamp
                = lastData == null
                ? DateTime.MinValue
                : lastData.Timestamp;

            // --- Determinar los registros que se almacenarán en BD ---
            // Especificar que fechas son UTC
            foreach ( DataPost item in dataPost )
                item.Timestamp
                    = DateTime.SpecifyKind(
                        item.Timestamp,
                        DateTimeKind.Utc
                    );

            // TODO: Incluir un algoritmo que mejore la solución al problema
            //       de datos replicados y sincronía.
            IEnumerable<DataPost> finalPost
                = (
                    from d in dataPost
                    where d.Timestamp > lastDataTimestamp
                    select d
                );

            // --- Almacenar los nuevos registros ---
            List<Data> addedData 
                = new List<Data>();

            foreach ( DataPost data in finalPost ) {
                Data newData
                    = new Data(){
                         Timestamp = data.Timestamp,
                         Steps = data.Steps
                    };

                Database.DataStore.Add(newData);
                addedData.Add(newData);
            }
            
            return Ok();
        }
    }
}
