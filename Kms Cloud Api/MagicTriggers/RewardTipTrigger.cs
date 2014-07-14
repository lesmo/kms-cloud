using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.MagicTriggers {
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public sealed class RewardTipTrigger : IDisposable {
        private Kms.Cloud.Database.Abstraction.WorkUnit Database;
        private User CurrentUser;
        private Int64 CurrentUserTotalDistance;

        public void Dispose() {
            Database.Dispose();
        }

        public RewardTipTrigger(User currentUser, Kms.Cloud.Database.Abstraction.WorkUnit database = null) {
            // --- Establecer objeto de conexión a BD ---
            Database = database ?? new Kms.Cloud.Database.Abstraction.WorkUnit();

            // --- Obtener objeto de Usuario ---
            CurrentUser = Database.UserStore[currentUser.Guid];

            if ( CurrentUser == null)
                return;

            CurrentUserTotalDistance = CurrentUser.UserDataTotalDistanceSum.TotalDistance;

            try {
                TriggerRewardsByDistance();
            } catch ( Exception ex ) {
                //TODO: Hacer log de error
            }
        }

        private void TriggerRewardsByDistance() {
            // > Obtener las Recompensas que ahora sean desbloqueables por el Usuario
            var rewards  = Database.RewardStore.GetAllForRegion(
                regionCode:
                    // + Obtener recompensas de la Región del Usuario
                    CurrentUser.RegionCode,
                filter: f =>
                    // + Obtener recomepnsas donde la Distancia de Debloqueo sea menor a la
                    //   Distancia Total Recorrida por el Usuario, y la recompensa no se haya
                    //   desbloqueado anteriormente por el Usuario.
                    f.DistanceTrigger <= CurrentUserTotalDistance
                    && ! f.UserEarnedReward.Any(r =>
                        r.User.Guid == CurrentUser.Guid
                    ),
                orderBy: o =>
                    // + Ordenar recompensas de Mayor a menor Distancia de Debloqueo
                    o.OrderByDescending(b => b.DistanceTrigger),
                extra: x =>
                    // + Obtener un máximo de recompensas
                    x.Take(40)
            );

            // > Iterar cada Recompensa y desbloquearla
            foreach ( Reward reward in rewards.Reverse() ) {
                Database.UserEarnedRewardStore.AddAndSave(new UserEarnedReward {
                    Reward = reward,
                    User   = CurrentUser
                });

                // TODO: Lanzar notificaciones, mailing y cualquier otra vía de notificación
            }
        }

        private void TriggerTipsByDays() {
            var userDaysRegistered = (DateTime.UtcNow - CurrentUser.CreationDate).TotalDays;

            // > Obtener los Tips que ahora sean liberables por el Usuario
            var tips  = Database.TipStore.GetAll(
                filter: f =>
                    // + Obtener Tips que tengan la condicional de Distancia
                    f.DaysTrigger == userDaysRegistered
                    // + ... que correspnda a su Nivel de Actividad
                    && f.MotionLevel.Any(t =>
                        t.Guid == CurrentUser.CurrentMotionLevel.Guid
                    )
                    // + ... y que no se haya liberado anteriormente por el Usuario
                    && ! f.UserTipHistory.Any(t =>
                        t.User.Guid == CurrentUser.Guid
                    ),
                orderBy: o =>
                    o.OrderBy(b => b.DistanceTrigger)
            );

            // > Iterar cada Tip y liberarlo
            foreach ( Tip tip in tips ) {
                Database.UserTipHistoryStore.AddAndSave(new UserTipHistory {
                    Tip = tip,
                    User = CurrentUser
                });

                // TODO: Lanzar notificaciones, mailing y cualquier otra vía de notificación
            }
        }
    }
}