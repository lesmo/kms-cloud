using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kms.Cloud.WebApp.Controllers {
    public partial class AjaxController {
        public JsonResult Tips(string cat, int page = 1, int perPage = 10) {
            // > Validar items por Página
            if ( perPage > 40 )
                throw new HttpException(400, "Tips Per Page is too high");

            // > Validar categoría
            TipCategory tipCategory
                = Database.TipCategoryStore[cat];

            if ( tipCategory == null )
                throw new HttpException(404, "Category not found");

            // > Obtener Tips de la Categoría
            var tips = Database.UserTipHistoryStore.GetAll(
                    filter: f =>
                        f.User.Guid == CurrentUser.Guid,
                    orderBy: o =>
                        o.OrderByDescending(b => b.CreationDate),
                    extra: x =>
                        x.Skip(page * perPage).Take(perPage),
                    include:
                        new string[] { "Tip" }
                ).Select(s =>
                    s.Tip.GetGlobalization(
                        CultureInfo.CurrentCulture
                    )
                ).Select(s =>
                    new {
                        text = s.Text,
                        source = s.Source
                    }
                );

            // > Devolver respuesta en JSON
            return Json(tips, JsonRequestBehavior.AllowGet);
        }
    }
}