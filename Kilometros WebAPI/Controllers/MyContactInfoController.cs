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
using Kilometros_WebAPI.Exceptions;
using Kilometros_WebGlobalization.API;

namespace Kilometros_WebAPI.Controllers {
    [Authorize]
    public class MyContactInfoController : ApiController {
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();

        [HttpGet]
        [Route("my/contact-info")]
        public ContactInfoResponse GetContactInfo() {
            KmsIdentity identity
                = MiscHelper.GetPrincipal<KmsIdentity>();
            ContactInfo contactInfo
                = identity.UserData.ContactInfo;

            /** Validar si existe Información de Contacto registrada **/
            if ( contactInfo == null )
                throw new HttpNoContentException(
                    ControllerStrings.Warning203_ContactInfoNotSet
                );
            
            /** Verificar si se tiene la cabecera {If-Modified-Since} **/
            DateTimeOffset? ifModifiedSince 
                = Request.Headers.IfModifiedSince;

            if ( ifModifiedSince.HasValue ) {
                if ( ifModifiedSince.Value.DateTime > contactInfo.LastEditDate )
                    throw new HttpNotModifiedException();
            }

            /** Preparar respuesta **/
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

        [HttpPost]
        [Route("my/account")]
        public IHttpActionResult PostAccount([FromBody]ContactInfoPost dataPost) {
            HttpResponseMessage response
                = Request.CreateResponse();

            KmsIdentity identity
                = MiscHelper.GetPrincipal<KmsIdentity>();
            ContactInfo contactInfo
                = identity.UserData.ContactInfo ?? new ContactInfo() {
                    User = identity.UserData
                };

            contactInfo.HomePhone
                = dataPost.HomePhone;
            contactInfo.MobilePhone
                = dataPost.MobilePhone;
            contactInfo.WorkPhone
                = dataPost.WorkPhone;

            if ( identity.UserData.ContactInfo == null )
                Database.ContactInfoStore.Add(contactInfo);
            else
                Database.ContactInfoStore.Update(contactInfo);

            Database.SaveChanges();

            return Ok();
        }
    }
}
