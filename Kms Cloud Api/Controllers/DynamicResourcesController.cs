using Kilometros_WebGlobalization.API;
using Kms.Cloud.Api.Exceptions;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Kms.Cloud.Api.Controllers {
    /// <summary>
    ///     Descargar "assets", "medios" o "recursos" generados dinámicamente por el servidor. Esto
    ///     incluye normalmente Fotografías de Regalos, Iconos de Recompensas, Tips, y las Fotos de
    ///     Perfil de los Usuarios. Todos los recursos del API devuelven una URL completa a las imágenes
    ///     mencionadas, y normalmente apuntan a estos métodos. La documentación está aquí, y seguirá
    ///     aquí, hasta el día en que los cerdos vuelen, por muy inútil que ésta parezca.
    /// </summary>
    public class DynamicResourcesController : BaseController {
        /// <summary>
        ///     Descargar una Imágen Dinámica.
        /// </summary>
        /// <param name="filename">Nombre del archivo.</param>
        /// <param name="ext">Extensión del archivo.</param>
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
