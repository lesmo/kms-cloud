using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Areas.HelpPage.CustomSamples {
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
        public const string Base64String
            = "M7Si3yeVCEGq7J/QexEsCQ==";
        public const string GuidBase641
            = "npZvYPwtbE2nqdYLu.mTzg";
        public const string GuidBase642
            = "aypZq67dY0+wtaDE4Ib72w";
        public const string GuidBase643
            = "F3yYzSYpxUan7uYC1Y45OQ";
    }
}