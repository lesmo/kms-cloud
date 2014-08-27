using System;
using System.Linq;
using System.Web.Mvc;
using Kms.Cloud.Database;
using System.Text;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace Kms.Cloud.Api.Areas.Login.Controllers {
    public class HomeController : Controller {
        private Kms.Cloud.Database.Abstraction.WorkUnit Database
            = new Kms.Cloud.Database.Abstraction.WorkUnit();

        private void BasicAuthChallengeResponse() {
            Response.StatusCode
                = 401;
            Response.StatusDescription
                = "Unauthorized";
            Response.AddHeader(
                "WWW-Authenticate",
                "Basic realm=\"" + WebApiConfig.KmsOAuthConfig.ApiRealm + "\""
            );

            Response.End();
        }

        /// <summary>
        /// Permite al usuario KMS iniciar sesión con KMS utilizando una interfaz Web
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "oauth")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "token")]
        [HttpGet, HttpPost]
        public ActionResult LogInUI(string oauth_token) {
            // TODO: Buscar cookie de sesión para
            return View();
        }

        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "oauth")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "token")]
        public ActionResult BasicLogin(string oauth_token) {
            // --- Validar cabecera Authorization ---
            Guid oAuthTokenGuid
                = new Guid();

            if ( string.IsNullOrEmpty(oauth_token) || ! Guid.TryParse(oauth_token, out oAuthTokenGuid) ) {
                BasicAuthChallengeResponse();
                return View("OAuthTokenInvalid");
            }

            if (
                ! Request.Headers.AllKeys.Contains("Authorization")
                || string.IsNullOrEmpty(Request.Headers["Authorization"])
                || ! Request.Headers["Authorization"].StartsWith("Basic", StringComparison.OrdinalIgnoreCase)
            ) {
                BasicAuthChallengeResponse();
                return View("OAuthTokenInvalid");
            }

            string credentialsEmail, credentialsPassword;

            if ( Request.Headers["Authorization"].StartsWith("Basic", StringComparison.OrdinalIgnoreCase) ) {
                try {
                    string authorizationLine
                        = Request.Headers["Authorization"].Split(
                            new char[]{' '},
                            2
                        )[1];
                    authorizationLine
                        = Encoding.ASCII.GetString(
                            Convert.FromBase64String(authorizationLine)
                        );

                    string[] authorizationValues
                        = authorizationLine.Split(
                            new char[]{':'},
                            2
                        );

                    credentialsEmail
                        = authorizationValues[0];
                    credentialsPassword
                        = authorizationValues[1];
                } catch ( Exception ex ) {
                    if ( ex is ArgumentException || ex is IndexOutOfRangeException ) {
                        BasicAuthChallengeResponse();
                        return View();
                    } else {
                        throw new InvalidOperationException(
                            "Uncaught exception occurred during Basic Authentication parsing",
                            ex
                        );
                    }
                }
            } else {
                BasicAuthChallengeResponse();
                return View("OAuthTokenInvalid");
            }
            
            // --- Buscar Token y validarlo ---
            Token token
                = Database.TokenStore.Get(oAuthTokenGuid);
            
            if ( token == null ) {
                Response.StatusCode
                    = 403;
                Response.StatusDescription
                    = "Forbidden";
                return View("OAuthTokenInvalid");
            }
            
            if ( ! token.ApiKey.BasicLoginEnabled ) {
                Response.StatusCode
                    = 403;
                Response.StatusDescription
                    = "Forbidden";
                return View("OAuthTokenInvalid");
            }

            if ( token.LoginAttempts > 10 ) {
                Response.StatusCode
                    = 403;
                Response.StatusDescription
                    = "Forbidden";

                Database.TokenStore.Delete(token.Guid);

                return View("OAuthLoginAttemptsExceeded");
            }

            // --- Buscar Usuario y comparar contraseña ---
            User user
                = Database.UserStore.GetFirst(
                    f => f.Email == credentialsEmail
                );

            if ( user == null ) {
                token.LoginAttempts++;
                Database.TokenStore.Update(token);
                Database.SaveChanges();

                BasicAuthChallengeResponse();
            } else if (
                !user.PasswordMatches(credentialsPassword)
            ) {
                token.LoginAttempts++;
                Database.TokenStore.Update(token);
                Database.SaveChanges();

                BasicAuthChallengeResponse();
            }

            // --- Autorizar Token y redirigir a CallbackUri ---
            token.User
                = user;
            token.LastUseDate
                = DateTime.UtcNow;
            token.VerificationCode
                = Guid.NewGuid();

            Database.TokenStore.Update(token);
            Database.SaveChanges();

            string callbackUri
                = token.CallbackUri ?? "http://api.kms.me/oauth/nocallback#";

            if ( callbackUri == "oob" ) {
                ViewData.Add(
                    "oob_verifier",
                    token.VerificationCode.Value.ToString("N")
                );
                return View("OAuthBasicLoginSuccess");
            } else {
                if ( !callbackUri.EndsWith("?", StringComparison.OrdinalIgnoreCase) && !callbackUri.EndsWith("#", StringComparison.OrdinalIgnoreCase) )
                    callbackUri += "?";

                return Redirect(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}oauth_token={1}&oauth_verifier={2}",
                        token.CallbackUri,
                        token.Guid.ToString("N"),
                        token.Secret.ToString("N")
                    )
                );
            }
        }

        [HttpGet]
        [Route("oauth/nocallback")]
        public ActionResult NoCallback() {
            return View();
        }
    }
}
