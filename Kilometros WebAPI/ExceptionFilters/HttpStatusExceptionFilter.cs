using Kilometros_WebAPI.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace Kilometros_WebAPI.ExceptionFilters {
    public class HttpStatusExceptionFilter : ExceptionFilterAttribute {
        public override void OnException(HttpActionExecutedContext httpContext) {
            if ( httpContext.Exception is HttpNotModifiedException ) {
                httpContext.Response
                    = new HttpResponseMessage(HttpStatusCode.NotModified);
            } else if ( httpContext.Exception is HttpNoContentException ) {
                httpContext.Response
                    = new HttpResponseMessage(HttpStatusCode.NoContent);
            } else if ( httpContext.Exception is HttpAlreadyLoggedInException ) {
                httpContext.Response
                    = new HttpResponseMessage(HttpStatusCode.Forbidden);
            } else if ( httpContext.Exception is HttpUnauthorizedException ) {
                httpContext.Response
                    = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            if ( httpContext.Exception.Message != null )
                httpContext.Response.Content
                    = new StringContent(httpContext.Exception.Message);
        }
    }
}