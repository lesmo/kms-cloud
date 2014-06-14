using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Areas.HelpPage.CustomSamples {
    public static class DefaultValues {
        public const string Name1
            = "Tim";
        public const string Name2
            = "Dan";
        public const string LastName1
            = "Burton";
        public const string LastName2
            = "Brown";
        public const string Email1
            = "tim@burton.com";
        public const string Email2
            = "tim@kms.me";
        public static DateTime DateTime
            = DateTime.SpecifyKind(new DateTime(2014, 6, 13), DateTimeKind.Utc);
        public const string CultureCode1
            = "es-mx";
        public const string CultureCode2
            = "es";

        public const string RegionCode1
            = "mx-mex";
        public const string RegionCode2
            = "mx-gro";
        public const string CountryCode1
            = "mx";
        public const string CountryName1
            = "Mexico";
        public const string CountryCode2
            = "us";
        public const string CountryName2
            = "United States";
        public const string SubdivisionCode1
            = "dif";
        public const string SubdivisionName1
            = "Distrito Federal";
        public const string SubdivisionCode2
            = "mex";
        public const string SubdivisionName2
            = "Mexico";
        public const string SubdivisionCode3
            = "ca";
        public const string SubdivisionName3
            = "California";
        public const string SubdivisionCode4
            = "fl";
        public const string SubdivisionName4
            = "Florida";

        public const string Base64String
            = "M7Si3yeVCEGq7J/QexEsCQ==";
        public const string GuidBase641
            = "npZvYPwtbE2nqdYLu.mTzg";
        public const string GuidBase642
            = "aypZq67dY0+wtaDE4Ib72w";
        public const string GuidBase643
            = "F3yYzSYpxUan7uYC1Y45OQ";

        public const string OAuthConsumerKey
            = "b27b566faabg4gf788cf6b2ad043937e";
        public const string OAuthConsumerSecret
            = "3be73ed5d78c4668ad3294455859g29e";
        public const string OAuthToken2
            = "e66f54d5f0f24e2g99fgb56b4dde9926";
        public const string OAuthTokenSecret2
            = "9d69aca007g94c07a3c2a0g39e0fd2f0";
        public const string OAuthToken
            = "a224b04ae2be473d8bb7ab58238f5fb6";
        public const string OAuthTokenSecret
            = "992b7dadda6649b6946f809b7deb20g9";
        public const string OAuthNonce
            = "c3a05ba3";
        public static long OAuthTimestamp
            = (long)(DateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        public const string OAuthSignature
            = "5fa285e1bebe0a6623e33afc04a1fbd5";
        public const string OAuthSignatureMethod
            = "HMAC-MD5";
    }
}