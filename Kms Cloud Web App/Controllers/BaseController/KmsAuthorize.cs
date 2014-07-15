using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kms.Cloud.WebApp.Controllers {
    /// <summary>
    ///     Este "proxy" del atributo "Authorize" de MVC es necesario para los casos en los que
    ///     un Usuario visita el Dashboard pero su cuenta ha sido eliminada. La sesión sigue
    ///     siendo válida y pasa todos los filtros de MVC correctamente, pero genera un error
    ///     al momento de buscar cualquier información.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class KmsAuthorize : AuthorizeAttribute {
        public override void OnAuthorization(AuthorizationContext filterContext) {
            var baseController = filterContext.Controller as BaseController;

            if ( baseController == null || baseController.CurrentUser != null )
                base.OnAuthorization(filterContext);
            else
                filterContext.Result = new RedirectResult("~/Login");
        }
    }
}