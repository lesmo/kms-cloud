using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.EntityLocalization {
    /// <summary>
    ///     Permite acceder de forma rápida y fácil a la Entidad que almacena texto y otros
    ///     recursos específicos de cada idioma.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de la Entidad que almacena el texto y otros recursos.
    /// </typeparam>
    public abstract class IEntityGlobalization<T> where T : IGlobalization {
        private Dictionary<int, T> _globalization
            = new Dictionary<int, T>();

        public virtual T GetGlobalization(CultureInfo culture = null) {
            // > Determinar si no se tiene ya en memoria la Globalización de ésta Entidad
            if ( culture == null )
                culture = CultureInfo.CurrentCulture;

            int hashCode
                = culture.GetHashCode();

            if ( this._globalization.ContainsKey(hashCode) )
                return this._globalization[hashCode];

            // > Obtener propiedad que apunta a entidad IGlobalization
            string cultureCode
                = culture.Name.ToLowerInvariant();

            PropertyInfo globalizationProperty = (
                from thisProperty in this.GetType().GetProperties()
                where thisProperty.GetType() == typeof(ICollection<IGlobalization>)
                select thisProperty
            ).FirstOrDefault();

            if ( globalizationProperty == null )
                throw new ArgumentException("Entity does not support globalization");

            IQueryable<IGlobalization> entityGlobalizationCollection
                = globalizationProperty.GetValue(this) as IQueryable<IGlobalization>;

            // > Obtener Globalización de la BD
            IGlobalization globalization
                = (
                    from g in entityGlobalizationCollection
                    where
                        g.CultureCode == cultureCode
                        || g.CultureCode.StartsWith(
                            culture.TwoLetterISOLanguageName
                        )
                    select g
                ).FirstOrDefault();

            // > Agregar Globalización a memoria y devolverla
            this._globalization.Add(
                hashCode,
                globalization == null ? null : (T)globalization
            );

            return (T)globalization;
        }
    }
}
