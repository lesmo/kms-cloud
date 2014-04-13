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
    public class GiftsController : IKMSController {
        /// <summary>
        ///     Devuelve el historial de los Regalos conseguidos por el Usuario.
        /// </summary>
        [HttpGet]
        [Route("my/gifts/history")]
        public IEnumerable<GiftResponse> GetRewards() {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Devuelve la información de canje del Regalo especificado.
        /// </summary>
        /// <param name="giftId">
        ///     ID del Regalo, obtenido en el Payload de la Notificación Push o en
        ///     el Historial de Recompensas (rewards/history).
        /// </param>
        [HttpGet]
        [Route("my/gifts/{giftId}")]
        public GiftResponse GetRewardGift(string giftId) {
            User user
                = OAuth.Token.User;

            // --- Buscar Regalo solicitado ---
            Guid? giftGuid
                = MiscHelper.GuidFromBase64(giftId);
            if ( ! giftGuid.HasValue )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning701_GiftNotFound
                );

            RewardGift rewardGift
                = Database.RewardGiftStore.Get(
                    giftGuid.Value
                );
            if ( rewardGift == null )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning701_GiftNotFound
                );

            // --- Verificar que el Regalo haya sido reclamado por el Usuario ---
            UserRewardGiftClaimed userRewardGiftClaim
                = Database.UserRewardGiftClaimedStore.GetFirst(
                    r => r.RewardGift.Guid == rewardGift.Guid && r.RedeemedByUser.Guid == user.Guid
                );
            if ( userRewardGiftClaim == null )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning701_GiftNotFound
                );

            // --- Obtener Información del Regalo ---
            // Obtener fotografías del Regalo
            List<string> rewardGiftPictures
                = new List<string>();
            foreach ( RewardGiftPicture picture in rewardGift.RewardGiftPictures )
                rewardGiftPictures.Add(
                    picture.Guid.ToString() + "." + picture.PictureExtension
                );

            // --- Obtener Regalo en el Idioma actual ---
            RewardGiftGlobalization rewardGiftGlobalization
                = Database.RewardGiftStore.GetGlobalization(
                    rewardGift
                ) as RewardGiftGlobalization;

            if ( rewardGiftGlobalization == null ) {
                return new GiftResponse() {
                    NamePlural
                        = null,
                    NameSingular
                        = null,

                    RedeemCode
                        = userRewardGiftClaim.RedeemCode,
                    RedeemPicture
                        = string.Format(
                            "{0}.{1}",
                            userRewardGiftClaim.Guid.ToBase64String(),
                            userRewardGiftClaim.PictureExtension
                        ),

                    Pictures
                        = rewardGiftPictures.ToArray()
                };
            } else {
                return new GiftResponse() {
                    NamePlural
                        = rewardGiftGlobalization.NamePlural,
                    NameSingular
                        = rewardGiftGlobalization.NameSingular,

                    RedeemCode
                        = userRewardGiftClaim.RedeemCode,
                    RedeemPicture
                        = string.Format(
                            "{0}.{1}",
                            userRewardGiftClaim.Guid.ToBase64String(),
                            userRewardGiftClaim.PictureExtension
                        ),

                    Pictures
                        = rewardGiftPictures.ToArray()
                };
            }
        }

        /// <summary>
        ///     Reclama o canjea el Regalo conseguido por el Usuario.
        /// </summary>
        /// <param name="giftId">
        ///     ID del Regalo, obtenido en el Payload de la Notificación Push o en
        ///     el Historial de Recompensas (rewards/history).
        /// </param>
        [HttpPost]
        [Route("my/gifts/{giftId}")]
        public GiftClaimResponse ClaimRewardGift(string giftId) {
            User user
                = OAuth.Token.User;

            // --- Buscar Regalo solicitado ---
            Guid? giftGuid
                = MiscHelper.GuidFromBase64(giftId);
            if ( !giftGuid.HasValue )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning701_GiftNotFound
                );

            RewardGift rewardGift
                = Database.RewardGiftStore.Get(
                    giftGuid.Value
                );
            if ( rewardGift == null )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning701_GiftNotFound
                );

            // --- Verificar que el Regalo haya sido obtenido por el Usuario ---
            UserEarnedReward userEarnedReward
                = Database.UserEarnedRewardStore.GetFirst(
                    r => r.User == user && r.Reward.RewardGift.Contains(rewardGift)
                );
            if ( userEarnedReward == null )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning701_GiftNotFound
                );

            // --- Verificar que el Regalo no haya sido Reclamado por el Usuario ---
            bool userClaimedGift
                = (
                    from r in rewardGift.UserRewardGiftClaimed
                    where r.RedeemedByUser.Guid == user.Guid && r.RewardGift.Guid == giftGuid.Value
                    select r
                ).FirstOrDefault() != null;
            if ( userClaimedGift )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning701_GiftNotFound
                );

            // --- Verificar que el Usuario tenga Información de Envío si el Regalo se envía ---
            if ( rewardGift.IsShipped ) {
                ShippingInformation shippingInformation
                    = user.ShippingInformation;

                if ( shippingInformation == null )
                    throw new HttpConflictException(
                        ControllerStrings.Warning205_ShippingInfoNotSet
                    );
            }

            // --- Obtener un objeto de Reclamo ---
            UserRewardGiftClaimed giftClaim
                = Database.UserRewardGiftClaimedStore.GetFirst(
                    r => r.RedeemedByUser == null && r.RewardGift == rewardGift
                );
            if ( giftClaim == null )
                throw new HttpNoContentException(
                    ControllerStrings.Warning702_GiftOutOfStock
                );

            // --- Asociar al Usuario con el Reclamo (efectivamente reclamando el regalo del usuario) ---
            giftClaim.RedeemedByUser
                = user;

            Database.UserRewardGiftClaimedStore.Update(giftClaim);
            Database.SaveChanges();

            // --- Devolver detalles de canje ---
            return new GiftClaimResponse() {
                ExpirationDate
                    = giftClaim.ExpirationDate.HasValue
                    ? giftClaim.ExpirationDate.Value
                    : DateTime.MaxValue,
                RedeemCode
                    = giftClaim.RedeemCode,
                RedeemPicture
                    = string.Format(
                        "{0}.{1}",
                        giftClaim.Guid.ToBase64String(),
                        giftClaim.PictureExtension
                    )
            };
        }
    }
}
