using System.Web.Http;
using System.Web.Mvc;
using System.Linq;

namespace Kms.Cloud.Api.Areas.HelpPage.Controllers {
	public class StaticPagesController : Controller {
		public StaticPagesController()
			: this(GlobalConfiguration.Configuration) {
		}

		public StaticPagesController(HttpConfiguration config) {
			Configuration = config;
		}

		public HttpConfiguration Configuration {
			get;
			private set;
		}

		public ActionResult KmsHttpHeaders() {
			ViewBag.Page = "HTTPHEADERS";
			return View();
		}

		public ActionResult OAuth() {
			var viewModel = Configuration.Services.GetApiExplorer().ApiDescriptions
				.Where(w => w.ActionDescriptor.ControllerDescriptor.ControllerName.ToUpper().StartsWith("OAUTH"))
				.ToList();

			ViewBag.Page = "OAUTH";
			ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
			return View("OAuth", viewModel);
		}
	}
}