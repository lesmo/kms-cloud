using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net.Http;

namespace Kilometros_WebAPI.Helpers {
    public class HttpRequestMessageHeadersHelper {
        private readonly HttpRequestMessage httpRequestMessage;

        public HttpRequestMessageHeadersHelper(HttpRequestMessage httpRequestMessage) {
            this.httpRequestMessage = httpRequestMessage;
        }

        public string[] GetHeaderValues(string headerKey) {
            IEnumerable<string> headerValues;
            HttpRequestMessage message = this.httpRequestMessage ?? new HttpRequestMessage();

            if ( message.Headers.TryGetValues(headerKey, out headerValues) )
                return headerValues.ToArray();
            else
                return new string[0];
        }

        public T[] GetHeaderValues<T>(string headerKey, Func<string, T> valueTransform) {
            IEnumerable<string> headerValues;
            HttpRequestMessage message = this.httpRequestMessage ?? new HttpRequestMessage();

            if ( !message.Headers.TryGetValues(headerKey, out headerValues) )
                return new T[0];

            List<T> headerReturnValues = new List<T>();
            foreach ( string headerValue in headerValues ) {
                    headerReturnValues.Add(
                        valueTransform(headerValue)
                    );
            }
            
            return headerReturnValues.ToArray();
        }

        public string GetHeaderValue(string headerKey) {
            string[] headerValues = this.GetHeaderValues(headerKey);

            if ( headerValues.Length > 0 )
                return headerValues[0];
            else
                return "";
        }

        public T GetHeaderValue<T>(string headerKey, Func<string, T> valueTransform) {
            T[] headerValues = this.GetHeaderValues<T>(headerKey, valueTransform);

            if ( headerValues.Length > 0 )
                return headerValues[0];
            else
                return default(T);
        }
    }
}