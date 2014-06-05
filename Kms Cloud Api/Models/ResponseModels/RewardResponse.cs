using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class RewardResponse {
        /// <summary>
        ///     ID de la Recompensa.
        /// </summary>
        public string RewardId { get; set; }
        /// <summary>
        ///     Fecha en la que se consiguió la Recompensa.
        /// </summary>
        public DateTime EarnDate { get; set; }

        /// <summary>
        ///     Título de la Recompensa.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        ///     Texto de la Recompensa.
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        ///     Fuente de la Recompensa (para citas, contextualización o argumentación).
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        ///     Regalos disponibles para Reclamar/Canjear por desbloquear la recompensa.
        /// </summary>
        public IEnumerable<RewardGiftResponse> RewardGifts { get; set; }

        /// <summary>
        ///     Códigos de Regiones en las que está disponbile ésta Recompensa.
        /// </summary>
        public IEnumerable<String> RegionCodes { get; set; }
    }
}