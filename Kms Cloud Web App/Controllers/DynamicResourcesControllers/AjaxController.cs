using Kms.Cloud.Database;
using Kms.Cloud.Database.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kms.Cloud.WebApp.Controllers {
	public class AjaxController : BaseController {
		// GET: /DynamicResources/Ajax/Overview.json
		public JsonResult Overview() {
			// > Obtener últimas 24hrs. de lecturas
			//   [MUST REVIEW] Las lecturas deberían distinguir entre la actividad {Correr},
			//                 {Caminar} y {Sueño}. Actualmente se "agregan" todas las actividades
			//                 que no correspondan a {Sueño}, puesto que la vista NO dispone de
			//                 ésta diferenciación en la Gráfica del día de Hoy.
			var lastData = (
				from d in CurrentUser.UserDataHourlyDistance
				where
					d.Activity != DataActivity.Sleep
				orderby d.Timestamp
				select d
			).Take(1).FirstOrDefault();

			var yesterdayStart = lastData == null
				? DateTime.UtcNow.AddHours(-24)
				: lastData.Timestamp.AddHours(-24);
			
			var lastDayActivity = (
				from d in CurrentUser.UserDataHourlyDistance
				where
					d.Activity != DataActivity.Sleep
					&& d.Timestamp > yesterdayStart
				group d by new {
					userGuid = d.User.Guid,
					year     = d.Timestamp.Year,
					month    = d.Timestamp.Month,
					day      = d.Timestamp.Day,
					hour     = d.Timestamp.Hour
				} into g
				select new {
					hour     = new DateTime(new TimeSpan(g.Key.hour, 0, 0).Ticks).ToString("h tt"),
					distance = RegionInfo.CurrentRegion.IsMetric
						? g.Sum(s => s.Distance).CentimetersToKilometers()
						: g.Sum(s => s.Distance).CentimetersToMiles(),
					steps    = g.Sum(s => s.Steps)
				}
			);

			// Obtener sumatoria por mes de los últimos 12 meses
			var lastMonthStart  = lastData.Timestamp.AddMonths(-12);
			var monthlyActivity = (
				from d in CurrentUser.UserDataHourlyDistance
				where d.Timestamp > lastMonthStart
				group d by new {
					year  = d.Timestamp.Year,
					month = d.Timestamp.Month
				} into g
				orderby g.Key.year, g.Key.month
				select new {
					year  = g.Key.year,
					month = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
						CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.month)
					),
					totalDistance = RegionInfo.CurrentRegion.IsMetric
						? g.Sum(s => s.Distance).CentimetersToKilometers()
						: g.Sum(s => s.Distance).CentimetersToMiles(),
					totalSteps    = g.Sum(s => s.Steps)
				}
			);

			object[] monthlyActivityFinal;
			var firstMonthlyActivity = monthlyActivity.FirstOrDefault();

			if ( monthlyActivity.Count(c => c.year == firstMonthlyActivity.year) == monthlyActivity.Count() ) {
				monthlyActivityFinal = monthlyActivity.ToArray();
			} else {
				var lastYear = firstMonthlyActivity.year;
				var monthlyActivityFinalList = new List<object>();
				var month = "";

				foreach ( var activity in monthlyActivity.Reverse() ) {
					if (activity.year == firstMonthlyActivity.year )
						month = activity.month;
					else
						month = activity.month + " (" + activity.year.ToString() + ")";

					monthlyActivityFinalList.Add(new {
						year          = activity.year,
						month         = month,
						totalDistance = activity.totalDistance,
						totalSteps    = activity.totalSteps
					});
				}

				monthlyActivityFinalList.Reverse();
				monthlyActivityFinal = monthlyActivityFinalList.ToArray();
			}
			
			// > Obtener sumatoria total por actividad
			var lastWeekStart = lastData.Timestamp.AddDays(-7);
			var activityDistribution = (
				from d in CurrentUser.UserDataTotalDistance
				where d.Timestamp >= lastWeekStart
				group d by new {
					activity = d.Activity
				} into g
				select new {
					activity      = g.Key.activity.ToString().ToLower(),
					totalDistance = g.Sum(s => s.TotalDistance),
					totalSteps    = g.Sum(s => s.TotalSteps)
				}
			);

			// > Preparar respuesta en JSON
			return Json(
				new {
					daily    = lastDayActivity,
					monthly  = monthlyActivityFinal,
					activity = activityDistribution,
				},
				JsonRequestBehavior.AllowGet
			);
		}

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
				).Select( s =>
					s.Tip.GetGlobalization(
						CultureInfo.CurrentCulture
					)
				).Select( s =>
					new {
						text   = s.Text,
						source = s.Source
					}
				);

			// > Devolver respuesta en JSON
			return Json(tips, JsonRequestBehavior.AllowGet);
		}

		public JsonResult FriendList(int page = 1, int perPage = 18) {
			// > Validar items por Página
			if ( perPage > 40 )
				throw new HttpException(400, "Friends Per Page is too high");

			if ( page < 1 )
				throw new HttpException(400, "Page number is invalid");
			else
				page--;

			var friends = Database.UserFriendStore.GetAll(
				filter: f =>
					(
						f.User.Guid == CurrentUser.Guid
						|| f.Friend.Guid == CurrentUser.Guid
					) && f.Accepted == true,
				orderBy: o =>
					o.OrderByDescending(b => b.CreationDate),
				extra: x =>
					x.Skip(page * perPage).Take(perPage),
				include:
					new string[] { "User.UserDataTotalDistance" }
			).Select(s =>
				// + Obtener sólo el Objeto de Usuario del Amigo, no del Usuario actual
				s.User.Guid == CurrentUser.Guid
					? s.Friend.UserDataTotalDistanceSum
					: s.User.UserDataTotalDistanceSum
			).Select(s => new {
				userId     = s.User.Guid.ToBase64String(),
				pictureUri = s.User.PictureUri,
				name       = s.User.Name,
				lastName   = s.User.LastName,
					
				totalDistance = RegionInfo.CurrentRegion.IsMetric
					? s.TotalDistance.CentimetersToKilometers()
					: s.TotalDistance.CentimetersToMiles(),
					
				totalKcal = s.TotalKcal,
				totalCo2  = s.TotalCo2,
				totalCash = s.TotalCash.DollarCentsToRegionCurrency()
			});

			return Json(friends, JsonRequestBehavior.AllowGet);
		}
		
		public JsonResult FriendRequests(int page = 1, int perPage = 10) {
			// > Validar items por Página
			if ( perPage > 40 )
				throw new HttpException(400, "Friend Requests Per Page is too high");

			var friendships = Database.UserFriendStore.GetAll(
				filter: f =>
					f.Friend.Guid == CurrentUser.Guid
					&& f.Accepted == false,
				orderBy: o =>
					o.OrderByDescending(b => b.CreationDate),
				extra: x =>
					x.Skip(page * perPage).Take(perPage),
				include:
					new string[] { "User.UserDataTotalDistance" }
			).Select(s =>
				new {
					userId     = s.User.Guid.ToBase64String(),
					pictureUri = s.User.PictureUri,
					name       = s.User.Name,
					lastName   = s.User.LastName,

					totalDistance
						= RegionInfo.CurrentRegion.IsMetric
						? s.User.UserDataTotalDistanceSum.TotalDistance.CentimetersToKilometers()
						: s.User.UserDataTotalDistanceSum.TotalDistance.CentimetersToMiles(),

					totalKcal = s.User.UserDataTotalDistanceSum.TotalKcal,
					totalCo2  = s.User.UserDataTotalDistanceSum.TotalCo2,
					totalCash = s.User.UserDataTotalDistanceSum.TotalCash.DollarCentsToRegionCurrency()
				}
			);

			return Json(friendships, JsonRequestBehavior.AllowGet);
		}
		
		public JsonResult FriendRequestAccept(string friendId) {
			// > Buscar la Amistad
			var friendGuid = new Guid().FromBase64String(friendId);
			var friendship = Database.UserFriendStore.GetFirst(
				filter: f =>
					f.User.Guid == friendGuid
					&& f.Friend.Guid == CurrentUser.Guid
					&& f.Accepted == false
			);

			if ( friendship == null )
				throw new HttpException(404, "Friendship not found");

			// > Aceptar la Amistad
			friendship.Accepted = true;

			Database.UserFriendStore.Update(friendship);
			Database.SaveChanges();

			// > Devolver respuesta OK
			return Json(new {
				ok = true
			});
		}

		public JsonResult FriendRequestReject(string friendId) {
			// > Buscar la Amistad
			var friendGuid = new Guid().FromBase64String(friendId);
			var friendship = Database.UserFriendStore.GetFirst(
				filter: f =>
					f.User.Guid == friendGuid
					&& f.Friend.Guid == CurrentUser.Guid
					&& f.Accepted == false
			);

			if ( friendship == null )
				throw new HttpException(404, "Friendship not found");

			// > Eliminar la solicitud de Amistad
			Database.UserFriendStore.Delete(friendship);
			Database.SaveChanges();

			// > Devolver respuesta OK
			return Json(new {
				ok = true
			});
		}
		
		public JsonResult Rewards(int page = 1, int perPage = 10) {
			// > Obtener las Recompensas Adquiridas por el Usuario
			var rewards = Database.UserEarnedRewardStore.GetAll(
				filter: f =>
					f.User.Guid == CurrentUser.Guid
					&& f.Discarded == true,
				orderBy: o =>
					o.OrderByDescending(b => b.CreationDate),
				extra: x =>
					x.Skip(page * perPage).Take(perPage),
				include:
					new string[] { "Reward" }
			).Select(s =>
				new {
					iconUri = GetDynamicResourceUri(s.Reward),

					sponsorUri = s.Reward.RewardSponsor == null
						? null
						: s.Reward.RewardSponsor.WebsiteUri,
					sponsorName = s.Reward.RewardSponsor == null
						? null
						: s.Reward.RewardSponsor.Name,

					triggerDistance = s.Reward.DistanceTrigger,
					unlockDate      = s.CreationDate.ToShortDateString(),

					title = s.Reward.GetGlobalization().Title,
					text  = s.Reward.GetGlobalization().Text

				}
			);

			// > Devolver respuesta
			return Json(rewards, JsonRequestBehavior.AllowGet);
		}
	}
}