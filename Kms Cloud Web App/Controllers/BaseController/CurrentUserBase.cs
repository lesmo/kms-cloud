using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Kms.Cloud.WebApp.Controllers {
    #if !DEBUG
        [RequireHttps]
    #endif
    public abstract partial class BaseController {
        public User CurrentUser {
            get {
                if ( this._currentUser != null )
                    return this._currentUser;

                if ( !User.Identity.IsAuthenticated )
                    return null;

                Guid userGuid;

                try {
                    userGuid
                        = new Guid().FromBase64String(User.Identity.Name);
                } catch {
                    FormsAuthentication.SignOut();
                    return null;
                }

                this._currentUser
                    = Database.UserStore.GetFirst(
                        filter:
                            f => f.Guid == userGuid,
                        include:
                            new string[] { "UserDataTotalDistance" }
                    );

                if ( this._currentUser == null )
                    FormsAuthentication.SignOut();

                return this._currentUser;
            }
        }
        private User _currentUser = null;
    }
}