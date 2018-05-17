using LystenApi.Utility.ApiServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace LystenApi.Controllers
{
    public class TopicPageController : Controller
    {
        ApiTopicServices ApiTopic = new ApiTopicServices();

        public ActionResult Topic(string Slug)
        {
            var mm = ApiTopic.GetTopicBySlug(Slug);
            return PartialView(mm);
        }
    }
}
