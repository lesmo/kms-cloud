using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Kms.Cloud.WebApp.Controllers {
    public partial class AjaxController {
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
                    unlockDate = s.CreationDate.ToShortDateString(),

                    title = s.Reward.GetGlobalization().Title,
                    text = s.Reward.GetGlobalization().Text
                }
            );

            // > Devolver respuesta
            return Json(rewards, JsonRequestBehavior.AllowGet);
        }

        public HttpStatusCodeResult DiscardReward(string id) {
            var earnedReward = Database.UserEarnedRewardStore.Get(id);

            if ( earnedReward == null || earnedReward.User.Guid != CurrentUser.Guid || earnedReward.Discarded )
                return new HttpStatusCodeResult(404);

            earnedReward.Discarded = true;
            Database.SaveChanges();

            return new HttpStatusCodeResult(200);
        }
    }
}