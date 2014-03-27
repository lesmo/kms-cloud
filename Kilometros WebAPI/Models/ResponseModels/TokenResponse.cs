using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.ResponseModels {
    public class TokenResponse {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public string[] Pending { get; set; }
    }
}