﻿using Kilometros_WebAPI.Models.HttpGet.DataController;
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
        /// Obtener los pasos dados y la distancia recorrida por el usuario dividido por hora.
        /// </summary>
        /// <param name="from">Fecha a partir de la cual buscar datos.</param>
        /// <param name="until">Fecha hasta la cual buscar datos.</param>
        /// <returns></returns>
        /// <remarks>
        /// Sólo puede solicitarse hasta un año como rango a la vez. No hay límite en cuanto a fechas en el pasado.
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
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;

            IEnumerable<UserDataHourlyDistance> distanceView
                = Database.UserDataHourlyDistance.GetAll(
                    f =>
                        f.User_Guid == user.Guid
                        && f.Activity == activityId
                        && f.Timestamp > from && f.Timestamp < until,
                    o => o.OrderBy(b => b.Timestamp)
                );

            // --- Preparar y devolver respueta ---
            return
                from d in distanceView
                select new DataDistanceResponse() {
                    Timestamp
                        = d.Timestamp,

                    Distance
                        = d.Distance,
                    Steps
                        = d.Steps
                };
        }

        /// <summary>
        /// Obtener la distancia total recorrida por el Usuario
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("data/total")]
        public DataTotalResponse GetDistanceTotal() {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;

            // --- Obtener Distancia Total ---
            IEnumerable<UserDataTotalDistance> distanceView
                = Database.UserDataTotalDistance.GetAll(
                    o => o.User_Guid == user.Guid
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
