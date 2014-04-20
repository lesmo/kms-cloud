using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.RequestModels {
    public class LoginPost {
        public string Email { get; set; }
        public string AccessHash { get; set; }
    }
}