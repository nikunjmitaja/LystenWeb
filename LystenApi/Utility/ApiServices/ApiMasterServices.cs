//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using LystenApi.Controllers.Api;
using LystenApi.Db;
using LystenApi.Models;
using LystenApi.ViewModel;
//using ZXing;
using static LystenApi.Controllers.Api.MasterController;
using System.Web.Configuration;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using NodaTime;

namespace LystenApi.Utility.ApiServices
{
    public class ApiMasterServices
    {
        MasterServices MS = new MasterServices();
        ApiUserServices US = new ApiUserServices();
        public List<CountryViewModel> GetCountryList()
        {
            List<CountryViewModel> cm = new List<CountryViewModel>();
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.Country_Master.Where(x => x.IsActive == true).ToList();
                if (obj.Count > 0)
                {
                    cm = AutoMapper.Mapper.Map<List<Country_Master>, List<CountryViewModel>>(obj);
                }
            }
            return cm;
        }


        public List<StateViewModel> GetStatelistByCountryName(int countryId)
        {
            List<StateViewModel> sm = new List<StateViewModel>();
            using (LystenEntities db = new LystenEntities())
            {
                List<State_Master> obj = db.State_Master.Where(x => x.IsActive == true && x.CountryId == countryId).ToList();
                if (obj.Count > 0)
                {
                    sm = AutoMapper.Mapper.Map<List<State_Master>, List<StateViewModel>>(obj);
                }
            }
            return sm;
        }
        public void sendMsg(int Id, string devicetocken, bool IsAccept, bool IsReject)
        {
            string ImagePath = "";
            string name = "";
            string strmsgbody = "";
            strmsgbody = "Hey There!";
            if (IsAccept)
            {
                strmsgbody += "Calling request accepted.";
            }

            if (IsReject)
            {
                strmsgbody += "Calling request rejected.";
            }
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
            int totunreadmsg = 20;
            payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"call\":{\"CallType\":\"" + IsAccept.ToString() == "true" ? "1" : "2" + "\",\"name\":\"" + name + "\"},\"acme1\":\"bar\",\"acme2\":42}";
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

        public dynamic SendDisclaimer(int UserId, int IsAdded)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var data = db.User_Master.Where(x => x.Id == UserId).FirstOrDefault();
                if (IsAdded == 1)
                {
                    data.IsDisclaimer = true;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    data.IsDisclaimer = false;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return data;
            }
        }

