using KeysPlus.Service.Models;
using KeysPlus.Service.Services;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
//using KeysPlus.Website.Areas.Tenants.Models;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using KeysPlus.Data;
using System.Collections.Generic;
using KeysPlus.Website.Areas.Tools;
using Microsoft.Ajax.Utilities;
using System.Web.Routing;

namespace KeysPlus.Website.Areas.Tenants.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        KeysEntities db = new KeysEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Onboarding()
        {
            return View();
        }

        public ActionResult SendRequest(string returnUrl)
        {
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            var model = TenantService.GetTenantRentals(login.Id);
            model.ForEach(x => x.AddressString = x.PropertyAddress.Number + " " + x.PropertyAddress.Street + ", " + x.PropertyAddress.Suburb + ", " + x.PropertyAddress.City + ", " + x.PropertyAddress.PostCode);
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.RequestTypes = RentalService.GetRequestTypes();
            return View(model);
        }

        public ActionResult UpdatePhoto()
        {
            return Json("");
        }

        [HttpGet]
        public JsonResult PaymentDate(int id,int tid)
        {
            var d = TenantService.GetPaymentDate(id, tid);
            return Json(new { date=d }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Onboarding(TenantDetailsModel model, HttpPostedFileBase imageFile)
        {
            var fs = Request.Files;
            var isAjax = Request.IsAjaxRequest();
            if (ModelState.IsValid)
            {
                var userName = User.Identity.Name;
                if (String.IsNullOrEmpty(userName))
                {
                    return Json(new { Success = false, ErrorMsg = "User does not exist!" });
                }
                var login = AccountService.GetLoginByEmail(userName);
                var tenant = login == null ? null : TenantService.GetTenantByLogin(login);
                var detailResult = TenantService.SaveDetails(model, login);
                if (detailResult.IsSuccess)
                {
                    var files = Request.Files;
                    var mediaResult = TenantService.AddTenantMediaFiles(Request.Files, tenant, Server.MapPath("~/images"));
                    if (isAjax)
                    {
                        return Json(new { Success = true });
                    }
                    else return RedirectToAction("Index");

                }
                else
                {
                    return Json(new { Success = false });
                }
            }
            return View(model);
        }

        public ActionResult MyRentals()
        {
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            if (TenantService.IsLoginATenant(login))
            {
                var data = TenantService.GetMyRentals(login.Id);
                data.ForEach(x => x.Model.MediaFiles.ForEach(y => y.InjectMediaModelViewProperties()));
                var model = new MyRentalsViewModel { };
                model.Items = data.ToList();
                ViewBag.RequestTypes = RentalService.GetRequestTypes().Where(x => x.Id != 3).ToList();
                TempData["CurrentLink"] = "MyRentals";
                return View(model);
            }
            return View("Error");

        }

        public JsonResult IsProfileComplete()
        {
            var user = User.Identity.Name;
            var isComplete = TenantService.IsProfileComplete(user);
            return Json(new { Success = isComplete });
        }

        [HttpPost]
        public JsonResult EditTenancyApplication(RentalApplicationModel model)
        {

            if (ModelState.IsValid)
            {
                var files = Request.Files;
                var userName = User.Identity.Name;
                if (String.IsNullOrEmpty(userName))
                {
                    return Json(new { Success = false, ErrorMsg = "User not exixt!" });
                }
                var login = AccountService.GetLoginByEmail(userName);
                var result = RentalService.EditRentallApllication(model, login, Request.Files);
                return result.IsSuccess ? Json(new { Success = true , MediaFiles = result.NewObject as List<MediaModel>}) : Json(new { Success = false, ErrorMsg = result.ErrorMessage });
            }
            else return Json(new { Success = false });


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteTenancyApplication(int id)
        {
            var files = Request.Files;
            var userName = User.Identity.Name;
            var result = RentalService.DeleteRentallApllication(id);
            return result.IsSuccess ? Json(new { Success = true }) : Json(new { Success = false, ErrorMsg = result.ErrorMessage });
        }

        public JsonResult UpdateRequest(RequestModel model)
        {

            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);

            var files = Request.Files;
            var result = PropertyService.UpdateRequest(model, login, Request.Files);
           
            if (result.IsSuccess)
            {
                return Json(new { Success = result.IsSuccess });
            }
            
            return Json(new { Success = false, Message = result.ErrorMessage });
           
        }
        [HttpPost]
        public JsonResult DeleteRequest(int id)
        {
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            var request = PropertyService.GetPropertyRequestById(id);
            if (request == null) return Json(new { Success = false, Message = "No record found!" });
            request.IsActive = false;
            var result = PropertyService.UpdateRequest(request, login, Request.Files, Server.MapPath("~/images"));
            if (result.IsSuccess)
            {
                return Json(new
                {
                    Success = true,
                    Message = "IsDeleted",
                    Updated = true,
                });
            }
            else
            {
                return Json(new { Success = false, Message = result.ErrorMessage });
            }
        }
        
        [HttpGet]
        public ActionResult LandLordRequests(LandlordRequestsSearchModel model)
        {
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            if (String.IsNullOrWhiteSpace(model.SortOrder))
            {
                model.SortOrder = "Latest First";
            }
            if (model.RequestStatus == null)
            {
                model.RequestStatus = PropertyRequestStatus.Submitted;
            }
            var res = TenantService.GetLandlordRequests(login, model);
            
            model.PagedInput = new PagedInput
            {
                ActionName = "LandlordRequests",
                ControllerName = "Home",
                PagedLinkValues = new RouteValueDictionary(new
                {
                    PropId = model.PropId,
                    SortOrder = model.SortOrder,
                    SearchString = model.SearchString,
                    ReturnUrl = model.ReturnUrl,
                    RequestStatus = model.RequestStatus
                }),

            };
            model.InputValues = new List<SearchInput>() {
                new SearchInput {Name = "PropertyId", Value = model.PropId.ToString()},
                new SearchInput { Name = "RequestStatus", Value = model.RequestStatus.ToString()}
            };
            
            var rvr = new RouteValueDictionary(new { PropId = model.PropId, SearchString = model.SearchString, ReturnUrl = model.ReturnUrl });
            var sortOrders = new List<SortOrderModel>();
            sortOrders.Add(new SortOrderModel { SortOrder = "Latest First" , ActionName = "LandLordRequest", RouteValues= rvr.AddRouteValue("SortOrder", "Latest First") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Earliest First", ActionName = "LandLordRequest", RouteValues = rvr.AddRouteValue("SortOrder", "Earliest First") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Request Status", ActionName = "LandLordRequest", RouteValues = rvr.AddRouteValue("SortOrder", "Request Status") });
            model.SortOrders = sortOrders;
            model.SearchCount = res.SearchCount;
            if (String.IsNullOrWhiteSpace(model.SearchString)) model.Page = 1;
            model.PageCount = res.Items.PageCount;
            model.Items = res.Items;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AccepLandlordRequest(int requestId)
        {

            var user = User.Identity.Name;
            var files = Request.Files;
            var login = AccountService.GetLoginByEmail(user);
            var result = TenantService.AccepLandlordRequest(requestId);
            if (result.IsSuccess)
            {
                return Json(new { Success = true, Message = "Request accepted successfull!" });
            }
            else
            {
                return Json(new { Success = false, ErrorMsg = result.ErrorMessage });
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddInspection(InspectionModel model)
        {

            var user = User.Identity.Name;
            var files = Request.Files;
            var login = AccountService.GetLoginByEmail(user);
            if (ModelState.IsValid)
            {
                var result = TenantService.AddInspection(model, login, Request.Files);
                if (result.IsSuccess)
                {
                    return Json(new { Success = true, Message = "Sucessfully Replied to the Property Owner's Request", Posted = true });
                }
                else
                {
                    return Json(new { Success = false, ErrorMsg = result.ErrorMessage });
                }
            }
            return Json(new { Success = false, ErrorMsg = "Invalid fields" });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditInspection (InspectionModel model)
        {
            var user = User.Identity.Name;
            var files = Request.Files;
            var login = AccountService.GetLoginByEmail(user);
            if (ModelState.IsValid)
            {
                var result = TenantService.EditInspection(model, login, Request.Files);
                if (result.IsSuccess)
                {
                    var media = result.NewObject as List<MediaModel>;
                    return Json(new { Success = true, Message = "Sucessfully Replied to the Property Owner's Request", MediaFiles = result.NewObject as List<MediaModel>});
                }
                else
                {
                    return Json(new { Success = false, ErrorMsg = result.ErrorMessage });
                }
            }
            return Json(new { Success = false, ErrorMsg = "Invalid fields" });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeclineRequest(int id, string reason)
        {
            var user = User.Identity.Name;            
            var login = AccountService.GetLoginByEmail(user);
            if (ModelState.IsValid)
            {
                var result = TenantService.DeclineRequest(new RequestModel {Id = id, Reason = reason }, login);
                if (result.IsSuccess)
                {
                    return Json(new { Success = true, Message = "Request has been declined!" });
                }
                else
                {
                    return Json(new { Success = false, ErrorMsg = result.ErrorMessage });
                }
            }
            return Json(new { Success = false, ErrorMsg = "Invalid fields" });
        }
        
        public ActionResult MyRequests(MyRequestSearchModel model)
        {
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            if (String.IsNullOrWhiteSpace(model.SortOrder))
            {
                model.SortOrder = "Latest Request";
            }
            if(model.RequestStatus == null)
            {
                model.RequestStatus = PropertyRequestStatus.Submitted;
            }
            var data = TenantService.GetAllRequests(model, login);
            model.PagedInput = new PagedInput
            {
                ActionName = "MyRequests",
                ControllerName = "Home",
                PagedLinkValues = new RouteValueDictionary(new {
                    SortOrder = model.SortOrder,
                    SearchString = model.SearchString,
                    PropertyId = model.PropertyId,
                    RequestStatus = model.RequestStatus
                })
            };
            model.InputValues = new List<SearchInput>() {
                new SearchInput {Name = "PropertyId", Value = model.PropertyId.ToString()},
                new SearchInput { Name = "RequestStatus", Value = model.RequestStatus.ToString()}
            };
            
            var rvr = new RouteValueDictionary(new { SearchString = model.SearchString , PropertyId = model.PropertyId, RequestStatus = model.RequestStatus});
            var sortOrders = new List<SortOrderModel>();
            sortOrders.Add(new SortOrderModel { SortOrder = "Latest Request", ActionName = "MyRequests", RouteValues = rvr.AddRouteValue("SortOrder", "Latest Request") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Earliest Request", ActionName = "MyRequests", RouteValues = rvr.AddRouteValue("SortOrder", "Earliest Request") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Requested Type", ActionName = "MyRequests", RouteValues = rvr.AddRouteValue("SortOrder", "Requested Type") });
            model.SortOrders = sortOrders;
            model.SearchCount = data.SearchCount;
            if (String.IsNullOrWhiteSpace(model.SearchString)) model.Page = 1;
            model.PageCount = data.Items.PageCount;
            model.Items = data.Items;
            model.EditUrl = "/Tenants/Home/UpdateRequest";
            model.DeleteUrl = "/Tenants/Home/DeleteRequest";
            TempData["CurrentLink"] = "MyRentApps";
            return View(model);
        }

        public ActionResult MyRentalApplications(RentalAppSearchModel model)
        {
            var user = User.Identity.Name;
            var login  = AccountService.GetLoginByEmail(user);
            if (String.IsNullOrWhiteSpace(model.SortOrder))
            {
                model.SortOrder = "Latest Date";
            }
            var res = TenantService.GetAllRentApplications(model, login);

            model.PagedInput = new PagedInput
            {
                ActionName = "MyRentalApplications",
                ControllerName = "Home",
                PagedLinkValues = new RouteValueDictionary(new { SortOrder = model.SortOrder, SearchString = model.SearchString })
            };
            var rvr = new RouteValueDictionary(new { SearchString = model.SearchString });
            var sortOrders = new List<SortOrderModel>();
            sortOrders.Add(new SortOrderModel { SortOrder = "Lowest Rent", ActionName = "MyRentalApplications", RouteValues = rvr.AddRouteValue("SortOrder", "Lowest Rent") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Highest Rent", ActionName = "MyRentalApplications", RouteValues = rvr.AddRouteValue("SortOrder", "Highest Rent") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Earliest Date", ActionName = "MyRentalApplications", RouteValues = rvr.AddRouteValue("SortOrder", "Earliest Date") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Latest Date", ActionName = "MyRentalApplications", RouteValues = rvr.AddRouteValue("SortOrder", "Latest Date") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Earliest Available", ActionName = "MyRentalApplications", RouteValues = rvr.AddRouteValue("SortOrder", "Earliest Available") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Latest Available", ActionName = "MyRentalApplications", RouteValues = rvr.AddRouteValue("SortOrder", "Latest Available") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Accepted First", ActionName = "MyRentalApplications", RouteValues = rvr.AddRouteValue("SortOrder", "Accepted First") });
            model.SortOrders = sortOrders;
            model.SearchCount = res.SearchCount;
            if (String.IsNullOrWhiteSpace(model.SearchString)) model.Page = 1;
            model.PageCount = res.Items.PageCount;
            model.Items = res.Items;
            model.EditUrl = "/Tenants/Home/EditTenancyApplication";
            model.DeleteUrl = "/Tenants/Home/DeleteTenancyApplication";
            TempData["CurrentLink"] = "MyRentApps";
            return View(model);
        }

        public JsonResult GetRequestTypes()
        {
            var result = PropertyService.GetRequestTypes();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}