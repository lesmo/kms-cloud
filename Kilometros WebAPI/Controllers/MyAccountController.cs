using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Kilometros_WebAPI.Models.HttpGet.My_Controllers;
using Kilometros_WebAPI.Models.HttpPost.My_Controller;
using Kilometros_WebAPI.Security;
using KilometrosDatabase;
using System.Net;
using System.Globalization;
using Kilometros_WebGlobalization.API;
using Kilometros_WebAPI.Helpers;
using System.Web;

namespace Kilometros_WebAPI.Controllers {
    /// <summary>
    /// Permite obtener y modificar la Ínformación de la Cuenta de Usuario.
    /// </summary>
    [Authorize]
    public class MyAccountController : ApiController {
        /// <summary>
        /// Acceso a los Repositorios de la BD.
        /// </summary>
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();

        /// <summary>
        /// Devuelve la Información de Cuenta de Usuario del Usuario.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("my/account")]
        public AccountResponse GetAccount() {
            // TODO: Añadir funcionalidad de If-Modified-Since

            KmsIdentity identity
                = MiscHelper.GetPrincipal<KmsIdentity>();
            User user
                = identity.UserData;
            
            return new AccountResponse() {
                AccountCreationDate
                    = user.CreationDate,
                Email
                    = user.Email,
                PreferredCultureCode
                    = user.PreferredCultureInfo.Name,
                RegionCode
                    = user.RegionCode
            };
        }

        /// <summary>
        /// Establece la Información de Contacto del Usuario.
        /// </summary>
        /// <param name="accountPost">Información de Contacto nueva.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("my/account")]
        public IHttpActionResult PostAccount([FromBody]AccountPost accountPost) {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;
            
            user.PreferredCultureCode
                = accountPost.PreferredCultureCode.ToLowerInvariant();
            user.RegionCode
                = accountPost.RegionCode.ToLowerInvariant();
            user.Email
                = accountPost.Email.ToLowerInvariant();
            
            this.Database.UserStore.Update(user);
            this.Database.SaveChanges();

            return Ok();
        }
    }
}