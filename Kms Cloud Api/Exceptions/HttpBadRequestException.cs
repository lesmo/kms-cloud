using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Exceptions {
    public class HttpBadRequestException : HttpProcessException {
        public HttpBadRequestException(string message) : base(message) {}
    }
}