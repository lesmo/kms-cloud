using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.RequestModels {
    public class DataPost {
        /// <summary>
        ///     Estampa de Tiempo del registro generado por el Dispositivo KMS en UTC.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        ///     Número de pasos reportados por el Dispositivo KMS.
        /// </summary>
        public Int32 Steps { get; set; }

        /// <summary>
        ///     Actividad que reporta el Dispositivo KMS. Puede ser: "running", "walking" o "sleeping".
        ///     En un futuro ésta propiedad podría no modificar la forma en que se agrupa estadísticamente
        ///     la información subida a la Nube KMS.
        /// </summary>
        public String Activity { get; set; }
    }
}