        public dynamic getdisclaimer()
        {
            using (LystenEntities db = new LystenEntities())
            {
                var data = db.Settings.Where(x => x.Name == "System.Disclaimer").FirstOrDefault();
                var sm = AutoMapper.Mapper.Map<Setting, SettingViewModel>(data);
                return sm;
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

        public dynamic acceptrejectcall(CallViewModel uM)
        {
            using (LystenEntities db = new LystenEntities())
            {

                Calling_Request qs = db.Calling_Request.Where(x => x.Id == uM.Id).FirstOrDefault();









                //TimeZoneInfo.ConvertTime(qs.AcceptDatetime.Value, qs.TimeZoneId, destinationTimeZone);

                var dt = DateTime.SpecifyKind(uM.AcceptDatetime == 1 ? qs.CallingDateTime1.Value : uM.AcceptDatetime == 2 ? qs.CallingDateTime2.Value : uM.AcceptDatetime == 3 ? qs.CallingDateTime3.Value : System.DateTime.Now, DateTimeKind.Local);

                qs.AcceptDatetime = uM.AcceptDatetime == 1 ? qs.CallingDateTime1 : uM.AcceptDatetime == 2 ? qs.CallingDateTime2 : uM.AcceptDatetime == 3 ? qs.CallingDateTime3 : System.DateTime.Now;




                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(qs.TimeZoneId);


                qs.AcceptDatetimeUTC = TimeZoneInfo.ConvertTimeToUtc(qs.AcceptDatetime.Value, easternZone);
                //= TimeZoneInfo.ConvertTimeToUtc(dt);




                qs.IsAccept = uM.IsAccept == 1 ? true : false;
                qs.IsReject = uM.IsAccept == 0 ? true : false;
                qs.RejectedNote = uM.RejectedNote;
                db.Entry(qs).State = EntityState.Modified;
                db.SaveChanges();

                return qs;
            }
        }

        public string getUserBraintreeId(int userId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                return Convert.ToString(db.BraintreeCustomerMappings.Where(x => x.UserId == userId).Select(x => x.BraintreeCustomerId).FirstOrDefault());
            }
        }

        public bool DeleteStripeCutomer(string userId)
        {
            try
            {
                using (LystenEntities db = new LystenEntities())
                {
                    var _Payment = db.BraintreeCustomerMappings.Where(x => x.BraintreeCustomerId == userId).FirstOrDefault();
                    if (_Payment != null)
                    {
                        db.BraintreeCustomerMappings.Remove(_Payment);
                        db.SaveChanges();
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public List<string> getUserStripList(int userId)
        {
            try
            {
                using (LystenEntities db = new LystenEntities())
                {

                    return db.BraintreeCustomerMappings.Where(x => x.UserId == userId).OrderByDescending(x => x.BraintreeCustomerId).Select(x => x.BraintreeCustomerId).ToList();
                }
            }
            catch
            {
                return new List<string>();
            }
        }


        public dynamic GetTopicList()
        {
            List<CategoryViewModel> sm = new List<CategoryViewModel>();
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.Categories.Where(x => x.IsActive == true).ToList();
                if (obj.Count > 0)
                {
                    sm = AutoMapper.Mapper.Map<List<Category>, List<CategoryViewModel>>(obj);
                }
            }
            return sm;
        }

        public List<CityViewModel> GetCityListByCountryAndStateName(int countryId, int stateId)
        {

            List<CityViewModel> sm = new List<CityViewModel>();
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.City_Master.Where(x => x.IsActive == true && x.CountryId == countryId && x.StateId == stateId).ToList();
                if (obj.Count > 0)
                {
                    sm = AutoMapper.Mapper.Map<List<City_Master>, List<CityViewModel>>(obj);
                }
            }
            return sm;
        }
        public MasterViewModel GetDashBoardData(string Date, string token)
        {
            int UserId = 0;
            int companyid = 0;
            DateTime dt = new DateTime();
            if (Date != "" && Date != null)
            {
                dt = Convert.ToDateTime(Date);
            }

            using (LystenEntities db = new LystenEntities())
            {
                var UserToken = db.AppAccessTokens.Where(x => x.AuthToken == token).Select(x => x.UserId).FirstOrDefault();
                //if (UserToken > 0)
                //{
                //    companyid = db.User_Master.Where(x => x.Id == UserToken).Select(x => x.CompanyId).FirstOrDefault().Value;
                //}


                MasterViewModel mm = new MasterViewModel();
                //{
                //    ItemCount = MS.FormatNumber(db.Item_Master.Where(x => x.IsActive == true && x.CompanyId == companyid).Count()),
                //    PolicyCount = MS.FormatNumber(db.Policy_Master.Where(x => x.IsActive == true && x.CompanyId == companyid).Count()),
                //    ProductionTotal = MS.FormatNumber((long)(db.Production_Master.Sum(x => x.Netweight).HasValue ? db.Production_Master.Sum(x => x.Netweight).Value : 0)),
                //    PartyCount = MS.FormatNumber(db.Party_Master.Count(x => x.IsActive == true && x.IsBroker == false && x.CompanyId == companyid)),
                //    DeliveryChalanCount = MS.FormatNumber((long)(db.Deliverychallan_Detail.Sum(x => x.Netwt).HasValue ? db.Deliverychallan_Detail.Sum(x => x.Netwt).Value : 0)),
                //    CurrencySymbol = (db.Settings.Where(x => x.Name == "System.CurrencySymbol").FirstOrDefault().Value)
                //};
                //var Salescount = db.Invoices.Where(x => x.IsActive == true).Select(x => x.TotalRate).ToList();
                //if (Salescount.Count > 0)
                //{
                //    mm.TotalRate = Convert.ToString(MS.FormatNumber((long)Salescount.Sum(x => x.Value)));
                //}
                //else
                //{
                //    mm.TotalRate = "0";
                //}
                //var obj = asdasdas(dt);
                //mm.Chart = obj;


                return mm;
            }
        }

        public dynamic SaveEvents(EventViewModel uM)
        {
            IdModel IM = new IdModel();


            using (LystenEntities db = new LystenEntities())
            {
                var data = DateTimeOffset.Parse(uM.CreatedDate);

                if (uM.Id > 0)
                {
                    Event qs = db.Events.Where(x => x.Id == uM.Id).Include(x => x.Category).FirstOrDefault();
                    qs.Title = uM.Title;
                    qs.Description = uM.Description;
                    qs.CategoryId = uM.CategoryId;
                    qs.Location = uM.Location;
                    qs.CreatedDate = data.UtcDateTime;
                    qs.Image = uM.Image;
                    qs.IsActive = true;
                    db.Entry(qs).State = EntityState.Modified;
                    db.SaveChanges();
                    IM.Id = qs.Id;
                    return IM;
                }
                else
                {
                    Event qs = new Event()
                    {
                        Title = uM.Title,
                        Description = uM.Description,
                        CategoryId = uM.CategoryId,
                        Location = uM.Location,
                        CreatedDate = data.UtcDateTime,
                        Image = uM.Image,
                        CreatorId = uM.UserId,
                        IsActive = true,
                    };
                    db.Events.Add(qs);
                    db.SaveChanges();
                    IM.Id = qs.Id;
                    return IM;
                }
            }
        }

        public dynamic getrequestcall()
        {
            using (LystenEntities db = new LystenEntities())
            {

                var GetSettingsValue = getsettingsbysystemname("System.CallingPricePerSec");



                RequestCallModel RCM = new RequestCallModel();
                RCM.TimeZone = TimeZoneInfo.GetSystemTimeZones().Select(x => new TimeZoneInfoModel()
                {
                    DisplayName = x.DisplayName,
                    Id = x.Id
                }).ToList();
                var data = db.CallingPriceMasters.Where(x => x.IsActive == true).ToList();
                RCM.Minutes = data.Select(x => new CallingPriceMastersModel()
                {
                    Name = x.Name + " ($" + Convert.ToString(Convert.ToInt32(x.Price)) + ")",
                    Time = x.Time,
                    Id = x.Id,
                    Amount = Convert.ToString(Convert.ToInt32(x.Price) * 100)
                }).ToList();
                return RCM;
            }
        }
        public dynamic getcategoryimageforevent(int CategoryId)
        {
            using (LystenEntities db = new LystenEntities())
            {

                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["categoryeventimage"]).Replace("~", "");

                return db.CategoryImageEvents.Where(x => x.CategoryId == CategoryId).Select(x => x.Image).FirstOrDefault() == null ? "" : db.CategoryImageEvents.Where(x => x.CategoryId == CategoryId).Select(x => x.Image).FirstOrDefault() == "" ? "" : baseURL + db.CategoryImageEvents.Where(x => x.CategoryId == CategoryId).Select(x => x.Image).FirstOrDefault();

            }
        }
        public dynamic getallevents(int Count, string Search)
        {
            using (LystenEntities db = new LystenEntities())
            {
                int skip = Count * 10;
                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["eventimagepath"]).Replace("~", "");

                var data = db.Categories.Where(x => x.IsActive == true).OrderBy(x => x.Name).Skip(skip).Take(10).ToList();


                List<EventMainModel> MainEMM = new List<EventMainModel>();
                if (Search == "" || Search == null)
                {
                    foreach (var item in data)
                    {
                        EventMainModel EMM = new EventMainModel();

                        var obj = (from dbs in db.Events
                                   where dbs.IsActive == true && dbs.CategoryId == item.Id
                                   select dbs).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                        if (obj != null)
                        {
                            EMM.Title = obj.Title == null ? "" : obj.Title;
                            EMM.Id = obj.Id;
                            EMM.Description = obj.Description == null ? "" : obj.Description;
                            EMM.Location = obj.Location == null ? "" : obj.Location;
                            EMM.CreatedDate = string.Format("{0:f}", obj.CreatedDate);
                            EMM.Image = obj.Image == null ? "" : obj.Image == "" ? "" : baseURL + obj.Image;
                            EMM.CategoryName = item.Name == null ? "" : item.Name;
                            EMM.CategoryId = item.Id;
                            EMM.CategoryImageForEvent = getcategoryimageforevent(item.Id);
                        }

                        else
                        {
                            EMM.Title = "";
                            EMM.Id = 0;
                            EMM.Description = "";
                            EMM.Location = "";
                            EMM.CreatedDate = "";
                            EMM.Image = "";
                            EMM.CategoryName = item.Name == null ? "" : item.Name;
                            EMM.CategoryId = item.Id;
                            EMM.CategoryImageForEvent = getcategoryimageforevent(item.Id);
                        }
                        MainEMM.Add(EMM);
                    }
                    return MainEMM;
                }

                else
                {
                    foreach (var item in data)
                    {
                        EventMainModel EMM = new EventMainModel();

                        var obj = (from dbs in db.Events
                                   where dbs.IsActive == true && dbs.CategoryId == item.Id
                                   select dbs).OrderByDescending(x => x.CreatedDate).Where(x => x.Title.ToLower().Contains(Search.ToLower()) || x.Description.ToLower().Contains(Search.ToLower())).FirstOrDefault();
                        if (obj != null)
                        {
                            EMM.Title = obj.Title == null ? "" : obj.Title;
                            EMM.Id = obj.Id;
                            EMM.Description = obj.Description == null ? "" : obj.Description;
                            EMM.Location = obj.Location == null ? "" : obj.Location;
                            EMM.CreatedDate = string.Format("{0:f}", obj.CreatedDate);
                            EMM.Image = obj.Image == null ? "" : obj.Image == "" ? "" : baseURL + obj.Image;
                            EMM.CategoryName = item.Name == null ? "" : item.Name;
                            EMM.CategoryId = item.Id;
                        }
                        else
                        {
                            EMM.Title = "";
                            EMM.Id = 0;
                            EMM.Description = "";
                            EMM.Location = "";
                            EMM.CreatedDate = "";
                            EMM.Image = "";
                            EMM.CategoryName = item.Name == null ? "" : item.Name;
                            EMM.CategoryId = item.Id;
                        }
                        MainEMM.Add(EMM);

                    }
                    return MainEMM;
                }
            }
        }

        public dynamic UpdateCallingRequest(decimal amount, PaymentModel pM, string id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.Calling_Request.Where(x => x.Id == pM.CallingRequestId).FirstOrDefault();
                obj.TotalCallingTime = pM.TotalCallingTime;
                obj.TotalAmount = amount +obj.TotalAmount;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                return obj;
            }
        }

