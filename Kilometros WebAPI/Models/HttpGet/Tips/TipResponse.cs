using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.HttpGet.Tips {
    public class TipResponse {
        public string TipId;

        public DateTime Timestamp;
        public string Text;
        public string Source;

        public TipCategoryResponse TipCategory;
    }
}