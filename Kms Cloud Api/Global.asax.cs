using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Kms.Cloud.Api {
    public class WebApiApplication : System.Web.HttpApplication {
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        protected void Application_Start() {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AreaRegistration.RegisterAllAreas();
        }

        /// <summary>
        ///     Devuelve la Versión del Ensamblado del API de KMS.
        /// </summary>
        public static Version AssemblyVersion {
            get {
                if ( WebApiApplication._assemblyVersion != null )
                    return WebApiApplication._assemblyVersion;

                var controllerType     = typeof(Controllers.AccountCreateController);
                var controllerAssembly = Assembly.GetAssembly(controllerType);
                var assemblyName       = controllerAssembly.GetName();

                WebApiApplication._assemblyVersion = assemblyName.Version;
                return WebApiApplication._assemblyVersion;
            }
        }

        private static Version _assemblyVersion;
    }
}
