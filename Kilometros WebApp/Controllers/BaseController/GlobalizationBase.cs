using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Kilometros_WebApp.Controllers {
    public abstract partial class BaseController : Controller {
        private CultureInfo GetCultureFromUserAgent() {
            if ( Request.UserLanguages.Length == 0 )
                return null;

            try {
                return new CultureInfo(Request.UserLanguages[0]);
            } catch ( CultureNotFoundException ) {
                return null;
            }
        }

        private CultureInfo GetCultureFromCookie() {
            HttpCookie cookie
                = HttpContext.Request.Cookies["KMS.Culture"];

            if ( cookie == null )
                return null;

            try {
                return new CultureInfo(cookie.Value);
            } catch ( CultureNotFoundException ) {
                return null;
            }
        }

        private CultureInfo GetCultureFromUrl() {
            if ( RouteData.Values["lang"] == null )
                return null;

            try {
                return new CultureInfo((string)RouteData.Values["lang"]);
            } catch ( CultureNotFoundException ) {
                return null;
            }
        }

        private CultureInfo GetCultureFromUserPreferences() {
            if ( CurrentUser == null )
                return null;
            else
                return CurrentUser.PreferredCultureInfo;
        }

        private CultureInfo GetUserCultureInfo() {
            // > Preparar búsqueda de segmento de Globalización en URL
            Regex cultureCodeRegex
                = new Regex(@"/[a-zA-Z]{2}-[a-zA-Z]{2}/?");
            
            // > Obtener la Cultura de donde sea aplicable
            CultureInfo culture
                = GetCultureFromUrl()
                ?? GetCultureFromCookie()
                ?? GetCultureFromUserPreferences()
                ?? GetCultureFromUserAgent()
                ?? new CultureInfo("en-GB");

            // > Establecer/Actualizar Cookie con el nuevo Código de Cultura
            Response.Cookies.Set(
                new HttpCookie("KMS.Culture", culture.Name) {
                    Expires
                        = DateTime.Now.AddYears(1)
                }
            );

            // > Si se determinó una Cultura diferente a la de la URL, re-dirigir a la URL con
            //   el Código de Cultura correcto
            if (
                RouteData.Values["lang"] == null
                || ((string)RouteData.Values["lang"]).ToLowerInvariant() != culture.Name.ToLowerInvariant()
            ) {
                Response.Redirect(
                    cultureCodeRegex.Replace(
                        Request.Url.AbsoluteUri,
                        culture.Name
                    ),
                    true
                );

                return null;
            }

            // > Establecer {lang} a la forma "pretty" de la Cultura
            RouteData.Values["lang"]
                = culture.Name;

            return culture;
        }

        protected override void ExecuteCore() {
            CultureInfo culture
                = GetUserCultureInfo();

            Thread.CurrentThread.CurrentCulture
                = culture;
            Thread.CurrentThread.CurrentUICulture
                = culture;
            
            base.ExecuteCore();
        }
    }
}