using Kms.Cloud.Api.Models.RequestModels;
using Kms.Cloud.Api.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Kms.Cloud.Api.Areas.HelpPage.CustomSamples {
    public static class ContactInfoControllerSamples {
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
                    CultureInfo.InvariantCulture,

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
                "ContactInfo",
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
                        = new Uri(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "http://api.kms.me/media/img/{0}.{1}",
                                DefaultValues.GuidBase642,
                                "png"
                            )
                        )
                }
            );
        }
    }
}