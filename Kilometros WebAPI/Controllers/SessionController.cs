using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Kilometros_WebAPI.Security;
using KilometrosDatabase;
using System.Web;
using Kilometros_WebAPI.HttpPostModels.SessionController;

namespace Kilometros_WebAPI.Controllers {
    public class SessionController : ApiController {
        public KilometrosDatabase.Abstraction.WorkUnit _db
            = new KilometrosDatabase.Abstraction.WorkUnit();

        public struct TokenResponse {
            public string token;
            public DateTime expires;
            public string[] pending;
        }

        [HttpPost]
        [Route("session/kms")]
        public HttpResponseMessage KmsLogin([FromBody]KmsLoginPost userPost) {
            User user = this._db.UserStore.GetAll(
                u => u.Email == userPost.Email
            ).FirstOrDefault();

            if ( user == null || userPost.AccessHash != user.PasswordString ) {
                HttpContext.Current.Response.Headers.Add(
                    "Warning",
                    "201 " + Resources.SessionController.Warning201_UserNotFound
                );

                return null;
            }

            KmsIdentity identity = (KmsIdentity)this.User.Identity;
            Token token = new Token() {
                ApiKey = identity.ApiKey,
                User = user,
                Guid = Guid.NewGuid(),

                CreationDate = DateTime.Now,
                LastUseDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddDays(30)
            };

            this._db.TokenStore.Add(token);
            this._db.SaveChanges();

            TokenResponse tokenResponse = new TokenResponse() {
                expires = DateTime.Now.AddDays(30),
                token = Convert.ToBase64String(token.Guid.ToByteArray()),
                pending = {}
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
            HttpResponseMessage response = new HttpResponseMessage();

            byte[] tokenBytes
                = Convert.FromBase64String(tokenString);
            Guid tokenGuid
                = new Guid(tokenBytes);
            Token token
                = this._db.TokenStore.Get(tokenGuid);

            if ( token == null ) {
                response.StatusCode = HttpStatusCode.NotFound;
                response.Headers.Add(
                    "Warning",
                    "401 " + Resources.SessionController.Warning401_TokenNotFound
                );
            } else {
                token.ExpirationDate
                    = token.ExpirationDate.Value.AddMonths(3);
                this._db.TokenStore.Update(token);
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
                = this._db.TokenStore.Get(tokenGuid);

            if ( token == null ) {
                response.StatusCode = HttpStatusCode.NotFound;
                response.Headers.Add(
                    "Warning",
                    "401 " + Resources.SessionController.Warning401_TokenNotFound
                );
            } else {
                this._db.TokenStore.Delete(tokenGuid);
                response.StatusCode = HttpStatusCode.NoContent;
            }

            return response;
        }
    }
}
