using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

using Kilometros_WebAPI;
using KilometrosDatabase;
using System.Text;
using System.Security.Cryptography;
using Kilometros_WebAPI.Security;
using System.Security.Principal;
using Kilometros_WebGlobalization.API;
using Kilometros_WebAPI.Helpers;
using System.IO;
using System.Collections.Specialized;

namespace Kilometros_WebAPI.MessageHandlers {
    public class RequestSecurityHandler : DelegatingHandler {
        private KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            HttpRequestMessageHeadersHelper reqHeadersHelper
                = new HttpRequestMessageHeadersHelper(request);

            // --- Validar que se recibió cabecera Authorization correctamente ---
            if ( request.Headers.Authorization.Scheme != "OAuth" ) {
                HttpResponseMessage response
                    = new HttpResponseMessage(HttpStatusCode.Unauthorized);

                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "101 " + MessageHandlerStrings.Warning101_AuthorizationNotFound
                );

                response.Headers.TryAddWithoutValidation(
                    "WWW-Authenticate",
                    "OAuth realm=\"" + WebApiConfig.KmsOAuthConfig.ApiRealm + "\""
                );

                return await MiscHelper.ReturnHttpResponseAndHalt(response);
            }

            // --- Extraer información de OAuth de la cabecera Authorize ---
            HttpOAuthAuthorization httpOAuth
                = HttpOAuthAuthorization.FromAuthenticationHeader(
                    request.Headers.Authorization,
                    Database
                );
            bool httpOAuthValidRequest
                = await httpOAuth.ValidateRequestAsync(request);

            if ( !httpOAuthValidRequest ) {
                HttpResponseMessage response
                    = new HttpResponseMessage(HttpStatusCode.Unauthorized);

                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "100 " + MessageHandlerStrings.Warning100_OAuthAuthorizationInvalid
                );

                response.Headers.TryAddWithoutValidation(
                    "WWW-Authenticate",
                    "OAuth realm=\"" + WebApiConfig.KmsOAuthConfig.ApiRealm + "\""
                );

                return await MiscHelper.ReturnHttpResponseAndHalt(response);
            }

            // --- Establecer contexto de seguridad ---
            KmsIdentity identity
                = new KmsIdentity(httpOAuth);
            GenericPrincipal principal
                = new GenericPrincipal(identity, new string[] { "User" });

            Helpers.MiscHelper.SetPrincipal(principal);

            // --- Continuar con la ejecución
            return await base.SendAsync(
                request, 
                cancellationToken
            );
        }
    }
}