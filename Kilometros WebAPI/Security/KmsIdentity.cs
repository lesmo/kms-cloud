using KilometrosDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Kilometros_WebAPI.Security {
    public class KMSIdentity : IIdentity {
        public User UserData {
            get {
                if ( this._oAuth != null && this._oAuth.Token != null && this._oAuth.Token.User != null )
                    return this._oAuth.Token.User;
                else
                    return null;
            }
        }

        public string AuthenticationType {
            get {
                return this._authenticationType;
            }
        }
        private string _authenticationType = "";

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

        public ApiKey ApiKey {
            get {
                if ( this._oAuth != null )
                    return this._oAuth.ConsumerKey;
                else
                    return null;
            }
        }

        public Token Token {
            get {
                if ( this._oAuth != null )
                    return this._oAuth.Token;
                else
                    return null;
            }
        }

        public HttpOAuthAuthorization OAuth {
            get {
                return this._oAuth;
            }
        }
        private HttpOAuthAuthorization _oAuth;

        public KMSIdentity(HttpOAuthAuthorization httpOAuth = null, string authenticationType = "KmsToken") {
            if ( httpOAuth == null )
                return;

            this._authenticationType
                = authenticationType;
            this._oAuth
                = httpOAuth;
        }
    }
}