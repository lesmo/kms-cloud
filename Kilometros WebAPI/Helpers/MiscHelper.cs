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

        internal static Task<HttpResponseMessage> ReturnHttpResponseAndHalt(HttpResponseMessage response) {
            TaskCompletionSource<HttpResponseMessage> task
                = new TaskCompletionSource<HttpResponseMessage>();
            task.SetResult(response);

            return task.Task;
        }

        internal static string Base64FromGuid(Guid guid) {
            byte[] guidBytes
                = guid.ToByteArray();
            string guidBase64String
                = Convert.ToBase64String(guidBytes);

            guidBase64String
                = guidBase64String.Replace('/', '.');
            guidBase64String
                = guidBase64String.Replace('+', '-');

            return guidBase64String.Substring(0, guidBase64String.Length - 2);
        }

        internal static Guid? GuidFromBase64(string base64String) {
            if ( base64String.Length != 22 )
                return null;

            base64String
                = base64String.Replace('.', '/');
            base64String
                = base64String.Replace('-', '+');

            byte[] guidBytes
                = Convert.FromBase64String(base64String + "==");

            return new Guid(guidBytes);
        }
    }
}