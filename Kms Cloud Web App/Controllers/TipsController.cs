using Kms.Cloud.WebApp.Models.Views;
using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kms.Cloud.WebApp.Controllers {
	[KmsAuthorize]
	public class TipsController : BaseController {
		private const int TipsPerPage = 10;

		// GET: /Tips/
		public ActionResult Index(string cat = null, int page = 1) {
			// Validar el número de Página
			if ( page < 1 )
				return RedirectToAction("Index", new {page = 1});
			else
				page--;

			// > Obtener las Categorías de Tips
			var tipCategories = Database.TipCategoryStore.GetAll().ToList();

			TipCategory tipCategory;
			
			if ( cat == null ) {
				tipCategory = tipCategories.FirstOrDefault();
				
				if ( tipCategory == null)
					throw new HttpException(591, "No tip categories are defined!");
			} else {
				tipCategory = tipCategories.FirstOrDefault(
					w => w.Guid == new Guid().FromBase64String(cat)
				);
				
				if ( tipCategory == null )
					return RedirectToAction("Index", new { page = page });
			}
			
			// > Obtener los Tips desbloqueados por el Usuario en la Categoría
			//   y Página solicitadas
			var tipHistory = Database.UserTipHistoryStore.GetAll(
				filter: f =>
					f.User.Guid == CurrentUser.Guid
					&& f.Tip.TipCategory.Guid == tipCategory.Guid,
				orderBy: o =>
					o.OrderByDescending(b => b.CreationDate),
				extra: x =>
					x.Skip(page * TipsPerPage).Take(TipsPerPage),
				include:
					new string[] { "Tip.TipCategory" }
			).Select(s => new TipModel {
				TipId    = s.Guid.ToBase64String(),
				Text     = s.Tip.GetGlobalization().Text,
				Category = tipCategory.GetGlobalization().Name,
				IconUri  = GetDynamicResourceUri(s.Tip.TipCategory)
			});

			// > Preparar valores para Vista
			var tipsValues = new TipsValues {
				// + Categorías
				Categories = tipCategories.Select(s => new TipCategoryModel {
					CategoryId = s.Guid.ToBase64String(),
					Name = s.GetGlobalization().Name,
					IconUri = GetDynamicResourceUri(s)
				}).ToArray(),

				// + Tips
				CurrentCategoryTips = tipHistory.ToArray(),

				// + Categoría actual
				CurrentCategory = new TipCategoryModel {
					CategoryId = tipCategory.Guid.ToBase64String(),
					Name = tipCategory.GetGlobalization().Name,
					IconUri = GetDynamicResourceUri(tipCategory)
				},

				// + Páginas totales disponibles
				CurrentCategoryTipsTotalPages = (int)Math.Ceiling(
					(double)Database.UserTipHistoryStore.GetCount(
						f => f.User.Guid == CurrentUser.Guid
					) / TipsPerPage
				)
			};

			// > Render de Vista
			return View(tipsValues);
		}

		// GET: /Tips/Detail
		public ActionResult Detail(string id) {
			var tipHistory = Database.UserTipHistoryStore.Get(id);

			if ( tipHistory == null || tipHistory.User.Guid != CurrentUser.Guid )
				return HttpNotFound();

		    var tipModel = new TipModel {
		        TipId    = tipHistory.Guid.ToBase64String(),
		        Text     = tipHistory.Tip.GetGlobalization().Text,
		        Category = tipHistory.Tip.TipCategory.GetGlobalization().Name,
		        IconUri  = GetDynamicResourceUri(tipHistory.Tip.TipCategory)
		    };

		    return View(tipModel);
		}
	}
}