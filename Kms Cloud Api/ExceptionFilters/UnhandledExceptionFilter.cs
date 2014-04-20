using Kms.Cloud.Api.Exceptions;
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
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace Kms.Cloud.Api.ExceptionFilters {
    [SuppressMessage("Microsoft.Design", "CA1018:MarkAttributesWithAttributeUsage")]
    public sealed class UnhandledExceptionFilterAttributeFilter : ExceptionFilterAttribute {
        public override void OnException(HttpActionExecutedContext actionExecutedContext) {
            if (
                actionExecutedContext.Exception is HttpProcessException
                || actionExecutedContext.Exception is DbEntityValidationException
            ) {
                return;
            } else if ( actionExecutedContext.Exception is NotImplementedException ) {
                actionExecutedContext.Response
                    = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                actionExecutedContext.Response.Content
                    = new StringContent(
                        ControllerStrings.GenericNotImplemented
                    );
            } else if (
                actionExecutedContext.Exception is ArgumentException
                || actionExecutedContext.Exception is ArgumentOutOfRangeException
            ) {
                actionExecutedContext.Response
                    = new HttpResponseMessage(HttpStatusCode.BadRequest);
                string responseMessage
                    = string.Format(
                        CultureInfo.CurrentCulture,
                        "{0}",
                        actionExecutedContext.Exception.Message
                    );
                actionExecutedContext.Response.Content
                    = new StringContent(responseMessage);
            } else if ( ! Debugger.IsAttached ) {
                // Throw in ELMAH call
            } else {
                throw new OperationCanceledException("Ahoy! An exception!", actionExecutedContext.Exception);
            }
        }
    }
}