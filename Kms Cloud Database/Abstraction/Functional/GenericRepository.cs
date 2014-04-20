using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Kms.Cloud.Database.Abstraction.Interfaces;

// > NOTA: De momento, GenericRepository funciona más como Proxy a la Clase Genérica Abstracta {IRepository}
//         que como cualquier otra cosa. Habría que considerar la posibilidad de re-estructurar la Librería
//         forma tal que {IRepository} se vuelva {GenericRepository} y retirar los Espacios de Nombres
//         {Interfaces} y {UnitTesting}.

namespace Kms.Cloud.Database.Abstraction {
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
