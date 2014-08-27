using System;

namespace Kms.Cloud.Api.Exceptions {
    [Serializable]
    public class HttpNotFoundException : HttpProcessException {
        public HttpNotFoundException(string message, Exception innerException) { }
        public HttpNotFoundException(string message = null) : base(message) { }
    }
}