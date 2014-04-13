using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase {
    public static class GuidStringify {
        /// <summary>
        ///     Convierte el Guid en su representación "compacta" de Base64, sin carácteres
        ///     considerados inválidos para URL.
        /// </summary>
        /// <param name="guid">
        ///     Guid a convertir a Base64.
        /// </param>
        /// <returns>
        ///     Devuelve cadena de representación "compacta" en Base 64 del Guid.
        /// </returns>
        public static string ToBase64String(this Guid guid) {
            byte[] guidBytes
                = guid.ToByteArray();
            string guidBase64String
                = Convert.ToBase64String(guidBytes);

            guidBase64String
                = guidBase64String.Replace('/', '$');
            guidBase64String
                = guidBase64String.Replace('+', '-');

            return guidBase64String.Remove(
                guidBase64String.Length - 2
            );
        }

        public static Guid FromBase64String(this Guid guid, string compactBase64) {
            if ( compactBase64.Length != 22 )
                return default(Guid);

            compactBase64
                = compactBase64.Replace('$', '/');
            compactBase64
                = compactBase64.Replace('-', '+');

            byte[] guidBytes
                = Convert.FromBase64String(
                    compactBase64 + "=="
                );

            return new Guid(guidBytes);
        }
    }
}
