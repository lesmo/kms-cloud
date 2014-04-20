using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Kms.Cloud.Api.Exceptions {
    [Serializable]
    public abstract class HttpProcessException : Exception {
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
        }
        protected HttpProcessException(string message = null) : base(message) { }
        protected HttpProcessException(string message, Exception innerException) : base(message, innerException) { }
    }
}