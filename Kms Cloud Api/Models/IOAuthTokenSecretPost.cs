namespace Kms.Cloud.Api.Models {
    public interface IOAuthTokenSecretPost : IOAuthTokenPost {
        string ID {
            get;
            set;
        }

        string Token {
            get;
            set;
        }

        string TokenSecret {
            get;
            set;
        }
    }
}
