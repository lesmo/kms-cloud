using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.RequestModels {
    public class AccountPost {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PreferredCultureCode { get; set; }
        public string RegionCode { get; set; }
        public string Password { get; set; }
    }
}