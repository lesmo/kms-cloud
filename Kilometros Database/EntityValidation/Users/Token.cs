﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace KilometrosDatabase {
    public partial class Token : IValidatableObject {
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
                    new ValidationResult("Expiration Date is in the past, time machines don't exist yet", new[] { "ExpirationDate" })
                );
            }

            return validationErrors;
        }
    }
}
