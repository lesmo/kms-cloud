using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.HttpGet.DataController {
    /// <summary>
    ///     Información sobre Total de pasos y distancias alcanzadas.
    /// </summary>
    public class DataTotalResponse {
        /// <summary>
        ///     Distancia total alcanzada corriendo.
        /// </summary>
        public long RunningTotalDistance;
        /// <summary>
        ///     Distancia total alcanzada caminando.
        /// </summary>
        public long WalkingTotalDistance;
        /// <summary>
        ///     Distancia total alcanzada.
        /// </summary>
        public long TotalDistance;

        /// <summary>
        ///     Pasos totales dados corriendo.
        /// </summary>
        public long RunningTotalSteps;
        /// <summary>
        ///     Pasos totales dados caminando.
        /// </summary>
        public long WalkingTotalSteps;
        /// <summary>
        ///     Pasos totales dados.
        /// </summary>
        public long TotalSteps;

        /// <summary>
        ///     Fecha del último dato registrado.
        /// </summary>
        public DateTime LastModified;
    }
}