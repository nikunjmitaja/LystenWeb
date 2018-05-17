using Nito.AspNetBackgroundTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using LystenApi.Models;
using LystenApi.Utility.ApiServices;
using LystenApi.Utility;
using api.ActionFilters;
using LystenApi.ActionFilters;
using System.Web;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http.Headers;
using LystenApi.Db;
using System.Web.Configuration;
using LystenApi.Utility.Providers;

namespace LystenApi.Controllers.Api
{
    [RoutePrefix("api/v1/account")]
    public class LoginController : BaseApiController
    {
        //CommonServices cs = new CommonServices();
        MasterServices MS = new MasterServices();
        ApiCommonServices ApiCommon = new ApiCommonServices();
        ApiException ApiEx = new ApiException();
        ApiMasterServices ApiMaster = new ApiMasterServices();
        ApiMessageFormat ap = new ApiMessageFormat();
        ApiUserServices Apiuser = new ApiUserServices();
        ApiMessageServices ApiMessage = new ApiMessageServices();

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }




        [AuthorizationRequired]
        [HttpPost]
        [Route("GetAllTopic")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetAllTopic()
        {
            ResultClassForNonAuth result = new ResultClassForNonAuth();
            try
            {
                var mm = ApiMaster.GetTopicList();
                if (mm.Count > 0)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.CategoryNoData;
                    result.Data = mm;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        [Route("testapi")]
        // GET api/<controller>/5
        public async Task<IHttpActionResult> testapi()
        {
            return Ok(new { });
        }



        [Route("gettocken")]
        // GET api/<controller>/5
        public async Task<IHttpActionResult> gettocken(int UserId)
        {
            var result = ApiCommon.GetLatestTockenByUserId(UserId);
            return Ok(result);
        }

        [Route("login")]
        [HttpPost]
        // POST api/<controller>
        public async Task<IHttpActionResult> Login([FromBody]UserMasterModel UM)
        {
            ResultClass result = new ResultClass();
            try
            {
                result = await ApiCommon.PostLoginAuthenticationAsync(UM);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }


        [Route("newlogin")]
        [HttpPost]
        // POST api/<controller>
        public async Task<IHttpActionResult> LoginNew(UserMasterModel UM)
        {
            ResultClass result = new ResultClass();
            try
            {
                result = await ApiCommon.LoginAuthenticationAsync(UM);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }


        [HttpGet]
        [Route("logout")]
        // POST api/<controller>
        public async Task<IHttpActionResult> Logout(int UserId)
        {
            ResultClassCommon result = new ResultClassCommon();


            result = ApiCommon.logoutUser(UserId);
            return Ok(result);
        }

        [Route("forgotpassword")]
        [HttpGet]
        // POST api/<controller>
        public async Task<IHttpActionResult> ForgotPassword(string Email)
        {
            ResultClassCommon result = new ResultClassCommon();
            try
            {
                result = ApiCommon.GetforgotPassword(Email.Trim());
                if (result.Code == 200)
                {
                    var obj = await ApiCommon.SendEmail(Email.Trim());
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        [AuthorizationRequired]
        [Route("changepassword")]
        [HttpPost]
        // PUT api/<controller>/5
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordModel CP)
        {
            ResultClassToken result = new ResultClassToken();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            result = ApiCommon.ChangePassword(CP, token);
            if (updatetoken)
            {
                token = result.AccessToken = accessToken;
            }
            else
            {
                result.AccessToken = "";
            }
            return Ok(result);
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IHttpActionResult> PostFile()
        {
            ResultClass result = new ResultClass();
            try
            {
                User_Master Um = new User_Master();
                Um.Email = HttpContext.Current.Request.Params["Email"];
                Um.UserName = HttpContext.Current.Request.Params["UserName"];
                Um.Password = HttpContext.Current.Request.Params["Password"];
                Um.Phone = HttpContext.Current.Request.Params["Phone"];
                Um.Age = Convert.ToInt16(HttpContext.Current.Request.Params["Age"]);
                Um.Gender = Convert.ToInt16(HttpContext.Current.Request.Params["Gender"]);
                Um.FullName = (HttpContext.Current.Request.Params["FullName"]);
                Um.DeviceToken = (HttpContext.Current.Request.Params["DeviceToken"]);
                Um.DeviceType = (HttpContext.Current.Request.Params["DeviceType"]);
                Um.IsActive = true;
                Um.Createdate = System.DateTime.Now;
                Um.Createdby = 1; Um.Image = "";

                Um.RoleId = Convert.ToInt32((HttpContext.Current.Request.Params["RoleId"]));
                //Um.Skill = (HttpContext.Current.Request.Params["Skill"]);
                //Um.PostalCode = (HttpContext.Current.Request.Params["PostalCode"]);
                //Um.SSN = (HttpContext.Current.Request.Params["SSN"]);
                //Um.DateOfBirth = (HttpContext.Current.Request.Params["DateOfBirth"]);
                //Um.Address = (HttpContext.Current.Request.Params["Address"]);
                //Um.TimeZone = (HttpContext.Current.Request.Params["TimeZone"]);
                //Um.CityId = Convert.ToInt32((HttpContext.Current.Request.Params["City"]));
                //Um.CountryId = Convert.ToInt32((HttpContext.Current.Request.Params["Country"]));
                //Um.StateId = Convert.ToInt32((HttpContext.Current.Request.Params["State"]));

                int iUploadedCnt = 0;

                // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["userimagepath"]);

                bool exists = System.IO.Directory.Exists(sPath);

                if (!exists)
                    System.IO.Directory.CreateDirectory(sPath);
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                result = await ApiCommon.RegisterUser(Um);

                // CHECK THE FILE COUNT.
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];

                    if (hpf.ContentLength > 0)
                    {
                        string ImagePath = result.Data.Id + "_" + hpf.FileName;
                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                        if (!File.Exists(sPath + Path.GetFileName(ImagePath)))
                        {
                            // SAVE THE FILES IN THE FOLDER.
                            hpf.SaveAs(sPath + Path.GetFileName(ImagePath));
                            Um.Image = ImagePath;
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                        else
                        {
                            File.Delete(sPath + Path.GetFileName(ImagePath));
                            hpf.SaveAs(sPath + Path.GetFileName(ImagePath));
                            Um.Image = ImagePath;
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                        Um.Id = result.Data.Id;
                        var dadata = Apiuser.UpdateProfilePic(Um);
                        dadata.Result.AccessToken = result.AccessToken;
                        return Ok(dadata.Result);

                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }



        [Route("newRegister")]
        [HttpPost]
        public async Task<IHttpActionResult> Regeister()
        {
            ResultClass result = new ResultClass();
            try
            {
                User_Master Um = new User_Master();
                Um.Email = HttpContext.Current.Request.Params["Email"];
                Um.UserName = HttpContext.Current.Request.Params["UserName"];
                try
                {
                    Um.Password = HttpContext.Current.Request.Params["Password"];
                    Um.Age = Convert.ToInt16(HttpContext.Current.Request.Params["Age"]);
                }
                catch { Um.Password = Um.UserName + "@2018"; Um.Age = 18; }
                Um.Phone = HttpContext.Current.Request.Params["Phone"];
                
                Um.Gender = Convert.ToInt16(HttpContext.Current.Request.Params["Gender"]);
                Um.FullName = (HttpContext.Current.Request.Params["FullName"]);

                Um.DeviceToken = (HttpContext.Current.Request.Params["DeviceToken"]);
                Um.DeviceType = (HttpContext.Current.Request.Params["DeviceType"]);
                Um.RoleId = Convert.ToInt32((HttpContext.Current.Request.Params["RoleId"]));
                Um.Skill = (HttpContext.Current.Request.Params["Skill"]);
                Um.PostalCode = (HttpContext.Current.Request.Params["PostalCode"]);
                Um.SSN = (HttpContext.Current.Request.Params["SSN"]);
                Um.DateOfBirth = (HttpContext.Current.Request.Params["DateOfBirth"]);
                Um.Address = (HttpContext.Current.Request.Params["Address"]);
                //Um.TimeZone = (HttpContext.Current.Request.Params["TimeZone"]);
                Um.CityId = Convert.ToInt32((HttpContext.Current.Request.Params["City"]));
                Um.CountryId = Convert.ToInt32((HttpContext.Current.Request.Params["Country"]));
                Um.StateId = Convert.ToInt32((HttpContext.Current.Request.Params["State"]));
                Um.IsActive = true;
                Um.Createdate = System.DateTime.Now;
                Um.Createdby = 1; Um.Image = "";

                int iUploadedCnt = 0;

                // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["userimagepath"]);

                bool exists = System.IO.Directory.Exists(sPath);

                if (!exists)
                    System.IO.Directory.CreateDirectory(sPath);
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                result = await ApiCommon.newRegisterUser(Um);

                // CHECK THE FILE COUNT.
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];

                    if (hpf.ContentLength > 0)
                    {
                        string ImagePath = result.Data.Id + "_" + hpf.FileName;
                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                        if (!File.Exists(sPath + Path.GetFileName(ImagePath)))
                        {
                            // SAVE THE FILES IN THE FOLDER.
                            hpf.SaveAs(sPath + Path.GetFileName(ImagePath));
                            Um.Image = ImagePath;
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                        else
                        {
                            File.Delete(sPath + Path.GetFileName(ImagePath));
                            hpf.SaveAs(sPath + Path.GetFileName(ImagePath));
                            Um.Image = ImagePath;
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                        Um.Id = result.Data.Id;
                        var dadata = Apiuser.UpdateProfilePic(Um);
                        dadata.Result.AccessToken = result.AccessToken;
                        return Ok(dadata.Result);

                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }
    }
}