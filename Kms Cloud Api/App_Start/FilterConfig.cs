using System.Web;
using System.Web.Mvc;

namespace Kms.Cloud.Api {
    public static class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}