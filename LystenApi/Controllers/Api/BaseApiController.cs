using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using LystenApi.Db;
using LystenApi.Models;
using System.Data;

namespace LystenApi.Controllers.Api
{
    public class BaseApiController : ApiController
    {
        public static bool updatetoken = false;
        public static string accessToken = "";
        public static async Task<TokenDetails> generatToken(string Email, string Password, string deviceid)
        {
            var request = HttpContext.Current.Request;
            var tokenServiceUrl = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath + "/oauth/Token";
            using (var client = new HttpClient())
            {
                var requestParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", Email),
                    new KeyValuePair<string, string>("password", Password),
                    new KeyValuePair<string, string>("deviceid", deviceid)
                };
                var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                var tokenServiceResponse =  client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded).Result;
                var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();
                var responseCode = tokenServiceResponse.StatusCode;
                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                TokenDetails objToken = jsonSerializer.Deserialize<TokenDetails>(responseString.ToString());
                //detocken(objToken.access_token);
                return objToken;
            }
        }

        public static IQueryable<T> CreatePagedResults<T, TResult>(IQueryable<T> query, int pageNum, int pageSize, out int rowsCount, out int page, out int totalPageCount)
        {
            if (pageSize <= 0) pageSize = 20;
            if (pageNum <= 0) pageNum = 1;
            //Total result count
            rowsCount = query.Count();
            var mod = rowsCount % pageSize;
            totalPageCount = (rowsCount / pageSize) + (mod == 0 ? 0 : 1);
            //If page number should be > 0 else set to first page
            //if (rowsCount <= pageSize || pageNum <= 0) pageNum = 1;

            //Calculate nunber of rows to skip on pagesize
            int excludedRows = (pageNum - 1) * pageSize;

            //query = isAscendingOrder ? query.OrderBy(orderByProperty) : query.OrderByDescending(orderByProperty);

            //Skip the required rows for the current page and take the next records of pagesize count
            page = pageNum + 1;
            return query.Skip(excludedRows).Take(pageSize);
        }

        public static void Add_UpdateToken(int userId, TokenDetails _token, int forceupdate = 0,string deviceType=null)
        {
            try
            {
                LystenEntities db = new LystenEntities();
                var chkToken = db.AppAccessTokens.Where(x => x.UserId == userId).FirstOrDefault();
                if (chkToken != null)
                {
                    if (chkToken.ExpiresOn <= DateTime.Now)
                    {
                        db.Entry(chkToken).State = EntityState.Modified;

                        TimeSpan t = TimeSpan.FromMinutes(Convert.ToInt16(ConfigurationManager.AppSettings["TokenExpireHour"].ToString()));
                        chkToken.AuthToken = _token.access_token;
                        chkToken.ExpiresOn = DateTime.Now.Add(t);
                        chkToken.IssuedOn = DateTime.Now;
                        
                        db.SaveChanges();
                    }
                    else
                    {
                        if (forceupdate > 0)
                        {
                            db.Entry(chkToken).State = EntityState.Modified;

                            TimeSpan t = TimeSpan.FromMinutes(Convert.ToInt16(ConfigurationManager.AppSettings["TokenExpireHour"].ToString()));
                            chkToken.AuthToken = _token.access_token;
                            chkToken.ExpiresOn = DateTime.Now.Add(t);
                            chkToken.IssuedOn = DateTime.Now;
                            chkToken.DeviceType = deviceType;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    AppAccessToken _tokenDetails = new AppAccessToken();
                    _tokenDetails.UserId = userId;
                    _tokenDetails.IssuedOn = DateTime.Now;
                    TimeSpan t = TimeSpan.FromMinutes(Convert.ToInt16(ConfigurationManager.AppSettings["TokenExpireHour"].ToString()));
                    _tokenDetails.ExpiresOn = DateTime.Now.Add(t);
                    _tokenDetails.AuthToken = _token.access_token;
                    _tokenDetails.DeviceType = deviceType;
                    db.AppAccessTokens.Add(_tokenDetails);
                    db.SaveChanges();
                }
                db.Dispose();
            }
            catch (Exception ex)
            {

              
            }
        }
    }
}
