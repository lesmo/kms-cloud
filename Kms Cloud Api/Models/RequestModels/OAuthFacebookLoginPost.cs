using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.RequestModels {
    public class OAuthFacebookLoginPost : IOAuthTokenPost {
        public string ID { get; set; }
        public string Code { get; set; }
    }
}