using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.RequestModels {
    public class DataPost {
        public DateTime Timestamp { get; set; }
        public Int32 Steps { get; set; }
        public String Activity { get; set; }
    }
}