using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.HttpGet.My_Controllers {
    public class ContactInfoResponse : IModifiedDate {
        public string HomePhone;
        public string MobilePhone;
        public string WorkPhone;
    }
}