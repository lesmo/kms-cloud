using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using Kilometros_WebGlobalization.Database;

namespace Kms.Cloud.Database {
    public partial class UserRewardGiftClaimed : IValidatableObject {
        /// <summary>
        /// Ejecutar la validación de Usuario
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            List<ValidationResult> validationErrors = new List<ValidationResult>();

            // > Validar la Fecha de Expiración
            if (
                this.ExpirationDate.HasValue
                && this.ExpirationDate.Value < DateTimeOffset.Now
            ) {
                validationErrors.Add(
                    new ValidationResult(
                        EntityValidationStrings.ExpirationDateIsPast,
                        new[] { "ExpirationDate" }
                    )
                );
            }

            return validationErrors;
        }
    }
}
