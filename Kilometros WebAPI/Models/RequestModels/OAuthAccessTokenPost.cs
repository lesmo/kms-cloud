using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.RequestModels {
    public class OAuthAccessTokenPost {
        public string oauth_verifier {
            get;
            set;
        }
    }
}