using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace KilometrosDatabase {
    public partial class RewardGiftToken : IValidatableObject {
        /// <summary>
        /// Ejecutar la validación de Usuario
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            List<ValidationResult> validationErrors = new List<ValidationResult>();

            // > Validar la Fecha de Expiración
            if (
                   this.ExpirationDate.HasValue
                && DateTime.Compare((DateTime)this.ExpirationDate, DateTime.UtcNow) < 0
            ) {
                validationErrors.Add(
                    new ValidationResult("Expiration Date is in the past, time machines don't exist yet", new[] { "ExpirationDate" })
                );
            }

            return validationErrors;
        }
    }
}
