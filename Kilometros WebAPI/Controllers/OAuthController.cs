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
using Kilometros_WebAPI.Models.RequestModels;

namespace Kilometros_WebAPI.Controllers {
    public class OAuthController : IKMSController {
        [HttpPost]
        [Route("oauth/request_token")]
        public HttpResponseMessage OAuthRequestToken() {
            // --- Evitar doble Login ---
            if ( Identity.IsAuthenticated )
                throw new HttpAlreadyLoggedInException(
                    ControllerStrings.Warning100_CannotLoginAgain
                );

            // --- Generar nuevo Token OAuth ---
            Token token
                = new Token() {
                    ApiKey
                        = Database.ApiKeyStore[Identity.ApiKey.Guid],
                    Guid
                        = Guid.NewGuid(),
                    Secret
                        = Guid.NewGuid(),

                    CallbackUri
                        = OAuth.CallbackUri == null
                        ? "oob"
                        : OAuth.CallbackUri.AbsoluteUri,

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
            return new HttpResponseMessage() {
                RequestMessage
                    = Request,

                StatusCode
                    = HttpStatusCode.OK,
                Content
                    = new StringContent(
                        string.Format(
                            "oauth_token={0}"
                            + "&oauth_token_secret={1}"
                            + "&oauth_callback_confirmed={2}"
                            + "&x_token_expires={3}",

                            token.Guid.ToString("N"),
                            token.Secret.ToString("N"),
                            Identity.OAuth.CallbackUri == null
                                ? "false"
                                : "true",
                            10 * 60
                        )
                    )
            };
        }
        
        [HttpPost]
        [Route("oauth/access_token")]
        public HttpResponseMessage OAuthAccessToken([FromBody]OAuthAccessTokenPost oAuthVerifier) {
            // --- Obtener información de OAuth y buscar Token ---
            Guid verifierCode;

            if ( string.IsNullOrEmpty(oAuthVerifier.oauth_verifier) && OAuth.VerifierCode.HasValue ) {
                verifierCode
                    = OAuth.VerifierCode.Value;
            } else if ( !Guid.TryParse(oAuthVerifier.oauth_verifier, out verifierCode) ) {
                throw new HttpUnauthorizedException(
                    "120 " + ControllerStrings.Warning104_RequestTokenInvalid
                );
            }

            if ( OAuth.Token.VerificationCode != verifierCode ) {
                throw new HttpUnauthorizedException(
                    "120 " + ControllerStrings.Warning104_RequestTokenInvalid
                );
            }

            // --- Generar nuevo Token con capacidad de Acceso ---
            Token newToken
                = new Token() {
                    ApiKey
                        = OAuth.ConsumerKey,
                    Guid
                        = Guid.NewGuid(),
                    Secret
                        = Guid.NewGuid(),
                    VerificationCode
                        = null,

                    User
                        = OAuth.Token.User,

                    CreationDate
                        = DateTime.UtcNow,
                    ExpirationDate
                        = DateTime.UtcNow.AddMonths(3),
                    LastUseDate
                        = DateTime.UtcNow,
                    LoginAttempts
                        = OAuth.Token.LoginAttempts,
                };

            Database.TokenStore.Add(newToken);
            Database.TokenStore.Delete(OAuth.Token.Guid);
            Database.SaveChanges();

            // --- Preparar y devolver respuesta ---
            return new HttpResponseMessage() {
                RequestMessage
                    = Request,

                StatusCode
                    = HttpStatusCode.OK,
                Content
                    = new StringContent(
                        string.Format(
                            "oauth_token={0}&oauth_token_secret={1}",
                            newToken.Guid.ToString("N"),
                            newToken.Secret.ToString("N")
                        )
                    )
            };
        }

        [Authorize]
        [HttpGet]
        [Route("oauth/session")]
        public HttpResponseMessage GetToken() {
            Token token
                = OAuth.Token;

            if ( token.ExpirationDate.HasValue ) {
                token.ExpirationDate
                    = token.ExpirationDate.Value.AddDays(90);

                Database.TokenStore.Update(token);
                Database.SaveChanges();

                return new HttpResponseMessage(HttpStatusCode.OK);
            } else {
                // TODO: Tal vez notificar que no se hizo nada?
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("oauth/session")]
        public HttpResponseMessage DeleteToken() {
            KMSIdentity identity
                = (KMSIdentity)User.Identity;
            Token token
                = identity.Token;

            Database.TokenStore.Delete(token.Guid);
            Database.SaveChanges();

            return new HttpResponseMessage(
                HttpStatusCode.NoContent
            );
        }
    }
}
