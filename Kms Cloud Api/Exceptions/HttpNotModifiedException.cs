using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Exceptions {
    public class HttpNotModifiedException : HttpProcessException {
        public HttpNotModifiedException() : base("Not Modified Since") {
        }
    }
}