using Kms.Cloud.Api.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kms.Cloud.Api.Controllers {
    [Authorize]
    public class OAuth3rdPartyAddController : IKMSController {
        [HttpPost]
        [Route("oauth/3rd/facebook/add")]
        public void FacebookAdd([FromBody]OAuth3rdLoginPost postData) {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("oauth/3rd/twitter/add")]
        public void TwitterAdd([FromBody]OAuth3rdLoginPost postData) {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("oauth/3rd/fitbit/add")]
        public void FitbitAdd([FromBody]OAuth3rdLoginPost postData) {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("oauth/3rd/nike/add")]
        public void NikeAdd([FromBody]OAuth3rdLoginPost postData) {
            throw new NotImplementedException();
        }
    }
}
