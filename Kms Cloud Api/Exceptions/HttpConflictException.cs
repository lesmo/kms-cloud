using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Exceptions {
    public class HttpConflictException : HttpProcessException {
        public HttpConflictException(string message) : base(message) {
        }
    }
}