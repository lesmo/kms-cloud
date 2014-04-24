using Kms.Cloud.WebApp.Models.Views;
using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kms.Cloud.WebApp.Controllers {
	public class RewardsController : BaseController {
		private const int RewardsPerPage = 10;

		// GET: /Rewards/
		public ActionResult Index(int page = 1) {
			// Validar el número de Página
			if ( page < 1 )
				return RedirectToAction("Index", new {
					page = 1
				});
			else
				page--;

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
						x.Skip(page * RewardsPerPage).Take(RewardsPerPage)
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
							= GetDynamicResourceUri(
								"Images",
								s.Reward.Guid.ToBase64String(),
								s.Reward.PictureExtension
							),
						SponsorIcon
							= s.Reward.RewardSponsor == null ? null
							: GetDynamicResourceUri(
								"Images",
								s.Reward.RewardSponsor.Guid.ToBase64String(),
								s.Reward.RewardSponsor.PictureExtension
							),
						SponsorName
							= s.Reward.RewardSponsor == null ? null
							: s.Reward.RewardSponsor.Name,
						
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