using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using LystenApi.ActionFilters;
using LystenApi.Utility.ApiServices;
using LystenApi.Utility;
using LystenApi.Utility.Providers;
using System.Web.Http.Results;
using LystenApi.Db;
using LystenApi.ViewModel;
using System.IO;
using LystenApi.Models;
using api.ActionFilters;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Web.Configuration;
using System.Security.Principal;
using System.Net.Mime;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data.SqlClient;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;
using PushSharp.Apple;
using PushSharp.Core;
using Newtonsoft.Json.Linq;
using Microsoft.Owin;

namespace LystenApi.Controllers.Api
{
    [AuthorizationRequired]
    [RoutePrefix("api/v1/user")]
    public class UserController : BaseApiController
    {
        ApiUserServices ApiUser = new ApiUserServices();
        ApiMessageFormat ap = new ApiMessageFormat();
        ApiUserServices US = new ApiUserServices();

        ApiException ApiEx = new ApiException();
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        public void sendMsgEvent(int EventID, int Id, string devicetocken)
        {
            string ImagePath = "";
            string name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                ImagePath = ApiUser.GetFavouriteImage(baseURL, Id);
                name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
            }

            int port = 2195;
            String hostname = (WebConfigurationManager.AppSettings["ApnsEnvironment"]);
            //String hostname = "gateway.push.apple.com";

            string certificatePath = System.Web.HttpContext.Current.Server.MapPath("~/Lysten-DevB.p12");

            string certificatePassword = "";

            X509Certificate2 clientCertificate = new X509Certificate2(certificatePath, certificatePassword, X509KeyStorageFlags.MachineKeySet);
            X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);


            TcpClient client = new TcpClient(hostname, port);

