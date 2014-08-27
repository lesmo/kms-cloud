using System;
using System.Runtime.Serialization;

namespace Kms.Cloud.Api.Exceptions {
    [Serializable]
    public class HttpUnauthorizedException : HttpProcessException {
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
        }
        public HttpUnauthorizedException(string message) : base(message) { }
        public HttpUnauthorizedException(string message, Exception innerException) : base(message, innerException) { }
    }
}