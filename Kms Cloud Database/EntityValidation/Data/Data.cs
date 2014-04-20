using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Kms.Cloud.Database {
    public partial class Data : IValidatableObject {
        /// <summary>
        /// Ejecutar la validación de Datos de Actividad
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            List<ValidationResult> validationErrors = new List<ValidationResult>();

            // > Validar que la Estampa de Tiempo no sea en el Futuro
            if ( this.Timestamp > DateTimeOffset.Now )
                validationErrors.Add(
                    new ValidationResult("Activity Data can't be in the future, there's not time machines yet.", new[] { "TimeStamp" })
                );

            return validationErrors;
        }
    }
}
