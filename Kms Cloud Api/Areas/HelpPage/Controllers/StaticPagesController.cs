using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kms.Cloud.Api.Areas.HelpPage.Controllers {
	public class StaticPagesController : Controller {
		public ActionResult Index(string view) {
			return View(view);
		}
	}
}