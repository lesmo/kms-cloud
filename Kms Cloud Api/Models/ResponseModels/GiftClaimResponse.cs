using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class GiftClaimResponse {
        /// <summary>
        ///     Fecha de Expiración del Código de Canje.
        /// </summary>
        public DateTime ExpirationDate { get; set; }

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
    }
}