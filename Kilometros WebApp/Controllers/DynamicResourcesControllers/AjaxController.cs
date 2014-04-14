using KilometrosDatabase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kilometros_WebApp.Controllers {
	public class AjaxController : BaseController {
		// GET: /DynamicResources/Ajax/Overview.json
		[Authorize]
		public JsonResult Overview() {
			// > Obtener últimas 24hrs. de lecturas
			//   [MUST REVIEW] Las lecturas deberían distinguir entre la actividad {Correr},
			//                 {Caminar} y {Sueño}. Actualmente se "agregan" todas las actividades
			//                 que no correspondan a {Sueño}, puesto que la vista NO dispone de
			//                 ésta diferenciación en la Gráfica del día de Hoy.
			IEnumerable<dynamic> lastDayActivity
				= (
					from d in CurrentUser.UserDataHourlyDistance
					where
						d.Activity != DataActivity.Sleep
						&& d.Timestamp > DateTime.UtcNow.AddHours(-24)
					group d by new {
						userGuid
							= d.User.Guid,
						year
							= d.Timestamp.Year,
						month
							= d.Timestamp.Month,
						day
							= d.Timestamp.Day,
						hour
							= d.Timestamp.Hour
					} into g
					select new {
						hour
							= g.Key.hour,
						distance
							= g.Sum(s => s.Distance),
						steps
							= g.Sum(s => s.Steps)
					}
				);

			// > Obtener sumatoria mensual de los últimos 12 meses
			IEnumerable<dynamic> monthlyActivity 
				= (
					from d in CurrentUser.UserDataHourlyDistance
					where d.Timestamp >= DateTime.UtcNow.AddMonths(-12)
					group d by new {
						year = d.Timestamp.Year,
						month = d.Timestamp.Month
					} into g
					select new {
						month
							= g.Key.month,
						totalDistance
							= g.Sum(s => s.Distance),
						totalSteps
							= g.Sum(s => s.Steps)
					}
				);
			
			// > Obtener sumatoria total por actividad
			IEnumerable<dynamic> activityDistribution
				= (
					from d in CurrentUser.UserDataTotalDistance
					where d.Timestamp >= DateTime.UtcNow.AddMonths(-12)
					group d by new {
						activity = d.Activity
					} into g
					select new {
						activity
							= g.Key.activity,
						totalDistance
							= g.Sum(s => s.TotalDistance),
						totalSteps
							= g.Sum(s => s.TotalSteps)
					}
				);

			// > Preparar respuesta en JSON
			var json = new {
				daily = new {
					labels = new List<string>(),
					values = new List<int>()
				},
				monthly = new {
					labels = new List<string>(),
					values = new List<int>()
				},
				activity = new {
					vabels = new List<string>(),
					values = new List<int>()
				},
			};

			return Json(json);
		}

		[Authorize]
		public JsonResult Tips(string cat, int page = 1, int tipsPerPage = 10) {
			// > Validar Tips por Página
			if ( tipsPerPage > 100 )
				throw new HttpException(400, "Tips Per Page is too high");

			// > Validar categoría
			TipCategory tipCategory
				= Database.TipCategoryStore.Get(cat);

			if ( tipCategory == null )
				throw new HttpException(404, "Category not found");

			// > Obtener Tips de la Categoría
			IEnumerable<dynamic> tips
				= Database.UserTipHistoryStore.GetAll(
					filter: f =>
						f.User.Guid == CurrentUser.Guid,
					orderBy: o =>
						o.OrderByDescending(b => b.CreationDate),
					extra: x =>
						x.Skip(page * tipsPerPage).Take(tipsPerPage),
					include:
						new string[] { "Tip" }
				).Select( s =>
					s.Tip.GetGlobalization(
						CultureInfo.CurrentCulture
					)
				).Select( s =>
					new {
						text
							= s.Text,
						source
							= s.Source
					}
				);

			// > Devolver respuesta en JSON
			return Json(tips);
		}
	}
}