using Kilometros_WebApp.Models.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Kilometros_WebApp.Controllers {
	/// <summary>
	///     Controlador bbase que contiene propiedades y/o métodos extra para facilitar el
	///     desarrollo de la WebApp.
	/// </summary>
	public abstract class BaseController : Controller {
		/// <summary>
		///     Provee acceso a un Contexto de Base de Datos para el
		///     controlador actual. Los objetos expuestos por Identity
		///     y OAuth NO pueden ser modificados o almacenados en éste
		///     contexto, debe obtenerse un objeto desde ésta instancia.
		/// </summary>
		protected KilometrosDatabase.Abstraction.WorkUnit Database {
			get {
				if ( this._database == null )
					this._database
						= new KilometrosDatabase.Abstraction.WorkUnit();

				return this._database;
			}
		}
		private KilometrosDatabase.Abstraction.WorkUnit _database = null;

		protected KilometrosDatabase.User CurrentUser {
			get {
				if ( this._currentUser != null )
					return this._currentUser;

				if ( ! User.Identity.IsAuthenticated )
					return null;
					
				Guid userGuid;

				try {
					byte[] userGuidBytes
						=  Convert.FromBase64String(User.Identity.Name);
					userGuid
						= new Guid(userGuidBytes);
				} catch {
					FormsAuthentication.SignOut();
					return null;
				}

				this._currentUser
					= Database.UserStore.GetFirst(
						filter:
							f => f.Guid == userGuid,
						include:
							new string[] { "UserDataTotalDistance" }
					);

				if ( this._currentUser == null )
					FormsAuthentication.SignOut();

				return this._currentUser;
			}
		}
		private KilometrosDatabase.User _currentUser = null;

		/// <summary>
		///     Devuelve los Valores que deben utilizarse en el Layout
		///     que se comparte por todos los Controladores y Vistas, principalmente
		///     en la Barra Lateral.
		/// </summary>
		protected LayoutValues LayoutValues {
			get {
				if ( this._layoutValues != null )
					return this._layoutValues;

				if ( CurrentUser == null )
					return null;

				// > Inicializar objeto
				this._layoutValues = new LayoutValues() {
					CultureInfo
						= CultureInfo.CurrentCulture,
					RegionInfo
						= RegionInfo.CurrentRegion,

					UserName
						= CurrentUser.Name,
					UserLastname
						= CurrentUser.LastName,
					UserPicture
						= new Uri(CurrentUser.PictureUri),

					LocationString
						= "{Not Implemented}",

					TotalDistanceCentimeters
						= CurrentUser.UserDataTotalDistance.TotalDistance,
				};

				// > Obtener distancia restante para la Próxima Recompensa del Usuario
				KilometrosDatabase.Reward nextReward
					= Database.RewardStore.GetFirstForRegion(
						regionCode:
							// + Obtener la Recompensa para el Código de Región del Usuario
							CurrentUser.RegionCode,
						filter: f =>
							// + Obtener la Recompensa inmediata siguiente según la Distancia del Usuario
							f.DistanceTrigger > CurrentUser.UserDataTotalDistance.TotalDistance,
						orderBy: o =>
							// + Ordenar las Recompensas según su Distancia de Debloqueo (Descendiente)
							o.OrderByDescending(b => b.DistanceTrigger)
					);

				if ( nextReward == null )
					throw new InvalidOperationException(
						"Current User's [" + CurrentUser.Guid.ToString("N") + "] Next Reward could "
						+ "not be mapped to any Reward because Database is empty, or Regional limitations "
						+ "return no Rewards. Never must a User be mapped to no next Reward."
					);
				else
					this._layoutValues.NextRewardDistanceCentimeters
						= nextReward.DistanceTrigger - CurrentUser.UserDataTotalDistance.TotalDistance;

				// > Obtener Últimas Recompensas del Usuario
				IEnumerable<KilometrosDatabase.UserEarnedReward> earnedRewards
					= Database.UserEarnedRewardStore.GetAll(
						filter: f =>
							// + Recompensas Ganadas por el Usuario
							f.User.Guid == CurrentUser.Guid,
						orderBy: o =>
							// + Ordenar por Fecha de Creación (Descendiente)
							o.OrderByDescending(b => b.CreationDate),
						extra: x =>
							// + Tomar los primeros 5
							x.Take(5),
						include:
							// + También cargar Recompensa y Regalo
							new string[]{"Reward.RewardGift"}
					);

				HashSet<Guid> earnedRewardGiftGuidHashSet
					= new HashSet<Guid>();
				foreach ( KilometrosDatabase.UserEarnedReward earnedReward in earnedRewards ) {
					foreach ( KilometrosDatabase.RewardGift rewardGift in earnedReward.Reward.RewardGift )
						earnedRewardGiftGuidHashSet.Add(rewardGift.Guid);
				}

				Guid[] earnedRewardGiftGuids
					= earnedRewardGiftGuidHashSet.ToArray();

				Guid[] claimedRewardGiftGuids
					= Database.UserRewardGiftClaimedStore.GetAll(
						filter: f =>
							// + Regalos Reclamados que correspondan a los obtenidos anteriormente
							earnedRewardGiftGuids.Contains(f.RewardGift.Reward.Guid),
						orderBy: o =>
							// + Ordenar por Fecha de Creación (Descendiente)
							o.OrderByDescending(b => b.CreationDate)
					).Select( f =>
						// + Obtener sólo el GUID del Regalo Reclamado
						f.RewardGift.Guid
					).ToArray();

				this._layoutValues.LastRewards = (
					from r in earnedRewards
					select new LayoutReward() {
						IncludesGift
							= r.Reward.RewardGift != null,
						IncludedGiftClaimedByUser
							= (
								from g in r.Reward.RewardGift
								where claimedRewardGiftGuids.Contains(g.Guid)
								select g.Guid
							).Count() > 0,

						Title
							= r.Reward.GetGlobalization(this._layoutValues.CultureInfo).Title,
						
						CultureInfo
							= this._layoutValues.CultureInfo,
						RegionInfo
							= this._layoutValues.RegionInfo,

						TriggerDistanceCentimeters
							= r.Reward.DistanceTrigger
					}
				).ToArray();

				// > Obtener Top de Amigos (Marcadores)
				IEnumerable<KilometrosDatabase.User> topFriends
					= Database.UserFriendStore.GetAll(
						filter: f => 
							// + Relaciones que contengan el GUID del Usuario en Amigo o Usuario
							f.Friend.Guid == CurrentUser.Guid
							|| f.User.Guid == CurrentUser.Guid,
						orderBy: o =>
							// + Ordenar por Distancia Total Recorrida (Descendiente)
							o.OrderByDescending(
								b => b.Friend.UserDataTotalDistance.TotalDistance
							).OrderByDescending(
								b => b.User.UserDataTotalDistance.TotalDistance
							),
						include:
							// + También cargar objeto(s) de Usuario y Distancia Total Recorrida
							new string[] { "User.UserDataTotalDistance" }
					).Select( s =>
						// + Obtener sólo el objeto del Usuario (no interesa el objeto de Amistad)
						s.User.Guid == CurrentUser.Guid
						&& s.User.UserDataTotalDistance.TotalDistance > s.Friend.UserDataTotalDistance.TotalDistance
							? s.User
							: s.Friend
					);

				// > Abstraer el Top de Amigos al objeto {LayoutFriend}
				this._layoutValues.TopFriends
					= (
						from f in topFriends
						select new LayoutFriend() {
							CultureInfo
								= this._layoutValues.CultureInfo,
							RegionInfo
								= this._layoutValues.RegionInfo,
					
							Name
								= f.Name,
							LastName
								= f.LastName,
							PictureUri
								= new Uri(f.PictureUri),

							TotalDistanceCentimeters
								= f.UserDataTotalDistance.TotalDistance
						}
					).ToArray();

				// > Devolver valores
				return this._layoutValues;
			}
		}
		private LayoutValues _layoutValues = null;
	}
}