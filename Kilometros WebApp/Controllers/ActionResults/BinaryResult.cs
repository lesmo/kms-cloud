using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kilometros_WebApp.Controllers {
    public class BinaryResult : ActionResult {
        /// <summary>
        ///     Cuerpo de la respuesta.
        /// </summary>
        public byte[] Content {
            get;
            set;
        }
        
        /// <summary>
        ///     Tipo MIME del Contenido de la respuesta.
        /// </summary>
        public string ContentType {
            get;
            set;
        }

        public override void ExecuteResult(ControllerContext context) {
            context.HttpContext.Response.Clear(); 

            context.HttpContext.Response.ContentType
                = string.IsNullOrEmpty(this.ContentType)
                ? "application/octet-stream"
                : this.ContentType;
            context.HttpContext.Response.OutputStream.Write(
                this.Content,
                0,
                this.Content.Length
            );
        }
    }
}