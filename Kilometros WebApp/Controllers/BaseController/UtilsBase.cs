using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebApp.Controllers {
    public abstract partial class BaseController {
        /// <summary>
        ///     Devuelve una Uri absoluta que apunta al Recurso generado dinámicamente especificado
        ///     por el nombre del archivo (normalmente el GUID del recurso en BD) y su extensión.
        /// </summary>
        /// <param name="method">
        ///     Método en controlador DynamicResources responsable de generar el recurso.
        /// </param>
        /// <param name="filename">
        ///     Nombre del archivo (normalmente el GUID del recurso en BD).
        /// </param>
        /// <param name="ext">
        ///     Extensión esperada por Método.
        /// </param>
        /// <returns>
        ///     URI absoluta que apunta al recurso descrito por los parámetros.
        /// </returns>
        protected Uri GetDynamicResourceUri(string method, string filename, string ext) {
            return new Uri(
                Url.Content(
                    string.Format(
                        "~/{0}/{1}.{2}",
                        "DynamicResources/Images",
                        filename,
                        ext
                    )
                )
            );
        }
    }
}