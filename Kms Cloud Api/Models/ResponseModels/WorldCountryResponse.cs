using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class WorldSubdivisionResponse {
        public String Code { get; set; }
        public String Name { get; set; }
    }

    public class WorldCountryResponse : WorldSubdivisionResponse {
        public IEnumerable<WorldSubdivisionResponse> Subdivisions { get; set; }
    }
}