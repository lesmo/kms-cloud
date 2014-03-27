using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.ResponseModels {
    public class AccountResponse {
        public DateTime AccountCreationDate { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string RegionCode { get; set; }

        public string PreferredCultureCode { get; set; }
        public string PictureUri { get; set; }
    }
}