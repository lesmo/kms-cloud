using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Kms.Cloud.Api.Models.RequestModels;
using Kms.Cloud.Api.Models.ResponseModels;
using Kms.Cloud.Api.Security;
using Kms.Cloud.Database;
using System.Net;
using System.Globalization;
using Kilometros_WebGlobalization.API;
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace Kms.Cloud.Api.Controllers {
    /// <summary>
    ///     Obtener y actualizar la Ínformación de una Cuenta de Usuario en la Nube KMS.
    /// </summary>
    [Authorize]
    public class AccountController : BaseController {
        /// <summary>
        ///     Devuelve la Información de la Cuenta de Usuario en la Nube KMS de la sesión actual.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [HttpGet]
        [Route("my/account")]
        public AccountResponse GetAccount() {
            // TODO: Añadir funcionalidad de If-Modified-Since
            return new AccountResponse() {
                AccountCreationDate
                    = CurrentUser.CreationDate,
                PictureUri
                    = CurrentUser.UserPicture == null
                    ? null
                    : GetDynamicResourceUri(CurrentUser.UserPicture),
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
        ///     Actualiza la Información de Cuenta de Usuario en la Nube KMS de la sesión actual.
        /// </summary>
        /// <param name="accountPost">
        ///     Nueva Información de Cuenta de Usuario en la Nube KMS.
        /// </param>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        [HttpPost, Route("my/account")]
        public HttpResponseMessage PostAccount([FromBody]AccountPost accountPost) {
            // TODO: Change user password!
            // TODO: Validate email address.

            CurrentUser.PreferredCultureCode
                = accountPost.PreferredCultureCode.ToUpper(CultureInfo.InvariantCulture);
            CurrentUser.RegionCode
                = accountPost.RegionCode.ToUpper(CultureInfo.InvariantCulture);
            CurrentUser.Email
                = accountPost.Email.ToLower(CultureInfo.InvariantCulture);
            
            Database.UserStore.Update(CurrentUser);
            Database.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}