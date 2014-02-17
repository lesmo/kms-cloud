using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Exceptions {
    public class HttpConflictException : HttpProcessException {
        public HttpConflictException(string message) : base(message) {
        }
    }
}