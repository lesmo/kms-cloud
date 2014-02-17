using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Exceptions {
    public class HttpUnauthorizedException : HttpProcessException {
        public HttpUnauthorizedException(string message) : base(message) {
        }
    }
}