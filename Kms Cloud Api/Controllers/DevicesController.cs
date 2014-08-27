using Kilometros_WebGlobalization.API;
using Kms.Cloud.Api.Exceptions;
using Kms.Cloud.Api.Models.ResponseModels;
using Kms.Cloud.Database.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;

namespace Kms.Cloud.Api.Controllers {
    /// <summary>
    ///     Obtener y asociar Dispositivos KMS a la Cuenta del Usuario.
    /// </summary>
    public class DevicesController : BaseController {
        private readonly KmsSerialEncoder SerialEncoder = new KmsSerialEncoder();

        /// <summary>
        ///     Obtener los dispositivos asociados con la Cuenta del Usuario.
        /// </summary>
        [HttpGet]
        [Route("my/devices")]
        public IEnumerable<DeviceResponse> ListDevices() {
            return Database.DeviceStore.GetAll(
                filter: f =>
                    f.User.Guid == CurrentUser.Guid,
                orderBy: o =>
                    o.OrderBy(b => b.LinkDate)
            ).Select(s => new DeviceResponse {
                LinkDate = s.LinkDate.Value,
                SerialString = SerialEncoder.Encode(s.SerialNumber)
            });
        }

        /// <summary>
        ///     Asociar un Número de Serie de Dispositivo KMS con el Usuario actual. Sólo es posible
        ///     asociar un Dispositivo a un Usuario, intentar asociar un dispositivo que ya está
        ///     asociado con un Usuario causará el asesinato de un cachorrito.
        /// </summary>
        /// <param name="serialString">
        ///     Número de Serie de de Dispositivo KMS. Si tu API-Key está en modo DEBUG, puedes utilizar
        ///     "1234567" para generar automáticamente un número de serie nuevo y asociarlo al Usuario.
        /// </param>
        /// <remarks>
        ///     Los Numeros de Serie pueden contener los carácteres 0123456789ACEFHJKLMNPRTVWXZ, son
        ///     indistintos de mayúsculas y minúsculas y tienen un largo de 7 carácteres, aunque en el 
        ///     futuro podrían tener un largo mayor. Usa esta información a tu favor, joven Padawan.
        /// </remarks>
        [HttpPost]
        [Route("my/devices/link/{serialString}")]
        public IHttpActionResult LinkDevice(String serialString) {
            Int64 serialNumber;

            if ( OAuth.ConsumerKey.DebugEnabled && serialString.ToUpper(CultureInfo.CurrentCulture) == "1234567") {
                serialNumber = new Random().Next(99999, 999999999);

                Database.DeviceStore.Add(new Database.Device {
                    SerialNumber = serialNumber
                });

                Database.SaveChanges();
            } else {
                try {
                    serialNumber = SerialEncoder.Decode(serialString);
                } catch {
                    throw new HttpBadRequestException("A01" + ControllerStrings.WarningA01_DeviceNotFound);
                }
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
            device.LinkDate = DateTime.UtcNow;

            if ( !String.IsNullOrEmpty(device.RegionCode) )
                CurrentUser.RegionCode = device.RegionCode;

            Database.SaveChanges();

            return Ok(); 
        }
    }
}
