using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using LystenApi.Controllers.Api;
using LystenApi.Db;
using LystenApi.Models;
using LystenApi.Utility.Providers;
using System.Data;
using System.Text;
using System.Web.Configuration;
using LystenApi.ViewModel;

namespace LystenApi.Utility.ApiServices
{
    public class ApiCommonServices : BaseApiController
    {
        private ResultClass objresult;// = new ResultClass();
        private ResultClassCommon objresultCommon;//= new ResultClassCommon();
        private ResultClassToken objresultToken;// = new ResultClassToken();
        private EmailServices es;//= new EmailServices();
        private ApiMessageFormat ap;//= new ApiMessageFormat();
        private ApiUserServices US;//= new ApiUserServices();

        public ApiCommonServices()
        {
            objresult = new ResultClass();
            objresultCommon = new ResultClassCommon();
            objresultToken = new ResultClassToken();
            es = new EmailServices();
            ap = new ApiMessageFormat();
            US = new ApiUserServices();
        }


        public async Task<ResultClass> PostLoginAuthenticationAsync(UserMasterModel objtblusermaster)
        {
            try
            {
                using (LystenEntities db = new LystenEntities())
                {

                    string baseURL = HttpContext.Current.Request.Url.Authority;
                    baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");

                    //db.Configuration.LazyLoadingEnabled = false;
                    var pwd = SecutiryServices.EncodePasswordToBase64(objtblusermaster.Password);

                    User_Master result = (from um23 in db.User_Master
                                          where um23.Email.ToUpper() == objtblusermaster.UserNameorEmail.ToUpper() && um23.Password == pwd
                                          select um23
                                 ).FirstOrDefault();
                    if (result == null)
                    {
                        result = (from um23 in db.User_Master
                                  where um23.UserName.ToUpper() == objtblusermaster.UserNameorEmail.ToUpper() && um23.Password == pwd
                                  select um23
                                 ).FirstOrDefault();
                    }
                    var model = new
                    {

                    };
                    if (result != null)
                    {
                        var id = result.Id;
                        if (result.IsActive == true)
                        {
                            objresult.Code = (int)HttpStatusCode.OK;
                            objresult.Msg = ap.Success;
                            //var user = result;
                            objresult.Data = AutoMapper.Mapper.Map<UserViewLoginModel>(result);
                            User_Master obj = result;//db.User_Master.Where(x => x.Id == id).FirstOrDefault();
                            obj.SessionId = null;
                            obj.IsLogin = true;
                            obj.DeviceType = objtblusermaster.DeviceType;
                            obj.DeviceToken = objtblusermaster.DeviceToken;
                            obj.TimeZone = objtblusermaster.TimeZone;
                            db.Entry(obj).State = EntityState.Modified;
                            db.SaveChanges();
                            objresult.Data.Image = US.GetFavouriteImage(baseURL, result.Id);
                        }
                        else
                        {
                            objresult.Code = (int)HttpStatusCode.Accepted;
                            objresult.Msg = ap.LoginUserIsNotActive;
                            //var user = result;
                            objresult.Data = AutoMapper.Mapper.Map<UserViewLoginModel>(result);
                        }

                    }
                    else if (result == null)
                    {
                        objresult.Code = (int)HttpStatusCode.NotFound;
                        objresult.Msg = ap.LoginUserInvalid;
                        objresult.Data = model;
                        objresult.AccessToken = "";

                    }
                    else
                    {
                        objresult.Code = (int)HttpStatusCode.Accepted;
                        objresult.Msg = ap.LoginUserIsNotActive;
                        result.Image = US.GetFavouriteImage(baseURL, result.Id);

                        objresult.Data = AutoMapper.Mapper.Map<UserViewLoginModel>(result);
                    }

                    if (objresult.Code == (int)HttpStatusCode.OK)
                    {
                        TokenDetails objToken = await generatToken((result.Email), (result.Password), (result.DeviceToken));
                        //User_Master obj1 = db.User_Master.Where(x => x.Id == result.Id).FirstOrDefault();
                        //obj1.DeviceType = objtblusermaster.DeviceType;
                        ////obj.DeviceToken = objtblusermaster.DeviceToken;
                        //db.Entry(obj1).State = EntityState.Modified;
                        //db.SaveChanges();
                        (objresult.AccessToken) = objToken.access_token;
                        var obj = objresult.Data as UserViewLoginModel;
                        Add_UpdateToken(obj.Id, objToken, 1, objtblusermaster.DeviceType);
                    }
                    //if (updatetoken)
                    //{
                    //    (objresult.Data as UserViewModel).AccessToken = accessToken;
                    //}
                    //else
                    //{
                    //    (objresult.Data as UserViewModel).AccessToken = "";
                    //}
                    //db.Configuration.LazyLoadingEnabled = true;
                    return objresult;
                }
            }
            catch (Exception ex)
            {
                objresult.Code = (int)HttpStatusCode.NotAcceptable;
                objresult.Msg = Convert.ToString(ex.Message);
                objresult.Data = "";
                objresult.AccessToken = "";
                return objresult;
            }
        }



