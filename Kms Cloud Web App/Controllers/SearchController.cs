using Kms.Cloud.WebApp.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kms.Cloud.WebApp.Controllers {
    [KmsAuthorize]
    public class SearchController : BaseController {
        // GET: Search
        public ActionResult Index(string q, int page = 1, int perPage = 10) {
            // > Validar items por Página
            if ( perPage > 40 )
                throw new HttpException(400, "Users Per Page is too high");
            
            var results = Database.UserStore.GetAll(
                filter: f =>
                    (f.Name + " " + f.LastName).Contains(q),
                extra: x =>
                    x.Skip(page * perPage).Take(perPage)
            ).Select(s => new FriendModel(s, this));

            var totalResults = Database.UserStore.GetCount(
                filter: f =>
                    (f.Name + " " + f.LastName).Contains(q)
            );

            return View(new SearchValues {
                SearchString = q,
                ResultsPages = totalResults,
                Results      = results
            });
        }
    }
}