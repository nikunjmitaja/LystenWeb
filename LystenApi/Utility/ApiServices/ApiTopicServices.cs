using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using LystenApi.Db;
using LystenApi.ViewModel;
using System.Data;
using LystenApi.Controllers.Api;
using System.Net;
using System.Threading.Tasks;
using LystenApi.Models;

namespace LystenApi.Utility.ApiServices
{
    public class ApiTopicServices
    {
        public dynamic GetTopicBySlug(string slug)
        {
            TopicViewModel Topic = new TopicViewModel();
            using (LystenEntities db = new LystenEntities())
            {
                string baseURL = HttpContext.Current.Request.Url.Authority;
                var obj = db.Topics.Where(x => x.Slug == slug).FirstOrDefault();
                Topic = AutoMapper.Mapper.Map<Topic, TopicViewModel>(obj);
                Topic.URL = baseURL + "/topic/" + slug;

            }
            return Topic;
        }
    }
}