using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using LystenApi.Db;
using LystenApi.Models;
using System.Data;

namespace LystenApi.Utility
{
    public class UserServices
    {
        EmailServices ES = new EmailServices();
        public dynamic SaveUser(UserModel objUM)
        {
            using (LystenEntities db = new LystenEntities())
            {
                if (objUM.Id > 0)
                {
                    var user = db.User_Master.Where(x => x.Id == objUM.Id).FirstOrDefault();
                    var sendflag = false;
                    if (user.Password != objUM.Password)
                    {
                        sendflag = true;
                    }
                    //user.Email = objUM.Email;
                    user.Password = objUM.Password;
                    user.Address = objUM.Address;
                    user.CountryId = objUM.Country;
                    user.StateId = objUM.State;
                    user.CityId = objUM.City;
                    user.Phone = objUM.Phone;
                    user.Mobile = objUM.Mobile;
                    user.Displayname = objUM.Displayname;
                    user.IsActive = true;
                    user.Modifydate = System.DateTime.Now;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    if (sendflag == true)
                    {
                        ES.SendToUser(user);
                    }
                    return new { Id = user.Id, Status = "Update" };
                }
                else
                {
                    if (db.User_Master.Any(x => x.Email == (objUM.Email).Trim()))
                    {
                        objUM.Id = db.User_Master.Where(x => x.Email == (objUM.Email).Trim()).Select(x => x.Id).FirstOrDefault();
                        return new { Id = objUM.Id, Status = "Exists" };
                    }
                    else
                    {
                        User_Master user = new User_Master();
                        user.Email = objUM.Email;
                        user.Password = objUM.Password;
                        user.Address = objUM.Address;
                        user.CountryId = objUM.Country;
                        user.StateId = objUM.State;
                        user.CityId = objUM.City;
                        user.Phone = objUM.Phone;
                        user.Mobile = objUM.Mobile;
                        user.Displayname = objUM.Displayname;
                        user.IsActive = true;
                        user.Createdate = System.DateTime.Now;
                        user.Modifydate = System.DateTime.Now;
                        user.Createdby = 1;
                        db.User_Master.Add(user);
                        db.SaveChanges();
                        ES.SendToUser(user);
                        return new { Id = user.Id, Status = "Insert" };
                    }
                }
            }
        }


        public dynamic GetAllUser()
        {
            using (LystenEntities db = new LystenEntities())
            {
                var id = HttpContext.Current.Request.Cookies["Userid"].Value;
                if (id == "1")
                {
                    return db.User_Master.Include(x => x.City_Master).Include(x => x.State_Master).Include(x => x.Country_Master).OrderBy(x => x.Displayname).ToList();
                }
                else
                {
                    return db.User_Master.Include(x => x.City_Master).Include(x => x.State_Master).Include(x => x.Country_Master).OrderBy(x => x.Displayname).Where(x => x.Id != 1).ToList();
                }
            }
        }


        public dynamic Profile(int id)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var result = db.User_Master.Include(x => x.Country_Master).Include(x => x.State_Master).Include(x => x.City_Master).Where(x => x.Id == id).FirstOrDefault();
                return result;
            }
        }

        public dynamic Changepassword(UserModel objcurpwd)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var paswrdenc = SecutiryServices.EncodePasswordToBase64(objcurpwd.Currentpassword);
                User_Master result = (from um in db.User_Master
                                      where um.Id == objcurpwd.Id &&
                                         um.Password == paswrdenc
                                      select um
                               ).FirstOrDefault();
                if (result != null)
                {
                    result.Password = SecutiryServices.EncodePasswordToBase64(objcurpwd.Newpassword);
                    db.Entry(result).State = EntityState.Modified;
                    db.SaveChanges();
                    return "Success";
                }
                else
                {
                    return "Invalid Password.";
                }
            }
        }
    }
}