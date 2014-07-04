using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class WebAppLinkResponse {

        /// <summary>
        ///     Secret que debes pasar por un hash de HMAC-SHA1 antes de utilizarlo en UriMask. El hash lo generas
        ///     utilizando la misma llave que utilizas para firmar las peticiones de OAuth (Consumer Secret + & +
        ///     Token Secret).
        /// </summary>
        /// <remarks>
        ///     Por ejemplo si tu Consumer Secret es "ABC" y tu Token Secret es "XYZ", la llave para el HMAC-SHA1
        ///     es "ABC&XYZ".
        /// </remarks>
        public String AutoLoginSecret {
            get;
            set;
        }

        /// <summary>
        ///     Máscara de la URI para auto-login en la Web App. Sustituye {0} en ésta cadena
        ///     por el HMAC-SHA1 del AutoLoginToken.
        /// </summary>
        public String UriMask {
            get;
            set;
        }

    }
}