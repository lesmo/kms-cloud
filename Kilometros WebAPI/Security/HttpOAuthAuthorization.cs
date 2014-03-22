using KilometrosDatabase;
using KilometrosDatabase.Abstraction;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Kilometros_WebAPI.Security {
    public class HttpOAuthAuthorization {
        public static HttpOAuthAuthorization FromAuthenticationHeader(
            AuthenticationHeaderValue authHeader,
            WorkUnit database
        ) {
            return new HttpOAuthAuthorization(authHeader.Parameter, database);
        }

        public HttpOAuthAuthorization(string oAuthParametersLine, WorkUnit database) {
            // --- Obtener parámetros de OAuth ---
            string[] oAuthParametersStrings
                = oAuthParametersLine.Split(new char[] { ',' });

            foreach ( string param in oAuthParametersStrings ) {
                string[] paramKeyValue
                    = param.Trim().Split(
                        new char[] { '=' },
                        2
                    );

                if ( paramKeyValue.Length != 2 )
                    continue;

                Match match
                    = Regex.Match(
                        paramKeyValue[1],
                        "\"([^\"]*)"
                    );

                if ( ! match.Success )
                    continue;

                oAuthParameters.Add(
                    paramKeyValue[0].Trim(),
                    match.Groups[1].Value
                );
            }

            // --- Capturar parámetros de OAuth ---
            if ( oAuthParameters.ContainsKey("oauth_consumer_key") ) {
                this.ConsumerKey
                    = database.ApiKeyStore.Get(
                    new Guid(oAuthParameters["oauth_consumer_key"])
                );
            } else {
                this.ConsumerKey
                    = null;
            }

            if ( oAuthParameters.ContainsKey("oauth_token") ) {
                this.Token
                    = database.TokenStore.Get(
                    new Guid(oAuthParameters["oauth_token"])
                );

                if (
                    this.Token.ExpirationDate.HasValue
                    && this.Token.ExpirationDate.Value < DateTime.UtcNow
                ) {
                    database.TokenStore.Delete(this.Token.Guid);
                    database.SaveChanges();

                    this.Token = null;
                }
            } else {
                this.Token
                    = null;
            }

            if ( 
                oAuthParameters.ContainsKey("oauth_signature")
                && oAuthParameters.ContainsKey("oauth_signature_method")
                && oAuthParameters["oauth_signature_method"] == "HMAC-SHA1"
            ) {
                this.Signature
                    = oAuthParameters["oauth_signature"];
            } else {
                this.Signature
                    = null;
            }

            if ( oAuthParameters.ContainsKey("oauth_timestamp") ) {
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
                if ( timestampSpan.Seconds < 181 && timestampSpan.Seconds > -179 )
                    this.Timestamp
                        = timestampBase;
                else
                    this.Timestamp
                        = null;
            } else {
                this.Timestamp
                    = null;
            }
            
            if ( oAuthParameters.ContainsKey("oauth_version") ) {
                if (
                    oAuthParameters["oauth_version"] == "1.0"
                    || oAuthParameters["oauth_version"] == "1.0a"
                ) {
                    this.Version
                        = new Version(1, 0);
                } else {
                    this.Version
                        = null;
                }
            } else {
                this.Version
                    = null;
            }

            if ( oAuthParameters.ContainsKey("oauth_nonce") ) {
                OAuthNonce nonce
                    = database.OAuthNonceStore.GetFirst(
                        f => f.Nonce == oAuthParameters["oauth_nonce"]
                    );

                if ( nonce == null ) {
                    this.Nonce
                        = new OAuthNonce() {
                            Nonce
                                = oAuthParameters["oauth_nonce"]
                        };
                    
                    database.OAuthNonceStore.Add(nonce);
                    database.SaveChanges();
                } else {
                    this.Nonce
                        = null;
                }
            } else {
                this.Nonce
                    = null;
            }

            if ( oAuthParameters.ContainsKey("oauth_callback") ) {
                try {
                    this.CallbackUri
                        = new Uri(oAuthParameters["oauth_callback"]);
                } catch {
                    this.CallbackUri
                        = null;
                }
            } else {
                this.CallbackUri
                    = null;
            }

            if ( oAuthParameters.ContainsKey("oauth_verifier") ) {
                try {
                    this.VerifierCode
                        = new Guid(oAuthParameters["oauth_verifier"]);
                } catch {
                    this.VerifierCode
                        = null;
                }
            } else {
                this.VerifierCode
                    = null;
            }
        }

        private Dictionary<string, string> oAuthParameters
            = new Dictionary<string, string>();

        /// <summary>
        /// {oauth_consumer_key}
        /// </summary>
        public readonly ApiKey ConsumerKey;
        /// <summary>
        /// {oauth_token}
        /// </summary>
        public readonly Token Token;
        /// <summary>
        /// {oauth_signature}
        /// </summary>
        public readonly string Signature;
        /// <summary>
        /// {oauth_timestamp}
        /// </summary>
        public readonly DateTime? Timestamp;
        /// <summary>
        /// {oauth_version}
        /// </summary>
        public readonly Version Version;
        /// <summary>
        /// {oauth_nonce}
        /// </summary>
        public readonly OAuthNonce Nonce;
        
        /// <summary>
        /// {oauth_callback}
        /// </summary>
        public readonly Uri CallbackUri;
        /// <summary>
        /// {oauth_verifier}
        /// </summary>
        public readonly Guid? VerifierCode;

        public bool IsValid {
            get {
                return this._isValid && this.IsInternalyValid;
            }
        }
        private bool IsInternalyValid {
            get {
                if ( this.ConsumerKey == null )
                    return false;
                if ( this.Nonce == null )
                    return false;
                if ( this.Signature == null )
                    return false;
                if ( this.Timestamp.HasValue == false )
                    return false;
                return true;
            }
        }
        private bool _isValid = false;

        public async Task<bool> ValidateRequestAsync(HttpRequestMessage request) {
            // --- Validar requisitos mínimos para validar ---
            if ( ! this.IsInternalyValid )
                return false;
            if ( this.IsValid )
                return true;

            // --- Validar firma de petición ---
            Stream contentStream
                = null;
            await request.Content.CopyToAsync(contentStream);

            // - Preparar contenedor para Base String -
            StringBuilder baseString
                = new StringBuilder();
            List<KeyValuePair<string, string>> contentItems
                = new List<KeyValuePair<string, string>>();
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
            if ( request.Method != HttpMethod.Get ) {
                // Obtener variables en cuerpo
                StreamReader contentStreamReader
                    = new StreamReader(contentStream);
                string contentString
                    = contentStreamReader.ReadToEnd();
                NameValueCollection contentNameValueCollection
                    = HttpUtility.ParseQueryString(contentString);

                // Concatenar valores de cabecera Authorization de OAuth
                for ( int i = 0; i < contentNameValueCollection.Count; i++ ) {
                    contentItems.Add(new KeyValuePair<string, string>(
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
                ).ToList<KeyValuePair<string, string>>();

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
            string hmacSha1Key
                = this.ConsumerKey.Secret.ToString("00000000000000000000000000000000") + "&";
            
            if ( this.Token != null )
                hmacSha1Key
                    += this.Token.Secret.ToString("00000000000000000000000000000000");

            // - [4] Calcular HMAC-SHA1 de BaseString -
            // Obtener HMAC-SHA1
            HMACSHA1 hmacSha1 = new HMACSHA1(
                Encoding.ASCII.GetBytes(hmacSha1Key)
            );
            byte[] hmacSha1Bytes
                = hmacSha1.ComputeHash(
                    Encoding.ASCII.GetBytes(baseString.ToString())
                );

            // Generar hash como Base64
            string hmacSha1Base64
                = Convert.ToBase64String(hmacSha1Bytes);

            // Comparar con hash recibido
            if ( this.Signature == hmacSha1Base64 ) {
                this._isValid = true;
            } else {
                this._isValid = false;
            }

            return this.IsValid;
        }
    }
}