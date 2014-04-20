using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.RequestModels{
    public class CreateKmsAccountPost {
        public string Name { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        public string RegionCode { get; set; }
        public string CultureCode { get; set; }
        public short UtcOffset { get; set; }

        public DateTime Birthdate { get; set; }
        public short Height { get; set; }
        public int Weight { get; set; }
        public char Gender { get; set; }
    }
}