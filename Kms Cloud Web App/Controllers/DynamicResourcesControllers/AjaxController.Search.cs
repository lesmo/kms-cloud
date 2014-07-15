using Kms.Cloud.Database;
using Kms.Cloud.Database.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kms.Cloud.WebApp.Controllers {
    public partial class AjaxController {
        public JsonResult SearchAutocomplete(string search) {
            if ( string.IsNullOrEmpty(search) || search.Length < 3 )
                throw new HttpException(400, "Search term cannot be smaller than 3 characters long");

            var usersAutocomplete = Database.UserStore.GetAll(
                filter: f =>
                    f.Name.StartsWith(search) || f.Name.StartsWith(search),
                orderBy: o =>
                    o.OrderBy(b => b.Name),
                extra: x =>
                    x.Take(10)
            ).Select(s => new {
                fullname = s.Name + " " + s.LastName,
                id = s.Guid.ToBase64String()
            });

            return Json(usersAutocomplete, JsonRequestBehavior.AllowGet);
        }
    }
}