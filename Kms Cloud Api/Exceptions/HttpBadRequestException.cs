using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Exceptions {
    [Serializable]
    public class HttpBadRequestException : HttpProcessException {
        public HttpBadRequestException(string message = null) : base(message) {}
        public HttpBadRequestException(string message, Exception ex) : base(message, ex) { }
    }
}