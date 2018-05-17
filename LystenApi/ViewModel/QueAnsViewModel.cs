using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LystenApi.ViewModel
{
    public class QueAnsViewModel
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public int QuestionId { get; set; }
        public string QuestionDescription { get; set; }
        public int UserId { get; set; }
        public string TopicId { get; set; }
        public string DisplayText { get;  set; }
        public string Description { get;  set; }
    }
}