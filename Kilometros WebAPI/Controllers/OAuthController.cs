using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using KilometrosDatabase;
using Kilometros_WebAPI.Security;
using Kilometros_WebAPI.Exceptions;
using Kilometros_WebGlobalization.API;

namespace Kilometros_WebAPI.Controllers {
    public class OAuthController : ApiController {
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();

        [HttpPost]
        [Route("oauth/request_token")]
        public IHttpActionResult OAuthRequestToken() {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;

            // --- Evitar doble Login ---
            if ( identity.IsAuthenticated )
                throw new HttpAlreadyLoggedInException(
                    ControllerStrings.Warning100_CannotLoginAgain
                );

            // --- Generar nuevo Token OAuth ---
            Token token
                = new Token() {
                    ApiKey
                        = identity.ApiKey,
                    Guid
                        = Guid.NewGuid(),
                    Secret
                        = Guid.NewGuid(),

                    CallbackUri
                        = identity.OAuth.CallbackUri.AbsoluteUri,

                    CreationDate
                        = DateTime.UtcNow,
                    LastUseDate
                        = DateTime.UtcNow,
                    ExpirationDate
                        = DateTime.UtcNow.AddMinutes(10)
                };

            Database.TokenStore.Add(token);
            Database.SaveChanges();

            // --- Preparar y devolver detalles de Token OAuth ---
            StringBuilder responseString
                = new StringBuilder();

            responseString.Append("oauth_token=");
            responseString.Append(
                token.Guid.ToString("00000000000000000000000000000000")
            );
            responseString.Append('&');

            responseString.Append("oauth_token_secret=");
            responseString.Append(
                token.Secret.ToString("00000000000000000000000000000000")
            );
            responseString.Append('&');

            responseString.Append("oauth_callback_confirmed=");
            responseString.Append(
                identity.OAuth.CallbackUri == null
                    ? "false"
                    : "true"
            );
            responseString.Append('&');

            responseString.Append("oauth_token_expires=");
            responseString.Append(10 * 60);

            return Ok<StringContent>(
                new StringContent(responseString.ToString())
            );
        }

        [Authorize]
        [HttpPost]
        [Route("oauth/access_token")]
        public IHttpActionResult OAuthAccessToken() {
            // --- Obtener información de OAuth y buscar Token ---
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            HttpOAuthAuthorization oAuth
                = identity.OAuth;
            Token token
                = identity.Token;

            if ( token.VerificationCode != oAuth.VerifierCode ) {
                throw new HttpUnauthorizedException(
                    "120" + ControllerStrings.Warning104_RequestTokenInvalid
                );
            }

            // --- Generar nuevo Token con capacidad de Acceso ---
            Token newToken
                = new Token() {
                    ApiKey
                        = oAuth.ConsumerKey,
                    Guid
                        = Guid.NewGuid(),
                    Secret
                        = Guid.NewGuid(),
                    VerificationCode
                        = token.VerificationCode,

                    User
                        = token.User,

                    CreationDate
                        = DateTime.UtcNow,
                    ExpirationDate
                        = DateTime.UtcNow.AddMonths(3),
                    LastUseDate
                        = DateTime.UtcNow,
                    LoginAttempts
                        = token.LoginAttempts,
                };

            Database.TokenStore.Add(newToken);
            Database.TokenStore.Delete(token.Guid);
            Database.SaveChanges();

            // --- Preparar y devolver respuesta ---
            StringBuilder responseString
                = new StringBuilder();

            responseString.Append("oauth_token=");
            responseString.Append(
                token.Guid.ToString("00000000000000000000000000000000")
            );
            responseString.Append('&');

            responseString.Append("oauth_token_secret=");
            responseString.Append(
                token.Secret.ToString("00000000000000000000000000000000")
            );

            return Ok<StringContent>(
                new StringContent(responseString.ToString())
            );
        }

        [Authorize]
        [HttpGet]
        [Route("oauth/session")]
        public IHttpActionResult GetToken() {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            Token token
                = identity.Token;

            if ( token.ExpirationDate.HasValue ) {
                token.ExpirationDate
                    = token.ExpirationDate.Value.AddDays(90);

                this.Database.TokenStore.Update(token);
                this.Database.SaveChanges();

                return Ok();
            } else {
                // TODO: Tal vez notificar que no se hizo nada?
                return Ok();
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("oauth/session")]
        public IHttpActionResult DeleteToken() {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            Token token
                = identity.Token;

            Database.TokenStore.Delete(token.Guid);

            throw new HttpNoContentException(
                ControllerStrings.Warning103_TokenDeleteOk
            );
        }
    }
}