        public Calling_Request GetRequestDataByDate(int Id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                return db.Calling_Request.Where(x => x.Id == Id).FirstOrDefault();
            }
        }
        public string getsettingsbysystemname(string callingPriceId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                return db.Settings.Where(x => x.Name == callingPriceId).Select(x => x.Value).FirstOrDefault();
            }
        }

        public CallingPriceMaster GetcallingpricebyId(int callingPriceId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                return db.CallingPriceMasters.Where(x => x.Id == callingPriceId).FirstOrDefault();
            }
        }

        public dynamic geteventsbyid(int eventId, int UserId)
        {
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["eventimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                var userdataTimeZone = db.User_Master.Where(x => x.Id == UserId).Select(x => x.TimeZone).FirstOrDefault();


                var obj = (from dbs in db.Events
                           where dbs.Id == eventId
                           select dbs).FirstOrDefault();
                return new EventViewModel()
                {
                    Id = obj.Id,
                    Description = obj.Description,
                    CategoryName = obj.Category.Name,
                    Location = obj.Location,
                    Title = obj.Title,
                    CategoryId = obj.CategoryId.Value,
                    //CreatedDate = string.Format("{0:f}", obj.CreatedDate),
                    CreatedDate = obj.CreatedDate == null ? System.DateTime.UtcNow : datetimeset(userdataTimeZone, obj.CreatedDate.Value),

                    Image = obj.Image == null ? "" : obj.Image == "" ? "" : baseURL + obj.Image
                };

            }
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

            var str = (ddd).ToString("dd MMMM yyyy HH:mm");


            //var dt = ddd.Date == DateTime.UtcNow.Date ? "Today " + Convert.ToDateTime((ddd)).ToString("HH:mm") : Convert.ToDateTime(ddd).ToString("dd MM yyyy HH:mm");
            return str;
        }


        public dynamic getalleventsbyCategoryId(int CategoryId, int Count, string Search, string Location)
        {
            using (LystenEntities db = new LystenEntities())
            {
                if (Location == null)
                {
                    Location = "";
                }
                int skip = Count * 10;
                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["eventimagepath"]).Replace("~", "");

                if (Search == "" || Search == null)
                {
                    if (Location == "")
                    {


                        var obj = (from dbs in db.Events
                                   where dbs.IsActive == true && dbs.CategoryId == CategoryId
                                   select dbs).OrderBy(x => x.CreatedDate).Skip(skip).Take(10).ToList();

                        return (from dbs in obj
                                select new EventViewModel()
                                {
                                    Id = dbs.Id,
                                    Description = dbs.Description,
                                    CategoryName = dbs.Category.Name,
                                    Location = dbs.Location,
                                    CreatedDate = string.Format("{0:f}", dbs.CreatedDate),
                                    Image = dbs.Image == null ? "" : dbs.Image == "" ? "" : baseURL + dbs.Image,
                                    Title = dbs.Title,
                                    CategoryId = dbs.CategoryId.Value,
                                }).ToList();
                    }
                    else
                    {
                        var obj = (from dbs in db.Events
                                   where dbs.IsActive == true && dbs.CategoryId == CategoryId
                                   select dbs).Where(x => x.Location.ToLower().Contains(Location.ToLower())).OrderBy(x => x.CreatedDate).Skip(skip).Take(10).ToList();

                        return (from dbs in obj
                                select new EventViewModel()
                                {
                                    Id = dbs.Id,
                                    Description = dbs.Description,
                                    CategoryName = dbs.Category.Name,
                                    Location = dbs.Location,
                                    CreatedDate = string.Format("{0:f}", dbs.CreatedDate),
                                    Image = dbs.Image == null ? "" : dbs.Image == "" ? "" : baseURL + dbs.Image,
                                    Title = dbs.Title,
                                    CategoryId = dbs.CategoryId.Value,
                                }).ToList();
                    }

                }
                else
                {
                    if (Location == "")
                    {

                        var obj = (from dbs in db.Events
                                   where dbs.IsActive == true && dbs.CategoryId == CategoryId
                                   select dbs).OrderBy(x => x.CreatedDate).Skip(skip).Take(10).Where(x => x.Title.ToLower().Contains(Search.ToLower()) || x.Description.ToLower().Contains(Search.ToLower())).ToList();


                        return (from dbs in obj
                                select new EventViewModel()
                                {
                                    Id = dbs.Id,
                                    Description = dbs.Description,
                                    CategoryName = dbs.Category.Name,
                                    Location = dbs.Location,
                                    CreatedDate = string.Format("{0:f}", dbs.CreatedDate),
                                    Image = dbs.Image == null ? "" : dbs.Image == "" ? "" : baseURL + dbs.Image,
                                    Title = dbs.Title,
                                    CategoryId = dbs.CategoryId.Value
                                }).ToList();
                    }
                    else
                    {
                        var obj = (from dbs in db.Events
                                   where dbs.IsActive == true && dbs.CategoryId == CategoryId
                                   select dbs).Where(x => x.Location.ToLower().Contains(Location.ToLower())).OrderByDescending(x => x.CreatedDate).Skip(skip).Take(10).Where(x => x.Title.ToLower().Contains(Search.ToLower()) || x.Description.ToLower().Contains(Search.ToLower())).ToList();

                        return (from dbs in obj
                                select new EventViewModel()
                                {
                                    Id = dbs.Id,
                                    Description = dbs.Description,
                                    CategoryName = dbs.Category.Name,
                                    Location = dbs.Location,
                                    CreatedDate = string.Format("{0:f}", dbs.CreatedDate),
                                    Image = dbs.Image == null ? "" : dbs.Image == "" ? "" : baseURL + dbs.Image,
                                    CategoryId = dbs.CategoryId.Value,
                                    Title = dbs.Title
                                }).ToList();
                    }

                }
            }
        }

        public dynamic deleteevent(int eventId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.Events.Where(x => x.Id == eventId).FirstOrDefault();
                obj.IsActive = false;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                return 1;
            }
        }

        public dynamic geteventlistbyuserid(int userId, int Count)
        {
            int skip = Count * 10;
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["eventimagepath"]).Replace("~", "");
            try
            {
                using (LystenEntities db = new LystenEntities())
                {
                    var obj = db.Events.Where(x => x.CreatorId == userId && x.IsActive == true).OrderBy(x => x.Id).Skip(skip).Take(10).ToList();
                    return (from dbs in obj
                            select new EventViewModel()
                            {
                                Id = dbs.Id,
                                Description = dbs.Description,
                                CategoryName = dbs.Category.Name,
                                Location = dbs.Location,
                                CreatedDate = string.Format("{0:f}", dbs.CreatedDate),
                                Image = dbs.Image == null ? "" : dbs.Image == "" ? "" : baseURL + dbs.Image,
                                CategoryId = dbs.CategoryId.Value,
                                Title = dbs.Title
                            }).ToList();
                }
            }
            catch
            {
                return new Event();
            }

        }

        public dynamic deletequestion(int questionId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.Questions.Where(x => x.Id == questionId).FirstOrDefault();
                obj.IsActive = false;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                return 1;
            }
        }

        public dynamic getquestionlistbyuser(int userId, int Count)
        {
            int skip = Count * 10;
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["eventimagepath"]).Replace("~", "");

            using (LystenEntities db = new LystenEntities())
            {

                var obj3 = (from db1 in db.Questions
                            where db1.IsActive == true && db1.UserId == userId
                            select db1
                        ).OrderByDescending(x => x.CreatedDate).ToList();

                var obj4 = (from db1 in obj3
                            select new nEWHomeQuestion()
                            {
                                QuestionId = db1.Id,
                                QuestionDisplayText = db1.DisplayText,
                                QuestionDescription = db1.Description ?? "",
                                UserId = (int)db1.UserId,
                                UserName = db.User_Master.Where(x => x.Id == db1.UserId).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == db1.UserId).Select(x => x.FullName).FirstOrDefault(),
                                CreatedDate = Convert.ToDateTime((db1.CreatedDate)).ToString("dd MMM, yyyy"),
                                UserImage = baseURL + db.User_Master.Where(z => z.Id == db1.UserId).Select(y => y.Image).FirstOrDefault(),
                                IsSubscribe = db.UserQuestionSubscribes.Where(x => x.UserId == db1.UserId && x.QuestionsId == db1.Id).FirstOrDefault() == null ? false : true
                            }).OrderByDescending(x => x.CreatedDate).ToList();
                return obj4;


            }
        }

        //private void Add_UpdateToken(int id, TokenDetails objToken, int v)
        //{
        //    throw new NotImplementedException();
        //}
    }


}