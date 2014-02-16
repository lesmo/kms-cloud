using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using KilometrosDatabase.Helpers;
using Kilometros_WebGlobalization.Database;

namespace KilometrosDatabase {
    public partial class User : IValidatableObject {
        /// <summary>
        /// Genera el Hash de la cadena que se asigne, o devuelve la representación
        /// en Texto del Hash en {User.Password}. La contraseña se trata como UTF-8.
        /// </summary>
        public string PasswordString {
            get {
                // > Convertir el Hash en byte[] a Texto si se necesita
                if ( this._passwordHashString == null ) {
                    StringBuilder hashString = new StringBuilder();

                    foreach ( byte b in this.Password )
                        hashString.Append(b.ToString("X2"));

                    this._passwordHashString = hashString.ToString();
                }

                return this._passwordHashString;
            }
            set {
                // > Calcular Hash y establecer el valor a almacenarse en BD
                SHA256 hashing
                    = SHA256.Create();
                byte[] stringBytes
                    = Encoding.UTF8.GetBytes(value);
                byte[] computedHash
                    = hashing.ComputeHash(stringBytes);
                byte[] computedHash2
                    = hashing.ComputeHash(computedHash);
                
                // > Almacenar nuevos valores
                this.Password = computedHash2;
                this._passwordHashString = null; // Forzar la re-conversión del Hash a Texto
            }
        }
        private string _passwordHashString = null;

        /// <summary>
        /// Información de la Cultura (Idioma) preferido por el Usuario
        /// </summary>
        public CultureInfo PreferredCultureInfo {
            get {
                if ( this._preferredCultureInfo == null ) {
                    try {
                        this._preferredCultureInfo = new CultureInfo(this.PreferredCultureCode);
                    } catch {
                        this._preferredCultureInfo = null;
                    }
                }
                 
                return this._preferredCultureInfo;   
            }
            set {
                this.PreferredCultureCode = value.Name;
                this._preferredCultureInfo = value;
            }
        }
        private CultureInfo _preferredCultureInfo = null;

        /// <summary>
        /// Ejecutar la validación de Usuario
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            List<ValidationResult> validationErrors = new List<ValidationResult>();

            // > Validar dirección de correo electrónico
            try {
                var e = new System.Net.Mail.MailAddress(this.Email);
            } catch {
                validationErrors.Add(
                    new ValidationResult(
                        EntityValidationStrings.EmailInvalid,
                        new [] {"Email"}
                    )
                );
            }

            // > Validar Cultura (Idioma) preferida
            if ( this.PreferredCultureInfo == null ) {
                validationErrors.Add(
                    new ValidationResult(
                        EntityValidationStrings.PreferredCultureInvalid,
                        new[] { "PreferredCultureCode" }
                    )
                );
            }

            return validationErrors;
        }
    }
}
