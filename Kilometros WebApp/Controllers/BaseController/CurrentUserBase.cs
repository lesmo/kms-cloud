using KilometrosDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Kilometros_WebApp.Controllers {
    public abstract partial class BaseController {
        protected User CurrentUser {
            get {
                if ( this._currentUser != null )
                    return this._currentUser;

                if ( !User.Identity.IsAuthenticated )
                    return null;

                Guid userGuid;

                try {
                    byte[] userGuidBytes
                        = Convert.FromBase64String(User.Identity.Name);
                    userGuid
                        = new Guid(userGuidBytes);
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