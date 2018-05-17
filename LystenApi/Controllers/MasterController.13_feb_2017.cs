using LystenApi.Db;
using LystenApi.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                var allRecords = new List<User_Master>();
                allRecords = MS.GetAllUser();
                List<User_Master> filteredRecords = null;

                if (!string.IsNullOrWhiteSpace(param.sSearch))
                {
                    allRecords = allRecords.Where(t => t.Id.ToString().Contains(param.sSearch.ToLower()) || t.Email.ToLower().ToString().Contains(param.sSearch.ToLower()) || t.UserName.ToLower().ToString().Contains(param.sSearch.ToLower()) || t.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())).OrderBy(t => t.Email).ToList();
                    filteredRecords = allRecords.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
                else
                {
                    filteredRecords = allRecords
                                        .Skip(param.iDisplayStart)
                                        .Take(param.iDisplayLength).OrderBy(t => t.Email).ToList();
                }
                int totalRecords = allRecords.Count();
                var result = from c in filteredRecords select new[] { "", c.Id.ToString(), c.Email, c.UserName, c.IsActive == true ? "Active" : "InActive", "" };
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



        #region Event
        public ActionResult Event()
        {
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
                    allRecords = allRecords.Where(t => t.Id.ToString().Contains(param.sSearch.ToLower()) || t.Category.Name.ToLower().ToString().Contains(param.sSearch.ToLower()) || t.Title.ToLower().ToString().Contains(param.sSearch.ToLower()) ||t.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())).OrderBy(t => t.Title).ToList();
                    filteredRecords = allRecords.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
                else
                {
                    filteredRecords = allRecords
                                        .Skip(param.iDisplayStart)
                                        .Take(param.iDisplayLength).OrderBy(t => t.Title).ToList();
                }
                int totalRecords = allRecords.Count();
                var result = from c in filteredRecords select new[] { "", c.Id.ToString(), c.Title,c.Category.Name,c.Description,c.Image, Convert.ToDateTime(c.CreatedDate).ToString("dd/MM/yyyy HH:mm"),c.IsActive == true ? "Active" : "InActive", "" };
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


        public JsonResult GetCallingById(int Id)
        {
            return Json(MS.GetEventById(Id), JsonRequestBehavior.AllowGet);
        }


        #endregion

    }
}