using System;

namespace Kms.Cloud.Api.Exceptions {
    [Serializable]
    public class HttpBadRequestException : HttpProcessException {
        public HttpBadRequestException(string message = null) : base(message) {}
        public HttpBadRequestException(string message, Exception ex) : base(message, ex) { }
    }
}