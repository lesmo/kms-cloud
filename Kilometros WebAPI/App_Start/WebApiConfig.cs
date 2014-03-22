using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Kilometros_WebAPI {
    public static class WebApiConfig {
        public static class KmsOAuthConfig {
            public static readonly string[] RequiredParams = {
                "oauth_consumer_key",
                "oauth_nonce", 
                "oauth_signature",
                "oauth_signature_method",
                "oauth_timestamp",
                //"oauth_token",
                "oauth_version"
            };

            public const string ApiRealm = "api.kms.me";
            public const string GuiRealm = "gui.api.kms.me";

            public static readonly string[] BypassOAuthAbsoluteUris = {
                "/account"
            };
        }
        
        public static void Register(HttpConfiguration config) {
            // --- Configurar ExceptionFilters ---
            config.Filters.Add(new ExceptionFilters.UnhandledExceptionFilter());
            config.Filters.Add(new ExceptionFilters.HttpStatusExceptionFilter());
            config.Filters.Add(new ExceptionFilters.DbValidationExceptionFilter());

            // --- Configurar MessageHandlers ---
            config.MessageHandlers.Add(new MessageHandlers.ResponseEncoder());
            config.MessageHandlers.Add(new MessageHandlers.ResponseLastModifiedHandler());
            config.MessageHandlers.Add(new MessageHandlers.RequestSecurityHandler());

            // --- Configurar rutas ---
            // Todas las rutas se establecen con el tag [Route(--)]
            config.MapHttpAttributeRoutes();
        }
    }
}
