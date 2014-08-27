using Kms.Cloud.Api.Security;
using Kms.Cloud.Database;
using Kms.Cloud.Database.Abstraction;
using System;
using System.Web;
using System.Web.Http;

namespace Kms.Cloud.Api.Controllers {
    [Authorize]
    public abstract class BaseController : ApiController {
        /// <summary>
        ///     Provee acceso a un Contexto de Base de Datos para el
        ///     controlador actual.
        /// </summary>
        protected WorkUnit Database {
            get {
                return (WorkUnit)HttpContext.Current.Items["Database"];
            }
        }

        /// <summary>
        ///     Contiene la Identidad del Contexto de Seguridad actual de
        ///     la petición, permitiéndo acceder a información 
        /// </summary>
        private KmsIdentity Identity = KmsIdentity.GetCurrentPrincipalIdentity();

        protected HttpOAuthAuthorization OAuth {
            get {
                if ( this._oAuth == null )
                    this._oAuth = new HttpOAuthAuthorization(Identity.OAuth, Database);

                return this._oAuth;
            }
        }
        private HttpOAuthAuthorization _oAuth = null;

        protected User CurrentUser {
            get {
                return this.OAuth.Token.User;
            }
        }

        protected Uri GetDynamicResourceUri(IPicture pictureObject) {
            return this.GetDynamicResourceUri(
                "Images",
                pictureObject.Guid.ToBase64String(),
                pictureObject.PictureExtension
            );
        }

        /// <summary>
        ///     Devuelve una Uri absoluta que apunta al Recurso generado dinámicamente especificado
        ///     por el nombre del archivo (normalmente el GUID del recurso en BD) y su extensión.
        /// </summary>
        /// <param name="method">
        ///     Método en controlador DynamicResources responsable de generar el recurso.
        /// </param>
        /// <param name="filename">
        ///     Nombre del archivo (normalmente el GUID del recurso en BD).
        /// </param>
        /// <param name="ext">
        ///     Extensión esperada por Método.
        /// </param>
        /// <returns>
        ///     URI absoluta que apunta al recurso descrito por los parámetros.
        /// </returns>
        protected Uri GetDynamicResourceUri(string method, string filename, string ext) {
            var contentUrl = Url.Content(
                string.Format(
                    "~/DynamicResources/{0}/{1}.{2}",
                    method,
                    filename,
                    ext
                )
            );

            return new Uri(
                Request.RequestUri,
                contentUrl
            );
        }
    }
}
