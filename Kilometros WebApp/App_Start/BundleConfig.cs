using System.Web;
using System.Web.Optimization;

namespace Kilometros_WebApp {
    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/lib/jquery").Include(
                "~/FrontendBundle/Scripts/Lib/jquery-{version}.js"
            ));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/lib/modernizr").Include(
            //            "~/FrontendBundle/Scripts/Lib/modernizr-*"));
            
            bundles.Add(new StyleBundle("~/css").Include(
                // 960.gs
                "~/FrontendBundle/Styles/Lib/960_16_col.css",
                "~/FrontendBundle/Styles/Lib/reset.css",
                "~/FrontendBundle/Styles/Lib/text.css", // se eliminó el selector "body" para evitar conflicto con {Shared.css}

                // KMS
                "~/FrontendBundle/Styles/Shared.css",
                "~/FrontendBundle/Styles/Navigation.css"
            ));
        }
    }
}
