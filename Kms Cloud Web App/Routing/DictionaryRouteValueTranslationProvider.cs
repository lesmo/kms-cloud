using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace System.Web.Routing {
    public class DictionaryRouteValueTranslationProvider : IRouteValueTranslationProvider {
        public IList<RouteValueTranslation> Translations {
            get;
            private set;
        }

        public DictionaryRouteValueTranslationProvider(IList<RouteValueTranslation> translations) {
            this.Translations
                = translations;
        }

        public RouteValueTranslation TranslateToRouteValue(string translatedValue, CultureInfo culture) {
            // Find translation in specified CultureInfo
            RouteValueTranslation  translation
                = (
                    from t in this.Translations
                    orderby t.Culture.Name descending
                    where
                        (
                            t.Culture.Name == culture.Name
                            || t.Culture.TwoLetterISOLanguageName == culture.TwoLetterISOLanguageName
                        ) && t.TranslatedValue == translatedValue
                    select t
                ).FirstOrDefault();
            
            if (translation != null)
                return translation;

            // Find translation without taking account on CultureInfo
            translation
                = this.Translations.Where(t =>
                    t.TranslatedValue == translatedValue
                ).FirstOrDefault();
            
            if (translation != null)
                return translation;

            // Return the current values
            return new RouteValueTranslation(
                culture,
                translatedValue,
                translatedValue
            );
        }

        public RouteValueTranslation TranslateToTranslatedValue(string routeValue, CultureInfo culture) {
            // Find translation in specified CultureInfo
            RouteValueTranslation translation
                = (
                    from t in this.Translations
                    orderby t.Culture.Name descending
                    where
                        (
                            t.Culture.Name == culture.Name
                            || t.Culture.TwoLetterISOLanguageName == culture.TwoLetterISOLanguageName
                        ) && t.RouteValue == routeValue
                    select t
                ).FirstOrDefault();
            
            if ( translation != null )
                return translation;

            // Find translation without taking account on CultureInfo
            translation
                = this.Translations.Where(
                    t => t.RouteValue == routeValue
                ).FirstOrDefault();

            if ( translation != null )
                return translation;

            // Return the current values
            return new RouteValueTranslation(
                culture,
                routeValue,
                routeValue
            );
        }
    }
}
