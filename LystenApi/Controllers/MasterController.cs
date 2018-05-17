using LystenApi.Db;
using LystenApi.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace LystenApi.Controllers
{
    [Authorize]
    public class MasterController : Controller
    {
        MasterServices MS = new MasterServices();
        UserServices US = new UserServices();
        // GET: Master
        public ActionResult Index()
        {
            return View();
        }

        #region UserList
        public ActionResult User()
        {
            return View();
        }
        public JsonResult GetAllUser(jQueryDataTableParamModel param)
        {
            try
            {
                List<User_Master> allRecords = new List<User_Master>();
                allRecords = MS.GetAllUser();
                List<User_Master> filteredRecords = null;

                if (!string.IsNullOrWhiteSpace(param.sSearch))
                {
                    allRecords = allRecords.Where(t => t.Email.Contains(param.sSearch) || t.UserName != null ? t.UserName.Contains(param.sSearch) : t.UserName == null).ToList();
                    filteredRecords = allRecords.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
                else
                {
                    filteredRecords = allRecords
                                        .Skip(param.iDisplayStart)
                                        .Take(param.iDisplayLength).OrderBy(t => t.Email).ToList();
                }
                int totalRecords = allRecords.Count();
                var result = from c in filteredRecords select new[] { "", c.Id.ToString(), c.Email, c.UserName, c.IsActive == true ? "Active" : "InActive", c.IsVerified == null || c.IsVerified == false ? "Not Verified" : "IsVerified", MS.GetRoleName(c.RoleId.Value), "" };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords,
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                return Json(new
                {
                    sEcho = 1,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<string[]> { }
                }, JsonRequestBehavior.AllowGet);

            }
        }


        public JsonResult GetUserById(int Id)
        {
            return Json(MS.GetUserById(Id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ActiveDeActiveUser(int Id)
        {
            try
            {
                return Json(MS.ActiveDeActiveUser(Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult MarkasVerified(int Id)
        {
            try
            {
                return Json(MS.MarkAsVerified(Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }



        #endregion

        #region EmailAccount

        public ActionResult EmailAccount()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveEmailAccount(EmailAccount UM)
        {
            try
            {
                var user = MS.SaveEmailAccountsData(UM);
                return Json(user, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetEmailAccount(jQueryDataTableParamModel param)
        {
            try
            {
                var allRecords = new List<EmailAccount>();
                allRecords = MS.GetAllEmailAccount(param);
                List<EmailAccount> filteredRecords = null;

                if (!string.IsNullOrWhiteSpace(param.sSearch))
                {
                    allRecords = allRecords.Where(t => t.Id.ToString().Contains(param.sSearch.ToLower()) || t.EmailId.ToLower().ToString().Contains(param.sSearch.ToLower()) || t.Password.ToLower().ToString().Contains(param.sSearch.ToLower()) || t.SMTPRelay.ToLower().ToString().Contains(param.sSearch.ToLower()) || t.Port.ToLower().ToString().Contains(param.sSearch.ToLower()) || t.EnableSSL.ToString().Contains(param.sSearch.ToLower())).OrderBy(t => t.EmailId).ToList();
                    filteredRecords = allRecords.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
                else
                {
                    filteredRecords = allRecords
                                        .Skip(param.iDisplayStart)
                                        .Take(param.iDisplayLength).OrderBy(t => t.EmailId).ToList();
                }
                int totalRecords = allRecords.Count();
                var result = from c in filteredRecords select new[] { "", c.Id.ToString(), c.EmailId, c.Password, c.SMTPRelay, c.Port, c.EnableSSL.ToString(), "" };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords,
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                return Json(new
                {
                    sEcho = 1,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<string[]> { }
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult EditEmailAccount(int Id)
        {
            return Json(MS.GetEmailAccountById(Id), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region EmailTemplates

        public ActionResult EmailTemplates()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAllEmailTemplates(jQueryDataTableParamModel param)
        {
            try
            {
                var allRecords = new List<EmailTemplate>();
                allRecords = MS.GetAllEmailTemplates(param);
                List<EmailTemplate> filteredRecords = null;

                if (!string.IsNullOrWhiteSpace(param.sSearch))
                {
                    allRecords = allRecords.Where(t => t.Id.ToString().Contains(param.sSearch.ToLower()) || t.Title.ToLower().ToString().Contains(param.sSearch.ToLower()) || t.Body.ToLower().ToString().Contains(param.sSearch.ToLower()) || t.SystemName.ToLower().ToString().Contains(param.sSearch.ToLower()) || t.Subject.ToLower().ToString().Contains(param.sSearch.ToLower())).OrderBy(t => t.Title).ToList();
                    filteredRecords = allRecords.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
                else
                {
                    filteredRecords = allRecords
                                        .Skip(param.iDisplayStart)
                                        .Take(param.iDisplayLength).OrderBy(t => t.Title).ToList();
                }
                int totalRecords = allRecords.Count();
                var result = from c in filteredRecords select new[] { "", c.Id.ToString(), c.Title, c.Subject, c.SystemName, c.Body, "" };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords,
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    sEcho = 1,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<string[]> { }
                }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public JsonResult SaveEmailTemplateData(EmailTemplate UM)
        {
            try
            {
                var user = MS.SaveEmailTemplateData(UM);
                return Json(user, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult EditEmailTempData(int TemplateId)
        {
            try
            {
                var user = MS.EditEmailTemplateData(TemplateId);
                return Json(user, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }

        #endregion

        #region Category
        public ActionResult Category()
        {
            return View();
        }

        public JsonResult GetAllCategory(jQueryDataTableParamModel param)
        {
            try
            {
                var allRecords = new List<Category>();
                allRecords = MS.GetAllCategory();
                List<Category> filteredRecords = null;

                if (!string.IsNullOrWhiteSpace(param.sSearch))
                {
                    allRecords = allRecords.Where(t => t.Id.ToString().Contains(param.sSearch.ToLower()) || t.Name.ToLower().ToString().Contains(param.sSearch.ToLower()) || t.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())).OrderBy(t => t.Name).ToList();
                    filteredRecords = allRecords.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
                else
                {
                    filteredRecords = allRecords
                                        .Skip(param.iDisplayStart)
                                        .Take(param.iDisplayLength).OrderBy(t => t.Name).ToList();
                }
                int totalRecords = allRecords.Count();
                var result = from c in filteredRecords select new[] { "", c.Id.ToString(), c.Name, c.IsActive == true ? "Active" : "InActive", "" };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords,
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                return Json(new
                {
                    sEcho = 1,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<string[]> { }
                }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public JsonResult EditCategory(int Id)
        {
            try
            {
                return Json(MS.EditCategory(Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }



        [HttpPost]
        public JsonResult SaveCategory(Category CM)
        {
            try
            {
                return Json(MS.SaveCategory(CM), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult ActiveDeActiveCategory(int Id)
        {
            try
            {
                return Json(MS.ActiveDeActiveCategory(Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }

        #endregion



        #region EventCategoryImage
        public ActionResult EventCategoryimage()
        {
            ViewBag.ddlcategory = MS.GetCategoryDDl();
            return View();
        }

        public JsonResult GetAllEventCategoryimage(jQueryDataTableParamModel param)
        {
            try
            {


                var allRecords = new List<CategoryImageEvent>();
                allRecords = MS.GetAllEventCategoryimage();
                List<CategoryImageEvent> filteredRecords = null;

                if (!string.IsNullOrWhiteSpace(param.sSearch))
                {
                    allRecords = allRecords.Where(t => t.Id.ToString().Contains(param.sSearch.ToLower()) || t.Category.Name.ToLower().ToString().Contains(param.sSearch.ToLower()) || t.Image.ToString().ToLower().Contains(param.sSearch.ToLower())).OrderBy(t => t.Category.Name).ToList();
                    filteredRecords = allRecords.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
                else
                {
                    filteredRecords = allRecords
                                        .Skip(param.iDisplayStart)
                                        .Take(param.iDisplayLength).OrderBy(t => t.Category.Name).ToList();
                }
                int totalRecords = allRecords.Count();
                var result = from c in filteredRecords select new[] { "", c.Id.ToString(), c.Category.Name, "/Images/categoryeventimage/" + c.Image, "" };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords,
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                return Json(new
                {
                    sEcho = 1,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<string[]> { }
                }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public JsonResult EditEventCategoryimage(int Id)
        {
            try
            {
                return Json(MS.EditEventCategoryimage(Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult SaveCategorypic()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        int Id = Convert.ToInt16(TempData["Id"]);
                        string filenameee = Id.ToString() + "_" + fname;
                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/images/categoryeventimage/"), filenameee);
                        file.SaveAs(fname);
                        int CId = Convert.ToInt16(TempData["CId"]);
                        CategoryImageEvent CM = new CategoryImageEvent();
                        CM.Image = filenameee;
                        CM.Id = Id;
                        CM.CategoryId = CId;
                        var obj = MS.SaveEventCategoryimage(CM);


                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }






        [HttpPost]
        public JsonResult SaveEventCategoryimage(CategoryImageEvent CM)
        {
            try
            {
                var dd = MS.SaveEventCategoryimage(CM);
                TempData["Id"] = dd.Id;
                TempData["CId"] = CM.CategoryId;
                TempData.Keep("Id");
                TempData.Keep("CId");
                return Json(dd, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }

        //[HttpPost]
        //public JsonResult ActiveDeActiveCategory(int Id)
        //{
        //    try
        //    {
        //        return Json(MS.ActiveDeActiveCategory(Id), JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonServices.ErrorLogging(ex);
        //        throw ex;
        //    }
        //}

        #endregion



        #region Event
        public ActionResult Event()
        {
            ViewBag.ddlcategory = MS.GetCategoryDDl();
            return View();
        }
        public JsonResult GetAllEvent(jQueryDataTableParamModel param)
        {
            try
            {
                var allRecords = new List<Event>();
                allRecords = MS.GetAllEvent();
                List<Event> filteredRecords = null;

                if (!string.IsNullOrWhiteSpace(param.sSearch))
                {
                    allRecords = allRecords.Where(t => t.Id.ToString().Contains(param.sSearch.ToLower()) || t.Category.Name.ToLower().ToString().Contains(param.sSearch.ToLower()) || t.Title.ToLower().ToString().Contains(param.sSearch.ToLower()) || t.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())).OrderBy(t => t.Title).ToList();
                    filteredRecords = allRecords.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
                else
                {
                    filteredRecords = allRecords
                                        .Skip(param.iDisplayStart)
                                        .Take(param.iDisplayLength).OrderBy(t => t.Title).ToList();
                }
                int totalRecords = allRecords.Count();
                var result = from c in filteredRecords select new[] { "", c.Id.ToString(), c.Title, c.Category.Name, c.Description, c.Image, Convert.ToDateTime(c.CreatedDate).ToString("dd/MM/yyyy HH:mm"), c.IsActive == true ? "Active" : "InActive", "" };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords,
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                return Json(new
                {
                    sEcho = 1,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<string[]> { }
                }, JsonRequestBehavior.AllowGet);

            }
        }


        public JsonResult GetEventById(int Id)
        {
            return Json(MS.GetEventById(Id), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ActiveDeActiveEvent(int Id)
        {
            try
            {
                return Json(MS.ActiveDeActiveEvent(Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }



        [HttpPost]
        public JsonResult SaveEvent(Event CM)
        {
            try
            {
                return Json(MS.SaveEvent(CM), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }



        #endregion

        #region Calling
        public ActionResult Calling()
        {
            return View();
        }
        public JsonResult GetAllCalling(jQueryDataTableParamModel param)
        {
            try
            {
                var allRecords = new List<Calling_Request>();
                allRecords = MS.GetAllCalling();
                List<Calling_Request> filteredRecords = null;

                if (!string.IsNullOrWhiteSpace(param.sSearch))
                {
                    allRecords = allRecords.Where(t => t.Id.ToString().Contains(param.sSearch.ToLower()) || t.User_Master1.FullName.ToString().Contains(param.sSearch.ToLower()) || t.User_Master1.FullName.ToLower().ToString().Contains(param.sSearch.ToLower())).OrderBy(t => t.Id).ToList();
                    filteredRecords = allRecords.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
                else
                {
                    filteredRecords = allRecords
                                        .Skip(param.iDisplayStart)
                                        .Take(param.iDisplayLength).OrderBy(t => t.Id).ToList();
                }
                int totalRecords = allRecords.Count();

                var result = from c in filteredRecords select new[] { "", c.Id.ToString(), c.User_Master.FullName, c.User_Master1.FullName, c.TotalAmount.Value.ToString("0.00"), Convert.ToDateTime(c.CallingDateTime1).ToString("dd-MM-yyy HH:mm"), c.CallingDateTime2 != null ? Convert.ToDateTime(c.CallingDateTime2).ToString("dd-MM-yyyy HH:mm") : "", c.CallingDateTime3 == null ? "" : Convert.ToDateTime(c.CallingDateTime3).ToString("dd-MM-yyyy HH:mm"), c.AcceptDatetime == null ? "" : Convert.ToDateTime(c.AcceptDatetime).ToString("dd-MM-yyyy HH:mm"), c.IsAccept.ToString(), c.IsReject.ToString(), c.RejectedNote };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords,
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                return Json(new
                {
                    sEcho = 1,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<string[]> { }
                }, JsonRequestBehavior.AllowGet);

            }
        }


        public JsonResult GetCallingById(int Id)
        {
            return Json(MS.GetEventById(Id), JsonRequestBehavior.AllowGet);
        }

       

        #endregion


        #region Category
        public ActionResult CallingPrice()
        {
            return View();
        }

        public JsonResult GetAllCallingPrice(jQueryDataTableParamModel param)
        {
            try
            {
                var allRecords = new List<CallingPriceMaster>();
                allRecords = MS.GetAllCallingPrices();
                List<CallingPriceMaster> filteredRecords = null;

                if (!string.IsNullOrWhiteSpace(param.sSearch))
                {
                    allRecords = allRecords.Where(t => t.Name.Contains(param.sSearch) || t.Price.Value.ToString().Contains(param.sSearch)).OrderBy(t => t.Name).ToList();
                    filteredRecords = allRecords.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
                else
                {
                    filteredRecords = allRecords
                                        .Skip(param.iDisplayStart)
                                        .Take(param.iDisplayLength).OrderBy(t => t.Name).ToList();
                }
                int totalRecords = allRecords.Count();
                var result = from c in filteredRecords select new[] { "", c.Id.ToString(), c.Name.ToString(),Convert.ToString( c.Time),Convert.ToString( c.Price.Value)  , c.IsActive == true ? "Active" : "InActive", "" };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords,
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                return Json(new
                {
                    sEcho = 1,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<string[]> { }
                }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public JsonResult EditCallPrice(int Id)
        {
            try
            {
                return Json(MS.EditCallPrice(Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }


        [HttpPost]
        public JsonResult SaveCallPrice(CallingPriceMaster CM)
        {
            try
            {
                return Json(MS.SaveCallingPriceMaster(CM), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }



        [HttpPost]
        public JsonResult ActivedeActiveCallingPrice(int Id)
        {
            try
            {
                return Json(MS.ActivedeActiveCallingPrice(Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw ex;
            }
        }

        #endregion
    }
}