        public async Task<ResultClass> LoginAuthenticationAsync(UserMasterModel objtblusermaster)
        {
            try
            {
                using (LystenEntities db = new LystenEntities())
                {

                    string baseURL = HttpContext.Current.Request.Url.Authority;
                    baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");


                    var pwd = SecutiryServices.EncodePasswordToBase64(objtblusermaster.Password);
                    User_Master result = (from um23 in db.User_Master
                                          where um23.Email.ToUpper() == objtblusermaster.UserNameorEmail.ToUpper() && um23.Password == pwd
                                          select um23
                                 ).FirstOrDefault();
                    if (result == null)
                    {
                        result = (from um23 in db.User_Master
                                  where um23.UserName.ToUpper() == objtblusermaster.UserNameorEmail.ToUpper() && um23.Password == pwd
                                  select um23
                                 ).FirstOrDefault();
                    }
                    var model = new
                    {

                    };
                    if (result != null)
                    {
                        var id = result.Id;
                        if (result.IsActive == true)
                        {
                            if ((result.IsVerified == null || result.IsVerified == false) && result.RoleId == 3)
                            {
                                objresult.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                                objresult.Msg = "User is not verified by Admin!";
                                objresult.Data = "";
                                objresult.AccessToken = "";
                                return objresult;
                            }
                            objresult.Code = (int)HttpStatusCode.OK;
                            objresult.Msg = ap.Success;
                            //var user = result;
                            objresult.Data = AutoMapper.Mapper.Map<UserViewLoginModel>(result);
                            User_Master obj = result;
                            obj.SessionId = null;
                            obj.IsLogin = true;
                            obj.DeviceType = objtblusermaster.DeviceType;
                            obj.DeviceToken = objtblusermaster.DeviceToken;
                            obj.TimeZone = objtblusermaster.TimeZone;
                            db.Entry(obj).State = EntityState.Modified;
                            db.SaveChanges();
                            objresult.Data.Image = US.GetFavouriteImage(baseURL, result.Id);
                        }
                        else
                        {
                            objresult.Code = (int)HttpStatusCode.Accepted;
                            objresult.Msg = ap.LoginUserIsNotActive;
                            //var user = result;
                            objresult.Data = AutoMapper.Mapper.Map<UserViewLoginModel>(result);
                        }

                    }
                    else if (result == null)
                    {
                        objresult.Code = (int)HttpStatusCode.NotFound;
                        objresult.Msg = ap.LoginUserInvalid;
                        objresult.Data = model;
                        objresult.AccessToken = "";

                    }
                    else
                    {
                        objresult.Code = (int)HttpStatusCode.Accepted;
                        objresult.Msg = ap.LoginUserIsNotActive;
                        result.Image = US.GetFavouriteImage(baseURL, result.Id);

                        objresult.Data = AutoMapper.Mapper.Map<UserViewLoginModel>(result);
                    }

                    if (objresult.Code == (int)HttpStatusCode.OK)
                    {
                        TokenDetails objToken = await generatToken((result.Email), (result.Password), (result.DeviceToken));
                        (objresult.AccessToken) = objToken.access_token;
                        var obj = objresult.Data as UserViewLoginModel;
                        Add_UpdateToken(obj.Id, objToken, 1, objtblusermaster.DeviceType);
                    }
                    //if (updatetoken)
                    //{
                    //    (objresult.Data as UserViewModel).AccessToken = accessToken;
                    //}
                    //else
                    //{
                    //    (objresult.Data as UserViewModel).AccessToken = "";
                    //}
                    return objresult;
                }
            }
            catch (Exception ex)
            {
                objresult.Code = (int)HttpStatusCode.NotAcceptable;
                objresult.Msg = Convert.ToString(ex.Message);
                objresult.Data = "";
                objresult.AccessToken = "";
                return objresult;
            }
        }

