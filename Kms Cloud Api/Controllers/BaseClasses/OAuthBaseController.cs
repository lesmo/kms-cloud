using Kilometros_WebGlobalization.API;
using Kms.Cloud.Api.Exceptions;
using Kms.Cloud.Api.Models;
using Kms.Cloud.Api.Properties;
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
using System.Web.Http;

namespace Kms.Cloud.Api.Controllers {
    public abstract class OAuthBaseController : BaseController {
        protected OAuthCredential OAuth3rdCredential {
            get;
            private set;
        }

        protected T OAuth3rdClient<T>(
            IOAuthTokenPost dataPost
        ) where T : FacebookClient {
            OAuthCryptoSet oAuthConsumer =
                new OAuthCryptoSet(
                    Settings.Default.FacebookClientConsumerKey,
                    Settings.Default.FacebookClientConsumerSecret
                );

            return Activator.CreateInstance(
                typeof(T),
                new object[] {
                    oAuthConsumer,
                    null,
                    null
                }
            ) as T;
        }

        protected T OAuth3rdClient<T>(
            IOAuthTokenSecretPost dataPost
        ) where T : OAuthClient {
            OAuthCryptoSet oAuthConsumer, oAuthToken =
                new OAuthCryptoSet(dataPost.Token, dataPost.TokenSecret);

            if ( typeof(T) == typeof(TwitterClient) ) {
                oAuthConsumer = new OAuthCryptoSet(
                    Settings.Default.TwitterClientConsumerKey,
                    Settings.Default.TwitterClientConsumerSecret
                );
            } else {
                throw new InvalidOperationException("Third Party OAuth Provider not supported");
            }

            return Activator.CreateInstance(
                typeof(T),
                new object[] {
                    oAuthConsumer, 
                    oAuthToken,
                    null
                }
            ) as T;
        }

        protected void ValidateOAuth3rdLoginRequest(OAuthService provider, IOAuthTokenPost dataPost) {
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
        
        protected void ValidateOAuth3rdAddRequest(OAuthService provider, IOAuthTokenPost dataPost) {
            this.OAuth3rdCredential =
                Database.OAuthCredentialStore.GetFirst(
                    filter: f =>
                        f.OAuthProvider == provider
                        && f.Uid == dataPost.ID
                );

            if ( this.OAuth3rdCredential != null || this.OAuth3rdCredential.User.Guid != CurrentUser.Guid ) {
                throw new HttpConflictException(
                    "109 " + ControllerStrings.Warning109_SocialTokenAlreadyInUse
                );
            }
        }

        protected HttpResponseMessage ExchangeOAuthAccessToken(User user = null) {
            Token newToken = new Token {
                ApiKey           = OAuth.ConsumerKey,
                Guid             = Guid.NewGuid(),
                Secret           = Guid.NewGuid(),
                VerificationCode = null,

                User             = user ?? CurrentUser ?? OAuth3rdCredential.User,

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