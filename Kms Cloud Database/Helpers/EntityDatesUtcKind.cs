using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kms.Cloud.Database.Helpers {
    internal static class EntityDatesUtcKind {
        public static TEntity ConvertDatesKindToUtc<TEntity>(TEntity entity) {
            if ( entity == null )
                return entity;

            IEnumerable<PropertyInfo> dateProperties =
                from thisProperty in typeof(TEntity).GetProperties()
                where thisProperty.GetType() == typeof(DateTime)
                select thisProperty;

            foreach ( PropertyInfo property in dateProperties ) {
                DateTime currentValue =
                    (DateTime)property.GetValue(entity);
                
                property.SetValue(
                    entity,
                    DateTime.SpecifyKind(currentValue, DateTimeKind.Utc)
                );
            }

            return entity;
        }
    }
}
