using Kilometros_WebApp.Models.Views;
using KilometrosDatabase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Kilometros_WebApp.Controllers {
    public abstract partial class BaseController {
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
                this._layoutValues = new LayoutValues() {
                    UserName
                        = CurrentUser.Name,
                    UserLastname
                        = CurrentUser.LastName,
                    UserPicture
                        = new Uri(CurrentUser.PictureUri),

                    LocationString
                        = "{Not Implemented}",

                    TotalDistanceCentimeters
                        = CurrentUser.UserDataTotalDistanceSum.TotalDistance,
                };

                // > Obtener la última recompensa obtenida por el Usuario
                UserEarnedReward lastReward
                    = Database.UserEarnedRewardStore.GetFirst(
                        filter: f =>
                            f.User.Guid == CurrentUser.Guid,
                        orderBy: o =>
                            o.OrderByDescending(b => b.CreationDate)
                    );

                if ( lastReward.Discarded ) {
                    // > Obtener distancia restante para la Próxima Recompensa del Usuario
                    Reward nextReward
                        = Database.RewardStore.GetFirstForRegion(
                            regionCode:
                        // + Obtener la Recompensa para el Código de Región del Usuario
                                CurrentUser.RegionCode,
                            filter: f =>
                                // + Obtener la Recompensa inmediata siguiente según la Distancia del Usuario
                                f.DistanceTrigger > CurrentUser.UserDataTotalDistanceSum.TotalDistance,
                            orderBy: o =>
                                // + Ordenar las Recompensas según su Distancia de Debloqueo (Descendiente)
                                o.OrderByDescending(b => b.DistanceTrigger)
                        );

                    if ( nextReward == null )
                        throw new InvalidOperationException(
                            "Current User's [" + CurrentUser.Guid.ToString("N") + "] Next Reward could"
                            + " not be mapped to any Reward because Database is empty, or Regional limitations"
                            + " return no Rewards. Never must a User be mapped to no next Reward."
                        );
                    else
                        this._layoutValues.NextRewardDistanceCentimeters
                            = nextReward.DistanceTrigger - CurrentUser.UserDataTotalDistanceSum.TotalDistance;
                } else {
                    this._layoutValues.RecentlyUnlockedReward
                        = new RewardModel() {
                            RewardId
                                = lastReward.Guid.ToBase64String()
                        };
                }

                // > Obtener el Tip del Día
                Tip tipOfTheDay
                    = Database.UserTipHistoryStore.GetFirst(
                        filter: f =>
                            f.User.Guid == CurrentUser.Guid,
                        orderBy: o =>
                            o.OrderByDescending(b => b.CreationDate),
                        include:
                            new string[] { "Tip.TipCategory" }
                    ).Tip;

                this._layoutValues.TipOfTheDay
                    = new TipModel() {
                        TipId
                            = tipOfTheDay.Guid.ToBase64String(),
                        Text
                            = tipOfTheDay.GetGlobalization().Text,
                        Category
                            = tipOfTheDay.TipCategory.GetGlobalization().Name,
                        IconUri
                            = GetDynamicResourceUri(
                                method:
                                    "Images",
                                filename:
                                    tipOfTheDay.TipCategory.Guid.ToBase64String(),
                                ext:
                                    tipOfTheDay.TipCategory.PictureExtension
                            )
                    };

                // > Devolver valores
                return this._layoutValues;
            }
        }
        private LayoutValues _layoutValues = null;
    }
}