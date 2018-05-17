using LystenApi.Db;
using LystenApi.Utility.ApiServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace LystenApi.Helpers
{
    public static class NotificationHelper
    {
        public static string serverKey = "AIzaSyA8TnixTCgxdJgIiL8o_DPDZHrwSBxh6mg";
        public static string SendPushNotification(int Id, string devicetocken, int QuestionId, string commentuser, string questionname)
        {
            string response;

            try
            {
                string ImagePath = "";
                string name = "";
                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
                using (LystenEntities db = new LystenEntities())
                {
                    ApiUserServices US = new ApiUserServices();

                    ImagePath = US.GetFavouriteImage(baseURL, Id);
                    name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
                }
                // From: https://console.firebase.google.com/project/x.y.z/settings/general/android:x.y.z

                // Projekt-ID: x.y.z
                // Web-API-Key: A...Y (39 chars)
                // App-ID: 1:...:android:...

                // From https://console.firebase.google.com/project/x.y.z/settings/
                // cloudmessaging/android:x,y,z
                // Server-Key: AAAA0...    ...._4

                //string serverKey = "AAAA33s2tZ8:APA91bFKBFRMEacmRGZCvdiZ4bq-kjqyzmogX-lAqzl0sWzsSlU0ainu_RE2ATmQDC8PlF2YaFhSqBmfR5FXCBXHN3kLM0gGzHojCwuyP7tEhd0b5WYZVq-XTLbJPZGSOZy_svenV1Hl";//"AIzaSyA8TnixTCgxdJgIiL8o_DPDZHrwSBxh6mg"; // Something very long
                string senderId = Convert.ToString(Id);//"959844890015";
                string deviceId = devicetocken;//"fNV1nvahRcQ:APA91bG8NH4CVIFfGhuNnHyrCuxWja6pcNsGBLZUIvuNlbakeOw_xsr0jz-dFetsJq0IxhHpO2nDiXlPgQrgJGQrQJAQztch5jKZBAgVOEj3O4cqN8LKG1fRk66jbVqcRIzCanleo40M"; // Also something very long, 
                                               // topic notification
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    data = new
                    {
                        body = commentuser,//"Greetings",
                        title = questionname,
                        //sound = "Enabled",
                        //icon = ImagePath,
                        Id = QuestionId,
                        Type = "Question",

                    }
                };
                string totunreadmsg = "20";
                string payload = "{\"aps\":{\"alert\":\"" + commentuser + "\",\"badge\":" + totunreadmsg + ",\"sound\":\"mailsent.wav\"},\"acme1\":\"bar\",\"acme2\":42}";
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return response;
        }

        public static void sendMsgCallAccept(int IsAccept, int Id, string devicetocken)
        {

            string ImagePath = "";
            string name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                ApiUserServices US = new ApiUserServices();

                ImagePath = US.GetFavouriteImage(baseURL, Id);
                name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
            }

            string response;

            try
            {
                // From: https://console.firebase.google.com/project/x.y.z/settings/general/android:x.y.z

                // Projekt-ID: x.y.z
                // Web-API-Key: A...Y (39 chars)
                // App-ID: 1:...:android:...

                // From https://console.firebase.google.com/project/x.y.z/settings/
                // cloudmessaging/android:x,y,z
                // Server-Key: AAAA0...    ...._4

                // string serverKey = "AIzaSyA8TnixTCgxdJgIiL8o_DPDZHrwSBxh6mg"; // Something very long
                string senderId = Convert.ToString(Id);//"959844890015";
                string deviceId = devicetocken;//"fNV1nvahRcQ:APA91bG8NH4CVIFfGhuNnHyrCuxWja6pcNsGBLZUIvuNlbakeOw_xsr0jz-dFetsJq0IxhHpO2nDiXlPgQrgJGQrQJAQztch5jKZBAgVOEj3O4cqN8LKG1fRk66jbVqcRIzCanleo40M"; // Also something very long, 
                int totunreadmsg = 20;                         // topic notification
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                string Checkbool = "";
                string strmsgbody = "";
                if (IsAccept == 1)
                {
                    Checkbool = "Accepted";
                }
                else
                {
                    Checkbool = "Rejected";
                }
                strmsgbody = "Message request has been " + Checkbool;
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    data = new
                    {
                        body = strmsgbody,
                        title = "Message",
                        Id= Id,
                        Type="Message"
                        //sound = "Enabled",
                        // Icon = ImagePath,

                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            // return response;
        }

        //IOS sendMsgEventsssss
        public static void SendEventPushNotification(int IsAccept, int Id, string devicetocken, string fullname)
        {

            string ImagePath = "";
            string name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                ApiUserServices US = new ApiUserServices();

                ImagePath = US.GetFavouriteImage(baseURL, Id);
                name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
            }

            string response;

            try
            {
                // From: https://console.firebase.google.com/project/x.y.z/settings/general/android:x.y.z

                // Projekt-ID: x.y.z
                // Web-API-Key: A...Y (39 chars)
                // App-ID: 1:...:android:...

                // From https://console.firebase.google.com/project/x.y.z/settings/
                // cloudmessaging/android:x,y,z
                // Server-Key: AAAA0...    ...._4

                //string serverKey = "AIzaSyA8TnixTCgxdJgIiL8o_DPDZHrwSBxh6mg"; // Something very long
                string senderId = Convert.ToString(Id);//"959844890015";
                string deviceId = devicetocken;//"fNV1nvahRcQ:APA91bG8NH4CVIFfGhuNnHyrCuxWja6pcNsGBLZUIvuNlbakeOw_xsr0jz-dFetsJq0IxhHpO2nDiXlPgQrgJGQrQJAQztch5jKZBAgVOEj3O4cqN8LKG1fRk66jbVqcRIzCanleo40M"; // Also something very long, 
                int totunreadmsg = 20;                         // topic notification
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                string Checkbool = "";
                string strmsgbody = "";
                string payload = "";
                if (IsAccept == 1)
                {
                    Checkbool = "Accepted";
                }
                else
                {
                    Checkbool = "Rejected";
                }
                strmsgbody = "Your call request to " + fullname + Checkbool + ".";

                payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"call\":{\"CallType\":\"" + IsAccept + "\"},\"acme1\":\"bar\",\"acme2\":42}";
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    data = new
                    {
                        body = strmsgbody,
                        title = "Calling",
                        sound = "Enabled"
                    }
                };
                //string payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"call\":{\"CallType\":\"" + IsAccept + "\"},\"acme1\":\"bar\",\"acme2\":42}";
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(payload);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
        }


        //Android
        public static void SendMsgEventPushNotification(int EventId, int Id, string devicetocken, int IsAdded, string Eventname)
        {
            string ImagePath = "";
            string name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                ApiUserServices US = new ApiUserServices();

                ImagePath = US.GetFavouriteImage(baseURL, Id);
                name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
            }

            string response;

            try
            {
                // From: https://console.firebase.google.com/project/x.y.z/settings/general/android:x.y.z

                // Projekt-ID: x.y.z
                // Web-API-Key: A...Y (39 chars)
                // App-ID: 1:...:android:...

                // From https://console.firebase.google.com/project/x.y.z/settings/
                // cloudmessaging/android:x,y,z
                // Server-Key: AAAA0...    ...._4

                //string serverKey = "AIzaSyA8TnixTCgxdJgIiL8o_DPDZHrwSBxh6mg"; // Something very long
                string senderId = Convert.ToString(Id);//"959844890015";
                string deviceId = devicetocken;//"fNV1nvahRcQ:APA91bG8NH4CVIFfGhuNnHyrCuxWja6pcNsGBLZUIvuNlbakeOw_xsr0jz-dFetsJq0IxhHpO2nDiXlPgQrgJGQrQJAQztch5jKZBAgVOEj3O4cqN8LKG1fRk66jbVqcRIzCanleo40M"; // Also something very long, 
                int totunreadmsg = 20;                         // topic notification
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                //string Checkbool = "";
                string strmsgbody = "";

                if (IsAdded == 1)
                {
                    strmsgbody = "Hey!!Let’s check" + Eventname + " is published.";
                }
                else
                {
                    strmsgbody = "Hey!!" + Eventname + " is updated.";
                }
                var data = new
                {
                    to = deviceId,
                    data = new
                    {
                        body = strmsgbody,//"Greetings",
                        title = Eventname,
                        Type = "Event",
                        //sound = "Enabled",
                        Id = EventId,
                        //Icon = ImagePath,
                    }

                };
                //strmsgbody = "Message request has been " + Checkbool;
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                var payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"event\":{\"EventID\":\"" + EventId + "\"},\"acme1\":\"bar\",\"acme2\":42}";
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
        }

        public static void sendMsgUserRequest(int GroupId, string devicetocken, int Id, string SenderFullName)
        {
            string ImagePath = "";
            string Name = "";
            string response = "";
            try
            {
                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["groupimagepath"]).Replace("~", "");
                //string serverKey = "AIzaSyA8TnixTCgxdJgIiL8o_DPDZHrwSBxh6mg"; // Something very long
                //string response = "";

                baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
                using (LystenEntities db = new LystenEntities())
                {
                    ApiUserServices US = new ApiUserServices();

                    ImagePath = US.GetFavouriteImage(baseURL, GroupId);
                    Name = db.User_Master.Where(x => x.Id == GroupId).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == GroupId).Select(x => x.FullName).FirstOrDefault();
                }
                string deviceId = devicetocken;
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                string strmsgbody = SenderFullName + " sent you a message request.";
                int totunreadmsg = 20;
                var data = new
                {
                    to = deviceId,
                    data = new
                    {
                        body = strmsgbody,//"Greetings",
                        title = SenderFullName,
                        Type = "Group",
                        //sound = "Enabled",
                        Id = GroupId,
                        //Icon = ImagePath,
                    }

                };
                string payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"inbox\":{},\"acme1\":\"bar\",\"acme2\":42}";
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                //tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch
            {

            }

        }

        public static void sendMsgGroup(int Id, string devicetocken, string fullname, string Gname, int Groupid)
        {
            string response = "";
            try
            {
                string deviceId = devicetocken;
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                string strmsgbody = fullname + " invited you for discussion in " + Gname;
                int totunreadmsg = 20;
                string payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"inbox\":{},\"acme1\":\"bar\",\"acme2\":42}";

                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    data = new
                    {
                        body = strmsgbody,//"Greetings",
                        title = Gname,
                        Type = "Group",
                        //sound = "Enabled",
                        Id = Groupid,
                        //Icon = ImagePath,
                    }

                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                //tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        //IOS :  sendMsg
        public static void SendPushNotificationToSubscribe(int Id, string devicetocken, int QuestionId, string username)
        {
            string ImagePath = "";
            string name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                ApiUserServices US = new ApiUserServices();

                ImagePath = US.GetFavouriteImage(baseURL, Id);
                name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
            }

            string response;

            try
            {
                // From: https://console.firebase.google.com/project/x.y.z/settings/general/android:x.y.z

                // Projekt-ID: x.y.z
                // Web-API-Key: A...Y (39 chars)
                // App-ID: 1:...:android:...

                // From https://console.firebase.google.com/project/x.y.z/settings/
                // cloudmessaging/android:x,y,z
                // Server-Key: AAAA0...    ...._4

                //string serverKey = "AIzaSyA8TnixTCgxdJgIiL8o_DPDZHrwSBxh6mg"; // Something very long
                string senderId = Convert.ToString(Id);//"959844890015";
                string deviceId = devicetocken;//"fNV1nvahRcQ:APA91bG8NH4CVIFfGhuNnHyrCuxWja6pcNsGBLZUIvuNlbakeOw_xsr0jz-dFetsJq0IxhHpO2nDiXlPgQrgJGQrQJAQztch5jKZBAgVOEj3O4cqN8LKG1fRk66jbVqcRIzCanleo40M"; // Also something very long, 
                int totunreadmsg = 20;                         // topic notification
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                //string Checkbool = "";
             string   strmsgbody = username + " subscribed your Question.";


                var data = new
                {
                    to = deviceId,
                    data = new
                    {
                        body = strmsgbody,//"Greetings",
                        title = "Subscribed",
                        Type = "Subscribe",
                        //sound = "Enabled",
                        Id = QuestionId,
                        //Icon = ImagePath,
                    }

                };
                //strmsgbody = "Message request has been " + Checkbool;
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                //var payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"event\":{\"EventID\":\"" + EventId + "\"},\"acme1\":\"bar\",\"acme2\":42}";
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
        }

        public static void SendCallingNotification(int Id, string devicetocken, string requestedusername)
        {

            string ImagePath = "";
            string name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                ApiUserServices US = new ApiUserServices();

                ImagePath = US.GetFavouriteImage(baseURL, Id);
                name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
            }

            string response;

            try
            {
                // From: https://console.firebase.google.com/project/x.y.z/settings/general/android:x.y.z

                // Projekt-ID: x.y.z
                // Web-API-Key: A...Y (39 chars)
                // App-ID: 1:...:android:...

                // From https://console.firebase.google.com/project/x.y.z/settings/
                // cloudmessaging/android:x,y,z
                // Server-Key: AAAA0...    ...._4

                //string serverKey = "AIzaSyA8TnixTCgxdJgIiL8o_DPDZHrwSBxh6mg"; // Something very long
                string senderId = Convert.ToString(Id);//"959844890015";
                string deviceId = devicetocken;//"fNV1nvahRcQ:APA91bG8NH4CVIFfGhuNnHyrCuxWja6pcNsGBLZUIvuNlbakeOw_xsr0jz-dFetsJq0IxhHpO2nDiXlPgQrgJGQrQJAQztch5jKZBAgVOEj3O4cqN8LKG1fRk66jbVqcRIzCanleo40M"; // Also something very long, 
                                   // topic notification
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                //string Checkbool = "";
               
                string strmsgbody = "";
                int totunreadmsg = 20;
                strmsgbody = requestedusername + " requested for a call.";


                var data = new
                {
                    to = deviceId,
                    data = new
                    {
                        body = strmsgbody,//"Greetings",
                        title = "Calling Request",
                        Type = "Call",
                        //sound = "Enabled",
                        Id = 0,
                        //Icon = ImagePath,
                    }

                };
                //strmsgbody = "Message request has been " + Checkbool;
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                //var payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"event\":{\"EventID\":\"" + EventId + "\"},\"acme1\":\"bar\",\"acme2\":42}";
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

        }

        public static void sendMsgPreCalling(int Id, string devicetocken, string Status, string Fullname)
        {
            string ImagePath = "";
            string name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                ApiUserServices US = new ApiUserServices();

                ImagePath = US.GetFavouriteImage(baseURL, Id);
                name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
            }

            string response;

            try
            {
                // From: https://console.firebase.google.com/project/x.y.z/settings/general/android:x.y.z

                // Projekt-ID: x.y.z
                // Web-API-Key: A...Y (39 chars)
                // App-ID: 1:...:android:...

                // From https://console.firebase.google.com/project/x.y.z/settings/
                // cloudmessaging/android:x,y,z
                // Server-Key: AAAA0...    ...._4

                //string serverKey = "AIzaSyA8TnixTCgxdJgIiL8o_DPDZHrwSBxh6mg"; // Something very long
                string senderId = Convert.ToString(Id);//"959844890015";
                string deviceId = devicetocken;//"fNV1nvahRcQ:APA91bG8NH4CVIFfGhuNnHyrCuxWja6pcNsGBLZUIvuNlbakeOw_xsr0jz-dFetsJq0IxhHpO2nDiXlPgQrgJGQrQJAQztch5jKZBAgVOEj3O4cqN8LKG1fRk66jbVqcRIzCanleo40M"; // Also something very long, 
                                               // topic notification
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                //string Checkbool = "";

                string strmsgbody = "";
                int totunreadmsg = 20;
                if (Status == "TO")
                {
                    strmsgbody = "You have a call from " + Fullname + " in 15 minutes.be ready!";
                }
                else
                {
                    strmsgbody = "Your calling time is in 15 minutes.be ready!";
                }


                var data = new
                {
                    to = deviceId,
                    data = new
                    {
                        body = strmsgbody,//"Greetings",
                        title = "Calling Request",
                        Type = "Call",
                        //sound = "Enabled",
                        Id = Id,
                        //Icon = ImagePath,
                    }

                };
                //strmsgbody = "Message request has been " + Checkbool;
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                //var payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"event\":{\"EventID\":\"" + EventId + "\"},\"acme1\":\"bar\",\"acme2\":42}";
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            //string ImagePath = "";
            //string name = "";
            //var certificatePath = HostingEnvironment.MapPath("~/Lysten-DevB.p12");


            //int port = 2195;
            //String hostname = (WebConfigurationManager.AppSettings["ApnsEnvironment"]);
            ////String hostname = "gateway.push.apple.com";

            //string certificatePassword = "";

            //X509Certificate2 clientCertificate = new X509Certificate2(certificatePath, certificatePassword, X509KeyStorageFlags.MachineKeySet);
            //X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);


            //TcpClient client = new TcpClient(hostname, port);

            //SslStream sslStream = new SslStream(
            //                client.GetStream(),
            //                false,
            //                new RemoteCertificateValidationCallback(ValidateServerCertificate),
            //                null
            //);
            //try
            //{
            //    sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Tls, false);
            //}
            //catch (AuthenticationException ex)
            //{
            //    client.Close();
            //    Exception Eccsssas12 = new Exception("Athentication Failed");
            //    CommonServices.ErrorLogging(Eccsssas12);
            //    System.Web.HttpContext.Current.Server.MapPath("~/Authenticationfailed.txt");
            //    return;
            //}

            ////// Encode a test message into a byte array.
            //MemoryStream memoryStream = new MemoryStream();
            //BinaryWriter writer = new BinaryWriter(memoryStream);
            //writer.Write((byte)0);  //The command
            //writer.Write((byte)0);  //The first byte of the deviceId length (big-endian first byte)
            //writer.Write((byte)32); //The deviceId length (big-endian second byte)
            //byte[] b0 = HexString2Bytes(devicetocken);
            //WriteMultiLineByteArray(b0);
            //writer.Write(b0);
            //String payload;
            //string strmsgbody = "";
            //int totunreadmsg = 20;
            //if (Status == "TO")
            //{
            //    strmsgbody = "You have a call from " + Fullname + " in 15 minutes.be ready!";
            //}
            //else
            //{
            //    strmsgbody = "Your calling time is in 15 minutes.be ready!";
            //}
            //payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"acme1\":\"bar\",\"acme2\":42}";
            //writer.Write((byte)0); //First byte of payload length; (big-endian first byte)
            //writer.Write((byte)payload.Length);     //payload length (big-endian second byte)
            //byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
            //writer.Write(b1);
            //writer.Flush();

            //byte[] array = memoryStream.ToArray();
            //try
            //{
            //    sslStream.Write(array);
            //    sslStream.Flush();
            //}
            //catch
            //{
            //}
            //client.Close();
        }

        public static void SendEmail(int Id, string devicetocken, string requestedusername)
        {

            
        }
        public static void sendMsgEventsssss(int ToId, string devicetocken, string message, string SenderFullName,int receiptID)
        {

            string response = "";
            try
            {
                string deviceId = devicetocken;
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                string strmsgbody = SenderFullName + " sent you a message";
                string messageBody = message;
                int totunreadmsg = 20;

                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    data = new
                    {
                        body = messageBody,//"Greetings",
                        title = strmsgbody,
                        Type = "Message",
                        //sound = "Enabled",
                        Id = receiptID,
                        //Icon = ImagePath,
                    }

                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                //tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}