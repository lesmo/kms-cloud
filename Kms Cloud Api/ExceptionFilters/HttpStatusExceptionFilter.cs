using Kms.Cloud.Api.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace Kms.Cloud.Api.ExceptionFilters {
    public sealed class HttpStatusExceptionFilterAttribute : ExceptionFilterAttribute {
        public override void OnException(HttpActionExecutedContext actionExecutedContext) {
            if ( actionExecutedContext.Exception is HttpNotModifiedException ) {
                actionExecutedContext.Response
                    = new HttpResponseMessage(HttpStatusCode.NotModified);
            } else if ( actionExecutedContext.Exception is HttpNoContentException ) {
                actionExecutedContext.Response
                    = new HttpResponseMessage(HttpStatusCode.NoContent);
            } else if ( actionExecutedContext.Exception is HttpNotFoundException ) {
                actionExecutedContext.Response
                    = new HttpResponseMessage(HttpStatusCode.NotFound);
            } else if ( actionExecutedContext.Exception is HttpAlreadyLoggedInException ) {
                actionExecutedContext.Response
                    = new HttpResponseMessage(HttpStatusCode.Forbidden);
            } else if ( actionExecutedContext.Exception is HttpUnauthorizedException ) {
                actionExecutedContext.Response
                    = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            } else if ( actionExecutedContext.Exception is HttpConflictException ) {
                actionExecutedContext.Response
                    = new HttpResponseMessage(HttpStatusCode.Conflict);
            } else if ( actionExecutedContext.Exception is HttpBadRequestException ) {
                actionExecutedContext.Response
                    = new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            if (
                actionExecutedContext.Exception != null
                && actionExecutedContext.Exception.Message != null
                && (actionExecutedContext.Exception is HttpProcessException)
                && !(actionExecutedContext.Exception is HttpNoContentException)
            ) {
                actionExecutedContext.Response.Content
                    = new StringContent(actionExecutedContext.Exception.Message);
            }
        }
    }
}