using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Kms.Cloud.WebApp.Controllers {
	public class LoginController : BaseController {
		// GET: /Login/
		public ActionResult Index() {
			return View();
		}

		// POST: /Login/
		[ValidateAntiForgeryToken]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult CreateSession(string email, string password, string returnUrl = null) {
			if ( User.Identity.IsAuthenticated )
				return Redirect("~/Overview");

			// > Buscar al Usuario en BD por su Email
			email
				= email.ToLower();
			User user
				= Database.UserStore.GetFirst(
					filter: f =>
						f.Email == email.ToLower()
				);

			// > Validar que el Usuario exista y las contraseñas coincidan
			if ( user == null || ! user.PasswordMatches(password) )
				return View("Index", true);

			// > Crear sesión y redirigir a donde aplique
			FormsAuthentication.SetAuthCookie(user.Guid.ToBase64String(), true);
			
			if ( returnUrl == null )
				return RedirectToAction("Index", "Overview");
			else
				return Redirect(returnUrl);
		}

		public ActionResult Facebook(string oauth_token) {
			return View();
		}

		public ActionResult Twitter() {
			return View();
		}
	}
}