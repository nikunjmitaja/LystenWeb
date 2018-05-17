using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using LystenApi.Db;
using LystenApi.Models;
using LystenApi.ViewModel;
using System.Web.Configuration;

namespace LystenApi.Utility
{
    public class MasterServices
    {
        //TPEntities db = new TPEntities();

        public dynamic GetAllCountry()
        {
            using (LystenEntities db = new LystenEntities())
            {
                return db.Country_Master.OrderBy(x => x.Name).ToList();
            }
        }

        public List<User_Master> GetAllUser()
        {
            using (LystenEntities db = new LystenEntities())
            {
                return db.User_Master.OrderByDescending(x => x.Id).ToList();
            }
        }

        public dynamic GetCountry()
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = (from db1 in db.Country_Master
                           where db1.IsActive == true
                           select new MasterModel()
                           {
                               CountryName = db1.Name,
                               CountryId = db1.Id
                           }).ToList();
                return obj;
            }
        }

        public dynamic GetCategoryDDl()
        {
            using (LystenEntities db = new LystenEntities())
            {
                return new SelectList(db.Categories.Where(x => x.IsActive == true).ToList(), "Id", "Name");
            }
        }
        public string GetRoleName(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                try
                {
                    if (id == 0) return "";
                    return db.RoleMasters.Where(x => x.RoleID == id).FirstOrDefault().Name;
                }
                catch
                {
                    return "";
                }

            }
        }



        public dynamic SaveCountry(Country_Master CM)
        {
            using (LystenEntities db = new LystenEntities())
            {
                if (CM.Id > 0)
                {
                    var country = db.Country_Master.Where(x => x.Id == CM.Id).FirstOrDefault();
                    country.ModifyDate = System.DateTime.Now;
                    country.Name = CM.Name;
                    country.IsActive = CM.IsActive;
                    country.CountryCode = CM.CountryCode;
                    db.Entry(country).State = EntityState.Modified;
                    db.SaveChanges();
                    return new { Id = CM.Id, Status = "Update" };
                }
                else
                {
                    if (db.Country_Master.Any(x => x.Name == (CM.Name).Trim()))
                    {
                        CM.Id = db.Country_Master.Where(x => x.Name == (CM.Name).Trim()).Select(x => x.Id).FirstOrDefault();
                        return new { Id = CM.Id, Status = "Exists" };
                    }
                    else
                    {
                        CM.CreatedDate = System.DateTime.Now;
                        CM.CreatedBy = 1;
                        db.Country_Master.Add(CM);
                        db.SaveChanges();
                        return new { Id = CM.Id, Status = "Insert" };
                    }
                }
            }
        }

        public object ActiveDeActiveUser(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var User = db.User_Master.Where(x => x.Id == id).FirstOrDefault();
                if (User.IsActive == true)
                    User.IsActive = false;
                else
                    User.IsActive = true;
                db.Entry(User).State = EntityState.Modified;
                db.SaveChanges();
                return new { Id = 1, Status = User.IsActive };
            }
        }

        public object MarkAsVerified(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var User = db.User_Master.Where(x => x.Id == id).FirstOrDefault();
                if (User.IsVerified == null || User.IsVerified == false)
                    User.IsVerified = true;

                db.Entry(User).State = EntityState.Modified;
                db.SaveChanges();
                try
                {
                    Helpers.EmailHelper.SendEmail(User.Email, User.UserName, User.Password);
                }
                catch
                {

                }


                return new { Id = 1, Status = User.IsActive };
            }
        }

        public dynamic GetUserById(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.User_Master.Where(x => x.Id == id).FirstOrDefault();
                return AutoMapper.Mapper.Map<User_Master, UserViewModelProfile>(obj);
            }

        }

        public dynamic EditCountry(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.Country_Master.Where(x => x.Id == id).FirstOrDefault();

                MasterModel mm = new MasterModel();
                mm.CountryCode = obj.CountryCode;
                mm.Name = obj.Name;
                mm.Id = obj.Id;
                mm.IsActive = obj.IsActive;
                return mm;
            }
        }

        public void updatesession(User_Master objuser)
        {
            using (LystenEntities db = new LystenEntities())
            {
                db.Entry(objuser).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public dynamic DeleteCountry(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var country = db.Country_Master.Where(x => x.Id == id).FirstOrDefault();
                country.IsActive = false;
                db.Entry(country).State = EntityState.Modified;
                db.SaveChanges();
                return new { Id = 1, Status = "Delete" };
            }
        }


        public dynamic Getuserbyid(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var result = db.User_Master.Where(x => x.Id == id).FirstOrDefault();
                // db.Dispose();
                return result;
            }
        }


        public dynamic EditEmailTemplateData(int Id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.EmailTemplates.Where(x => x.Id == Id).FirstOrDefault();
                return obj;
            }
        }

        public dynamic SaveEmailAccountsData(EmailAccount UM)
        {
            using (LystenEntities db = new LystenEntities())
            {
                if (UM.Id == 0)
                {
                    db.EmailAccounts.Add(UM);
                    db.SaveChanges();
                    return new { AccountId = UM.Id, Status = "Insert" };
                }
                else
                {
                    var obj = db.EmailAccounts.Where(x => x.Id == UM.Id).FirstOrDefault();
                    obj.EmailId = UM.EmailId;
                    obj.Password = UM.Password;
                    obj.Port = UM.Port;
                    obj.SMTPRelay = UM.SMTPRelay;
                    obj.EnableSSL = UM.EnableSSL;
                    obj.SMTPRelay = UM.SMTPRelay; db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    return new
                    {
                        AccountId
                            = UM.Id,
                        Status = "Update"
                    };
                }
            }
        }

        public dynamic GetAllEmailAccount(jQueryDataTableParamModel param)
        {
            using (LystenEntities db = new LystenEntities())
            {
                return db.EmailAccounts.ToList();
            }
        }


        public dynamic SaveEmailTemplateData(EmailTemplate UM)
        {
            try
            {
                using (LystenEntities db = new LystenEntities())
                {
                    EmailTemplate emailtemp = new EmailTemplate();
                    if (UM.Id != 0)
                    {
                        emailtemp = db.EmailTemplates.Where(x => x.Id == UM.Id).FirstOrDefault();
                        emailtemp.Title = UM.Title;
                        emailtemp.Subject = UM.Subject;
                        emailtemp.Body = UM.Body;
                        emailtemp.SystemName = UM.SystemName;
                        db.Entry(emailtemp).State = EntityState.Modified;
                        db.SaveChanges();
                        return new { TemplateId = UM.Id, Status = "Update" };
                    }
                    else
                    {
                        if (db.EmailTemplates.Any(x => x.SystemName == UM.SystemName))
                        {
                            return new { TemplateId = UM.Id, Status = "Exists" };
                        }
                        db.EmailTemplates.Add(UM);
                        db.SaveChanges();
                        return new { TemplateId = UM.Id, Status = "Insert" };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public dynamic GetAllCategory()
        {
            using (LystenEntities db = new LystenEntities())
            {
                return db.Categories.ToList();
            }
        }

        public dynamic GetEmailAccountById(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                return db.EmailAccounts.Where(x => x.Id == id).FirstOrDefault();
            }
        }
        public List<EmailTemplate> GetAllEmailTemplates(jQueryDataTableParamModel param)
        {
            using (LystenEntities db = new LystenEntities())
            {
                return db.EmailTemplates.ToList();
            }
        }

        public dynamic EditCategory(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.Categories.Where(x => x.Id == id).FirstOrDefault();
                return AutoMapper.Mapper.Map<Category, CategoryMasterViewModel>(obj);
            }
        }

        public dynamic SaveCategory(Category CM)
        {
            using (LystenEntities db = new LystenEntities())
            {
                if (CM.Id > 0)
                {
                    var Category = db.Categories.Where(x => x.Id == CM.Id).FirstOrDefault();
                    Category.ModifyDate = System.DateTime.Now;
                    Category.Name = CM.Name;
                    Category.Description = CM.Description;
                    db.Entry(Category).State = EntityState.Modified;
                    db.SaveChanges();
                    return new { Id = CM.Id, Status = "Update" };
                }
                else
                {
                    if (db.Categories.Any(x => x.Name == (CM.Name).Trim()))
                    {
                        CM.Id = db.Categories.Where(x => x.Name == (CM.Name).Trim()).Select(x => x.Id).FirstOrDefault();
                        return new { Id = CM.Id, Status = "Exists" };
                    }
                    else
                    {
                        CM.CreatedDate = System.DateTime.Now;
                        CM.IsActive = true;
                        db.Categories.Add(CM);
                        db.SaveChanges();
                        return new { Id = CM.Id, Status = "Insert" };
                    }
                }
            }
        }

        public dynamic ActiveDeActiveCategory(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var Categories = db.Categories.Where(x => x.Id == id).FirstOrDefault();
                if (Categories.IsActive == true)
                    Categories.IsActive = false;
                else
                    Categories.IsActive = true;
                db.Entry(Categories).State = EntityState.Modified;
                db.SaveChanges();
                return new { Id = 1, Status = Categories.IsActive };
            }
        }

        public List<Event> GetAllEvent()
        {
            using (LystenEntities db = new LystenEntities())
            {
                return db.Events.OrderByDescending(x => x.Id).Include(x => x.Category).ToList();
            }
        }

        public EventViewModel GetEventById(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.Events.Where(x => x.Id == id).FirstOrDefault();
                return AutoMapper.Mapper.Map<Event, EventViewModel>(obj);
            }
        }

        public object ActiveDeActiveEvent(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var Event = db.Events.Where(x => x.Id == id).FirstOrDefault();
                if (Event.IsActive == true)
                    Event.IsActive = false;
                else
                    Event.IsActive = true;
                db.Entry(Event).State = EntityState.Modified;
                db.SaveChanges();
                return new { Id = 1, Status = Event.IsActive };
            }
        }

        public List<CategoryImageEvent> GetAllEventCategoryimage()
        {
            using (LystenEntities db = new LystenEntities())
            {
                return db.CategoryImageEvents.Include(X => X.Category).ToList();
            }
        }

        public List<Calling_Request> GetAllCalling()
        {
            using (LystenEntities db = new LystenEntities())
            {
                return db.Calling_Request.OrderByDescending(x => x.Id).Include(x => x.User_Master).Include(x => x.User_Master1).ToList();
                // return db.Calling_Request.OrderByDescending(x => x.Id).Include(x => x.User_Master1).ToList();
            }
        }

        #region Calling Price
        public List<CallingPriceMaster> GetAllCallingPrices()
        {
            using (LystenEntities db = new LystenEntities())
            {
                return db.CallingPriceMasters.ToList();
                // return db.Calling_Request.OrderByDescending(x => x.Id).Include(x => x.User_Master1).ToList();
            }
        }
        public dynamic SaveCallingPriceMaster(CallingPriceMaster viewModel)
        {
            using (LystenEntities db = new LystenEntities())
            {
                if (viewModel.Id > 0)
                {
                    var callingPrice = db.CallingPriceMasters.Where(x => x.Id == viewModel.Id).FirstOrDefault();
                    callingPrice.ModifiedDate = System.DateTime.Now;
                    callingPrice.Name = viewModel.Name;
                    callingPrice.Description = viewModel.Description;
                    callingPrice.Price = viewModel.Price;
                    callingPrice.Time = viewModel.Time;
                    callingPrice.ModifiedDate = DateTime.Now;
                    db.Entry(callingPrice).State = EntityState.Modified;
                    db.SaveChanges();
                    return new { Id = viewModel.Id, Status = "Update" };
                }
                else
                {
                    if (db.Categories.Any(x => x.Name == (viewModel.Name).Trim()))
                    {
                        viewModel.Id = db.Categories.Where(x => x.Name == (viewModel.Name).Trim()).Select(x => x.Id).FirstOrDefault();
                        return new { Id = viewModel.Id, Status = "Exists" };
                    }
                    else
                    {

                        viewModel.IsActive = true;
                        viewModel.CreatedDate = DateTime.Now;
                        viewModel.ModifiedDate = DateTime.Now;
                        db.CallingPriceMasters.Add(viewModel);
                        db.SaveChanges();
                        return new { Id = viewModel.Id, Status = "Insert" };
                    }
                }


            }
        }

        public object ActivedeActiveCallingPrice(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var Event = db.CallingPriceMasters.Where(x => x.Id == id).FirstOrDefault();
                if (Event.IsActive == true)
                    Event.IsActive = false;
                else
                    Event.IsActive = true;
                db.Entry(Event).State = EntityState.Modified;
                db.SaveChanges();
                return new { Id = 1, Status = Event.IsActive };
            }
        }

        public dynamic EditCallPrice(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.CallingPriceMasters.Where(x => x.Id == id).FirstOrDefault();
                return new { Name = obj.Name, Description = obj.Description, Id = obj.Id, Time = obj.Time, Price = obj.Price, IsActive = obj.IsActive };
            }
        }

        #endregion


        public dynamic SaveEvent(Event CM)
        {
            using (LystenEntities db = new LystenEntities())
            {
                if (CM.Id > 0)
                {
                    var Event = db.Events.Where(x => x.Id == CM.Id).FirstOrDefault();
                    Event.ModifiedDate = System.DateTime.Now;
                    Event.Description = CM.Description;
                    Event.Title = CM.Title;
                    Event.CategoryId = CM.CategoryId;
                    Event.IsActive = CM.IsActive;
                    Event.Location = CM.Location;
                    db.Entry(Event).State = EntityState.Modified;
                    db.SaveChanges();
                    return new { Id = CM.Id, Status = "Update" };
                }
                else
                {
                    if (db.Events.Any(x => x.Title == (CM.Title).Trim()))
                    {
                        CM.Id = db.Events.Where(x => x.Title == (CM.Title).Trim()).Select(x => x.Id).FirstOrDefault();
                        return new { Id = CM.Id, Status = "Exists" };
                    }
                    else
                    {
                        CM.IsActive = true;
                        CM.CreatedDate = System.DateTime.Now;
                        CM.CreatedBy = 1;
                        db.Events.Add(CM);
                        db.SaveChanges();
                        return new { Id = CM.Id, Status = "Insert" };
                    }
                }
            }
        }

        public dynamic EditEventCategoryimage(int id)
        {
            string baseURL = (WebConfigurationManager.AppSettings["categoryeventimage"]).Replace("~", "");

            using (LystenEntities db = new LystenEntities())
            {
                return db.CategoryImageEvents.Where(x => x.Id == id).Select(x => new CategoryEventImageVM()
                {

                    Image = "/Images/categoryeventimage/" + x.Image,
                    CategoryId = x.CategoryId.Value,
                    Id = x.Id
                }).FirstOrDefault();
            }
        }

        internal dynamic SaveEventCategoryimage(CategoryImageEvent CM)
        {

            using (LystenEntities db = new LystenEntities())
            {
                if (CM.Id > 0)
                {
                    var CategoryImageEvents = db.CategoryImageEvents.Where(x => x.Id == CM.Id).FirstOrDefault();
                    CategoryImageEvents.Image = CM.Image;
                    db.Entry(CategoryImageEvents).State = EntityState.Modified;
                    db.SaveChanges();
                    return new { Id = CM.Id, Status = "Update" };
                }
                else
                {
                    if (db.CategoryImageEvents.Any(x => x.CategoryId == (CM.CategoryId)))
                    {
                        CM.Id = db.CategoryImageEvents.Where(x => x.CategoryId == (CM.CategoryId)).Select(x => x.Id).FirstOrDefault();
                        return new { Id = CM.Id, Status = "Exists" };
                    }
                    else
                    {
                        db.CategoryImageEvents.Add(CM);
                        db.SaveChanges();
                        return new { Id = CM.Id, Status = "Insert" };
                    }
                }
            }
        }
    }
}