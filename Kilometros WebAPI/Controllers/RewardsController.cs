﻿using Kilometros_WebAPI.Exceptions;
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
    public class RewardsController : IKMSController {
        [HttpGet]
        [Route("rewards/history")]
        public IEnumerable<RewardResponse> GetRewardsHistory(int page = 1, int perPage = 20) {
            // --- Obtener la última Recompensa conseguida ---
            UserEarnedReward lastReward
                = Database.UserEarnedRewardStore.GetFirst(
                    filter: f =>
                        f.User.Guid == CurrentUser.Guid,
                    orderBy: o =>
                        o.OrderBy(by => by.CreationDate)
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
                = Database.UserEarnedRewardStore.GetAll(
                    filter: f =>
                        f.User.Guid == CurrentUser.Guid,
                    orderBy: o =>
                        o.OrderByDescending(b => b.CreationDate),
                    extra: x =>
                        x.Skip(page * perPage).Take(perPage),
                    include:
                        new string[] { "Reward.RewardGlobalization", "RewardGift.RewardPictures" }
                );

            // --- Preparar respuesta ---
            List<RewardResponse> response
                = new List<RewardResponse>();

            foreach ( UserEarnedReward earnedReward in rewards ) {
                // Obtener Recompensa en el idioma actual
                RewardGlobalization rewardGlobalization
                    = earnedReward.Reward.GetGlobalization();

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
                            where uc.RedeemedByUser.Guid == CurrentUser.Guid
                            select uc
                        ).FirstOrDefault() != null;

                    // Obtener Regalo en el Idioma actual
                    rewardGiftsList.Add(new RewardGiftResponse {
                        RewardGiftId
                            = rewardGift.Guid.ToBase64String(),

                        Stock
                            = rewardGift.Stock,
                        NamePlural 
                            = rewardGift.GetGlobalization().NamePlural,
                        NameSingular
                            = rewardGift.GetGlobalization().NameSingular,
                        Claimed
                            = userClaimed,

                        Pictures
                            = rewardGiftPictures.ToArray()
                    });
                }

                // Obtener Regiones a las que aplica la Recompensa
                List<string> rewardRegions
                    = new List<string>();
                foreach ( RewardRegionalization region in earnedReward.Reward.RewardRegionalization )
                    rewardRegions.Add(region.RegionCode);

                response.Add(new RewardResponse {
                    RewardId
                        = earnedReward.Guid.ToBase64String(),
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
            // --- Obtener la Recompensa solicitada ---
            UserEarnedReward reward
                = Database.UserEarnedRewardStore.Get(earnedRewardId);

            if ( reward == null )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning901_RewardNotFound
                );

            if ( reward.User.Guid != CurrentUser.Guid )
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
                        picture.Guid.ToBase64String()
                        + "." + picture.PictureExtension
                    );

                // Determinar si el Regalo se reclamó por el Usuario
                bool userClaimed
                    = rewardGift.UserRewardGiftClaimed.Where(w =>
                        w.RedeemedByUser.Guid == CurrentUser.Guid
                    ).FirstOrDefault() != null;

                // Obtener Regalo en el Idioma actual
                rewardGiftsList.Add(new RewardGiftResponse {
                    RewardGiftId
                        = rewardGift.Guid.ToBase64String(),

                    Stock
                        = rewardGift.Stock,
                    NamePlural
                        = rewardGift.GetGlobalization().NamePlural,
                    NameSingular
                        = rewardGift.GetGlobalization().NameSingular,
                    Claimed
                            = userClaimed,

                    Pictures
                        = rewardGiftPictures.ToArray()
                });
            }

            // --- Obtener Regiones a las que aplica la Recompensa ---
            List<string> rewardRegions
                = new List<string>();
            foreach ( RewardRegionalization region in reward.Reward.RewardRegionalization )
                rewardRegions.Add(region.RegionCode);

            // --- Preparar y devolver respuesta ---
            return new RewardResponse {
                RewardId
                    = reward.Guid.ToBase64String(),
                EarnDate
                    = reward.CreationDate,

                Title
                    = reward.Reward.GetGlobalization().Title,
                Text
                    = reward.Reward.GetGlobalization().Text,
                Source
                    = reward.Reward.GetGlobalization().Source,

                RewardGifts
                    = rewardGiftsList.ToArray(),
                RegionCodes
                    = rewardRegions.ToArray()
            };
        }
    }
}
