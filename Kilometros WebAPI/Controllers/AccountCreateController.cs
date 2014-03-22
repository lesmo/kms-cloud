using Kilometros_WebAPI.Exceptions;
using Kilometros_WebAPI.Security;
using Kilometros_WebGlobalization.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using KilometrosDatabase;
using Kilometros_WebAPI.Models.ResponseModels;
using Kilometros_WebAPI.Models.RequestModels;

namespace Kilometros_WebAPI.Controllers {
    /// <summary>
    ///     Permite generar una nueva Cuenta en la Nube de KMS. Para crear una nueva cuenta
    ///     que permitirá login con Facebook, Twitter, Fitbit o Nike+, será necesario crear
    ///     una cuenta en éste recurso y posteriormente utilizar el apropiado en OAuth3rdPartyAdd.
    /// </summary>
    public class AccountCreateController : ApiController {
        KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();

        /// <summary>
        ///     Crea una nueva Cuenta en la Nube KMS.
        /// </summary>
        /// <param name="dataPost">
        ///     Información de la nueva cuenta de Usuario.
        /// </param>
        /// <returns>
        ///     
        /// </returns>
        [HttpPost]
        [Route("account")]
        public IHttpActionResult CreateKmsAccount([FromBody]CreateKmsAccountPost dataPost) {
            // --- Validar que no haya sesión iniciada ---
            KmsIdentity identity
                = (KmsIdentity)User.Identity;

            if ( identity.UserData != null )
                throw new HttpAlreadyLoggedInException(
                    "201" + ControllerStrings.Warning201_CannotCreateUserWithSessionOpen
                );

            // --- Validar que no haya un usuario con el mismo Email ---
            User userSearch
                =  Database.UserStore.GetFirst(
                    f => f.Email == dataPost.Email.ToLower()
                );

            if ( userSearch != null )
                throw new HttpConflictException(
                    "206" + ControllerStrings.Warning206_CannotCreateUserWithEmail
                );

            // --- Crear cuenta de Usuario ---
            User user = new User() {
                Name
                    = dataPost.Name,
                LastName
                    = dataPost.LastName,

                Email
                    = dataPost.Email.ToLower(),
                PasswordString
                    = dataPost.Password,

                PreferredCultureCode
                    = dataPost.CultureCode,
                RegionCode
                    = dataPost.RegionCode,
                UtcOffset
                    = dataPost.UtcOffset
            };
            
            Database.UserStore.Add(user);
            Database.SaveChanges();

            // --- Devolver respuesta ---
            return Ok();
        }
    }
}
