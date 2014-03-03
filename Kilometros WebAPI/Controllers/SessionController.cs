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
using System.Text;

namespace Kilometros_WebAPI.Controllers {
    public class SessionController : ApiController {
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();

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

        [Authorize]
        [HttpGet]
        [Route("session")]
        public IHttpActionResult GetToken() {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            Token token
                = identity.Token;

            if ( token.ExpirationDate.HasValue ) {
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

        [Authorize]
        [HttpDelete]
        [Route("session")]
        public IHttpActionResult DeleteToken() {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            Token token
                = identity.Token;

            Database.TokenStore.Delete(token.Guid);

            throw new HttpNoContentException(
                ControllerStrings.Warning103_TokenDeleteOk
            );
        }
    }
}
