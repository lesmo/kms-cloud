using Kms.Cloud.WebApp.Models.Views;
using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Kms.Cloud.WebApp.Controllers {
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
		internal Kms.Cloud.Database.Abstraction.WorkUnit Database {
			get {
				if ( this._database == null )
					this._database
						= new Kms.Cloud.Database.Abstraction.WorkUnit();

				return this._database;
			}
		}
		private Kms.Cloud.Database.Abstraction.WorkUnit _database = null;
	}
}