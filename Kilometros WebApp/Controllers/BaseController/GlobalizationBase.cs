using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kilometros_WebApp.Controllers {
    public abstract partial class BaseController : Controller {
        // > Preparar búsqueda de segmento de Globalización en URL
        private Regex UrlCultureCodeRegex
            = new Regex(@"/([a-zA-Z]{2})-([a-zA-Z]{2})/?");
        
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
            // > Obtener la Cultura de donde sea aplicable
            CultureInfo culture
                = GetCultureFromUrl()
                ?? GetCultureFromCookie()
                ?? GetCultureFromUserPreferences()
                ?? GetCultureFromUserAgent()
                ?? new CultureInfo(SupportedCultures.Cultures.First());

            // > Verificar que la cultura esté catalogada como soportada
            if ( ! SupportedCultures.Cultures.Any(a => a.ToLower() == culture.Name) )
                culture = new CultureInfo(SupportedCultures.Cultures.First());

            // > Establecer/Actualizar Cookie con el nuevo Código de Cultura
            Response.Cookies.Set(
                new HttpCookie("KMS.Culture", culture.Name) {
                    Expires
                        = DateTime.Now.AddYears(1)
                }
            );

            // > Si se determinó una Cultura y la URL no lo refleja, redirigir a la URL que
            //   lo refleja.
            if ( RouteData.Values["lang"] == null ) {
                Response.Redirect(
                    new Uri(
                        Request.Url,
                        "/" + culture.Name + Request.Url.PathAndQuery
                    ).AbsoluteUri
                );

                return null;
            } else if ( ((string)RouteData.Values["lang"]).ToLower() != culture.Name.ToLower() ) {
                string tmp
                    = UrlCultureCodeRegex.Replace(
                        Request.Url.AbsoluteUri,
                        "/" + culture.Name
                    );
                Response.Redirect(
                    tmp,
                    true
                );

                return null;
            }

            // > Establecer {lang} a la forma "pretty" de la Cultura
            RouteData.Values["lang"]
                = culture.Name;

            return culture;
        }

        protected override void Initialize(RequestContext request) {
            base.Initialize(request);

            CultureInfo culture
                = GetUserCultureInfo();

            if ( culture == null ) {
                Response.End();
                return;
            }

            Thread.CurrentThread.CurrentCulture
                = culture;
            Thread.CurrentThread.CurrentUICulture
                = culture;

            // > Es necesario limpiar caché para que {RegionInfo.CurrentRegion}
            //   contenga la información regional actualizada, y no la del servidor
            Thread.CurrentThread.CurrentCulture.ClearCachedData();
            Thread.CurrentThread.CurrentUICulture.ClearCachedData();
        }
    }
}