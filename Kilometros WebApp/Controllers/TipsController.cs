using Kilometros_WebApp.Models.Views;
using KilometrosDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kilometros_WebApp.Controllers {
	public class TipsController : BaseController {
		const int TipsPerPage
			= 10;

		protected TipsValues TipsValues {
			get {
				if ( this._tipValues != null )
					return this._tipValues;

				// > Inicializar valores de Tips
				this._tipValues
					= new TipsValues();

				// > Devolver valores de Tips
				return this._tipValues;
			}
		}
		private TipsValues _tipValues;

		// GET: /Tips/
		public ActionResult Index(string cat = null, int page = 1) {
			// > Obtener las Categorías de Tips
			IEnumerable<TipCategoryGlobalization> tipCategories
				= Database.TipCategoryStore.GetAll().Select( s =>
					s.GetGlobalization()
				);

			TipCategory tipCategory;

			if ( cat == null ) {
				tipCategory
					= tipCategories.FirstOrDefault().TipCategory;
				
				if ( tipCategory == null)
					throw new HttpException(591, "No tip categories are defined!");
			} else {
				tipCategory
					= tipCategories.Select(s =>
						s.TipCategory
					).Where(w =>
						w.Guid == new Guid().FromBase64String(cat)
					).FirstOrDefault();
				
				if ( tipCategory == null )
					return RedirectToAction("Tips", "Index");
			}
			
			// > Obtener los Tips desbloqueados por el Usuario en la Categoría
			//   y Página solicitadas
			IEnumerable<Tip> tipHistoryTemp
				= Database.UserTipHistoryStore.GetAll(
					filter: f =>
						f.User.Guid == CurrentUser.Guid
						&& f.Tip.TipCategory.Guid == tipCategory.Guid,
					orderBy: o =>
						o.OrderByDescending(b => b.CreationDate),
					extra: x =>
						x.Skip(page * TipsPerPage).Take(TipsPerPage),
					include:
						new string[] { "Tip.TipCategory" }
				).Select( s =>
					s.Tip
				);

			// > Obtener las cadenas de Globalización de los Tips y sus Categorías
			List<TipModel> tipHistory
				= new List<TipModel>();

			foreach ( Tip tip in tipHistoryTemp ) {
				TipGlobalization tipGlobalization
					= tip.GetGlobalization();
				TipCategoryGlobalization tipCategoryGlobalization
					= tip.TipCategory.GetGlobalization();
				Uri tipCategoryIconUri
					= GetDynamicResourceUri(
						method:
							"Images",
						filename:
							tip.TipCategory.Guid.ToBase64String(),
						ext:
							tip.TipCategory.PictureExtension
					);

				tipHistory.Add(
					new TipModel() {
						TipId
							= tip.Guid.ToBase64String(),
						Text
							= tipGlobalization.Text,
						Category
							= tipCategoryGlobalization.Name,
						IconUri
							= tipCategoryIconUri
					}
				);
			}

			// > Preparar valores para Vista
			// + Categorías
			TipsValues.Categories
				= tipCategories.Select(s =>
					new TipCategoryModel() {
						CategoryId
							= s.TipCategory.Guid.ToBase64String(),
						Name
							= s.Name,
						IconUri
							= GetDynamicResourceUri(
								method:
									"Images",
								filename:
									s.TipCategory.Guid.ToBase64String(),
								ext:
									s.TipCategory.PictureExtension
							)
					}
				).ToArray();

			// + Tips
			TipsValues.CurrentCategoryTips
				= tipHistory.ToArray();

			// + Categoría actual
			TipsValues.CurrentCategory
				= tipCategories.Where(w =>
					w.TipCategory.Guid == tipCategory.Guid
				).Select( s =>
					new TipCategoryModel() {
						Name
							= s.Name,
						IconUri
							= GetDynamicResourceUri(
								method:
									"Images",
								filename:
									s.TipCategory.Guid.ToBase64String(),
								ext:
									s.TipCategory.PictureExtension
							)					}
				).FirstOrDefault();

			// + Páginas totales disponibles
			TipsValues.CurrentCategoryTipsTotalPages
				= (int)Math.Ceiling(
					(float)tipCategory.Tip.Where( w =>
						w.UserTipHistory.Any(t =>
							t.User.Guid == CurrentUser.Guid
						)
					).Count() / TipsPerPage
				);

			// > Añadir valores a Vista
			ViewData.Add(
				"LayoutValues",
				LayoutValues
			);
			ViewData.Add(
				"TipsValues",
				TipsValues
			);

			// > Render de Vista
			return View();
		}
	}
}