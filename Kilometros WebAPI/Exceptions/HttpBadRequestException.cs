using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Exceptions {
    public class HttpBadRequestException : HttpProcessException {
        public HttpBadRequestException(string message) : base(message) {}
    }
}