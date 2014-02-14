using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.HttpGet.SessionController {
    public class TokenResponse {
        public string Token;
        public DateTime Expires;
        public string[] Pending;
    }
}