using Kilometros_WebGlobalization.API;
using Kms.Cloud.Api.Exceptions;
using Kms.Cloud.Api.Models.RequestModels;
using Kms.Cloud.Api.Models.ResponseModels;
using Kms.Cloud.Api.Properties;
using Kms.Cloud.Api.Security;
using Kms.Cloud.Database;
using Kms.Interop.OAuth;
using Kms.Interop.OAuth.SocialClients;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Kms.Cloud.Api.Controllers {
    public class OAuth3rdPartyLoginController : OAuthBaseController {
        /// <summary>
        ///     Intercambia un Request Token de OAuth para la Nube de KMS por un Access Token,
        ///     utilizando un Código de Facebook.
        /// </summary>
        /// <remarks>
        ///     El Código de Facebook NO ES el Token, es un código generado por el proceso de Login
        ///     y es diferente del Token. Facebook utiliza este proceso para poder compartir la
        ///     Sesión del Usuario entre Apps y Servidor Web, y mantener así Tokens independientes
        ///     y seguros.
        /// </remarks>
        /// <param name="dataPost">
        ///     Aparte de la cabecera propia de OAuth, en el cuerpo de POST deben incluirse el Código
        ///     de Facebook y el ID de Usuario en Faceboook.
        /// </param>
        [AllowAnonymous]
        [HttpPost, Route("oauth/3rd/facebook/login")]
        public HttpResponseMessage FacebookLogin([FromBody]OAuthFacebookLoginPost dataPost) {
            var facebookClient = OAuth3rdClient<FacebookClient>(
                dataPost,
                new OAuthCryptoSet(
                    Settings.Default.FacebookClientConsumerKey,
                    Settings.Default.FacebookClientConsumerSecret
                )
            );

            string facebookUserID;
            try {
                facebookClient.ExchangeCodeForToken(dataPost.Code);
                facebookUserID = facebookClient.UserID;
            } catch ( OAuthUnexpectedResponse ex ) {
                OAuth.Token.LoginAttempts++;
                Database.TokenStore.Update(OAuth.Token);
                Database.SaveChanges();

                throw new HttpBadRequestException(
                    "105 " + ControllerStrings.Warning105_SocialTokenNotFound,
                    ex
                );
            }

            OAuth3rdCredential.Token = facebookClient.Token.Key;
            Database.OAuthCredentialStore.Update(OAuth3rdCredential);

            return ExchangeOAuthAccessToken();
        }

        [AllowAnonymous]
        [HttpPost, Route("oauth/3rd/twitter/login")]
        public HttpResponseMessage TwitterLogin([FromBody]OAuth3rdLoginPost dataPost) {
            var twitterClient = OAuth3rdClient<TwitterClient>(
                dataPost,
                new OAuthCryptoSet(
                    Settings.Default.TwitterClientConsumerKey,
                    Settings.Default.TwitterClientConsumerSecret
                )
            );

            string twitterUserID;
            try {
                twitterUserID = twitterClient.UserID;
            } catch ( OAuthUnexpectedResponse ex ) {
                OAuth.Token.LoginAttempts++;
                Database.TokenStore.Update(OAuth.Token);
                Database.SaveChanges();

                throw new HttpBadRequestException(
                    "105 " + ControllerStrings.Warning105_SocialTokenNotFound,
                    ex
                );
            }

            OAuth3rdCredential.Token  = twitterClient.Token.Key;
            OAuth3rdCredential.Secret = twitterClient.Token.Secret;
            Database.OAuthCredentialStore.Update(OAuth3rdCredential);

            return ExchangeOAuthAccessToken();
        }

        [HttpPost]
        [Route("oauth/3rd/fitbit/login")]
        public void FitbitLogin() {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("oauth/3rd/nike/login")]
        public void NikeLogin() {
            throw new NotImplementedException();
        }
    }
}
