using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kilometros_WebApp.Controllers {
    public class BinaryResult : ActionResult {
        public byte[] Content {
            get;
            set;
        }
        
        public string ContentType {
            get;
            set;
        }

        public override void ExecuteResult(ControllerContext context) {
            context.HttpContext.Response.Clear(); 

            context.HttpContext.Response.ContentType
                = this.ContentType;
            context.HttpContext.Response.OutputStream.Write(
                this.Content,
                0,
                this.Content.Length
            );
        }
    }
}