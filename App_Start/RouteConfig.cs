using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TradeKerala2017
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
        //    routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        //    routes.MapRoute(
        //   "names", // Route name
        //   "{names}", // URL with parameters
        //   new { controller = "Public", action = "About" } // Parameter defaults
        // );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Public", action = "Newindex", id = UrlParameter.Optional }
            );

          


        }
    }
}
