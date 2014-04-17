using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Kilometros_WebAPI.Models.RequestModels;
using Kilometros_WebAPI.Models.ResponseModels;
using Kilometros_WebAPI.Security;
using KilometrosDatabase;
using System.Net;
using System.Globalization;
using Kilometros_WebGlobalization.API;
using Kilometros_WebAPI.Helpers;
using System.Web;

namespace Kilometros_WebAPI.Controllers {
    /// <summary>
    ///     Devuelve y modifica la Ínformación de la Cuenta de Usuario en la Nube KMS.
    /// </summary>
    [Authorize]
    public class AccountController : IKMSController {
        /// <summary>
        ///     Devuelve la Información de Cuenta de Usuario en la Nube KMS.
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpGet]
        [Route("my/account")]
        public AccountResponse GetAccount() {
            // TODO: Añadir funcionalidad de If-Modified-Since
            return new AccountResponse() {
                AccountCreationDate
                    = CurrentUser.CreationDate,
                UserId
                    = CurrentUser.Guid.ToBase64String(),
                Name
                    = CurrentUser.Name,
                LastName
                    = CurrentUser.LastName,
                Email
                    = CurrentUser.Email,
                PreferredCultureCode
                    = CurrentUser.PreferredCultureInfo.Name,
                RegionCode
                    = CurrentUser.RegionCode
            };
        }

        /// <summary>
        ///     Establece la Información de Cuenta de Usuario en la Nube KMS.
        /// </summary>
        /// <param name="accountPost">
        ///     Nueva Información de Cuenta de Usuario en la Nube KMS.
        /// </param>
        /// <returns>HTTP 200 OK</returns>
        [HttpPost]
        [Route("my/account")]
        public HttpResponseMessage PostAccount([FromBody]AccountPost accountPost) {
            CurrentUser.PreferredCultureCode
                = accountPost.PreferredCultureCode.ToLowerInvariant();
            CurrentUser.RegionCode
                = accountPost.RegionCode.ToLowerInvariant();
            CurrentUser.Email
                = accountPost.Email.ToLowerInvariant();
            
            Database.UserStore.Update(CurrentUser);
            Database.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}