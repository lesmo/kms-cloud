using Kilometros_WebAPI.Helpers;
using Kilometros_WebAPI.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kilometros_WebAPI.Controllers {
    public abstract class IKMSController : ApiController {
        /// <summary>
        ///     Provee acceso a un Contexto de Base de Datos para el
        ///     controlador actual. Los objetos expuestos por Identity
        ///     y OAuth NO pueden ser modificados o almacenados en éste
        ///     contexto, debe obtenerse un objeto desde ésta instancia.
        /// </summary>
        protected KilometrosDatabase.Abstraction.WorkUnit Database {
            get {
                if ( this._database == null )
                    this._database
                        = new KilometrosDatabase.Abstraction.WorkUnit();
                
                return this._database;
            }
        }
        private KilometrosDatabase.Abstraction.WorkUnit _database = null;

        /// <summary>
        ///     Contiene la Identidad del Contexto de Seguridad actual de
        ///     la petición, permitiéndo acceder a información 
        /// </summary>
        protected KMSIdentity Identity {
            get {
                if ( this._identity == null )
                    this._identity 
                        = MiscHelper.GetPrincipal().Identity as KMSIdentity;

                return this._identity;
            }
        }
        private KMSIdentity _identity = null;

        protected HttpOAuthAuthorization OAuth {
            get {
                if ( this._oAuth == null )
                    this._oAuth
                        = new HttpOAuthAuthorization(Identity.OAuth, Database);

                return this._oAuth;
            }
        }
        private HttpOAuthAuthorization _oAuth = null;
    }
}
