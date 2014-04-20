using Kms.Cloud.Api.Models;
using Kms.Cloud.Api.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Kms.Cloud.Api.MessageHandlers {
    public class ResponseLastModifiedHandler : DelegatingHandler {
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.DateTime.ToString(System.String)")]
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