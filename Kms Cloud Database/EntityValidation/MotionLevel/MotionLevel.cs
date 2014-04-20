using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Kms.Cloud.Database {
    public partial class MotionLevel : IValidatableObject {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            List<ValidationResult> validationErrors = new List<ValidationResult>();

            // > Validar Umbral de Días para considerar a un Usuario en éste Nivel de Actividad
            if ( this.DaysThresholdStart > this.DaysThresholdEnd )
                validationErrors.Add(
                    new ValidationResult("Days Threshold is invalid", new[] { "DaysThresholdStart", "DaysThresholdEnd" })
                );

            // > Validar Umbral de Días para considerar a un Usuario en éste Nivel de Actividad
            if ( this.DistanceThresholdStart > this.DistanceThresholdEnd )
                validationErrors.Add(
                    new ValidationResult("Distance Threshold is invalid", new[] { "DistanceThresholdStart", "DistanceThresholdEnd" })
                );
            
            return validationErrors;
        }
    }
}
