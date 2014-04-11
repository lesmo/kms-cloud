using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.EntityLocalization {
    public abstract class IEntityGlobalization<T> where T : IGlobalization {
        private Dictionary<CultureInfo, T> _globalization
            = new Dictionary<CultureInfo,T>();

        public T GetGlobalization(CultureInfo culture = null) {
            // > Determinar si no se tiene ya en memoria la Globalización de ésta Entidad
            if ( culture == null )
                culture = CultureInfo.CurrentCulture;

            if ( this._globalization.ContainsKey(culture) )
                return this._globalization[culture];

            // > Obtener propiedad que apunta a entdidad IGlobalization
            string currentCultureCode
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
                    where g.CultureCode == currentCultureCode
                        || g.CultureCode.StartsWith(
                            culture.TwoLetterISOLanguageName
                        )
                    select g
                ).FirstOrDefault();

            // > Agregar Globalización a memoria y devolverla
            this._globalization.Add(
                culture,
                globalization == null ? null : (T)globalization
            );
            
            return this._globalization[culture];
        }
    }
}
