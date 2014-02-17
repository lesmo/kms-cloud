using Kilometros_WebAPI.Exceptions;
using Kilometros_WebAPI.Models.HttpGet.RewardsController;
using Kilometros_WebAPI.Security;
using KilometrosDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kilometros_WebAPI.Controllers {
    [Authorize]
    public class RewardsController : ApiController {
        KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();

        [HttpGet]
        [Route("rewards/history")]
        public RewardResponse[] GetRewardsHistory(int page = 1) {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;

            /** Obtener la última Recompensa conseguida **/
            UserEarnedReward lastReward
                = Database.UserEarnedRewardStore.GetFirst(
                    r => r.User == user,
                    o => o.OrderBy(by => by.CreationDate)
                );

            /** Verificar si se tiene la cabecera {If-Modified-Since} **/
            DateTimeOffset? ifModifiedSince
                = Request.Headers.IfModifiedSince;

            if ( ifModifiedSince.HasValue && lastReward != null ) {
                if ( ifModifiedSince.Value.UtcDateTime > lastReward.CreationDate )
                    throw new HttpNotModifiedException();
            }

            /** Obtener las Recompensas conseguidas **/
            IEnumerable<UserEarnedReward> rewards
                = (
                    from reward in user.UserEarnedReward
                    orderby reward.CreationDate
                    select reward
                ).Skip(10 * page).Take(10);

            /** Preparar respuesta **/
            List<RewardResponse> response
                = new List<RewardResponse>();

            foreach ( UserEarnedReward reward in rewards ) {
                // Obtener Recompensa en el idioma actual
                RewardGlobalization rewardGlobalization
                    = Database.RewardStore.GetGlobalization(
                        reward.Reward
                    ) as RewardGlobalization;
            }
        }
    }
}
