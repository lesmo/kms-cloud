using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.RequestModels {
    public class OAuthAccessTokenPost {
        public string oauth_verifier {
            get;
            set;
        }
    }
}