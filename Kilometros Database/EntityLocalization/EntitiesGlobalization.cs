using KilometrosDatabase.EntityLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace KilometrosDatabase {
    public partial class Tip : IEntityGlobalization<TipGlobalization> { }
    public partial class IPicture : IEntityGlobalization<IGlobalization> {
        /// <summary>
        ///     #HACK!# Permite acceder de forma rápida y fácil a la Entidad que almacena texto y otros
        ///     recursos específicos de cada idioma. Hack para darle la vuelta a la limitante de polimorfismo
        ///     de Clases Parciales, por falta de imaginación.
        /// </summary>
        /// <typeparam name="T">
        ///     Tipo de la Entidad que almacena el texto y otros recursos.
        /// </typeparam>
        /// <param name="culture"></param>
        /// <returns></returns>
        public T GetGlobalization<T>(CultureInfo culture = null) where T : IGlobalization {
            if ( this is TipCategory )
                return (T)base.GetGlobalization(culture);
            else
                throw new InvalidOperationException(
                    "This particular instance of IPicture does not support Globalization."
                );
        }
    }

    public partial class Reward : IEntityGlobalization<RewardGlobalization> { }
    public partial class RewardGift : IEntityGlobalization<RewardGiftGlobalization> { }
    public partial class MotionLevel : IEntityGlobalization<MotionLevelGlobalization> { }
}
