using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kms.Cloud.WebApp {
    public class RouteConfig {
        const string LanguageValidation
            = @"[a-zA-Z]{2}-[a-zA-Z]{2}";

        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                "LoginWeb",
                "Login/Web",
                new {
                    controller = "Login",
                    action = "Web"
                }
            );

            routes.MapRoute(
                "LoginAuto",
                "Login/Auto",
                new {
                    controller = "Login",
                    action = "Auto"
                }
            );

            routes.MapRoute(
                "DynamicResources",
                "{controller}/{action}/{filename}.{ext}"
            );

            routes.MapTranslatedRoute(
                "AjaxGlobalization",
                "{lang}/DynamicResources/Ajax/{action}.json",
                new {
                    controller = "Ajax"
                },
                null,
                new {
                    lang = LanguageValidation
                }
            );

            routes.MapTranslatedRoute(
                "Ajax",
                "DynamicResources/Ajax/{action}.json",
                new {
                    controller = "Ajax"
                },
                null
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
