using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Exceptions {
    [Serializable]
    public abstract class HttpProcessException : Exception {
        public override string Message {
            get {
                return this._message;
            }
        }
        protected string _message;

        public HttpProcessException(string message) {
            this._message = message;
        }
    }
}