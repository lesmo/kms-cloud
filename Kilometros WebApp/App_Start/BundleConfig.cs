using System.Web;
using System.Web.Optimization;

namespace Kilometros_WebApp {
    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/js").Include(
                // jQuery + jQuery UI (Metro)
                "~/FrontendBundle/Scripts/Lib/jquery-1.10.2.js",
                "~/FrontendBundle/Scripts/Lib/jquery-ui-1.10.4.js",
                
                // Some other misc libraries
                "~/FrontendBundle/Scripts/Lib/respond.js",

                // KMS Scripts
                "~/FrontendBundle/Scripts/HeaderSearch.js"
            ));
            
            bundles.Add(new StyleBundle("~/css").Include(
                // 960.gs
                "~/FrontendBundle/Styles/Lib/960_16_col.css",
                "~/FrontendBundle/Styles/Lib/reset.css",
                "~/FrontendBundle/Styles/Lib/text.css", // se eliminó el selector "body" para evitar conflicto con {Shared.css}

                // jQuery UI (Metro)
                "~/FrontendBundle/Styles/Lib/jquery-ui.css",

                // KMS Common
                "~/FrontendBundle/Styles/Shared.css",

                // KMS Layout
                "~/FrontendBundle/Styles/Layout/Navigation.css",
                "~/FrontendBundle/Styles/Layout/Footer.css"
            ));
        }
    }
}
