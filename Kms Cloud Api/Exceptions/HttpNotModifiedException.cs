using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Exceptions {
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors"), Serializable]
    public class HttpNotModifiedException : HttpProcessException {
        public HttpNotModifiedException() : base("Not Modified Since") {}
    }
}