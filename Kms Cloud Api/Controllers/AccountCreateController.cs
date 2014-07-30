using Kms.Cloud.Api.Exceptions;
using Kms.Cloud.Api.Models.RequestModels;
using Kms.Cloud.Api.Security;
using Kilometros_WebGlobalization.API;
using Kms.Cloud.Database;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using Kms.Cloud.Api.MagicTriggers;

namespace Kms.Cloud.Api.Controllers {
    /// <summary>
    ///     Permite generar una nueva Cuenta en la Nube de KMS. Para crear una nueva Cuenta
    ///     que permitirá login con Facebook, Twitter, Fitbit o Nike+, será necesario crear
    ///     una Cuenta en éste recurso y después utilizar el apropiado dentro del Controlador
    ///     OAuth3rdPartyAdd. Los Usuarios creados deberán asociar un Número de Serie de
    ///     Dispositivo KMS antes de poder sincronizar información.
    /// </summary>
    public class AccountCreateController : BaseController {
        /// <summary>
        ///     Crear una nueva Cuenta en la Nube KMS.
        /// </summary>
        /// <param name="dataPost">
        ///     Información de la nueva cuenta de Usuario.
        /// </param>
        [AllowAnonymous]
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        [HttpPost, Route("account")]
        public HttpResponseMessage CreateKmsAccount([FromBody]CreateKmsAccountPost dataPost) {
            // --- Validar que API-Key tenga autorización de crear cuentas --
            if ( ! OAuth.ConsumerKey.AccuntCreateEnabled )
                throw new HttpUnauthorizedException(
                    "107 " + ControllerStrings.Warning107_ConsumerNotAllowed
                );

            // --- Validar que se tenga un Request Token ---
            if ( ! OAuth.IsRequestToken )
                throw new HttpUnauthorizedException(
                    "104 " + ControllerStrings.Warning104_RequestTokenInvalid
                );

            // --- Validar que no haya un usuario con el mismo Email ---
            User userSearch
                = Database.UserStore.GetFirst(
                    f => f.Email == dataPost.Email.ToLower()
                );

            if ( userSearch != null )
                throw new HttpConflictException(
                    "206 " + ControllerStrings.Warning206_CannotCreateUserWithEmail
                );

            // --- Crear cuenta de Usuario ---
            User user
                = new User() {
                    Name
                        = dataPost.Name,
                    LastName
                        = dataPost.LastName,
                    BirthDate
                        = dataPost.Birthdate,

                    Email
                        = dataPost.Email.ToLower(CultureInfo.InvariantCulture),
                    PasswordString
                        = dataPost.Password,

                    PreferredCultureCode
                        = dataPost.CultureCode,
                    RegionCode
                        = dataPost.RegionCode,
                    UtcOffset
                        = dataPost.UtcOffset
                };

            // --- Crear perfil Físico del Usuario ---
            UserBody userBody
                = new UserBody() {
                    Height
                        = dataPost.Height,
                    Weight
                        = dataPost.Weight,
                    Sex
                        = dataPost.Gender.ToString().ToUpper(CultureInfo.InvariantCulture),
                    User
                        = user
                };

            // Calcular zancada a partir del género y altura
            if ( userBody.Sex == "F" ) {
                userBody.StrideLengthWalking
                    = (int)(userBody.Height * 0.413);//* 2; stride != step length
                userBody.StrideLengthRunning
                    = (int)(userBody.Height * 1.13);
            } else if ( userBody.Sex == "M" ) {
                userBody.StrideLengthRunning
                    = (int)(userBody.Height * 0.415);//* 2;
                userBody.StrideLengthRunning
                    = (int)(userBody.Height * 1.15);
            }

            // --- Crear un Data Point en 0 ---
            Data data
                = new Data {
                    Timestamp = DateTime.UtcNow,
                    Steps = 0,
                    Activity = 0,
                    StrideLength = 0,
                    EqualsKcal = 0,
                    EqualsCo2 = 0,
                    EqualsCash = 0,
                    User = user
                };

            // --- Crear MotionLevelHistory ---
            MotionLevel motionLevel
                = Database.MotionLevelStore.GetFirst(
                    orderBy: o =>
                        o.OrderBy(b => b.DistanceThresholdStart)
                );

            UserMotionLevelHistory motionLevelHistory
                = new UserMotionLevelHistory {
                    User = user,
                    MotionLevel = motionLevel
                };

            // --- Crear Token OAuth ---
            Token token
                = new Token() {
                    ApiKey = OAuth.ConsumerKey,
                    Guid   = Guid.NewGuid(),
                    Secret = Guid.NewGuid(),
                    User   = user,

                    ExpirationDate = DateTime.UtcNow.AddMonths(3)
                };

            // --- Integrar cambios a BD ---
            Database.UserStore.Add(user);
            Database.UserBodyStore.Add(userBody);
            Database.UserMotionLevelStore.Add(motionLevelHistory);
            Database.DataStore.Add(data);
            Database.TokenStore.Add(token);
            Database.TokenStore.Delete(OAuth.Token);

            var triggers = new RewardTipTrigger(user, Database);
            triggers.TriggerRewardsByDistance();
            triggers.TriggerTipsByDays();

            Database.SaveChanges();

            // --- Devolver respuesta ---
            HttpResponseMessage response
                = new HttpResponseMessage() {
                    RequestMessage
                        = Request,

                    StatusCode
                        = HttpStatusCode.OK,
                    Content
                        = new StringContent(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                "oauth_token={0}&oauth_token_secret={1}&x_expiration_date={2}",
                                token.Guid.ToString("N"),
                                token.Secret.ToString("N"),
                                token.ExpirationDate.ToString()
                            )
                        )
                };

            response.Headers.TryAddWithoutValidation(
                "Warning",
                "000 " + ControllerStrings.Warning108_RequestTokenExchanged
            );

            return response;
        }
    }
}