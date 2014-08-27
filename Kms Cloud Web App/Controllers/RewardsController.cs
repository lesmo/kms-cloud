using Kms.Cloud.WebApp.Models.Views;
using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kms.Cloud.WebApp.Controllers {
	[KmsAuthorize]
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
			var modelValues = new RewardsValues();

			// > Obtener las próximas 5 + 1 recompensas (+1 para sección inicial)
			modelValues.NextRewards = Database.RewardStore.GetAllForRegion(
				regionCode:
					CurrentUser.RegionCode,
				filter: f =>
					f.DistanceTrigger > CurrentUser.UserDataTotalDistanceSum.TotalDistance,
				orderBy: o =>
					o.OrderBy(b => b.DistanceTrigger),
				extra: x =>
					x.Take(6)
			).Select(s => new RewardUnknownModel {
				RemainingDistanceCentimeters =
					(double)(s.DistanceTrigger - CurrentUser.UserDataTotalDistanceSum.TotalDistance),
				TriggerDistanceCentimeters =
					(long)s.DistanceTrigger
			}).ToArray();

			// > Obtener las últimas recompensas
			modelValues.UnlockedRewards = Database.UserEarnedRewardStore.GetAll(
				filter: f =>
					f.User.Guid == CurrentUser.Guid
					&& f.Discarded == true,
				orderBy: o =>
					o.OrderByDescending(b => b.CreationDate),
				extra: x =>
					x.Skip(page * RewardsPerPage).Take(RewardsPerPage),
				include:
					new string[] { "Reward" }
			).Select(s => new RewardModel {
				IconUri     = GetDynamicResourceUri(s.Reward),
				SponsorIcon = s.Reward.RewardSponsor == null
					? null
					: GetDynamicResourceUri(s.Reward.RewardSponsor),
				SponsorName = s.Reward.RewardSponsor == null
					? null
					: s.Reward.RewardSponsor.Name,
					
				TriggerDistanceCentimeters = (long)s.Reward.DistanceTrigger,
				UnlockDate                 = s.CreationDate,

				Title = s.Reward.GetGlobalization().Title,
				Text  = s.Reward.GetGlobalization().Text
			}).ToArray();

			// > Calcular páginas totales disponibles
			modelValues.TotalPages = (int)Math.Ceiling(
				(double)CurrentUser.UserEarnedReward.Count() / RewardsPerPage
			);

			// > Devolver la vista
			return View(modelValues);
		}

		public ActionResult Detail(string id) {
			var earnedReward = Database.UserEarnedRewardStore.Get(id);

			if ( earnedReward == null )
				return HttpNotFound();

		    var rewardModel = new RewardModel {
		        IconUri = GetDynamicResourceUri(earnedReward.Reward),
		        SponsorIcon = earnedReward.Reward.RewardSponsor == null
		                          ? null
		                          : GetDynamicResourceUri(earnedReward.Reward),
		        SponsorName = earnedReward.Reward.RewardSponsor == null
		                          ? null
		                          : earnedReward.Reward.RewardSponsor.Name,

		        TriggerDistanceCentimeters = (long)earnedReward.Reward.DistanceTrigger,
		        UnlockDate = earnedReward.CreationDate,

		        Title = earnedReward.Reward.GetGlobalization().Title,
		        Text = earnedReward.Reward.GetGlobalization().Text
		    };

		    return View(rewardModel);
		}
	}
}