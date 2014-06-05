using Kilometros_WebGlobalization.API;
using Kms.Cloud.Api.Exceptions;
using Kms.Cloud.Api.Models.RequestModels;
using Kms.Cloud.Api.Properties;
using Kms.Cloud.Database;
using Kms.Interop.OAuth;
using Kms.Interop.OAuth.SocialClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kms.Cloud.Api.Controllers {
    /// <summary>
    ///     Permite asociar cuentas de Redes Sociales a la Cuenta del Usuario de la sesión actual,
    ///     de forma que pueda iniciar sesión con ellas y, en su caso, integrar información de servicios
    ///     de terceros soportados por KMS.
    /// </summary>
    public class OAuth3rdPartyAddController : OAuthBaseController {
        /// <summary>
        ///     Asociar una cuenta de Facebook con el Usuario actual de KMS.
        /// </summary>
        /// <param name="dataPost">
        ///     Código de sesión y ID del Usuario en Facebook.
        /// </param>
        [HttpPost]
        [Route("oauth/3rd/facebook/add")]
        public IHttpActionResult FacebookAdd([FromBody]OAuthFacebookLoginPost dataPost) {
            ValidateOAuth3rdAddRequest(OAuthService.Facebook, dataPost);
            var facebookClient = OAuth3rdClient<FacebookClient>(dataPost);

            OAuthCredential facebookCredential;
            try {
                facebookClient.ExchangeCodeForToken(dataPost.Code);

                facebookCredential = new OAuthCredential {
                    OAuthProvider = OAuthService.Facebook,
                    Uid       = facebookClient.UserID,
                    Token     = facebookClient.Token.Key,
                    IsInvalid = false,

                    User      = CurrentUser
                };
            } catch ( OAuthUnexpectedResponse ex ) {
                throw new HttpNotFoundException(
                    "105 " + ControllerStrings.Warning105_SocialTokenNotFound,
                    ex
                );
            }
            
            Database.OAuthCredentialStore.Add(facebookCredential);
            Database.SaveChanges();

            return Ok();
        }

        /// <summary>
        ///     Asociar una cuenta de Twitter con el Usuario actual de KMS.
        /// </summary>
        /// <param name="dataPost">
        ///     Información de Sesión del Usuario de Twitter.
        /// </param>
        [HttpPost]
        [Route("oauth/3rd/twitter/add")]
        public IHttpActionResult TwitterAdd([FromBody]OAuth3rdLoginPost dataPost) {
            ValidateOAuth3rdAddRequest(OAuthService.Twitter, dataPost);
            var twitterClient = OAuth3rdClient<TwitterClient>(dataPost);

            OAuthCredential twitterCredential;
            try {
                twitterCredential = new OAuthCredential {
                    OAuthProvider = OAuthService.Twitter,
                    Uid       = twitterClient.UserID,
                    Token     = twitterClient.Token.Key,
                    Secret    = twitterClient.Token.Secret,
                    IsInvalid = false,

                    User      = CurrentUser
                };
            } catch ( OAuthUnexpectedResponse ex ) {
                throw new HttpNotFoundException(
                    "105 " + ControllerStrings.Warning105_SocialTokenNotFound,
                    ex
                );
            }

            Database.OAuthCredentialStore.Add(twitterCredential);
            Database.SaveChanges();

            return Ok();
        }

        /// <summary>
        ///     Asociar una cuenta de Fitbit con el Usuario actual de KMS. Actualmente
        ///     la funcionalidad no está implementada, y enviar peticiones causará
        ///     la muerte de un gatito.
        /// </summary>
        /// <param name="postData"></param>
        [HttpPost]
        [Route("oauth/3rd/fitbit/add")]
        public void FitbitAdd([FromBody]OAuth3rdLoginPost postData) {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Asociar una cuenta de Nike+ con el Usuario actual de KMS. Actualmente
        ///     la funcionalidad no está implementada, y enviar peticiones causará
        ///     la muerte de un gatito.
        /// </summary>
        /// <param name="postData"></param>
        [HttpPost]
        [Route("oauth/3rd/nike/add")]
        public void NikeAdd([FromBody]OAuth3rdLoginPost postData) {
            throw new NotImplementedException();
        }
    }
}
