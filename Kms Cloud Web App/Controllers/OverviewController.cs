﻿using Kms.Cloud.Database.Helpers;
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
		private OverviewValues OverviewValues {
			get {
				// > Devolver valores si ya se obtuvieron antes
				if ( this._overviewValues != null )
					return this._overviewValues;

				this._overviewValues
					= new OverviewValues();
				
				// > Obtener Tip del Día (último Tip)
				Kms.Cloud.Database.Tip lastTip
					= Database.UserTipHistoryStore.GetFirst(
						filter: f =>
							f.User.Guid == CurrentUser.Guid,
						orderBy: o =>
							o.OrderByDescending(b => b.CreationDate),
						include:
							new string[] { "Tip.TipCategory" }
					).Tip;

				this.OverviewValues.TipOfTheDay
					= new TipModel() {
						Category
							= lastTip.TipCategory.GetGlobalization().Name,
						IconUri
							= GetDynamicResourceUri(
								"Images",
								lastTip.TipCategory.Guid.ToBase64String(),
								lastTip.TipCategory.PictureExtension
							),
						Text
							= lastTip.GetGlobalization().Text,
						TipId
							= lastTip.Guid.ToBase64String()
					};
			   
				// > Obtener registro de actividades de las últimas 24 hrs
				//   [MUST REVIEW + OPTIMIZE]
				DateTime lowerBound
					= DateTime.UtcNow.AddHours(-24);
				Kms.Cloud.Database.Data[] lastDayData
					=  Database.DataStore.GetAll(
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
				DateTime? tmpTimestamp
					= null;

				// Inicialización de variables de propiedades
				this._overviewValues.TodayDistanceCentimeters
					= 0;
				this._overviewValues.EquivalentCo2Grams
					= 0;

				foreach ( Kms.Cloud.Database.Data data in lastDayData ) {
					if (
						data.Activity == Kms.Cloud.Database.DataActivity.Sleep
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
						this._overviewValues.EquivalentKcalRaw
							+= data.EqualsKcal;
						this._overviewValues.EquivalentCashRaw
							+= data.EqualsCash;
					}
				}

				// > Establecer Tip del Dïa
				this._overviewValues.TipOfTheDay
					= this.LayoutValues.TipOfTheDay;

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