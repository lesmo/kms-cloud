using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Exceptions {
    [Serializable]
    public class HttpConflictException : HttpProcessException {
        public HttpConflictException(string message, Exception innerException) : base(message, innerException) { }
        public HttpConflictException(string message = null) : base(message) { }
    }
}