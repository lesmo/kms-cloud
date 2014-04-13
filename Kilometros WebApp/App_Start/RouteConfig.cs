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
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new {
                    controller = "Entry",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "DynamicResources",
                url: "DynamicResources/{action}/{filename}.{ext}",
                defaults: new {
                    controller = "DynamicResources"
                }
            );

            routes.MapRoute(
                name: "DynamicResources",
                url: "DynamicResources/Ajax/{action}.json",
                defaults: new {
                    controller = "Ajax"
                }
            );
        }
    }
}
