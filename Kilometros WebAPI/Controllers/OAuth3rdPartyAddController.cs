using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kilometros_WebAPI.Controllers {
    [Authorize]
    public class OAuth3rdPartyAddController : ApiController {
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();

        [HttpPost]
        [Route("oauth/3rd/facebook/add")]
        public void FacebookAdd([FromBody]string facebook_token) {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("oauth/3rd/twitter/add")]
        public void TwitterAdd() {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("oauth/3rd/fitbit/add")]
        public void FitbitAdd() {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("oauth/3rd/nike/add")]
        public void NikeAdd() {
            throw new NotImplementedException();
        }
    }
}
