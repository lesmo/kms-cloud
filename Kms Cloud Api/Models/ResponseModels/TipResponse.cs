using System;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class TipResponse {
        public string TipId { get; set; }

        public DateTime Timestamp { get; set; }
        public string Text { get; set; }
        public string Source { get; set; }

        public TipCategoryResponse TipCategory { get; set; }
    }
}