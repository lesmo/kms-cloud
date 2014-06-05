using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.RequestModels {
    public class ContactInfoPost {
        /// <summary>
        ///     Teléfono de Casa del Usuario.
        /// </summary>
        public string HomePhone { get; set; }

        /// <summary>
        ///     Teléfono Móvil del Usuario.
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        ///     Teléfono de Trabajo del Usuario.
        /// </summary>
        public string WorkPhone { get; set; }
    }
}