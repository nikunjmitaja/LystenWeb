using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LystenApi.Db;
using LystenApi.Models;
using LystenApi.Utility;
//using ZXing;
using System.Drawing;
using System.Collections.ObjectModel;
using NodaTime;
//using Microsoft.Reporting.WebForms;

namespace LystenApi.Controllers
{
    public class LoginController : Controller
    {
        CommonServices cs = new CommonServices();
        MasterServices MS = new MasterServices();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (Request.IsAuthenticated)
            {
                return RedirectToRoute("dashboard");
            }
            Response.Cookies["UserName"].Value = "0";
            Response.Cookies["Password"].Value = "0";
            return View();
        }

        public ActionResult Login123(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (Request.IsAuthenticated)
            {
                return RedirectToRoute("dashboard");
            }
            Response.Cookies["UserName"].Value = "0";
            Response.Cookies["Password"].Value = "0";
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection FC, string ReturnUrl)
        {
            try
            {
                UserMasterModel UM = new UserMasterModel();
                UM.Email = Request.Form["Email"];
                UM.Password = Request.Form["Password"];
                if (Convert.ToString(Request.Form["RememberMe"]) == "on")
                {
                    UM.RememberMe = true;
                }
                else
                {
                    UM.RememberMe = false;
                }
                var result = cs.PostLoginAuthentication(UM);
                if (result.Code == 200)
                {
                    if (UM.RememberMe)
                    {
                        Response.Cookies["UserName"].Expires = DateTime.Now.AddYears(1);
                        Response.Cookies["Password"].Expires = DateTime.Now.AddYears(1);
                        Response.Cookies["UserName"].Value = UM.Email;
                        Response.Cookies["Password"].Value = UM.Password;

                    }
                    else
                    {
                        Response.Cookies["UserName"].Expires = DateTime.Now.AddYears(-1);
                        Response.Cookies["Password"].Expires = DateTime.Now.AddYears(-1);
                        Response.Cookies["UserName"].Value = "0";
                        Response.Cookies["Password"].Value = "0";

                    }

                    var objuser = (result.Data[0]);
                    objuser.SessionId = System.Web.HttpContext.Current.Session.SessionID;
                    MS.updatesession(objuser);

                    HttpCookie UserCookies = new HttpCookie("Userid");
                    UserCookies.Value = Convert.ToString(objuser.Id);
                    UserCookies.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Add(UserCookies);

                

                    HttpCookie SessionIDCookies = new HttpCookie("SessionID");
                    SessionIDCookies.Value = System.Web.HttpContext.Current.Session.SessionID;
                    SessionIDCookies.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Add(SessionIDCookies);

                    //HttpContext.Application["usr_" + objuser.Id] = HttpContext.Session.SessionID;

                    // FormsAuthentication.SetAuthCookie(objuser.um.Username, false);
                    FormsAuthentication.SetAuthCookie(objuser.Email, false);
                    if (ReturnUrl != null)
                    {
                        ReturnUrl = ReturnUrl.Remove(0, 1);
                        return RedirectToRoute(ReturnUrl);
                    }
                    if (objuser.RoleId == 1){
                        return RedirectToRoute("dashboard");
                    }
                    else{
                        return View();
                    }
                }
                else
                {
                    ViewBag.Errormsg = "Invalid Email Or Password!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult ForgotPassword(string EmailId)
        {
            try
            {
                var i = cs.GetforgotPassword(EmailId);
                return Json(i, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            string loggedOutPageUrl = "Logout.aspx";
            Response.Write("<script language='javascript'>");
            Response.Write("function ClearHistory()");
            Response.Write("{");
            Response.Write(" var backlen=history.length;");
            Response.Write(" history.go(-backlen);");
            Response.Write(" window.location.href='" + loggedOutPageUrl + "'; ");
            Response.Write("}");
            Response.Write("</script>");

            //HttpContext.Application.Remove("usr_" + userid);
            HttpContext.Request.Cookies.Remove("Userid");
            FormsAuthentication.SignOut();

            return RedirectToAction("Login");
        }
    }
}