        public dynamic GetforgotPassword(string email)
        {

            using (LystenEntities db = new LystenEntities())
            {
                User_Master obj = new User_Master();
                obj = db.User_Master.Where(x => x.Email == email).FirstOrDefault();
                if (obj != null)
                {
                    objresultCommon.Code = Convert.ToInt32(HttpStatusCode.OK);
                    objresultCommon.Msg = ap.ForgotMessage;
                }
                else
                {
                    objresultCommon.Code = Convert.ToInt32(HttpStatusCode.Created);
                    objresultCommon.Msg = ap.ForgotMessageNotExist;
                }
                return objresultCommon;
            }
        }

        public ResultClassToken GetLatestTockenByUserId(int userId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.AppAccessTokens.Where(x => x.UserId == userId).Select(x => x.AuthToken).FirstOrDefault();
                if (obj != null && obj != "")
                {
                    objresultToken.Code = (int)HttpStatusCode.OK;
                    objresultToken.Msg = ap.Success;
                    objresultToken.AccessToken = obj;
                }
                else
                {
                    objresultToken.Code = (int)HttpStatusCode.NotFound;
                    objresultToken.Msg = ap.UserNotExist;
                    objresultToken.AccessToken = "";
                }
                return objresultToken;
            }
        }

        public dynamic logoutUser(int UserId)
        {
            ResultClassCommon result = new ResultClassCommon();
            using (LystenEntities db = new LystenEntities())
            {
                int companyid = 0;
                var obj = db.User_Master.Where(x => x.Id == UserId).FirstOrDefault();

                if (obj != null)
                {
                    obj.IsLogin = false;
                    obj.DeviceToken = null;
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.GlobalError;
                }

            }
            return result;
        }

        public async Task<ResultClass> RegisterUser(User_Master um)
        {
            using (LystenEntities db = new LystenEntities())
            {
                if (string.IsNullOrEmpty(um.Password))
                {
                    um.Password = um.UserName + "@2018";
                }
                var pwd = SecutiryServices.EncodePasswordToBase64(um.Password);
                var result = (from um23 in db.User_Master
                              where um23.Email.ToUpper() == um.Email.ToUpper() && um23.Password == pwd
                              select um23
                             ).FirstOrDefault();

                um.Password = pwd;
                var model = new
                {
                };

                if (result != null)
                {
                    objresult.Code = (int)HttpStatusCode.Found;
                    objresult.Msg = ap.UserEMailExist;
                    objresult.Data = model;
                }
                else
                {
                    if (db.User_Master.Any(x => x.UserName.ToLower().Trim() == um.UserName.ToLower().Trim()))
                    {
                        objresult.Code = (int)HttpStatusCode.Found;
                        objresult.Msg = ap.UserNameExist;
                        objresult.Data = model;
                        (objresult.AccessToken) = "";
                        return objresult;
                    }
                    um.Displayname = um.FullName;
                    um.IsLogin = true;
                    um.IsDisclaimer = false;


                    um.RoleId = um.RoleId;

                    //um.PostalCode = um.PostalCode;
                    //um.Skill = um.Skill;
                    //um.SSN = um.SSN;
                    um.IsVerified = false;
                    //um.DateOfBirth = um.DateOfBirth;
                    //um.TimeZone = um.TimeZone;
                    //um.Address = um.Address;
                    //um.CityId = um.CityId;
                    //um.StateId = um.StateId;
                    //um.CountryId = um.CountryId;

                    db.User_Master.Add(um);
                    db.SaveChanges();
                    objresult.Code = (int)HttpStatusCode.OK;
                    objresult.Msg = ap.Success;
                    objresult.Data = AutoMapper.Mapper.Map<UserViewModel>(um);
                    objresult.Data.Favourite = new List<ProfileFavourite>() { };
                }
                        (objresult.AccessToken) = "";
                if (objresult.Code == (int)HttpStatusCode.OK)
                {
                    TokenDetails objToken = await generatToken(um.Email, um.Password, um.DeviceToken);
                    (objresult.AccessToken) = objToken.access_token;
                    var obj = AutoMapper.Mapper.Map<UserViewModel>(objresult.Data);
                    Add_UpdateToken(obj.Id, objToken, 1, um.DeviceType);
                }
                return objresult;
            }
        }

