using Kms.Cloud.Database;
using Kms.Cloud.Database.Helpers;
using Kms.Cloud.WebApp.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kms.Cloud.WebApp.Controllers {
    [KmsAuthorize]
    public class ProfilesController : BaseController {
        // GET: Profiles
        public ActionResult Index(string id) {
            if ( string.IsNullOrEmpty(id) )
                return Redirect("~/Overview");

            var user = Database.UserStore.Get(id);

            if ( user == null )
                return Redirect("~/Overview");

            var profile = new ProfileModel(user, this);
            profile.UnlockedRewards = Database.UserEarnedRewardStore.GetAll(
                filter: f =>
                    f.User.Guid == user.Guid,
                orderBy: o =>
                    o.OrderBy(b => b.CreationDate),
                extra: x =>
                    x.Take(12),
                include: new string[] { "Reward" }
            ).Select(s => new RewardModel {
                IconUri     = GetDynamicResourceUri(s.Reward),
                SponsorIcon = s.Reward.RewardSponsor == null
                    ? null
                    : GetDynamicResourceUri(s.Reward.RewardSponsor),
                SponsorName = s.Reward.RewardSponsor == null
                    ? null
                    : s.Reward.RewardSponsor.Name,

                RewardId = s.Guid.ToBase64String(),

                Title = s.Reward.GetGlobalization().Title,
                Text = s.Reward.GetGlobalization().Text,

                TriggerDistanceCentimeters = (long)s.Reward.DistanceTrigger,
                UnlockDate = s.CreationDate
            });

            return View(profile);
        }
    }
}