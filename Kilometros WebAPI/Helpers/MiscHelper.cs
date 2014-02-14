using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Kilometros_WebAPI.Helpers {
    public static class MiscHelper {
        public static IPrincipal Principal {
            get {
                if ( HttpContext.Current != null )
                    return HttpContext.Current.User;
                else
                    return Thread.CurrentPrincipal;
            }
            set {
                Thread.CurrentPrincipal = value;

                if ( HttpContext.Current != null )
                    HttpContext.Current.User = value;
            }
        }

        public static bool BytesEqual(byte[] a, byte[] b) {
            if ( a == null )
                return b == null;

            if ( b == null )
                return false;
                
            if ( a.Length != b.Length )
                return false;

            for (int i = 0; i < a.Length; i++)
                if ( a[i] != b[i])
                    return false;

            return true;
        }

        internal static void SetPrincipal(IPrincipal principal) {
            Thread.CurrentPrincipal = principal;

            if ( HttpContext.Current != null )
                HttpContext.Current.User = principal;
        }

        internal static PrincipalType GetPrincipal<PrincipalType>() {
            if ( Thread.CurrentPrincipal != null )
                return (PrincipalType)Thread.CurrentPrincipal;
            else
                return (PrincipalType)HttpContext.Current.User;
        }

        internal static Task<HttpResponseMessage> ReturnResponseAndHalt(HttpResponseMessage response) {
            TaskCompletionSource<HttpResponseMessage> task
                = new TaskCompletionSource<HttpResponseMessage>();
            task.SetResult(response);

            return task.Task;
        }
    }
}