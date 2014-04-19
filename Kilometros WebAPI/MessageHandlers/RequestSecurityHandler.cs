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
            #if !DEBUG
            // --- Validar que la peitición venga de HTTPS
            if ( request.RequestUri.Scheme != Uri.UriSchemeHttps ) {
                HttpResponseMessage response
                    = new HttpResponseMessage(HttpStatusCode.Forbidden);

                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "000 " + MessageHandlerStrings.Warning000_HttpsRequired
                );

                return await MiscHelper.ReturnHttpResponseAndHalt(response);
            }
            #endif

            HttpRequestMessageHeadersHelper reqHeadersHelper
                = new HttpRequestMessageHeadersHelper(request);

            // --- Validar que no ésta URI no esté en lista de ByPass ---
            if (
                WebApiConfig.KmsOAuthConfig.BypassOAuthAbsoluteUris.Contains(
                    request.RequestUri.AbsolutePath.TrimEnd(new char[] { '/' })
                )
            ) {
                // Crear Principal genérico Anónimo y continuar ejecución
                GenericPrincipal anonPrincipal
                    = new GenericPrincipal(
                        new KMSIdentity(),
                        new string[] { "Anonymous" }
                    );

                MiscHelper.SetPrincipal(anonPrincipal);

                return await base.SendAsync(
                    request,
                    cancellationToken
                );
            }

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
                = false;
            httpOAuthValidRequest
                = await httpOAuth.ValidateRequestAsync(request);

            if ( httpOAuthValidRequest ) {
                // Actualizar LastUseDate de Token
                if( httpOAuth.Token != null )
                    Database.TokenStore.Update(httpOAuth.Token);
            } else {
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
            KMSIdentity identity
                = new KMSIdentity(httpOAuth);
            GenericPrincipal principal
                = new GenericPrincipal(identity, new string[] { "User" });

            Helpers.MiscHelper.SetPrincipal(principal);

            // --- Continuar con la ejecución ---
            return await base.SendAsync(
                request, 
                cancellationToken
            );
        }
    }
}