using Kilometros_WebAPI.Exceptions;
using Kilometros_WebAPI.Helpers;
using Kilometros_WebAPI.Models.ResponseModels;
using Kilometros_WebAPI.Security;
using Kilometros_WebGlobalization.API;
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
        public IEnumerable<RewardResponse> GetRewardsHistory(int page = 1) {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;

            // --- Obtener la última Recompensa conseguida ---
            UserEarnedReward lastReward
                = Database.UserEarnedRewardStore.GetFirst(
                    r => r.User == user,
                    o => o.OrderBy(by => by.CreationDate)
                );

            // --- Verificar si se tiene la cabecera {If-Modified-Since} ---
            DateTimeOffset? ifModifiedSince
                = Request.Headers.IfModifiedSince;

            if ( ifModifiedSince.HasValue && lastReward != null ) {
                if ( ifModifiedSince.Value.UtcDateTime > lastReward.CreationDate )
                    throw new HttpNotModifiedException();
            }

            // --- Obtener las Recompensas conseguidas ---
            IEnumerable<UserEarnedReward> rewards
                = (
                    from reward in user.UserEarnedReward
                    orderby reward.CreationDate
                    select reward
                ).Skip(10 * page).Take(10);

            // --- Preparar respuesta ---
            List<RewardResponse> response
                = new List<RewardResponse>();

            foreach ( UserEarnedReward earnedReward in rewards ) {
                // Obtener Recompensa en el idioma actual
                RewardGlobalization rewardGlobalization
                    = Database.RewardStore.GetGlobalization(
                        earnedReward.Reward
                    ) as RewardGlobalization;

                // Buscar y obtener regalos asociados
                IEnumerable<RewardGift> rewardGifts
                    = earnedReward.Reward.RewardGift;
                List<RewardGiftResponse> rewardGiftsList
                    = new List<RewardGiftResponse>();

                foreach ( RewardGift rewardGift in rewardGifts ) {
                    // Obtener fotografías del Regalo
                    List<string> rewardGiftPictures
                        = new List<string>();
                    foreach ( RewardGiftPicture picture in rewardGift.RewardGiftPictures )
                        rewardGiftPictures.Add(
                            picture.Guid.ToString() + "." + picture.PictureExtension
                        );

                    // Determinar si el Regalo se reclamó por el Usuario
                    bool userClaimed
                        = (
                            from uc in rewardGift.UserRewardGiftClaimed
                            where uc.RedeemedByUser == user
                            select uc
                        ).FirstOrDefault() != null;

                    // Obtener Regalo en el Idioma actual
                    RewardGiftGlobalization rewardGiftGlobalization
                        = Database.RewardGiftStore.GetGlobalization(
                            rewardGift
                        ) as RewardGiftGlobalization;
                    if ( rewardGiftGlobalization == null ) {
                        rewardGiftsList.Add(new RewardGiftResponse() {
                            RewardGiftId
                                = MiscHelper.Base64FromGuid(rewardGift.Guid),

                            Stock
                                = rewardGift.Stock,
                            NamePlural
                                = null,
                            NameSingular
                                = null,
                            Claimed
                                = userClaimed,

                            Pictures
                                = rewardGiftPictures.ToArray()
                        });
                    } else {
                        rewardGiftsList.Add(new RewardGiftResponse() {
                            RewardGiftId
                                = MiscHelper.Base64FromGuid(rewardGift.Guid),

                            Stock
                                = rewardGift.Stock,
                            NamePlural 
                                = rewardGiftGlobalization.NamePlural,
                            NameSingular
                                = rewardGiftGlobalization.NameSingular,
                            Claimed
                                = userClaimed,

                            Pictures
                                = rewardGiftPictures.ToArray()
                        });
                    }
                }

                // Obtener Regiones a las que aplica la Recompensa
                List<string> rewardRegions
                    = new List<string>();
                foreach ( RewardRegionalization region in earnedReward.Reward.RewardRegionalization )
                    rewardRegions.Add(region.RegionCode);

                response.Add(new RewardResponse() {
                    RewardId
                        = MiscHelper.Base64FromGuid(earnedReward.Guid),
                    EarnDate
                        = earnedReward.CreationDate,

                    Title
                        = rewardGlobalization.Title,
                    Text
                        = rewardGlobalization.Text,
                    Source
                        = rewardGlobalization.Source,

                    RewardGifts
                        = rewardGiftsList.ToArray(),
                    RegionCodes
                        = rewardRegions.ToArray()
                });
            }

            return response;
        }

        [HttpGet]
        [Route("rewards/{earnedRewardId}")]
        public RewardResponse GetReward(string earnedRewardId) {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;

            // --- Obtener la Recompensa solicitada ---
            Guid? rewardGuid
                = MiscHelper.GuidFromBase64(earnedRewardId);
            if ( ! rewardGuid.HasValue )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning901_RewardNotFound
                );

            UserEarnedReward reward
                = Database.UserEarnedRewardStore.Get(rewardGuid.Value);
            if ( reward == null )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning901_RewardNotFound
                );
            if ( reward.User.Guid != user.Guid )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning901_RewardNotFound
                );

            // --- Buscar y obtener regalos asociados ---
            IEnumerable<RewardGift> rewardGifts
                = reward.Reward.RewardGift;
            List<RewardGiftResponse> rewardGiftsList
                = new List<RewardGiftResponse>();

            foreach ( RewardGift rewardGift in rewardGifts ) {
                // Obtener fotografías del Regalo
                List<string> rewardGiftPictures
                    = new List<string>();
                foreach ( RewardGiftPicture picture in rewardGift.RewardGiftPictures )
                    rewardGiftPictures.Add(
                        picture.Guid.ToString("00000000000000000000000000000000")
                        + "." + picture.PictureExtension
                    );

                // Determinar si el Regalo se reclamó por el Usuario
                bool userClaimed
                    = (
                        from uc in rewardGift.UserRewardGiftClaimed
                        where uc.RedeemedByUser.Guid == user.Guid
                        select uc
                    ).FirstOrDefault() != null;

                // Obtener Regalo en el Idioma actual
                RewardGiftGlobalization rewardGiftGlobalization
                    = Database.RewardGiftStore.GetGlobalization(
                        rewardGift
                    ) as RewardGiftGlobalization;

                if ( rewardGiftGlobalization == null ) {
                    rewardGiftsList.Add(new RewardGiftResponse() {
                        RewardGiftId
                            = MiscHelper.Base64FromGuid(rewardGift.Guid),

                        Stock
                            = rewardGift.Stock,
                        NamePlural
                            = null,
                        NameSingular
                            = null,
                        Claimed
                            = userClaimed,

                        Pictures
                            = rewardGiftPictures.ToArray()
                    });
                } else {
                    rewardGiftsList.Add(new RewardGiftResponse() {
                        RewardGiftId
                            = MiscHelper.Base64FromGuid(rewardGift.Guid),

                        Stock
                            = rewardGift.Stock,
                        NamePlural
                            = rewardGiftGlobalization.NamePlural,
                        NameSingular
                            = rewardGiftGlobalization.NameSingular,
                        Claimed
                                = userClaimed,

                        Pictures
                            = rewardGiftPictures.ToArray()
                    });
                }
            }

            // --- Obtener cadenas en el Idioma actual ---
            RewardGlobalization rewardGlobalization
                 = Database.RewardStore.GetGlobalization(reward.Reward) as RewardGlobalization;

            // --- Obtener Regiones a las que aplica la Recompensa ---
            List<string> rewardRegions
                = new List<string>();
            foreach ( RewardRegionalization region in reward.Reward.RewardRegionalization )
                rewardRegions.Add(region.RegionCode);

            // --- Preparar y devolver respuesta ---
            return new RewardResponse() {
                RewardId
                    = MiscHelper.Base64FromGuid(reward.Guid),
                EarnDate
                    = reward.CreationDate,

                Title
                    = rewardGlobalization.Title,
                Text
                    = rewardGlobalization.Text,
                Source
                    = rewardGlobalization.Source,

                RewardGifts
                    = rewardGiftsList.ToArray(),
                RegionCodes
                    = rewardRegions.ToArray()
            };
        }
    }
}
