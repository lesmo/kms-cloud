using Kms.Cloud.Api.Areas.HelpPage.CustomSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace Kms.Cloud.Api.Areas.HelpPage {
    public static class CustomSamplesRegister {
        public static void Register(Dictionary<Type, object> typeSamples, HttpConfiguration config) {
            // Obtener todas las clases en Kms.Cloud.Api.Areas.HelpPage.CustomSamples
            IEnumerable<Type> types
                = from t in Assembly.GetExecutingAssembly().GetTypes()
                  where t.IsClass && t.Namespace == @"Kms.Cloud.Api.Areas.HelpPage.CustomSamples"
                  select t;

            // Llamar a todos los "Register" de las Clases
            foreach ( Type type in types ) {
                MethodInfo method
                    = type.GetMethod("Register");
                
                if (method != null)
                    method.Invoke(null, new object[]{typeSamples, config});
            }
        }
    }
}