using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Kms.Cloud.Api.Models.RequestModels;
using Kms.Cloud.Api.Models.ResponseModels;
using Kms.Cloud.Database;
using Kms.Cloud.Api.Exceptions;
using Kilometros_WebGlobalization.API;
using System.Diagnostics.CodeAnalysis;

namespace Kms.Cloud.Api.Controllers {
    /// <summary>
    ///     Obtener y actualizar el Perfil Físico del Usuario en la Nube KMS de la sesión actual.
    /// </summary>
    public class PhysiqueController : BaseController {

        /// <summary>
        ///     Obtener el Perfil Fïsico del Usuario actual.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [HttpGet, Route("my/physique")]
        public PhysiqueResponse GetPhysique() {
            UserBody physique
                = CurrentUser.UserBody;

            // --- Validar si existe Perfil Físico ---
            if ( physique == null )
                throw new HttpNoContentException(
                    ControllerStrings.Warning204_PhysicalInfoNotSet
                );

            // --- Verificar si se tiene la cabecera {If-Modified-Since} ---
            DateTimeOffset? ifModifiedSince
                = Request.Headers.IfModifiedSince;

            if ( ifModifiedSince.HasValue ) {
                if ( ifModifiedSince.Value.DateTime > physique.LastEditDate )
                    throw new HttpNotModifiedException();
            }

            // --- Preparar y devolver respuesta ---
            return new PhysiqueResponse() {
                Height
                    = physique.Height,
                Weight
                    = physique.Weight,
                Sex
                    = physique.Sex,

                StrideLengthWalking
                    = physique.StrideLengthWalking,
                StrideLengthRunning
                    = physique.StrideLengthRunning,

                LastModified
                    = physique.LastEditDate
            };
        }

        /// <summary>
        ///     Actualizar el Perfil Físico del Usuario.
        /// </summary>
        /// <param name="dataPost">Nueva información del Perfil Físico del Usuario.</param>
        [HttpPost]
        [Route("my/physique")]
        public HttpResponseMessage PostPhysique([FromBody]PhysiquePost dataPost) {
            UserBody physique
                = CurrentUser.UserBody ?? new UserBody() {
                    User
                        = OAuth.Token.User
                };

            physique.Height
                = (Int16)dataPost.Height;
            physique.Weight
                = dataPost.Weight;
            physique.Sex
                = dataPost.Sex;
            //physique.StrideLengthRunning
            //    = dataPost.StrideLengthRunning;
            //physique.StrideLengthWalking
            //    = dataPost.StrideLengthWalking;

            if ( OAuth.Token.User.UserBody == null )
                Database.UserBodyStore.Add(physique);
            else
                Database.UserBodyStore.Update(physique);

            return new HttpResponseMessage() {
                StatusCode
                    = HttpStatusCode.OK
            };
        }
    }
}
