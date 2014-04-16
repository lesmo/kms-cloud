using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Kilometros_WebApp {
    public static class RouteTranslationConfig {
        private static Dictionary<string, CultureInfo> _cultures
            = new Dictionary<string,CultureInfo>();
        private static CultureInfo Culture(string culture) {
            if ( _cultures.ContainsKey(culture) )
                return _cultures[culture];
            
            _cultures.Add(culture, new CultureInfo(culture));

            return _cultures[culture];
        }
            
        public static DictionaryRouteValueTranslationProvider ControllerGlobalization
            = new DictionaryRouteValueTranslationProvider(
                new List<RouteValueTranslation> {
                    new RouteValueTranslation(Culture("es-MX"), "Friends", "Amigos"),
                    new RouteValueTranslation(Culture("es-MX"), "Login", "IniciarSesion"),
                    new RouteValueTranslation(Culture("es-MX"), "Overview", "Perfil"),
                    new RouteValueTranslation(Culture("es-MX"), "Rewards", "Recompensas"),
                    new RouteValueTranslation(Culture("es-MX"), "Tips", "Tips"),
                }
            );
    }
}