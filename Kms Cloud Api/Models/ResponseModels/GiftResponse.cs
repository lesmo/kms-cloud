using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class GiftResponse {
        /// <summary>
        ///     Nombre en singular del Regalo. Por ejemplo: "Boleto de Cine".
        /// </summary>
        public string NameSingular { get; set; }

        /// <summary>
        ///     Nombre en plural del Regalo. Por ejemplo: "Boletos de Cine".
        /// </summary>
        public string NamePlural { get; set; }

        /// <summary>
        ///     Código de Canje.
        /// </summary>
        public string RedeemCode { get; set; }

        /// <summary>
        ///     URL a la representación gráfica del Código de Canje. Por ejemplo: un
        ///     Código QR, un Código de Barras, o un MIcrosoft Tag (sí, claro) o algo
        ///     similar. Podría aprovecharse Passbook en iOS para almacenar éste
        ///     código en el dispositivo del Usuario.
        /// </summary>
        public Uri RedeemPicture { get; set; }

        /// <summary>
        ///     Lista de URLs de las fotografías del regalo. Por ejemplo: una fotografía
        ///     de un frappé.
        /// </summary>
        public IEnumerable<Uri> Pictures { get; set; }
    }
}