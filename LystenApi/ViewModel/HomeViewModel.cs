using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LystenApi.ViewModel
{
    public class HomeViewModel
    {
        public int UserId { get; set; }
        public List<HomeTopicModel> TopicModel { get; set; }
    }



    public class HomeViewModelQ
    {
        public List<nEWHomeQuestion> Question { get; set; }
    }



    public class OnlyHomeViewModel
    {
        public int UserId { get; set; }
        public List<OnlyHomeTopicModel> TopicModel { get; set; }
    }
    public class HomeTopicModel
    {
        public int TopicId { get; set; }
        public string Name { get; set; }
        public int TotalCount { get; set; }

        public string Description { get; set; }
        public List<HomeQuestion> Question { get; set; }
    }


    public class OnlyHomeTopicModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class nEWHomeQuestion
    {
        public int QuestionId { get; set; }
        public string QuestionDisplayText { get; set; }
        public string QuestionDescription { get; set; }
        public int UserId { get; set; }
        public string UserImage { get; set; }
        public bool IsSubscribe { get; set; }
        public String CreatedDate { get; set; }
        public string UserName { get; set; }

    }
    public class HomeQuestion
    {
        public int QuestionId { get; set; }
        public string QuestionDisplayText { get; set; }
        public string QuestionDescription { get; set; }
        public int UserId { get; set; }
        public string UserImage { get; set; }
        public bool IsSubscribe { get; set; }
        public String CreatedDate { get; set; }
        public string UserName { get; set; }

        public List<HomeAnswer> Answered { get; set; }
    }
    public class HomeAnswer
    {
        public int AnswerId { get; set; }
        public string AnswerDisplayText { get; set; }
        public string AnswerDescription { get; set; }
        public int UserId { get; set; }
        public string UserImage { get; set; }
        public bool IsSubscribe { get; set; }
        public string CreatedDate { get; set; }
        public string UserName { get; set; }
        public string QuestionName { get; set; }
    }

    public class HomeAnswerList
    {
        public string QuestionName { get; set; }
        public List<AnswerList> Answer { get; set; }


    }
    public class AnswerList
    {
        public int AnswerId { get; set; }
        public string AnswerDisplayText { get; set; }
        public string AnswerDescription { get; set; }
        public int UserId { get; set; }
        public string UserImage { get; set; }
        public bool IsSubscribe { get; set; }
        public string CreatedDate { get; set; }
        public string UserName { get; set; }
    }
}