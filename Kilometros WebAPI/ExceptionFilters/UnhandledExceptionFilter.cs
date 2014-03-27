using Kilometros_WebAPI.Exceptions;
using Kilometros_WebGlobalization.API;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace Kilometros_WebAPI.ExceptionFilters {
    public class UnhandledExceptionFilter : ExceptionFilterAttribute {
        public override void OnException(HttpActionExecutedContext httpContext) {
            if (
                httpContext.Exception is HttpProcessException
                || httpContext.Exception is DbEntityValidationException
            ) {
                return;
            } else if ( httpContext.Exception is NotImplementedException ) {
                httpContext.Response
                    = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                httpContext.Response.Content
                    = new StringContent(
                        ControllerStrings.GenericNotImplemented
                    );
            } else if (
                httpContext.Exception is ArgumentException
                || httpContext.Exception is ArgumentOutOfRangeException
            ) {
                httpContext.Response
                    = new HttpResponseMessage(HttpStatusCode.BadRequest);
                string responseMessage
                    = string.Format(
                        ControllerStrings.GenericValidationError,
                        httpContext.Exception.Message
                    );
                httpContext.Response.Content
                    = new StringContent(responseMessage);
            } else if ( ! Debugger.IsAttached ) {
                // Throw in ELMAH call
            } else {
                throw new Exception("Ahoy! An exception!", httpContext.Exception);
            }
        }
    }
}