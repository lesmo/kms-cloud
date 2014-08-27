using Kms.Cloud.Database.Helpers;
using Kms.Cloud.WebApp.Models.Views;
using Kilometros_WebGlobalization.Database;
using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Kms.Cloud.WebApp.Properties;

namespace Kms.Cloud.WebApp.Controllers {
    public abstract partial class BaseController {
        protected override void OnActionExecuted(System.Web.Mvc.ActionExecutedContext filterContext) {
            filterContext.Controller.ViewBag.Layout = this.LayoutValues;
            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        ///     Devuelve los Valores que deben utilizarse en el Layout
        ///     que se comparte por todos los Controladores y Vistas, principalmente
        ///     en la Barra Lateral.
        /// </summary>
        protected LayoutValues LayoutValues {
            get {
                if ( this._layoutValues != null )
                    return this._layoutValues;

                if ( CurrentUser == null )
                    return null;

                // > Inicializar objeto
                var userPicture = GetDynamicResourceUri(
                    CurrentUser.UserPicture
                    ?? Database.IPictureStore.Get(Settings.Default.KmsNoPictureFemale)
                );

                if ( CurrentUser.UserPicture == null && CurrentUser.UserBody.Sex == "m" )
                    userPicture = GetDynamicResourceUri(
                        Database.IPictureStore.Get(Settings.Default.KmsNoPicturMale)
                    );

                this._layoutValues = new LayoutValues {
                    UserName       = CurrentUser.Name,
                    UserLastname   = CurrentUser.LastName,
                    UserPicture    = userPicture,
                    LocationString = RegionInfo.CurrentRegion.NativeName,

                    TotalDistanceCentimeters = 
                        CurrentUser.UserDataTotalDistanceSum.TotalDistance,
                    AjaxCache =
                        CurrentUser.UserDataTotalDistanceSum.Timestamp.ToJavascriptEpoch(),
                    UserSignup =
                        CurrentUser.CreationDate.ToJavascriptEpoch()
                };

                // > Obtener la última recompensa obtenida por el Usuario
                var lastReward = Database.UserEarnedRewardStore.GetFirst(
                    filter : f => f.User.Guid == CurrentUser.Guid,
                    orderBy: o => o.OrderByDescending(b => b.CreationDate)
                );
                this._layoutValues.RecentlyUnlockedReward = new RewardModel(lastReward, this);
                
                // > Obtener distancia restante para la Próxima Recompensa del Usuario
                var nextReward = Database.RewardStore.GetFirstForRegion(
                    // + Obtener la Recompensa para el Código de Región del Usuario
                    regionCode: CurrentUser.RegionCode,

                    // + Obtener la Recompensa inmediata siguiente según la Distancia del Usuario
                    filter: f => f.DistanceTrigger > CurrentUser.UserDataTotalDistanceSum.TotalDistance,

                    // + Ordenar las Recompensas según su Distancia de Desbloqueo (Descendiente)
                    orderBy: o => o.OrderBy(b => b.DistanceTrigger)
                );

                if ( nextReward == null )
                    throw new InvalidOperationException(
                        "Current User's [" + CurrentUser.Guid.ToString("N") + "] Next Reward could"
                        + " not be mapped to any Reward because Database is empty, or Regional limitations"
                        + " return no Rewards. Never must a User be mapped to no next Reward."
                    );

                this._layoutValues.NextRewardDistanceCentimeters = 
                    (long)(nextReward.DistanceTrigger - CurrentUser.UserDataTotalDistanceSum.TotalDistance);
                
                // > Obtener el Tip del Día
                var tipOfTheDay = Database.UserTipHistoryStore.GetFirst(
                    filter : f => f.User.Guid == CurrentUser.Guid,
                    orderBy: o => o.OrderByDescending(b => b.CreationDate),
                    include: new string[] { "Tip.TipCategory" }
                ).Tip;

                this._layoutValues.TipOfTheDay = new TipModel {
                    TipId    = tipOfTheDay.Guid.ToBase64String(),
                    Text     = tipOfTheDay.GetGlobalization().Text,
                    Category = tipOfTheDay.TipCategory.GetGlobalization().Name,
                    IconUri  = GetDynamicResourceUri(
                        method  : "Images",
                        filename: tipOfTheDay.TipCategory.Guid.ToBase64String(),
                        ext     : tipOfTheDay.TipCategory.PictureExtension
                    )
                };

                // > Obtener Notificaciones
                var notifications = Database.NotificationStore.GetAll(
                    filter: f =>
                        f.User.Guid == CurrentUser.Guid,
                    orderBy: o =>
                        o.OrderByDescending(b => b.CreationDate),
                    extra: x =>
                        x.Take(7)
                );

                this._layoutValues.Notifications = (
                    from notification in notifications
                    where notification.NotificationType == NotificationType.FriendRequest
                    let user = Database.UserStore[notification.ObjectGuid]
                    where user != null
                    select new LayoutNotification {
                        IconUri = new Uri(user.PictureUri), Title = user.Name, Description = NotificationStrings.SentYouAFriendRequest, Discarded = notification.Discarded
                    }
                ).ToArray();

                // > Devolver valores
                return this._layoutValues;
            }
        }
        private LayoutValues _layoutValues = null;
    }
}