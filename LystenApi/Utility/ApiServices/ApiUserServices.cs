using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using LystenApi.Db;
using LystenApi.ViewModel;
using System.Data;
using LystenApi.Models;
using System.Threading.Tasks;
using LystenApi.Utility.Providers;
using System.Net;
using System.Web.Configuration;
using LystenApi.Controllers.Api;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Net.Security;
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
using System.Security.Authentication;
using NodaTime;

namespace LystenApi.Utility.ApiServices
{
    public class ApiUserServices : BaseApiController
    {
        MasterServices MS = new MasterServices(); ResultClass objresult = new ResultClass();
        ResultClassCommon objresultCommon = new ResultClassCommon();
        ResultClassToken objresultToken = new ResultClassToken();
        EmailServices es = new EmailServices();
        ApiMessageFormat ap = new ApiMessageFormat();
        public async Task<UserViewModelProfile> GetUserProfile(int userId)
        {
            UserViewModelProfile UserProfile = new UserViewModelProfile();
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.User_Master.Where(x => x.Id == userId).FirstOrDefault();

                if (obj != null)
                {
                    var data = db.Favourites.Where(x => x.UserId == userId).ToList();
                    var Newdata = data.Select(x => new ProfileFavourite()
                    {
                        UserId = (int)x.UserId,
                        FavoriteUserId = (int)x.FavoriteUserId,
                        FavoriteUserImage = GetFavouriteImage(baseURL, x.FavoriteUserId),
                        Name = db.User_Master.Where(y => y.Id == x.FavoriteUserId).Select(y => y.FullName).FirstOrDefault()

                    }).ToList();
                    if (obj.Image == null || obj.Image == "")
                    {
                        obj.Image = "";
                    }
                    else
                    {
                        obj.Image = baseURL + obj.Image;
                    }
                    UserProfile = AutoMapper.Mapper.Map<User_Master, UserViewModelProfile>(obj);
                    UserProfile.Age = (obj.Age == null ? 0 : obj.Age);
                    UserProfile.CollegeUniversity = (obj.CollegeUniversity == null ? "" : obj.CollegeUniversity);
                    UserProfile.Degree = (obj.Degree == null ? "" : obj.Degree);
                    UserProfile.Occupation = (obj.Occupation == null ? "" : obj.Occupation);
                    UserProfile.Gender = db.GenderMasters.Where(x => x.Id == obj.Id).Select(x => x.Name).FirstOrDefault() == null ? "" : db.GenderMasters.Where(x => x.Id == obj.Id).Select(x => x.Name).FirstOrDefault();
                    UserProfile.Favourite = Newdata;
                    UserProfile.TotalQuestionsCount = db.Questions.Where(x => x.UserId == userId && x.IsActive == true).ToList().Count();
                    UserProfile.TotalEventsCount = db.Events.Where(x => x.CreatorId == userId && x.IsActive == true).ToList().Count();
                    UserProfile.UserName = obj.UserName == null ? "" : obj.UserName;
                    UserProfile.IsVerified = obj.IsVerified == null || obj.IsVerified == false ? false : true;
                }
            }
            return UserProfile;
        }

        public dynamic getanswerbyquestionid(int questionId, int count, int UserId)
        {
            int skip = 10 * count;
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            HomeAnswerList HA = new HomeAnswerList();




            using (LystenEntities db = new LystenEntities())
            {

                var userdataTimeZone = db.User_Master.Where(x => x.Id == UserId).Select(x => x.TimeZone).FirstOrDefault();



                var obj = db.Answers.Where(x => x.QuestionId == questionId && x.IsActive == true).OrderByDescending(x => x.Id).Skip(skip).Take(10).ToList();
                HA.QuestionName = db.Questions.Where(x => x.Id == questionId).Select(x => x.DisplayText).FirstOrDefault();
                if (HA.QuestionName == null)
                    HA.QuestionName = "";
                var questionList = obj.Where(x => x.QuestionId == questionId).ToList();
                HA.Answer = new List<AnswerList>();
                foreach (var _ans in questionList)
                {
                    AnswerList ans = new AnswerList();
                    ans.AnswerId = _ans.Id;
                    ans.AnswerDisplayText = _ans.DisplayText == null ? "" : _ans.DisplayText;
                    ans.AnswerDescription = _ans.Discription == null ? "" : _ans.Discription;
                    ans.UserId = Convert.ToInt32(_ans.UserId);
                    ans.UserImage = db.User_Master.Where(t => t.Id == _ans.UserId).Select(t => t.Image).FirstOrDefault() == null ? "" : baseURL + db.User_Master.Where(z => z.Id == _ans.UserId).Select(y => y.Image).FirstOrDefault();
                    ans.UserName = db.User_Master.Where(y => y.Id == _ans.UserId).Select(y => y.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(y => y.Id == _ans.UserId).Select(y => y.FullName).FirstOrDefault();
                    //IsSubscribe = db.UserQuestionSubscribes.Where(x => x.UserId == x.UserId && x.QuestionsId == db1.Id).FirstOrDefault() == null ? false : true,
                    //CreatedDate = x.CreatedDate.Value.Date == DateTime.Now.Date ? "Today " + Convert.ToDateTime((x.CreatedDate)).ToString("HH:mm") : Convert.ToDateTime((x.CreatedDate)).ToString("dd MMM, yyyy HH:mm"),

                    ans.CreatedDate = _ans.CreatedDate.Value == null || userdataTimeZone == null ? Convert.ToString(System.DateTime.UtcNow) : datetimeset(userdataTimeZone, _ans.CreatedDate.Value);
                    HA.Answer.Add(ans);

                }
            }
            return HA;
        }

        public dynamic GetOnlineUser(int Count, string Search, int UserId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                int skip = 10 * Count;
                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
                OnlineUserViewModel uv = new OnlineUserViewModel();
                if (Search != "" && Search != null)
                {
                    uv.TotalCount = db.User_Master.Where(x => x.IsLogin == true && x.Id != UserId && x.RoleId != 1 && x.Displayname.ToLower().Contains(Search.ToLower())).ToList().Count();

                    uv.userdetail =
            db.User_Master.Where(x => x.IsLogin == true && x.Id != UserId && x.RoleId != 1).Select(x => new userdetail()
            {
                Id = x.Id,
                Email = x.Email,
                Displayname = (x.FullName == null) ? "" : x.FullName == "" ? "" : x.FullName,
                Image = x.Image == null ? "" : x.Image == "" ? "" : baseURL + x.Image
            }).Where(x => x.Displayname.ToLower().Contains(Search.ToLower())).OrderBy(x => x.Displayname).Skip(skip).Take(10).ToList();
                }
                else
                {
                    uv.TotalCount = db.User_Master.Where(x => x.IsLogin == true && x.Id != UserId).Where(x => x.RoleId != 1).ToList().Count();

                    uv.userdetail =
            db.User_Master.Where(x => x.IsLogin == true && x.Id != UserId).Where(x => x.RoleId != 1).Select(x => new userdetail()
            {
                Id = x.Id,
                Email = x.Email,
                Displayname = (x.FullName == null) ? "" : x.FullName == "" ? "" : x.FullName,
                Image = x.Image == null ? "" : x.Image == "" ? "" : baseURL + x.Image
            }).OrderBy(x => x.Displayname).Skip(skip).Take(10).ToList();
                }

                return uv;
            }
        }

