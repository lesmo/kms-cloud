using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Diagnostics.CodeAnalysis;

namespace Kms.Cloud.Api.Security {
    public class KmsPrincipal : IPrincipal {
        public KmsPrincipal(KmsIdentity identity) {
            this.Identity = identity;
        }

        public IIdentity Identity {
            get;
            private set;
        }

        public bool IsInRole(string role) {
            throw new NotImplementedException(
                "KMS uses no Roles for Users as of v" + WebApiApplication.AssemblyVersion.ToString()
            );
        }

        public void SetAsCurrent() {
            KmsPrincipal.SetCurrentPrincipal(this);
        }

        public static KmsPrincipal CurrentPrincipal() {
            if ( Thread.CurrentPrincipal != null )
                return Thread.CurrentPrincipal as KmsPrincipal;
            else if ( HttpContext.Current.User != null )
                return HttpContext.Current.User as KmsPrincipal;
            
            new KmsPrincipal(new KmsIdentity()).SetAsCurrent();
            return CurrentPrincipal();
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetCurrentPrincipal(KmsPrincipal principal) {
            Thread.CurrentPrincipal = principal;

            if ( HttpContext.Current != null )
                HttpContext.Current.User = principal;
        }
    }
}