using System;
using Kilometros_WebAPI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http;
using System.Net.Http;
using System.Threading;
using System.Net;

namespace Kilometros_WebApi_UnitTesting {
    [TestClass]
    public class SessionControllerTest {
        [TestMethod]
        public void KmsLogin_ReturnsNotFound_WhenUserNotFound() {
            HttpConfiguration config
                = new HttpConfiguration();
            config.Routes.MapHttpRoute("Default", "{controller}/{id}");
            
            HttpServer server
                = new HttpServer(config);

            using ( HttpMessageInvoker client = new HttpMessageInvoker(server) ) {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/Movies/-1"))
                using ( HttpResponseMessage response = client.SendAsync(request, CancellationToken.None).Result ) {
                    Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
                }
            }
        }
    }
}
