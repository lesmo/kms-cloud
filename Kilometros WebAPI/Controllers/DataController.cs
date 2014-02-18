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

namespace Kilometros_WebAPI.Controllers {
    /// <summary>
    /// Permite obtener y enviar Datos generados por el Podómetro de KMS.
    /// </summary>
    [Authorize]
    public class DataController : ApiController {
        private WorkUnit _db = new WorkUnit();

        /// <summary>
        /// Enviar los Datos generados por el Podómetro KMS.
        /// </summary>
        /// <param name="dataPost">Datos de la Pulesera</param>
        /// <returns></returns>
        [HttpPost]
        [Route("data")]
        public IHttpActionResult Post([FromBody]IEnumerable<DataPost> dataPost) {
            // Determinar la última fecha registrada
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            Data lastData
                = identity.UserData.Data
                    .OrderByDescending(data => data.Timestamp)
                    .Take(1)
                    .FirstOrDefault();
            DateTime lastDataTimestamp
                = lastData == null ? DateTime.MinValue : lastData.Timestamp;

            // Determinar los registros que se almacenarán en BD
            // TODO: Calcular fecha UTC a partir del Huso Horario configurado
            //       por el Usuario
            foreach ( DataPost item in dataPost )
                item.Timestamp = DateTime.SpecifyKind(
                    item.Timestamp,
                    DateTimeKind.Utc
                );

            // TODO: Incluir un algoritmo que mejore la solución al problema
            //       de datos replicados y sincronía.
            IEnumerable<DataPost> finalPost
                = from d in dataPost
                  where d.Timestamp > lastDataTimestamp
                  select d;

            // --- Almacenar los nuevos registros ---
            List<Data> addedData = new List<Data>();
            foreach ( DataPost data in finalPost ) {
                Data newData = new Data(){
                     Timestamp = data.Timestamp,
                     Steps = data.Steps
                };

                this._db.DataStore.Add(newData);
                addedData.Add(newData);
            }
            
            return Ok();
        }
    }
}
