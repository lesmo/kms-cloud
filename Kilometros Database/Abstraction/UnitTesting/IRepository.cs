using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.Abstraction.UnitTesting {
    public abstract class IRepository<TEntity> : Interfaces.IRepository<TEntity> where TEntity : class {
        private List<TEntity> _objects = new List<TEntity>();

        public override void Add(TEntity entity) {
            this._objects.Add(entity);
        }

        public override void Delete(TEntity entity) {
            this._objects.Remove(entity);
        }

        public override void Delete(Int64 id) {
            this._objects.RemoveAt((int)id);
        }

        public override void Delete(Guid guid) {
            
        }
    }
}
