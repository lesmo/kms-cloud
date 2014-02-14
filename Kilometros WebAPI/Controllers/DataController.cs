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
using System.Data.Entity.Validation;
using Kilometros_WebGlobalization.API;

namespace Kilometros_WebAPI.Controllers {
    [Authorize]
    public class DataController : ApiController {
        private WorkUnit _db = new WorkUnit();

        [HttpPost]
        [Route("data")]
        public HttpResponseMessage Post([FromBody]DataPost[] dataPost) {
            /** Determinar la última fecha registrada **/
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            Data lastData
                = identity.UserData.Data
                    .OrderByDescending(data => data.TimeStamp)
                    .Take(1)
                    .FirstOrDefault();
            DateTime lastDataTimeStamp
                = lastData == null ? DateTime.MinValue : lastData.TimeStamp;

            /** Determinar los registros que se almacenarán en BD **/
            // TODO: Incluir un algoritmo que mejore la solución al problema
            //       de datos replicados.
            IEnumerable<DataPost> finalPost
                = from d in dataPost
                  where d.Timestamp > lastDataTimeStamp
                  select d;

            /** Almacenar los nuevos registros **/
            foreach ( DataPost data in finalPost ) {
                Data newData = new Data(){
                     TimeStamp = data.Timestamp,
                     Steps = data.Steps
                };

                this._db.DataStore.Add(newData);
            }
            
            int savedRecords = this._db.SaveChanges();
            
            return Request.CreateResponse(HttpStatusCode.Created);
        }
    }
}
