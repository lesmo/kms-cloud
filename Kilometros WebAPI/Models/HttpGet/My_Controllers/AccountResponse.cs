﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.HttpGet.My_Controllers {
    public class AccountResponse {
        public DateTime AccountCreationDate;
        public string Email;
        public string RegionCode;
        public string PreferredCultureCode;
    }
}