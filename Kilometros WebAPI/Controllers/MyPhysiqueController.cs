using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using Kilometros_WebAPI.Models.HttpGet.My_Controllers;
using Kilometros_WebAPI.Models.HttpPost.My_Controller;
using Kilometros_WebAPI.Security;
using Kilometros_WebAPI.Helpers;
using KilometrosDatabase;
using System.Globalization;
using Kilometros_WebAPI.Exceptions;
using Kilometros_WebGlobalization.API;

namespace Kilometros_WebAPI.Controllers {
    [Authorize]
    public class MyPhysiqueController : ApiController {
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();

        [HttpGet]
        [Route("my/physique")]
        public PhysiqueResponse GetPhysique() {
            KmsIdentity identity
                = MiscHelper.GetPrincipal<KmsIdentity>();
            UserBody physique
                = identity.UserData.UserBody;

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
                Age
                    = physique.Age,
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
        public IHttpActionResult PostPhysique([FromBody]PhysiquePost dataPost) {
            KmsIdentity identity
                = MiscHelper.GetPrincipal<KmsIdentity>();
            UserBody physique
                = identity.UserData.UserBody ?? new UserBody() {
                    User
                        = identity.UserData
                };

            physique.Age
                = dataPost.Age;
            physique.Height
                = dataPost.Height;
            physique.Weight
                = dataPost.Weight;
            physique.Sex
                = dataPost.Sex;
            physique.StrideLenghtRunning
                = dataPost.StrideLengthRunning;
            physique.StrideLengthWalking
                = dataPost.StrideLengthWalking;

            if ( identity.UserData.UserBody == null )
                Database.UserBodyStore.Add(physique);
            else
                Database.UserBodyStore.Update(physique);
            return Ok();
        }
    }
}
