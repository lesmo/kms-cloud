using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;
using KilometrosDatabase.Helpers;
using System.Globalization;

namespace KilometrosDatabase.Abstraction.Interfaces {
    public abstract class IRepository<TEntity> where TEntity : class {
        /// <summary>
        /// Contexto actual de la Base de Datos.
        /// </summary>
        protected DbSet _dbSet;
        protected MainModelContainer _context;

        private Type _type = typeof(TEntity);

        /// <summary>
        /// Devuelve la Entidad que representa los valores y textos en el
        /// idioma y cultura especificados.
        /// </summary>
        /// <param name="entity">Entidad de la cuál se desprenderá la Globalización</param>
        /// <param name="culture">Cultura e Idioma que se intentará obtener</param>
        /// <returns>Entidad de Globalización o {null} si no se encontró ninguna</returns>
        public IGlobalization GetGlobalization(
            TEntity entity,
            CultureInfo culture = null
        ) {
            if ( culture == null )
                culture = CultureInfo.CurrentCulture;

            string currentCultureCode
                = culture.Name.ToLowerInvariant();

            PropertyInfo globalizationProperty = (
                from thisProperty in this._type.GetProperties()
                where thisProperty.GetType() == typeof(ICollection<IGlobalization>)
                select thisProperty
            ).FirstOrDefault();

            if ( globalizationProperty == null )
                throw new ArgumentException("Entity does not support globalization");

            ICollection<IGlobalization> entityGlobalizationCollection
                = globalizationProperty.GetValue(entity) as ICollection<IGlobalization>;
                
            return (
                from IGlobalization eg in entityGlobalizationCollection
                where eg.CultureCode == currentCultureCode
                    || eg.CultureCode.StartsWith(
                        CultureInfo.CurrentCulture.TwoLetterISOLanguageName
                    )
                select eg
            ).FirstOrDefault();
        }

        /// <summary>
        /// Inicializar el Contexto de Base de Datos utilizado por éste Repositorio.
        /// </summary>
        /// <param name="dbContext">Contenedor de las Clases (Contexto de Base de Datos)</param>
        public IRepository(MainModelContainer dbContext = null) {
            if ( dbContext == null )
                return;

            this._context = dbContext;
            this._dbSet = dbContext.Set<TEntity>();
        }

        /// <summary>
        /// Devuelve todos las Entidades almacenadas en la BD, opcionalmente filtrándolas.
        /// </summary>
        /// <returns>Enumeración de los objetos almacenados en la BD.</returns>
        public virtual IEnumerable<TEntity> GetAll(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string[] include = null
        ) {
            IQueryable<TEntity> query
                = (IQueryable<TEntity>)this._dbSet.AsQueryable();

            if ( filter != null)
                query = query.Where(filter);

            if ( include != null && include.Length > 0 ) {
                foreach ( string includeItem in include )
                    query
                        = query.Include(includeItem);
            }

            List<TEntity>returnValue
                = orderBy != null
                ? orderBy(query).ToList()
                : query.ToList();

            for ( int i = 0; i < returnValue.Count; i++ ) {
                returnValue[i]
                    = EntityDatesUtcKind.ConvertDatesKindToUtc<TEntity>(
                        returnValue[i]
                    );
            }

            return (IEnumerable<TEntity>)returnValue;    
        }

        /// <summary>
        /// Devuelve una sola Entidad, opcionalmente filtrándo del universo.
        /// </summary>
        /// <returns>Objetos almacenado en la BD.</returns>
        public virtual TEntity GetFirst(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string[] include = null
        ) {
            IQueryable<TEntity> query
                = (IQueryable<TEntity>)this._dbSet.AsQueryable();

            if ( filter != null )
                query = query.Where(filter);

            if ( include != null && include.Length > 0 ) {
                foreach ( string includeItem in include )
                    query
                        = query.Include(includeItem);
            }

            TEntity returnValue
                = (
                    orderBy != null
                    ? orderBy(query).Take(1)
                    : query.Take(1)
                ).FirstOrDefault();

            returnValue
                = EntityDatesUtcKind.ConvertDatesKindToUtc<TEntity>(
                    returnValue
                );

            return returnValue;
        }

