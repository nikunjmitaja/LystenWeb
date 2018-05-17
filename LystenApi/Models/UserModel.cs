using LystenApi.Db;
using LystenApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LystenApi.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Displayname { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public List<ProfileFavourite> Favourite { get; set; }
    }

    public class UserViewModelProfile
    {
        public string FullName { get; set; }
        public int Id { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }

        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }

        public string Email { get; set; }
        public string Displayname { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }

        public string CollegeUniversity { get; set; }
        public string Degree { get; set; }
        public string Occupation { get; set; }
        public string UserName { get; set; }

        public List<ProfileFavourite> Favourite { get; set; }
        public int TotalQuestionsCount { get;  set; }
        public int TotalEventsCount { get;  set; }

        public Nullable<bool> IsVerified { get; set; }
        public string SSN { get; set; }

        public string PostalCode { get; set; }

        public string Skill { get; set; }

        public string DateOfBirth { get; set; }

        
    }


    public class UserViewModelProfileView
    {
        public string FullName { get; set; }
        public int Id { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string Email { get; set; }
        public string Displayname { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string CollegeUniversity { get; set; }
        public string Degree { get; set; }
        public string Occupation { get; set; }
        public bool IsFavourite { get; set; }
        public bool? IsOnline { get;  set; }
        public List<HomeQuestion> Question { get; set; }
        public int CallingRequestId { get;  set; }
        public Nullable<bool> IsVerified { get; set; }
    }

    public class OnlineUserViewModel
    {
        public int TotalCount { get; set; }
        public List<userdetail> userdetail { get; set; }
    }

    public class userdetailcalling
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Displayname { get; set; }
        public string Image { get; set; }
        public bool IsAccept { get; set; }
        public bool IsReject { get; set; }
        public string CallingDateTime1 { get; set; }
        public string CallingDateTime2 { get; set; }
        public string CallingDateTime3 { get; set; }
        public string RejectedNote { get; set; }
        public int FromUserId { get;  set; }
        public int ToUserId { get;  set; }
        public bool IsCalling { get;  set; }
        public dynamic AcceptedDatetime { get;  set; }
    }

    public class userdetailcallinglist
    {
        public List<userdetailcalling> OtherCalling { get; set; }
        public List<userdetailcalling> MyCalling { get; set; }

    }

    public class userdetail
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Displayname { get; set; }
        public string Image { get; set; }
    }
    public class UserViewLoginModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Displayname { get; set; }
        public string Image { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public bool IsDisclaimer { get; set; }

        
    }

    public class ProfileFavourite
    {
        public int UserId { get; set; }
        public int FavoriteUserId { get; set; }
        public string FavoriteUserImage { get;  set; }
        public string Name { get;  set; }
    }


    public class ViewProfileFavourite
    {
        public int UserId { get; set; }
        public int FavoriteUserId { get; set; }
        public string FavoriteUserImage { get; set; }
        public string Name { get; set; }
    }
    

    public class UserMasterModel
    {
        public string UserNameorEmail { get; set; }
        public dynamic Image { get; set; }
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string SessionId { get; set; }
        public string Displayname { get; set; }
        public string DeviceToken { get; set; }
        public string Address { get; set; }
        public bool RememberMe { get; set; }
        public Nullable<int> Country { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> City { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public int CompanyId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> Createdby { get; set; }
        public Nullable<System.DateTime> Createdate { get; set; }
        public Nullable<System.DateTime> Modifydate { get; set; }
        public virtual City_Master City_Master { get; set; }
        public virtual Country_Master Country_Master { get; set; }
        public virtual State_Master State_Master { get; set; }
        public string UserName { get;  set; }
        public string DeviceType { get; set; }

        public bool IsVerified { get; set; }

        public string UserType { get; set; }

        public string TimeZone { get; set; }
        //public HttpPostedFileBase Image { get; set; }
    }

    public class UserModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string SessionId { get; set; }
        public string Displayname { get; set; }
        public string Address { get; set; }
        public Nullable<int> Country { get; set; }
        public string Countryname { get; set; }
        public Nullable<int> State { get; set; }
        public string Statename { get; set; }
        public Nullable<int> City { get; set; }
        public string Cityname { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public System.DateTime Expirydate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> Createdby { get; set; }
        public Nullable<System.DateTime> Createdate { get; set; }
        public Nullable<System.DateTime> Modifydate { get; set; }
        public string Allmodids { get; set; }
        public int CompanyId { get; set; }
        public string Companyname { get; set; }
        public string Website { get; set; }

        /// <summary>
        ///  Change Password
        /// </summary>
        public string Currentpassword { get; set; }
        public string Newpassword { get; set; }

    }
}