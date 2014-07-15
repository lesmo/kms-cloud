using Kms.Cloud.Database;
using Kms.Cloud.Database.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kms.Cloud.WebApp.Controllers {
    public partial class AjaxController {
        public JsonResult FriendList(int page = 1, int perPage = 18) {
            // > Validar items por Página
            if ( perPage > 40 )
                throw new HttpException(400, "Friends Per Page is too high");

            if ( page < 1 )
                throw new HttpException(400, "Page number is invalid");
            else
                page--;

            var friends = Database.UserFriendStore.GetAll(
                filter: f =>
                    (
                        f.User.Guid == CurrentUser.Guid
                        || f.Friend.Guid == CurrentUser.Guid
                    ) && f.Accepted == true,
                orderBy: o =>
                    o.OrderByDescending(b => b.CreationDate),
                extra: x =>
                    x.Skip(page * perPage).Take(perPage),
                include:
                    new string[] { "User.UserDataTotalDistance" }
            ).Select(s =>
                // + Obtener sólo el Objeto de Usuario del Amigo, no del Usuario actual
                s.User.Guid == CurrentUser.Guid
                    ? s.Friend.UserDataTotalDistanceSum
                    : s.User.UserDataTotalDistanceSum
            ).Select(s => new {
                userId = s.User.Guid.ToBase64String(),
                pictureUri = s.User.PictureUri,
                name = s.User.Name,
                lastName = s.User.LastName,

                totalDistance = RegionInfo.CurrentRegion.IsMetric
                    ? s.TotalDistance.CentimetersToKilometers()
                    : s.TotalDistance.CentimetersToMiles(),

                totalKcal = s.TotalKcal,
                totalCo2 = s.TotalCo2,
                totalCash = s.TotalCash.DollarCentsToRegionCurrency()
            });

            return Json(friends, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FriendRequests(int page = 1, int perPage = 10) {
            // > Validar items por Página
            if ( perPage > 40 )
                throw new HttpException(400, "Friend Requests Per Page is too high");

            var friendships = Database.UserFriendStore.GetAll(
                filter: f =>
                    f.Friend.Guid == CurrentUser.Guid
                    && f.Accepted == false,
                orderBy: o =>
                    o.OrderByDescending(b => b.CreationDate),
                extra: x =>
                    x.Skip(page * perPage).Take(perPage),
                include:
                    new string[] { "User.UserDataTotalDistance" }
            ).Select(s =>
                new {
                    userId = s.User.Guid.ToBase64String(),
                    pictureUri = s.User.PictureUri,
                    name = s.User.Name,
                    lastName = s.User.LastName,

                    totalDistance
                        = RegionInfo.CurrentRegion.IsMetric
                        ? s.User.UserDataTotalDistanceSum.TotalDistance.CentimetersToKilometers()
                        : s.User.UserDataTotalDistanceSum.TotalDistance.CentimetersToMiles(),

                    totalKcal = s.User.UserDataTotalDistanceSum.TotalKcal,
                    totalCo2 = s.User.UserDataTotalDistanceSum.TotalCo2,
                    totalCash = s.User.UserDataTotalDistanceSum.TotalCash.DollarCentsToRegionCurrency()
                }
            );

            return Json(friendships, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FriendRequestAccept(string friendId) {
            // > Buscar la Amistad
            var friendGuid = new Guid().FromBase64String(friendId);
            var friendship = Database.UserFriendStore.GetFirst(
                filter: f =>
                    f.User.Guid == friendGuid
                    && f.Friend.Guid == CurrentUser.Guid
                    && f.Accepted == false
            );

            if ( friendship == null )
                throw new HttpException(404, "Friendship not found");

            // > Aceptar la Amistad
            friendship.Accepted = true;

            Database.UserFriendStore.Update(friendship);
            Database.SaveChanges();

            // > Devolver respuesta OK
            return Json(new {
                ok = true
            });
        }

        public JsonResult FriendRequestReject(string friendId) {
            // > Buscar la Amistad
            var friendGuid = new Guid().FromBase64String(friendId);
            var friendship = Database.UserFriendStore.GetFirst(
                filter: f =>
                    f.User.Guid == friendGuid
                    && f.Friend.Guid == CurrentUser.Guid
                    && f.Accepted == false
            );

            if ( friendship == null )
                throw new HttpException(404, "Friendship not found");

            // > Eliminar la solicitud de Amistad
            Database.UserFriendStore.Delete(friendship);
            Database.SaveChanges();

            // > Devolver respuesta OK
            return Json(new {
                ok = true
            });
        }
    }
}