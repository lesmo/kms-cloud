using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Kilometros_WebAPI.MessageHandlers {
    public class RequestArrayHandler : DelegatingHandler {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            if ( request.Content.IsFormData() ) {
                NameValueCollection formData
                    = await request.Content.ReadAsFormDataAsync();
                List<KeyValuePair<string, string>> newFormData
                    = new List<KeyValuePair<string, string>>();

                bool isArrayBody
                    = (
                        from k in formData.AllKeys
                        where k.EndsWith("[]")
                        select k
                    ).Count() == formData.Count;

                if ( isArrayBody ) {
                    Dictionary<string, short> bodyKeys
                        = new Dictionary<string,short>();

                    foreach ( string key in formData.AllKeys ) {
                        string[] values
                            = formData.GetValues(key);
                        string keyName
                            = key.Remove(key.Length - 2);

                        for ( int i = 0; i < values.Length; i++ ) {
                            if ( bodyKeys.ContainsKey(key) )
                                bodyKeys[keyName]++;
                            else
                                bodyKeys.Add(keyName, 1);

                            newFormData.Add(
                                new KeyValuePair<string, string>(
                                    string.Format(
                                        "[{0}][{1}]",
                                        i,
                                        keyName
                                    ),
                                    values[i]
                                )
                            );
                        }
                    }
                } else {
                    foreach ( string key in formData.AllKeys ) {
                        string[] values
                            = formData.GetValues(key);

                        foreach ( string value in values ) {
                            newFormData.Add(
                                new KeyValuePair<string, string>(
                                    key,
                                    value
                                )
                            );
                        }
                    }
                }

                request.Content
                    = new FormUrlEncodedContent(
                        newFormData
                    );
            }
            
            // --- Continuar con la ejecución ---
            return await base.SendAsync(
                request,
                cancellationToken
            );
        }
    }
}