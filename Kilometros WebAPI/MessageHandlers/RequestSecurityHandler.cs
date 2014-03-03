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
                    "OAuth realm=\"" + WebApiConfig.KmsOAuthConfig.Realm + "\""
                );

                return await MiscHelper.ReturnHttpResponseAndHalt(response);
            }

            // --- Extraer información de OAuth de la cabecera Authorize ---
            Dictionary<string, string> oAuthParameters
                = new Dictionary<string,string>();

            string[] oAuthParametersStrings
                = request.Headers.Authorization.Parameter.Split(new char[]{','});

            foreach ( string param in oAuthParametersStrings ) {
                string[] paramKeyValue
                    = param.Split(new char[]{'='}, 2);

                if ( paramKeyValue.Length != 2 )
                    continue;

                oAuthParameters.Add(
                    paramKeyValue[0],
                    paramKeyValue[1].Replace("\"","")
                );
            }

            // Validar que estén establecidos todos los campos obligatorios de OAuth
            foreach ( string param in WebApiConfig.KmsOAuthConfig.RequiredParams ) {
                if ( !oAuthParameters.ContainsKey(param) ) {
                    HttpResponseMessage response
                        = new HttpResponseMessage(HttpStatusCode.Unauthorized);

                    response.Headers.TryAddWithoutValidation(
                        "Warning",
                        "102 " + string.Format(
                            MessageHandlerStrings.Warning102_OAuthFieldMissing,
                            param
                        )
                    );

                    response.Headers.TryAddWithoutValidation(
                        "WWW-Authenticate",
                        "OAuth realm=\"" + WebApiConfig.KmsOAuthConfig.Realm + "\""
                    );

                    return await MiscHelper.ReturnHttpResponseAndHalt(response);
                }
            }

            // --- Validar Timestamp de OAuth ---
            // Obtener segundos desde Enero 1 de 1970 00:00:00
            string oAuthTimestampString
                = oAuthParameters["oauth_timestamp"];
            Int64 oAuthTimestampSeconds
                = 0;
            Int64.TryParse(
                oAuthTimestampString,
                out oAuthTimestampSeconds
            );
            
            // Crear objeto de fecha
            DateTime timestampBase
                = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            timestampBase
                = timestampBase.AddSeconds(oAuthTimestampSeconds);

            // Comparar tiempos (no debe estar más atrás ni delante de 3 minutos)
            TimeSpan timestampSpan
                = DateTime.UtcNow - timestampBase;
            if ( timestampSpan.Seconds > 180 || timestampSpan.Seconds < -180 ) {
                HttpResponseMessage response
                        = new HttpResponseMessage(HttpStatusCode.Unauthorized);

                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "105 " + MessageHandlerStrings.Warning105_OAuthTimestampInvalid
                );

                response.Headers.TryAddWithoutValidation(
                    "WWW-Authenticate",
                    "OAuth realm=\"" + WebApiConfig.KmsOAuthConfig.Realm + "\""
                );

                return await MiscHelper.ReturnHttpResponseAndHalt(response);
            }

            // --- Validar Nonce de OAuth ---
            OAuthNonce oAuthNonce
                = Database.OAuthNonceStore.GetFirst(
                    f => f.Nonce == oAuthParameters["oauth_nonce"]
                );

            if ( oAuthNonce == null ) {
                // Añadir Nonce a BD
                Database.OAuthNonceStore.Add(new OAuthNonce() {
                    Nonce
                        = oAuthParameters["oauth_nonce"]
                });

                Database.SaveChanges();
            } else {
                HttpResponseMessage response
                        = new HttpResponseMessage(HttpStatusCode.Unauthorized);

                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "100 " + MessageHandlerStrings.Warning100_OAuthNonceInvalid
                );

                response.Headers.TryAddWithoutValidation(
                    "WWW-Authenticate",
                    "OAuth realm=\"" + WebApiConfig.KmsOAuthConfig.Realm + "\""
                );

                return await MiscHelper.ReturnHttpResponseAndHalt(response);
            }

            // --- Validar que el API-Key exista en BD ---
            string apiKeyString
                = oAuthParameters["oauth_consumer_key"];
            ApiKey apiKey
                = this.GetApiKey(apiKeyString);

            if ( apiKey == null ) {
                HttpResponseMessage response
                    = new HttpResponseMessage(HttpStatusCode.Unauthorized);

                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "103 " + MessageHandlerStrings.Warning103_OAuthConsumerKeyInvalid
                );

                response.Headers.TryAddWithoutValidation(
                        "WWW-Authenticate",
                        "OAuth realm=\"" + WebApiConfig.KmsOAuthConfig.Realm + "\""
                    );

                return await MiscHelper.ReturnHttpResponseAndHalt(response);
            }

            // --- Validar firma de petición ---
            Stream contentStream
                = null;
            await request.Content.CopyToAsync(contentStream);

            // - Preparar contenedor para Base String -
            StringBuilder baseString
                = new StringBuilder();
            List<KeyValuePair<string, string>> contentItems
                = new List<KeyValuePair<string,string>>();
            foreach ( KeyValuePair<string, string> param in oAuthParameters ) {
                if ( param.Key == "oauth_signature" ) // esto afecta hash generado
                    continue;

                contentItems.Add(new KeyValuePair<string, string>(
                    param.Key,
                    param.Value
                ));
            }

            // - [1] Agregar método HTTP -
            baseString.Append(request.Method.ToString());
            baseString.Append('&');

            // - [2] Agregar URL de petición -
            baseString.Append(
                HttpUtility.UrlEncode(
                    request.RequestUri.ToString()
                )
            );
            baseString.Append('&');

            // - [3] Agregar parámetros ordenados a BaseString -
            // Ordenar parámetros en POST o GET (QueryString) según sea el caso
            if ( request.Method == HttpMethod.Post ) {
                // Obtener variables en cuerpo
                StreamReader contentStreamReader
                    = new StreamReader(contentStream);
                string contentString
                    = contentStreamReader.ReadToEnd();
                NameValueCollection contentNameValueCollection
                    = HttpUtility.ParseQueryString(contentString);
                        
                // Concatenar valores de cabecera Authorization de OAuth
                for ( int i = 0; i < contentNameValueCollection.Count; i++ ) {
                    contentItems.Add(new KeyValuePair<string,string>(
                        contentNameValueCollection.GetKey(i),
                        contentNameValueCollection.GetValues(i).FirstOrDefault()
                    ));
                }

            } else {
                foreach ( KeyValuePair<string, string> item in request.GetQueryNameValuePairs() )
                    contentItems.Add(item);
            }

            // Ordenar parámetros
            contentItems
                = (
                    from i in contentItems
                    orderby i.Key, i.Value
                    select i
                ).ToList<KeyValuePair<string,string>>();

            // Agregar parámetros a BaseString
            foreach ( KeyValuePair<string, string> item in contentItems ) {
                // Nombre de parámetro
                baseString.Append(
                    HttpUtility.UrlEncode(
                        item.Key
                    )
                );

                // = (símbolo igual)
                baseString.Append("%3D");

                // Valor de parámetro
                baseString.Append(
                    HttpUtility.UrlEncode(
                        item.Value
                    )
                );
            }

            // Generar llave de HMAC-SHA1
            Token token
                = null;
            string hmacSha1Key
                = apiKey.Secret.ToString("00000000000000000000000000000000");

            if ( oAuthParameters.ContainsKey("oauth_token") ) {
                token 
                    = Database.TokenStore.Get(
                        new Guid(oAuthParameters["oauth_token"])
                    );

                if ( token != null )
                    hmacSha1Key
                        += token.Secret.ToString("00000000000000000000000000000000");
            }

            // - [4] Calcular HMAC-SHA1 de BaseString -
            // Obtener HMAC-SHA1
            HMACSHA1 hmacSha1 = new HMACSHA1(
                Encoding.ASCII.GetBytes(hmacSha1Key)
            );
            byte[] hmacSha1Bytes
                = hmacSha1.ComputeHash(
                    Encoding.ASCII.GetBytes(baseString.ToString())
                );

            // Generar hash como String
            StringBuilder hmacSha1Hash
                = new StringBuilder();
            foreach ( byte b in hmacSha1Bytes )
                hmacSha1Hash.Append(b.ToString("X2"));

            // Comparar con Firma de Petición
            if ( hmacSha1Hash.ToString() != oAuthParameters["oauth_signature"] ) {
                HttpResponseMessage response
                    = new HttpResponseMessage(HttpStatusCode.Unauthorized);

                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "106 " + MessageHandlerStrings.Warning106_OAuthSignatureInvalid
                );

                response.Headers.TryAddWithoutValidation(
                        "WWW-Authenticate",
                        "OAuth realm=\"" + WebApiConfig.KmsOAuthConfig.Realm + "\""
                    );

                return response;
            }

            // --- Establecer contexto de seguridad ---
            User identityUser
                = token != null && token.User != null
                ? token.User
                : null;
            KmsIdentity identity
                = new KmsIdentity(identityUser, "KmsToken", apiKey);
            GenericPrincipal principal
                = new GenericPrincipal(identity, new string[] { "User" });

            Thread.CurrentPrincipal
                = principal;
            HttpContext.Current.User
                = principal;

            // --- Continuer con la ejecución
            return await base.SendAsync(
                request, 
                cancellationToken
            );
        }

        private ApiKey GetApiKey(string apiKeyString) {
            if ( apiKeyString.Length == 0 )
                return null;

            try {
                //byte[] apiKeyBytes
                //    = Convert.FromBase64String(apiKeyString);
                Guid apiKeyGuid
                    = new Guid(apiKeyString);
                ApiKey apiKey
                    = this.Database.ApiKeyStore.Get(apiKeyGuid);

                return apiKey;
            } catch {
                return null;
            }
        }

        private byte[] GetRequestSignature(string requestSignatureString) {
            if ( requestSignatureString.Length == 0 )
                return null;

            try {
                return Convert.FromBase64String(requestSignatureString);
            } catch {
                return null;
            }
        }

        private Token GetToken(string tokenString) {
            if ( tokenString.Length == 0 )
                return null;

            try {
                byte[] tokenBytes
                    = Convert.FromBase64String(tokenString);
                Guid tokenGuid
                    = new Guid(tokenBytes);
                Token token
                    = this.Database.TokenStore.Get(tokenGuid);

                return token;
            } catch {
                return null;
            }
        }

        private bool TokenIsExpired(Token token) {
            if ( DateTime.Now > token.ExpirationDate ) {
                // Eliminar el Token expirado
                this.Database.TokenStore.Delete(token.Guid);

                return true;
            } else {
                // Actualizar la fecha de último de uso
                token.LastUseDate = DateTime.Now;
                this.Database.TokenStore.Update(token);

                return false;
            }
        }
    }
}