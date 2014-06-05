using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.RequestModels{
    public class CreateKmsAccountPost {
        /// <summary>
        ///     Nombre (real) del Usuario.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Apellido del Usuario.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Dirección de E-mail del Usuario.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Contraseña a utilizarse para iniciar sesión en KMS. Enviada en texto plano. ¡No te preocupes!
        ///     La conexión con KMS es através de SSL, así que todo está bien.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Código de Región del Usuario.
        /// </summary>
        public string RegionCode { get; set; }

        /// <summary>
        ///     Código de Cultura del Usuario.
        /// </summary>
        public string CultureCode { get; set; }

        /// <summary>
        ///     ATENCIÓN: Propiedad "deprecated". Se eliminará del API y BD muy pronto.
        ///     Franja UTC preferida por el Usuario.
        /// </summary>
        public short UtcOffset { get; set; }

        /// <summary>
        ///     Fecha de nacimiento del Usuario.
        /// </summary>
        public DateTime Birthdate { get; set; }

        /// <summary>
        ///     Estatura del Usuario EN CENTÍMETROS.
        /// </summary>
        public short Height { get; set; }

        /// <summary>
        ///     Peso del Usuario EN GRAMOS.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        ///     Género del Usuario. Puede ser "m" para Masculino, o "f" para Femenino.
        ///     Es indistitnto para mayúsculas y minúsculas.
        /// </summary>
        public char Gender { get; set; }
    }
}