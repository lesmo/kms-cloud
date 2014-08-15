using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace Kms.Cloud.Api {
    public static class WebApiConfig {
        internal static class KmsOAuthConfig {
            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            public static readonly HashSet<string> RequiredParams = new HashSet<string> {
                "oauth_consumer_key",
                "oauth_nonce", 
                "oauth_signature",
                "oauth_signature_method",
                "oauth_timestamp",
                //"oauth_token",
                "oauth_version"
            };

            public const string ApiRealm = "api.kms.me";

            public static readonly HashSet<string> BypassOAuthAbsoluteUris = new HashSet<string> {
                "scheduler/*"
            };
        }
        
        public static void Register(HttpConfiguration config) {
            // --- Configurar ExceptionFilters ---
            config.Filters.Add(new ExceptionFilters.HttpStatusExceptionFilterAttribute());
            config.Filters.Add(new ExceptionFilters.DbValidationExceptionFilter());
            config.Filters.Add(new ExceptionFilters.UnhandledExceptionFilterAttributeFilter());

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
