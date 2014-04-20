using Kilometros_WebGlobalization.API;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace Kms.Cloud.Api.ExceptionFilters {
    public sealed class DbValidationExceptionFilter : ExceptionFilterAttribute {
        public override void OnException(HttpActionExecutedContext actionExecutedContext) {
            if ( actionExecutedContext.Exception is DbEntityValidationException ) {
                DbEntityValidationException dbException
                    = (DbEntityValidationException)actionExecutedContext.Exception;
                DbEntityValidationResult dbValidationResult
                    = dbException.EntityValidationErrors.FirstOrDefault();
                DbValidationError dbFirstValidationError
                    = dbValidationResult.ValidationErrors.FirstOrDefault();

                string responseText
                    = string.Format(
                        "{0}:{1}",
                        dbFirstValidationError.PropertyName,
                        dbFirstValidationError.ErrorMessage
                    );

                actionExecutedContext.Response
                    = new HttpResponseMessage(HttpStatusCode.BadRequest);
                actionExecutedContext.Response.Content
                    = new StringContent(responseText);
            }
        }
    }
}