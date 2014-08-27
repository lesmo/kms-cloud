using System;
using System.Net.Http;
using System.Web.Http;
using Kms.Cloud.Api.Models.RequestModels;
using Kms.Cloud.Api.Models.ResponseModels;
using Kms.Cloud.Database;
using System.Net;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

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

            var regionMetadata = Database.RegionStore.GetRegionMetadata(CurrentUser.RegionCode);
            var regionString   = "";
            
            if ( regionMetadata.Region != null && regionMetadata.RegionSubdivision != null ) {
                regionString = String.Format(
                    "{0}, {1}",
                    regionMetadata.RegionSubdivision.Name,
                    regionMetadata.Region.Name
                );
            } else {
                regionString = "Tikal";
            }

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
                FullName
                    = CurrentUser.FullName,
                Email
                    = CurrentUser.Email,
                PreferredCultureCode
                    = CurrentUser.PreferredCultureInfo.Name,
                RegionCode
                    = CurrentUser.RegionCode,
                RegionFull
                    = regionString
            };
        }

        /// <summary>
        ///     Actualiza la Información de Cuenta de Usuario en la Nube KMS de la sesión actual.
        /// </summary>
        /// <param name="accountPost">
        ///     Nueva Información de Cuenta de Usuario en la Nube KMS.
        /// </param>
        /// <remarks>
        ///     Por el momento: no es posible cambiar la contraseña del Usuario utilizando éste método,
        ///     la dirección de E-mail no se valida en el servidor (aún).
        /// </remarks>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        [HttpPost, Route("my/account")]
        public HttpResponseMessage PostAccount([FromBody]AccountPost accountPost) {
            // TODO: Change user password!
            // TODO: Validate email address.

            CurrentUser.PreferredCultureCode
                = accountPost.PreferredCultureCode.ToUpper(CultureInfo.InvariantCulture);
            CurrentUser.Email
                = accountPost.Email.ToLower(CultureInfo.InvariantCulture);

            if ( CurrentUser.RegionCode.Split(new char[] { '-' }).Length > 2 )
                CurrentUser.RegionCode
                    = accountPost.RegionCode.ToUpper(CultureInfo.InvariantCulture);

            Database.UserStore.Update(CurrentUser);
            Database.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}