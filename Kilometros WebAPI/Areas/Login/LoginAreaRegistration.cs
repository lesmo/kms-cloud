using System.Web.Mvc;

namespace Kilometros_WebAPI.Areas.Login {
    public class LoginAreaRegistration : AreaRegistration  {
        public override string AreaName  {
            get  {
                return "Login";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "OAuth_LoginBasic",
                "oauth/authorize-basic",
                new {
                    controller = "Home",
                    action = "LoginBasic"
                }
            );
        }
    }
}