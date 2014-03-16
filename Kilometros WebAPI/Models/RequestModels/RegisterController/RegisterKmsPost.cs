using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.HttpPost.RegisterController {
    public class RegisterKmsPost {
        public string Email;
        public string Password;
        public string RegionCode;
        public string PreferredCultureCode;
    }
}