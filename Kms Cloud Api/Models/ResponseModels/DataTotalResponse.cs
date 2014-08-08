using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.ResponseModels {
    /// <summary>
    ///     Información sobre Total de pasos y distancias alcanzadas.
    /// </summary>
    public class DataTotalResponse {
        /// <summary>
        ///     Distancia total alcanzada corriendo.
        /// </summary>
        public double RunningTotalDistance { get; set; }
        /// <summary>
        ///     Distancia total alcanzada caminando.
        /// </summary>
        public double WalkingTotalDistance { get; set; }
        /// <summary>
        ///     Distancia total alcanzada.
        /// </summary>
        public double TotalDistance { get; set; }

        /// <summary>
        ///     Pasos totales dados corriendo.
        /// </summary>
        public long RunningTotalSteps { get; set; }
        /// <summary>
        ///     Pasos totales dados caminando.
        /// </summary>
        public long WalkingTotalSteps { get; set; }
        /// <summary>
        ///     Pasos totales dados.
        /// </summary>
        public long TotalSteps { get; set; }

        /// <summary>
        ///     Fecha del último dato registrado.
        /// </summary>
        public DateTime LastModified { get; set; }
    }
}