        /// <summary>
        /// Devuelve la Entidad asignada a éste ID.
        /// </summary>
        /// <param name="id">ID de la Entidad</param>
        /// <returns>Entidad</returns>
        public virtual TEntity Get(Int64 id) {
            return EntityDatesUtcKind.ConvertDatesKindToUtc<TEntity>(
                (TEntity)this._dbSet.Find(id)
            );
        }
        
        /// <summary>
        /// Devuelve la Entidad asignada a éste GUID.
        /// </summary>
        /// <param name="guid">GUID de la Entidad</param>
        /// <returns>Entidad</returns>
        public virtual TEntity Get(Guid guid) {
            return EntityDatesUtcKind.ConvertDatesKindToUtc<TEntity>(
                (TEntity)this._dbSet.Find(guid)
            );
        }

        /// <summary>
        /// Añade una nueva Entidad a la BD.
        /// </summary>
        /// <param name="entity">Entidad a añadir</param>
        public virtual void Add(TEntity entity) {
            // Obtener sólo las propiedades configuradas a establecerse con fecha y hora actuales
            IEnumerable<PropertyInfo> setDateProperties =
                from thisProperty in this._type.GetProperties()
                where
                    RepositoryConfig.DateTimeEntityPropertiesConfig.AutosetOnInsert.Contains(thisProperty.Name)
                select thisProperty;

            // Establecer el valor de las propiedades
            Type dateTimeType = typeof(DateTime);

            foreach ( PropertyInfo property in setDateProperties ) {
                dynamic value = property.GetValue(entity);

                if ( typeof(DateTime) == value.GetType() )
                    property.SetValue(entity, DateTime.UtcNow);
            }
                
            this._dbSet.Add(entity);
        }

        /// <summary>
        /// Elimina la Entidad con el ID especificado.
        /// </summary>
        /// <param name="id">ID de la Entidad a eliminar</param>
        public virtual void Delete(Int64 id) {
            this.Delete(
                this.Get(id)
            );
        }

        /// <summary>
        /// Elimina la Entidad con el GUID especificado.
        /// </summary>
        /// <param name="guid">GUID de la Entidad</param>
        public virtual void Delete(Guid guid) {
            this.Delete(
                this.Get(guid)
            );
        }

        /// <summary>
        /// Elimina la Entidad especificada de la BD.
        /// </summary>
        /// <param name="entity">Entidad a eliminar</param>
        public virtual void Delete(TEntity entity) {
            if ( this._context.Entry(entity).State == EntityState.Detached )
                this._dbSet.Attach(entity);

            this._dbSet.Remove(entity);
        }

        /// <summary>
        /// Almacena la información modificada de la Entidad en la BD.
        /// </summary>
        /// <param name="entity">Entidad con la información modificada</param>
        public virtual void Update(TEntity entity) {
            // Obtener sólo las propiedades configuradas a establecerse con fecha y hora actuales
            IEnumerable<PropertyInfo> setDateProperties =
                from thisProperty in this._type.GetProperties()
                where
                    RepositoryConfig.DateTimeEntityPropertiesConfig.AutosetOnUpdate.Contains(thisProperty.Name)
                select thisProperty;

            // Establecer el valor de las propiedades
            foreach ( PropertyInfo property in setDateProperties ) {
                dynamic value = property.GetValue(entity);

                if ( value.GetType() == typeof(DateTime) )
                    property.SetValue(entity, DateTime.UtcNow);
            }

            this._dbSet.Attach(entity);
            this._context.Entry(entity).State
                = EntityState.Modified;
        }
    }
}