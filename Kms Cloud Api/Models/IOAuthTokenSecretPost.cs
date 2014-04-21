using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
