using System;
using System.Collections.Generic;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class TokenResponse {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public IEnumerable<String> Pending { get; set; }
    }
}