            SslStream sslStream = new SslStream(
                            client.GetStream(),
                            false,
                            new RemoteCertificateValidationCallback(ValidateServerCertificate),
                            null
            );
            try
            {
                sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Tls, false);
            }
            catch (AuthenticationException ex)
            {
                client.Close();
                Exception Eccsssas12 = new Exception("Athentication Failed");
                CommonServices.ErrorLogging(Eccsssas12);
                System.Web.HttpContext.Current.Server.MapPath("~/Authenticationfailed.txt");
                return;
            }

            //// Encode a test message into a byte array.
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);
            writer.Write((byte)0);  //The command
            writer.Write((byte)0);  //The first byte of the deviceId length (big-endian first byte)
            writer.Write((byte)32); //The deviceId length (big-endian second byte)
            byte[] b0 = HexString2Bytes(devicetocken);
            WriteMultiLineByteArray(b0);
            writer.Write(b0);
            String payload;
            string strmsgbody = "";
            int totunreadmsg = 20;
            strmsgbody = "A new event has been created!";
            payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"event\":{\"EventID\":\"" + EventID + "\"}";
            writer.Write((byte)0); //First byte of payload length; (big-endian first byte)
            writer.Write((byte)payload.Length);     //payload length (big-endian second byte)
            byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
            writer.Write(b1);
            writer.Flush();

            byte[] array = memoryStream.ToArray();
            try
            {
                sslStream.Write(array);
                sslStream.Flush();
            }
            catch
            {
            }
            client.Close();
        }

        [AuthorizationRequired]
        [HttpGet]
        [Route("SendEventNotification")]
        // POST api/<controller>
        public async Task<IHttpActionResult> SendEventNotification(int EventId,int IsAdded)
        {
            try
            {
                ResultClass result = new ResultClass();
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                using (LystenEntities db = new LystenEntities())
                {
                    var UserList = db.User_Master.Where(x => x.IsActive == true && x.RoleId != 1 && x.DeviceToken != null).ToList();
                    var Eventname = db.Events.Where(x => x.Id == EventId).FirstOrDefault();
                    foreach (var item in UserList)
                    {
                        if (item.DeviceToken != null)
                        {
                            if (item.DeviceType == "Android")
                            {
                                Helpers.NotificationHelper.SendMsgEventPushNotification(Eventname.CategoryId.Value, item.Id, item.DeviceToken, IsAdded, Eventname.Title);
                            }
                            else
                            {
                                sendMsgEventsssss(Eventname.CategoryId.Value, item.Id, item.DeviceToken, IsAdded, Eventname.Title);
                            }
                        }
                        //sendMsgEvent(EventId, item.Id, item.DeviceToken);
                    }
                }

                result.Code = (int)HttpStatusCode.OK;
                result.Msg = ap.Success;
                result.Data = "";
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
                throw ex;
            }
        }





        //[AuthorizationRequired]
        //[HttpGet]
        //[Route("AddtoOffline")]
        //// POST api/<controller>
        //public async Task<IHttpActionResult> AddtoOffline()
        //{
        //    try
        //    {
        //        ResultClass result = new ResultClass();
        //        var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
        //        var userdata = ApiUser.AddtoOffline();
        //        if (userdata != null)
        //        {
        //            result.Code = (int)HttpStatusCode.OK;
        //            result.Msg = ap.Success;
        //            result.Data = userdata;
        //        }
        //        else
        //        {
        //            result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
        //            result.Msg = ap.GlobalError;
        //            result.Data = userdata;
        //        }
        //        if (updatetoken)
        //        {
        //            token = result.AccessToken = accessToken;
        //        }
        //        else
        //        {
        //            result.AccessToken = "";
        //        }
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [AuthorizationRequired]
        [HttpPost]
        [Route("ReportUser")]
        // POST api/<controller>
        public async Task<IHttpActionResult> ReportUser(ReportUserViewModel RUVM)
        {
            try
            {
                ResultClassToken result = new ResultClassToken();
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
                var userdata = ApiUser.ReportUser(RUVM);
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
                throw ex;
            }
        }

        [AuthorizationRequired]
        [HttpGet]
        [Route("GetOnlineUser")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetOnlineUser(int Count, string Search, int UserId)
        {
            try
            {
                ResultClass result = new ResultClass();
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
                var userdata = ApiUser.GetOnlineUser(Count, Search, UserId);
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
                throw ex;
            }
        }

        [AuthorizationRequired]
        [HttpGet]
        [Route("GetAllUsers")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetAllUsers(int GroupId)
        {
            ResultClass result = new ResultClass();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiUser.GetAllUsers(GroupId);
                if (mm.Count > 0)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.UserNodata;
                    result.Data = mm; result.Data = mm;
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
        [Route("GetDashboardData")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetDashboardData(int UserId, int CategoryId, int Count)
        {
            ResultClass result = new ResultClass();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiUser.GetDashboardData(UserId, CategoryId, Count);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.AlreadyFavourite;
                    result.Data = mm; result.Data = mm;
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
        [Route("GetQuestionsByCategory")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetQuestionsByCategory(int Categoryid, int Count, int UserId)
        {
            ResultClass result = new ResultClass();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiUser.GetQuestionsByCategory(Categoryid, Count, UserId);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.AlreadyFavourite;
                    result.Data = mm; result.Data = mm;
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
        [Route("GetTopicByUsers")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetTopicByUsers(int UserId)
        {
            ResultClass result = new ResultClass();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiUser.GetTopicByUser(UserId);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.AlreadyFavourite;
                    result.Data = mm; result.Data = mm;
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
        [Route("userprofile")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetUserDetail(int UserId)
        {
            try
            {
                ResultClass result = new ResultClass();

                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                var userdata = await ApiUser.GetUserProfile(UserId);
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
                throw ex;
            }
        }

        [AuthorizationRequired]
        [HttpGet]
        [Route("userprofileview")]
        // POST api/<controller>
        public async Task<IHttpActionResult> userprofileview(int UserId, int VUserId, int Count)
        {
            try
            {
                ResultClass result = new ResultClass();

                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                var userdata = ApiUser.userprofileview(UserId, VUserId, Count);
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
                throw ex;
            }
        }

        [Route("updateprofile")]
        [HttpPost]
        public async Task<IHttpActionResult> PostFile()
        {
            ResultClass result = new ResultClass();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                User_Master Um = new User_Master()
                {
                    Email = HttpContext.Current.Request.Params["Email"],
                    UserName = HttpContext.Current.Request.Params["UserName"],
                    Password = HttpContext.Current.Request.Params["Password"],
                    Phone = HttpContext.Current.Request.Params["Phone"],
                    Age = Convert.ToInt16(HttpContext.Current.Request.Params["Age"]),
                    Gender = Convert.ToInt16(HttpContext.Current.Request.Params["Gender"]),
                    Id = Convert.ToInt16(HttpContext.Current.Request.Params["Id"]),
                    IsActive = true,
                    Createdate = System.DateTime.Now,
                    Createdby = 1
                };
                int iUploadedCnt = 0;

                // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["userimagepath"]);

                bool exists = System.IO.Directory.Exists(sPath);

                if (!exists)
                    System.IO.Directory.CreateDirectory(sPath);
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

                // CHECK THE FILE COUNT.
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];

                    if (hpf.ContentLength > 0)
                    {
                        string ImagePath = Um.Id + "_" + hpf.FileName;
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
                    }
                }
                result = await ApiUser.SaveUser(Um);
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

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        [AuthorizationRequired]
        [HttpPost]
        [Route("AddUserTopic")]
        // POST api/<controller>
        public async Task<IHttpActionResult> AddUserTopic(TopicViewModel UM)
        {
            ResultClassToken result = new ResultClassToken();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiUser.AddUserTopic(UM);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.CategoryNoData;
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

        public void sendMsg(int Id, string devicetocken,int QuestionId,string username)
        {
            string ImagePath = "";
            string name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                ImagePath = US.GetFavouriteImage(baseURL, Id);
                name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
            }

            int port = 2195;
            String hostname = (WebConfigurationManager.AppSettings["ApnsEnvironment"]);
            //String hostname = "gateway.push.apple.com";

            string certificatePath = System.Web.HttpContext.Current.Server.MapPath("~/Lysten-DevB.p12");

            string certificatePassword = "";

            X509Certificate2 clientCertificate = new X509Certificate2(certificatePath, certificatePassword, X509KeyStorageFlags.MachineKeySet);
            X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);


            TcpClient client = new TcpClient(hostname, port);

            SslStream sslStream = new SslStream(
                            client.GetStream(),
                            false,
                            new RemoteCertificateValidationCallback(ValidateServerCertificate),
                            null
            );
            try
            {
                sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Tls, false);
            }
            catch (AuthenticationException ex)
            {
                client.Close();
                Exception Eccsssas12 = new Exception("Athentication Failed");
                CommonServices.ErrorLogging(Eccsssas12);
                System.Web.HttpContext.Current.Server.MapPath("~/Authenticationfailed.txt");
                return;
            }

            //// Encode a test message into a byte array.
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);
            writer.Write((byte)0);  //The command
            writer.Write((byte)0);  //The first byte of the deviceId length (big-endian first byte)
            writer.Write((byte)32); //The deviceId length (big-endian second byte)
            byte[] b0 = HexString2Bytes(devicetocken);
            WriteMultiLineByteArray(b0);
            writer.Write(b0);
            String payload;
            string strmsgbody = "";
            int totunreadmsg = 20;

            strmsgbody = username + " subscribed your Question.";

            //payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"nav\":{\"image\":\"" + ImagePath + "\",\"name\":\"" + name + "\"},\"acme1\":\"bar\",\"acme2\":42}";
            payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"ans\":{\"QuestionID\":\"" + QuestionId + "\"},}";

            writer.Write((byte)0); //First byte of payload length; (big-endian first byte)
            writer.Write((byte)payload.Length);     //payload length (big-endian second byte)
            byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
            writer.Write(b1);
            writer.Flush();

            byte[] array = memoryStream.ToArray();
            try
            {
                sslStream.Write(array);
                sslStream.Flush();
            }
            catch
            {
            }
            client.Close();
        }





        public void sendMsgEventsssss(int EventId, int Id, string devicetocken,int IsAdded,string Eventname)
        {
            string ImagePath = "";
            string name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                ImagePath = US.GetFavouriteImage(baseURL, Id);
                name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
            }

            int port = 2195;
            String hostname = (WebConfigurationManager.AppSettings["ApnsEnvironment"]);
            //String hostname = "gateway.push.apple.com";

            string certificatePath = System.Web.HttpContext.Current.Server.MapPath("~/Lysten-DevB.p12");

            string certificatePassword = "";

            X509Certificate2 clientCertificate = new X509Certificate2(certificatePath, certificatePassword, X509KeyStorageFlags.MachineKeySet);
            X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);


            TcpClient client = new TcpClient(hostname, port);

            SslStream sslStream = new SslStream(
                            client.GetStream(),
                            false,
                            new RemoteCertificateValidationCallback(ValidateServerCertificate),
                            null
            );
            try
            {
                sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Tls, false);
            }
            catch (AuthenticationException ex)
            {
                client.Close();
                Exception Eccsssas12 = new Exception("Athentication Failed");
                CommonServices.ErrorLogging(Eccsssas12);
                System.Web.HttpContext.Current.Server.MapPath("~/Authenticationfailed.txt");
                return;
            }

            //// Encode a test message into a byte array.
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);
            writer.Write((byte)0);  //The command
            writer.Write((byte)0);  //The first byte of the deviceId length (big-endian first byte)
            writer.Write((byte)32); //The deviceId length (big-endian second byte)
            byte[] b0 = HexString2Bytes(devicetocken);
            WriteMultiLineByteArray(b0);
            writer.Write(b0);
            String payload;
            string strmsgbody = "";

            if (IsAdded == 1)
            {
                strmsgbody = "Hey!!Let’s check" + Eventname + " is published.";
            }
            else
            {
                strmsgbody = "Hey!!" + Eventname + " is updated.";
            }
            int totunreadmsg = 20;
            payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"event\":{\"EventID\":\"" + EventId + "\"},\"acme1\":\"bar\",\"acme2\":42}";
            writer.Write((byte)0); //First byte of payload length; (big-endian first byte)
            writer.Write((byte)payload.Length);     //payload length (big-endian second byte)
            byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
            writer.Write(b1);
            writer.Flush();

            byte[] array = memoryStream.ToArray();
            try
            {
                sslStream.Write(array);
                sslStream.Flush();
            }
            catch
            {
            }
            client.Close();
        }



        private byte[] HexString2Bytes(string hexString)
        {
            //check for null
            if (hexString == null) return null;
            //get length
            int len = hexString.Length;
            if (len % 2 == 1) return null;
            int len_half = len / 2;
            //create a byte array
            byte[] bs = new byte[len_half];
            try
            {
                //convert the hexstring to bytes
                for (int i = 0; i != len_half; i++)
                {
                    bs[i] = (byte)Int32.Parse(hexString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Exception : " + ex.Message);
            }
            //return the byte array
            return bs;
        }
        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
        public static void WriteMultiLineByteArray(byte[] bytes)
        {
            const int rowSize = 20;
            int iter;

            Console.WriteLine("initial byte array");
            Console.WriteLine("------------------");

            for (iter = 0; iter < bytes.Length - rowSize; iter += rowSize)
            {
                Console.Write(
                    BitConverter.ToString(bytes, iter, rowSize));
                Console.WriteLine("-");
            }

            Console.WriteLine(BitConverter.ToString(bytes, iter));
            Console.WriteLine();
        }

        [AuthorizationRequired]
        [HttpPost]
        [Route("subscribequestion")]
        // POST api/<controller>
        public async Task<IHttpActionResult> subscribequestion(QueAnsViewModel UM)
        {


            ResultClassToken result = new ResultClassToken();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {


                using (LystenEntities db = new LystenEntities())
                {
                    var obj = db.Questions.Where(x => x.Id == UM.QuestionId).FirstOrDefault();
                    var username = db.User_Master.Where(x => x.Id == UM.UserId).Select(x=>x.FullName).FirstOrDefault();
                    if (obj != null)
                    {
                        if (!db.UserQuestionSubscribes.Any(x => x.UserId == UM.UserId && x.QuestionsId == UM.QuestionId))
                        {
                            if (obj.User_Master.DeviceToken != null && obj.User_Master.DeviceToken != "")
                            {
                                if (obj.User_Master.DeviceType == "Android")
                                {
                                    var _id = obj.UserId.Value;
                                    Helpers.NotificationHelper.SendPushNotificationToSubscribe(_id, obj.User_Master.DeviceToken, obj.Id, username);
                                }
                                else
                                {
                                    sendMsg(obj.Id, obj.User_Master.DeviceToken, obj.Id, username);
                                }
                            }
                        }
                    }

                }




                var mm = ApiUser.subscribequestion(UM);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.CategoryNoData;
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
                CommonServices.ErrorLogging(ex);
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        [AuthorizationRequired]
        [HttpPost]
        [Route("askquestion")]
        // POST api/<controller>
        public async Task<IHttpActionResult> askquestion(QueAnsViewModel UM)
        {
            ResultClassToken result = new ResultClassToken();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiUser.askquestion(UM);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.CategoryNoData;
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
        [Route("gettopicbyuser")]
        // POST api/<controller>
        public async Task<IHttpActionResult> gettopicbyuser(int UserId)
        {
            ResultClass result = new ResultClass();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiUser.gettopicbyuser(UserId);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.CategoryNoData;
                    result.Data = mm; result.Data = mm;
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
        [Route("addfavourite")]
        // POST api/<controller>
        public async Task<IHttpActionResult> addfavourite(FavouriteViewModel FM)
        {
            ResultClass result = new ResultClass();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiUser.addfavourite(FM);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.AlreadyFavourite;
                    result.Data = mm; result.Data = mm;
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
        [Route("sendanswer")]
        // POST api/<controller>
        public async Task<IHttpActionResult> sendanswer(QueAnsViewModel UM)
        {
            ResultClassToken result = new ResultClassToken();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiUser.sendanswer(UM);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.CategoryNoData;
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

        /// <summary>
        /// Auther : Vatsal Bariya
        /// Created Date: 04/12/2017
        /// For the Upload profile Pic of the Customer
        /// </summary>
        /// <returns></returns>
        /// 
        [AuthorizationRequired]
        [Route("updateprofilePic")]
        [HttpPost]
        public async Task<IHttpActionResult> PostProfilePic()
        {
            ResultClass result = new ResultClass();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                User_Master Um = new User_Master()
                {

                    Id = Convert.ToInt16(HttpContext.Current.Request.Params["Id"]),
                    //IsActive = true,
                    Modifydate = System.DateTime.Now,
                    //Createdby = 1
                };
                int iUploadedCnt = 0;

                // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["userimagepath"]);

                bool exists = System.IO.Directory.Exists(sPath);

                if (!exists)
                    System.IO.Directory.CreateDirectory(sPath);
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

                // CHECK THE FILE COUNT.
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];

                    if (hpf.ContentLength > 0)
                    {
                        string ImagePath = Um.Id + "_" + hpf.FileName;
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
                    }
                }


                result = await ApiUser.UpdateProfilePic(Um);
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

        /// <summary>
        /// Auther : Vatsal Bariya
        /// Created Date: 04/12/2017
        /// For the Update the Profile Information of the User
        /// </summary>
        /// <returns></returns>
        [Route("updateprofileInfo")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateProfileInfo()
        {
            ResultClass result = new ResultClass(); //result = new User_Master();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                User_Master Um = new User_Master()
                {
                    //Email = HttpContext.Current.Request.Params["Email"],
                    Displayname = HttpContext.Current.Request.Params["DisplayName"],
                    Password = HttpContext.Current.Request.Params["Password"],
                    Phone = HttpContext.Current.Request.Params["Phone"],
                    Age = Convert.ToInt16(HttpContext.Current.Request.Params["Age"]),
                    Gender = Convert.ToInt16(HttpContext.Current.Request.Params["Gender"]),
                    Id = Convert.ToInt16(HttpContext.Current.Request.Params["Id"]),
                    CollegeUniversity = HttpContext.Current.Request.Params["CollegeUniversity"],
                    Degree = HttpContext.Current.Request.Params["Degree"],
                    Occupation = HttpContext.Current.Request.Params["Occupation"],
                    CountryId = Convert.ToInt32(HttpContext.Current.Request.Params["CountryId"]),
                    StateId = Convert.ToInt32(HttpContext.Current.Request.Params["StateId"]),
                    CityId = Convert.ToInt32(HttpContext.Current.Request.Params["CityId"]),
                    FullName = (HttpContext.Current.Request.Params["FullName"])
                    //IsActive = true,
                    //Createdate = System.DateTime.Now,
                    //Createdby = 1
                };

                result = await ApiUser.UpdateInfo(Um);
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

        /// <summary>
        /// Auther : Vatsal Bariya
        /// Created Date: 04/12/2017
        /// TO Get Online User Information 
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [AuthorizationRequired]
        [HttpGet]
        [Route("GetUserInfoById")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetUserById(int UserId)
        {
            try
            {
                ResultClass result = new ResultClass();
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
                var userdata = ApiUser.GetOnlineUserById(UserId);
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
                throw ex;
            }
        }

        /// <summary>
        /// Auther : Vatsal Bariya
        /// Created Date: 04/12/2017
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        [AuthorizationRequired]
        [HttpGet]
        [Route("GetGroupInfo")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetGroupbyUserId(int UserId, int GroupId)
        {
            ResultClass result = new ResultClass();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiUser.GetGroupInfobyUserAndGroupId(UserId, GroupId);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Msg = "No Such group found for the User";
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
        [Route("GetAllUsersList")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetAllUserss(int Count, int UserId)
        {
            try
            {
                ResultClass result = new ResultClass();
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
                var userdata = ApiUser.GetAllUsersList(Count, UserId);
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
                throw ex;
            }
        }



        [AuthorizationRequired]
        [HttpGet]
        [Route("GetUsersCallingRequested")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetUsersCallingRequested(int UserId)
        {
            ResultClass result = new ResultClass();
            try
            {
               
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
                var userdata = ApiUser.GetUsersCallingRequested(UserId);
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
                result.Code = (int)HttpStatusCode.NotFound;
                result.Msg = ex.Message;
                result.Data = "";
                result.AccessToken = "";
                return Ok(result);
            }
        }


        [AuthorizationRequired]
        [HttpGet]
        [Route("GetCallingUser")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetCallingUser(int UserId)
        {
            try
            {
                ResultClass result = new ResultClass();
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
                var userdata = ApiUser.GetUserDetail(UserId);
                if (userdata != null)
                {
                    //if (!string.IsNullOrEmpty(userdata.DeviceToken))
                    //    CallingNotification(UserId, userdata.DeviceToken);
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
                throw ex;
            }
        }


        #region CallingNotification 
        public void CallingNotification(int Id, string devicetocken)
        {
            string ImagePath = "";
            string name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                ImagePath = US.GetFavouriteImage(baseURL, Id);
                name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
            }

            int port = 2195;
            String hostname = (WebConfigurationManager.AppSettings["ApnsEnvironment"]);
            //String hostname = "gateway.push.apple.com";

            string certificatePath = System.Web.HttpContext.Current.Server.MapPath("~/Lysten-DevB.p12");

            string certificatePassword = "";

            X509Certificate2 clientCertificate = new X509Certificate2(certificatePath, certificatePassword, X509KeyStorageFlags.MachineKeySet);
            X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);


            TcpClient client = new TcpClient(hostname, port);

            SslStream sslStream = new SslStream(
                            client.GetStream(),
                            false,
                            new RemoteCertificateValidationCallback(ValidateServerCertificate),
                            null
            );
            try
            {
                sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Tls, false);
            }
            catch (AuthenticationException ex)
            {
                client.Close();
                Exception Eccsssas12 = new Exception("Athentication Failed");
                CommonServices.ErrorLogging(Eccsssas12);
                System.Web.HttpContext.Current.Server.MapPath("~/Authenticationfailed.txt");
                return;
            }

            //// Encode a test message into a byte array.
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);
            writer.Write((byte)0);  //The command
            writer.Write((byte)0);  //The first byte of the deviceId length (big-endian first byte)
            writer.Write((byte)32); //The deviceId length (big-endian second byte)
            byte[] b0 = HexString2Bytes(devicetocken);
            WriteMultiLineByteArray(b0);
            writer.Write(b0);
            String payload;
            string strmsgbody = "";
            int totunreadmsg = 20;
            strmsgbody = "Incoming Call from  " + name;
            payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"nav\":{\"image\":\"" + ImagePath + "\",\"name\":\"" + name + "\"},\"acme1\":\"bar\",\"acme2\":42}";
            writer.Write((byte)0); //First byte of payload length; (big-endian first byte)
            writer.Write((byte)payload.Length);     //payload length (big-endian second byte)
            byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
            writer.Write(b1);
            writer.Flush();

            byte[] array = memoryStream.ToArray();
            try
            {
                sslStream.Write(array);
                sslStream.Flush();
            }
            catch
            {
            }
            client.Close();
        }

        #endregion
        //[AuthorizationRequired]
        //[HttpGet]
        //[Route("GetGroupListData")]
        //// POST api/<controller>
        //public async Task<IHttpActionResult> GetGroupListData(int UserId)
        //{
        //    ResultClass result = new ResultClass();
        //    var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
        //    try
        //    {
        //        var mm = ApiUser.GetGroupListDatas(UserId);
        //        if (mm != null)
        //        {
        //            result.Code = (int)HttpStatusCode.OK;
        //            result.Msg = ap.Success;
        //            result.Data = mm;
        //        }
        //        else
        //        {
        //            result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
        //            result.Msg = ap.AlreadyFavourite;
        //            result.Data = mm; result.Data = mm;
        //        }
        //        if (updatetoken)
        //        {
        //            token = result.AccessToken = accessToken;
        //        }
        //        else
        //        {
        //            result.AccessToken = "";
        //        }
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ApiEx.FireException(result, ex));
        //    }
        //}
    }
}