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

        private void Application_BeginRequest(object sender, EventArgs e) {
            HttpContext.Current.Items.Add(
                "Database",
                new Kms.Cloud.Database.Abstraction.WorkUnit()
            );
        }

        /// <summary>
        ///     Devuelve la Versión del Ensamblado del API de KMS.
        /// </summary>
        public static Version AssemblyVersion {
            get {
                if ( WebApiApplication._assemblyVersion == null )
                    WebApiApplication.LoadAssemblyMeta();
                
                return WebApiApplication._assemblyVersion;
            }
        }

        public static String AssemblyName {
            get {
                if ( string.IsNullOrEmpty(WebApiApplication._assemblyName) )
                    WebApiApplication.LoadAssemblyMeta();

                return WebApiApplication._assemblyName;
            }
        }

        private static void LoadAssemblyMeta() {
            var controllerType     = typeof(Controllers.AccountCreateController);
            var controllerAssembly = Assembly.GetAssembly(controllerType);
            var assemblyName       = controllerAssembly.GetName();
            
            WebApiApplication._assemblyName = assemblyName.Name;
            WebApiApplication._assemblyVersion = assemblyName.Version;
        }
        
        private static String _assemblyName;
        private static Version _assemblyVersion;
    }
}
