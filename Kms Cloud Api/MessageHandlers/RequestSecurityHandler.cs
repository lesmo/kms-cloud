using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

using Kms.Cloud.Api;
using Kms.Cloud.Database;
using System.Text;
using System.Security.Cryptography;
using Kms.Cloud.Api.Security;
using System.Security.Principal;
using Kilometros_WebGlobalization.API;
using System.IO;
using System.Collections.Specialized;
using Kms.Cloud.Database.Abstraction;

namespace Kms.Cloud.Api.MessageHandlers {
    public class RequestSecurityHandler : DelegatingHandler {
        private WorkUnit Database;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            Database = (WorkUnit)HttpContext.Current.Items["Database"];

            #if !DEBUG
            // --- Validar que la peitición venga de HTTPS
            if ( request.RequestUri.Scheme != Uri.UriSchemeHttps ) {
                HttpResponseMessage response
                    = new HttpResponseMessage(HttpStatusCode.Forbidden);

                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "000 " + MessageHandlerStrings.Warning000_HttpsRequired
                );

                return await response.NewHttpResponseTask();
            }
            #endif

            // --- Validar que no ésta URI no esté en lista de ByPass ---
            var comparableUri = request.RequestUri.AbsolutePath.TrimStart(
                new char[] {
                    '/'
                });
            if (
                WebApiConfig.KmsOAuthConfig.BypassOAuthAbsoluteUris.Any(a =>
                    a == comparableUri
                    || (
                        a.EndsWith("*") &&
                        comparableUri.StartsWith(a.Remove(a.Length - 1))
                    )
                )
            ) {
                // Crear Principal Anónimo y continuar ejecución
                new KmsPrincipal(new KmsIdentity()).SetAsCurrent();

                return await base.SendAsync(
                    request,
                    cancellationToken
                );
            }

            // --- Validar que se recibió cabecera Authorization correctamente ---
            if ( request.Headers.Authorization == null || request.Headers.Authorization.Scheme != "OAuth" ) {
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

                return await response.NewHttpResponseTask();
            }

            // --- Extraer información de OAuth de la cabecera Authorize ---
            var httpOAuth = HttpOAuthAuthorization.FromAuthenticationHeader(
                request.Headers.Authorization
            );

            var httpOAuthValidRequest = await httpOAuth.ValidateRequestAsync(request);
            if ( httpOAuthValidRequest ) {
                // Actualizar LastUseDate de Token
                if ( httpOAuth.Token != null && httpOAuth.Token.LastUseDate < DateTime.UtcNow.AddMinutes(-1) ) {
                    httpOAuth.Token.IPAddress = request.GetClientIpAddress();
                    Database.TokenStore.Update(httpOAuth.Token);
                }
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

                return await response.NewHttpResponseTask();
            }

            // --- Establecer contexto de seguridad ---
            var identity = new KmsIdentity(httpOAuth);
            new KmsPrincipal(identity).SetAsCurrent();
            
            // --- Continuar con la ejecución ---
            return await base.SendAsync(
                request, 
                cancellationToken
            );
        }
    }
}