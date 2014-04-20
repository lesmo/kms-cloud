using Kms.Cloud.Api.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kms.Cloud.Api.Controllers {
    public abstract class IKMSController : ApiController {
        /// <summary>
        ///     Provee acceso a un Contexto de Base de Datos para el
        ///     controlador actual. Los objetos expuestos por Identity
        ///     y OAuth NO pueden ser modificados o almacenados en éste
        ///     contexto, debe obtenerse un objeto desde ésta instancia.
        /// </summary>
        protected Kms.Cloud.Database.Abstraction.WorkUnit Database {
            get {
                if ( this._database == null )
                    this._database
                        = new Kms.Cloud.Database.Abstraction.WorkUnit();
                
                return this._database;
            }
        }
        private Kms.Cloud.Database.Abstraction.WorkUnit _database = null;

        /// <summary>
        ///     Contiene la Identidad del Contexto de Seguridad actual de
        ///     la petición, permitiéndo acceder a información 
        /// </summary>
        private KMSIdentity Identity = KMSIdentity.GetCurrentPrincipalIdentity();

        protected HttpOAuthAuthorization OAuth {
            get {
                if ( this._oAuth == null )
                    this._oAuth
                        = new HttpOAuthAuthorization(Identity.OAuth, Database);

                return this._oAuth;
            }
        }
        private HttpOAuthAuthorization _oAuth = null;

        protected Kms.Cloud.Database.User CurrentUser {
            get {
                return this.OAuth.Token.User;
            }
        }
    }
}
