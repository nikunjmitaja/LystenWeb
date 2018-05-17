using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using LystenApi.Db;
using LystenApi.ViewModel;
using System.Data;
using static LystenApi.Controllers.Api.MessageController;
using LystenApi.Controllers.Api;
using System.Web.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.IO;
using NodaTime;

namespace LystenApi.Utility.ApiServices
{
    public class ApiMessageServices
    {
        MasterServices MS = new MasterServices();
        ApiUserServices US = new ApiUserServices();

        public dynamic SaveUserData(MessageModel uM)
        {
            MessageModel Messages = new MessageModel();
            using (LystenEntities db = new LystenEntities())
            {


            }
            return Messages;
        }


        public dynamic datetimeset(string userdataTimeZone, DateTime Date)
        {
            var ddd = DateTime.UtcNow;
            var dt123 = DateTime.SpecifyKind(Date, DateTimeKind.Utc);
            Instant instant = Instant.FromDateTimeUtc(dt123);
            IDateTimeZoneProvider timeZoneProvider = DateTimeZoneProviders.Tzdb;
            var usersTimezoneId = userdataTimeZone; //just an example
            var usersTimezone = timeZoneProvider[usersTimezoneId];
            var usersZonedDateTime = instant.InZone(usersTimezone);
            ddd = usersZonedDateTime.ToDateTimeUnspecified();
            var dt = ddd.Date == DateTime.UtcNow.Date ? "Today " + Convert.ToDateTime((ddd)).ToString("HH:mm") : Convert.ToDateTime(ddd).ToString("dd MM yyyy HH:mm");
            return dt;


        }
        public dynamic GetMessages(MessageModel mM)
        {
            List<MessageModel> Messages = new List<MessageModel>();
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");


            using (LystenEntities db = new LystenEntities())
            {
                var userdataTimeZone = db.User_Master.Where(x => x.Id == mM.CreatorId).Select(x => x.TimeZone).FirstOrDefault();

                if (mM.RecipientId == 0)
                {



                    var obj = db.MessageRecipients.Where(x => x.RecipientGroupId == mM.RecipientGroupId).OrderBy(x => x.Id).ToList();
                    Messages = obj.Select(x => new MessageModel()
                    {
                        MessageId = (int)x.MessageId,
                        Body = x.Message.Body,
                        RecipientGroupId = x.RecipientGroupId,
                        CreatorId = x.Message.CreatorId,
                        IsRead = (bool)x.IsRead,
                        //CreatedDate = Convert.ToDateTime(x.Message.CreatedDate).ToString("dd MM yyyy HH:MM"),
                        //CreatedDate = x.Message.CreatedDate.Value.Date == DateTime.Now.Date ? "Today " + Convert.ToDateTime((x.Message.CreatedDate)).ToString("HH:mm") : Convert.ToDateTime((x.Message.CreatedDate)).ToString("dd MMM, yyyy HH:mm"),
                        CreatedDate = x.Message.CreatedDate == null ? System.DateTime.UtcNow : datetimeset(userdataTimeZone, x.Message.CreatedDate.Value),
                        Image = US.GetFavouriteImage(baseURL, x.Message.CreatorId),
                        CreatorName = db.User_Master.Where(y => y.Id == x.Message.User_Master.Id).Select(y => y.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(y => y.Id == x.Message.User_Master.Id).Select(y => y.FullName).FirstOrDefault(),
                        ParentMessageId = 0,
                        ExpiryDate = System.DateTime.Now,
                        RecipientId = 0
                    }).ToList();

                }
                else
                {

                    var obj123 = db.MessageRequests.Where(x => x.ToUserId == mM.CreatorId && x.FromUserId == mM.RecipientId || x.ToUserId == mM.RecipientId && x.FromUserId == mM.CreatorId).FirstOrDefault();
                    if (obj123 != null)
                    {

                        var obj = db.MessageRecipients.Where(x => x.RecipientId == mM.RecipientId && x.Message.CreatorId == mM.CreatorId || x.RecipientId == mM.CreatorId && x.Message.CreatorId == mM.RecipientId).OrderBy(x => x.Id).ToList();
                        Messages = obj.Select(x => new MessageModel()
                        {
                            MessageId = (int)x.MessageId,
                            Body = x.Message.Body,
                            RecipientGroupId = 0,
                            CreatorId = x.Message.CreatorId,
                            IsRead = (bool)x.IsRead,
                            //CreatedDate = x.Message.CreatedDate.Value.Date == DateTime.Now.Date ? "Today " + Convert.ToDateTime((x.Message.CreatedDate)).ToString("HH:mm") : Convert.ToDateTime((x.Message.CreatedDate)).ToString("dd MMM, yyyy HH:mm"),
                            CreatedDate = x.Message.CreatedDate == null ? System.DateTime.UtcNow : datetimeset(userdataTimeZone, x.Message.CreatedDate.Value),

                            //CreatedDate = Convert.ToDateTime(x.Message.CreatedDate).ToString("dd MM yyyy HH:MM"),
                            Image = US.GetFavouriteImage(baseURL, x.Message.CreatorId),
                            CreatorName = db.User_Master.Where(y => y.Id == x.Message.User_Master.Id).Select(y => y.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(y => y.Id == x.Message.User_Master.Id).Select(y => y.FullName).FirstOrDefault(),
                            ParentMessageId = 0,
                            ExpiryDate = System.DateTime.Now,
                            RecipientId = x.RecipientId
                        }).ToList();
                    }
                }
            }
            return Messages;
        }





        public void sendMsgGroup(int Id, string devicetocken, string fullname, string Gname)
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
            string strmsgbody = fullname + " invited you for discussion in " + Gname;
            int totunreadmsg = 20;

            string Checkbool = "";
            //if (IsAccept==1)
            //{
            //    Checkbool = "Accrpted";
            //}
            //else
            //{
            //    Checkbool = "Rejected";
            //}
            //strmsgbody = "Call request has been" + Checkbool;
            //payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"call\":{\"CallType\":\"" + IsAccept.ToString() == "1" ? "1" : "2" + "\",\"name\":\"" + name + "\"},\"acme1\":\"bar\",\"acme2\":42}";


            payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"inbox\":{},\"acme1\":\"bar\",\"acme2\":42}";
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
        public object AddUserToGroup(UserGroupViewModel gVM)
        {
            List<MessageModel> Messages = new List<MessageModel>();
            using (LystenEntities db = new LystenEntities())
            {
                UserGroupMapping UGM = new UserGroupMapping();


                if (gVM.IsAdded == "1")
                {
                    var obj = gVM.UsersId.Split(',');
                    foreach (var item in obj)
                    {
                        int id = Convert.ToInt32(item);
                        if (!db.UserGroupMappings.Any(x => x.UserId == id && x.GroupId == gVM.GroupId))
                        {
                            UGM = new UserGroupMapping()
                            {
                                GroupId = gVM.GroupId,
                                UserId = Convert.ToInt16(item)

                            };
                            db.UserGroupMappings.Add(UGM);
                            db.SaveChanges();

                            var group = db.Groups.Where(x => x.Id == gVM.GroupId).FirstOrDefault();
                            var userdetail = db.User_Master.Where(x => x.Id == UGM.UserId).FirstOrDefault();


                            if (group.GroupTypeId == 2)
                            {
                                if (userdetail.DeviceToken != null && userdetail.DeviceToken != "")
                                {
                                    if (userdetail.DeviceType == "Android")
                                    {
                                        Helpers.NotificationHelper.sendMsgGroup(userdetail.Id, userdetail.DeviceToken, group.User_Master.FullName, group.Name,group.Id);

                                    }
                                    else
                                    {
                                        sendMsgGroup(userdetail.Id, userdetail.DeviceToken, group.User_Master.FullName, group.Name);
                                    }
                                }

                            }
                        }
                    }
                }
                else if (gVM.IsAdded == "0")
                {
                    var obj = gVM.UsersId.Split(',');
                    foreach (var item in obj)
                    {
                        int id = Convert.ToInt32(item);
                        if (db.UserGroupMappings.Any(x => x.UserId == id && x.GroupId == gVM.GroupId))
                        {
                            UGM = db.UserGroupMappings.Where(x => x.UserId == id && x.GroupId == gVM.GroupId).FirstOrDefault();
                            if (UGM != null)
                            {
                                db.Entry(UGM).State = EntityState.Deleted;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                else if (gVM.IsAdded == "2")
                {
                    var obj = gVM.UsersId.Split(',');
                    foreach (var item in obj)
                    {
                        int id = Convert.ToInt32(item);
                        if (db.UserGroupMappings.Any(x => x.UserId == id && x.GroupId == gVM.GroupId))
                        {
                            UGM = db.UserGroupMappings.Where(x => x.UserId == id && x.GroupId == gVM.GroupId).FirstOrDefault();
                            if (UGM != null)
                            {
                                db.Entry(UGM).State = EntityState.Deleted;
                                db.SaveChanges();
                            }
                        }
                    }

                    foreach (var item in obj)
                    {
                        int id = Convert.ToInt32(item);

                        var newgroupdata = db.UserGroupMappings.Where(x => x.GroupId == gVM.GroupId).Select(x => x.Id).ToList();


                        var msgRdata = db.MessageRecipients.Where(x => x.RecipientGroupId == gVM.GroupId).ToList();
                        if (msgRdata.Count > 0)
                        {
                            foreach (var itemss in msgRdata)
                            {
                                var msgdata = db.Messages.Where(x => x.Id == itemss.MessageId).FirstOrDefault();
                                if (msgdata != null)
                                {
                                    db.Entry(msgdata).State = EntityState.Deleted;
                                    db.SaveChanges();
                                }
                                var Mdata = db.MessageRecipients.Where(x => x.Id == itemss.Id).FirstOrDefault();
                                if (Mdata != null)
                                {
                                    db.Entry(Mdata).State = EntityState.Deleted;
                                    db.SaveChanges();
                                }


                            }

                        }

                        foreach (var itemss in newgroupdata)
                        {
                            UGM = db.UserGroupMappings.Where(x => x.Id == itemss).FirstOrDefault();
                            if (UGM != null)
                            {
                                db.Entry(UGM).State = EntityState.Deleted;
                                db.SaveChanges();
                            }
                        }
                        if (db.WebSocketSessionDbs.Any(x => x.GroupID == gVM.GroupId))
                        {
                            var websocket = db.WebSocketSessionDbs.Where(x => x.GroupID == gVM.GroupId).FirstOrDefault();
                            db.Entry(websocket).State = EntityState.Deleted;
                            db.SaveChanges();
                        }

                        if (db.Groups.Any(x => x.Id == gVM.GroupId && x.CreatorId == id))
                        {
                            var objdata = db.Groups.Where(x => x.Id == gVM.GroupId && x.CreatorId == id).FirstOrDefault();
                            db.Entry(objdata).State = EntityState.Deleted;
                            db.SaveChanges();
                        }
                    }
                }
                return UGM;
            }
        }

        public dynamic CreateGroup(GroupViewModel GVM)
        {
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["groupimagepath"]).Replace("~", "");
            GroupViewModel data = new GroupViewModel();

            Group G = new Group();
            using (LystenEntities db = new LystenEntities())
            {

                int CreatorId = Convert.ToInt32(GVM.CreatorId);
                int CategoryId = Convert.ToInt32(GVM.CategoryId);
                G = new Group()
                {
                    Name = GVM.Name,
                    IsActive = true,
                    CreatedDate = System.DateTime.Now,
                    CreatorId = CreatorId,
                    GroupTypeId = Convert.ToInt32(GVM.GroupTypeId),
                    CategoryId = CategoryId
                };
                db.Groups.Add(G);
                db.SaveChanges();

                data = AutoMapper.Mapper.Map<Group, GroupViewModel>(G);
                if (G.Image == null || G.Image == "")
                {
                    data.Image = "";
                }
                else
                {
                    data.Image = baseURL + GVM.Image;
                }

                if (!db.UserCategoryMappings.Any(x => x.UserId == CreatorId && x.CategoryId == CategoryId))
                {

                    UserCategoryMapping uc = new UserCategoryMapping()
                    {
                        CategoryId = CategoryId,
                        UserId = CreatorId
                    };
                    db.UserCategoryMappings.Add(uc);
                    db.SaveChanges();
                }
            }
            return data;
        }

        public dynamic SendMessage(MessageModel mM)
        {
            MessageModel Messages = new MessageModel();
            using (LystenEntities db = new LystenEntities())
            {
                Message ms = new Message();
                ms.Body = mM.Body;
                ms.CreatedDate = System.DateTime.Now;
                ms.ParentMessageId = 0;
                ms.CreatorId = mM.CreatorId;
                db.Messages.Add(ms);
                db.SaveChanges();

                MessageRecipient MR = new MessageRecipient();
                if (mM.RecipientId == null)
                {
                    MR.RecipientGroupId = mM.RecipientGroupId;
                }
                else
                {
                    MR.RecipientId = mM.RecipientId;
                }
                MR.MessageId = ms.Id;
                MR.IsRead = false;
                db.MessageRecipients.Add(MR);
                db.SaveChanges();
            }
            return Messages;
        }

        public dynamic GetCategoryList(int UserId)
        {
            List<GroupListViewModel> groupList = new List<GroupListViewModel>();
            using (LystenEntities db = new LystenEntities())
            {
                var categoryList = new List<GroupListViewModel>();
                categoryList = db.UserCategoryMappings.Where(x => x.UserId == UserId).Select(x => new GroupListViewModel()
                {
                    CategoryId = x.CategoryId == null ? 0 : x.CategoryId.Value,
                    CategoryName = x.Category.Name,
                    UserId = x.UserId == null ? 0 : x.UserId.Value,


                }).ToList();

                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["groupimagepath"]).Replace("~", "");

                foreach (var category in categoryList)
                {
                    List<GroupViewModel> model = new List<GroupViewModel>();

                    var allgroup = db.Groups.Where(x => x.GroupTypeId == 1 && x.IsActive == true).OrderByDescending(x => x.Id).ToList();

                    var group = db.Groups.Where(x => x.CategoryId == category.CategoryId && x.GroupTypeId == 2 && x.CreatorId == UserId && x.IsActive == true).OrderByDescending(x => x.Id).ToList();

                    if (group.Count == 0)
                    {
                        var data = db.UserGroupMappings.Where(x => x.Group.CategoryId == category.CategoryId && x.Group.GroupTypeId == 2 && x.UserId == UserId && x.Group.IsActive == true).OrderByDescending(x => x.GroupId).Select(x => x.Group).Distinct().ToList();
                        foreach (var item in data)
                        {
                            GroupViewModel groupmodel = new GroupViewModel();

                            if (item.CategoryId == category.CategoryId)
                            {
                                groupmodel.Id = item.Id;
                                if (item.Image == null || item.Image == "")
                                {
                                    groupmodel.Image = "";
                                }
                                else
                                {
                                    groupmodel.Image = baseURL + item.Image;
                                }

                                groupmodel.Name = item.Name == null ? "" : item.Name;
                                groupmodel.CreatorId = Convert.ToString(item.CreatorId);
                                groupmodel.GroupTypeId = Convert.ToString(item.GroupTypeId);
                                groupmodel.CategoryId = Convert.ToString(item.CategoryId);
                                groupmodel.IsOwner = 0;
                                groupmodel.IsMember = 0;

                                var obbbbb = db.UserGroupMappings.Where(x => x.GroupId == item.Id && x.UserId == UserId).FirstOrDefault();

                                if (obbbbb != null)
                                {
                                    groupmodel.IsMember = 1;
                                }

                                if (item.CreatorId == UserId)
                                {
                                    groupmodel.IsOwner = 1;
                                }
                                model.Add(groupmodel);
                            }
                        }
                    }
                    foreach (var item in group)
                    {
                        GroupViewModel groupmodel = new GroupViewModel();

                        if (item.CategoryId == category.CategoryId)
                        {
                            groupmodel.Id = item.Id;
                            if (item.Image == null || item.Image == "")
                            {
                                groupmodel.Image = "";
                            }
                            else
                            {
                                groupmodel.Image = baseURL + item.Image;
                            }

                            groupmodel.Name = item.Name == null ? "" : item.Name;
                            groupmodel.CreatorId = Convert.ToString(item.CreatorId);
                            groupmodel.GroupTypeId = Convert.ToString(item.GroupTypeId);
                            groupmodel.CategoryId = Convert.ToString(item.CategoryId);
                            groupmodel.IsOwner = 0;
                            groupmodel.IsMember = 0;

                            var obbbbb = db.UserGroupMappings.Where(x => x.GroupId == item.Id && x.UserId == UserId).FirstOrDefault();

                            if (obbbbb != null)
                            {
                                groupmodel.IsMember = 1;
                            }


                            if (item.CreatorId == UserId)
                            {
                                groupmodel.IsOwner = 1;
                                groupmodel.IsMember = 1;
                            }
                            model.Add(groupmodel);
                        }
                    }

                    foreach (var item in allgroup)
                    {
                        GroupViewModel groupmodel = new GroupViewModel();

                        if (item.CategoryId == category.CategoryId)
                        {
                            groupmodel.Id = item.Id;
                            if (item.Image == null || item.Image == "")
                            {
                                groupmodel.Image = "";
                            }
                            else
                            {
                                groupmodel.Image = baseURL + item.Image;
                            }

                            groupmodel.Name = item.Name == null ? "" : item.Name;
                            groupmodel.CreatorId = Convert.ToString(item.CreatorId);
                            groupmodel.GroupTypeId = Convert.ToString(item.GroupTypeId);
                            groupmodel.CategoryId = Convert.ToString(item.CategoryId);
                            groupmodel.IsOwner = 0;
                            groupmodel.IsMember = 0;

                            var obbbbb = db.UserGroupMappings.Where(x => x.GroupId == item.Id && x.UserId == UserId).FirstOrDefault();

                            if (obbbbb != null)
                            {
                                groupmodel.IsMember = 1;
                            }


                            if (item.CreatorId == UserId)
                            {
                                groupmodel.IsOwner = 1;
                                groupmodel.IsMember = 1;

                            }
                            model.Add(groupmodel);
                        }
                    }

                    category.GroupList = new List<GroupViewModel>();
                    category.GroupList = model;

                }

                groupList = categoryList;
            }


            return groupList.Select(x => new
            {
                UserId = UserId,
                Category = groupList
            }).FirstOrDefault();
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

        public void sendMsgCallAccept(int IsAccept, int Id, string devicetocken)
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

            string Checkbool = "";
            //if (IsAccept==1)
            //{
            //    Checkbool = "Accrpted";
            //}
            //else
            //{
            //    Checkbool = "Rejected";
            //}
            //strmsgbody = "Call request has been" + Checkbool;
            //payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"call\":{\"CallType\":\"" + IsAccept.ToString() == "1" ? "1" : "2" + "\",\"name\":\"" + name + "\"},\"acme1\":\"bar\",\"acme2\":42}";

            if (IsAccept == 1)
            {
                Checkbool = "Accepted";
            }
            else
            {
                Checkbool = "Rejected";
            }
            strmsgbody = "Message request has been " + Checkbool;

            payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"inbox\":{},\"acme1\":\"bar\",\"acme2\":42}";
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


        public dynamic messagerequest(MessageRequestModel MM)
        {
            using (LystenEntities db = new LystenEntities())
            {

                MessageRequest MR = new MessageRequest();
                if (db.MessageRequests.Any(x => x.FromUserId == MM.MRUserId && x.ToUserId == MM.UserId || x.FromUserId == MM.UserId && x.ToUserId == MM.MRUserId))
                {
                    MR = db.MessageRequests.Where(x => x.FromUserId == MM.MRUserId && x.ToUserId == MM.UserId || x.FromUserId == MM.UserId && x.ToUserId == MM.MRUserId).FirstOrDefault();
                    if (MM.IsAccept == 0)
                    {
                        MR.IsAccept = false;
                        MR.IsReject = true;
                        MR.IsRequested = false;
                        db.Entry(MR).State = EntityState.Modified;
                        db.SaveChanges();

                        if (MR.User_Master.DeviceToken != null)
                        {
                            if (MR.User_Master.DeviceType == "Android")
                            {
                                Helpers.NotificationHelper.sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master.DeviceToken);
                            }
                            else
                            {
                                sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master.DeviceToken);
                            }
                        }
                        else if (MR.User_Master1.DeviceToken != null)
                        {
                            if (MR.User_Master1.DeviceType == "Android")
                            {
                                Helpers.NotificationHelper.sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master1.DeviceToken);
                            }
                            else
                            {
                                sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master1.DeviceToken);
                            }
                        }
                    }
                    else
                    {
                        MR.IsAccept = true;
                        MR.IsReject = false;
                        MR.IsRequested = false;
                        db.Entry(MR).State = EntityState.Modified;
                        db.SaveChanges();
                        if (MR.User_Master.DeviceToken != null)
                        {
                            if (MR.User_Master.DeviceType == "Android")
                            {
                                Helpers.NotificationHelper.sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master.DeviceToken);
                            }
                            else
                            {
                                sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master.DeviceToken);
                            }
                        }
                        else if (MR.User_Master1.DeviceToken != null)
                        {
                            if (MR.User_Master1.DeviceType == "Android")
                            {
                                Helpers.NotificationHelper.sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master1.DeviceToken);
                            }
                            else
                            {
                                sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master1.DeviceToken);
                            }
                        }

                    }
                }
                else
                {
                    if (MM.IsAccept == 0)
                    {
                        MR.FromUserId = MM.MRUserId;
                        MR.ToUserId = MM.UserId;
                        MR.IsAccept = false;
                        MR.IsReject = true;
                        MR.IsRequested = false;
                        db.MessageRequests.Add(MR);
                        db.SaveChanges();
                        if (MR.User_Master.DeviceToken != null)
                        {
                            if (MR.User_Master.DeviceType == "Android")
                            {
                                Helpers.NotificationHelper.sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master.DeviceToken);
                            }
                            else
                            {
                                sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master.DeviceToken);
                            }
                        }
                        else if (MR.User_Master1.DeviceToken != null)
                        {
                            if (MR.User_Master1.DeviceType == "Android")
                            {
                                Helpers.NotificationHelper.sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master1.DeviceToken);
                            }
                            else
                            {
                                sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master1.DeviceToken);
                            }
                        }
                    }
                    else
                    {
                        MR.FromUserId = MM.MRUserId;
                        MR.ToUserId = MM.UserId;
                        MR.IsAccept = false;
                        MR.IsReject = true;
                        MR.IsRequested = false;
                        db.MessageRequests.Add(MR);
                        db.SaveChanges();
                        if (MR.User_Master.DeviceToken != null)
                        {
                            if (MR.User_Master.DeviceType == "Android")
                            {
                                Helpers.NotificationHelper.sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master.DeviceToken);
                            }
                            else
                            {
                                sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master.DeviceToken);
                            }
                        }
                        else if (MR.User_Master1.DeviceToken != null)
                        {
                            if (MR.User_Master1.DeviceType == "Android")
                            {
                                Helpers.NotificationHelper.sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master1.DeviceToken);
                            }
                            else
                            {
                                sendMsgCallAccept(0, MR.ToUserId.Value, MR.User_Master1.DeviceToken);
                            }
                        }

                    }
                }
                return MR;
            }
        }

        public dynamic getinboxmessage(int UserId, int Count)
        {
            int skip = Count * 10;
            MainInboxMessageModel Minbox = new MainInboxMessageModel();
            List<InboxMessageModel> InboxUser = new List<InboxMessageModel>();
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            var TotalCount = 0;
            using (LystenEntities db = new LystenEntities())
            {


                var userdataTimeZone = db.User_Master.Where(x => x.Id == UserId).Select(x => x.TimeZone).FirstOrDefault();







                List<int> Ids = new List<int>();

                var obj = db.MessageRequests.Where(x => x.FromUserId == UserId || x.ToUserId == UserId).ToList();
                foreach (var item in obj)
                {
                    if (!Ids.Contains(item.ToUserId.Value) && item.ToUserId.Value != UserId)
                        Ids.Add(item.ToUserId.Value);

                    if (!Ids.Contains(item.FromUserId.Value) && item.FromUserId.Value != UserId)
                        Ids.Add(item.FromUserId.Value);
                }
                if (obj.Count > 0)
                {
                    TotalCount = Ids.ToList().Count;
                    var data = Ids.ToList().Skip(skip).Take(10);
                    foreach (var item in data)
                    {
                        //var obj123 = db.MessageRecipients.Where(x => x.RecipientId == UserId || x.Message.CreatorId == UserId && x.RecipientGroupId == null).Select(x => new { x.User_Master.UserName }).OrderBy(x => x.UserName).ToList();
                        var newdatttt = db.User_Master.Where(x => x.Id == item).FirstOrDefault();

                        var datess = db.MessageRecipients.Where(x => x.User_Master.Id == UserId && x.Message.CreatorId == item || x.User_Master.Id == item && x.Message.CreatorId == UserId).OrderByDescending(x => x.Id).Select(x => x.Message.CreatedDate.Value).FirstOrDefault();
                        InboxMessageModel newModel = new InboxMessageModel()
                        {
                            Id = newdatttt.Id,
                            Image = US.GetFavouriteImage(baseURL, newdatttt.Id),
                            CreatorName = newdatttt.FullName == null ? "" : newdatttt.FullName,
                            IsAccept = db.MessageRequests.Where(t => t.FromUserId == UserId && t.ToUserId == item || t.FromUserId == item && t.ToUserId == UserId).FirstOrDefault() == null ? false : db.MessageRequests.Where(t => t.FromUserId == UserId && t.ToUserId == item || t.FromUserId == item && t.ToUserId == UserId).Select(y => y.IsAccept).FirstOrDefault(),
                            IsReject = db.MessageRequests.Where(t => t.FromUserId == UserId && t.ToUserId == item || t.FromUserId == item && t.ToUserId == UserId).FirstOrDefault() == null ? false : db.MessageRequests.Where(t => t.FromUserId == UserId && t.ToUserId == item || t.FromUserId == item && t.ToUserId == UserId).Select(y => y.IsReject).FirstOrDefault(),
                            IsRequested = db.MessageRequests.Where(t => t.FromUserId == UserId && t.ToUserId == item || t.FromUserId == item && t.ToUserId == UserId).FirstOrDefault() == null ? false : db.MessageRequests.Where(t => t.FromUserId == UserId && t.ToUserId == item || t.FromUserId == item && t.ToUserId == UserId).Select(y => y.IsRequested).FirstOrDefault(),


                            //CreatedDate = db1.CreatedDate.Value.Date == DateTime.Now.Date ? "Today " + Convert.ToDateTime((db1.CreatedDate)).ToString("HH:mm") : Convert.ToDateTime((db1.CreatedDate)).ToString("dd MMM, yyyy HH:mm"),
                            CreatedDate = datess == null ? System.DateTime.UtcNow : datetimeset(userdataTimeZone, datess),



                            //CreatedDate = datess.Date == DateTime.Now.Date ? "Today " + Convert.ToDateTime((datess)).ToString("HH:mm") : Convert.ToDateTime(datess).ToString("dd MM yyyy HH:mm"),
                            Message = db.MessageRecipients.Where(x => x.User_Master.Id == UserId && x.Message.CreatorId == item || x.User_Master.Id == item && x.Message.CreatorId == UserId).OrderByDescending(x => x.Id).Select(x => x.Message.Body).FirstOrDefault() == null ? "" : db.MessageRecipients.Where(x => x.User_Master.Id == UserId && x.Message.CreatorId == item || x.User_Master.Id == item && x.Message.CreatorId == UserId).OrderByDescending(x => x.Id).Select(x => x.Message.Body).FirstOrDefault(),
                            IsWaiting = false
                        };
                        var data123 = db.MessageRequests.Where(t => t.FromUserId == UserId && t.ToUserId == item).FirstOrDefault();
                        if (data123 == null)
                        {
                            if (newModel.IsAccept.Value)
                            {
                                newModel.IsRequested = false;
                                newModel.IsWaiting = false;
                            }
                            else if (newModel.IsReject.Value)
                            {
                                newModel.IsRequested = false;
                                newModel.IsWaiting = false;
                            }
                            else
                            {
                                newModel.IsRequested = true;
                                newModel.IsWaiting = false;
                            }

                        }
                        else
                        {
                            newModel.IsRequested = false;
                            newModel.IsWaiting = true;
                        }

                        InboxUser.Add(newModel);
                    }
                }
            }
            Minbox.InboxUser = InboxUser;
            Minbox.TotalCount = TotalCount;
            return Minbox;
        }

        public dynamic UpdateGroup(GroupViewModel GVM)
        {
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["groupimagepath"]).Replace("~", "");
            GroupViewModel data = new GroupViewModel();
            Group G = new Group();
            using (LystenEntities db = new LystenEntities())
            {
                G = db.Groups.Where(x => x.Id == GVM.Id).FirstOrDefault();
                G.Name = GVM.Name;
                G.ModifiedDate = System.DateTime.Now;
                if (GVM.Image != null)
                {
                    data.Image = baseURL + GVM.Image;
                    G.Image = GVM.Image;
                    db.Entry(G).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    data.Image = baseURL + GVM.Image;
                }
                data = AutoMapper.Mapper.Map<Group, GroupViewModel>(G);
                if (G.Image == null || G.Image == "")
                {
                    data.Image = "";
                }
                else
                {
                    data.Image = baseURL + GVM.Image;
                }
                return data;

            }
        }

        public dynamic GetGroupInfobyUserAndGroupId(int UserId, int GroupId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
                List<MessageModel> uvm = new List<MessageModel>();
                var obj = db.Messages.Where(z => z.MessageRecipients.Any(x => x.RecipientGroupId == GroupId) && z.CreatorId == UserId).ToList();
                if (obj == null)
                {
                    uvm = null;
                }
                else
                {
                    uvm = obj.Select(x => new MessageModel()
                    {
                        MessageId = x.Id,

                        Image = baseURL + x.User_Master.Image,
                        CreatedDate = Convert.ToDateTime(x.CreatedDate).ToString("dd MM yyyy HH:MM"),
                        CreatorName = x.User_Master.UserName,
                        CreatorId = x.User_Master.Id,
                        //GroupTypeId=Convert.ToString( x.Group.GroupTypeId.Value),

                    }).ToList();
                }
                return uvm;
            }
        }

        public dynamic GetGroupMembers(int GroupId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
                List<UsersViewModelGroup> uvm = new List<UsersViewModelGroup>();
                var obj = db.UserGroupMappings.Where(z => z.GroupId == GroupId).ToList();
                if (obj == null)
                {
                    uvm = null;
                }
                else
                {
                    uvm = obj.Select(x => new UsersViewModelGroup()
                    {
                        UserId = x.Id,

                        Image = baseURL + x.User_Master.Image,

                        UserName = x.User_Master.UserName,

                        //GroupTypeId=Convert.ToString( x.Group.GroupTypeId.Value),

                    }).ToList();
                }
                return uvm;
            }
        }

        public class InboxMessageModel
        {
            public InboxMessageModel()
            {
            }
            public string Image { get; set; }
            public string CreatorName { get; set; }
            public int Id { get; set; }
            public bool? IsRequested { get; set; }
            public bool? IsReject { get; set; }
            public bool? IsAccept { get; set; }
            public string CreatedDate { get; set; }
            public string Message { get; set; }
            public bool? IsWaiting { get; set; }
        }
        private class MainInboxMessageModel
        {
            public MainInboxMessageModel()
            {
            }
            public int TotalCount { get; set; }
            public List<InboxMessageModel> InboxUser { get; set; }
        }
    }
}