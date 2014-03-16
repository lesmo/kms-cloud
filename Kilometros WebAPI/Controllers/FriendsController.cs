using Kilometros_WebAPI.Exceptions;
using Kilometros_WebAPI.Helpers;
using Kilometros_WebAPI.Models.ResponseModels;
using Kilometros_WebAPI.Security;
using KilometrosDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kilometros_WebAPI.Controllers {
    /// <summary>
    ///     Permite interactuar con la lista de Amigos del Usuario.
    /// </summary>
    [Authorize]
    public class FriendsController : ApiController {
        /// <summary>
        ///     Acceso a los Repositorios de la BD.
        /// </summary>
        public KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();

        /// <summary>
        ///     Devuelve la lista de Amigos.
        /// </summary>
        [HttpGet]
        [Route("friends")]
        public IEnumerable<FriendResponse> GetFriendsList() {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;
            IEnumerable<UserFriend> userFriends
                = Database.UserFriendStore.GetAll(
                    f =>
                        f.User.Guid == user.Guid
                        && f.Accepted == true
                );
            
            return (
                from f in userFriends
                select new FriendResponse() {
                    UserId
                        = MiscHelper.Base64FromGuid(f.Friend.Guid),
                    Name
                        = f.Friend.Name,
                    LastName
                        = f.Friend.LastName,
                    PictureUri
                        = f.Friend.PictureUri
                }
            );
        }

        /// <summary>
        ///      Devuelve la lista de marcadores de los Amigos. La lista incluye al Usuario, y está
        ///      ordenada del más alto al más bajo.
        /// </summary>
        [HttpGet]
        [Route("friends/highscores")]
        public IEnumerable<FriendScoreResponse> GetFriendsHighscores() {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;

            List<User> userFriends
                = Database.UserFriendStore.GetAll(
                    f =>
                        f.User.Guid == user.Guid
                        && f.Accepted == true,
                    null,
                    new string[] {"UserDataTotalDistance"}
                ).Select<UserFriend, User>(
                    u =>
                        u.User
                ).ToList();
            userFriends.Add(user);
            
            return (
                from u in userFriends
                select new FriendScoreResponse() {
                    Friend
                        = new FriendResponse() {
                            UserId
                                = MiscHelper.Base64FromGuid(u.Guid),
                            CreationDate
                                = DateTime.UtcNow,
                            Name
                                = u.Name,
                            LastName
                                = u.LastName,
                            PictureUri
                                = u.PictureUri
                        },
                    TotalDistance
                        = u.UserDataTotalDistance.TotalDistance,
                    IsMe
                        = u.Guid == user.Guid
                }
            ).OrderByDescending(b => b.TotalDistance);
        }

        /// <summary>
        ///     Envia una Solicitud de Amistad al Usuario especificado.
        /// </summary>
        [HttpPost]
        [Route("friends/requests/{userId}")]
        public IHttpActionResult RequestFriendship(string userId) {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;
            Guid friendUserGuid
                = MiscHelper.GuidFromBase64(userId).Value;

            // --- Validar que el Usuario al que se enviará la solicitud exista ---
            User friendUser
                = Database.UserStore.Get(friendUserGuid);
            if ( friendUser == null )
                throw new HttpNotFoundException(
                    ""
                );

            // --- Validar que no exista la Amistad ---
            bool alreadyFriends
                = Database.UserFriendStore.GetFirst(
                    f =>
                        (f.User.Guid == user.Guid && f.Friend.Guid == friendUserGuid)
                        || (f.Friend.Guid == user.Guid && f.User.Guid == friendUserGuid)
                ) != null;

            if ( alreadyFriends )
                throw new HttpConflictException(
                    "401 " + ""
                );

            // --- Crear la solicitud de Amistad ---
            UserFriend friendship
                = new UserFriend() {
                    Id
                        = Guid.NewGuid(),
                    User
                        = user,
                    Friend
                        = friendUser
                };
            Database.UserFriendStore.Add(friendship);
            Database.SaveChanges();

            // --- Devolver respuesta ---
            return Ok();
        }

        /// <summary>
        ///     Acepta la Solicitud de Amistad del Usuario especificado.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("friends/requests/{userId}/accept")]
        public IHttpActionResult AcceptFriendship(string userId) {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;
            Guid friendUserGuid
                = MiscHelper.GuidFromBase64(userId).Value;

            // --- Validar que el Usuario al que se le aceptará la solicitud exista ---
            User friendUser
                = Database.UserStore.Get(friendUserGuid);
            if ( friendUser == null )
                throw new HttpNotFoundException(
                    ""
                );

            // --- Validar que la Solicitud de Amistad exista ---
            UserFriend friendship
                = Database.UserFriendStore.GetFirst(
                    f =>
                        f.User.Guid == user.Guid
                        && f.Friend.Guid == friendUser.Guid
                        && f.Accepted == false
                );
            if ( friendship == null )
                throw new HttpNotFoundException(
                    ""
                );

            // --- Aceptar la Amistad ---
            friendship.Accepted = true;

            Database.UserFriendStore.Update(friendship);
            Database.SaveChanges();

            // --- Devolver respuesta ---
            return Ok();
        }

        /// <summary>
        ///     Devuelve las Solicitudes de Amistad recibidas por el Usuario.
        /// </summary>
        [HttpGet]
        [Route("friends/requests/received")]
        public IEnumerable<FriendResponse> GetReceivedFriendRequests() {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;
            IEnumerable<UserFriend> userFriends
                = Database.UserFriendStore.GetAll(
                    f =>
                        f.Friend.Guid == user.Guid
                        && f.Accepted == false,
                    null,
                    new string[] { "User", "Friend" }
                );

            return (
                from f in userFriends
                select new FriendResponse() {
                    UserId
                        = MiscHelper.Base64FromGuid(f.Friend.Guid),
                    CreationDate
                        = f.CreationDate,
                    Name
                        = f.Friend.Name,
                    LastName
                        = f.Friend.LastName,
                    PictureUri
                        = f.Friend.PictureUri
                }
            );
        }

        /// <summary>
        ///     Devuelve las Solicitudes de Amistad recibidas por el Usuario.
        /// </summary>
        [HttpGet]
        [Route("friends/requests/sent")]
        public IEnumerable<FriendResponse> GetSentFriendRequests() {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;
            IEnumerable<UserFriend> userFriends
                = Database.UserFriendStore.GetAll(
                    f =>
                        f.User.Guid == user.Guid
                        && f.Accepted == false,
                    null,
                    new string[] { "User", "Friend" }
                );

            return (
                from f in userFriends
                select new FriendResponse() {
                    UserId
                        = MiscHelper.Base64FromGuid(f.Friend.Guid),
                    CreationDate
                        = f.CreationDate,
                    Name
                        = f.Friend.Name,
                    LastName
                        = f.Friend.LastName,
                    PictureUri
                        = f.Friend.PictureUri
                }
            );
        }
    }
}
