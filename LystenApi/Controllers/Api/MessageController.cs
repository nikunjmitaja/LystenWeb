using LystenApi.ActionFilters;
using LystenApi.Utility;
using LystenApi.Utility.ApiServices;
using LystenApi.Utility.Providers;
using LystenApi.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace LystenApi.Controllers.Api
{
    [AuthorizationRequired]
    [RoutePrefix("api/v1/message")]
    public class MessageController : BaseApiController
    {
        ApiMessageServices ApiMessage = new ApiMessageServices();
        ApiMessageFormat ap = new ApiMessageFormat();
        ApiException ApiEx = new ApiException();
        ApiUserServices ApiUser = new ApiUserServices();


        [AuthorizationRequired]
        [HttpPost]
        [Route("AddUserToGroup")]
        // POST api/<controller>
        public async Task<IHttpActionResult> AddUserToGroup(UserGroupViewModel GVM)
        {
            ResultClassToken result = new ResultClassToken();

            try
            {

                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                var mm = ApiMessage.AddUserToGroup(GVM);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.MessageNoData;
                }
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
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        [AuthorizationRequired]
        [HttpPost]
        [Route("CreateGroup")]
        // POST api/<controller>
        public async Task<IHttpActionResult> CreateGroup()
        {
            ResultClass result = new ResultClass();

            try
            {

                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                GroupViewModel GVM = new GroupViewModel();
                GVM.Name = HttpContext.Current.Request.Params["Name"];
                GVM.GroupTypeId = HttpContext.Current.Request.Params["GroupTypeId"];
                GVM.CreatorId = HttpContext.Current.Request.Params["CreatorId"];
                GVM.CategoryId = HttpContext.Current.Request.Params["CategoryId"];


                int iUploadedCnt = 0;

                // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["groupimagepath"]);

                bool exists = System.IO.Directory.Exists(sPath);

                if (!exists)
                    System.IO.Directory.CreateDirectory(sPath);
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;


                var mm = ApiMessage.CreateGroup(GVM);



                // CHECK THE FILE COUNT.
                if (mm != null)
                {

                    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                    {
                        System.Web.HttpPostedFile hpf = hfc[iCnt];

                        if (hpf.ContentLength > 0)
                        {
                            // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                            string ImagePath = mm.Id + "_" + hpf.FileName;

                            // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                            if (!File.Exists(sPath + Path.GetFileName(ImagePath)))
                            {
                                // SAVE THE FILES IN THE FOLDER.
                                hpf.SaveAs(sPath + Path.GetFileName(ImagePath));
                                GVM.Image = ImagePath;
                                iUploadedCnt = iUploadedCnt + 1;
                            }
                            else
                            {
                                File.Delete(sPath + Path.GetFileName(ImagePath));
                                hpf.SaveAs(sPath + Path.GetFileName(ImagePath));
                                GVM.Image = ImagePath;
                                iUploadedCnt = iUploadedCnt + 1;
                            }
                            GVM.Id = mm.Id;
                            result = ApiUser.UpdateGroupPic(GVM);

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
                    }
                }


                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.MessageNoData;
                    result.Data = mm;
                }
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
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }


        [AuthorizationRequired]
        [HttpPost]
        [Route("UpdateGroup")]
        // POST api/<controller>
        public async Task<IHttpActionResult> UpdateGroup()
        {
            ResultClass result = new ResultClass();
            try
            {
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
                GroupViewModel GVM = new GroupViewModel();
                GVM.Name = HttpContext.Current.Request.Params["Name"];
                GVM.Id = Convert.ToInt32(HttpContext.Current.Request.Params["Id"]);
                int iUploadedCnt = 0;
                // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["groupimagepath"]);
                bool exists = System.IO.Directory.Exists(sPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(sPath);
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                // CHECK THE FILE COUNT.
                if (GVM.Id != 0)
                {

                }
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    if (hpf.ContentLength > 0)
                    {
                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                        string ImagePath = GVM.Id + "_" + hpf.FileName;
                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                        if (!File.Exists(sPath + Path.GetFileName(ImagePath)))
                        {
                            // SAVE THE FILES IN THE FOLDER.
                            hpf.SaveAs(sPath + Path.GetFileName(ImagePath));
                            GVM.Image = ImagePath;
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                        else
                        {
                            File.Delete(sPath + Path.GetFileName(ImagePath));
                            hpf.SaveAs(sPath + Path.GetFileName(ImagePath));
                            GVM.Image = ImagePath;
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                        GVM.Id = GVM.Id;
                       var mm1 = ApiMessage.UpdateGroup(GVM);
                        if (mm1 != null)
                        {
                            result.Code = (int)HttpStatusCode.OK;
                            result.Msg = ap.Success;
                            result.Data = mm1;
                        }
                        else
                        {
                            result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                            result.Msg = ap.MessageNoData;
                            result.Data = mm1;
                        }
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
                }
                var mm = ApiMessage.UpdateGroup(GVM);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.MessageNoData;
                    result.Data = mm;
                }
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
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        public class MessageRequestModel
        {
            public int UserId { get; set; }
            public int MRUserId { get; set; }

            public int IsAccept { get; set; }

        }

        [AuthorizationRequired]
        [HttpPost]
        [Route("messagerequest")]
        // POST api/<controller>
        public async Task<IHttpActionResult> messagerequest(MessageRequestModel MM)
        {
            try
            {
                ResultClassToken result = new ResultClassToken();

                var userdata = ApiMessage.messagerequest(MM);
                if (userdata != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.GlobalError;
                }
                if (updatetoken)
                {
                    result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        [HttpGet]
        [Route("getinboxmessage")]
        // POST api/<controller>
        public async Task<IHttpActionResult> getinboxmessage(int UserId,int Count)
        {
            try
            {
                ResultClass result = new ResultClass();

                var userdata = ApiMessage.getinboxmessage(UserId, Count);
                if (userdata != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = userdata;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.GlobalError;
                    result.Data = userdata;
                }
                if (updatetoken)
                {
                    result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        [HttpPost]
        [Route("getmessage")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetMessages([FromBody] MessageModel MM)
        {
            try
            {
                ResultClass result = new ResultClass();

                var userdata = ApiMessage.GetMessages(MM);
                if (userdata != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = userdata;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.GlobalError;
                    result.Data = userdata;
                }
                if (updatetoken)
                {
                    result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("sendmessage")]
        // POST api/<controller>
        public async Task<IHttpActionResult> SendMessage([FromBody] MessageModel MM)
        {
            try
            {
                ResultClass result = new ResultClass();

                var userdata = ApiMessage.SendMessage(MM);
                if (userdata != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = userdata;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.GlobalError;
                    result.Data = userdata;
                }
                if (updatetoken)
                {
                    result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public class MessageModel
        {
            public int MessageId { get; set; }
            public string Body { get; set; }
            public int? RecipientId { get; set; }
            public int? RecipientGroupId { get; set; }
            public int? CreatorId { get; set; }
            public bool IsRead { get; set; }
            public int? ParentMessageId { get; set; }
            public DateTime? ExpiryDate { get; set; }
            public string CreatedDate { get; set; }
            public int Counter { get; set; }
            public string Image { get; set; }

            [AutoMapper.IgnoreMap]
            public string CreatorName { get; set; }
        }


        /// <summary>
        /// Auther: Vatsal Bariya
        /// Date: 4/12/2017
        /// To gat the Cateogry using User Id
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [AuthorizationRequired]
        [HttpGet]
        [Route("GetGroupCategory")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetCategoryList(int UserId)
        {
            try
            {
                ResultClass result = new ResultClass();

                var userdata = ApiMessage.GetCategoryList(UserId);
                if (userdata != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = userdata;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.GroupNodata;
                    result.Data = userdata;
                }
                if (updatetoken)
                {
                    result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Auther : Vatsal Bariya
        /// Created Date: 04/12/2017
        /// For the Group Discusssion 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        [AuthorizationRequired]
        [HttpGet]
        [Route("GetGroupDiscussion")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetGroupbyUserId(int UserId, int GroupId)
        {
            ResultClass result = new ResultClass();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiMessage.GetGroupInfobyUserAndGroupId(UserId, GroupId);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.LoginUserInvalid;
                    result.Data = mm;
                }
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
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        [AuthorizationRequired]
        [HttpGet]
        [Route("GetGroupMembersList")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetGroupMembers(int GroupId)
        {
            ResultClass result = new ResultClass();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiMessage.GetGroupMembers(GroupId);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.LoginUserInvalid;
                    result.Data = mm;
                }
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
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }
    }
}
