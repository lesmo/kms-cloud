using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Exceptions {
    public class HttpNotFoundException : HttpProcessException {
        public HttpNotFoundException(string message) : base(message) {
        }
    }
}