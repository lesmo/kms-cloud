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
			return Redirect("http://www.kms.me/#login");
		}

		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		public ActionResult Auto(string k, string h, string d = null) {
			// Parsear Key de Token de Auto-Login
			Int64 key;

			try {
				key = new Base36Encoder().Decode(k);
			} catch {
				return Redirect("http://www.kms.me/#login");
			}

			// Buscar Token de Auto-Login por su llave
			var autologinToken = Database.WebAutoLoginTokenStore.GetFirst(
				filter: f =>
					f.Key == key,
				orderBy: o =>
					o.OrderBy(b => b.CreationDate)
			);

			if ( autologinToken == null )
				return Redirect("http://www.kms.me/#login");

			// Determinar si aún es vigente el Token y la IP de origen coincide
			if (
				autologinToken.CreationDate < DateTime.UtcNow.AddMinutes(-2)
				|| (
					autologinToken.IPAddress != null
					&& autologinToken.IPAddress != Request.UserHostAddress
				)
			) {
				Database.WebAutoLoginTokenStore.Delete(autologinToken.Id);
				Database.SaveChanges();

				return Redirect("http://www.kms.me/#login");
			}

			// Obtener Consumer Secret y Token Secret
			var consumerSecret = autologinToken.Token.ApiKey.Secret.ToString("N");
			var tokenSecret    = autologinToken.Token.Secret.ToString("N");

			// Calcular hash HMAC-SHA1 de Secreto de Token de Auto-Login
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

				return Redirect("http://www.kms.me/#login");
			}

			// Crear sesión y eliminar Token de Auto-Login
			var user = autologinToken.Token.User;
			FormsAuthentication.SetAuthCookie(user.Guid.ToBase64String(), true);

			Database.WebAutoLoginTokenStore.Delete(autologinToken.Id);

			if ( d == "token" )
				Database.TokenStore.Delete(autologinToken.Token);

			Database.SaveChanges();

			// Redirigir a Dashboard
			return Redirect("~/Overview");
		}

		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		public JsonResult Web(string email, string password, string nonce, string apikey) {
			// > Validar que los campos no vengan vacíos
			if ( String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password) || String.IsNullOrEmpty(password) || string.IsNullOrEmpty(apikey) )
				return Json(new {
					error = "A field is empty"
				}, JsonRequestBehavior.AllowGet);

			// > Validar que el API-Key sea válido
			var apiKey = Database.ApiKeyStore.Get(apikey);
			if ( apiKey == null || apiKey.BasicLoginEnabled == false )
				throw new HttpException(403, "Invalid API-Key");

			// > Buscar al Usuario en BD por su Email
			email = email.ToLower();
			var user = Database.UserStore.GetFirst(
				filter: f =>
					f.Email == email.ToLower()
			);

			// > Validar que el Usuario exista y las contraseñas coincidan
			if ( user == null || ! user.PasswordMatches(password) )
				return Json(new {
					error = "User not found"
				}, JsonRequestBehavior.AllowGet);

			// > Generar nuevo Token y WebAutoLoginToken
			var token = new Token {
				ApiKey = apiKey,
				ExpirationDate = DateTime.UtcNow.AddMinutes(-5),
				IPAddress = null,
				Secret = Guid.NewGuid(),
				User = user
			};

			var autologinToken = new WebAutoLoginToken {
				IPAddress = null,
				Key = (Int64)(new Random().NextDouble() * 10000000000000000000),
				Secret = Guid.NewGuid(),
				Token = token
			};

			// > Almacenar componentes en BD
			Database.TokenStore.Add(token);
			Database.WebAutoLoginTokenStore.Add(autologinToken);
			Database.SaveChanges();

			// > Calcular hash HMAC-SHA1 de Secreto de Token de Auto-Login
			var hmacSha1Key = apiKey.Secret + "&" + token.Secret;
			var hmacSha1 = new HMACSHA1(Encoding.UTF8.GetBytes(hmacSha1Key));
			var hmacSha1Bytes = hmacSha1.ComputeHash(
				Encoding.UTF8.GetBytes(autologinToken.Secret.ToString("N"))
			);
			var hmacSha1String = new StringBuilder(hmacSha1Bytes.Length * 2);

			for ( int i = 0; i < hmacSha1Bytes.Length; i++ )
				hmacSha1String.Append(hmacSha1Bytes[i].ToString("x2"));

			// > Devolver componentes de la URL
			return Json(new {
				k = new Base36Encoder().Encode(autologinToken.Key),
				h = hmacSha1String.ToString()
			}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult WebFacebook(string oauth_token) {
			return Redirect("http://www.kms.me/#loginfail");
		}

		public ActionResult WebTwitter() {
			return Redirect("http://www.kms.me/#loginfail");
		}
	}
}