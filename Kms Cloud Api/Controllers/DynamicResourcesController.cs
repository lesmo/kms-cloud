using Kilometros_WebGlobalization.API;
using Kms.Cloud.Api.Exceptions;
using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Kms.Cloud.Api.Controllers {
    public class DynamicResourcesController : BaseController {
        [AllowAnonymous]
        [HttpGet, Route("DynamicResources/Images/{filename}.{ext}")]
        public HttpResponseMessage Image(string filename, string ext) {
            var picture = Database.IPictureStore.Get(filename);

            if ( picture == null || picture.PictureExtension != ext )
                throw new HttpNotFoundException(
                    "601 " + ControllerStrings.Warning601_ResourceNotFound
                );

            var memory   = new MemoryStream(picture.Picture);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            
            response.Content = new StreamContent(memory);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(picture.PictureMimeType);

            return response;
        }
    }
}
