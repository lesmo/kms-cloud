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
    /// <summary>
    ///     Estos recursos están definidos escencialmente por el protocolo OAuth 1.0a, y permiten obtener
    ///     un Request Token, intercambiar un Request Token por un Access Token teniendo un Verifier Code
    ///     y cerrar una sesión en KMS eliminando el Token de la Sesión activa.
    /// </summary>
    public class OAuthController : OAuthBaseController {
        /// <summary>
        ///     Obtener un Request Token de OAuth para la Nube KMS.
        /// </summary>
        [AllowAnonymous]
        [HttpPost, Route("oauth/request_token")]
        public HttpResponseMessage OAuthRequestToken() {
            var identity = KmsIdentity.GetCurrentPrincipalIdentity();

            // --- Evitar doble Login ---
            if ( identity.IsAuthenticated )
                throw new HttpAlreadyLoggedInException(
                    "100" + ControllerStrings.Warning100_CannotLoginAgain
                );

            // --- Generar nuevo Token OAuth ---
            Token token = new Token {
                ApiKey = OAuth.ConsumerKey,
                Guid   = Guid.NewGuid(),
                Secret = Guid.NewGuid(),

                CallbackUri
                    = OAuth.CallbackUri == null
                    ? "oob"
                    : OAuth.CallbackUri.AbsoluteUri,

                ExpirationDate = DateTime.UtcNow.AddMinutes(10)
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

        /// <summary>
        ///     Obtener un Access Token para la Nube KMS, intercambiando un Verifier Code. Debe tenerse un Request Token
        ///     válido en la Cabecera HTTP Authorization.
        /// </summary>
        /// <param name="oAuthVerifier">
        ///     El Verifier Code obtenido por el proceso de Login Web.
        /// </param>
        /// <remarks>
        ///     Esta petición debe realizarse con un Request Token de KMS en las cabeceras HTTP.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost, Route("oauth/access_token")]
        public HttpResponseMessage OAuthAccessToken([FromBody]OAuthAccessTokenPost oAuthVerifier) {
            // --- Obtener información de OAuth y buscar Token ---
            Guid verifierCode;

            if ( OAuth.Token == null )
                throw new HttpBadRequestException(
                    "106" + ControllerStrings.Warning106_RequestTokenRequired
                );

            if ( string.IsNullOrEmpty(oAuthVerifier.oauth_verifier) && OAuth.VerifierCode.HasValue ) {
                verifierCode = OAuth.VerifierCode.Value;
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

        /// <summary>
        ///     Actualizar la fecha de expiración del Access Token de la Nube KMS. También puede
        ///     utilizarse éste recurso para determinar si un Token todavía es válido para interactuar
        ///     con la Nube KMS.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [HttpGet, Route("oauth/session")]
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

        /// <summary>
        ///     Eliminar el Token de Sesión del Usuario actual en la Nube KMS.
        /// </summary>
        /// <returns></returns>
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
