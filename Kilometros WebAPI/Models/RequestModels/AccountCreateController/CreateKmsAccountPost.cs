using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.HttpPost.AccountCreateController {
    public class CreateKmsAccountPost {
        public string Name;
        public string LastName;

        public string Email;
        public string Password;

        public string RegionCode;
        public string CultureCode;
        public short UtcOffset;
    }
}