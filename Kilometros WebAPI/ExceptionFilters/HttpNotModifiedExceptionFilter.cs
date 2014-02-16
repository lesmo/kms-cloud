using Kilometros_WebAPI.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace Kilometros_WebAPI.ExceptionFilters {
    public class HttpNotModifiedExceptionFilter : ExceptionFilterAttribute {
        public override void OnException(HttpActionExecutedContext httpContext) {
            if ( httpContext.Exception is HttpNotModifiedException ) {
                httpContext.Response
                    = new HttpResponseMessage(HttpStatusCode.NotModified);
            }

            base.OnException(httpContext);
        }
    }
}