        public dynamic ReportUser(ReportUserViewModel RUVM)
        {
            using (LystenEntities db = new LystenEntities())
            {
                ReportUser RU = new ReportUser()
                {
                    ToUserId = RUVM.ToUserId,
                    FromUserId = RUVM.FromUserId,
                    Description = RUVM.Description
                };
                db.ReportUsers.Add(RU);
                db.SaveChanges();
                return RU;
            }
        }

        public string GetFavouriteImage(string baseURL, int? favoriteUserId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.User_Master.Where(z => z.Id == favoriteUserId).Select(y => y.Image).FirstOrDefault();
                if (obj == null || obj == "")
                {
                    obj = "";
                }
                else
                {
                    obj = baseURL + obj;
                }
                return obj;
            }
        }

        public async Task<ResultClass> SaveUser(User_Master um)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var pwd = SecutiryServices.EncodePasswordToBase64(um.Password);
                var result = (from um23 in db.User_Master
                              where um23.Id == um.Id
                              select um23
                             ).FirstOrDefault();


                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");

                if (result != null)
                {
                    var obj = db.User_Master.Where(x => x.Id == result.Id).FirstOrDefault();
                    //obj.UserName = um.UserName;
                    obj.Phone = um.Phone;
                    obj.Age = um.Age;
                    obj.Gender = um.Gender;
                    obj.Modifydate = System.DateTime.Now;
                    if (um.Image != null && um.Image != "")
                    {
                        obj.Image = um.Image;
                    }

                    obj.SessionId = null;
                    obj.DeviceToken = um.DeviceToken;
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    objresult.Code = (int)HttpStatusCode.OK;
                    objresult.Msg = ap.Success;
                    var user = result;
                    user.Image = obj.Image == "" ? "" : obj.Image == null ? "" : baseURL + obj.Image;
                    objresult.Data = AutoMapper.Mapper.Map<UserViewModel>(user);
                    objresult.Data.Favourite = new List<ProfileFavourite>();


                }
                else
                {
                    objresult.Code = (int)HttpStatusCode.InternalServerError;
                    objresult.Msg = ap.GlobalError;
                    objresult.Data = AutoMapper.Mapper.Map<UserViewModel>(result);
                }
                return objresult;
            }
        }

        public ResultClass UpdateGroupPic(GroupViewModel gVM)
        {
            using (LystenEntities db = new LystenEntities())
            {
                //var pwd = SecutiryServices.EncodePasswordToBase64(um.Password);

                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["groupimagepath"]).Replace("~", "");
                if (gVM != null)
                {
                    var obj = db.Groups.Where(x => x.Id == gVM.Id).FirstOrDefault();
                    //obj.UserName = um.UserName;

                    obj.Image = gVM.Image;
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    objresult.Code = (int)HttpStatusCode.OK;
                    objresult.Msg = ap.Success;
                    objresult.Data = obj;
                    var group = obj;
                    group.Image = obj.Image == null ? "" : obj.Image == "" ? "" : baseURL + obj.Image;
                    objresult.Data = AutoMapper.Mapper.Map<GroupViewModel>(group);
                }
                else
                {
                    objresult.Code = (int)HttpStatusCode.NotFound;
                    objresult.Msg = ap.ItemNoData;
                    objresult.Data = new { };
                }

                return objresult;
            }
        }

        public dynamic userprofileview(int userId, int vUserId, int Count)
        {
            int skip = Count * 10;

            UserViewModelProfileView UserProfile = new UserViewModelProfileView();
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.User_Master.Where(x => x.Id == vUserId).FirstOrDefault();

                if (obj != null)
                {
                    var data = db.Favourites.Where(x => x.UserId == userId && x.FavoriteUserId == vUserId).FirstOrDefault();

                    if (obj.Image == null || obj.Image == "")
                    {
                        obj.Image = "";
                    }
                    else
                    {
                        obj.Image = baseURL + obj.Image;
                    }


                    //var NowUTCdate = DateTime.UtcNow;
                    //var NowUTCdate15Min = DateTime.UtcNow.AddMinutes(-15);

                    //var newdata = db.Calling_Request.Where(x => x.FromUserId == userId && x.ToUserId == vUserId || x.FromUserId == vUserId && x.ToUserId == userId).Where(x => x.IsAccept == true && (x.AcceptDatetimeUTC >= NowUTCdate15Min && x.AcceptDatetimeUTC <= NowUTCdate)).OrderByDescending(x => x.Id).FirstOrDefault();


                    //var callingdata = (from dd in db.Calling_Request
                    //                   where dd.AcceptDatetimeUTC >= NowUTCdate15Min && dd.AcceptDatetimeUTC <= NowUTCdate
                    //                   select dd).ToList();


                    UserProfile = AutoMapper.Mapper.Map<User_Master, UserViewModelProfileView>(obj);
                    UserProfile.Age = (obj.Age == null ? 0 : obj.Age);
                    UserProfile.CollegeUniversity = (obj.CollegeUniversity == null ? "" : obj.CollegeUniversity);
                    UserProfile.Degree = (obj.Degree == null ? "" : obj.Degree);
                    UserProfile.Occupation = (obj.Occupation == null ? "" : obj.Occupation);
                    UserProfile.Gender = db.GenderMasters.Where(x => x.Id == obj.Id).Select(x => x.Name).FirstOrDefault() == null ? "" : db.GenderMasters.Where(x => x.Id == obj.Id).Select(x => x.Name).FirstOrDefault();
                    UserProfile.IsFavourite = data == null ? false : true;
                    UserProfile.IsOnline = obj.IsLogin;
                    UserProfile.Displayname = obj.UserName;
                    UserProfile.CallingRequestId = 0;
                    UserProfile.IsVerified = obj.IsVerified == null || obj.IsVerified == false ? false : true;
                    //if (newdata != null)
                    //{
                    //    if (newdata.AcceptDatetimeUTC.Value.Date == NowUTCdate.Date)
                    //    {

                    //        var obj123 = (newdata.AcceptDatetimeUTC.Value - NowUTCdate.TimeOfDay).Minute;

                    //        if (obj123 == 10 || obj123 == -10)
                    //            UserProfile.CallingRequestId = newdata.Id;
                    //    }
                    //    else
                    //    {
                    //        UserProfile.CallingRequestId = 0;
                    //    }
                    //}
                    //else
                    //{
                    //    UserProfile.CallingRequestId = 0;

                    //}



                    var ques = db.Questions.Where(x => x.UserId == vUserId && x.IsActive == true).OrderByDescending(x => x.CreatedDate).Skip(skip).Take(10).ToList();
                    UserProfile.Question = (from db1 in ques
                                            select new HomeQuestion()
                                            {
                                                QuestionId = db1.Id,
                                                QuestionDisplayText = db1.DisplayText,
                                                QuestionDescription = db1.Description ?? "",
                                                UserId = (int)db1.UserId,
                                                UserName = db.User_Master.Where(x => x.Id == db1.UserId).Select(x => x.UserName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == db1.UserId).Select(x => x.UserName).FirstOrDefault(),
                                                CreatedDate = Convert.ToDateTime((db1.CreatedDate)).ToString("dd MMM, yyyy"),
                                                UserImage = baseURL + db.User_Master.Where(z => z.Id == db1.UserId).Select(y => y.Image).FirstOrDefault(),
                                                IsSubscribe = db.UserQuestionSubscribes.Where(x => x.UserId == db1.UserId && x.QuestionsId == db1.Id).FirstOrDefault() == null ? false : true,
                                                Answered = GetAnswerList(db1.Id),
                                            }).OrderByDescending(x => x.CreatedDate).ToList();

                }
            }
            return UserProfile;
        }


        public dynamic GetUserById(int fromUserId)
        {
            using (LystenEntities db = new LystenEntities())
            {

                return db.User_Master.Where(x => x.Id == fromUserId).FirstOrDefault();
            }
        }

        public dynamic GetAllUsers(int GroupId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
                List<UsersViewModelGroup> uvm = new List<UsersViewModelGroup>();
                var Groupdata = db.Groups.Where(x => x.Id == GroupId).FirstOrDefault();
                var data = db.UserGroupMappings.Where(x => x.GroupId == GroupId).ToList();
                uvm = data.Select(x => new UsersViewModelGroup()
                {
                    UserName = x.User_Master.FullName == null ? "" : x.User_Master.FullName,
                    UserId = x.User_Master.Id,
                    Image = baseURL + x.User_Master.Image,
                    IsSelected = GetUserByGroupBool(GroupId, x.User_Master.Id)

                }).ToList();
                if (Groupdata != null)
                {
                    UsersViewModelGroup uvm2 = new UsersViewModelGroup();
                    uvm2.UserName = Groupdata.User_Master.FullName == null ? "" : Groupdata.User_Master.FullName;
                    uvm2.UserId = Groupdata.User_Master.Id;
                    uvm2.Image = baseURL + Groupdata.User_Master.Image;
                    uvm2.IsSelected = GetUserByGroupBool(GroupId, Groupdata.User_Master.Id);
                    uvm.Add(uvm2);
                }
                return uvm;
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
            var dt = ddd.Date == DateTime.UtcNow.Date ? "Today " + Convert.ToDateTime((ddd)).ToString("HH:mm") : Convert.ToDateTime(ddd).ToString("dd MM yyyy HH:mm");
            return dt;
        }

        public dynamic datetimesetcae1(string userdataTimeZone, DateTime Date, string MainTimezone)
        {



            // var qwedt = DateTime.SpecifyKind(Date, sourceDateTimeKind);
            // var currentUserTimeZoneInfo = this.CurrentTimeZone;
            //var sss= TimeZoneInfo.ConvertTime(dt, currentUserTimeZoneInfo);

            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(MainTimezone);


            var date = TimeZoneInfo.ConvertTimeToUtc(Date, easternZone);

            var dnnnn = System.DateTime.UtcNow;




            var ddd = DateTime.UtcNow;
            var dt123 = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            Instant instant = Instant.FromDateTimeUtc(dt123);
            IDateTimeZoneProvider timeZoneProvider = DateTimeZoneProviders.Tzdb;
            var usersTimezoneId = userdataTimeZone; //just an example
            var usersTimezone = timeZoneProvider[usersTimezoneId];
            var usersZonedDateTime = instant.InZone(usersTimezone);
            ddd = usersZonedDateTime.ToDateTimeUnspecified();
            var dt = ddd.Date == DateTime.UtcNow.Date ? "Today " + Convert.ToDateTime((ddd)).ToString("HH:mm") : Convert.ToDateTime(ddd).ToString("dd MM yyyy HH:mm");
            return dt;
        }


        public dynamic Acceptdatetimesetcae(string userdataTimeZone, DateTime Date, string MainTimezone)
        {



            // var qwedt = DateTime.SpecifyKind(Date, sourceDateTimeKind);
            // var currentUserTimeZoneInfo = this.CurrentTimeZone;
            //var sss= TimeZoneInfo.ConvertTime(dt, currentUserTimeZoneInfo);

            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(MainTimezone);


            var date = TimeZoneInfo.ConvertTimeToUtc(Date, easternZone);

            var dnnnn = System.DateTime.UtcNow;




            var ddd = DateTime.UtcNow;
            var dt123 = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            Instant instant = Instant.FromDateTimeUtc(dt123);
            IDateTimeZoneProvider timeZoneProvider = DateTimeZoneProviders.Tzdb;
            var usersTimezoneId = userdataTimeZone; //just an example
            var usersTimezone = timeZoneProvider[usersTimezoneId];
            var usersZonedDateTime = instant.InZone(usersTimezone);
            ddd = usersZonedDateTime.ToDateTimeUnspecified();
            var dt = Convert.ToDateTime(ddd).ToString("dd MM yyyy HH:mm");
            return dt;
        }



        private bool GetUserByGroupBool(int groupId, int userid)
        {
            using (LystenEntities db = new LystenEntities())
            {
                return db.UserGroupMappings.Where(x => x.GroupId == groupId && x.UserId == userid).FirstOrDefault() == null ? false : true;
            }
        }

        public dynamic GetQuestionsByCategory(int categoryid, int Count, int UserId)
        {

            int skip = Count * 10;
            HomeViewModelQ HVM = new HomeViewModelQ();
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {

                var userdataTimeZone = db.User_Master.Where(x => x.Id == UserId).Select(x => x.TimeZone).FirstOrDefault();
                List<int> ssa = new List<int>();

                //var data = db.QuestionsCategoryMappings.Where(x => x.CategoryId == categoryid && x.Question.IsActive == true).OrderByDescending(x => x.Question.CreatedDate).Select(x => x.QuestionId).Distinct().OrderByDescending(x => x.Value).Skip(skip).Take(10).ToList();
                //foreach (var itemaa in data)
                //{
                //    if (!ssa.Contains(itemaa.Value))
                //        ssa.Add(itemaa.Value);
                //}
                var data = db.QuestionsCategoryMappings.Where(x => x.CategoryId == categoryid && x.QuestionId != null && x.CategoryId != null).Select(x => x.QuestionId).Distinct().OrderByDescending(x => x.Value).Skip(skip).Take(10).ToList();


                foreach (var itemaa in data)
                {
                    //var id = GetQuestionById(itemaa.Value);
                    //if (id != 0)
                    if (!ssa.Contains(itemaa.Value))
                        ssa.Add(itemaa.Value);
                }




                var obj3 = (from db1 in db.Questions
                            join ss in ssa on db1.Id equals ss
                            where db1.IsActive == true
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
                                //CreatedDate = Convert.ToDateTime((db1.CreatedDate)).ToString("dd MMM, yyyy"),
                                //CreatedDate = db1.CreatedDate.Value.Date == DateTime.Now.Date ? "Today " + Convert.ToDateTime((db1.CreatedDate)).ToString("HH:mm") : Convert.ToDateTime((db1.CreatedDate)).ToString("dd MMM, yyyy HH:mm"),
                                CreatedDate = userdataTimeZone == null ? (System.DateTime.UtcNow).ToString() : datetimeset(userdataTimeZone, db1.CreatedDate.Value),

                                UserImage = baseURL + db.User_Master.Where(z => z.Id == db1.UserId).Select(y => y.Image).FirstOrDefault(),
                                IsSubscribe = db.UserQuestionSubscribes.Where(x => x.UserId == UserId && x.QuestionsId == db1.Id).FirstOrDefault() == null ? false : true,
                            }).OrderByDescending(x => x.CreatedDate).ToList();

                HVM.Question = obj4;
            }

            return HVM;

        }


        public int GetQuestionById(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                try
                {
                    var questionList = db.Questions.Where(x => x.Id == id && x.IsActive == true).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    if (questionList != null)
                    {
                        return questionList.Id;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch { return 0; }

            }
        }
        public dynamic GetTopicByUser(int userId)
        {
            OnlyHomeViewModel HVM = new OnlyHomeViewModel();

            var obj1 = new List<int?>();
            List<OnlyHomeTopicModel> HTMM = new List<OnlyHomeTopicModel>();
            using (LystenEntities db = new LystenEntities())
            {
                obj1 = db.UserCategoryMappings.Where(x => x.UserId == userId && x.Category.IsActive == true).Select(x => x.CategoryId).Distinct().ToList();

                foreach (var item in obj1)
                {
                    OnlyHomeTopicModel HM = new OnlyHomeTopicModel();

                    HM.Id = item.Value;
                    HM.Name = db.Categories.Where(x => x.Id == item.Value).Select(x => x.Name).FirstOrDefault();
                    HTMM.Add(HM);
                }
            }
            HVM.TopicModel = HTMM;
            HVM.UserId = userId;
            return HVM;
        }

        public dynamic GetDashboardData(int userId, int CategoryId, int Count)
        {
            int skip = Count * 10;
            HomeViewModel HVM = new HomeViewModel();
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");

            var obj1 = new List<int?>();
            using (LystenEntities db = new LystenEntities())
            {
                obj1 = db.UserCategoryMappings.Where(x => x.UserId == userId && x.CategoryId == CategoryId).Select(x => x.CategoryId).ToList();

                List<HomeTopicModel> HTMM = new List<HomeTopicModel>();
                List<int> ssa = new List<int>();
                if (CategoryId != 0)
                {
                    var data = db.QuestionsCategoryMappings.Where(x => x.CategoryId == CategoryId).Select(x => x.QuestionId).Distinct().OrderByDescending(x => x.Value).Skip(skip).Take(10).ToList();
                    foreach (var itemaa in data)
                    {
                        if (!ssa.Contains(itemaa.Value))
                            ssa.Add(itemaa.Value);
                    }
                }
                else
                {
                    obj1 = db.UserCategoryMappings.Where(x => x.UserId == userId).Select(x => x.CategoryId).ToList();

                    foreach (var item in obj1)
                    {
                        var data = db.QuestionsCategoryMappings.Where(x => x.CategoryId == item.Value).Select(x => x.QuestionId).Distinct().OrderByDescending(x => x.Value).Skip(skip).Take(10).ToList();
                        foreach (var itemaa in data)
                        {
                            if (!ssa.Contains(itemaa.Value))
                                ssa.Add(itemaa.Value);
                        }
                    }
                }

                var obj3 = (from db1 in db.Questions
                            join ss in ssa on db1.Id equals ss
                            where db1.IsActive == true
                            select db1
                           ).OrderByDescending(x => x.CreatedDate).ToList();

                var obj4 = (from db1 in obj3
                            select new HomeQuestion()
                            {
                                QuestionId = db1.Id,
                                QuestionDisplayText = db1.DisplayText,
                                QuestionDescription = db1.Description ?? "",
                                UserId = (int)db1.UserId,
                                UserName = db.User_Master.Where(x => x.Id == db1.UserId).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == db1.UserId).Select(x => x.FullName).FirstOrDefault(),
                                CreatedDate = Convert.ToDateTime((db1.CreatedDate)).ToString("dd MMM, yyyy"),
                                UserImage = baseURL + db.User_Master.Where(z => z.Id == db1.UserId).Select(y => y.Image).FirstOrDefault(),
                                IsSubscribe = db.UserQuestionSubscribes.Where(x => x.UserId == db1.UserId && x.QuestionsId == db1.Id).FirstOrDefault() == null ? false : true,
                                Answered = GetAnswerList(db1.Id),
                            }).OrderByDescending(x => x.CreatedDate).ToList();

                foreach (var item in obj1)
                {
                    HomeTopicModel HM = new HomeTopicModel();
                    List<HomeQuestion> HomeQuestion = new List<HomeQuestion>();

                    HomeQuestion HQ = new HomeQuestion();

                    HM.TopicId = item.Value;
                    HM.TotalCount = db.QuestionsCategoryMappings.Where(x => x.CategoryId == item.Value).Distinct().ToList().Count();
                    HM.Name = db.Categories.Where(x => x.Id == item.Value).Select(x => x.Name).FirstOrDefault();
                    HM.Description = db.Categories.Where(x => x.Id == item.Value).Select(x => x.Description).FirstOrDefault();
                    var data = db.QuestionsCategoryMappings.Where(x => x.CategoryId == item.Value).Select(x => x.QuestionId).Distinct().OrderByDescending(x => x.Value).Skip(skip).Take(10).ToList();
                    foreach (var itemaa in data)
                    {
                        HQ = obj4.Where(x => x.QuestionId == itemaa).FirstOrDefault();
                        HomeQuestion.Add(HQ);
                    }
                    HM.Question = HomeQuestion;
                    HM.Description = "";
                    HTMM.Add(HM);
                }

                HVM.TopicModel = HTMM;
                HVM.UserId = userId;
                return HVM;
            }
        }

        public List<HomeAnswer> GetAnswerList(int id)
        {
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            List<HomeAnswer> HA = new List<HomeAnswer>();
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.Answers.Where(x => x.QuestionId == id && x.IsActive == true).ToList();

                HA = obj.Where(x => x.QuestionId == id).Select(x => new HomeAnswer()
                {
                    AnswerId = x.Id,
                    AnswerDisplayText = x.DisplayText,
                    AnswerDescription = x.Discription ?? "",
                    UserId = (int)x.UserId,
                    UserImage = baseURL + db.User_Master.Where(z => z.Id == x.UserId).Select(y => y.Image).FirstOrDefault(),
                    UserName = db.User_Master.Where(y => y.Id == x.UserId).Select(y => y.UserName).FirstOrDefault() == null ? "" : db.User_Master.Where(y => y.Id == x.UserId).Select(y => y.UserName).FirstOrDefault(),
                    //IsSubscribe = db.UserQuestionSubscribes.Where(x => x.UserId == x.UserId && x.QuestionsId == db1.Id).FirstOrDefault() == null ? false : true,
                    CreatedDate = Convert.ToDateTime((x.CreatedDate)).ToString("dd MMM, yyyy"),
                }).OrderBy(x => x.AnswerId).ToList();
            }
            return HA;
        }

        public User_Master SaveUserData(UserProfileViewModel uM)
        {
            User_Master UserProfile = new User_Master();
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.User_Master.Where(x => x.Id == uM.UserId).FirstOrDefault();
                if (obj != null)
                {
                    int CountryId = 0;
                    int StateId = 0;
                    int CityId = 0;

                    if (!db.Country_Master.Any(x => x.Name.ToLower().Trim() == uM.CountryName.ToLower().Trim()))
                    {
                        Country_Master cm = new Country_Master();
                        cm.Name = uM.CountryName;
                        cm.IsActive = true;
                        cm.CreatedDate = System.DateTime.Now;
                        cm.CreatedBy = 1;
                        db.Country_Master.Add(cm);
                        db.SaveChanges();
                        CountryId = cm.Id;
                    }
                    else
                    {
                        CountryId = db.Country_Master.Where(x => x.Name.ToLower().Trim() == uM.CountryName.ToLower().Trim()).Select(x => x.Id).FirstOrDefault();
                    }
                    if (!db.State_Master.Any(x => x.Name.ToLower().Trim() == uM.StateName.ToLower().Trim() && x.CountryId == CountryId))
                    {
                        State_Master cm = new State_Master();
                        cm.Name = uM.StateName;
                        cm.CountryId = CountryId;
                        cm.IsActive = true;
                        cm.CreatedDate = System.DateTime.Now;
                        cm.CreatedBy = 1;
                        db.State_Master.Add(cm);
                        db.SaveChanges();
                        StateId = cm.Id;
                    }
                    else
                    {
                        StateId = db.State_Master.Where(x => x.Name.ToLower().Trim() == uM.StateName.ToLower().Trim() && x.CountryId == CountryId).Select(x => x.Id).FirstOrDefault();
                    }
                    if (!db.City_Master.Any(x => x.Name.ToLower().Trim() == uM.CityName.ToLower().Trim() && x.CountryId == CountryId && x.StateId == StateId))
                    {
                        City_Master cm = new City_Master();
                        cm.Name = uM.CityName;
                        cm.CountryId = CountryId;
                        cm.StateId = StateId;
                        cm.CreatedDate = System.DateTime.Now;
                        cm.CreatedBy = 1;
                        cm.IsActive = true;
                        db.City_Master.Add(cm);
                        db.SaveChanges();
                        CityId = cm.Id;
                    }
                    else
                    {
                        CityId = db.City_Master.Where(x => x.Name.ToLower().Trim() == uM.CityName.ToLower().Trim() && x.CountryId == CountryId && x.StateId == StateId).Select(x => x.Id).FirstOrDefault();
                    }
                    obj.Address = uM.Address;
                    obj.CountryId = CountryId;
                    obj.StateId = StateId;
                    obj.CityId = CityId;
                    obj.Mobile = uM.Mobile;
                    obj.Phone = uM.Phone;
                    obj.Modifydate = System.DateTime.Now;
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    UserProfile = obj;
                }
                else
                {
                    UserProfile = obj;
                }

            }
            return UserProfile;
        }

        public dynamic AddUserTopic(TopicViewModel uM)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = new UserCategoryMapping();
                if (uM.TopicId != "")
                {
                    var topicId = uM.TopicId.Split(',');

                    var data = db.UserCategoryMappings.Where(x => x.UserId == uM.UserId).Select(x => x.Id).ToList();
                    foreach (var item in data)
                    {
                        var newdata = db.UserCategoryMappings.Where(x => x.Id == item).FirstOrDefault();
                        db.Entry(newdata).State = EntityState.Deleted;
                        db.SaveChanges();
                    }

                    foreach (var item in topicId)
                    {
                        int CategoryId = Convert.ToInt16(item);
                        obj = db.UserCategoryMappings.Where(x => x.UserId == uM.UserId && x.CategoryId == CategoryId).FirstOrDefault();
                        if (obj == null)
                        {
                            obj = new UserCategoryMapping()
                            {
                                UserId = uM.UserId,
                                CategoryId = CategoryId
                            };
                            db.UserCategoryMappings.Add(obj);
                            db.SaveChanges();
                        }
                    }
                }
                return obj;
            }
        }
        public class data
        {

        }
        public dynamic subscribequestion(QueAnsViewModel uM)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = new UserQuestionSubscribe();
                if (db.UserQuestionSubscribes.Any(x => x.UserId == uM.UserId && x.QuestionsId == uM.QuestionId))
                {

                    obj = db.UserQuestionSubscribes.Where(x => x.UserId == uM.UserId && x.QuestionsId == uM.QuestionId).FirstOrDefault();
                    db.Entry(obj).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                else
                {


                    obj = new UserQuestionSubscribe()
                    {
                        UserId = uM.UserId,
                        QuestionsId = uM.QuestionId
                    };
                    db.UserQuestionSubscribes.Add(obj);
                    db.SaveChanges();
                }

                return obj;

            }
        }

        public dynamic askquestion(QueAnsViewModel uM)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = new QuestionsCategoryMapping();
                if (uM.TopicId != "")
                {
                    var topicId = uM.TopicId.Split(',');




                    Question qs = new Question()
                    {
                        UserId = uM.UserId,
                        DisplayText = uM.Question,
                        Description = uM.QuestionDescription,
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    db.Questions.Add(qs);
                    db.SaveChanges();
                    foreach (var item in topicId)
                    {
                        int CategoryId = Convert.ToInt16(item);
                        obj = new QuestionsCategoryMapping()
                        {
                            QuestionId = qs.Id,
                            CategoryId = CategoryId
                        };
                        db.QuestionsCategoryMappings.Add(obj);
                        db.SaveChanges();

                        if (!db.UserCategoryMappings.Any(x => x.UserId == uM.UserId && x.CategoryId == CategoryId))
                        {

                            UserCategoryMapping uc = new UserCategoryMapping()
                            {
                                CategoryId = CategoryId,
                                UserId = uM.UserId
                            };
                            db.UserCategoryMappings.Add(uc);
                            db.SaveChanges();
                        }
                    }
                }
                return obj;
            }
        }

        public dynamic gettopicbyuser(int userId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                return (from dbs in db.Categories
                        join ss in db.UserCategoryMappings on dbs.Id equals ss.CategoryId
                        select new TopicUserViewModel()
                        {
                            TopicName = dbs.Name,
                            UserId = ss.UserId,
                            TopicDescription = string.IsNullOrEmpty(dbs.Description) ? "" : ""
                        }).ToList();
            }
        }
        public void sendMsg(int Id, string devicetocken, int QuestionId, string commentuser, string questionname)
        {

            Exception Ex = new Exception("1");
            CommonServices.ErrorLogging(Ex);
            string ImagePath = "";
            string name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                ImagePath = GetFavouriteImage(baseURL, Id);
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
            strmsgbody = commentuser + " commented on " + questionname;
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

        public dynamic sendanswer(QueAnsViewModel uM)
        {
            using (LystenEntities db = new LystenEntities())
            {
                Answer qs = new Answer()
                {
                    UserId = uM.UserId,
                    DisplayText = uM.DisplayText,
                    Discription = uM.Description,
                    QuestionId = uM.QuestionId,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                };
                db.Answers.Add(qs);
                db.SaveChanges();
                var obj = db.UserQuestionSubscribes.Where(x => x.QuestionsId == qs.QuestionId && x.UserId != qs.UserId).ToList();
                var commentusername = db.User_Master.Where(x => x.Id == uM.UserId).Select(x => x.FullName).FirstOrDefault();
                var questionname = db.Questions.Where(x => x.Id == uM.QuestionId).Select(x => x.DisplayText).FirstOrDefault();

                var objnew = db.Questions.Where(x => x.Id == qs.QuestionId && x.UserId != qs.UserId).ToList();// && x.UserId != qs.UserId




                foreach (var item in obj)
                {

                    if (item.User_Master.DeviceToken != null)
                    {
                        if (item.User_Master.DeviceType == "Android")
                        {
                            Helpers.NotificationHelper.SendPushNotification(item.UserId.Value, item.User_Master.DeviceToken, uM.QuestionId, commentusername, questionname);
                        }
                        else
                        {
                            sendMsg(item.UserId.Value, item.User_Master.DeviceToken, uM.QuestionId, commentusername, questionname);
                        }
                    }
                }

                foreach (var item in objnew)
                {
                    try
                    {
                        if (item.User_Master != null)
                        {
                            if (item.User_Master.DeviceToken != null)
                            {
                                if (item.User_Master.DeviceType == "Android")
                                {
                                    Helpers.NotificationHelper.SendPushNotification(item.UserId.Value, item.User_Master.DeviceToken, uM.QuestionId, commentusername, questionname);
                                }
                                else
                                {
                                    sendMsg(item.UserId.Value, item.User_Master.DeviceToken, uM.QuestionId, commentusername, questionname);
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                }
                return qs;
            }
        }

        public object addfavourite(FavouriteViewModel fm)
        {
            using (LystenEntities db = new LystenEntities())
            {
                Favourite fv = new Favourite();
                if (fm.IsAdded == "1")
                {
                    if (db.Favourites.Any(x => x.UserId == fm.UserId && x.FavoriteUserId == fm.FavoriteUserId))
                    {
                        return fv = null;
                    }
                    fv.UserId = (int)fm.UserId;
                    fv.FavoriteUserId = (int)fm.FavoriteUserId;
                    fv.CreatedDate = System.DateTime.Now;
                    db.Favourites.Add(fv);
                    db.SaveChanges();
                }
                else
                {
                    fv = db.Favourites.Where(x => x.UserId == fm.UserId && x.FavoriteUserId == fm.FavoriteUserId).FirstOrDefault();
                    db.Entry(fv).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                return fv;

            }
        }


        public async Task<ResultClass> UpdateProfilePic(User_Master um)
        {
            using (LystenEntities db = new LystenEntities())
            {
                //var pwd = SecutiryServices.EncodePasswordToBase64(um.Password);
                var result = (from um23 in db.User_Master
                              where um23.Id == um.Id
                              select um23
                             ).FirstOrDefault();


                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
                if (result != null)
                {
                    var obj = db.User_Master.Where(x => x.Id == result.Id).FirstOrDefault();
                    //obj.UserName = um.UserName;
                    obj.Modifydate = System.DateTime.Now;
                    obj.Image = um.Image;
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    objresult.Code = (int)HttpStatusCode.OK;
                    objresult.Msg = ap.Success;
                    objresult.Data = obj;
                    var user = result;
                    user.Image = obj.Image == null ? "" : obj.Image == "" ? "" : baseURL + obj.Image;
                    objresult.Data = AutoMapper.Mapper.Map<UserViewModel>(user);
                    objresult.Data.Favourite = new List<ProfileFavourite>();

                }
                else
                {
                    objresult.Code = (int)HttpStatusCode.NotFound;
                    objresult.Msg = ap.ItemNoData;
                }

                return objresult;
            }
        }


        public async Task<ResultClass> UpdateInfo(User_Master um)
        {
            ResultClass UserProfile = new ResultClass();
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.User_Master.Where(x => x.Id == um.Id).FirstOrDefault();
                if (obj != null)
                {

                    obj.Displayname = um.Displayname;
                    obj.FullName = um.FullName;
                    // obj.LastName = um.LastName;
                    obj.Age = um.Age;
                    obj.Gender = um.Gender;
                    obj.Mobile = um.Mobile;
                    obj.Phone = um.Phone;
                    obj.Modifydate = System.DateTime.Now;
                    obj.CollegeUniversity = um.CollegeUniversity;
                    obj.Degree = um.Degree;
                    obj.Occupation = um.Occupation;
                    obj.CountryId = um.CountryId;
                    obj.StateId = um.StateId;
                    obj.CityId = um.CityId;
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    UserProfile.Data = obj;
                    UserProfile.Code = (int)HttpStatusCode.OK;
                    UserProfile.Msg = ap.Success;
                    var user = obj;
                    string baseURL = HttpContext.Current.Request.Url.Authority;
                    if (obj.Image == null | obj.Image == "")
                    {
                        user.Image = "";
                    }
                    else
                    {
                        user.Image = baseURL + (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "") + obj.Image;
                    }
                    user.Displayname = obj.Displayname == null ? "" : obj.Displayname == "" ? "" : obj.Displayname;
                    UserProfile.Data = AutoMapper.Mapper.Map<UserViewModel>(user);
                    UserProfile.Data.Favourite = new List<ProfileFavourite>();
                }
                else
                {
                    UserProfile.Code = (int)HttpStatusCode.NotFound;
                    UserProfile.Msg = "No user found";
                }

            }
            return UserProfile;

        }

        public dynamic GetOnlineUserById(int UserId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
                return db.User_Master.Where(x => x.IsLogin == true && x.Id == UserId).Select(x => new userdetail()
                {
                    Id = x.Id,
                    Email = x.Email,
                    Displayname = x.UserName,
                    Image = baseURL + x.Image
                }).FirstOrDefault();
            }
        }

        public dynamic GetGroupInfobyUserAndGroupId(int UserId, int GroupId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
                GroupViewModel uvm = new GroupViewModel();
                var obj = db.UserGroupMappings.Where(z => z.UserId == UserId && z.Group.GroupTypeId == GroupId).FirstOrDefault();
                if (obj == null)
                {
                    uvm = null;
                }
                else
                {
                    uvm = new GroupViewModel()
                    {
                        Id = obj.GroupId == null ? 0 : obj.GroupId.Value,
                        Name = obj.Group.Name,
                        Image = baseURL + obj.Group.Image,
                        CreatorId = Convert.ToString(db.Groups.Where(x => x.Id == obj.GroupId).Select(x => x.CreatorId).FirstOrDefault()),
                        GroupTypeId = Convert.ToString(obj.Group.GroupTypeId.Value),
                        CategoryId = Convert.ToString(obj.Group.CategoryId)
                    };
                }
                return uvm;
            }
        }


        public dynamic GetAllUsersList(int Count, int UserId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                OnlineUserViewModel uv = new OnlineUserViewModel();

                uv.TotalCount = db.User_Master.Where(x => x.IsActive == true).Where(x => x.RoleId != 1 && x.Id != UserId).ToList().Count();
                int skip = 10 * Count;
                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
                uv.userdetail =
                 db.User_Master.Where(x => x.RoleId != 1 && x.Id != UserId).Select(x => new userdetail()
                 {
                     Id = x.Id,
                     Email = x.Email,
                     Displayname = x.FullName == null ? "" : x.FullName,
                     Image = (x.Image == null) ? "" : x.Image == "" ? "" : (baseURL + x.Image)
                 }).OrderBy(x => x.Displayname).Skip(skip).Take(10).ToList();
                return uv;
            }
        }


        public bool CheckCall(int Id)
        {
            bool check = false;
            using (LystenEntities db = new LystenEntities())
            {
                var NowUTCdate = DateTime.UtcNow;
                var NowUTCdate15Min = DateTime.UtcNow.AddMinutes(-5);
                try
                {
                    var newdata = db.Calling_Request.Where(x => x.Id == Id).Where(x => x.IsAccept == true && (x.AcceptDatetimeUTC >= NowUTCdate15Min && x.AcceptDatetimeUTC <= NowUTCdate)).OrderByDescending(x => x.Id).FirstOrDefault();

                    if (newdata != null)
                    {
                        if (newdata.IsAccept.Value)
                        {

                            Exception EEEE = new Exception(newdata.AcceptDatetimeUTC.Value.Date.ToString());
                            CommonServices.ErrorLogging(EEEE);

                            Exception EEEEs = new Exception(NowUTCdate.Date.ToString());
                            CommonServices.ErrorLogging(EEEEs);



                            if (newdata.AcceptDatetimeUTC.Value.Date == NowUTCdate.Date)
                            {

                                var obj123 = (newdata.AcceptDatetimeUTC.Value - NowUTCdate).Minutes;

                                if (obj123 < 6)
                                    check = true;
                            }
                            else
                            {
                                check = false;
                            }
                        }
                        else
                        {
                            check = false;
                        }
                    }
                    else
                    {
                        check = false;
                    }
                }
                catch
                {
                     check = false;
                }
                return check;
            }
        }

        public dynamic GetUsersCallingRequested(int userId)
        {


            using (LystenEntities db = new LystenEntities())
            {

                try
                {
                    string baseURL = HttpContext.Current.Request.Url.Authority;
                    baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");


                    var userdataTimeZone = db.User_Master.Where(x => x.Id == userId).Select(x => x.TimeZone).FirstOrDefault();


                    var varOtherCalling = db.Calling_Request.Where(x => x.ToUserId == userId).ToList();
                    var varMyCalling = db.Calling_Request.Where(x => x.FromUserId == userId).ToList();

                    userdetailcallinglist uvlist = new userdetailcallinglist()
                    {
                        OtherCalling = varOtherCalling.Select(x => new userdetailcalling()
                        {
                            FromUserId = x.User_Master.Id,
                            Id = x.Id,
                            Email = x.User_Master.Email,
                            Displayname = x.User_Master == null ? "" : x.User_Master.FullName == null ? "" : x.User_Master.FullName,
                            Image = (x.User_Master.Image == null) ? "" : x.User_Master.Image == "" ? "" : (baseURL + x.User_Master.Image),
                            IsAccept = x.IsAccept == null ? false : x.IsAccept.Value,
                            IsReject = x.IsReject == null ? false : x.IsReject.Value,

                            CallingDateTime1 = x.CallingDateTime1 == null ? "" : string.IsNullOrEmpty(userdataTimeZone) ? x.CallingDateTime1.Value : datetimesetcae1(userdataTimeZone, x.CallingDateTime1.Value, x.TimeZoneId),
                            CallingDateTime2 = x.CallingDateTime2 == null ? "" : string.IsNullOrEmpty(userdataTimeZone) ? x.CallingDateTime2.Value : datetimesetcae1(userdataTimeZone, x.CallingDateTime2.Value, x.TimeZoneId),
                            CallingDateTime3 = x.CallingDateTime3 == null ? "" : string.IsNullOrEmpty(userdataTimeZone) ? x.CallingDateTime3.Value : datetimesetcae1(userdataTimeZone, x.CallingDateTime3.Value, x.TimeZoneId),
                            IsCalling = CheckCall(x.Id),
                            AcceptedDatetime = x.AcceptDatetime == null ? "" : string.IsNullOrEmpty(userdataTimeZone) ? x.AcceptDatetime.Value : Acceptdatetimesetcae(userdataTimeZone, x.AcceptDatetime.Value, x.TimeZoneId),
                            //CallingDateTime1 = Convert.ToDateTime(x.CallingDateTime1).ToString("dd MMM yyyy HH:mm"),
                            //CallingDateTime2 = x.CallingDateTime2 == null ? null : Convert.ToDateTime(x.CallingDateTime2).ToString("dd MMM yyyy HH:mm"),
                            //CallingDateTime3 = x.CallingDateTime3 == null ? null : Convert.ToDateTime(x.CallingDateTime3).ToString("dd MMM yyyy HH:mm"),
                            RejectedNote = x.RejectedNote == null ? "" : x.RejectedNote
                        }).OrderBy(x => x.Displayname).ToList(),
                     
                        MyCalling = varMyCalling.Select(x => new userdetailcalling()
                        {
                            ToUserId = x.User_Master1.Id,
                            Id = x.Id,
                            Email = x.User_Master1.Email,
                            Displayname = x.User_Master1 == null ? "" : x.User_Master1.FullName == null ? "" : x.User_Master1.FullName,
                            Image = (x.User_Master1.Image == null) ? "" : x.User_Master1.Image == "" ? "" : (baseURL + x.User_Master1.Image),
                            IsAccept = x.IsAccept == null ? false : x.IsAccept.Value,
                            IsReject = x.IsReject == null ? false : x.IsReject.Value,

                            AcceptedDatetime = x.AcceptDatetime == null ? "" : string.IsNullOrEmpty(userdataTimeZone) ? x.AcceptDatetime.Value : Acceptdatetimesetcae(userdataTimeZone, x.AcceptDatetime.Value, x.TimeZoneId),

                            CallingDateTime1 = x.CallingDateTime1 == null ? "" : string.IsNullOrEmpty(userdataTimeZone) ? x.CallingDateTime1.Value : datetimesetcae1(userdataTimeZone, x.CallingDateTime1.Value, x.TimeZoneId),
                            CallingDateTime2 = x.CallingDateTime2 == null ? "" : string.IsNullOrEmpty(userdataTimeZone) ? x.CallingDateTime2.Value : datetimesetcae1(userdataTimeZone, x.CallingDateTime2.Value, x.TimeZoneId),
                            CallingDateTime3 = x.CallingDateTime3 == null ? "" : string.IsNullOrEmpty(userdataTimeZone) ? x.CallingDateTime3.Value : datetimesetcae1(userdataTimeZone, x.CallingDateTime3.Value, x.TimeZoneId),
                            IsCalling = CheckCall(x.Id),


                            //CallingDateTime1 = Convert.ToDateTime(x.CallingDateTime1).ToString("dd MMM yyyy HH:mm"),
                            //CallingDateTime2 = x.CallingDateTime2 == null ? null : Convert.ToDateTime(x.CallingDateTime2).ToString("dd MMM yyyy HH:mm"),
                            //CallingDateTime3 = x.CallingDateTime3 == null ? null : Convert.ToDateTime(x.CallingDateTime3).ToString("dd MMM yyyy HH:mm"),
                            RejectedNote = x.RejectedNote == null ? "" : x.RejectedNote
                        }).OrderBy(x => x.Displayname).ToList()
                        
                };
                return uvlist;
            }
                catch(Exception ex)
            {
                return new userdetailcallinglist()
                {
                    MyCalling = new List<userdetailcalling>() { },
                    OtherCalling = new List<userdetailcalling>()
                };
            }

        }
    }

    //public dynamic GetGroupListDatas(int userId)
    //{
    //    HomeViewModel HVM = new HomeViewModel();
    //    string baseURL = HttpContext.Current.Request.Url.Authority;
    //    baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
    //    using (LystenEntities db = new LystenEntities())
    //    {
    //        var obj1 = db.UserCategoryMappings.Where(x => x.UserId == userId).Select(x => x.CategoryId).ToList();
    //        List<HomeTopicModel> HTMM = new List<HomeTopicModel>();
    //        List<int> ssa = new List<int>();
    //        foreach (var item in obj1)
    //        {
    //            var data = db.Groups.Where(x => x.GroupTypeId == item.Value).Select(x => x.Id).Distinct().ToList();
    //            foreach (var itemaa in data)
    //            {


    //                if (!ssa.Contains(itemaa))
    //                    ssa.Add(itemaa);
    //            }
    //        }
    //        //var obj3 = (from db1 in db.Questions
    //        //            join ss in ssa on db1.Id equals ss
    //        //            select db1
    //        //           ).ToList();

    //        var obj4 = (from db1 in ssa
    //                    select new HomeQuestion()
    //                    {
    //                        QuestionId = db1.Id,
    //                        QuestionDisplayText = db1.DisplayText,
    //                        QuestionDescription = db1.Description ?? "",
    //                        UserId = (int)db1.UserId,
    //                        UserImage = baseURL + db.User_Master.Where(z => z.Id == db1.UserId).Select(y => y.Image).FirstOrDefault(),
    //                        IsSubscribe = db.UserQuestionSubscribes.Where(x => x.UserId == db1.UserId && x.QuestionsId == db1.Id).FirstOrDefault() == null ? false : true,
    //                        Answered = GetAnswerList(db1.Id),
    //                        CreatedDate = db1.CreatedDate
    //                    }).OrderByDescending(x => x.CreatedDate).ToList();

    //        foreach (var item in obj1)
    //        {
    //            HomeTopicModel HM = new HomeTopicModel();
    //            List<HomeQuestion> HomeQuestion = new List<HomeQuestion>();

    //            HomeQuestion HQ = new HomeQuestion();

    //            HM.TopicId = item.Value;
    //            HM.Name = db.Categories.Where(x => x.Id == item.Value).Select(x => x.Name).FirstOrDefault();
    //            HM.Description = db.Categories.Where(x => x.Id == item.Value).Select(x => x.Description).FirstOrDefault();
    //            var data = db.QuestionsCategoryMappings.Where(x => x.CategoryId == item.Value).Select(x => x.QuestionId).Distinct().ToList();
    //            foreach (var itemaa in data)
    //            {
    //                HQ = obj4.Where(x => x.QuestionId == itemaa).FirstOrDefault();
    //                HomeQuestion.Add(HQ);
    //            }
    //            HM.Question = HomeQuestion;
    //            HM.Description = "";
    //            HTMM.Add(HM);
    //        }

    //        HVM.TopicModel = HTMM;
    //        HVM.UserId = userId;
    //        return HVM;
    //    }
    //}
    public UserMasterModel GetUserDetail(int UserId)
    {
        using (LystenEntities db = new LystenEntities())
        {
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");

            var user = db.User_Master.Where(x => x.Id == UserId).ToList();


            UserMasterModel uvlist = user.Select(x => new UserMasterModel()
            {

                Id = x.Id,
                Email = x.Email,
                Displayname = x.UserName == null ? "" : x.UserName,
                Image = (x.Image == null) ? "" : x.Image == "" ? "" : (baseURL + x.Image),
                DeviceToken = x.DeviceToken,

            }).FirstOrDefault();
            return uvlist;
        }
    }
}
}