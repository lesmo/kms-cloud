using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class DeviceResponse {
        public DateTime LinkDate {
            get;
            set;
        }

        public String SerialString {
            get;
            set;
        }
    }
}