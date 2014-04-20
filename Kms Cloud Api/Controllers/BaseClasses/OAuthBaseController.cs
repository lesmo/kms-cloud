using Kilometros_WebGlobalization.API;
using Kms.Cloud.Api.Exceptions;
using Kms.Cloud.Api.Models;
using Kms.Cloud.Database;
using Kms.Interop.OAuth;
using Kms.Interop.OAuth.SocialClients;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Kms.Cloud.Api.Controllers {
    public abstract class OAuthBaseController : BaseController {
        protected OAuthCredential OAuth3rdCredential {
            get;
            private set;
        }

        protected T OAuth3rdClient<T>(
            IOAuthTokenPost dataPost,
            OAuthCryptoSet oAuthConsumer
        ) where T : FacebookClient {
            ValidateOAuthLoginRequest(OAuthService.Facebook, dataPost);

            return Activator.CreateInstance(
                typeof(T),
                new object[] {
                    oAuthConsumer
                }
            ) as T;
        }

        protected T OAuth3rdClient<T>(
            IOAuthTokenSecretPost dataPost,
            OAuthCryptoSet oAuthConsumer
        ) where T : OAuthClient {
            if ( typeof(T) == typeof(TwitterClient) )
                ValidateOAuthLoginRequest(OAuthService.Twitter, dataPost);
            else
                throw new InvalidOperationException("Third Party OAuth Provider not supported");

            return Activator.CreateInstance(
                typeof(T),
                new object[] {
                    oAuthConsumer, 
                    new OAuthCryptoSet(dataPost.Token, dataPost.TokenSecret)
                }
            ) as T;
        }

        protected void ValidateOAuthLoginRequest(OAuthService provider, IOAuthTokenPost dataPost) {
            if ( !OAuth.IsRequestToken )
                throw new HttpBadRequestException(
                    "106 " + ControllerStrings.Warning106_RequestTokenRequired
                );

            this.OAuth3rdCredential =
                Database.OAuthCredentialStore.GetFirst(
                    filter: f =>
                        f.OAuthProvider == provider
                        && f.Uid == dataPost.ID,
                    include:
                        new string[] { "User" }
                );

            if ( this.OAuth3rdCredential == null ) {
                OAuth.Token.LoginAttempts++;
                Database.TokenStore.Update(OAuth.Token);
                Database.SaveChanges();

                throw new HttpBadRequestException(
                    "105 " + ControllerStrings.Warning105_SocialTokenNotFound
                );
            }
        }

        protected HttpResponseMessage ExchangeOAuthAccessToken(User user = null) {
            if ( ! OAuth.IsRequestToken )    
                throw new InvalidOperationException(
                    "Can't return new Access Token when current Request is being served to an Access Token."
                );

            Token newToken = new Token {
                ApiKey           = OAuth.ConsumerKey,
                Guid             = Guid.NewGuid(),
                Secret           = Guid.NewGuid(),
                VerificationCode = null,

                User             = user ?? OAuth3rdCredential.User,

                ExpirationDate   = DateTime.UtcNow.AddMonths(3),
                LoginAttempts    = OAuth.Token.LoginAttempts
            };

            Database.TokenStore.Add(newToken);
            Database.TokenStore.Delete(OAuth.Token);
            Database.SaveChanges();

            return new HttpResponseMessage {
                RequestMessage = Request,
                StatusCode     = HttpStatusCode.OK,

                Content = new StringContent(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "oauth_token={0}&oauth_token_secret={1}",
                        newToken.Guid.ToString("N"),
                        newToken.Secret.ToString("N")
                    )
                )
            };
        }
    }
}