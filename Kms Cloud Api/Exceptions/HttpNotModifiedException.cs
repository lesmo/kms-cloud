using System;
using System.Diagnostics.CodeAnalysis;

namespace Kms.Cloud.Api.Exceptions {
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors"), Serializable]
    public class HttpNotModifiedException : HttpProcessException {
        public HttpNotModifiedException() : base("Not Modified Since") {}
    }
}