using KilometrosDatabase.Abstraction.Interfaces;
using KilometrosDatabase.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.Abstraction.Functional {
    /// <summary>
    ///     Repositorio de Recompensas.
    /// </summary>
    public sealed class RewardRepository : IRepository<Reward> {
        public RewardRepository(MainModelContainer dbContext) : base(dbContext) { }

        public IEnumerable<Reward> GetAllForRegion(
            string regionCode,
            Expression<Func<Reward, bool>> filter = null,
            Func<IQueryable<Reward>, IOrderedQueryable<Reward>> orderBy = null,
            Func<IOrderedQueryable<Reward>, IQueryable<Reward>> extra = null,
            string[] include = null
        ) {
            // > No ponernos elegantes si se solicitaron Entidades de todas las Regiones
            if ( string.IsNullOrEmpty(regionCode) || regionCode.Length < 2 )
                throw new ArgumentException(
                    "Region Code filter cannot be a single character or empty.",
                    "regionCode"
                );

            // > Preparar Region Code
            string[] regionCodeParts
                = new string[3] { null, null, null};
            regionCode.ToLowerInvariant().Split(
                new char[] { '-' }, 3
            ).CopyTo(regionCodeParts, 0);

            // TODO: Sustituir validación con RegEx
            if ( regionCodeParts[1] == null && regionCodeParts[2] != null )
                throw new ArgumentException(
                    "Region Code filter contains an invalid format. Expected a Subdivision Code, but none was found and a Particular Region was found.",
                    "regionCode"
                );

            // > Determinar la condicional a utilizar para obtener sólo resultados
            //   que aplican a la Región especificada
            Func<RewardRegionalization, bool> regionFilter;

            if ( regionCodeParts[1] == null ) { // - Si se tiene {país}
                regionFilter = f => !(
                    (
                        f.RegionCode == regionCodeParts[0]
                        || f.RegionCode == regionCodeParts[0] + "-*"
                    ) && f.Exclude == true
                );
            } else if ( regionCodeParts[2] == null ) { // - Si se tiene {país-subdivisión}
                regionFilter = f => !(
                    (
                        f.RegionCode == regionCodeParts[0]
                        || f.RegionCode == regionCodeParts[0] + "-*"
                        || f.RegionCode == regionCodeParts[0] + "-" + regionCodeParts[1]
                    ) && f.Exclude == true
                );
            } else { // - Si se tiene {país-subdivisión-particular}
                regionFilter = f => !(
                    (
                        f.RegionCode == regionCodeParts[0]
                        || f.RegionCode == regionCodeParts[0] + "-*"
                        || f.RegionCode == regionCodeParts[0] + "-" + regionCodeParts[1]
                        || f.RegionCode == regionCodeParts[0] + "-" + regionCodeParts[1] + "-*"
                        || f.RegionCode == regionCodeParts[0] + "-" + regionCodeParts[1] + "-" + regionCodeParts[1]
                    ) && f.Exclude == true
                );
            }

            // > Devolver respuesta, aplicando el Filtro de Región correspondiente
            return base.GetAll(
                (
                    filter ?? PredicateBuilder.True<Reward>()
                ).And( r =>
                    r.RewardRegionalization.Any(regionFilter)
                ),
                orderBy,
                extra,
                include
            );
        }
    }
}
