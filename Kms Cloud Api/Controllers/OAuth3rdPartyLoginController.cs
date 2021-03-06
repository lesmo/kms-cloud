﻿using Kilometros_WebGlobalization.API;
using Kms.Cloud.Api.Exceptions;
using Kms.Cloud.Api.Models.RequestModels;
using Kms.Cloud.Database;
using Kms.Interop.OAuth;
using Kms.Interop.OAuth.SocialClients;
using System;
using System.Net.Http;
using System.Web.Http;

namespace Kms.Cloud.Api.Controllers {
    /// <summary>
    ///     Permite iniciar sesión en la Nube de KMS intercambiando un Request Token de KMS por un Access Token
    ///     utilizando el Access Token de alguna Red Social soportada.
    /// </summary>
    public class OAuth3rdPartyLoginController : OAuthBaseController {
        /// <summary>
        ///     Intercambiar un Request Token de OAuth para la Nube de KMS por un Access Token,
        ///     utilizando un Código de Facebook.
        /// </summary>
        /// <param name="dataPost">
        ///     Aparte de la cabecera propia de OAuth para KMS, en el cuerpo de POST deben incluirse el Código
        ///     de Facebook y el ID de Usuario en Faceboook.
        /// </param>
        /// <remarks>
        ///     Esta petición debe realizarse con un Request Token de KMS en las cabeceras HTTP.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost, Route("oauth/3rd/facebook/login")]
        public HttpResponseMessage FacebookLogin([FromBody]OAuthFacebookLoginPost dataPost) {
            ValidateOAuth3rdLoginRequest(OAuthService.Facebook, dataPost);
            var facebookClient = OAuth3rdClient<FacebookClient>(dataPost);

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

        /// <summary>
        ///     Intercambiar un Request Token de OAuth para la Nube de KMS por un Access Token,
        ///     utilizando un Access Token de Twitter.
        /// </summary>
        /// <param name="dataPost">
        ///     Aparte de la cabecera propia de OAuth para KMS, en el cuerpo de POST deben incluirse el Token,
        ///     Token Secret y ID del Usuario en Twitter.
        /// </param>
        /// <remarks>
        ///     Esta petición debe realizarse con un Request Token de KMS en las cabeceras HTTP.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost, Route("oauth/3rd/twitter/login")]
        public HttpResponseMessage TwitterLogin([FromBody]OAuth3rdLoginPost dataPost) {
            ValidateOAuth3rdLoginRequest(OAuthService.Twitter, dataPost);
            var twitterClient = OAuth3rdClient<TwitterClient>(dataPost);

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

        /// <summary>
        ///     Intercambiar un Request Token de OAuth para la Nube de KMS por un Access Token,
        ///     utilizando un Access Token de Fitbit. Actualmente
        ///     la funcionalidad no está implementada, y enviar peticiones causará
        ///     la muerte de un gatito.
        /// </summary>
        /// <param name="dataPost">
        ///     Aparte de la cabecera propia de OAuth para KMS, en el cuerpo de POST deben incluirse el Token,
        ///     Token Secret y ID del Usuario en Fitbit.
        /// </param>
        /// <remarks>
        ///     Esta petición debe realizarse con un Request Token de KMS en las cabeceras HTTP.
        /// </remarks>
        [HttpPost]
        [Route("oauth/3rd/fitbit/login")]
        public void FitbitLogin() {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Intercambiar un Request Token de OAuth para la Nube de KMS por un Access Token,
        ///     utilizando un Access Token de Nike+. Actualmente
        ///     la funcionalidad no está implementada, y enviar peticiones causará
        ///     la muerte de un gatito.
        /// </summary>
        /// <param name="dataPost">
        ///     Aparte de la cabecera propia de OAuth para KMS, en el cuerpo de POST deben incluirse el Token,
        ///     Token Secret y ID del Usuario en Nike+.
        /// </param>
        /// <remarks>
        ///     Esta petición debe realizarse con un Request Token de KMS en las cabeceras HTTP.
        /// </remarks>
        [HttpPost]
        [Route("oauth/3rd/nike/login")]
        public void NikeLogin() {
            throw new NotImplementedException();
        }
    }
}
