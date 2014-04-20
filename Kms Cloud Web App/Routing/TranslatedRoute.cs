using Kms.Cloud.WebApp;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace System.Web.Routing {
    public class TranslatedRoute : Route {
        public const string DetectedCultureKey
            = "__ROUTING_DETECTED_CULTURE";

        public bool SetDetectedCulture {
            get;
            set;
        }

        public RouteValueDictionary RouteValueTranslationProviders {
            get;
            private set;
        }
        
        private CultureInfo GetCultureFromString(string culture) {
            if ( string.IsNullOrEmpty(culture) )
                return null;

            try {
                return new CultureInfo(culture);
            } catch ( CultureNotFoundException ) {
                return null;
            }
        }

        public TranslatedRoute(
            string url,
            RouteValueDictionary defaults,
            RouteValueDictionary routeValueTranslationProviders,
            IRouteHandler routeHandler
        ) : base(url, defaults, routeHandler) {
            this.RouteValueTranslationProviders
                = routeValueTranslationProviders;
        }

        public TranslatedRoute(
            string url,
            RouteValueDictionary defaults,
            RouteValueDictionary routeValueTranslationProviders,
            RouteValueDictionary constraints,
            IRouteHandler routeHandler
        ) : base(url, defaults, constraints, routeHandler) {
            this.RouteValueTranslationProviders
                = routeValueTranslationProviders;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext) {
            RouteData routeData
                = base.GetRouteData(httpContext);

            if (routeData == null)
                return null;

            // > Determinar Cultura de la petición
            CultureInfo culture;

            try {
                culture
                    = this.GetCultureFromString(
                        (string)routeData.Values["lang"]
                    )
                    ?? this.GetCultureFromString(
                        (httpContext.Request.Cookies.Get("KMS.Culture") ?? new HttpCookie("KMS.Culture", null)).Value
                    )
                    ?? this.GetCultureFromString(
                        httpContext.Request.UserLanguages[0]
                    )
                    ?? new CultureInfo("en-GB");
            } catch {
                culture
                    = new CultureInfo("en-GB");
            }

            // > Verificar que la cultura esté catalogada como soportada
            if ( ! SupportedCultures.Cultures.Any(a => a.ToLower() == culture.Name.ToLower()) )
                culture
                    = new CultureInfo(SupportedCultures.Cultures.First());

            // > Si {routeData} no contiene {lang}, redirigir a la URL que sí lo contiene
            if ( routeData.Values["lang"] == null ) {
                httpContext.Response.Redirect(
                    new Uri(
                        httpContext.Request.Url,
                        "/" + culture.Name + httpContext.Request.Url.PathAndQuery
                    ).AbsoluteUri,
                    true
                );
                httpContext.Response.End();

                return null;
            }

            // > Si {routeData} no coincide con {lang}, redirigir a la URL con {lang}
            if ( ((string)routeData.Values["lang"]).ToLower() != culture.Name.ToLower() ) {
                string tmp
                    = new Regex(@"/([a-zA-Z]{2})-([a-zA-Z]{2})").Replace(
                        httpContext.Request.Url.AbsoluteUri,
                        "/" + culture.Name
                    );
                httpContext.Response.Redirect(
                    tmp,
                    true
                );
                httpContext.Response.End();

                return null;
            }

            // > Establecer cultura actual
            System.Threading.Thread.CurrentThread.CurrentCulture
                = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture
                = culture;

            // > Limpiar caché para que RegionInfo refleje cambios
            System.Threading.Thread.CurrentThread.CurrentCulture.ClearCachedData();
            System.Threading.Thread.CurrentThread.CurrentUICulture.ClearCachedData();

            // > Establecer cookie con Cultura actual
            httpContext.Response.Cookies.Set(
                new HttpCookie("KMS.Culture", CultureInfo.CurrentCulture.Name.ToLower())
            );

            // > Traducir otros elementos en la ruta
            foreach (KeyValuePair<string, object> pair in this.RouteValueTranslationProviders) {
                IRouteValueTranslationProvider translationProvider 
                    = pair.Value as IRouteValueTranslationProvider;

                if (
                    translationProvider != null
                    && routeData.Values.ContainsKey(pair.Key)
                ) {
                    RouteValueTranslation translation
                        = translationProvider.TranslateToRouteValue(
                            routeData.Values[pair.Key].ToString(),
                            CultureInfo.CurrentCulture
                        );

                    routeData.Values[pair.Key]
                        = translation.RouteValue;
                }
            }

            // > Devolver info de la ruta
            return routeData;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {
            RouteValueDictionary translatedValues
                = values;

            // > Establecer {lang}
            if ( ! values.ContainsKey("lang") )
                values.Add("lang", CultureInfo.CurrentCulture.Name);
            else if ( (string)values["lang"] != CultureInfo.CurrentCulture.Name )
                values["lang"] = CultureInfo.CurrentCulture.Name;

            // > Traducir valores de la Ruta
            foreach (KeyValuePair<string, object> pair in this.RouteValueTranslationProviders) {
                IRouteValueTranslationProvider translationProvider
                    = pair.Value as IRouteValueTranslationProvider;

                if (
                    translationProvider != null
                    && translatedValues.ContainsKey(pair.Key)
                ) {
                    RouteValueTranslation translation =
                        translationProvider.TranslateToTranslatedValue(
                            translatedValues[pair.Key].ToString(),
                            CultureInfo.CurrentCulture
                        );

                    translatedValues[pair.Key]
                        = translation.TranslatedValue;
                }
            }

            return base.GetVirtualPath(requestContext, translatedValues);
        }
    }
}