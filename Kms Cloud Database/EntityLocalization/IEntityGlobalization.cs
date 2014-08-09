using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kms.Cloud.Database.EntityLocalization {
    /// <summary>
    ///     Permite acceder de forma rápida y fácil a la Entidad que almacena texto y otros
    ///     recursos específicos de cada idioma.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de la Entidad que almacena el texto y otros recursos.
    /// </typeparam>
    public abstract class IEntityGlobalization<T> where T : IGlobalization, new() {
        private Dictionary<int, object> _globalization
            = new Dictionary<int, object>();

        internal virtual GT GetGlobalization<GT>(CultureInfo culture) where GT : IGlobalization, new() {
            // > Determinar si no se tiene ya en memoria la Globalización de ésta Entidad
            if ( culture == null )
                culture = CultureInfo.CurrentUICulture;

            int hashCode
                = culture.LCID;

            if ( this._globalization.ContainsKey(hashCode) )
                return (GT)this._globalization[hashCode];

            if ( this._globalization.ContainsKey(hashCode) )
                return (GT)this._globalization[hashCode];

            // > Obtener propiedad que apunta a entidad IGlobalization
            string cultureCode
                = culture.Name.ToLower();

            PropertyInfo globalizationProperty = (
                from PropertyInfo p in this.GetType().GetProperties()
                where p.PropertyType == typeof(ICollection<GT>)
                select p
            ).FirstOrDefault();

            if ( globalizationProperty == null )
                throw new ArgumentException("Entity does not support globalization");

            IQueryable<GT> entityGlobalizationCollection
                = (globalizationProperty.GetValue(this) as ICollection<GT>).AsQueryable();

            // > Obtener Globalización de la BD
            GT globalization
                = (
                    from g in entityGlobalizationCollection
                    where
                        g.CultureCode == cultureCode
                        || g.CultureCode.StartsWith(
                            culture.TwoLetterISOLanguageName
                        )
                    select g
                ).FirstOrDefault();

            if ( globalization == null )
                globalization
                    = new GT();

            // > Agregar Globalización a memoria y devolverla
            this._globalization.Add(
                hashCode,
                globalization == null ? null : globalization
            );

            return globalization;
        }

        public virtual T GetGlobalization(CultureInfo culture = null) {
            return this.GetGlobalization<T>(culture);
        }
    }
}
