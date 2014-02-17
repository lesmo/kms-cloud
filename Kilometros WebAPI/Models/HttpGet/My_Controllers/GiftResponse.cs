using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.HttpGet.My_Controllers {
    public class GiftResponse {
        public string NameSingular;
        public string NamePlural;

        public string RedeemCode;
        public string RedeemPicture;

        public string[] Pictures;
    }
}