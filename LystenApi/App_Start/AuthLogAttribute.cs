using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LystenApi.Utility;
using LystenApi.Db;
using System.Web.Security;

namespace LystenApi
{
    public class AuthLogAttribute : AuthorizeAttribute
    {
        UserServices US = new UserServices();
        public AuthLogAttribute()
        {
            View = "AuthorizeFailed";
        }

        public string View { get; set; }

        /// <summary>
        /// Check for Authorization
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            //IsUserAuthorized(filterContext);
        }

        /// <summary>
        /// Method to check if the user is Authorized or not
        /// if yes continue to perform the action else redirect to error page
        /// </summary>
        /// <param name="filterContext"></param>
        //private void IsUserAuthorized(AuthorizationContext filterContext)
        //{

        //    var isauth = false;
        //    if (HttpContext.Current.Request.Cookies.Count > 0)
        //    {
        //        if (HttpContext.Current.Request.Cookies["Userid"] != null)
        //        {
        //            var id = HttpContext.Current.Request.Cookies["Userid"].Value;
        //            List<User_Modules_Mapping> result = US.GetModulesByUserid(Convert.ToInt32(id));
        //            if (result != null)
        //            {
        //                string[] roleIds = Roles.Split(',');
        //                foreach (var roleId in roleIds)
        //                {
        //                    var flag = result.Any(s => s.Modules_Master.Name.Contains(roleId));
        //                    //appRoles.AddRange(roleList[roleId].Split(new[] { ',' }));
        //                    if (flag == true)
        //                    {
        //                        isauth = true;
        //                        filterContext.Result = null;
        //                        return;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (isauth == false)
        //    {

        //        if (filterContext.HttpContext.User.Identity.IsAuthenticated)
        //        {

        //            // var result = new ViewResult { ViewName = View };
        //            var vr = new ViewResult();
        //            vr.ViewName = View;

        //            ViewDataDictionary dict = new ViewDataDictionary();
        //            dict.Add("Message", "Sorry you are not Authorized to Perform this Action");

        //            vr.ViewData = dict;

        //            var result = vr;

        //            filterContext.Result = result;
        //        }
        //    }
        //    else
        //    {
        //        filterContext.Result = null;
        //        return;
        //    }



        /*  // If the Result returns null then the user is Authorized 
          if (filterContext.Result == null)
              return;

          //If the user is Un-Authorized then Navigate to Auth Failed View 
          if (filterContext.HttpContext.User.Identity.IsAuthenticated)
          {

              // var result = new ViewResult { ViewName = View };
              var vr = new ViewResult();
              vr.ViewName = View;

              ViewDataDictionary dict = new ViewDataDictionary();
              dict.Add("Message", "Sorry you are not Authorized to Perform this Action");

              vr.ViewData = dict;

              var result = vr;

              filterContext.Result = result;
          }

    }  */
    }
}
