using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Exceptions {
    public class HttpAlreadyLoggedInException : HttpProcessException {
        public HttpAlreadyLoggedInException(string message) : base(message) {
        }
    }
}