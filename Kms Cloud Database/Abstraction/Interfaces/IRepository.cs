using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;
using Kms.Cloud.Database.Helpers;
using System.Globalization;

namespace Kms.Cloud.Database.Abstraction.Interfaces {
    public abstract class IRepository<TEntity> where TEntity : class {
        /// <summary>
        /// Contexto actual de la Base de Datos.
        /// </summary>
        protected DbSet _dbSet;
        protected MainModelContainer _context;
        
        private IEnumerable<PropertyInfo> _autosetInsertDateProperties;
        private IEnumerable<PropertyInfo> _autosetUpdateDateProperties;
        private PropertyInfo _autosetGuidProperty;
        private bool _autosetInitialized = false;

        private void InitializePropertyInfo() {
            if ( this._autosetInitialized )
                return;

            var entityProperties = typeof(TEntity).GetProperties();

            this._autosetInsertDateProperties =
                entityProperties
                    .Where(w => RepositoryConfig.DateTimeEntityPropertiesConfig.AutosetOnInsert.Contains(w.Name));
            this._autosetUpdateDateProperties = 
                entityProperties
                    .Where(w => RepositoryConfig.DateTimeEntityPropertiesConfig.AutosetOnUpdate.Contains(w.Name));
            this._autosetGuidProperty =
                entityProperties
                    .Where(w => RepositoryConfig.GuidPropertiesConfig.AutosetOnInsert.Contains(w.Name))
                    .FirstOrDefault();

            this._autosetInitialized
                = true;
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
        /// <param name="filter">Función aplicada para filtrar los registros.</param>
        /// <param name="orderBy">Función aplicada para ordenar los registros.</param>
        /// <param name="extra">Función aplicada después de filtrar y ordenar los registros.</param>
        /// <param name="include">Arreglo con Entidades relacionadas que deberían cargarse en memoria junto con el resultado.</param>
        /// <returns>Enumeración de los objetos almacenados en la BD.</returns>
        public virtual IEnumerable<TEntity> GetAll(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extra = null,
            string[] include = null
        ) {
            var query
                = (IQueryable<TEntity>)this._dbSet.AsQueryable();
            query
                = query.AsExpandable();

            if ( filter != null)
                query = query.Where(filter);

            if ( orderBy == null ) {
                query
                    = extra == null
                    ? query
                    : extra(query);
            } else {
                query
                    = extra == null
                    ? orderBy(query)
                    : extra(orderBy(query));
            }

            if ( include != null && include.Length > 0 ) {
                foreach ( string includeItem in include )
                    query
                        = query.Include(includeItem);
            }

            List<TEntity>returnValue
                = query.ToList();

            for ( int i = 0; i < returnValue.Count; i++ ) {
                returnValue[i]
                    = EntityDatesUtcKind.ConvertDatesKindToUtc<TEntity>(
                        returnValue[i]
                    );
            }

            return (IEnumerable<TEntity>)returnValue;    
        }

        /// <summary>
        /// Devuelve una sola Entidad, opcionalmente filtrándo el universo.
        /// </summary>
        /// <returns>Objetos almacenado en la BD.</returns>
        public virtual TEntity GetFirst(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extra = null,
            string[] include = null
        ) {
            return this.GetAll(
                filter,
                orderBy,
                x => extra == null ? x.Take(1) : extra(x).Take(1),
                include
            ).FirstOrDefault();
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
        ///     Devuelve la Entidad asignada éste GUID , en representación de cadena compacta, o ID.
        /// </summary>
        /// <param name="guidString">
        ///     Cadena de Representación Compacta del GUID, o ID.</param>
        /// <returns>Entidad</returns>
        public virtual TEntity Get(string guidString) {
            var guid = new Guid().FromBase64String(guidString);

            if ( guid != default(Guid) )
                return this.Get(guid);

            if ( Guid.TryParse(guidString, out guid) )
                return this.Get(guid);

            Int64 idLong;
            if ( Int64.TryParse(guidString, out idLong) )
                return this.Get(idLong);

            return null;
        }

        /// <summary>
        /// Añade una nueva Entidad a la BD.
        /// </summary>
        /// <param name="entity">Entidad a añadir</param>
        public virtual void Add(TEntity entity) {
            this.InitializePropertyInfo();

            if ( this._autosetGuidProperty != null ) {
                this._autosetGuidProperty.SetValue(entity, Guid.NewGuid());
            }

            this._autosetInsertDateProperties.ForEach((p) => {
                if ( p.PropertyType == typeof(DateTime) ) {
                    if ( (DateTime)p.GetValue(entity) == DateTime.MinValue )
                        p.SetValue(entity, DateTime.UtcNow);
                } else {
                    if ( ! ((DateTime?)p.GetValue(entity)).HasValue )
                        p.SetValue(entity, (DateTime?)DateTime.UtcNow);
                }
            });
            
            this._dbSet.Add(entity);
        }

        public virtual void AddAndSave(TEntity entity) {
            this.Add(entity);
            this._context.SaveChanges();
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
            this.InitializePropertyInfo();

            this._autosetUpdateDateProperties.ForEach((p) => {
                if ( p.PropertyType == typeof(DateTime) ) {
                    if ( (DateTime)p.GetValue(entity) == DateTime.MinValue )
                        p.SetValue(entity, DateTime.UtcNow);
                } else if ( p.PropertyType == typeof(DateTime?) ) {
                    if ( !((DateTime?)p.GetValue(entity)).HasValue )
                        p.SetValue(entity, (DateTime?)DateTime.UtcNow);
                }
            });

            this._dbSet.Attach(entity);
            this._context.Entry(entity).State = EntityState.Modified;
        }

        public TEntity this[Guid guid] {
            get {
                return this.Get(guid);
            }
        }

        public TEntity this[string guid] {
            get {
                return this.Get(guid);
            }
        }
    }
}