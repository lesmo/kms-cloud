using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Kilometros_WebAPI.Models.HttpGet.UserController;
using Kilometros_WebAPI.Models.HttpPost.UserController;
using Kilometros_WebAPI.Security;
using KilometrosDatabase;
using System.Net;
using System.Globalization;
using Kilometros_WebGlobalization.API;

namespace Kilometros_WebAPI.Controllers {

    public class UserController : ApiController {
        [HttpGet]
        [Route("my/account")]
        public HttpResponseMessage GetAccount() {
            KmsIdentity identity = (KmsIdentity)User.Identity;
            AccountResponse responseContent = new AccountResponse() {
                AccountCreationDate = identity.UserData.CreationDate,
                Email = identity.UserData.Email,
                PreferredCultureCode = identity.UserData.PreferredCultureInfo.Name,
                RegionCode = identity.UserData.RegionCode
            };
            
            return Request.CreateResponse<AccountResponse>(
                HttpStatusCode.OK,
                responseContent
            );
        }

        [HttpPost]
        [Route("my/account")]
        public HttpResponseMessage PostAccount([FromBody]AccountPost accountPost) {
            HttpResponseMessage response = Request.CreateResponse();

            KmsIdentity identity = (KmsIdentity)User.Identity;
            User user = identity.UserData;

            try {
                user.RegionCode = accountPost.RegionCode;

                CultureInfo cultureInfo = new CultureInfo(accountPost.PreferredCultureCode);
                user.PreferredCultureInfo = cultureInfo;

                user.Email = accountPost.Email;

                response.StatusCode = HttpStatusCode.Created;
            } catch {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Headers.TryAddWithoutValidation(
                    "Warning",
                    "501" + ControllerStrings.Warning501_AccountDataInvalid
                );
            }

            return response;
        }
    }
}