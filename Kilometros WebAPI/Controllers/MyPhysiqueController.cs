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

namespace Kilometros_WebAPI.Controllers {
    [Authorize]
    public class MyPhysiqueController : ApiController {
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();
        private HttpServerUtility _httpServerUtility
            = new HttpServerUtility();

        [HttpGet]
        [Route("my/physique")]
        public HttpResponseMessage GetPhysique() {
            KmsIdentity identity
                = MiscHelper.GetPrincipal<KmsIdentity>();
            UserBody physique
                = identity.UserData.UserBody;

            /** Validar si existe Perfil Físico **/
            if ( physique == null ) {
                return Request.CreateResponse(
                    HttpStatusCode.NoContent
                );
            }

            /** Verificar si se tiene la cabecera {If-Modified-Since} **/
            DateTimeOffset? ifModifiedSince
                = Request.Headers.IfModifiedSince;

            if ( ifModifiedSince.HasValue ) {
                if ( ifModifiedSince.Value.DateTime > physique.LastEditDate )
                    return Request.CreateResponse(
                        HttpStatusCode.NotModified
                    );
            }

            /** Preparar y devolver respuesta **/
            PhysiqueResponse responseContent
                = new PhysiqueResponse() {
                    Age
                        = physique.Age,
                    Height
                        = physique.Height,
                    Weight
                        = physique.Weight,
                    Sex
                        = physique.Sex,
                    LastEdit
                        = physique.LastEditDate
                };

            HttpResponseMessage response
                = Request.CreateResponse<PhysiqueResponse>(
                    HttpStatusCode.OK,
                    responseContent
                );

            response.Headers.TryAddWithoutValidation(
                "Last-Modified",
                physique.LastEditDate.ToString(DateTimeFormatInfo.InvariantInfo.RFC1123Pattern)
            );

            return response;
        }
    }
}
