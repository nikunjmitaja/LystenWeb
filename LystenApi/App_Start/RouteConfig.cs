using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LystenApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");



            


            routes.MapRoute(
name: "EventCategoryimage",
url: "EventCategoryimage",
defaults: new { controller = "Master", action = "EventCategoryimage", id = UrlParameter.Optional });



            routes.MapRoute(
name: "calling",
url: "calling",
defaults: new { controller = "Master", action = "calling", id = UrlParameter.Optional });

            routes.MapRoute(
name: "callingprices",
url: "callingprices",
defaults: new { controller = "Master", action = "CallingPrice", id = UrlParameter.Optional });





            routes.MapRoute(
    name: "event",
    url: "event",
    defaults: new { controller = "Master", action = "Event", id = UrlParameter.Optional });


            routes.MapRoute(
           name: "category",
           url: "category",
           defaults: new { controller = "Master", action = "Category", id = UrlParameter.Optional });


            routes.MapRoute(
     name: "topic",
     url: "topic/{slug}",
     defaults: new { controller = "TopicPage", action = "Topic", slug = UrlParameter.Optional });


            routes.MapRoute(
           name: "emailaccount",
           url: "emailaccount",
           defaults: new { controller = "Master", action = "emailaccount", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "emailtemplates",
                url: "emailtemplates",
                defaults: new { controller = "Master", action = "EmailTemplates", id = UrlParameter.Optional });


            routes.MapRoute(
                  name: "user",
                  url: "user",
                  defaults: new { controller = "Master", action = "User", id = UrlParameter.Optional });



            routes.MapRoute(
           name: "logout",
           url: "logout",
           defaults: new { controller = "Login", action = "logout", id = UrlParameter.Optional });


            routes.MapRoute(
               name: "profile",
               url: "profile",
               defaults: new { controller = "User", action = "Profile", id = UrlParameter.Optional });

            routes.MapRoute(
              name: "dashboard",
              url: "dashboard",
              defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "login",
                url: "login",
                defaults: new { controller = "Login", action = "Login", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}
