using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperWebSocket;
using SuperSocket.SocketBase;
using System.Net.Sockets;
using LystenApi.Db;
using System.Data;
using Microsoft.Owin;
using static LystenApi.Controllers.Api.MessageController;
using LystenApi.Utility.ApiServices;
using System.Web;
using System.Web.Configuration;
using LystenApi.Utility.Providers;
using System.Net;
using System.Web.Script.Serialization;

namespace LystenApi.Utility
{
    public class Server
    {
        ApiUserServices US = new ApiUserServices();
        ApiMessageFormat ap = new ApiMessageFormat();
        private WebSocketServer appServer;
        public void Setup()
        {

            try
            {

                appServer = new WebSocketServer();

                if (!appServer.Setup(80)) //Setup with listening port
                {
                    Exception ex = new Exception("server was not set up.");
                    CommonServices.ErrorLogging(ex);
                    return;
                }
                else
                {
                    Exception ex = new Exception("server set up successfully..");
                    CommonServices.ErrorLogging(ex);
                }
                appServer.NewSessionConnected += new SessionHandler<WebSocketSession>(appServer_NewSessionConnected);
                appServer.SessionClosed += new SessionHandler<WebSocketSession, CloseReason>(appServer_SessionClosed);
                appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(appServer_NewMessageReceived);

            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
            }
        }

        public void Start()
        {
            try
            {
                if (!appServer.Start())
                {
                    appServer.Start();
                    Exception ex = new Exception("server set up successfully.but not started..");
                    CommonServices.ErrorLogging(ex);
                }
                else
                {
                    Exception ex = new Exception("server set up and started successfully..");
                    CommonServices.ErrorLogging(ex);
                }
                if (appServer.Start())
                {
                    Exception ex = new Exception("server set up and started successfully..");
                    CommonServices.ErrorLogging(ex);
                }
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
            }
        }
        public void Stop()
        {
            Exception ex = new Exception("server stopped successfully..");
            CommonServices.ErrorLogging(ex);
            appServer.Stop();
        }

        public void appServer_NewMessageReceived(WebSocketSession session, string message)
        {
            try
            {
                ResultClass result = new ResultClass();

                string baseURL = (WebConfigurationManager.AppSettings["WebSiteUrl"]);
                baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");

                var Message = message.Split(',')[2].Split(':')[1];
                var UserId = message.Split(',')[0].Split(':')[1];
                var GroupId = message.Split(',')[1].Split(':')[1];

                int GId = Convert.ToInt32(GroupId);
                int Uid = Convert.ToInt32(UserId);


                List<WebSocketSessionDb> list = new List<WebSocketSessionDb>();
                MessageModel MessagesModel = new MessageModel();
                using (LystenEntities db = new LystenEntities())
                {

                    Message ms = new Message()
                    {
                        Body = Message,
                        CreatedDate = System.DateTime.Now,
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

                    MessagesModel = new MessageModel()
                    {
                        MessageId = ms.Id,
                        Body = ms.Body,
                        RecipientGroupId = GId,
                        CreatorId = Uid,
                        IsRead = false,
                        CreatedDate = Convert.ToDateTime(ms.CreatedDate).ToString("dd MM yyyy HH:MM"),
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
                    if (item.UserId.Value != Uid)
                    {
                        foreach (WebSocketSession session1 in appServer.GetAllSessions().Where(x => x.SessionID == item.SessionId))
                        {
                            session1.Send(json);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
            }
        }

        public void appServer_NewSessionConnected(WebSocketSession session)
        {
            try
            {
                var GroupId = session.Path.Split(',')[1].Split(':')[1];
                var UserId = session.Path.Split(',')[0].Split(':')[1];

                using (LystenEntities db = new LystenEntities())
                {
                    int gid = Convert.ToInt32(GroupId);
                    int uid = Convert.ToInt32(UserId);


                    var obj = db.Groups.Where(x => x.Id == gid).FirstOrDefault();
                    if (obj.GroupTypeId == 1)
                    {
                        if (!db.Groups.Any(x => x.CreatorId == uid && x.Id == gid))
                        {
                            if (!db.UserGroupMappings.Any(x => x.UserId == uid && x.GroupId == gid))
                            {
                                UserGroupMapping ugm = new UserGroupMapping()
                                {
                                    GroupId = gid,
                                    UserId = uid
                                };
                                db.UserGroupMappings.Add(ugm);
                                db.SaveChanges();
                            }
                        }
                    }
                    WebSocketSessionDb webs = new WebSocketSessionDb();

                    if (db.WebSocketSessionDbs.Any(x => x.GroupID == gid && x.UserId == uid))
                    {
                        webs = db.WebSocketSessionDbs.Where(x => x.GroupID == gid && x.UserId == uid).FirstOrDefault();
                        webs.GroupID = Convert.ToInt32(GroupId);
                        webs.UserId = Convert.ToInt32(UserId);
                        webs.SessionId = session.SessionID;
                        db.Entry(webs).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        webs.GroupID = Convert.ToInt32(GroupId);
                        webs.UserId = Convert.ToInt32(UserId);
                        webs.SessionId = session.SessionID;
                        db.WebSocketSessionDbs.Add(webs);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
            }
        }
        public void appServer_SessionClosed(WebSocketSession session, CloseReason value)
        {
            try
            {
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
            }
        }
    }
}