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
            User user
                = OAuth.Token.User;

            return new AccountResponse() {
                AccountCreationDate
                    = user.CreationDate,
                UserId
                    = MiscHelper.Base64FromGuid(user.Guid),
                Name
                    = user.Name,
                LastName
                    = user.LastName,
                Email
                    = user.Email,
                PreferredCultureCode
                    = user.PreferredCultureInfo.Name,
                RegionCode
                    = user.RegionCode
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
            User user
                = OAuth.Token.User;
            
            user.PreferredCultureCode
                = accountPost.PreferredCultureCode.ToLowerInvariant();
            user.RegionCode
                = accountPost.RegionCode.ToLowerInvariant();
            user.Email
                = accountPost.Email.ToLowerInvariant();
            
            Database.UserStore.Update(user);
            Database.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}