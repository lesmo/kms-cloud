using Kilometros_WebAPI.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using Kilometros_WebAPI.Models.RequestModels;
using Kilometros_WebAPI.Models.ResponseModels;
using Kilometros_WebAPI.Helpers;
using KilometrosDatabase;
using System.Globalization;
using Kilometros_WebAPI.Exceptions;
using Kilometros_WebGlobalization.API;

namespace Kilometros_WebAPI.Controllers {
    /// <summary>
    ///     Devuelve y modifica la Ínformación de la Contacto del Usuario en la Nube KMS.
    /// </summary>
    [Authorize]
    public class ContactInfoController : IKMSController {
        /// <summary>
        ///     Devuelve la Información de Contacto del Usuario en la Nube KMS.
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpGet]
        [Route("my/contact-info")]
        public ContactInfoResponse GetContactInfo() {
            ContactInfo contactInfo
                = CurrentUser.ContactInfo;

            // --- Validar si existe Información de Contacto registrada ---
            if ( contactInfo == null )
                throw new HttpNoContentException(
                    ControllerStrings.Warning203_ContactInfoNotSet
                );
            
            // --- Verificar si se tiene la cabecera {If-Modified-Since} ---
            DateTimeOffset? ifModifiedSince 
                = Request.Headers.IfModifiedSince;

            if ( ifModifiedSince.HasValue ) {
                if ( ifModifiedSince.Value.DateTime > contactInfo.LastEditDate )
                    throw new HttpNotModifiedException();
            }

            // --- Preparar respuesta ---
            return new ContactInfoResponse() {
                HomePhone
                    = contactInfo.HomePhone,
                MobilePhone
                    = contactInfo.MobilePhone,
                WorkPhone
                    = contactInfo.WorkPhone,
                LastModified
                    = contactInfo.LastEditDate
            };
        }

        /// <summary>
        ///     Establece la Información de Contacto del Usuario en la Nube KMS.
        /// </summary>
        /// <param name="dataPost">
        ///     Nueva Información de Contacto del Usuario en la Nube KMS.
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [Route("my/contact-info")]
        public HttpResponseMessage PostAccount([FromBody]ContactInfoPost dataPost) {
            ContactInfo contactInfo
                = CurrentUser.ContactInfo ?? new ContactInfo() {
                    User = CurrentUser
                };

            contactInfo.HomePhone
                = dataPost.HomePhone;
            contactInfo.MobilePhone
                = dataPost.MobilePhone;
            contactInfo.WorkPhone
                = dataPost.WorkPhone;

            if ( CurrentUser.ContactInfo == null )
                Database.ContactInfoStore.Add(contactInfo);
            else
                Database.ContactInfoStore.Update(contactInfo);

            Database.SaveChanges();

            return new HttpResponseMessage() {
                RequestMessage
                    = Request,

                StatusCode
                    = HttpStatusCode.OK
            };
        }
    }
}
