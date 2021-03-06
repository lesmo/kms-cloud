﻿using System;
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
    ///     Obtener y modificar la Ínformación de la Contacto del Usuario en la Nube KMS.
    /// </summary>
    [Authorize]
    public class ContactInfoController : BaseController {
        /// <summary>
        ///     Obtener la Información de Contacto del Usuario en la Nube KMS.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [HttpGet, Route("my/contact-info")]
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
