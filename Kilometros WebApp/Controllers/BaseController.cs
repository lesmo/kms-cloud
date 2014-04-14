using Kilometros_WebApp.Models.Views;
using KilometrosDatabase;
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

		protected User CurrentUser {
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
		private User _currentUser = null;

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
						= CurrentUser.UserDataTotalDistanceSum,
				};

				// > Obtener la última recompensa obtenida por el Usuario
				UserEarnedReward lastReward
					= Database.UserEarnedRewardStore.GetFirst(
						filter: f =>
							f.User.Guid == CurrentUser.Guid,
						orderBy: o =>
							o.OrderByDescending(b => b.CreationDate)
					);

				if ( lastReward.Discarded ) {
					// > Obtener distancia restante para la Próxima Recompensa del Usuario
					Reward nextReward
						= Database.RewardStore.GetFirstForRegion(
							regionCode:
						// + Obtener la Recompensa para el Código de Región del Usuario
								CurrentUser.RegionCode,
							filter: f =>
								// + Obtener la Recompensa inmediata siguiente según la Distancia del Usuario
								f.DistanceTrigger > CurrentUser.UserDataTotalDistanceSum,
							orderBy: o =>
								// + Ordenar las Recompensas según su Distancia de Debloqueo (Descendiente)
								o.OrderByDescending(b => b.DistanceTrigger)
						);

					if ( nextReward == null )
						throw new InvalidOperationException(
							"Current User's [" + CurrentUser.Guid.ToString("N") + "] Next Reward could"
							+ " not be mapped to any Reward because Database is empty, or Regional limitations"
							+ " return no Rewards. Never must a User be mapped to no next Reward."
						);
					else
						this._layoutValues.NextRewardDistanceCentimeters
							= nextReward.DistanceTrigger - CurrentUser.UserDataTotalDistanceSum;
				} else {
					this._layoutValues.RecentlyUnlockedRewardGuid
						= lastReward.Guid.ToBase64String();
				}

				// > Obtener el Tip del Día
				Tip tipOfTheDay
					= Database.UserTipHistoryStore.GetFirst(
						filter: f =>
							f.User.Guid == CurrentUser.Guid,
						orderBy: o =>
							o.OrderByDescending(b => b.CreationDate),
						include:
							new string[] { "Tip.TipCategory" }
					).Tip;

				this._layoutValues.TipOfTheDay
					= new TipModel() {
						TipId
							= tipOfTheDay.Guid.ToBase64String(),
						Text
							= tipOfTheDay.GetGlobalization(
								this._layoutValues.CultureInfo
							).Text,
						Category
							= tipOfTheDay.TipCategory.GetGlobalization<TipCategoryGlobalization>(
								this._layoutValues.CultureInfo
							).Name,
						IconUri
							= GetDynamicResourceUri(
								method:
									"Images",
								filename:
									tipOfTheDay.TipCategory.Guid.ToBase64String(),
								ext:
									tipOfTheDay.TipCategory.PictureExtension
							)
					};

				// > Devolver valores
				return this._layoutValues;
			}
		}
		private LayoutValues _layoutValues = null;

		/// <summary>
		///     Devuelve una Uri absoluta que apunta al Recurso generado dinámicamente especificado
		///     por el nombre del archivo (normalmente el GUID del recurso en BD) y su extensión.
		/// </summary>
		/// <param name="method">
		///     Método en controlador DynamicResources responsable de generar el recurso.
		/// </param>
		/// <param name="filename">
		///     Nombre del archivo (normalmente el GUID del recurso en BD).
		/// </param>
		/// <param name="ext">
		///     Extensión esperada por Método.
		/// </param>
		/// <returns>
		///     URI absoluta que apunta al recurso descrito por los parámetros.
		/// </returns>
		protected Uri GetDynamicResourceUri(string method, string filename, string ext) {
			return new Uri(
				Url.Content(
					string.Format(
						"~/{0}/{1}.{2}",
						"DynamicResources/Images",
						filename,
						ext
					)
				)
			);
		}
	}
}