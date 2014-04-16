using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kilometros_WebApp {
    public class RouteConfig {
        const string LanguageValidation
            = @"[a-zA-Z]{2}-[a-zA-Z]{2}";
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                "DynamicResources",
                "{controller}/{action}/{filename}.{ext}"
            );
            
            routes.MapTranslatedRoute(
                "DefaultGlobalization",
                "{lang}/{controller}/{action}",
                new {
                    controller = "Overview",
                    action = "index"
                },
                new {
                    controller = RouteTranslationConfig.ControllerGlobalization
                },
                new {
                    lang = LanguageValidation
                }
            );

            routes.MapTranslatedRoute(
                "Default",
                "{controller}/{action}",
                new {
                    controller = "Overview",
                    action = "index"
                },
                new {
                    controller = RouteTranslationConfig.ControllerGlobalization
                }
            );
        }
    }
}
