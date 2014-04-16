using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase {
    public partial class User {
        /// <summary>
        /// Devuelve la Edad del Usuario a partir de la Fecha de Nacimiento almacenada
        /// en la Base de Datos.
        /// </summary>
        public short Age {
            get {
                short age
                    = (short)(DateTime.Today.Year - this.BirthDate.Year);

                if ( this.BirthDate > DateTime.Today.AddYears(-age) )
                    age--;

                return age;
            }
        }

        private byte[] ComputePasswordHash(string password) {
            SHA256 hashing
                    = new SHA256CryptoServiceProvider();
            byte[] stringBytes
                = Encoding.UTF8.GetBytes(password);
            byte[] computedHash
                = hashing.ComputeHash(stringBytes);
            byte[] computedHash2
                = hashing.ComputeHash(computedHash);

            return computedHash;
        }

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
                // > Almacenar nuevos valores
                this.Password = this.ComputePasswordHash(value);
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
        /// Distancia Total Recorrida por el Usuario (sumatoria de distancia de todas las actividades,
        /// excepto Sueño).
        /// </summary>
        public UserDataTotalDistance UserDataTotalDistanceSum {
            get {
                if ( this._userDataTotalDistanceSum != null )
                    return this._userDataTotalDistanceSum;

                this._userDataTotalDistanceSum
                    = (
                        from d in this.UserDataTotalDistance
                        where
                            d.User.Guid == this.Guid
                            && d.Activity != DataActivity.Sleep
                        group d by new {
                            userGuid
                                = d.User.Guid
                        } into g
                        select new UserDataTotalDistance {
                            TotalDistance
                                = g.Sum(s => s.TotalDistance),
                            TotalSteps
                                = g.Sum(s => s.TotalSteps),
                            TotalKcal
                                = g.Sum(s => s.TotalKcal),
                            TotalCo2
                                = g.Sum(s => s.TotalCo2),
                            TotalCash
                                = g.Sum(s => s.TotalCash)
                        }
                    ).FirstOrDefault();

                if ( this._userDataTotalDistanceSum == null )
                    this._userDataTotalDistanceSum
                        = new UserDataTotalDistance() {
                            TotalDistance
                                = 0,
                            TotalSteps
                                = 0,
                            TotalKcal
                                = 0,
                            TotalCo2
                                = 0,
                            TotalCash
                                = 0
                        };
                
                return this._userDataTotalDistanceSum;
            }
        }
        private UserDataTotalDistance _userDataTotalDistanceSum = null;

        /// <summary>
        /// Determina si la contraseña del Usuario coincide con ésta especificada.
        /// </summary>
        /// <param name="password">Contraseña contra la cual se comparará la Contraseña del Usuario.</param>
        /// <returns>Si la contraseña coincide.</returns>
        public bool PasswordMatches(string password) {
            byte[] passwordHash
                = this.ComputePasswordHash(password);

            return passwordHash.SequenceEqual(this.Password);
        }
    }
}
