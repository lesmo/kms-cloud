using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kilometros_WebApp {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name:
                    "Default",
                url:
                    "{controller}/{action}/{id}",
                defaults:
                    new {
                        controller = "Entry",
                        action = "Index",
                        id = UrlParameter.Optional
                    }
            );

            routes.MapRoute(
                name:
                    "DefaultGlobalization",
                url:
                    "{lang}/{controller}/{action}/{id}",
                defaults:
                    new {
                        controller = "Entry",
                        action = "Index",
                        id = UrlParameter.Optional
                    },
                constraints:
                    new {
                        lang = @"\w{2,3}(-\w{4})?(-\w{2,3})?"
                    }
            );

            routes.MapRoute(
                name:
                    "DynamicResources",
                url:
                    "DynamicResources/{action}/{filename}.{ext}",
                defaults: new {
                    controller = "DynamicResources"
                }
            );

            routes.MapRoute(
                name:
                    "DynamicResourcesGlobalization",
                url:
                    "{lang}/DynamicResources/{action}/{filename}.{ext}",
                defaults: new {
                    controller = "DynamicResources"
                },
                constraints:
                    new {
                        lang = @"\w{2,3}(-\w{4})?(-\w{2,3})?"
                    }
            );

            routes.MapRoute(
                name: "DynamicResourcesAjax",
                url: "DynamicResources/Ajax/{action}.json",
                defaults: new {
                    controller = "Ajax"
                }
            );

            routes.MapRoute(
                name:
                    "DynamicResourcesAjaxGlobalization",
                url:
                    "{lang}/DynamicResources/Ajax/{action}.json",
                defaults:
                    new {
                        controller = "Ajax"
                    },
                constraints:
                    new {
                        lang = @"\w{2,3}(-\w{4})?(-\w{2,3})?"
                    }
            );
        }
    }
}
