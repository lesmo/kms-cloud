using Kilometros_WebGlobalization.API;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace Kilometros_WebAPI.ExceptionFilters {
    public class DbValidationExceptionFilter : ExceptionFilterAttribute {
        public override void OnException(HttpActionExecutedContext httpContext) {
            if ( httpContext.Exception is DbEntityValidationException ) {
                DbEntityValidationException dbException
                    = (DbEntityValidationException)httpContext.Exception;
                DbEntityValidationResult dbValidationResult
                    = dbException.EntityValidationErrors.FirstOrDefault();
                DbValidationError dbFirstValidationError
                    = dbValidationResult.ValidationErrors.FirstOrDefault();

                string responseText
                    = string.Format(
                        "{0}: {1}",
                        dbFirstValidationError.PropertyName,
                        dbFirstValidationError.ErrorMessage
                    );

                httpContext.Response
                    = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpContext.Response.Content
                    = new StringContent(responseText);
            }
        }
    }
}