﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace KilometrosDatabase {
    partial class UserBody : IValidatableObject {
        /// <summary>
        /// Ejecutar validación de Complexión Física del Usuario
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            List<ValidationResult> validationErrors = new List<ValidationResult>();

            // > Validar que la Edad esté entre 0 y 110
            if ( this.Age < 0)
                validationErrors.Add(
                    new ValidationResult("User is too young", new[] { "Age" })
                );
            if ( this.Age > 110 )
                validationErrors.Add(
                    new ValidationResult("User is too old", new[] { "Age" })
                );

            // > Validar que la Altura del usuario sea entre 40cm y 300cm
            if ( this.Height < 40 )
                validationErrors.Add(
                    new ValidationResult("User is too short", new[] { "Height" })
                );
            if ( this.Height > 300 )
                validationErrors.Add(
                    new ValidationResult("User is too tall", new[] { "Height" })
                );

            // > Validar el Peso del Usuario
            if ( this.Weight < 2000 )
                validationErrors.Add(
                    new ValidationResult("User is too light", new[] { "Weight" })
                );
            if ( this.Weight > 300000 )
                validationErrors.Add(
                    new ValidationResult("User is too heavy", new[] { "Weight" })
                );

            // > Validar el sexo del Usuario
            this.Sex = this.Sex.ToLowerInvariant();
            if ( this.Sex != "m" && this.Sex != "f" )
                validationErrors.Add(
                    new ValidationResult("Invalid Sex", new[] { "Sex" })
                );

            return validationErrors;
        }
    }
}