﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Kilometros_WebAPI.Models.HttpGet.My_Controllers;
using Kilometros_WebAPI.Models.HttpPost.My_Controller;
using Kilometros_WebAPI.Security;
using KilometrosDatabase;
using System.Net;
using System.Globalization;
using Kilometros_WebGlobalization.API;
using Kilometros_WebAPI.Helpers;
using System.Web;

namespace Kilometros_WebAPI.Controllers {
    [Authorize]
    public class MyAccountController : ApiController {
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();
        private HttpServerUtility _httpServerUtility
            = HttpContext.Current.Server;

        [HttpGet]
        [Route("my/account")]
        public HttpResponseMessage GetAccount() {
            // TODO: Añadir funcionalidad de If-Modified-Since

            KmsIdentity identity
                = MiscHelper.GetPrincipal<KmsIdentity>();
            
            AccountResponse responseContent
                = new AccountResponse() {
                    AccountCreationDate
                        = identity.UserData.CreationDate,
                    Email
                        = identity.UserData.Email,
                    PreferredCultureCode
                        = identity.UserData.PreferredCultureInfo.Name,
                    RegionCode
                        = identity.UserData.RegionCode
                };
            
            return Request.CreateResponse<AccountResponse>(
                HttpStatusCode.OK,
                responseContent
            );
        }

        [HttpPost]
        [Route("my/account")]
        public HttpResponseMessage PostAccount([FromBody]AccountPost accountPost) {
            HttpResponseMessage response
                = Request.CreateResponse();

            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;

            try {
                if ( accountPost.PreferredCultureCode != null ) {
                    CultureInfo cultureInfo
                        = new CultureInfo(accountPost.PreferredCultureCode.ToLowerInvariant());
                    user.PreferredCultureInfo
                        = cultureInfo;
                }

                if ( accountPost.RegionCode != null )
                    user.RegionCode
                        = accountPost.RegionCode.ToLowerInvariant();

                if ( accountPost.Email != null )
                    user.Email
                        = accountPost.Email.ToLowerInvariant();
                
                this.Database.UserStore.Update(user);
                this.Database.SaveChanges();

                response.StatusCode
                    = HttpStatusCode.Created;
            } catch {
                string warningString
                    = ControllerStrings.Warning501_AccountDataInvalid;
                response.StatusCode
                    = HttpStatusCode.BadRequest;

                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "501" + this._httpServerUtility.UrlEncode(warningString)
                );
            }

            return response;
        }
    }
}