using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Exceptions {
    public class HttpUnauthorizedException : HttpProcessException {
        public HttpUnauthorizedException(string message) : base(message) {
        }
    }
}