using Kilometros_WebAPI.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using Kilometros_WebAPI.Models.HttpGet.My_Controllers;
using Kilometros_WebAPI.Models.HttpPost.My_Controller;
using Kilometros_WebAPI.Helpers;
using KilometrosDatabase;
using System.Globalization;

namespace Kilometros_WebAPI.Controllers {
    [Authorize]
    public class MyContactInfoController : ApiController {
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();
        private HttpServerUtility _httpServerUtility
            = new HttpServerUtility();

        [HttpGet]
        [Route("my/contact-info")]
        public HttpResponseMessage GetContactInfo() {
            KmsIdentity identity
                = MiscHelper.GetPrincipal<KmsIdentity>();
            ContactInfo contactInfo
                = identity.UserData.ContactInfo;

            /** Validar si existe Información de Contacto registrada **/
            if ( contactInfo == null ) {
                return Request.CreateResponse(
                    HttpStatusCode.NoContent
                );
            }
            
            /** Verificar si se tiene la cabecera {If-Modified-Since} **/
            DateTimeOffset? ifModifiedSince 
                = Request.Headers.IfModifiedSince;

            if ( ifModifiedSince.HasValue ) {
                if ( ifModifiedSince.Value.DateTime > contactInfo.LastEditDate )
                    return Request.CreateResponse(
                        HttpStatusCode.NotModified
                    );
            }

                /** Preparar respuesta **/
            ContactInfoResponse responseContent
                = new ContactInfoResponse() {
                    HomePhone
                        = contactInfo.HomePhone,
                    MobilePhone
                        = contactInfo.MobilePhone,
                    WorkPhone
                        = contactInfo.WorkPhone,
                    LastEdit
                        = contactInfo.LastEditDate
                };

            HttpResponseMessage response
                = Request.CreateResponse<ContactInfoResponse>(
                    HttpStatusCode.OK,
                    responseContent
                );
            
            response.Headers.TryAddWithoutValidation(
                "Last-Modified",
                contactInfo.LastEditDate.ToString(DateTimeFormatInfo.InvariantInfo.RFC1123Pattern)
            );

            return response;
        }

        [HttpPost]
        [Route("my/account")]
        public HttpResponseMessage PostAccount([FromBody]ContactInfoPost contactInfoPost) {
            HttpResponseMessage response
                = Request.CreateResponse();

            KmsIdentity identity
                = MiscHelper.GetPrincipal<KmsIdentity>();
            ContactInfo contactInfo
                = identity.UserData.ContactInfo ?? new ContactInfo();

            contactInfo.HomePhone
                = contactInfoPost.HomePhone;
            contactInfo.MobilePhone
                = contactInfoPost.MobilePhone;
            contactInfo.WorkPhone
                = contactInfoPost.WorkPhone;
            contactInfo.LastEditDate
                = DateTime.UtcNow;

            if ( identity.UserData.ContactInfo == null ) {
                contactInfo.User
                    = identity.UserData;

                this.Database.ContactInfoStore.Add(contactInfo);
            } else {
                this.Database.ContactInfoStore.Update(contactInfo);
            }

            this.Database.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created);
        }
    }
}
