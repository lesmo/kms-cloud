using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using Kms.Cloud.Api.Models.RequestModels;
using Kms.Cloud.Api.Models.ResponseModels;
using Kms.Cloud.Api.Security;
using Kms.Cloud.Database;
using System.Globalization;
using Kms.Cloud.Api.Exceptions;
using Kilometros_WebGlobalization.API;

namespace Kms.Cloud.Api.Controllers {
    [Authorize]
    public class PhysiqueController : IKMSController {
        [HttpGet]
        [Route("my/physique")]
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

        [HttpPost]
        [Route("my/physique")]
        public HttpResponseMessage PostPhysique([FromBody]PhysiquePost dataPost) {
            UserBody physique
                = CurrentUser.UserBody ?? new UserBody() {
                    User
                        = OAuth.Token.User
                };

            physique.Height
                = dataPost.Height;
            physique.Weight
                = dataPost.Weight;
            physique.Sex
                = dataPost.Sex;
            physique.StrideLengthRunning
                = dataPost.StrideLengthRunning;
            physique.StrideLengthWalking
                = dataPost.StrideLengthWalking;

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
