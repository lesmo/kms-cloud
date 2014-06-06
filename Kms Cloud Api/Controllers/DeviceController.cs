using Kilometros_WebGlobalization.API;
using Kms.Cloud.Api.Exceptions;
using Kms.Cloud.Database.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kms.Cloud.Api.Controllers {
    public class DeviceController : BaseController {
        public IHttpActionResult LinkDevice(String serialString) {
            Int64 serialNumber;
            
            try {
                serialNumber = Base36Encoder.Decode(serialString);
            } catch {
                throw new HttpBadRequestException("A01" + ControllerStrings.WarningA01_DeviceNotFound);
            }

            var device = Database.DeviceStore.GetFirst(
                filter: f =>
                    f.SerialNumber == serialNumber
            );

            if ( device == null )
                throw new HttpNotFoundException("A01" + ControllerStrings.WarningA01_DeviceNotFound);

            if ( device.User != null )
                throw new HttpConflictException("A02" + ControllerStrings.WarningA02_DeviceAlreadyLinked);

            device.User = CurrentUser;
            Database.SaveChanges();

            return Ok(); 
        }


    }
}
