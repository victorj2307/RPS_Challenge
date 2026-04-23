using System.Web;
using System.Web.Http;

namespace RPS.Challenge.Web.API {
    public class Global : HttpApplication {
        protected void Application_Start() {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
