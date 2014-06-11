using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.RequestModels {
    public class OAuthFacebookLoginPost : IOAuthTokenPost {
        /// <summary>
        ///     ID del Usuario de Facebook, y es un número. Sin embargo, el número es enorme y
        ///     por recomendación de Facebook debe tratarse como una cadena de texto.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        ///     Código de sesión de Facebook. Cuando se realiza el proceso normal de Login, DEBE
        ///     solicitarse a Facebook que se genere un Token y un Código.
        /// </summary>
        /// <remarks>
        ///     El Código de Facebook NO ES el Token, es un código generado por el proceso de Login
        ///     y es diferente del Token. Facebook utiliza este proceso para poder compartir la
        ///     Sesión del Usuario entre Apps y Servidor Web, y mantener así Tokens independientes
        ///     y seguros.
        /// </remarks>
        public string Code { get; set; }
    }
}