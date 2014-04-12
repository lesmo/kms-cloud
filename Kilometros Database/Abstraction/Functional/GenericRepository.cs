using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.Abstraction {
    using System.Data.Entity;
    using KilometrosDatabase.Abstraction.Interfaces;

    /// <summary>
    /// Repositorio Genérico.
    /// </summary>
    /// <typeparam name="TEntity">Tipo de la Entidad representada por éste Repositorio.</typeparam>
    public sealed class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class {
        /// <summary>
        /// Repositorio Genérico.
        /// </summary>
        /// <param name="dbContext">Contexto de BD, normalmente {MainModelContainer}</param>
        public GenericRepository(MainModelContainer dbContext) : base(dbContext) { }
    }
}