        public async Task<ResultClass> newRegisterUser(User_Master um)
        {
            using (LystenEntities db = new LystenEntities())
            {
                if (string.IsNullOrEmpty(um.Password))
                {
                    um.Password = um.UserName + "@2018";
                }
                var pwd = SecutiryServices.EncodePasswordToBase64(um.Password);
                var result = (from um23 in db.User_Master
                              where um23.Email.ToUpper() == um.Email.ToUpper() && um23.Password == pwd
                              select um23
                             ).FirstOrDefault();

                um.Password = pwd;
                var model = new
                {
                };

                if (result != null)
                {
                    objresult.Code = (int)HttpStatusCode.Found;
                    objresult.Msg = ap.UserEMailExist;
                    objresult.Data = model;
                }
                else
                {
                    if (db.User_Master.Any(x => x.UserName.ToLower().Trim() == um.UserName.ToLower().Trim()))
                    {
                        objresult.Code = (int)HttpStatusCode.Found;
                        objresult.Msg = ap.UserNameExist;
                        objresult.Data = model;
                        (objresult.AccessToken) = "";
                        return objresult;
                    }
                    um.Displayname = um.FullName;
                    um.DeviceType = um.DeviceType;
                    um.IsLogin = true;
                    um.IsDisclaimer = false;
                    um.RoleId = um.RoleId;

                    um.PostalCode = um.PostalCode;
                    um.Skill = um.Skill;
                    um.SSN = um.SSN;
                    um.IsVerified = false;
                    um.DateOfBirth = um.DateOfBirth;
                    um.TimeZone = um.TimeZone;
                    um.Address = um.Address;
                    um.CityId = um.CityId;
                    um.StateId = um.StateId;
                    um.CountryId = um.CountryId;

                    db.User_Master.Add(um);
                    db.SaveChanges();
                    objresult.Code = (int)HttpStatusCode.OK;
                    objresult.Msg = ap.Success;
                    objresult.Data = AutoMapper.Mapper.Map<UserViewModel>(um);
                    objresult.Data.Favourite = new List<ProfileFavourite>() { };
                }
                        (objresult.AccessToken) = "";
                if (objresult.Code == (int)HttpStatusCode.OK)
                {
                    TokenDetails objToken = await generatToken(um.Email, um.Password, um.DeviceToken);
                    (objresult.AccessToken) = objToken.access_token;
                    var obj = AutoMapper.Mapper.Map<UserViewModel>(objresult.Data);
                    Add_UpdateToken(obj.Id, objToken, 1, um.DeviceType);
                }
                return objresult;
            }
        }
        public async Task<String> SendEmail(string email)
        {
            String additionalProductDetails = string.Empty;
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.User_Master.Where(x => x.Email == email).Select(x => x.Password).FirstOrDefault();
                if (obj != null)
                {
                    var PASS = SecutiryServices.DecodeFrom64(obj);
                    es.SendUserForgotPassword(email, PASS);
                }
            }
            return additionalProductDetails;
        }

        public ResultClassToken ChangePassword(ChangePasswordModel cP, string token)
        {
            ResultClassToken rc = new ResultClassToken();
            using (LystenEntities db = new LystenEntities())
            {
                var pwd = SecutiryServices.EncodePasswordToBase64(cP.CurrentPassword);

                var data = db.User_Master.Where(x => x.Password == pwd && x.Id == cP.UserId).FirstOrDefault();
                if (data != null)
                {
                    data.Password = SecutiryServices.EncodePasswordToBase64(cP.NewPassword);
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                    rc.Code = (int)HttpStatusCode.OK;
                    rc.Msg = ap.Success;
                    //objresult.Data = model;
                }
                else
                {
                    rc.Code = (int)HttpStatusCode.NotFound;
                    rc.Msg = ap.CurrentPaasswordNotSame;
                    //objresult.Data = model;
                }
            }
            return rc;
        }
    }
}