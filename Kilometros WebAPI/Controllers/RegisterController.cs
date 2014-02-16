using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Kilometros_WebAPI.Models.HttpPost.RegisterController;
using System.Web;
using Kilometros_WebGlobalization.API;
using KilometrosDatabase;
using System.Data.Entity.Validation;
using System.Security.Cryptography;

namespace Kilometros_WebAPI.Controllers {
    public class RegisterController : ApiController {
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();
        private HttpServerUtility _httpServerUtility
            = HttpContext.Current.Server;

        [HttpPost]
        [Route("register/kms")]
        public HttpResponseMessage RegisterKms([FromBody]RegisterKmsPost registerPost) {
            /** Evitar que Usuarios que ya tienen una sesión registren usuarios **/
            if ( this.User.Identity.IsAuthenticated ) {
                HttpResponseMessage response
                        = Request.CreateResponse(HttpStatusCode.Forbidden);
                string warningString
                    = string.Format(
                        ControllerStrings.Warning201_CannotCreateUserWithSessionOpen,
                        this.User.Identity.Name
                    );

                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "201" + this._httpServerUtility.UrlEncode(warningString)
                );

                return response;
            }

            /** Obtener Hash de Contraseña **/
            byte[] passwordBytes
                = new byte[]{};

            try {
                passwordBytes
                    = Convert.FromBase64String(registerPost.Password);
            } catch {
                HttpResponseMessage response
                    = Request.CreateResponse(HttpStatusCode.BadRequest);
                string warningString
                    = string.Format(ControllerStrings.Warning202_PasswordHashInvalid, registerPost.Password);

                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "202" + this._httpServerUtility.UrlEncode(warningString)
                );

                return response;
            }

            /** Generar segundo Hash de Contraseña **/
            SHA256 sha256
                = new SHA256CryptoServiceProvider();
            byte[] passwordHashBytes
                = sha256.ComputeHash(passwordBytes);

            /** Guardar nueva cuente de Usuario y enviar emails de verificación **/
            try {
                User newUser
                    = new User() {
                        Email = registerPost.Email,
                        Password = passwordHashBytes,
                        PreferredCultureCode = registerPost.PreferredCultureCode
                    };

                this.Database.UserStore.Add(newUser);
                this.Database.SaveChanges();
            } catch ( DbEntityValidationException exception ) {
                HttpResponseMessage response
                    = Request.CreateResponse(HttpStatusCode.BadRequest);
                string warningString
                    = string.Format(
                        ControllerStrings.GenericValidationError,
                        exception.EntityValidationErrors.FirstOrDefault().ValidationErrors.FirstOrDefault().ErrorMessage
                    );

                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "203" + this._httpServerUtility.UrlEncode(warningString)
                );

                return response;
            }

            /** Reportar éxito **/
            return Request.CreateResponse(
                HttpStatusCode.Created
            );
        }

        [HttpPost]
        [Route("register/facebook")]
        public HttpResponseMessage RegisterTwitter() {
        
        }
    }
}
