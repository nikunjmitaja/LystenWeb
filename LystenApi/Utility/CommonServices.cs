using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using LystenApi.Db;
using LystenApi.Models;
using System.Data;

namespace LystenApi.Utility
{
    public class CommonServices
    {
        ResultClass objresult = new ResultClass();
        //TPEntities db = new TPEntities();
        EmailServices es = new EmailServices();

        public string Convertdatetostring(DateTime dt)
        {
            return dt.ToString("dd MMM yyyy");
        }

        public static void ErrorLogging(Exception ex)
        {
            string strPath = "";
              
            try
            {
                  strPath = System.Web.Hosting.HostingEnvironment.MapPath("~/ErrorLog/Log_" + System.DateTime.Now.ToString("ddMMyyyy") + ".txt");
                if (!File.Exists(strPath))
                {
                    File.Create(strPath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(strPath))
                {
                    sw.WriteLine("");
                    sw.WriteLine("=============Error Logging ===========");
                    sw.WriteLine("-------------------Start------------------- " + DateTime.Now);
                    sw.WriteLine("");
                    sw.WriteLine("Inner Message: " + ex.InnerException);
                    sw.WriteLine("Error Message: " + ex.Message);
                    sw.WriteLine("Stack Trace: " + ex.StackTrace);
                    sw.WriteLine("");
                    sw.WriteLine("-------------------End------------------- " + DateTime.Now);
                    sw.WriteLine("");
                    sw.Close();
                }
            }
            catch {   }
        }

        public dynamic PostLoginAuthentication(UserMasterModel objtblusermaster)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var paswrdenc = SecutiryServices.EncodePasswordToBase64(objtblusermaster.Password);
                var result = (from um in db.User_Master
                              where um.Email.ToUpper() == objtblusermaster.Email.ToUpper() && um.Password == paswrdenc && um.IsActive == true
                              select um
                             ).ToList();

                if (result.Count > 0)
                {
                    int id = result.Select(y => y.Id).FirstOrDefault();
                    var chkToken = db.AppAccessTokens.Where(x => x.UserId == id).FirstOrDefault();
                    if (chkToken != null)
                    {
                        db.Entry(chkToken).State = EntityState.Deleted;
                        db.SaveChanges();
                    }

                    objresult.Code = 200;
                    objresult.Msg = "Success";
                    objresult.Data = result;
                }
                else
                {
                    objresult.Code = 0;
                    objresult.Msg = "Something Went Wrong.";
                    objresult.Data = result;
                }
                return objresult;
            }
        }

        public dynamic GetforgotPassword(string emailId)
        {
            using (LystenEntities db = new LystenEntities())
            {
                var obj = db.User_Master.Where(x => x.Email == emailId).Select(x => x.Password).FirstOrDefault();
                if (obj != null)
                {
                    es.SendUserForgotPassword(emailId, obj);
                    objresult.Code = Convert.ToInt32(HttpStatusCode.OK);
                    objresult.Msg = HttpStatusCode.OK.ToString();
                    objresult.Data = 1;
                }
                else
                {
                    objresult.Code = Convert.ToInt32(HttpStatusCode.NotFound);
                    objresult.Msg = HttpStatusCode.NotFound.ToString();
                    objresult.Data = 0;
                }
                return objresult.Data;
            }
        }
    }
}