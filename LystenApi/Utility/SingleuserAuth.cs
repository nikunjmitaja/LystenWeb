using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LystenApi.Utility;
using LystenApi.Db;

namespace LystenApi.Utility
{
    public class SingleuserAuth : ActionFilterAttribute
    {
        MasterServices MS = new MasterServices();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Request.Cookies.Count > 0)
            {
                if (HttpContext.Current.Request.Cookies["Userid"] != null && HttpContext.Current.Request.Cookies["SessionID"] != null)
                {
                    var id = HttpContext.Current.Request.Cookies["Userid"].Value;
                    User_Master result = MS.Getuserbyid(Convert.ToInt32(id));
                    if (result == null)
                    {
                        FormsAuthentication.SignOut();
                        HttpContext.Current.Response.Redirect("/Login");
                    }
                    else if (result.SessionId == null)
                    {
                        FormsAuthentication.SignOut();
                        HttpContext.Current.Response.Redirect("/Login");
                    }
                    else if (!result.SessionId.Equals(Convert.ToString(HttpContext.Current.Request.Cookies["SessionID"].Value)))
                    {
                        FormsAuthentication.SignOut();
                        HttpContext.Current.Response.Redirect("/Login");
                    }
                }
                else
                {
                    FormsAuthentication.SignOut();
                    HttpContext.Current.Response.Redirect("/Login");
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}