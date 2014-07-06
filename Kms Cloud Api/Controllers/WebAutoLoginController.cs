using Kms.Cloud.Api.Models.ResponseModels;
using Kms.Cloud.Api.Properties;
using Kms.Cloud.Database;
using Kms.Cloud.Database.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kms.Cloud.Api.Controllers {
    public class WebAutoLoginController : BaseController {
        private Base36Encoder Base36Encoder = new Base36Encoder();

        /// <summary>
        ///     Generar un nuevo Token de Inicio de Sesión Automático para el Dashboard Web.
        /// </summary>
        [HttpGet]
        [Route("my/web-autologin")]
        public WebAppLinkResponse GetLink() {
            var autoLoginToken = new WebAutoLoginToken {
                Token = OAuth.Token,
                Key = (Int64)(new Random().NextDouble() * 10000000000000000000),
                Secret = Guid.NewGuid(),
                IPAddress = Request.GetClientIpAddress()
            };

            Database.WebAutoLoginTokenStore.Add(autoLoginToken);
            Database.SaveChanges();

            return new WebAppLinkResponse {
                UriMask = String.Format(
                    Settings.Default.KmsWebAppUriMask,
                    Base36Encoder.Encode(autoLoginToken.Key)
                ),
                AutoLoginSecret = autoLoginToken.Secret.ToString("N")
            };
        }
    }
}
