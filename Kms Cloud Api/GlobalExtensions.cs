using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Kms.Cloud.Api {
    internal static class GlobalExtensions {
        public static Task<HttpResponseMessage> NewHttpResponseTask(this HttpResponseMessage @this) {
            var taskCompletion = new TaskCompletionSource<HttpResponseMessage>();
            taskCompletion.SetResult(@this);

            return taskCompletion.Task;
        }
    }
}