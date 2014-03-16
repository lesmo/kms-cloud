using Kilometros_WebAPI.Models.HttpGet.My_Controllers;
using Kilometros_WebAPI.Models.HttpPost.My_Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Kilometros_WebAPI.Areas.HelpPage.CustomSamples {
    public class MyContactInfoControllerSamples {
        public static void Register(Dictionary<Type, object> typeSamples, HttpConfiguration config) {
            // --- Petición ---
            typeSamples.Add(
                typeof(AccountPost),
                new AccountPost() {
                    Email
                        = DefaultValues.Email1,
                    PreferredCultureCode
                        = DefaultValues.CultureCode1,
                    RegionCode
                        = DefaultValues.CultureCode1,
                    Password
                        = DefaultValues.Base64String
                }
            );

            config.SetSampleRequest(
                string.Format(
                    "Email={0}\n"
                    + "&PreferredCultureCode={1}\n"
                    + "&RegionCode={2}\n"
                    + "&Password={3}",

                    HttpUtility.UrlEncode(DefaultValues.Email1),
                    HttpUtility.UrlEncode(DefaultValues.CultureCode1),
                    HttpUtility.UrlEncode(DefaultValues.RegionCode1),
                    HttpUtility.UrlEncode(DefaultValues.Base64String)
                ),
                new MediaTypeHeaderValue("application/x-www-form-urlencoded"),
                "MyContactInfo",
                "PostAccount"
            );

            // --- Respuesta ---
            typeSamples.Add(
                typeof(AccountResponse),
                new AccountResponse() {
                    AccountCreationDate
                        = DefaultValues.DateTime,
                    UserId
                        = DefaultValues.GuidBase641,
                    Name
                        = DefaultValues.Name1,
                    LastName
                        = DefaultValues.LastName1,
                    Email
                        = DefaultValues.Email1,
                    PreferredCultureCode
                        = DefaultValues.CultureCode1,
                    RegionCode
                        = DefaultValues.RegionCode1,
                    PictureUri
                        = string.Format(
                            "http://api.kms.me/media/img/{0}.{1}",
                            DefaultValues.GuidBase642,
                            "png"
                        )
                }
            );
        }
    }
}