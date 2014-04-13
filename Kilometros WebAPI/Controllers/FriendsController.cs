using Kilometros_WebAPI.Exceptions;
using Kilometros_WebAPI.Helpers;
using Kilometros_WebAPI.Models.ResponseModels;
using Kilometros_WebAPI.Security;
using Kilometros_WebGlobalization.API;
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
    public class FriendsController : IKMSController {
        /// <summary>
        ///     Devuelve la lista de Amigos.
        /// </summary>
        [HttpGet]
        [Route("friends")]
        public IEnumerable<FriendResponse> GetFriendsList() {
            User user
                = OAuth.Token.User;
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
                        = f.Friend.Guid.ToBase64String(),
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
            KMSIdentity identity
                = (KMSIdentity)User.Identity;
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
                                = u.Guid.ToBase64String(),
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
        public HttpResponseMessage RequestFriendship(string userId) {
            User user
                = OAuth.Token.User;
            Guid friendUserGuid
                = new Guid().FromBase64String(userId);

            // --- Validar que el Usuario al que se enviará la solicitud exista ---
            User friendUser
                = Database.UserStore.Get(friendUserGuid);
            if ( friendUser == null )
                throw new HttpNotFoundException(
                    "401 " + ControllerStrings.Warning401_FriendNotFound
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
                    "402 " + ControllerStrings.Warning402_FriendshipAlreadyExists
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
            return new HttpResponseMessage() {
                RequestMessage
                    = Request,

                StatusCode
                    = HttpStatusCode.OK
            };
        }

        /// <summary>
        ///     Acepta la Solicitud de Amistad del Usuario especificado.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("friends/requests/{userId}/accept")]
        public HttpResponseMessage AcceptFriendship(string userId) {
            User user
                = OAuth.Token.User;
            Guid friendUserGuid
                = new Guid().FromBase64String(userId);

            // --- Validar que el Usuario al que se le aceptará la solicitud exista ---
            User friendUser
                = Database.UserStore.Get(friendUserGuid);
            if ( friendUser == null )
                throw new HttpNotFoundException(
                    "401 " + ControllerStrings.Warning401_FriendNotFound
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
                    "403 " + ControllerStrings.Warning403_FriendshipRequestNotFound
                );

            // --- Aceptar la Amistad ---
            friendship.Accepted = true;

            Database.UserFriendStore.Update(friendship);
            Database.SaveChanges();

            // --- Devolver respuesta ---
            return new HttpResponseMessage() {
                RequestMessage
                    = Request,

                StatusCode
                    = HttpStatusCode.OK
            };
        }

        /// <summary>
        ///     Devuelve las Solicitudes de Amistad recibidas por el Usuario.
        /// </summary>
        [HttpGet]
        [Route("friends/requests/received")]
        public IEnumerable<FriendResponse> GetReceivedFriendRequests() {
            User user
                = OAuth.Token.User;
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
                        = f.Friend.Guid.ToBase64String(),
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
            User user
                = OAuth.Token.User;
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
                        = f.Friend.Guid.ToBase64String(),
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
