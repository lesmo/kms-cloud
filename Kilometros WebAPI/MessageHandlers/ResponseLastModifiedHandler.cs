using Kilometros_WebAPI.Models;
using Kilometros_WebAPI.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Kilometros_WebAPI.MessageHandlers {
    public class ResponseLastModifiedHandler : DelegatingHandler {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>(
                (responseToCompleteTask) => {
                    HttpResponseMessage response
                        = responseToCompleteTask.Result;
                    
                    dynamic responseObject;
                    response.TryGetContentValue<dynamic>(out responseObject);

                    if ( responseObject is IModifiedDate ) {
                        response.Headers.TryAddWithoutValidation(
                            "Last-Modified",
                            ((IModifiedDate)responseObject).LastModified.ToString(
                                (new DateTimeFormatInfo()).RFC1123Pattern
                            )
                        );
                    }

                    return response;
                }
            );
        }
    }
}