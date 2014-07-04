using Kms.Cloud.Database;
using Kms.Cloud.Database.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Kms.Cloud.WebApp.Controllers {
	public class LoginController : BaseController {
		// GET: /Login/
		public ActionResult Index() {
			return View();
		}

		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		public ActionResult Auto(string k, string h) {
			// Parsear Key de Token de Auto-Login
			Int64 key;

			try {
				key = new Base36Encoder().Decode(k);
			} catch {
				return Redirect("~/Login");
			}

			// Buscar Token de Auto-Login por su llave
			var autologinToken = Database.WebAutoLoginTokenStore.GetFirst(
				filter: f =>
					f.Key == key,
				orderBy: o =>
					o.OrderBy(b => b.CreationDate)
			);

			if ( autologinToken == null )
				return Redirect("~/Login");

			// Determinar si aún es vigente el Token y la IP de origen coincide
			if (
				autologinToken.CreationDate < DateTime.UtcNow.AddMinutes(-2)
				|| autologinToken.IPAddress != Request.UserHostAddress
			) {
				Database.WebAutoLoginTokenStore.Delete(autologinToken.Id);
				Database.SaveChanges();

				return Redirect("~/Login");
			}

			// Obtener Consumer Secret y Token Secret
			var consumerSecret = autologinToken.Token.Secret.ToString("N");
			var tokenSecret    = autologinToken.Token.ApiKey.Secret.ToString("N");

			// Calcular hash HMAC-SHA1 de Token
			var hmacSha1Key = consumerSecret + "&" + tokenSecret;
			var hmacSha1    = new HMACSHA1(Encoding.UTF8.GetBytes(hmacSha1Key));
			var hmacSha1Bytes = hmacSha1.ComputeHash(
				Encoding.UTF8.GetBytes(autologinToken.Secret.ToString("N"))
			);
			var hmacSha1String = new StringBuilder(hmacSha1Bytes.Length * 2);

			for ( int i = 0; i < hmacSha1Bytes.Length; i++ )
				hmacSha1String.Append(hmacSha1Bytes[i].ToString("x2"));

			// Validar que {s} coincida con Hash calculado
			if ( hmacSha1String.ToString().ToUpper() != h.ToUpper() ) {
				Database.WebAutoLoginTokenStore.Delete(autologinToken.Id);
				Database.SaveChanges();

				return Redirect("~/Login");
			}

			// Crear sesión y eliminar Token de Auto-Login
			var user = autologinToken.Token.User;
			FormsAuthentication.SetAuthCookie(user.Guid.ToBase64String(), true);

			Database.WebAutoLoginTokenStore.Delete(autologinToken.Id);
			Database.SaveChanges();

			// Redirigir a Dashboard
			return Redirect("~/Overview");

		}

		// POST: /Login/
		// TODO: Re-enable AntiForgeryToken check
		//[ValidateAntiForgeryToken]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult CreateSession(string email, string password, string returnUrl = null) {
			if ( User.Identity.IsAuthenticated )
				return Redirect("~/Overview");

			// > Buscar al Usuario en BD por su Email
			email = email.ToLower();
			var user = Database.UserStore.GetFirst(
				filter: f =>
					f.Email == email.ToLower()
			);

			// > Validar que el Usuario exista y las contraseñas coincidan
			if ( user == null || ! user.PasswordMatches(password) )
				return Redirect("http://www.kms.me/#loginfail");
			
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