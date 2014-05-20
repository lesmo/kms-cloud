using Kms.Cloud.Api.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kms.Cloud.Api.Controllers {
    [AllowAnonymous]
    public class WorldController : BaseController {
        [HttpGet, Route("world")]
        public IEnumerable<WorldCountryResponse> GetWorld() {
            return Database.RegionStore.GetAll(
                include:
                    new string[] { "RegionSubdivision" }
            ).Select(r =>
                new WorldCountryResponse {
                    Code = r.IsoCode,
                    Name = r.Name,
                    Subdivisions = r.RegionSubdivision.Select(s =>
                        new WorldSubdivisionResponse {
                            Code = s.IsoCode,
                            Name = s.Name
                        }
                    )
                }
            );
        }
    }
}
