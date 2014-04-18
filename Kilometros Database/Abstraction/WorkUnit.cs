using KilometrosDatabase.Abstraction.Functional;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.Abstraction {
    public sealed class WorkUnit : IDisposable {
        private MainModelContainer _context = new MainModelContainer();

        // FUCKING HACK
        private static Type type = typeof(System.Data.Entity.SqlServer.SqlProviderServices);

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

        /// <summary>
        /// Almacén de Nonce de OAuth
        /// </summary>
        public GenericRepository<OAuthNonce> OAuthNonceStore {
            get {
                if ( this._oAuthNonceRepository == null )
                    this._oAuthNonceRepository = new GenericRepository<OAuthNonce>(this._context);

                return this._oAuthNonceRepository;
            }
        }
        private GenericRepository<OAuthNonce> _oAuthNonceRepository = null;
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

        /// <summary>
        /// Almacén de Amigos de los Usuarios
        /// </summary>
        public GenericRepository<UserFriend> UserFriendStore {
            get {
                if ( this._userFriendRepository == null )
                    this._userFriendRepository = new GenericRepository<UserFriend>(this._context);

                return this._userFriendRepository;
            }
        }
        private GenericRepository<UserFriend> _userFriendRepository = null;

        /// <summary>
        /// Almacén de Credenciales OAuth the terceros
        /// </summary>
        public GenericRepository<OAuthCredential> OAuthCredentialStore {
            get {
                if ( this._oAuthCredentialRepository == null )
                    this._oAuthCredentialRepository = new GenericRepository<OAuthCredential>(this._context);

                return this._oAuthCredentialRepository;
            }
        }
        private GenericRepository<OAuthCredential> _oAuthCredentialRepository = null;
        #endregion

        #region User Data Storage
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

        /// <summary>
        /// Almacén de Historial de Tips de Usuario
        /// </summary>
        public GenericRepository<UserTipHistory> UserTipHistoryStore {
            get {
                if ( this._userTipHistoryStore == null )
                    this._userTipHistoryStore = new GenericRepository<UserTipHistory>(this._context);

                return this._userTipHistoryStore;
            }
        }
        private GenericRepository<UserTipHistory> _userTipHistoryStore = null;
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
        public RewardRepository RewardStore {
            get {
                if ( this._rewardRepository == null )
                    this._rewardRepository = new RewardRepository(this._context);

                return this._rewardRepository;
            }
        }
        private RewardRepository _rewardRepository = null;

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
        public GenericRepository<RewardGiftPicture> RewardPictureStore {
            get {
                if ( this._rewardGiftPictureRepository == null )
                    this._rewardGiftPictureRepository = new GenericRepository<RewardGiftPicture>(this._context);

                return this._rewardGiftPictureRepository;
            }
        }
        private GenericRepository<RewardGiftPicture> _rewardGiftPictureRepository = null;

        /// <summary>
        /// Almacén de asociación entre Regalos y Usuario que han reclamado esos regalos
        /// </summary>
        public GenericRepository<UserRewardGiftClaimed> UserRewardGiftClaimedStore {
            get {
                if ( this._userRewardGiftClaimedRepository == null )
                    this._userRewardGiftClaimedRepository = new GenericRepository<UserRewardGiftClaimed>(this._context);

                return this._userRewardGiftClaimedRepository;
            }
        }
        private GenericRepository<UserRewardGiftClaimed> _userRewardGiftClaimedRepository = null;
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
        public GenericRepository<TipCategoryGlobalization> TipCategoryGlobalizationStore {
            get {
                if ( this._tipCategoryGlobalizationStore == null )
                    this._tipCategoryGlobalizationStore = new GenericRepository<TipCategoryGlobalization>(this._context);

                return this._tipCategoryGlobalizationStore;
            }
        }
        private GenericRepository<TipCategoryGlobalization> _tipCategoryGlobalizationStore = null;

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

        #region Data Views
        /// <summary>
        /// Almacén de Distancia por Hora
        /// </summary>
        public GenericRepository<UserDataHourlyDistance> UserDataHourlyDistance {
            get {
                if ( this._userDataHourlyDistance == null )
                    this._userDataHourlyDistance = new GenericRepository<UserDataHourlyDistance>(this._context);

                return this._userDataHourlyDistance;
            }
        }
        private GenericRepository<UserDataHourlyDistance> _userDataHourlyDistance = null;

        /// <summary>
        /// Almacén de Distancia Total
        /// </summary>
        public GenericRepository<UserDataTotalDistance> UserDataTotalDistance {
            get {
                if ( this._userDataTotalDistance == null )
                    this._userDataTotalDistance = new GenericRepository<UserDataTotalDistance>(this._context);

                return this._userDataTotalDistance;
            }
        }
        private GenericRepository<UserDataTotalDistance> _userDataTotalDistance = null;
        #endregion

        /// <summary>
        ///     Acceso directo al almacén de imágenes
        /// </summary>
        public GenericRepository<IPicture> IPictureStore {
            get {
                if ( this._iPicture == null )
                    this._iPicture = new GenericRepository<IPicture>(this._context);

                return this._iPicture;
            }
        }
        private GenericRepository<IPicture> _iPicture = null;

        /// <summary>
        ///     Almacén de Notificaciones
        /// </summary>
        public GenericRepository<Notification> NotificationStore {
            get {
                if ( this._notification == null )
                    this._notification = new GenericRepository<Notification>(this._context);

                return this._notification;
            }
        }
        private GenericRepository<Notification> _notification = null;
    }
}
