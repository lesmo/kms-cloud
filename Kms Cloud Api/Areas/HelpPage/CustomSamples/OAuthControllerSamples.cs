using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Kms.Cloud.Api.Areas.HelpPage.CustomSamples {
    public static class OAuthControllerSamples {
        public static void Register(Dictionary<Type, object> typeSamples, HttpConfiguration config) {
            // -- OAuth Request Token --
            // Request
            config.SetSampleRequest(
                string.Format(
                    CultureInfo.InvariantCulture,

                    "Authorization: OAuth\n"
                    + "  oauth_consumer_key=\"{0}\"\n"
                    + "  oauth_signature=\"{1}\"\n"
                    + "  oauth_signature_method=\"{2}\"\n"
                    + "  oauth_timestamp=\"{3}\"\n"
                    + "  oauth_nonce=\"{4}\"",

                    DefaultValues.OAuthConsumerKey,
                    DefaultValues.OAuthSignature,
                    DefaultValues.OAuthSignatureMethod,
                    DefaultValues.OAuthTimestamp,
                    DefaultValues.OAuthNonce
                ),
                new System.Net.Http.Headers.MediaTypeHeaderValue("http-header/authorization"),
                "OAuth",
                "OAuthRequestToken"
            );

            // Response
            config.SetSampleResponse(
                string.Format(
                    CultureInfo.InvariantCulture,

                    "oauth_token={0}\n"
                    + "&oauth_token_secret={1}\n"
                    + "&oauth_callback_confirmed={2}\n"
                    + "&x_token_expires={3}",

                    DefaultValues.OAuthToken,
                    DefaultValues.OAuthTokenSecret,
                    "true",
                    10 * 60
                ),
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/form-url-encoded"),
                "OAuth",
                "OAuthRequestToken"
            );

            // -- OAuth Access Token --
            // Request
            config.SetSampleRequest(
                string.Format(
                    CultureInfo.InvariantCulture,

                    "Authorization: OAuth\n"
                    + "  oauth_consumer_key=\"{0}\"\n"
                    + "  oauth_signature=\"{1}\"\n"
                    + "  oauth_signature_method=\"{2}\"\n"
                    + "  oauth_timestamp=\"{3}\"\n"
                    + "  oauth_nonce=\"{4}\"\n"
                    + "  oauth_verifier=\"{5}\"",

                    DefaultValues.OAuthConsumerKey,
                    DefaultValues.OAuthSignature,
                    DefaultValues.OAuthSignatureMethod,
                    DefaultValues.OAuthTimestamp,
                    DefaultValues.OAuthNonce,
                    DefaultValues.OAuthTokenSecret
                ),
                new System.Net.Http.Headers.MediaTypeHeaderValue("http-header/authorization"),
                "OAuth",
                "OAuthAccessToken"
            );

            // Reponse
            config.SetSampleResponse(
                string.Format(
                    CultureInfo.InvariantCulture,

                    "oauth_token={0}\n&oauth_token_secret={1}",

                    DefaultValues.OAuthToken2,
                    DefaultValues.OAuthTokenSecret2
                ),
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/form-url-encoded"),
                "OAuth",
                "OAuthAccessToken"
            );
        }
    }
}