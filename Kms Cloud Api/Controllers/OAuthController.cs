using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Kms.Cloud.Database;
using Kms.Cloud.Api.Security;
using Kms.Cloud.Api.Exceptions;
using Kilometros_WebGlobalization.API;
using Kms.Cloud.Api.Models.RequestModels;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace Kms.Cloud.Api.Controllers {
    public class OAuthController : OAuthBaseController {
        [HttpPost]
        [Route("oauth/request_token")]
        public HttpResponseMessage OAuthRequestToken() {
            var identity = KmsIdentity.GetCurrentPrincipalIdentity();

            // --- Evitar doble Login ---
            if ( identity.IsAuthenticated )
                throw new HttpAlreadyLoggedInException(
                    ControllerStrings.Warning100_CannotLoginAgain
                );

            // --- Generar nuevo Token OAuth ---
            Token token
                = new Token {
                    ApiKey
                        = OAuth.ConsumerKey,
                    Guid
                        = Guid.NewGuid(),
                    Secret
                        = Guid.NewGuid(),

                    CallbackUri
                        = OAuth.CallbackUri == null
                        ? "oob"
                        : OAuth.CallbackUri.AbsoluteUri,

                    ExpirationDate
                        = DateTime.UtcNow.AddMinutes(10)
                };

            Database.TokenStore.Add(token);
            Database.SaveChanges();

            // --- Preparar y devolver detalles de Token OAuth ---
            return new HttpResponseMessage {
                RequestMessage
                    = Request,

                StatusCode
                    = HttpStatusCode.OK,
                Content
                    = new StringContent(
                        string.Format(
                            CultureInfo.InvariantCulture,

                            "oauth_token={0}"
                            + "&oauth_token_secret={1}"
                            + "&oauth_callback_confirmed={2}"
                            + "&x_token_expires={3}",

                            token.Guid.ToString("N"),
                            token.Secret.ToString("N"),
                            identity.OAuth.CallbackUri == null
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

            // --- Generar nuevo Access Token ---
            return ExchangeOAuthAccessToken();
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [Authorize, HttpGet, Route("oauth/session")]
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
            var identity = KmsIdentity.GetCurrentPrincipalIdentity();
            var token    = identity.OAuth.Token;

            Database.TokenStore.Delete(token.Guid);
            Database.SaveChanges();

            return new HttpResponseMessage(
                HttpStatusCode.NoContent
            );
        }
    }
}
