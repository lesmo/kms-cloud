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
            responseString.Append("oauth_token_expires=");
            responseString.Append(10 * 60);

            return Ok<StringContent>(
                new StringContent(responseString.ToString())
            );
        }

        [HttpPost]
        [Route("oauth/access_token")]
        public IHttpActionResult OAuthAccessToken() {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;

            
        }
    }
}
