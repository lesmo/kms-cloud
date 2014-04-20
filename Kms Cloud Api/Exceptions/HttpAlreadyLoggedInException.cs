using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Kms.Cloud.Api.Exceptions {
    [Serializable]
    public class HttpAlreadyLoggedInException : HttpProcessException {
        public override void GetObjectData(SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            base.GetObjectData(info, context);
        }
        public HttpAlreadyLoggedInException(string message, Exception innerException) : base(message, innerException) { }
        public HttpAlreadyLoggedInException(string message = null) : base(message) { }
    }
}