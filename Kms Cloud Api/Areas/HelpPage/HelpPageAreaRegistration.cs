using System.Web.Http;
using System.Web.Mvc;

namespace Kms.Cloud.Api.Areas.HelpPage {
    public class HelpPageAreaRegistration : AreaRegistration {
        public override string AreaName {
            get {
                return "HelpPage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "HelpPage_OAuth",
                "docs/oauth",
                new {
                    controller = "StaticPages",
                    action = "OAuth"
                }
            );

            context.MapRoute(
                "HelpPage_Static",
                "docs/static/{action}",
                new {
                    controller = "StaticPages",
                    action = "Index"
                }
            );

            context.MapRoute(
                "HelpPage_Default",
                "docs/{action}/{apiId}",
                new { controller = "Help", action = "Index", apiId = UrlParameter.Optional }
            );

            HelpPageConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}