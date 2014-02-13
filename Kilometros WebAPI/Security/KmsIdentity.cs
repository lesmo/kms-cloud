using KilometrosDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Kilometros_WebAPI.Security {
    public class KmsIdentity : IIdentity {
        public User UserData {
            get {
                return this._userData;
            }
        }
        private User _userData = null;

        public string AuthenticationType {
            get {
                return this._authenticationType;
            }
        }
        private string _authenticationType = "";

        public string Name {
            get {
                return this._name;
            }
        }
        private string _name = "";

        public bool IsAuthenticated {
            get {
                return this._isAuthenticated;
            }
        }
        private bool _isAuthenticated = false;

        public ApiKey ApiKey {
            get {
                return this._apiKey;
            }
        }
        private ApiKey _apiKey;

        public KmsIdentity(User user = null, string authenticationType = "KmsToken", ApiKey apiKey = null) {
            if ( user == null )
                return;

            this._authenticationType = authenticationType;
            this._name = user.Email;
            this._isAuthenticated = true;

            this._userData = user;
            this._apiKey = apiKey;
        }
    }
}