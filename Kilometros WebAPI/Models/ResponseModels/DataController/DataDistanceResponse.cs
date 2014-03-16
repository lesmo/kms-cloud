using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.HttpGet.DataController {
    public class DataDistanceResponse {
        public DateTime Timestamp;

        public long Distance;
        public long Steps;
    }
}