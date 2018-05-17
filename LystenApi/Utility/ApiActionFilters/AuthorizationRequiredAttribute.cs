using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.IdentityModel;
using System.Threading.Tasks;
using LystenApi.Db;
using LystenApi.Controllers.Api;
using LystenApi.Models;

namespace LystenApi.ActionFilters
{
    public class AuthorizationRequiredAttribute : ActionFilterAttribute
    {
        private const string Token = "bearer";

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            if (filterContext.Request.Headers.Authorization == null)
            {
                filterContext.Response = filterContext.Request.CreateResponse((new { AuthorizeStatus = HttpStatusCode.Unauthorized, ReasonPhrase = "Token Validation Failed", Code = HttpStatusCode.Unauthorized, Version = HttpVersion.Version10 }));

            }
            else
            {
                if (filterContext.Request.Headers.Authorization.Scheme.ToLower().Contains(Token))
                {
                    var tokenValue = filterContext.Request.Headers.Authorization.Parameter;
                    if (tokenValue != null)
                    {
                        LystenEntities _db = new LystenEntities();
                        var chkToken = _db.AppAccessTokens.AsEnumerable().Where(top => top.AuthToken == Token).FirstOrDefault();

                        if (checkToken(tokenValue))
                        {
                        }
                        else
                        {
                            filterContext.Response = filterContext.Request.CreateResponse((new { AuthorizeStatus = HttpStatusCode.Unauthorized, ReasonPhrase = "Token Validation Failed", Code = HttpStatusCode.Unauthorized, Version = HttpVersion.Version10 }));
                        }
                    }
                }
                else
                {
                    filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Token Not Sent", StatusCode = HttpStatusCode.Unauthorized, Version = HttpVersion.Version10 };
                }
            }

            base.OnActionExecuting(filterContext);
        }
        public dynamic checkToken(string Token)
        {
            LystenEntities _db = new LystenEntities();
            var chkToken = _db.AppAccessTokens.AsEnumerable().Where(top => top.AuthToken == Token).FirstOrDefault();
            if (chkToken == null)
            {
                return false;
            }
            User_Master model = _db.User_Master.Where(x => x.Id == chkToken.UserId).FirstOrDefault();
            if (chkToken.ExpiresOn <= DateTime.Now)
            {
                //TimeSpan t = new TimeSpan(1, 0, 0, 0, 0);
                //chkToken.ExpiresOn = DateTime.Now.Add(t);
                //_db.SaveChanges();
                BaseApiController.updatetoken = true;
                TokenDetails objToken = api.Helpers.AsyncHelpers.RunSync<TokenDetails>(() => BaseApiController.generatToken(model.Email, model.Password, model.DeviceToken));
                if (String.IsNullOrEmpty(objToken.error))
                {
                    BaseApiController.Add_UpdateToken(model.Id, objToken);
                    BaseApiController.accessToken = objToken.access_token;
                }
                return true;
            }
            BaseApiController.updatetoken = false;
            BaseApiController.accessToken = "";
            return true;
        }
    }
}