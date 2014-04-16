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
	public abstract partial class BaseController : Controller {
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
	}
}