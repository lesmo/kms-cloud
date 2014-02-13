using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
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

        public static void SetPrincipal(IPrincipal principal) {
            Thread.CurrentPrincipal = principal;

            if ( HttpContext.Current != null )
                HttpContext.Current.User = principal;
        }
    }
}