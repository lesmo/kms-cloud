using Kilometros_WebGlobalization.API;
using Kms.Cloud.Api.Exceptions;
using Kms.Cloud.Api.Models.RequestModels;
using Kms.Cloud.Api.Properties;
using Kms.Cloud.Database;
using Kms.Interop.OAuth;
using Kms.Interop.OAuth.SocialClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kms.Cloud.Api.Controllers {
    [Authorize]
    public class OAuth3rdPartyAddController : OAuthBaseController {
        [HttpPost]
        [Route("oauth/3rd/facebook/add")]
        public IHttpActionResult FacebookAdd([FromBody]OAuthFacebookLoginPost dataPost) {
            ValidateOAuth3rdAddRequest(OAuthService.Facebook, dataPost);
            var facebookClient = OAuth3rdClient<FacebookClient>(dataPost);

            OAuthCredential facebookCredential;
            try {
                facebookClient.ExchangeCodeForToken(dataPost.Code);

                facebookCredential = new OAuthCredential {
                    OAuthProvider = OAuthService.Facebook,
                    Uid       = facebookClient.UserID,
                    Token     = facebookClient.Token.Key,
                    IsInvalid = false,

                    User      = CurrentUser
                };
            } catch ( OAuthUnexpectedResponse ex ) {
                throw new HttpBadRequestException(
                    "105 " + ControllerStrings.Warning105_SocialTokenNotFound,
                    ex
                );
            }
            
            Database.OAuthCredentialStore.Add(facebookCredential);
            Database.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("oauth/3rd/twitter/add")]
        public IHttpActionResult TwitterAdd([FromBody]OAuth3rdLoginPost dataPost) {
            ValidateOAuth3rdAddRequest(OAuthService.Twitter, dataPost);
            var twitterClient = OAuth3rdClient<TwitterClient>(dataPost);

            OAuthCredential twitterCredential;
            try {
                twitterCredential = new OAuthCredential {
                    OAuthProvider = OAuthService.Twitter,
                    Uid       = twitterClient.UserID,
                    Token     = twitterClient.Token.Key,
                    Secret    = twitterClient.Token.Secret,
                    IsInvalid = false,

                    User      = CurrentUser
                };
            } catch ( OAuthUnexpectedResponse ex ) {
                throw new HttpBadRequestException(
                    "105 " + ControllerStrings.Warning105_SocialTokenNotFound,
                    ex
                );
            }

            Database.OAuthCredentialStore.Add(twitterCredential);
            Database.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("oauth/3rd/fitbit/add")]
        public void FitbitAdd([FromBody]OAuth3rdLoginPost postData) {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("oauth/3rd/nike/add")]
        public void NikeAdd([FromBody]OAuth3rdLoginPost postData) {
            throw new NotImplementedException();
        }
    }
}
