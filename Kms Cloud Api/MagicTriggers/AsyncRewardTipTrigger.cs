using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.MagicTriggers {
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public sealed class AsyncRewardTipTrigger : IDisposable {
        private Kms.Cloud.Database.Abstraction.WorkUnit Database
            = new Kms.Cloud.Database.Abstraction.WorkUnit();
        private User CurrentUser;

        public void Dispose() {
            Database.Dispose();
        }

        public AsyncRewardTipTrigger(User currentUser) {
            // --- Obtener objeto de Usuario ---
            CurrentUser = Database.UserStore[currentUser.Guid];

            if ( CurrentUser == null)
                return;

            #if DEBUG
                TriggerTipsByDistance();
                TriggerRewardsByDistance();
                return;
            #endif

            // > Éste método se llamara por un Thread diferente de la ejecución
            //   normal, y si ocurre una excepción sin manejar durante estos procesos
            //   el Thread Pool se matará. Por ello se encierra en este raro {try catch}
            //   después del {return} anterior.
            try {
                TriggerTipsByDistance();
                TriggerRewardsByDistance();
            } catch {
                return;
            }
        }

        private void TriggerRewardsByDistance() {
            // > Obtener las Recompensas que ahora sean desbloqueables por el Usuario
            IEnumerable<Reward> rewards
                = Database.RewardStore.GetAllForRegion(
                    regionCode:
                        // + Obtener recompensas de la Región del Usuario
                        CurrentUser.RegionCode,
                    filter: f =>
                        // + Obtener recomepnsas donde la Distancia de Debloqueo sea menor a la
                        //   Distancia Total Recorrida por el Usuario, y la recompensa no se haya
                        //   desbloqueado anteriormente por el Usuario.
                        f.DistanceTrigger < CurrentUser.UserDataTotalDistanceSum.TotalDistance
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
                UserEarnedReward unlockedReward
                    = new UserEarnedReward {
                        Reward
                            = reward,
                        User
                            = CurrentUser
                    };

                Database.UserEarnedRewardStore.AddAndSave(unlockedReward);

                // TODO: Lanzar notificaciones, mailing y cualquier otra vía de notificación
            }
        }

        private void TriggerTipsByDistance() {
            // > Obtener los Tips que ahora sean liberables por el Usuario
            IEnumerable<Tip> tips
                = Database.TipStore.GetAll(
                    filter: f =>
                        // + Obtener Tips que tengan la condicional de Distancia
                        f.DistanceTrigger.HasValue
                        // + ... que esa Distancia sea menor a la Distancia Total recorrida por el Usuario
                        && f.DistanceTrigger.Value < CurrentUser.UserDataTotalDistanceSum.TotalDistance
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
                UserTipHistory unlockedTip
                    = new UserTipHistory {
                        Tip
                            = tip,
                        User
                            = CurrentUser
                    };

                Database.UserTipHistoryStore.AddAndSave(unlockedTip);

                // TODO: Lanzar notificaciones, mailing y cualquier otra vía de notificación
            }
        }
    }
}