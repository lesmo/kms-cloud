﻿using System;

namespace Kms.Cloud.Api.Exceptions {
    [Serializable]
    public class HttpNoContentException : HttpProcessException {
        public HttpNoContentException(string message, Exception innerException) : base(message, innerException) { }
        public HttpNoContentException(string message = null) : base(message) { }
    }
}