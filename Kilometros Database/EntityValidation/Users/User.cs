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
        /// Ejecutar la validación de Usuario
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            List<ValidationResult> validationErrors = new List<ValidationResult>();

            // > Validar que la Edad esté entre 0 y 110
            if ( this.Age < 0 )
                validationErrors.Add(
                    new ValidationResult(
                        EntityValidationStrings.UserTooYoung,
                        new[] { "Age" }
                    )
                );
            if ( this.Age > 110 )
                validationErrors.Add(
                    new ValidationResult(
                        EntityValidationStrings.UserTooOld,
                        new[] { "Age" }
                    )
                );

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
