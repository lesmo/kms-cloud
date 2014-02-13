using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.Abstraction {
    public sealed class WorkUnit : IDisposable {
        private MainModelContainer _context = new MainModelContainer();

        /// <summary>
        /// Sólo aquí... implementando IDisposable
        /// </summary>
        public void Dispose() {
            this._context.Dispose();
        }

        public IEnumerable<DbEntityValidationResult> GetValidationErrors() {
            return this._context.GetValidationErrors();
        }
        
        /// <summary>
        /// Almacena en BD los cambios realizados hasta el momento.
        /// </summary>
        public int SaveChanges() {
            return this._context.SaveChanges();
        }

        #region Security and Access Control to the API
        /// <summary>
        /// Almacén de Api-Keys de las Apps
        /// </summary>
        public GenericRepository<ApiKey> ApiKeyStore {
            get {
                if ( this._apiKeyRepository == null )
                    this._apiKeyRepository = new GenericRepository<ApiKey>(this._context);

                return this._apiKeyRepository;
            }
        }
        private GenericRepository<ApiKey> _apiKeyRepository = null;

        /// <summary>
        /// Almacén de Tokens de Sesión de las Apps
        /// </summary>
        public GenericRepository<Token> TokenStore {
            get {
                if ( this._tokenRepository == null )
                    this._tokenRepository = new GenericRepository<Token>(this._context);

                return this._tokenRepository;
            }
        }
        private GenericRepository<Token> _tokenRepository = null;
        #endregion

        #region User Information Storage
        /// <summary>
        /// Almacén de Usuarios
        /// </summary>
        public GenericRepository<User> UserStore {
            get {
                if ( this._userRepository == null )
                    this._userRepository = new GenericRepository<User>(this._context);

                return this._userRepository;
            }
        }
        private GenericRepository<User> _userRepository = null;

        /// <summary>
        /// Almacén de Complexión Física de los Usuarios
        /// </summary>
        public GenericRepository<UserBody> UserBodyStore {
            get {
                if ( this._userBodyRepository == null )
                    this._userBodyRepository = new GenericRepository<UserBody>(this._context);

                return this._userBodyRepository;
            }
        }
        private GenericRepository<UserBody> _userBodyRepository = null;

        /// <summary>
        /// Almacén de Información de Contacto de los Usuarios
        /// </summary>
        public GenericRepository<ContactInfo> ContactInfoStore {
            get {
                if ( this._contactInfoRepository == null )
                    this._contactInfoRepository = new GenericRepository<ContactInfo>(this._context);

                return this._contactInfoRepository;
            }
        }
        private GenericRepository<ContactInfo> _contactInfoRepository = null;
        #endregion


        #region User Wristband Device Ownership Storage
        /// <summary>
        /// Almacén de Histórico de Propiedad de Dispositivo
        /// </summary>
        public GenericRepository<WristBandUserOwnership> WristBandUserOwnershipStore {
            get {
                if ( this._wristBandUserOwnershipRepository == null )
                    this._wristBandUserOwnershipRepository = new GenericRepository<WristBandUserOwnership>(this._context);

                return this._wristBandUserOwnershipRepository;
            }
        }
        private GenericRepository<WristBandUserOwnership> _wristBandUserOwnershipRepository = null;

        /// <summary>
        /// Almacén de información de Dispositivos
        /// </summary>
        public GenericRepository<WristBand> WristBandStore {
            get {
                if ( this._wristBandRepository == null )
                    this._wristBandRepository = new GenericRepository<WristBand>(this._context);

                return this._wristBandRepository;
            }
        }
        private GenericRepository<WristBand> _wristBandRepository = null;

        /// <summary>
        /// Almacén de Información de Actividad
        /// </summary>
        public GenericRepository<Data> DataStore {
            get {
                 if ( this._dataRepository == null )
                    this._dataRepository = new GenericRepository<Data>(this._context);

                return this._dataRepository;
            }
        }
        private GenericRepository<Data> _dataRepository = null;
        #endregion

        #region Tips Storage
        /// <summary>
        /// Almacén de Tips
        /// </summary>
        public GenericRepository<Tip> TipStore {
            get {
                if ( this._tipRepository == null )
                    this._tipRepository = new GenericRepository<Tip>(this._context);

                return this._tipRepository;
            }
        }
        private GenericRepository<Tip> _tipRepository = null;

        /// <summary>
        /// Almacén de Categorías de Tips
        /// </summary>
        public GenericRepository<TipCategory> TipCategoryStore {
            get {
                if ( this._tipCategoryRepository == null )
                    this._tipCategoryRepository = new GenericRepository<TipCategory>(this._context);

                return this._tipCategoryRepository;
            }
        }
        private GenericRepository<TipCategory> _tipCategoryRepository = null;
        #endregion

        #region Motion Level and History
        /// <summary>
        /// Almacén de Nivel de Actividad de los Usuarios
        /// </summary>
        public GenericRepository<MotionLevel> MotionLevelStore {
            get {
                if ( this._motionLevelRepository == null )
                    this._motionLevelRepository = new GenericRepository<MotionLevel>(this._context);

                return this._motionLevelRepository;
            }
        }
        private GenericRepository<MotionLevel> _motionLevelRepository = null;

        /// <summary>
        /// Almacén de Historial de cambios en el Nivel de Actividad de los Usuarios
        /// </summary>
        public GenericRepository<UserMotionLevelHistory> UserMotionLevelStore {
            get {
                if ( this._userMotionLevelHistoryRepository == null )
                    this._userMotionLevelHistoryRepository = new GenericRepository<UserMotionLevelHistory>(this._context);

                return this._userMotionLevelHistoryRepository;
            }
        }
        private GenericRepository<UserMotionLevelHistory> _userMotionLevelHistoryRepository = null;
        #endregion

        #region Rewards (not including Gifts) and User Earned Rewards Storage
        /// <summary>
        /// Almacén de Recompensas
        /// </summary>
        public GenericRepository<Reward> RewardStore {
            get {
                if ( this._rewardRepository == null )
                    this._rewardRepository = new GenericRepository<Reward>(this._context);

                return this._rewardRepository;
            }
        }
        private GenericRepository<Reward> _rewardRepository = null;

        /// <summary>
        /// Almacén de la Regionalización de las Recompensas
        /// </summary>
        public GenericRepository<RewardRegionalization> RewardRegionalizationStore {
            get {
                if ( this._rewardRegionalization == null )
                    this._rewardRegionalization = new GenericRepository<RewardRegionalization>(this._context);

                return this._rewardRegionalization;
            }
        }
        private GenericRepository<RewardRegionalization> _rewardRegionalization = null;

        /// <summary>
        /// Almacén de Recompensas conseguidas por los Usuarios
        /// </summary>
        public GenericRepository<UserEarnedReward> UserEarnedRewardStore {
            get {
                if ( this._userEarnedRewardRepository == null )
                    this._userEarnedRewardRepository = new GenericRepository<UserEarnedReward>(this._context);

                return this._userEarnedRewardRepository;
            }
        }
        private GenericRepository<UserEarnedReward> _userEarnedRewardRepository = null;
        #endregion

        #region Reward Gifts and Redeeming Codes Storage
        /// <summary>
        /// Almacén de Regalos por Recompensa
        /// </summary>
        public GenericRepository<RewardGift> RewardGiftStore {
            get {
                if ( this._rewardGiftRepository == null )
                    this._rewardGiftRepository = new GenericRepository<RewardGift>(this._context);

                return this._rewardGiftRepository;
            }
        }
        private GenericRepository<RewardGift> _rewardGiftRepository = null;

        /// <summary>
        /// Almacén de Imágenes de los Regalos por Recompensa
        /// </summary>
        public GenericRepository<RewardGiftImage> RewardImageStore {
            get {
                if ( this._rewardGiftImageRepository == null )
                    this._rewardGiftImageRepository = new GenericRepository<RewardGiftImage>(this._context);

                return this._rewardGiftImageRepository;
            }
        }
        private GenericRepository<RewardGiftImage> _rewardGiftImageRepository = null;

        /// <summary>
        /// Almacén de Códigos de Canje de Regalos por Recompensa
        /// </summary>
        public GenericRepository<RewardGiftToken> RewardGiftTokenStore {
            get {
                if ( this._rewardGiftTokenRepository == null )
                    this._rewardGiftTokenRepository = new GenericRepository<RewardGiftToken>(this._context);

                return this._rewardGiftTokenRepository;
            }
        }
        private GenericRepository<RewardGiftToken> _rewardGiftTokenRepository = null;
        #endregion

        #region Globalization Storage
        /// <summary>
        /// Almacén de Cadenas de Traducción de Niveles de Actividad
        /// </summary>
        public GenericRepository<MotionLevelGlobalization> MotionLevelGlobalizationStore {
            get {
                if ( this._motionLevelGlobalizationRepository == null )
                    this._motionLevelGlobalizationRepository = new GenericRepository<MotionLevelGlobalization>(this._context);

                return this._motionLevelGlobalizationRepository;
            }
        }
        private GenericRepository<MotionLevelGlobalization> _motionLevelGlobalizationRepository = null;

        /// <summary>
        /// Almacén de Cadenas de Traducción de Regalos por Recompensa
        /// </summary>
        public GenericRepository<RewardGiftGlobalization> RewardGiftGlobalizationStore {
            get {
                if ( this._rewardGiftGlobalizationRepository == null )
                    this._rewardGiftGlobalizationRepository = new GenericRepository<RewardGiftGlobalization>(this._context);

                return this._rewardGiftGlobalizationRepository;
            }
        }
        private GenericRepository<RewardGiftGlobalization> _rewardGiftGlobalizationRepository = null;

        /// <summary>
        /// Almacén de Cadenas de Traducción de Recompensas
        /// </summary>
        public GenericRepository<RewardGlobalization> RewardGlobalizationStore {
            get {
                if ( this._rewardGlobalizationRepository == null )
                    this._rewardGlobalizationRepository = new GenericRepository<RewardGlobalization>(this._context);

                return this._rewardGlobalizationRepository;
            }
        }
        private GenericRepository<RewardGlobalization> _rewardGlobalizationRepository = null;

        /// <summary>
        /// Almacén de Cadenas de Traducción de Categorías de TIps
        /// </summary>
        public GenericRepository<TipCategoryGlobalization> TipCategoryGlobalization {
            get {
                if ( this._tipCategoryGlobalization == null )
                    this._tipCategoryGlobalization = new GenericRepository<TipCategoryGlobalization>(this._context);

                return this._tipCategoryGlobalization;
            }
        }
        private GenericRepository<TipCategoryGlobalization> _tipCategoryGlobalization = null;

        /// <summary>
        /// Almacén de Cadenas de Traducción de Tips
        /// </summary>
        public GenericRepository<TipGlobalization> TipGlobalizationStore {
            get {
                if ( this._tipGlobalization == null )
                    this._tipGlobalization = new GenericRepository<TipGlobalization>(this._context);

                return this._tipGlobalization;
            }
        }
        private GenericRepository<TipGlobalization> _tipGlobalization = null;
        #endregion
    }
}
