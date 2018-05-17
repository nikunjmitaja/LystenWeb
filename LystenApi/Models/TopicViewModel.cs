using LystenApi.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LystenApi.Models
{
    public class TopicViewModel
    {
        public int Id { get; set; }
        public string SystemName { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsActive { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string Slug { get; set; }
        public string URL { get;  set; }
        public int UserId { get; set; }
        public string  TopicId { get; set; }
    }
}