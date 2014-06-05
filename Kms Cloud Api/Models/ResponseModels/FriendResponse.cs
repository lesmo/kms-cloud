using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class FriendResponse {
        /// <summary>
        ///     ID de Usuario del Amigo.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        ///     Fecha de Creación de la Cuenta del Amigo.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        ///     Nombre (real) del Usuario amigo.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Apellido del Usuario amigo.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     URL a la Foto de Perfil del amigo.
        /// </summary>

        public Uri PictureUri { get; set; }
    }
}