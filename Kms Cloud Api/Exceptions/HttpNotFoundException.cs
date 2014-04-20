using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Exceptions {
    [Serializable]
    public class HttpNotFoundException : HttpProcessException {
        public HttpNotFoundException(string message, Exception innerException) { }
        public HttpNotFoundException(string message = null) : base(message) { }
    }
}