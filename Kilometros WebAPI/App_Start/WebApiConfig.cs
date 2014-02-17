﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Kilometros_WebAPI {
    public static class WebApiConfig {
        public static class KmsHttpHeaders {
            public const string ApiKey = "X-Kms-Api-Key";
            public const string RequestSignature = "X-Kms-Api-Request-Signature";
            public const string Token = "X-Kms-Api-Token";
        }
        
        public static void Register(HttpConfiguration config) {
            /** Configurar ExceptionFilters **/
            config.Filters.Add(new ExceptionFilters.HttpStatusExceptionFilter());
            config.Filters.Add(new ExceptionFilters.DbValidationExceptionFilter());

            /** Configurar MessageHandlers **/
            config.MessageHandlers.Add(new MessageHandlers.ResponseEncoder());
            config.MessageHandlers.Add(new MessageHandlers.ResponseLastModifiedHandler());
            config.MessageHandlers.Add(new MessageHandlers.RequestSecurityHandler());

            config.MapHttpAttributeRoutes();

            /*** Por defectuoso ***/
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}"
            );
        }
    }
}
