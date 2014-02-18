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
using Kilometros_WebAPI.Exceptions;
using Kilometros_WebAPI.Helpers;

namespace Kilometros_WebAPI.Controllers {
    public class SessionController : ApiController {
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();

        [HttpPost]
        [Route("session/kms")]
        public TokenResponse KmsLogin([FromBody]LoginPost dataPost) {
            // --- Evitar doble Login ---
            if ( this.User.Identity.IsAuthenticated )
                throw new HttpAlreadyLoggedInException(
                    ControllerStrings.Warning100_CannotLoginAgain
                );
            
            // --- Obtener bytes de contraseña, buscar al Usuario y validar contraseña ---
            byte[] passwordBytes
                = Convert.FromBase64String(dataPost.AccessHash);
            User user
                = this.Database.UserStore.GetAll(
                    u => u.Email == dataPost.Email
                ).FirstOrDefault();

            if ( user == null || ! MiscHelper.BytesEqual(passwordBytes, user.Password) )
                throw new HttpUnauthorizedException(
                    string.Format(
                        ControllerStrings.Warning101_UserNotFound,
                        dataPost.Email
                    )
                );

            // --- Generar nuevo Token ---
            KmsIdentity identity
                = (KmsIdentity)this.User.Identity;
            Token token
                = new Token() {
                    ApiKey
                        = identity.ApiKey,
                    User
                        = user,
                    Guid
                        = Guid.NewGuid(),

                    CreationDate
                        = DateTime.UtcNow,
                    LastUseDate
                        = DateTime.UtcNow,
                    ExpirationDate
                        = DateTime.UtcNow.AddDays(30)
                };

            this.Database.TokenStore.Add(token);
            this.Database.SaveChanges();

            // --- Preparar y enviar respuesta ---
            return new TokenResponse() {
                Expires
                    = token.ExpirationDate.Value,
                Token
                    = Convert.ToBase64String(token.Guid.ToByteArray()),
                Pending
                    = {}
            };
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
        public IHttpActionResult GetToken(string tokenString) {
            HttpResponseMessage response
                = Request.CreateResponse();

            byte[] tokenBytes
                = Convert.FromBase64String(tokenString);
            Guid tokenGuid
                = new Guid(tokenBytes);
            Token token
                = this.Database.TokenStore.Get(tokenGuid);

            if ( token == null ) {
                return NotFound();
            } else if ( token.ExpirationDate.HasValue ) {
                token.ExpirationDate
                    = token.ExpirationDate.Value.AddDays(90);

                this.Database.TokenStore.Update(token);
                this.Database.SaveChanges();

                return Ok();
            } else {
                // TODO: Tal vez notificar que no se hizo nada?
                return Ok();
            }
        }

        [HttpDelete]
        [Route("session/{tokenString}")]
        public IHttpActionResult DeleteToken(string tokenString) {
            byte[] tokenBytes
                = Convert.FromBase64String(tokenString);
            Guid tokenGuid  
                = new Guid(tokenBytes);
            Token token
                = this.Database.TokenStore.Get(tokenGuid);

            if ( token == null ) {
                return NotFound();
            } else {
                this.Database.TokenStore.Delete(tokenGuid);
                throw new HttpNoContentException(
                    ControllerStrings.Warning103_TokenDeleteOk
                );
            }
        }
    }
}
