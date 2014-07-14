using Kms.Cloud.Database.Helpers;
using Kms.Cloud.WebApp.Models.Views;
using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kms.Cloud.WebApp.Controllers {
	public class OverviewController : BaseController {
		//
		// GET: /Overview/
		[Authorize]
		public ActionResult Index() {
			var modelValues = new OverviewValues();
			
			// > Obtener Tip del Día (último Tip)
			var lastTip = Database.UserTipHistoryStore.GetFirst(
				filter: f =>
					f.User.Guid == CurrentUser.Guid,
				orderBy: o =>
					o.OrderByDescending(b => b.CreationDate),
				include:
					new string[] { "Tip.TipCategory" }
			).Tip;

			modelValues.TipOfTheDay = new TipModel {
				Category = lastTip.TipCategory.GetGlobalization().Name,
				IconUri  = GetDynamicResourceUri(lastTip.TipCategory),
				Text     = lastTip.GetGlobalization().Text,
				TipId    = lastTip.Guid.ToBase64String()
			};
			
			// > Obtener registro de actividades de las últimas 24 hrs
			//   [MUST REVIEW + OPTIMIZE]
			var lowerBound  = DateTime.UtcNow.AddHours(-24);
			var lastDayData =  Database.DataStore.GetAll(
				filter: f =>
					f.User.Guid == CurrentUser.Guid
					&& f.Timestamp < DateTime.UtcNow
					&& f.Timestamp > lowerBound,
				orderBy: o =>
					o.OrderBy(b => b.Timestamp)
			).ToArray();

			// > Calcular Horas de Sueño
			//   [MUST REVIEW + OPTIMIZE]
			// Inicialización de variables temporales
			DateTime? tmpTimestamp = null;

			// Inicialización de variables de propiedades
			modelValues.TodayDistanceCentimeters = 0;
			modelValues.EquivalentCo2Grams = 0;

			foreach ( var data in lastDayData ) {
				if ( data.Activity == Kms.Cloud.Database.DataActivity.Sleep ) {
					// + Si no hay Timestamp inicial, establecerlo
					if ( !tmpTimestamp.HasValue )
						tmpTimestamp = data.Timestamp;                        
				} else {
					// + Si hay Timestamp inicial se calcula la diferencia temporal,
					//   se añade al total de Horas de Sueño y se "blanquea" el
					//   Timestamp inicial
					if ( tmpTimestamp.HasValue ) {
						modelValues.TodaySleepTime.Add(
							data.Timestamp - tmpTimestamp.Value
						);

						tmpTimestamp = null;
					}

					// + Sumar valores de otras propiedades
					modelValues.TodayDistanceCentimeters += data.Steps * data.StrideLength;
					modelValues.EquivalentCo2Grams       += data.EqualsCo2;
					modelValues.EquivalentKcalRaw        += data.EqualsKcal;
					modelValues.EquivalentCashRaw        += data.EqualsCash;
				}
			}

			// > Establecer Tip del Día
			modelValues.TipOfTheDay = this.LayoutValues.TipOfTheDay;

			// > Devolver la vista
			return View(modelValues);
		}
	}
}