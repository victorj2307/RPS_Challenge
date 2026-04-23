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
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Tournament", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
