using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Web.WebSockets;
using LystenApi.Db;
using System.Data;
using LystenApi.Utility;
using System.Web.Configuration;
using static LystenApi.Controllers.Api.MessageController;
using System.Net;
using LystenApi.Utility.Providers;
using LystenApi.Utility.ApiServices;
using System.Web.Script.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using NodaTime;

namespace LystenApi
{
    public class MyWebSocketHandle : WebSocketHandler
    {
        private static WebSocketCollection clients = new WebSocketCollection();
        ApiMessageFormat ap = new ApiMessageFormat();
        private string name;
        ApiUserServices US = new ApiUserServices();

        public override void OnOpen()
        {
            try
            {
                clients.Add(this);
                if (this.WebSocketContext.RequestUri.Query.Split(',')[1].Split(':').Contains("GroupId"))
                {

                    var GroupId = this.WebSocketContext.RequestUri.Query.Split(',')[1].Split(':')[1];
                    var UserId = this.WebSocketContext.RequestUri.Query.Split(',')[0].Split(':')[1];
                    using (LystenEntities db = new LystenEntities())
                    {
                        int gid = Convert.ToInt32(GroupId);
                        int uid = Convert.ToInt32(UserId);

                        //var obj = db.Groups.Where(x => x.Id == gid).FirstOrDefault();
                        //if (obj.GroupTypeId == 1)
                        //{
                        //    if (!db.Groups.Any(x => x.CreatorId == uid && x.Id == gid))
                        //    {
                        //        if (!db.UserGroupMappings.Any(x => x.UserId == uid && x.GroupId == gid))
                        //        {
                        //            UserGroupMapping ugm = new UserGroupMapping()
                        //            {
                        //                GroupId = gid,
                        //                UserId = uid
                        //            };
                        //            db.UserGroupMappings.Add(ugm);
                        //            db.SaveChanges();
                        //        }
                        //    }
                        //}
                        WebSocketSessionDb webs = new WebSocketSessionDb();

                        if (db.WebSocketSessionDbs.Any(x => x.GroupID == gid && x.UserId == uid))
                        {
                            webs = db.WebSocketSessionDbs.Where(x => x.GroupID == gid && x.UserId == uid).FirstOrDefault();
                            webs.GroupID = Convert.ToInt32(GroupId);
                            webs.UserId = Convert.ToInt32(UserId);
                            webs.SessionId = this.WebSocketContext.SecWebSocketKey;
                            db.Entry(webs).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            webs.GroupID = Convert.ToInt32(GroupId);
                            webs.UserId = Convert.ToInt32(UserId);
                            webs.SessionId = this.WebSocketContext.SecWebSocketKey;
                            db.WebSocketSessionDbs.Add(webs);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {

                    var ReceiptionId = this.WebSocketContext.RequestUri.Query.Split(',')[1].Split(':')[1];
                    var UserId = this.WebSocketContext.RequestUri.Query.Split(',')[0].Split(':')[1];
                    using (LystenEntities db = new LystenEntities())
                    {
                        int rid = Convert.ToInt32(ReceiptionId);
                        int uid = Convert.ToInt32(UserId);


                        WebSocketSessionDb webs = new WebSocketSessionDb();

                        if (db.WebSocketSessionDbs.Any(x => x.RecipientId == rid && x.UserId == uid))
                        {
                            webs = db.WebSocketSessionDbs.Where(x => x.RecipientId == rid && x.UserId == uid).FirstOrDefault();
                            webs.RecipientId = Convert.ToInt32(rid);
                            webs.UserId = Convert.ToInt32(UserId);
                            webs.SessionId = this.WebSocketContext.SecWebSocketKey;
                            db.Entry(webs).State = EntityState.Modified;
                            db.SaveChanges();

                        }
                        else
                        {

                            webs.RecipientId = Convert.ToInt32(rid);
                            webs.UserId = Convert.ToInt32(UserId);
                            webs.SessionId = this.WebSocketContext.SecWebSocketKey;
                            db.WebSocketSessionDbs.Add(webs);
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
            }
        }

        public override void OnMessage(string message)
        {
            try
            {

                ResultClassForNonAuth result = new ResultClassForNonAuth();
                string baseURL = (WebConfigurationManager.AppSettings["WebSiteUrl"]);
                baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
                if (message.Split(',')[1].Split(':').Contains("GroupId"))
                {
                    var Message = message.Split(',')[2].Split(':')[1];
                    var UserId = message.Split(',')[0].Split(':')[1];
                    var GroupId = message.Split(',')[1].Split(':')[1];

                    int GId = Convert.ToInt32(GroupId);
                    int Uid = Convert.ToInt32(UserId);
                    var groupname = "";

                    List<WebSocketSessionDb> list = new List<WebSocketSessionDb>();
                    MessageModel MessagesModel = new MessageModel();
                    using (LystenEntities db = new LystenEntities())
                    {

                        groupname = db.Groups.Where(x => x.Id == GId).Select(x => x.Name).FirstOrDefault();
                        Message ms = new Message()
                        {
                            Body = Message,
                            CreatedDate = DateTime.UtcNow,
                            ParentMessageId = 0,
                            CreatorId = Uid
                        };
                        db.Messages.Add(ms);
                        db.SaveChanges();

                        MessageRecipient MR = new MessageRecipient()
                        {
                            RecipientGroupId = GId,
                            RecipientId = null,
                            MessageId = ms.Id,
                            IsRead = false
                        };
                        db.MessageRecipients.Add(MR);
                        db.SaveChanges();


                        int gid = Convert.ToInt32(GroupId);
                        list = db.WebSocketSessionDbs.Where(x => x.GroupID == gid).ToList();



                        var userdataTimeZone = db.User_Master.Where(x => x.Id == Uid).Select(x => x.TimeZone).FirstOrDefault();
                        var ddd = DateTime.UtcNow;
                        if (userdataTimeZone != null && userdataTimeZone != "")
                        {
                            Instant instant = Instant.FromDateTimeUtc(ms.CreatedDate.Value);
                            IDateTimeZoneProvider timeZoneProvider = DateTimeZoneProviders.Tzdb;
                            var usersTimezoneId = userdataTimeZone; //just an example
                            var usersTimezone = timeZoneProvider[usersTimezoneId];
                            var usersZonedDateTime = instant.InZone(usersTimezone);
                            ddd = usersZonedDateTime.ToDateTimeUnspecified();
                        }



                        MessagesModel = new MessageModel()
                        {
                            MessageId = ms.Id,
                            Body = ms.Body,
                            RecipientGroupId = GId,
                            CreatorId = Uid,
                            IsRead = false,
                            CreatedDate = ddd.Date == DateTime.UtcNow.Date ? "Today " + Convert.ToDateTime((ddd)).ToString("HH:mm") : Convert.ToDateTime(ddd).ToString("dd MM yyyy HH:mm"),

                            //CreatedDate = Convert.ToDateTime(ms.CreatedDate).ToString("dd MM yyyy HH:MM"),
                            Image = US.GetFavouriteImage(baseURL, Uid),
                            CreatorName = db.User_Master.Where(y => y.Id == Uid).Select(y => y.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(y => y.Id == Uid).Select(y => y.FullName).FirstOrDefault(),
                            ParentMessageId = 0,
                            ExpiryDate = System.DateTime.Now,
                            RecipientId = 0
                        };
                        result.Code = (int)HttpStatusCode.OK;
                        result.Msg = ap.Success;
                        result.Data = MessagesModel;
                    }
                    var json = new JavaScriptSerializer().Serialize(result);
                    foreach (var item in list)
                    {
                        foreach (var data in clients.Where(x => x.WebSocketContext.SecWebSocketKey == item.SessionId))
                        {
                            clients.SingleOrDefault(r => ((WebSocketHandler)r).WebSocketContext.SecWebSocketKey == item.SessionId).Send(json);
                        }
                        using (LystenEntities db = new LystenEntities())
                        {
                            var obj = db.User_Master.Where(x => x.Id == item.UserId.Value).FirstOrDefault();
                            var sendername = db.User_Master.Where(x => x.Id == Uid).Select(x => x.FullName).FirstOrDefault();

                            if (obj != null)
                            {
                                if (obj.DeviceToken != null)
                                {
                                    try
                                    {
                                        sendMsgUser(GId, true, obj.DeviceToken, Message, groupname, sendername);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                        }
                    }

                }
                else
                {

                    var Message = message.Split(',')[2].Split(':')[1];
                    var UserId = message.Split(',')[0].Split(':')[1];
                    var ReceiptionId = message.Split(',')[1].Split(':')[1];

                    int RId = Convert.ToInt32(ReceiptionId);
                    int Uid = Convert.ToInt32(UserId);


                    List<WebSocketSessionDb> list = new List<WebSocketSessionDb>();
                    MessageModel MessagesModel = new MessageModel();
                    var SenderFullName = "";

                    using (LystenEntities db = new LystenEntities())
                    {
                        SenderFullName = db.User_Master.Where(x => x.Id == Uid).Select(x => x.FullName).FirstOrDefault();

                        if (!db.MessageRequests.Any(x => x.ToUserId == RId && x.FromUserId == Uid || x.FromUserId == RId && x.ToUserId == Uid))
                        {
                            MessageRequest M1R = new MessageRequest();
                            M1R.ToUserId = Convert.ToInt32(RId);
                            M1R.FromUserId = Convert.ToInt32(Uid);
                            M1R.IsAccept = false;
                            M1R.IsReject = false;
                            M1R.IsRequested = true;
                            db.MessageRequests.Add(M1R);
                            db.SaveChanges();
                            var obj = db.User_Master.Where(x => x.Id == M1R.ToUserId).FirstOrDefault();


                            if (obj.DeviceToken != null || obj.DeviceToken != "")
                            {
                                try
                                {
                                    if (obj.DeviceType == "Android")
                                    {
                                        Helpers.NotificationHelper.sendMsgUserRequest(obj.Id, obj.DeviceToken, 0, SenderFullName);
                                    }
                                    else
                                    {
                                        sendMsgUserRequest(obj.Id, obj.DeviceToken, 0, SenderFullName);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CommonServices.ErrorLogging(ex);
                                }

                            }
                        }
                        Message ms = new Message()
                        {
                            Body = Message,
                            CreatedDate = DateTime.UtcNow,
                            ParentMessageId = 0,
                            CreatorId = Uid
                        };
                        db.Messages.Add(ms);
                        db.SaveChanges();
                        MessageRecipient MR = new MessageRecipient()
                        {
                            RecipientGroupId = null,
                            RecipientId = RId,
                            MessageId = ms.Id,
                            IsRead = false
                        };
                        db.MessageRecipients.Add(MR);
                        db.SaveChanges();
                        list = db.WebSocketSessionDbs.Where(x => x.RecipientId == RId && x.UserId == Uid || (x.RecipientId == Uid && x.UserId == RId)).ToList();


                        var userdataTimeZone = db.User_Master.Where(x => x.Id == Uid).Select(x => x.TimeZone).FirstOrDefault();
                        var ddd = DateTime.UtcNow;
                        if (userdataTimeZone != null && userdataTimeZone != "")
                        {
                            Instant instant = Instant.FromDateTimeUtc(ms.CreatedDate.Value);
                            IDateTimeZoneProvider timeZoneProvider = DateTimeZoneProviders.Tzdb;
                            var usersTimezoneId = userdataTimeZone; //just an example
                            var usersTimezone = timeZoneProvider[usersTimezoneId];
                            var usersZonedDateTime = instant.InZone(usersTimezone);
                            ddd = usersZonedDateTime.ToDateTimeUnspecified();
                        }
                        try
                        {
                            MessagesModel = new MessageModel()
                            {
                                MessageId = ms.Id,
                                Body = ms.Body,
                                RecipientGroupId = 0,
                                CreatorId = Uid,
                                IsRead = false,

                                CreatedDate = ddd.Date == DateTime.UtcNow.Date ? "Today " + Convert.ToDateTime((ddd)).ToString("HH:mm") : Convert.ToDateTime(ddd).ToString("dd MM yyyy HH:mm"),


                                //CreatedDate = ms.CreatedDate.Value.Date == DateTime.Now.Date ? "Today " + Convert.ToDateTime((ms.CreatedDate.Value)).ToString("HH:mm") : Convert.ToDateTime(ms.CreatedDate.Value).ToString("dd MM yyyy HH:mm"),

                                //CreatedDate = Convert.ToDateTime(ms.CreatedDate).ToString("dd MM yyyy HH:MM"),
                                Image = US.GetFavouriteImage(baseURL, Uid),
                                CreatorName = db.User_Master.Where(y => y.Id == Uid).Select(y => y.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(y => y.Id == Uid).Select(y => y.FullName).FirstOrDefault(),
                                ParentMessageId = 0,
                                ExpiryDate = System.DateTime.Now,
                                RecipientId = RId
                            };
                        }
                        catch (Exception ex)
                        {
                            CommonServices.ErrorLogging(ex);
                        }
                        result.Code = (int)HttpStatusCode.OK;
                        result.Msg = ap.Success;
                        result.Data = MessagesModel;
                    }
                    try
                    {
                        var json = new JavaScriptSerializer().Serialize(result);
                        foreach (var item in list)
                        {

                            foreach (var data in clients.Where(x => x.WebSocketContext.SecWebSocketKey == item.SessionId))
                            {

                                clients.SingleOrDefault(r => ((WebSocketHandler)r).WebSocketContext.SecWebSocketKey == item.SessionId).Send(json);
                            }
                            using (LystenEntities db = new LystenEntities())
                            {
                                var obj = db.User_Master.Where(x => x.Id == item.RecipientId.Value).FirstOrDefault();


                                if (obj != null)
                                {
                                    //if (obj.DeviceToken != null)
                                    //{
                                    //    sendMsgEventsssss(item.UserId.Value, obj.DeviceToken, Message, SenderFullName);
                                    //}

                                    try
                                    {
                                        if (obj.DeviceToken != null)
                                        {
                                            if (obj.DeviceType == "Android")
                                            {
                                                Helpers.NotificationHelper.sendMsgEventsssss(item.UserId.Value, obj.DeviceToken, Message, SenderFullName, MessagesModel.MessageId);
                                            }
                                            else
                                            {
                                                sendMsgEventsssss(item.UserId.Value, obj.DeviceToken, Message, SenderFullName);
                                                //sendMsgUserRequest(obj.Id, obj.DeviceToken, 0, SenderFullName);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CommonServices.ErrorLogging(ex);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonServices.ErrorLogging(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
            }
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

        public void sendMsgUserRequest(int GroupId, string devicetocken, int Id, string SenderFullName)
        {


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
                Console.WriteLine("Authentication failed");
                client.Close();
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
            strmsgbody = SenderFullName + " sent you a message request.";

            Debug.WriteLine("during testing via device!");
            System.Web.HttpContext.Current.Server.MapPath("APNSduringdevice.txt");
            string ImagePath = "";
            string Name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["groupimagepath"]).Replace("~", "");


            baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                ImagePath = US.GetFavouriteImage(baseURL, GroupId);
                name = db.User_Master.Where(x => x.Id == GroupId).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == GroupId).Select(x => x.FullName).FirstOrDefault();
            }


            string GroupIdString = Convert.ToString(GroupId);

            //if (IsAccept == 1)
            //{
            //    Checkbool = "Accepted";
            //}
            //else
            //{
            //    Checkbool = "Rejected";
            //}
            //strmsgbody = "Call request has been " + Checkbool;

            payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"inbox\":{},\"acme1\":\"bar\",\"acme2\":42}";

            writer.Write((byte)0); //First byte of payload length; (big-endian first byte)
            writer.Write((byte)payload.Length);     //payload length (big-endian second byte)

            byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
            writer.Write(b1);
            writer.Flush();

            byte[] array = memoryStream.ToArray();
            Debug.WriteLine("This is being sent...\n\n");
            Debug.WriteLine(array);
            try
            {
                sslStream.Write(array);
                sslStream.Flush();
            }
            catch
            {
                Debug.WriteLine("Write failed buddy!!");
                System.Web.HttpContext.Current.Server.MapPath("Writefailed.txt");
            }

            client.Close();
            Debug.WriteLine("Client closed.");
            System.Web.HttpContext.Current.Server.MapPath("APNSSuccess.txt");

        }

        public void sendMsgUser(int GroupId, bool Check, string devicetocken, string Body, string groupname, string senderfullname)
        {
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
            catch (AuthenticationException)
            {
                Console.WriteLine("Authentication failed");
                client.Close();
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
            strmsgbody = senderfullname + " sent a message in " + groupname + ".";

            Debug.WriteLine("during testing via device!");
            System.Web.HttpContext.Current.Server.MapPath("APNSduringdevice.txt");
            string ImagePath = "";
            string Name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["groupimagepath"]).Replace("~", "");
            bool IsMember = false;
            bool IsOwner = false;
            string GroupIdString = "";

            payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"nav\":{\"type\":\"" + Check.ToString() + "\",\"typeID\":" + GroupIdString + ",\"image\":\"" + ImagePath + "\",\"name\":\"" + name + "\",\"IsMember\":\"" + IsMember.ToString() + "\",\"IsOwner\":\"" + IsOwner.ToString() + "\"},\"acme1\":\"bar\",\"acme2\":42}";

            if (Check)
            {
                using (LystenEntities db = new LystenEntities())
                {
                    Group G = db.Groups.Where(x => x.Id == GroupId).FirstOrDefault();
                    if (G != null)
                    {
                        if (G.Image != null)
                        {
                            ImagePath = G.Image = baseURL + G.Image;
                            name = G.Name;
                        }
                    }
                    var obj = db.User_Master.Where(x => x.DeviceToken == devicetocken).FirstOrDefault();
                    if (obj != null)
                    {
                        var data = db.UserGroupMappings.Where(x => x.GroupId == GroupId && x.UserId == obj.Id).FirstOrDefault();
                        if (data != null)
                        {

                            IsMember = true;
                            IsOwner = false;
                        }
                        else
                        {
                            if (G.CreatorId == obj.Id)
                            {
                                IsMember = true;
                                IsOwner = true;
                            }
                        }
                    }
                    GroupIdString = Convert.ToString(G.Id);
                    payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"nav\":{\"type\":\"" + Check.ToString() + "\",\"typeID\":" + GroupIdString + ",\"image\":\"" + ImagePath + "\",\"name\":\"" + G.Name + "\",\"IsMember\":\"" + IsMember.ToString() + "\",\"IsOwner\":\"" + IsOwner.ToString() + "\"},\"acme1\":\"bar\",\"acme2\":42}";

                }


            }
            //else
            //{
            //    GroupIdString = Convert.ToString(GroupId);

            //    Exception ex = new Exception("Testing 1");
            //    CommonServices.ErrorLogging(ex);
            //    baseURL = HttpContext.Current.Request.Url.Authority;
            //    baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            //    using (LystenEntities db = new LystenEntities())
            //    {
            //        Exception ex2 = new Exception("Testing 2");
            //        CommonServices.ErrorLogging(ex2);

            //        ImagePath = US.GetFavouriteImage(baseURL, GroupId);
            //        name = db.User_Master.Where(x => x.Id == GroupId).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == GroupId).Select(x => x.FullName).FirstOrDefault();

            //        Exception ex23asdasd2 = new Exception(name);
            //        CommonServices.ErrorLogging(ex23asdasd2);

            //        Exception ex232 = new Exception("Testing 3");
            //        CommonServices.ErrorLogging(ex232);
            //    }
            //    payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"nav\":{\"type\":\"" + Check.ToString() + "\",\"typeID\":" + GroupIdString + ",\"image\":\"" + ImagePath + "\",\"name\":\"" + name + "\",\"IsMember\":\"" + ImagePath + "\",\"IsOwner\":\"" + name + "\"},\"acme1\":\"bar\",\"acme2\":42}";

            //}




            writer.Write((byte)0); //First byte of payload length; (big-endian first byte)
            writer.Write((byte)payload.Length);     //payload length (big-endian second byte)

            byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
            writer.Write(b1);
            writer.Flush();

            byte[] array = memoryStream.ToArray();
            Debug.WriteLine("This is being sent...\n\n");
            Debug.WriteLine(array);
            try
            {
                sslStream.Write(array);
                sslStream.Flush();
            }
            catch
            {
                Debug.WriteLine("Write failed buddy!!");
                System.Web.HttpContext.Current.Server.MapPath("Writefailed.txt");
            }

            client.Close();
            Debug.WriteLine("Client closed.");
            System.Web.HttpContext.Current.Server.MapPath("APNSSuccess.txt");
        }

        //public void sendMsg(int Id, string devicetocken,string body)
        //{
        //    string ImagePath = "";
        //    string name = "";
        //    string baseURL = HttpContext.Current.Request.Url.Authority;
        //    baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
        //    using (LystenEntities db = new LystenEntities())
        //    {
        //        ImagePath = US.GetFavouriteImage(baseURL, Id);
        //        name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
        //    }

        //    int port = 2195;
        //    String hostname = (WebConfigurationManager.AppSettings["ApnsEnvironment"]);
        //    //String hostname = "gateway.push.apple.com";

        //    string certificatePath = System.Web.HttpContext.Current.Server.MapPath("~/Lysten-DevB.p12");

        //    string certificatePassword = "";

        //    X509Certificate2 clientCertificate = new X509Certificate2(certificatePath, certificatePassword, X509KeyStorageFlags.MachineKeySet);
        //    X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);


        //    TcpClient client = new TcpClient(hostname, port);

        //    SslStream sslStream = new SslStream(
        //                    client.GetStream(),
        //                    false,
        //                    new RemoteCertificateValidationCallback(ValidateServerCertificate),
        //                    null
        //    );
        //    try
        //    {
        //        sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Tls, false);
        //    }
        //    catch (AuthenticationException )
        //    {
        //        client.Close();
        //        Exception Eccsssas12 = new Exception("Athentication Failed");
        //        CommonServices.ErrorLogging(Eccsssas12);
        //        System.Web.HttpContext.Current.Server.MapPath("~/Authenticationfailed.txt");
        //        return;
        //    }



        //   string GroupIdString = Convert.ToString(Id);

        //    Exception ex = new Exception("Testing 1");
        //    CommonServices.ErrorLogging(ex);
        //    baseURL = HttpContext.Current.Request.Url.Authority;
        //    baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
        //    using (LystenEntities db = new LystenEntities())
        //    {
        //        Exception ex2 = new Exception("Testing 2");
        //        CommonServices.ErrorLogging(ex2);

        //        ImagePath = US.GetFavouriteImage(baseURL, Id);
        //        name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();

        //        Exception ex23asdasd2 = new Exception(name);
        //        CommonServices.ErrorLogging(ex23asdasd2);

        //        Exception ex232 = new Exception("Testing 3");
        //        CommonServices.ErrorLogging(ex232);
        //    }


        //    //// Encode a test message into a byte array.
        //    MemoryStream memoryStream = new MemoryStream();
        //    BinaryWriter writer = new BinaryWriter(memoryStream);
        //    writer.Write((byte)0);  //The command
        //    writer.Write((byte)0);  //The first byte of the deviceId length (big-endian first byte)
        //    writer.Write((byte)32); //The deviceId length (big-endian second byte)
        //    byte[] b0 = HexString2Bytes(devicetocken);
        //    WriteMultiLineByteArray(b0);
        //    writer.Write(b0);
        //    String payload;
        //    string strmsgbody = "";
        //    int totunreadmsg = 20;
        //    strmsgbody = "Hey There!";
        //    payload = "{\"aps\":{\"alert\":\"" + body + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"nav\":{\"type\":\"false\",\"typeID\":" + GroupIdString + ",\"image\":\"" + ImagePath + "\",\"name\":\"" + name + "\",\"IsMember\":\"" + ImagePath + "\",\"IsOwner\":\"" + name + "\"},\"acme1\":\"bar\",\"acme2\":42}";


        //    writer.Write((byte)0); //First byte of payload length; (big-endian first byte)
        //    writer.Write((byte)payload.Length);     //payload length (big-endian second byte)
        //    byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
        //    writer.Write(b1);
        //    writer.Flush();

        //    byte[] array = memoryStream.ToArray();
        //    try
        //    {
        //        sslStream.Write(array);
        //        sslStream.Flush();
        //    }
        //    catch
        //    {
        //    }
        //    client.Close();
        //}

        public void sendMsgEventsssss(int Id, string devicetocken, string message, string SenderFullName)
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
            catch (AuthenticationException ex1)
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


            strmsgbody = SenderFullName + " sent you a message";
            //payload = "{\"aps\":{\"alert\":\"" + message + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"event\":{\"EventID\":\"" + 0 + "\"},\"acme1\":\"bar\",\"acme2\":42}";
            payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"nav\":{\"type\":\"false\",\"typeID\":" + Id + ",\"image\":\"" + ImagePath + "\",\"name\":\"" + name + "\"},\"acme1\":\"bar\",\"acme2\":42}";

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

    }
}