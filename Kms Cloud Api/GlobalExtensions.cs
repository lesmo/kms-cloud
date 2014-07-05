using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel.Channels;
using System.Web;

namespace Kms.Cloud.Api {
    internal static class GlobalExtensions {
        public static Task<HttpResponseMessage> NewHttpResponseTask(this HttpResponseMessage @this) {
            var taskCompletion = new TaskCompletionSource<HttpResponseMessage>();
            taskCompletion.SetResult(@this);

            return taskCompletion.Task;
        }

        public static String GetClientIpAddress(this HttpRequestMessage request) {
            // Source: http://forums.asp.net/post/4855159.aspx
            if ( request.Properties.ContainsKey("MS_HttpContext") ) {
                var prop = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request;
                return prop.UserHostAddress;
            } else if ( request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name) ) {
                var prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            } else {
                return String.Empty;
            }
        }
    }
}