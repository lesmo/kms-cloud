using System;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Kms.Cloud.Api.Areas.HelpPage.ModelDescriptions;
using Kms.Cloud.Api.Areas.HelpPage.Models;

namespace Kms.Cloud.Api.Areas.HelpPage.Controllers {
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public class HelpController : Controller {
        private const string ErrorViewName = "Error";

        public HelpController()
            : this(GlobalConfiguration.Configuration) {
        }

        public HelpController(HttpConfiguration config) {
            Configuration = config;
        }

        public HttpConfiguration Configuration { get; private set; }

        public ActionResult Index() {
            var viewModel = Configuration.Services.GetApiExplorer().ApiDescriptions
                .Where(w => ! w.ActionDescriptor.ControllerDescriptor.ControllerName.ToUpper().StartsWith("OAUTH"))
                .ToList();

            ViewBag.Page = "RESTAPI";
            ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
            return View(viewModel);
        }

        public ActionResult Api(string apiId) {
            if (!String.IsNullOrEmpty(apiId)) {
                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                if (apiModel != null)
                {
                    return View(apiModel);
                }
            }

            ViewBag.Page = apiId.ToUpper().StartsWith("OAUTH") ? "OAUTH" : "RESTAPI";
            return View(ErrorViewName);
        }

        public ActionResult ResourceModel(string modelName) {
            if (!String.IsNullOrEmpty(modelName)) {
                ModelDescriptionGenerator modelDescriptionGenerator = Configuration.GetModelDescriptionGenerator();
                ModelDescription modelDescription;
                if (modelDescriptionGenerator.GeneratedModels.TryGetValue(modelName, out modelDescription)) {
                    return View(modelDescription);
                }
            }

            return View(ErrorViewName);
        }
    }
}