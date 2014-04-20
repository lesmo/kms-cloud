using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Kms.Cloud.Api.Security {
    public class KmsIdentity : IIdentity {
        /// <summary>
        ///     Devuelve el objeto de Identidad que ostenta el Principal del Thread actual.
        /// </summary>
        public static KmsIdentity GetCurrentPrincipalIdentity() {
            return KmsPrincipal.CurrentPrincipal().Identity as KmsIdentity;
        }

        public string AuthenticationType {
            get;
            private set;
        }

        public string Name {
            get {
                if ( this._oAuth != null && this._oAuth.Token != null && this._oAuth.Token.User != null )
                    return this._oAuth.Token.User.Email;
                else
                    return "";
            }
        }

        public bool IsAuthenticated {
            get {
                // Si hay un Token y NO tiene Verifier, entonces verificamos si tiene un Usuario asociado
                if ( this._oAuth != null && this._oAuth.Token != null && ! this._oAuth.Token.VerificationCode.HasValue)
                    return this._oAuth.Token.User != null;
                else
                    return false;
            }
        }

        public HttpOAuthAuthorization OAuth {
            get {
                return this._oAuth;
            }
        }
        private HttpOAuthAuthorization _oAuth;

        public KmsIdentity(HttpOAuthAuthorization httpOAuth = null, string authenticationType = "KmsToken") {
            if ( httpOAuth == null )
                return;

            this.AuthenticationType
                = authenticationType;
            this._oAuth
                = httpOAuth;
        }
    }
}