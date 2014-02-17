﻿using System;
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

namespace Kilometros_WebAPI.MessageHandlers {
    public class RequestSecurityHandler : DelegatingHandler {
        private KilometrosDatabase.Abstraction.WorkUnit _database
            = new KilometrosDatabase.Abstraction.WorkUnit();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            HttpRequestMessageHeadersHelper reqHeadersHelper
                = new HttpRequestMessageHeadersHelper(request);

            /** Validar que se recibió Firma de Petición **/
            string requestSignatureString
                = reqHeadersHelper.GetHeaderValue(WebApiConfig.KmsHttpHeaders.RequestSignature);
            byte[] requestSignatureBytes
                = this.GetRequestSignature(requestSignatureString);

            if ( requestSignatureBytes == null ) {
                HttpResponseMessage response
                    = new HttpResponseMessage(HttpStatusCode.Forbidden);

                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "101 " + MessageHandlerStrings.Warning101_RequestSignatureInvalid
                );

                return MiscHelper.ReturnHttpResponseAndHalt(response);
            }

            /** Validar que el API-Key exista en BD **/
            string apiKeyString
                = reqHeadersHelper.GetHeaderValue(WebApiConfig.KmsHttpHeaders.ApiKey);
            ApiKey apiKey
                = this.GetApiKey(apiKeyString);

            if ( apiKey == null ) {
                HttpResponseMessage response
                    = new HttpResponseMessage(HttpStatusCode.Forbidden);
                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "102 " + MessageHandlerStrings.Warning102_ApiKeyInvalid
                );

                return MiscHelper.ReturnHttpResponseAndHalt(response);
            }


            /** Obtener Token **/
            string tokenString
                = reqHeadersHelper.GetHeaderValue(WebApiConfig.KmsHttpHeaders.Token);
            Token token
                = this.GetToken(tokenString);

            if ( token != null && this.TokenIsExpired(token) ) {
                HttpResponseMessage response
                    = new HttpResponseMessage(HttpStatusCode.Forbidden);
                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "401 " + MessageHandlerStrings.Warning103_SessionTokenInvalid
                );

                return MiscHelper.ReturnHttpResponseAndHalt(response);
            }

            /** Validar que el API-Request-Signature sea correcto **/
            SHA256 sha1 = new SHA256CryptoServiceProvider();

            // Obtener URL
            string urlString = request.RequestUri.PathAndQuery;
            byte[] urlBytes  = Encoding.ASCII.GetBytes(urlString);
                
            // Obtener Hash de API-Secret + Token + URL
            byte[] apiSecretBytes
                = apiKey.Secret.ToByteArray();
            byte[] signatureSourceBytes
                = apiSecretBytes.Concat(token.Guid.ToByteArray()).Concat(urlBytes).ToArray();
            byte[] expectedSignatureBytes
                = sha1.ComputeHash(signatureSourceBytes);

            // Comparar Firmas de Petición
            bool signatureIsCorrect
                = Helpers.MiscHelper.BytesEqual(
                    requestSignatureBytes,
                    expectedSignatureBytes
                );

            if ( ! signatureIsCorrect ) {
                HttpResponseMessage response
                    = new HttpResponseMessage(HttpStatusCode.Forbidden);
                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "101 " + MessageHandlerStrings.Warning101_RequestSignatureInvalid
                );

                return MiscHelper.ReturnHttpResponseAndHalt(response);
            }

            /** Establecer contexto de Seguridad **/
            User identityUser
                = token != null ? token.User : null;
            KmsIdentity identity
                = new KmsIdentity(identityUser, "KmsToken", apiKey);
            GenericPrincipal principal
                = new GenericPrincipal(identity, new string[] {"User"});
            
            Thread.CurrentPrincipal  = principal;
            HttpContext.Current.User = principal;

 	        return base.SendAsync(request, cancellationToken);
        }

        private ApiKey GetApiKey(string apiKeyString) {
            if ( apiKeyString.Length == 0 )
                return null;

            try {
                byte[] apiKeyBytes
                    = Convert.FromBase64String(apiKeyString);
                Guid apiKeyGuid
                    = new Guid(apiKeyBytes);
                ApiKey apiKey
                    = this._database.ApiKeyStore.Get(apiKeyGuid);

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
                    = this._database.TokenStore.Get(tokenGuid);

                return token;
            } catch {
                return null;
            }
        }

        private bool TokenIsExpired(Token token) {
            if ( DateTime.Now > token.ExpirationDate ) {
                // Eliminar el Token expirado
                this._database.TokenStore.Delete(token.Guid);

                return true;
            } else {
                // Actualizar la fecha de último de uso
                token.LastUseDate = DateTime.Now;
                this._database.TokenStore.Update(token);

                return false;
            }
        }
    }
}