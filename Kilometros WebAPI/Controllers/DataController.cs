using Kilometros_WebAPI.HttpPostModels.DataController;
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
        [Route("data/{deviceIdString}/single")]
        public HttpResponseMessage Post(string deviceIdString, [FromBody]DataPost dataPost) {
            return this.Post(deviceIdString, new DataPost[]{dataPost});
        }

        [HttpPost]
        [Route("data/{deviceIdString}")]
        public HttpResponseMessage Post(string deviceIdString, [FromBody]DataPost[] dataPost) {
            /** Validar que ID de Dispositivo sea válido **/
            HttpResponseMessage response = Request.CreateResponse();
            long deviceId = 0;

            try {
                deviceId = Base36Encoder.Decode(deviceIdString);
            } catch {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "401 " + string.Format(ControllerStrings.Warning401_DeviceIdInvalid, deviceIdString)
                );
                
                return response;
            }

            /** Validar propiedad del Dispositivo **/
            KmsIdentity kmsIdentity = (KmsIdentity)this.User.Identity;
            User kmsUser = kmsIdentity.UserData;
            
            IEnumerable<WristBand> deviceResult
                = from wb in kmsUser.WristBandUserOwnership
                  where
                    wb.User.Guid == kmsUser.Guid
                    && wb.WristBand.Id == Base36Encoder.Decode(deviceIdString)
                  select wb.WristBand;

            if ( deviceResult.Count() == 0 ) {
                response.StatusCode = HttpStatusCode.NotFound;
                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "402 " + string.Format(ControllerStrings.Warning402_DeviceNotOwned, deviceIdString)
                );

                return response;
            }

            /** Determinar la última fecha registrada **/
            WristBandUserOwnership owner
                = deviceResult
                    .FirstOrDefault()
                    .WristBandUserOwnership;
            Data lastData
                = owner.Data
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
                     Steps = data.Steps,
                     WristBandUserOwnership = owner
                };

                this._db.DataStore.Add(newData);
            }
            
            int savedRecords = this._db.SaveChanges();
            response.StatusCode = HttpStatusCode.Created;
            
            return response;
        }
    }
}
