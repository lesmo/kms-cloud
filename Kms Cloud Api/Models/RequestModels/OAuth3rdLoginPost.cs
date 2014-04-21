using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.RequestModels {
    public class OAuth3rdLoginPost : IOAuthTokenSecretPost {
        /// <summary>
        ///     Token de Sesión generado por el servicio OAuth de Terceros.
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        ///     Token Secret de Sesión generado por el servicio OAuth de Terceros.
        ///     Los servicios que utilicen OAuth 2.0 no utilizan un Token Secret,
        ///     y en esos casos debe omitirse.
        /// </summary>
        public string TokenSecret { get; set; }
        /// <summary>
        ///     ID del Usuario en el servicio OAuth de Terceros.
        /// </summary>
        public string ID { get; set; }
    }
}