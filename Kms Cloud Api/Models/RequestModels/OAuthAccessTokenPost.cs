using System.Diagnostics.CodeAnalysis;

namespace Kms.Cloud.Api.Models.RequestModels {
    public class OAuthAccessTokenPost {
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "oauth")]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "verifier")]
        public string oauth_verifier {
            get;
            set;
        }
    }
}