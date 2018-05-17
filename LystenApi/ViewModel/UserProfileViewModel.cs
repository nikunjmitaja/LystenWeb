using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LystenApi.ViewModel
{

    public class UsersViewModelGroup
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Image { get; set; }
        public bool IsSelected { get;  set; }
    }

    public class UserProfileViewModel
    {
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string Email { get; set; }
        public string Displayname { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public int UserId { get; set; }
        public string DeviceToken { get; set; }
        public string CompanyName { get; internal set; }
        public int CompanyId { get; internal set; }
    }


    public class UserProfileViewModelSave
    {
        public string Rights { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string Email { get; set; }
        public string Displayname { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public int Id { get; set; }
        public string DeviceToken { get; set; }
        public string CompanyName { get;  set; }
        public string Password { get;  set; }
    }
    public class UserProfileViewModelSaveList
    {
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        public string StateName { get; set; }
        public bool IsActive { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
    }

    public class UserProfileViewModelSaveBYId
    {
    
        public int Id { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string StateName { get; set; }
        public bool IsActive { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }

    }
}