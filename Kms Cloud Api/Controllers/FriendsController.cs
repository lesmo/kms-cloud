using Kms.Cloud.Api.Exceptions;
using Kms.Cloud.Api.Models.ResponseModels;
using Kms.Cloud.Api.Security;
using Kilometros_WebGlobalization.API;
using Kms.Cloud.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics.CodeAnalysis;

namespace Kms.Cloud.Api.Controllers {
    /// <summary>
    ///     Permite interactuar con la lista de Amigos del Usuario.
    /// </summary>
    [Authorize]
    public class FriendsController : BaseController {
        /// <summary>
        ///     Devuelve la lista de Amigos.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [HttpGet, Route("friends")]
        public IEnumerable<FriendResponse> GetFriendsList() {
            return Database.UserFriendStore.GetAll(
                filter:  f =>
                    // + Obtener Amistades donde el Usuario sea partícipe
                    (
                        f.User.Guid == CurrentUser.Guid
                        || f.User.Guid == CurrentUser.Guid
                    ) && f.Accepted == true,
                include:
                    // + Incluir el objeto de Usuario del Amigo, y el Usuario
                    new string[] { "Friend", "User" }
            ).Select(s =>
                // + Obtener únicamente el objeto de Usuario del Amigo, no del Usuario
                s.User.Guid == CurrentUser.Guid
                    ? s.Friend
                    : s.User
            ).Select(f =>
                // + Transformar el objeto de Usuario a Modelo {FriendResponse}
                new FriendResponse {
                    UserId
                        = f.Guid.ToBase64String(),
                    Name
                        = f.Name,
                    LastName
                        = f.LastName,
                    PictureUri
                        = new Uri(f.PictureUri)
                }
            );
        }

        /// <summary>
        ///      Devuelve la lista de marcadores de los Amigos. La lista incluye al Usuario, y está
        ///      ordenada del más alto al más bajo.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [HttpGet, Route("friends/highscores")]
        public IEnumerable<FriendScoreResponse> GetFriendsHighScores() {
            // > Obtener 
            List<User> userFriends
                = Database.UserFriendStore.GetAll(
                    filter: f =>
                        // + Obtener Amistades que sean Aceptadas e involucren al Usuario actual
                        (
                            f.User.Guid == CurrentUser.Guid
                            || f.User.Guid == CurrentUser.Guid
                        ) && f.Accepted == true,
                    include:
                        // + Incluir objetos de Distancia Total para el Amigo y el Usuario
                        new string[] { "User.UserDataTotalDistance", "Friend.UserDataTotalDistance" }
                ).Select(s =>
                    // + Obtener únicamente el objeto de Usuario del Amigo, no del Usuario
                    s.User.Guid == CurrentUser.Guid
                        ? s.Friend
                        : s.User
                ).ToList();

            // > Añadir el objeto del Usuario actual
            userFriends.Add(CurrentUser);

            // > Devolver respuesta
            return userFriends.OrderByDescending(b =>
                b.UserDataTotalDistanceSum.TotalDistance
            ).Select(s =>
                new FriendScoreResponse {
                    Friend
                        = new FriendResponse {
                            UserId
                                = s.Guid.ToBase64String(),
                            CreationDate
                                = s.CreationDate,
                            Name
                                = s.Name,
                            LastName
                                = s.LastName,
                            PictureUri
                                = new Uri(s.PictureUri)
                        },
                    TotalDistance
                        = s.UserDataTotalDistanceSum.TotalDistance,
                    IsMe
                        = s.Guid == CurrentUser.Guid
                }
            );
        }

