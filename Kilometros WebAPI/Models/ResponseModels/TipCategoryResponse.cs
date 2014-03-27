using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.ResponseModels {
    public class TipCategoryResponse {
        public string TipCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}