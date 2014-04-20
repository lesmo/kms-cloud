using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace System.Web.Routing
{
    public static class TranslatedRouteCollectionExtensions
    {
        public static TranslatedRoute MapTranslatedRoute(this RouteCollection routes, string name, string url, object defaults, object routeValueTranslationProviders) {
            TranslatedRoute route = new TranslatedRoute(
                url,
                new RouteValueDictionary(defaults),
                new RouteValueDictionary(routeValueTranslationProviders),
                new MvcRouteHandler()
            );
            routes.Add(name, route);
            return route;
        }

        public static TranslatedRoute MapTranslatedRoute(this RouteCollection routes, string name, string url, object defaults, object routeValueTranslationProviders, object constraints) {
            TranslatedRoute route = new TranslatedRoute(
                url,
                new RouteValueDictionary(defaults),
                new RouteValueDictionary(routeValueTranslationProviders),
                new RouteValueDictionary(constraints),
                new MvcRouteHandler()
            );
            routes.Add(name, route);
            return route;
        }
    }
}
