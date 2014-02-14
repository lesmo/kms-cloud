using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Kilometros_WebAPI.Security;
using KilometrosDatabase;
using System.Web;
using Kilometros_WebAPI.Models.HttpPost.SessionController;
using Kilometros_WebGlobalization.API;
using Kilometros_WebAPI.Models.HttpGet.SessionController;
using System.Globalization;

namespace Kilometros_WebAPI.Controllers {
    public class SessionController : ApiController {
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();
        private HttpServerUtility _httpServerUtility
            = new HttpServerUtility();

        [HttpPost]
        [Route("session/kms")]
        public HttpResponseMessage KmsLogin([FromBody]LoginPost userPost) {
            /** Evitar doble login **/
            if ( this.User.Identity.IsAuthenticated ) {
                HttpResponseMessage response
                    = Request.CreateResponse(HttpStatusCode.Forbidden);
                string warningString
                    = string.Format(ControllerStrings.Warning100_CannotLoginAgain, this.User.Identity.Name);

                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "100" + this._httpServerUtility.UrlEncode(warningString)
                );

                return response;
            }

            /** Buscar al Usuario y validar contraseña **/
            User user = this.Database.UserStore.GetAll(
                u => u.Email == userPost.Email
            ).FirstOrDefault();

            if ( user == null || userPost.AccessHash != user.PasswordString ) {
                HttpResponseMessage response
                    = Request.CreateResponse(HttpStatusCode.Unauthorized);
                string warningString
                    = ControllerStrings.Warning101_UserNotFound;

                response.Headers.Add(
                    "Warning",
                    "101 " + this._httpServerUtility.UrlEncode(warningString)
                );

                return response;
            }

            /** Generar nuevo Token **/
            KmsIdentity identity = (KmsIdentity)this.User.Identity;
            Token token = new Token() {
                ApiKey = identity.ApiKey,
                User = user,
                Guid = Guid.NewGuid(),

                CreationDate = DateTime.Now,
                LastUseDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddDays(30)
            };

            this.Database.TokenStore.Add(token);
            this.Database.SaveChanges();

            /** Preparar y enviar respuesta **/
            TokenResponse tokenResponse = new TokenResponse() {
                Expires = DateTime.Now.AddDays(30),
                Token = Convert.ToBase64String(token.Guid.ToByteArray()),
                Pending = {}
            };

            return Request.CreateResponse<TokenResponse>(
                HttpStatusCode.Created, 
                tokenResponse
            );
        }

        [HttpPost]
        [Route("session/facebook")]
        public void FacebookLogin() {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("session/twitter")]
        public void TwitterLogin() {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("session/{tokenString}")]
        public HttpResponseMessage GetToken(string tokenString) {
            HttpResponseMessage response
                = Request.CreateResponse();

            byte[] tokenBytes
                = Convert.FromBase64String(tokenString);
            Guid tokenGuid
                = new Guid(tokenBytes);
            Token token
                = this.Database.TokenStore.Get(tokenGuid);

            if ( token == null ) {
                string warningString
                    = string.Format(ControllerStrings.Warning102_TokenNotFound, tokenString);

                response.StatusCode = HttpStatusCode.NotFound;
                response.Headers.Add(
                    "Warning",
                    "102 " + this._httpServerUtility.UrlEncode(warningString)
                );
            } else {
                if ( token.ExpirationDate.HasValue ) {
                    token.ExpirationDate
                        = token.ExpirationDate.Value.AddDays(90);

                    this.Database.TokenStore.Update(token);
                    this.Database.SaveChanges();

                    response.Headers.TryAddWithoutValidation(
                        "Expires",
                        token.ExpirationDate.Value.ToString(
                            DateTimeFormatInfo.InvariantInfo.RFC1123Pattern
                        )
                    );
                }

                response.StatusCode = HttpStatusCode.OK;
            }

            return response;
        }

        [HttpDelete]
        [Route("session/{tokenString}")]
        public HttpResponseMessage DeleteToken(string tokenString) {
            HttpResponseMessage response = new HttpResponseMessage();

            byte[] tokenBytes
                = Convert.FromBase64String(tokenString);
            Guid tokenGuid  
                = new Guid(tokenBytes);
            Token token
                = this.Database.TokenStore.Get(tokenGuid);

            if ( token == null ) {
                string warningString
                    = string.Format(ControllerStrings.Warning102_TokenNotFound, tokenString);

                response.StatusCode = HttpStatusCode.NotFound;
                response.Headers.Add(
                    "Warning",
                    "102 " + this._httpServerUtility.UrlEncode(warningString)
                );
            } else {
                this.Database.TokenStore.Delete(tokenGuid);
                response.StatusCode = HttpStatusCode.NoContent;
            }

            return response;
        }
    }
}
