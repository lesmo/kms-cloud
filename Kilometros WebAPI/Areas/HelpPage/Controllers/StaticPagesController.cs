using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kilometros_WebAPI.Areas.HelpPage.Controllers {
	public class StaticPagesController : Controller {
		public ActionResult Index(string view) {
			return View(view);
		}
	}
}