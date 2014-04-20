using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class DataDistanceResponse {
        public DateTime Timestamp { get; set; }

        public long Distance { get; set; }
        public long Steps { get; set; }
    }
}