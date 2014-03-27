using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KilometrosDatabase;
using System.Security.Cryptography;
using System.Text;

namespace Kilometros_WebAPI.Areas.Login.Controllers {
    public class HomeController : Controller {
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();

        private void BasicAuthChallengeResponse() {
            Response.StatusCode
                = 401;
            Response.StatusDescription
                = "Unauthorized";
            Response.AddHeader(
                "WWW-Authenticate",
                "Basic realm=\"" + WebApiConfig.KmsOAuthConfig.GuiRealm + "\""
            );

            Response.End();
        }

        /// <summary>
        /// Permite al usuario KMS iniciar sesión con KMS utilizando una interfaz Web
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public ActionResult LoginGui(string oauth_token) {
            // TODO: Buscar cookie de sesión para
            return View();
        }
        
        public ActionResult LoginBasic(string oauth_token = null) {
            // --- Validar cabecera Authorization ---
            Guid oAuthTokenGuid
                = new Guid();

            if ( oauth_token == null || ! Guid.TryParse(oauth_token, out oAuthTokenGuid) ) {
                BasicAuthChallengeResponse();
                return View("OAuthTokenInvalid");
            }

            if (
                ! Request.Headers.AllKeys.Contains("Authorization")
                || string.IsNullOrEmpty(Request.Headers["Authorization"])
                || ! Request.Headers["Authorization"].StartsWith("Basic")
            ) {
                BasicAuthChallengeResponse();
                return View("OAuthTokenInvalid");
            }

            string credentialsEmail
                = "";
            byte[] credentialsPasswordBytes
                = null;

            if ( Request.Headers["Authorization"].StartsWith("Basic") ) {
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
                    string credentialsPassword
                        = authorizationValues[1];
                    
                    SHA256 sha256
                        = new SHA256CryptoServiceProvider();
                    credentialsPasswordBytes
                        =  sha256.ComputeHash(
                            Encoding.ASCII.GetBytes(credentialsPassword)
                        );
                    credentialsPasswordBytes
                        = sha256.ComputeHash(credentialsPasswordBytes);
                } catch {
                    BasicAuthChallengeResponse();
                    return View();
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
                ! Helpers.MiscHelper.BytesEqual(user.Password, credentialsPasswordBytes)
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
                if ( !callbackUri.EndsWith("?") && !callbackUri.EndsWith("#") )
                    callbackUri += "?";

                return Redirect(
                    string.Format(
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
