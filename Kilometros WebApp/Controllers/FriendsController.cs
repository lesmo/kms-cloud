using KilometrosDatabase;
using Kilometros_WebApp.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace Kilometros_WebApp.Controllers {
	public class FriendsController : BaseController {
		// GET: /Friends/
		public ActionResult Index() {
			// > Inicializar valores de Amigos
			FriendsValues friendValues
				= new FriendsValues();

			// > Obtener Solicitudes de Amistad
			friendValues.FriendRequests
				= Database.UserFriendStore.GetAll(
					filter: f =>
						f.Friend.Guid == CurrentUser.Guid
						&& f.Accepted == false,
					orderBy: o =>
						o.OrderByDescending(b => b.CreationDate),
					extra: x =>
						x.Take(5),
					include:
						new string[] { "User" }
				).Select(s =>
					new FriendModel(s.User)
				).ToArray();

			// > Obtener Amigos
			friendValues.Friends
				= Database.UserFriendStore.GetAll(
					filter: f =>
						(
							f.User.Guid == CurrentUser.Guid
							|| f.Friend.Guid == CurrentUser.Guid
						) && f.Accepted == true,
					orderBy: o =>
						o.OrderByDescending(b => b.CreationDate),
					extra: x =>
						x.Take(18),
					include:
						new string[] { "User" }
				).Select(s =>
					// + Obtener sólo el Objeto de Usuario del Amigo, no del Usuario actual
					s.User.Guid == CurrentUser.Guid
						? s.Friend
						: s.User
				).Select(s =>
					new FriendModel(s)
				).ToArray();

			// > Establecer valores para la Vista
			ViewData.Add(
				"LayoutValues",
				this.LayoutValues
			);
			ViewData.Add(
				"FriendsValues",
				friendValues
			);

			// > Mostrar la vista
			return View();
		}
	}
}