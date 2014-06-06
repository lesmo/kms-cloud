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
    /// <summary>
    ///     Obtener y asociar Dispositivos KMS a la Cuenta del Usuario. Los Numeros de Serie pueden
    ///     contener números, sólo las letras ACEFHJKLMNPRTVWXZ, son indistintos de mayúsculas y
    ///     minúsculas y tienen un largo de 7 carácteres, aunque en el futuro podrían tener un largo
    ///     mayor.
    /// </summary>
    public class DeviceController : BaseController {
        private const String serialStringCharMap = "0123456789ACEFHJKLMNPRTVWXZ";

        /// <summary>
        ///     Asociar un Número de Serie de Dispositivo KMS con el Usuario actual. Sólo es posible
        ///     asociar un Dispositivo a un Usuario, intentar asociar un dispositivo que ya está
        ///     asociado con un Usuario causará el asesinato de un cachorrito.
        /// </summary>
        /// <param name="serialString">Número de Serie de de Dispositivo KMS</param>
        public IHttpActionResult LinkDevice(String serialString) {
            Int64 serialNumber;
            var serialEncoder = new BaseNumericEncoder(serialStringCharMap);
            
            try {
                serialNumber = serialEncoder.Decode(serialString);
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
