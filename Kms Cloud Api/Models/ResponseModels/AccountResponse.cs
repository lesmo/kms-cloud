using System;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class AccountResponse {
        /// <summary>
        ///     Fecha de Creación de la Cuenta.
        /// </summary>
        public DateTime AccountCreationDate { get; set; }
        /// <summary>
        ///     ID de la Cuenta.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        ///     Nombre (real) del Usuario.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Apellido del Usuario.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Nombre Completo del Usuario. Es el Nombre + Apellido con
        ///     mayúscula en las primeras letras.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        ///     Dirección de E-mail del Usuario.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Código de Región del Usuario (por ejemplo "MX-DIF").
        /// </summary>
        public string RegionCode { get; set; }

        /// <summary>
        ///     Región del Usuario en texto (por ejemplo "Distrito Federal, México").
        /// </summary>
        public string RegionFull { get; set; }

        /// <summary>
        ///     Código de Cultura preferido por el Usuario.
        /// </summary>
        public string PreferredCultureCode { get; set; }

        /// <summary>
        ///     URL a la Fotografía de Perfil del Usuario.
        /// </summary>
        public Uri PictureUri { get; set; }
    }
}