using Kms.Cloud.Api.Exceptions;
using Kms.Cloud.Api.Models.ResponseModels;
using Kms.Cloud.Api.Security;
using Kilometros_WebGlobalization.API;
using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace Kms.Cloud.Api.Controllers {
    [Authorize]
    public class GiftsController : BaseController {
        /// <summary>
        ///     Devuelve el historial de los Regalos conseguidos por el Usuario.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [HttpGet, Route("my/gifts/history")]
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
            // --- Buscar Regalo solicitado ---
            RewardGift rewardGift
                = Database.RewardGiftStore[giftId];
            if ( rewardGift == null )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning701_GiftNotFound
                );

            // --- Verificar que el Regalo haya sido reclamado por el Usuario ---
            UserRewardGiftClaimed userRewardGiftClaim
                = Database.UserRewardGiftClaimedStore.GetFirst(
                    filter: f =>
                        f.RewardGift.Guid == rewardGift.Guid
                        && f.RedeemedByUser.Guid == CurrentUser.Guid
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
            return new GiftResponse() {
                NamePlural
                    = rewardGift.GetGlobalization().NamePlural,
                NameSingular
                    = rewardGift.GetGlobalization().NameSingular,

                RedeemCode
                    = userRewardGiftClaim.RedeemCode,
                RedeemPicture
                    = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}",
                        userRewardGiftClaim.Guid.ToBase64String(),
                        userRewardGiftClaim.PictureExtension
                    ),

                Pictures
                    = rewardGiftPictures
            };
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
            // --- Buscar Regalo solicitado ---
            RewardGift rewardGift
                = Database.RewardGiftStore.Get(giftId);
            if ( rewardGift == null )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning701_GiftNotFound
                );

            // --- Verificar que el Regalo haya sido obtenido por el Usuario ---
            UserEarnedReward userEarnedReward
                = Database.UserEarnedRewardStore.GetFirst(
                    filter: f =>
                        f.User.Guid == CurrentUser.Guid 
                        && f.Reward.RewardGift.Any(r => r.Guid == rewardGift.Guid)
                );

            if ( userEarnedReward == null )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning701_GiftNotFound
                );

            // --- Verificar que el Regalo no haya sido Reclamado por el Usuario ---
            bool userClaimedGift
                = rewardGift.UserRewardGiftClaimed.Where(w =>
                    w.RedeemedByUser.Guid == CurrentUser.Guid
                    && w.RewardGift.Guid == rewardGift.Guid
                ).FirstOrDefault() != null;

            if ( userClaimedGift )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning701_GiftNotFound
                );

            // --- Verificar que el Usuario tenga Información de Envío si el Regalo se envía ---
            if ( rewardGift.IsShipped ) {
                ShippingInformation shippingInformation
                    = CurrentUser.ShippingInformation;

                if ( shippingInformation == null )
                    throw new HttpConflictException(
                        ControllerStrings.Warning205_ShippingInfoNotSet
                    );
            }

            // --- Obtener un objeto de Reclamo ---
            UserRewardGiftClaimed giftClaim
                = Database.UserRewardGiftClaimedStore.GetFirst(
                    filter: f =>
                        f.RedeemedByUser == null
                        && f.RewardGift.Guid == rewardGift.Guid
                );

            if ( giftClaim == null )
                throw new HttpNoContentException(
                    ControllerStrings.Warning702_GiftOutOfStock
                );

            // --- Asociar al Usuario con el Reclamo (efectivamente reclamando el regalo del usuario) ---
            giftClaim.RedeemedByUser
                = CurrentUser;

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
                        CultureInfo.InvariantCulture,
                        "{0}.{1}",
                        giftClaim.Guid.ToBase64String(),
                        giftClaim.PictureExtension
                    )
            };
        }
    }
}
