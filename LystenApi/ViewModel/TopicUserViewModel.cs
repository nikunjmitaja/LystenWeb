using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LystenApi.ViewModel
{
    public class TopicUserViewModel
    {
        public string TopicName { get;  set; }
        public int? UserId { get;  set; }
        public string TopicDescription { get;  set; }
    }
}