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
using Kilometros_WebAPI.Exceptions;

namespace Kilometros_WebAPI.Controllers {
    public class RegisterController : ApiController {
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();
        
        [HttpPost]
        [Route("register/kms")]
        public IHttpActionResult RegisterKms([FromBody]RegisterKmsPost registerPost) {
            // --- Evitar que Usuarios que ya tienen una sesión registren usuarios ---
            if ( this.User.Identity.IsAuthenticated )
                throw new HttpAlreadyLoggedInException(
                    ControllerStrings.Warning201_CannotCreateUserWithSessionOpen
                );

            // --- Obtener Hash de Contraseña ---
            byte[] passwordBytes
                = Convert.FromBase64String(registerPost.Password);

            // --- Generar segundo Hash de Contraseña ---
            SHA256 sha256
                = new SHA256CryptoServiceProvider();
            byte[] passwordHashBytes
                = sha256.ComputeHash(passwordBytes);

            // --- Guardar nueva cuente de Usuario y enviar emails de verificación ---
            User newUser
                = new User() {
                    Email = registerPost.Email,
                    Password = passwordHashBytes,
                    PreferredCultureCode = registerPost.PreferredCultureCode
                };

            this.Database.UserStore.Add(newUser);
            this.Database.SaveChanges();
            
            // --- Reportar éxito ---
            return Ok();
        }

        [HttpPost]
        [Route("register/facebook")]
        public HttpResponseMessage RegisterTwitter() {
            throw new NotImplementedException();
        }
    }
}