        /// <summary>
        ///     Envia una Solicitud de Amistad al Usuario especificado.
        /// </summary>
        [HttpPost]
        [Route("friends/requests/{userId}")]
        public HttpResponseMessage RequestFriendship(string userId) {
            Guid friendUserGuid
                = new Guid().FromBase64String(userId);

            // --- Validar que el Usuario al que se enviará la solicitud exista ---
            User friendUser
                = Database.UserStore[friendUserGuid];
            if ( friendUser == null )
                throw new HttpNotFoundException(
                    "401 " + ControllerStrings.Warning401_FriendNotFound
                );

            // --- Validar que no exista la Amistad ---
            bool alreadyFriends
                = Database.UserFriendStore.GetFirst(
                    filter: f =>
                        // + Obtener Amistad donde el Usuario y el Amigo sean partícipes
                        (
                            f.User.Guid == CurrentUser.Guid
                            && f.Friend.Guid == friendUserGuid
                        ) || (
                            f.Friend.Guid == CurrentUser.Guid
                            && f.User.Guid == friendUserGuid
                        )
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
                        = CurrentUser,
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
            Guid friendUserGuid
                = new Guid().FromBase64String(userId);

            // --- Validar que el Usuario al que se le aceptará la solicitud exista ---
            User friendUser
                = Database.UserStore[friendUserGuid];
            if ( friendUser == null )
                throw new HttpNotFoundException(
                    "401 " + ControllerStrings.Warning401_FriendNotFound
                );

            // --- Validar que la Solicitud de Amistad exista ---
            UserFriend friendship
                = Database.UserFriendStore.GetFirst(
                    filter: f =>
                        // + Obtener la Amistad en la que el Usuario actual está
                        //   descrito como el Amigo, y el Amigo está descrito como
                        //   el Usuario.
                        f.User.Guid == friendUser.Guid
                        && f.Friend.Guid == CurrentUser.Guid
                        && f.Accepted == false
                );

            if ( friendship == null )
                throw new HttpNotFoundException(
                    "403 " + ControllerStrings.Warning403_FriendshipRequestNotFound
                );

            // --- Aceptar la Amistad ---
            friendship.Accepted
                = true;

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
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [HttpGet, Route("friends/requests/received")]
        public IEnumerable<FriendResponse> GetReceivedFriendRequests() {
            return Database.UserFriendStore.GetAll(
                filter: f =>
                    // + Obtener Amistades donde el Usuario está descrito
                    //   como el Amigo, y la Amistad no ha sido aceptada.
                    f.Friend.Guid == CurrentUser.Guid
                    && f.Accepted == false,
                orderBy: o =>
                    o.OrderByDescending(b => b.CreationDate),
                include:
                    // + Incluir al Usuario y al Amigo
                    new string[] { "User" }
            ).Select(s =>
                // + Transformar el objeto de Amistad a objeto de Usuario a
                //   partir de la propiedad de Usuario
                new FriendResponse {
                    UserId
                        = s.User.Guid.ToBase64String(),
                    CreationDate
                        = s.CreationDate,
                    Name
                        = s.User.Name,
                    LastName
                        = s.User.LastName,
                    PictureUri
                        = new Uri(s.User.PictureUri)
                }
            );
        }

        /// <summary>
        ///     Devuelve las Solicitudes de Amistad enviadas por el Usuario.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [HttpGet, Route("friends/requests/sent")]
        public IEnumerable<FriendResponse> GetSentFriendRequests() {
            return Database.UserFriendStore.GetAll(
                filter: f =>
                    // + Obtener Amistades donde el Usuario está descrito
                    //   como el Usuario, y la Amistad no ha sido aceptada.
                    f.User.Guid == CurrentUser.Guid
                    && f.Accepted == false,
                orderBy: o =>
                    o.OrderByDescending(b => b.CreationDate),
                include:
                    // + Incluir al Usuario y al Amigo
                    new string[] { "Friend" }
            ).Select(s =>
                // + Transformar el objeto de Amistad a objeto de Usuario a partir
                //   de la propiedad de Amigo
                new FriendResponse() {
                    UserId
                        = s.Friend.Guid.ToBase64String(),
                    CreationDate
                        = s.CreationDate,
                    Name
                        = s.Friend.Name,
                    LastName
                        = s.Friend.LastName,
                    PictureUri
                        = new Uri(s.Friend.PictureUri)
                }
            );
        }
    }
}
