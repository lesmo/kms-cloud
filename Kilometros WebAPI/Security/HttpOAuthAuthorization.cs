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
        
        /// <summary>
        ///     Crea una nueva instancia de Autorización HTTP OAuth 1.0a a partir de otra instancia similar,
        ///     pero modificando el Contexto de Base de Datos al que están asociados los objetos Consumer
        ///     y Token por el que se especifique.
        /// </summary>
        /// <param name="oAuthObject">
        ///     Instancia a partir de la cual generar la nueva instancia.
        /// </param>
        /// <param name="database">
        ///     Contexto de Base de Datos a utilizar para asociar el Consumer y Token.
        /// </param>
        public HttpOAuthAuthorization(HttpOAuthAuthorization oAuthObject, WorkUnit database) {
            this.CallbackUri
                = oAuthObject.CallbackUri;
            this.ConsumerKey
                = database.ApiKeyStore.Get(oAuthObject.ConsumerKey.Guid);
            this.Nonce
                = oAuthObject.Nonce;
            this.oAuthParameters
                = oAuthObject.oAuthParameters;
            this.Signature
                = oAuthObject.Signature;
            this.Timestamp
                = oAuthObject.Timestamp;
            if ( oAuthObject.Token != null )
                this.Token
                    = database.TokenStore.Get(oAuthObject.Token.Guid);
            this.VerifierCode
                = oAuthObject.VerifierCode;
            this.Version
                = oAuthObject.Version;
        }

        /// <summary>
        ///     Crea una nueva instancia de Autorización HTTP OAuth 1.0a a partir de los parámetros
        ///     de la cabecera HTTP Authorization.
        /// </summary>
        /// <param name="oAuthParametersLine">
        ///     Cadena después de "OAuth " de la cabecera HTTP Authorization.
        /// </param>
        /// <param name="database">
        ///     Contexto de Base de Datos a utilizar para identificar el Consumer y Token.
        /// </param>
        public HttpOAuthAuthorization(string oAuthParametersLine, WorkUnit database) {
            // --- Obtener parámetros de OAuth ---
            string[] oAuthParametersStrings
                = oAuthParametersLine.Trim().Split(new char[] { ',' });

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
                    this.Token != null
                    && this.Token.ExpirationDate.HasValue
                    && this.Token.ExpirationDate.Value < DateTime.UtcNow
                ) {
                    database.TokenStore.Delete(this.Token.Guid);
                    database.SaveChanges();

                    this.Token
                        = null;
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
                    = HttpUtility.UrlDecode(
                        oAuthParameters["oauth_signature"]
                    );
            } else {
                this.Signature
                    = null;
            }

            if ( oAuthParameters.ContainsKey("oauth_timestamp") ) {
                string oAuthTimestampString
                    = oAuthParameters["oauth_timestamp"];
                long oAuthTimestampSeconds
                    = long.Parse(
                        oAuthTimestampString
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
                string oAuthNonce
                    = oAuthParameters["oauth_nonce"];

                OAuthNonce nonce
                    = database.OAuthNonceStore.GetFirst(
                        f => f.Nonce == oAuthNonce
                    );

                if ( nonce == null ) {
                    this.Nonce
                        = new OAuthNonce() {
                            Nonce
                                = oAuthParameters["oauth_nonce"]
                        };
                        
                    database.OAuthNonceStore.Add(this.Nonce);
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

        /// <summary>
        /// Devuelve si ésta petición se realizó con un Request Token.
        /// </summary>
        public bool IsRequestToken {
            get {
                return this.IsValid && this.Token != null && this.Token.User == null;
            }
        }

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
                = await request.Content.ReadAsStreamAsync();
            //request.Content.CopyToAsync(contentStream).Wait();

            // - Preparar contenedor para Base String -
            StringBuilder baseString
                = new StringBuilder();
            NameValueCollection contentItems
                = new NameValueCollection();
            foreach ( KeyValuePair<string, string> param in this.oAuthParameters ) {
                if ( param.Key == "oauth_signature" ) // esto afecta hash generado
                    continue;

                contentItems.Add(
                    param.Key,
                    param.Value
                );
            }

            // - [1] Agregar método HTTP -
            baseString.Append(request.Method.ToString());
            baseString.Append('&');

            // - [2] Agregar URL de petición -
            baseString.Append(
                Uri.EscapeDataString(
                    request.RequestUri.ToString()
                )
            );
            baseString.Append('&');

            // - [3] Agregar parámetros BaseString -
            // Ordenar parámetros en POST o GET (QueryString) según sea el caso
            StringBuilder contentString
                = new StringBuilder();

            if ( request.GetQueryNameValuePairs().Count() > 0 ) {
                foreach ( KeyValuePair<string, string> item in request.GetQueryNameValuePairs().OrderBy(b => b.Key) )
                    contentItems.Add(
                        item.Key,
                        item.Value
                    );
            }

            if ( request.Content.IsFormData() )  {
                NameValueCollection formData
                    = await request.Content.ReadAsFormDataAsync();
                List<KeyValuePair<string, string>> formDataList
                    = new List<KeyValuePair<string, string>>();

                foreach ( string key in formData.AllKeys ) {
                    string[] values
                        = formData.GetValues(key);

                    foreach ( string value in values ) {
                        contentItems.Add(
                            key,
                            value
                        );

                        formDataList.Add(
                            new KeyValuePair<string, string>(
                                key,
                                value
                            )
                        );
                    }
                }

                request.Content
                    = new FormUrlEncodedContent(
                        formDataList
                    );
            }

            foreach ( string key in contentItems.AllKeys.OrderBy(b => b).ToArray() ) {
                string[] values
                    = contentItems.GetValues(key);

                foreach ( string value in values.OrderBy(b => b).ToArray() ) {
                    contentString.Append(
                        string.Format(
                            "{0}={1}&",
                            Uri.EscapeDataString(key),
                            Uri.EscapeDataString(value)
                        )
                    );
                }
            }

            baseString.Append(
                Uri.EscapeDataString(
                    contentString.ToString().Remove(
                        contentString.Length - 1
                    )
                )
            );

            // Generar llave de HMAC-SHA1
            string hmacSha1Key
                = this.ConsumerKey.Secret.ToString("N") + "&";
            
            if ( this.Token != null )
                hmacSha1Key
                    += this.Token.Secret.ToString("N");

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