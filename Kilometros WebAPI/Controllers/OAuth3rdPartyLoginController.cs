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
    public class OAuth3rdPartyLoginController : ApiController {
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();

        [HttpPost]
        [Route("oauth/3rd/facebook/login")]
        public void FacebookLogin([FromBody]string facebook_token) {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;

            if ( identity.UserData != null ) {
            }

            OAuthCredential oAuthCredential
                = Database.OAuthCredentialStore.GetFirst(
                    f => f.OAuthProvider == OAuthService.Facebook && f.Token == facebook_token
                );

            if ( oAuthCredential == null ) {
                throw new HttpUnauthorizedException(
                    "105" + ControllerStrings.Warning105_SocialTokenInvalid
                );
            }
        }

        [HttpPost]
        [Route("oauth/3rd/twitter/login")]
        public void TwitterLogin() {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("oauth/3rd/fitbit/login")]
        public void FitbitLogin() {
        }

        [HttpPost]
        [Route("oauth/3rd/nike/login")]
        public void NikeLogin() {
        }
    }
}
