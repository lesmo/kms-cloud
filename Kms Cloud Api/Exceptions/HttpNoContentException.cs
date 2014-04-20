using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Exceptions {
    public class HttpNoContentException : HttpProcessException {
        public HttpNoContentException(string message) : base(message) {
        }
    }
}