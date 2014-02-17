using Kilometros_WebAPI.Exceptions;
using Kilometros_WebAPI.Helpers;
using Kilometros_WebAPI.Models.HttpGet.My_Controllers;
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
    public class MyGiftsController : ApiController {
        KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();

        [HttpGet]
        [Route("my/gifts/{giftGuidBase64}")]
        public GiftResponse GetRewardGift(string giftGuidBase64) {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;

            /** Buscar Regalo solicitado **/
            Guid? giftGuid
                = MiscHelper.GuidFromBase64(giftGuidBase64);
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

            /** Verificar que el Regalo haya sido reclamado por el Usuario **/
            UserRewardGiftClaimed userRewardGiftClaim
                = Database.UserRewardGiftClaimedStore.GetFirst(
                    r => r.RewardGift == rewardGift && r.RedeemedByUser == user
                );
            if ( userRewardGiftClaim == null )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning701_GiftNotFound
                );

            /** Obtener Información del Regalo **/
            // Obtener fotografías del Regalo
            List<string> rewardGiftPictures
                = new List<string>();
            foreach ( RewardGiftPicture picture in rewardGift.RewardGiftPictures )
                rewardGiftPictures.Add(
                    picture.Guid.ToString() + "." + picture.PictureExtension
                );

            /** Obtener Regalo en el Idioma actual **/
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
                            MiscHelper.Base64FromGuid(userRewardGiftClaim.Guid),
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
                            MiscHelper.Base64FromGuid(userRewardGiftClaim.Guid),
                            userRewardGiftClaim.PictureExtension
                        ),

                    Pictures
                        = rewardGiftPictures.ToArray()
                };
            }
        }

        [HttpPost]
        [Route("my/gifts/{giftGuidBase64}")]
        public GiftClaimResponse ClaimRewardGift(string giftGuidBase64) {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;

            /** Buscar Regalo solicitado **/
            Guid? giftGuid
                = MiscHelper.GuidFromBase64(giftGuidBase64);
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

            /** Verificar que el Regalo haya sido obtenido por el Usuario **/
            UserEarnedReward userEarnedReward
                = Database.UserEarnedRewardStore.GetFirst(
                    r => r.User == user && r.Reward.RewardGift.Contains(rewardGift)
                );
            if ( userEarnedReward == null )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning701_GiftNotFound
                );

            /** Verificar que el Usuario tenga Información de Envío si el Regalo se envía **/
            if ( rewardGift.IsShipped ) {
                ShippingInformation shippingInformation
                    = user.ShippingInformation;

                if ( shippingInformation == null )
                    throw new HttpConflictException(
                        ControllerStrings.Warning205_ShippingInfoNotSet
                    );
            }

            /** Obtener un objeto de Reclamo **/
            UserRewardGiftClaimed giftClaim
                = Database.UserRewardGiftClaimedStore.GetFirst(
                    r => r.RedeemedByUser == null && r.RewardGift == rewardGift
                );
            if ( giftClaim == null )
                throw new HttpNoContentException(
                    ControllerStrings.Warning702_GiftOutOfStock
                );

            /** Asociar al Usuario con el Reclamo (efectivamente reclamando el regalo del usuario) **/
            giftClaim.RedeemedByUser = user;

            Database.UserRewardGiftClaimedStore.Update(giftClaim);
            Database.SaveChanges();

            /** Devolver detalles de canje **/
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
                        MiscHelper.Base64FromGuid(giftClaim.Guid),
                        giftClaim.PictureExtension
                    )
            };
        }
    }
}
