using Kilometros_WebApp.Models.Views;
using KilometrosDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kilometros_WebApp.Controllers {
	public class RewardsController : BaseController {
		// GET: /Rewards/
		public ActionResult Index() {
			// > Inicializar valores de Vista
			RewardsValues rewardsValues
				= new RewardsValues();

			// > Obtener las próximas 5 + 1 recompensas (+1 para sección inicial)
			rewardsValues.NextRewards
				= Database.RewardStore.GetAllForRegion(
					regionCode:
						CurrentUser.RegionCode,
					filter: f =>
						f.DistanceTrigger > CurrentUser.UserDataTotalDistanceSum.TotalDistance,
					orderBy: o =>
						o.OrderBy(b => b.DistanceTrigger),
					extra: x =>
						x.Take(6)
				).Select(s =>
					new RewardUnknownModel() {
						RemainingDistanceCentimeters
							= s.DistanceTrigger - CurrentUser.UserDataTotalDistanceSum.TotalDistance,
						TriggerDistanceCentimeters
							= s.DistanceTrigger
					}
				).ToArray();

			// > Obtener las últimas 5 recompensas desbloqueadas
			rewardsValues.UnlockedRewards
				= Database.UserEarnedRewardStore.GetAll(
					filter: f =>
						f.User.Guid == CurrentUser.Guid
						&& f.Discarded == true,
					orderBy: o =>
						o.OrderByDescending(b => b.CreationDate),
					extra: x =>
						x.Take(10),
					include:
						new string[] { "Reward" }
				).Select(s =>
					new RewardModel() {
						IconUri
							= null,
						SponsorIcon
							= null,
						SponsorName
							= "NOT IMPLEMENETED",
						
						TriggerDistanceCentimeters
							= s.Reward.DistanceTrigger,
						UnlockDate
							= s.CreationDate,

						Title
							= s.Reward.GetGlobalization().Title,
						Text
							= s.Reward.GetGlobalization().Text
					}
				).ToArray();

			// > Calcular páginas totales disponibles
			//   TODO: Calcular... duh
			rewardsValues.TotalPages
				= 10;

			// > Preparar valores para la vista
			ViewData.Add(
				"LayoutValues",
				this.LayoutValues
			);
			ViewData.Add(
				"RewardsValues",
				rewardsValues
			);

			return View();
		}
	}
}