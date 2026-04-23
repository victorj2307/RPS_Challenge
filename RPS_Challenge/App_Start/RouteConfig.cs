using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace RPS_Challenge
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "LegacyDefaultPage",
                url: "Default.aspx",
                defaults: new { controller = "Tournament", action = "Index" }
            );

            routes.MapRoute(
                name: "LegacyAboutPage",
                url: "About.aspx",
                defaults: new { controller = "About", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Tournament", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
