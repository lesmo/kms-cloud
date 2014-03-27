using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Kilometros_WebAPI.Security;
using KilometrosDatabase;
using System.Web;
using Kilometros_WebAPI.Models.ResponseModels;
using Kilometros_WebGlobalization.API;
using Kilometros_WebAPI.Models.RequestModels;
using System.Globalization;
using Kilometros_WebAPI.Exceptions;
using Kilometros_WebAPI.Helpers;
using System.Text;

namespace Kilometros_WebAPI.Controllers {
    public class OAuth3rdPartyLoginController : IKMSController {
        [HttpPost]
        [Route("oauth/3rd/facebook/login")]
        public void FacebookLogin([FromBody]OAuth3rdLoginPost postData) {
            if ( ! OAuth.IsRequestToken )
                throw new HttpUnauthorizedException(
                    "105 " + ControllerStrings.Warning106_RequestTokenRequired
                );

            OAuthCredential oAuthCredential
                = Database.OAuthCredentialStore.GetFirst(
                    f =>
                        f.OAuthProvider == OAuthService.Facebook
                        && f.Uid == postData.SocialId
                );

            if ( oAuthCredential == null )
                throw new HttpNotFoundException(
                    "105 " + ControllerStrings.Warning105_SocialTokenNotFound
                );

            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("oauth/3rd/twitter/login")]
        public void TwitterLogin() {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("oauth/3rd/fitbit/login")]
        public void FitbitLogin() {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("oauth/3rd/nike/login")]
        public void NikeLogin() {
            throw new NotImplementedException();
        }
    }
}
