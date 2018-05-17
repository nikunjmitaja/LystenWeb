using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using LystenApi.Db;
using LystenApi.Models;
using LystenApi.ViewModel;
using static LystenApi.Controllers.Api.MessageController;

namespace LystenApi.Mappers
{
    public class GlobalMapper
    {
        public static void Init()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<User_Master, UserViewModel>();
                cfg.CreateMap<Group, GroupViewModel>();
                cfg.CreateMap<Setting, SettingViewModel>();
                cfg.CreateMap<User_Master, UserViewLoginModel>().BeforeMap((s, d) => d.Displayname = s.UserName);

                cfg.CreateMap<Event, EventViewModel>().BeforeMap((s, d) => d.CategoryName = s.Category.Name);

                cfg.CreateMap<User_Master, UserViewModelProfile>().BeforeMap((s, d) => d.CountryName = s.Country_Master != null ? s.Country_Master.Name : "").BeforeMap((s, d) => d.StateName = s.State_Master != null ? s.State_Master.Name : "").BeforeMap((s, d) => d.CityName = s.City_Master!=null?s.City_Master.Name:"").ForAllMembers(opts => opts.Condition((User_Master, UserViewModelProfile, srcMember) => srcMember != null));
                cfg.CreateMap<User_Master, UserViewModelProfileView>().BeforeMap((s, d) => d.CountryName = s.Country_Master != null ? s.Country_Master.Name : "").BeforeMap((s, d) => d.StateName = s.State_Master != null ? s.State_Master.Name : "").BeforeMap((s, d) => d.CityName = s.City_Master != null ? s.City_Master.Name : "").ForAllMembers(opts => opts.Condition((User_Master, UserViewModelProfile, srcMember) => srcMember != null));
                cfg.CreateMap<Category, CategoryViewModel>().ForAllMembers(opts => opts.Condition((Category, CategoryViewModel, srcMember) => srcMember != null));
                cfg.CreateMap<Category, CategoryMasterViewModel>();
                cfg.CreateMap<Topic, TopicViewModel>();
                cfg.CreateMap<UserViewModel, User_Master>();
                cfg.CreateMap<MessageRecipient, MessageModel>().BeforeMap((s, d) => d.CreatorId = s.Message.CreatorId).BeforeMap((s, d) => d.ExpiryDate = s.Message.ExpiryDate).BeforeMap((s, d) => d.ParentMessageId = s.Message.ParentMessageId).BeforeMap((s, d) => d.Body = s.Message.Body).BeforeMap((s, d) => d.CreatedDate = Convert.ToDateTime(s.Message.CreatedDate).ToString("dd MM yyyy"));
                cfg.CreateMap<User_Master, UserMasterModel>();
                cfg.CreateMap<User_Master, UserProfileViewModelSaveList>().BeforeMap((s, d) => d.CountryName = s.Country_Master.Name).BeforeMap((s, d) => d.StateName = s.State_Master.Name).BeforeMap((s, d) => d.CityName = s.City_Master.Name);
                cfg.CreateMap<Country_Master, CountryViewModel>();
                cfg.CreateMap<State_Master, StateViewModel>().BeforeMap((s, d) => d.CountryName = s.Country_Master.Name);
                cfg.CreateMap<City_Master, CityViewModel>().BeforeMap((s, d) => d.CountryName = s.Country_Master.Name).BeforeMap((s, d) => d.StateName = s.State_Master.Name);
            });
        }
    }
}