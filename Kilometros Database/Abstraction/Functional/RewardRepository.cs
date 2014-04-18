using KilometrosDatabase.Abstraction.Interfaces;
using KilometrosDatabase.Helpers;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KilometrosDatabase.Abstraction.Functional {
    /// <summary>
    ///     Repositorio de Recompensas.
    /// </summary>
    public sealed class RewardRepository : IRepository<Reward> {
        /// <summary>
        ///     Crea la abstracción de acceso como Repositorio a Recompensas.
        /// </summary>
        /// <param name="dbContext">
        ///     Contexto de BD, normalmente {MainModelContainer}.
        /// </param>
        public RewardRepository(MainModelContainer dbContext) : base(dbContext) { }

        /// <summary>
        ///     Devuelve las Recompensas del Código de Región especificado, aplicándole los
        ///     filtros, ordenamientos y transformaciones extras aplicables.
        /// </summary>
        /// <param name="regionCode">
        ///     Código de Región del que se busca obtener las Recompensas.
        /// </param>
        /// <param name="filter">
        ///     Función aplicada para filtrar los registros.
        /// </param>
        /// <param name="orderBy">
        ///     Función aplicada para ordenar los registros.
        /// </param>
        /// <param name="extra">
        ///     Función aplicada después de filtrar y ordenar los registros.
        /// </param>
        /// <param name="include">
        ///     Arreglo con Entidades relacionadas que deberían cargarse en memoria junto con el resultado.
        /// </param>
        /// <returns>
        ///     Enumeración de los objetos almacenados en la BD.
        /// </returns>
        public IEnumerable<Reward> GetAllForRegion(
            string regionCode,
            Expression<Func<Reward, bool>> filter = null,
            Func<IQueryable<Reward>, IOrderedQueryable<Reward>> orderBy = null,
            Func<IQueryable<Reward>, IQueryable<Reward>> extra = null,
            string[] include = null
        ) {
            // > No ponernos elegantes si se solicitaron Entidades de todas las Regiones
            if ( string.IsNullOrEmpty(regionCode) || regionCode.Length < 2 )
                throw new ArgumentException(
                    "Region Code filter cannot be a single character or empty.",
                    "regionCode"
                );

            // > Preparar Region Code
            regionCode
                = regionCode.ToLowerInvariant().Trim();

            bool validRegionCode
                =  new Regex(
                    @"^([a-z]{2})(\-[a-z]{3}(\-[a-z]*)?)?$"
                ).IsMatch(regionCode);

            if ( ! validRegionCode )
                throw new ArgumentException(
                    "Region Code filter contains an invalid format.",
                    "regionCode"
                );

            string[] regionCodeParts
                = new string[3] { null, null, null};
            regionCode.ToLowerInvariant().Split(
                new char[] { '-' }, 3
            ).CopyTo(regionCodeParts, 0);

            // > Determinar la condicional a utilizar para obtener sólo resultados
            //   que aplican a la Región especificada
            Expression<Func<RewardRegionalization, bool>> regionFilter;
            string regionCodePart1, regionCodePart2, regionCodePart3;

            regionCodePart1
                = regionCodeParts[0];
            regionCodePart2
                = regionCodeParts[1];
            regionCodePart3
                = regionCodeParts[2];

            if ( regionCodePart2 == null ) { // - Si se tiene {país}
                regionFilter = f => !(
                    (
                        f.RegionCode == regionCodePart1
                        || f.RegionCode == regionCodePart1 + "-*"
                    ) && f.Exclude == true
                );
            } else if ( regionCodePart3 == null ) { // - Si se tiene {país-subdivisión}
                regionFilter = f => !(
                    (
                        f.RegionCode == regionCodePart1
                        || f.RegionCode == regionCodePart1 + "-*"
                        || f.RegionCode == regionCodePart1 + "-" + regionCodePart2
                    ) && f.Exclude == true
                );
            } else { // - Si se tiene {país-subdivisión-particular}
                regionFilter = f => !(
                    (
                        f.RegionCode == regionCodePart1
                        || f.RegionCode == regionCodePart1 + "-*"
                        || f.RegionCode == regionCodePart1 + "-" + regionCodePart2
                        || f.RegionCode == regionCodePart1 + "-" + regionCodePart2 + "-*"
                        || f.RegionCode == regionCodePart1 + "-" + regionCodePart2 + "-" + regionCodePart2
                    ) && f.Exclude == true
                );
            }

            // > Devolver respuesta, aplicando el Filtro de Región correspondiente
            return base.GetAll(
                (
                    filter ?? PredicateBuilder.True<Reward>()
                ).And(f =>
                    f.RewardRegionalization.Any(regionFilter.Compile())
                ).Expand(),
                orderBy,
                extra,
                include
            );
        }

        /// <summary>
        ///     Devuelve una sola Recompensa del Código de Región especificado, aplicándole los
        ///     filtros, ordenamientos y transformaciones extras aplicables.
        /// </summary>
        /// <returns>
        ///     Objetos almacenado en la BD.
        /// </returns>
        public Reward GetFirstForRegion(
            string regionCode,
            Expression<Func<Reward, bool>> filter = null,
            Func<IQueryable<Reward>, IOrderedQueryable<Reward>> orderBy = null,
            Func<IQueryable<Reward>, IQueryable<Reward>> extra = null,
            string[] include = null
        ) {
            return this.GetAllForRegion(
                regionCode,
                filter,
                orderBy,
                x => extra == null ? x.Take(1) : extra(x).Take(1),
                include
            ).FirstOrDefault();
        }
    }
}
