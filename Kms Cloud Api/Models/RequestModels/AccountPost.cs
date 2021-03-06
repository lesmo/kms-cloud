﻿namespace Kms.Cloud.Api.Models.RequestModels {
    public class AccountPost {
        /// <summary>
        ///     Nombre (real) del Usuario.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Dirección de E-mail del Usuario.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Código de Cultura preferido por el Usuario.
        /// </summary>
        public string PreferredCultureCode { get; set; }

        /// <summary>
        ///     Código de Región del Usuario.
        /// </summary>
        /// <remarks>
        ///     Si el usuario tiene una Región Particular asociada (por ejemplo si es MX-DIF-TELMEX), no será
        ///     posible modificar ningún detalle de la región del Usuario.
        /// </remarks>
        public string RegionCode { get; set; }

        /// <summary>
        ///     Contraseña a utilizarse para iniciar sesión en KMS. Enviada en texto plano. ¡No te preocupes!
        ///     La conexión con KMS es através de SSL, así que todo está bien. Cambiar la contraseña invalidará
        ///     TODOS los Tokens de Sesión del Usuario actual.
        /// </summary>
        public string Password { get; set; }
    }
}