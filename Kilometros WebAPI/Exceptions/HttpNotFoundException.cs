﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Exceptions {
    public class HttpNotFoundException : HttpProcessException {
        public HttpNotFoundException(string message) : base(message) {
        }
    }
}