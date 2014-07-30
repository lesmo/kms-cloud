using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kms.Cloud.Database.Abstraction.Interfaces;

namespace Kms.Cloud.Database.Abstraction.Functional {
    public sealed class RegionRepository : IRepository<Region> {
        /// <summary>
        ///     Crea la abstracción de acceso como Repositorio a Regiones.
        /// </summary>
        /// <param name="dbContext">
        ///     Contexto de BD, normalmente {MainModelContainer}.
        /// </param>
        public RegionRepository(MainModelContainer dbContext) : base(dbContext) { }

        /// <summary>
        ///     Devuelve los objetos que componen la cadena de Código de Región.
        /// </summary>
        /// <param name="regionCode">
        ///     Código de Región a "parsear".
        /// </param>
        public RegionMetadata GetRegionMetadata(string regionCode) {
            var codes = regionCode.Split(new char[] {'-'}, 4);
            var returnMetadata = new RegionMetadata();
            var currentCode = "";

            if (codes.Length < 1 || String.IsNullOrEmpty(codes[0]))
                return returnMetadata;

            currentCode = codes[0];
            returnMetadata.Region = GetFirst(
                f => f.IsoCode == currentCode
            );

            if ( returnMetadata.Region == null || codes.Length < 2 || String.IsNullOrEmpty(codes[1]) )
                return returnMetadata;

            currentCode = codes[1];
            returnMetadata.RegionSubdivision = returnMetadata.Region.RegionSubdivision.FirstOrDefault(
                w => w.IsoCode == currentCode
            );

            if ( returnMetadata.RegionSubdivision == null || codes.Length < 3 || String.IsNullOrEmpty(codes[2]) )
                return returnMetadata;

            currentCode = codes[2];
            returnMetadata.RegionParticular = returnMetadata.RegionSubdivision.RegionParticular.FirstOrDefault(
                w => w.Code == currentCode
            );

            return returnMetadata;
        }
    }
}
