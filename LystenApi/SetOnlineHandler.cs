using LystenApi.Db;
using LystenApi.Utility;
using LystenApi.Utility.ApiServices;
using LystenApi.Utility.Providers;
using Microsoft.Web.WebSockets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using static LystenApi.Controllers.Api.MessageController;

namespace LystenApi
{
    public class SetOnlineHandler : WebSocketHandler
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

              
                if (message != null && message != "")
                {
                    var UserId = message.Split(',')[0].Split(':')[1];
                    var IsLogin = message.Split(',')[1].Split(':')[1];

                    if (IsLogin == "1")
                    {
                        var TimeZone = message.Split(',')[2].Split(':')[1];

                        using (LystenEntities db = new LystenEntities())
                        {
                            if (UserId != "" && TimeZone != "")
                            {
                                int uid = Convert.ToInt32(UserId);

                                var obj = db.User_Master.Where(x => x.Id == uid).FirstOrDefault();

                                if (obj != null)
                                {
                                    obj.TimeZone = TimeZone;
                                    obj.IsLogin = true;
                                    db.Entry(obj).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        using (LystenEntities db = new LystenEntities())
                        {
                            int uid = Convert.ToInt32(UserId);

                            var obj = db.User_Master.Where(x => x.Id == uid).FirstOrDefault();
                            if (obj != null)
                            {
                                obj.IsLogin = false;
                                //obj.TimeZone = "";
                                //obj.DeviceToken = null;
                                db.Entry(obj).State = EntityState.Modified;
                                db.SaveChanges();
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
}
