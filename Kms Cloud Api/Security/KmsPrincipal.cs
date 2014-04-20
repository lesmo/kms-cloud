using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Kms.Cloud.Api;

namespace Kms.Cloud.Api.Security {
    public class KmsPrincipal : IPrincipal {
        public KmsPrincipal(KMSIdentity identity) {
            this.Identity = identity;
        }

        public IIdentity Identity {
            get;
            private set;
        }

        public bool IsInRole(string role) {
            throw new NotImplementedException(
                "KMS uses no Roles for Users as of v" + WebApiApplication.GetAssemblyVersion().ToString()
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
            
            new KmsPrincipal(new KMSIdentity()).SetAsCurrent();
            return CurrentPrincipal();
        }

        public static void SetCurrentPrincipal(KmsPrincipal principal) {
            Thread.CurrentPrincipal = principal;

            if ( HttpContext.Current != null )
                HttpContext.Current.User = principal;
        }
    }
}