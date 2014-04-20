using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Kms.Cloud.Api.Security;
using Kms.Cloud.Database;
using System.Web;
using Kms.Cloud.Api.Models.ResponseModels;
using Kilometros_WebGlobalization.API;
using Kms.Cloud.Api.Models.RequestModels;
using System.Globalization;
using Kms.Cloud.Api.Exceptions;
using System.Text;

namespace Kms.Cloud.Api.Controllers {
    public class OAuth3rdPartyLoginController : BaseController {
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
