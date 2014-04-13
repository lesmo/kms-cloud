using Kilometros_WebApp.Models.Views;
using KilometrosDatabase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kilometros_WebApp.Controllers {
	public class OverviewController : BaseController {
		private OverviewValues OverviewValues {
			get {
				// > Devolver valores si ya se obtuvieron antes
				if ( this._overviewValues != null )
					return this._overviewValues;

				this._overviewValues
					= new OverviewValues();

				this._overviewValues.CultureInfo
					= CultureInfo.CurrentCulture;
				this._overviewValues.RegionInfo
					= new RegionInfo(
						this.CurrentUser.RegionCode
					);

				// > Obtener Tip del Día (último Tip)
				KilometrosDatabase.Tip lastTip
					= Database.UserTipHistoryStore.GetFirst(
						filter: f =>
							f.User.Guid == this.CurrentUser.Guid,
						orderBy: o =>
							o.OrderByDescending(b => b.CreationDate),
						include:
							new string[] { "Tip.TipCategory" }
					).Tip;
			   
				this._overviewValues.TipOfTheDayText
					= lastTip.GetGlobalization(
						this._overviewValues.CultureInfo
					).Text;
				this._overviewValues.TipOfTheDayCategory
					= lastTip.TipCategory.GetGlobalization<KilometrosDatabase.TipCategoryGlobalization>(
						this._overviewValues.CultureInfo
					).Name;
				this._overviewValues.TipOfTheDayCategoryIconUri
					= new Uri(
						Url.Content(
							string.Format(
								"DynamicResources/Images/{0}.{1}",
								lastTip.TipCategory.Guid.ToBase64String(),
								lastTip.TipCategory.PictureExtension
							)
						)
					);

				// > Obtener registro de actividades de las últimas 24 hrs
				//   [MUST REVIEW + OPTIMIZE]
				KilometrosDatabase.Data[] lastDayData
					=  Database.DataStore.GetAll(
						filter: f =>
							f.User.Guid == this.CurrentUser.Guid
							&& f.Timestamp < DateTime.UtcNow
							&& f.Timestamp > DateTime.UtcNow.AddHours(-24),
						orderBy: o =>
							o.OrderBy(b => b.Timestamp)
					).ToArray();

				// > Calcular Horas de Sueño
				//   [MUST REVIEW + OPTIMIZE]

				// Inicialización de variables temporales
				DateTime? tmpTimestamp
					= null;

				// Inicialización de variables de propiedades
				this._overviewValues.TodayDistanceCentimeters
					= this._overviewValues.EquivalentCo2Grams
					= 0;

				foreach ( KilometrosDatabase.Data data in lastDayData ) {
					if (
						data.Activity == KilometrosDatabase.DataActivity.Sleep
					) {
						// + Si no hay Timestamp inicial, establecerlo
						if ( !tmpTimestamp.HasValue )
							tmpTimestamp
								= data.Timestamp;                        
					} else {
						// + Si hay Timestamp inicial se calcula la diferencia temporal,
						//   se añade al total de Horas de Sueño y se "blanquea" el
						//   Timestamp inicial
						if ( tmpTimestamp.HasValue ) {
							this._overviewValues.TodaySleepTime.Add(
								data.Timestamp - tmpTimestamp.Value
							);

							tmpTimestamp
								= null;
						}

						// + Sumar valores de otras propiedades
						this._overviewValues.TodayDistanceCentimeters
							+= data.Steps * data.StrideLength;

						this._overviewValues.EquivalentCo2Grams
							+= data.EqualsCo2;
						this._overviewValues.EquivalentKcal
							+= data.EqualsKcal;
						this._overviewValues.EquivalentCash
							+= data.EqualsCash;
					}
				}

				// > Devolver valores
				return this._overviewValues;
			}
		}
		private OverviewValues _overviewValues;

		//
		// GET: /Overview/
		[Authorize]
		public ActionResult Index() {
			ViewData.Add(
				"LayoutValues",
				this.LayoutValues
			);
			ViewData.Add(
				"OverviewValues",
				this.OverviewValues
			);

			return View();
		}
	}
}