using KilometrosDatabase;
using KilometrosDatabase.Helpers;
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
							= RegionInfo.CurrentRegion.IsMetric
							? g.Sum(s => s.Distance).CentimetersToKilometers()
							: g.Sum(s => s.Distance).CentimetersToMiles(),
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
							= CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.month),
						totalDistance
							= RegionInfo.CurrentRegion.IsMetric
							? g.Sum(s => s.Distance).CentimetersToKilometers()
							: g.Sum(s => s.Distance).CentimetersToMiles(),
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
			return Json(
				new {
					daily
						= lastDayActivity,
					monthly
						= monthlyActivity,
					activity
						= activityDistribution,
				}
			);
		}

		[Authorize]
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
			IEnumerable<dynamic> tips
				= Database.UserTipHistoryStore.GetAll(
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
						text
							= s.Text,
						source
							= s.Source
					}
				);

			// > Devolver respuesta en JSON
			return Json(tips);
		}

		[Authorize]
		public JsonResult FriendList(int page = 1, int perPage = 18) {
			// > Validar items por Página
			if ( perPage > 40 )
				throw new HttpException(400, "Friends Per Page is too high");

			IEnumerable<dynamic> friends
				= Database.UserFriendStore.GetAll(
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
				).Select(s =>
					new {
						userId
							= s.User.Guid.ToBase64String(),
						name
							= s.User.Name,
						lastName
							= s.User.LastName,
						pictureUri
							= s.User.PictureUri,
						totalDistance
							= RegionInfo.CurrentRegion.IsMetric
							? s.TotalDistance.CentimetersToKilometers()
							: s.TotalDistance.CentimetersToMiles(),
						totalKcal
							= s.TotalKcal,
						totalCo2
							= s.TotalCo2,
						totalCash
							= s.TotalCash
					}
				);

			return Json(friends);
		}

		[Authorize]
		public JsonResult FriendRequests(int page = 1, int perPage = 10) {
			// > Validar items por Página
			if ( perPage > 40 )
				throw new HttpException(400, "Friend Requests Per Page is too high");

			IEnumerable<dynamic> friendships
				= Database.UserFriendStore.GetAll(
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
						userId
							= s.User.Guid.ToBase64String(),
						name
							= s.User.Name,
						lastName 
							= s.User.LastName,
						pictureUri
							= s.User.PictureUri,
						totalDistance
							= RegionInfo.CurrentRegion.IsMetric
							? s.User.UserDataTotalDistanceSum.TotalDistance.CentimetersToKilometers()
							: s.User.UserDataTotalDistanceSum.TotalDistance.CentimetersToMiles()
					}
				);

			return Json(friendships);
		}

		[Authorize]
		public JsonResult FriendRequestAccept(string friendId) {
			// > Buscar la Amistad
			Guid friendGuid
				= new Guid().FromBase64String(friendId);
			UserFriend friendship 
				= Database.UserFriendStore.GetFirst(
					filter: f =>
						f.User.Guid == friendGuid
						&& f.Friend.Guid == CurrentUser.Guid
						&& f.Accepted == false
				);

			if ( friendship == null )
				throw new HttpException(404, "Friendship not found");

			// > Aceptar la Amistad
			friendship.Accepted
				= true;

			Database.UserFriendStore.Update(friendship);
			Database.SaveChanges();

			// > Devolver respuesta OK
			return Json(new {
				ok = true
			});
		}

		[Authorize]
		public JsonResult Rewards(int page = 1, int perPage = 10) {
			// > Obtener las Recompensas Adquiridas por el Usuario
			IEnumerable<dynamic> rewards    
				= Database.UserEarnedRewardStore.GetAll(
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
						iconUri
							= Url.Content(
								string.Format(
									"~/DynamicResources/Images/{0}.{1}",
									s.Reward.Guid.ToBase64String(),
									s.Reward.PictureExtension
								)
							),
						sponsorUri
							= s.Reward.RewardSponsor.WebsiteUri,
						sponsorName
							= s.Reward.RewardSponsor.Name,

						triggerDistance
							= s.Reward.DistanceTrigger,
						unlockDate
							= s.CreationDate.ToShortDateString(),

						title
							= s.Reward.GetGlobalization().Title,
						text
							= s.Reward.GetGlobalization().Text

					}
				);

			// > Devolver respuesta
			return Json(rewards);
		}
	}
}