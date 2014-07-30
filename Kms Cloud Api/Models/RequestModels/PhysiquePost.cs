using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.RequestModels{
    public class PhysiquePost {
        /// <summary>
        ///     Edad del Usuario. Exacta.
        /// </summary>
        public short Age { get; set; }

        /// <summary>
        ///     Altura del Usuario EN CENTÍMETROS.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        ///     Peso del Usuario EN GRAMOS.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        ///     Género del Usuario. Puede ser "M" para Masculino o "M" para Femenino.
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        ///     Distancia de zancada del Usuario caminando EN CENTÍMETROS. Actualmente se
        ///     ignora por completo.
        /// </summary>
        public int StrideLengthWalking { get; set; }

        /// <summary>
        ///     Distancia de zancada del Usuario corriendo EN CENTÍMETROS. Actualmente se
        ///     ignora por completo.
        /// </summary>
        public int StrideLengthRunning { get; set; }
    }
}