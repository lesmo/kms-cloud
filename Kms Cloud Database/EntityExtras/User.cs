using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CryptSharp;
using CryptSharp.Utility;

namespace Kms.Cloud.Database {
    public partial class User {
        /// <summary>
        ///     Devuelve la Edad del Usuario a partir de la Fecha de Nacimiento almacenada
        ///     en la Base de Datos.
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

        private byte[] ComputeSha256PasswordHash(string password) {
            var hashing      = new SHA256CryptoServiceProvider();
            var stringBytes  = Encoding.UTF8.GetBytes(password);
            var computedHash = hashing.ComputeHash(stringBytes);
            
            return computedHash;
        }

        /// <summary>
        /// Establece la contraseña del Usuario. 
        /// </summary>
        public void SetPassword(string password) {
            // > Validar la nueva contraseña
            if ( String.IsNullOrEmpty(password) || password.Length < 6 )
                throw new ArgumentException("Password cannot be less than 6 characters long");

            // > Generar Salt + Hash
            var salt = Crypter.Blowfish.GenerateSalt(new CrypterOptions {
                { CrypterOption.Rounds, 9 }
            });
            var bcrypt = Crypter.Blowfish.Crypt(password, salt);

            // > Establecer la contraseña en la Entidad
            this.Password = Encoding.UTF8.GetBytes(bcrypt);
            this.PasswordSalt = Encoding.UTF8.GetBytes(salt);
        }

        /// <summary>
        /// Determina si la contraseña del Usuario coincide con ésta especificada.
        /// </summary>
        /// <param name="password">Contraseña contra la cual se comparará la Contraseña del Usuario.</param>
        /// <returns>Si la contraseña coincide.</returns>
        public bool PasswordMatches(string password) {
            if ( this.Password == null )
                return false;

            // > Determinar si el hash almacenado es BCRYPT o SHA256
            var storedPasswordHash = Encoding.UTF8.GetString(this.Password);
            if ( storedPasswordHash.StartsWith("$2a$") ) {
                // Si no está almacenado el salt, preceder como si no coincidieran
                if ( this.PasswordSalt == null )
                    return false;

                // Re-generar hash con salt almacenado
                var salt   = Encoding.UTF8.GetString(this.PasswordSalt);
                var bcrypt = Crypter.Blowfish.Crypt(password, salt);
                    
                // Devolver si los hashes coinciden
                return storedPasswordHash == bcrypt;
            } else {
                // Validar contraseña
                var shaBytes = this.ComputeSha256PasswordHash(password);
                var matches  = shaBytes.SequenceEqual(this.Password);

                if ( ! matches )
                    return false;

                // Si la contraseña coincide, la "upgradeamos" a BCRYPT
                this.SetPassword(password);
                return true;
            }
        }

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
                            Timestamp
                                = g.Max(s => s.Timestamp),
                            TotalDistance
                                = g.Sum(s => s.TotalDistance),
                            TotalSteps
                                = g.Sum(s => s.TotalSteps),
                            TotalKcal
                                = (double)g.Sum(s => s.TotalKcal),
                            TotalCo2
                                = (double)g.Sum(s => s.TotalCo2),
                            TotalCash
                                = (double)g.Sum(s => s.TotalCash)
                        }
                    ).FirstOrDefault();

                if ( this._userDataTotalDistanceSum == null )
                    this._userDataTotalDistanceSum
                        = new UserDataTotalDistance() {
                            Timestamp
                                = default(DateTime),
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

        public MotionLevel CurrentMotionLevel {
            get {
                if (  this._currentMotionLevel != null )
                    return this._currentMotionLevel;

                UserMotionLevelHistory motionHistory
                    = this.UserMotionLevelHistory
                        .OrderByDescending(b => b.CreationDate)
                        .Take(1)
                        .FirstOrDefault();

                if ( motionHistory == null )
                    return null;

                this._currentMotionLevel
                    = motionHistory.MotionLevel;

                return this._currentMotionLevel;
            }
        }
        private MotionLevel _currentMotionLevel;

        /// <summary>
        ///     Devuelve el nombre completo del Usuario en Title Casing
        /// </summary>
        public String FullName {
            get {
                if ( ! String.IsNullOrEmpty(this._fullName) )
                    return this._fullName;

                this._fullName = CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(this.Name + " " +this.LastName);
                return this._fullName;
            }
        }
        private String _fullName;

    }
}
