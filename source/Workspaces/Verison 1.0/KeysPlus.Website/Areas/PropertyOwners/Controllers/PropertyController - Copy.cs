﻿using KeysPlus.Data;
using KeysPlus.Service.Models;
using KeysPlus.Website.Areas.Tools;
using KeysPlus.Service.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.Ajax.Utilities;
using PagedList;
using System.Linq;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Threading.Tasks;
using System.IO;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using System.Data;

namespace KeysPlus.Website.Areas.PropertyOwners.Controllers
{
    [Authorize]
    public class PropertyController : Controller
    {

        private KeysEntities db = new KeysEntities();
        decimal actualTotalRepayment = 0;

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddNewProperty()
        {
            var freqs = PropertyOwnerService.GetAllPaymentFrequencies();
            var propertyTypes = PropertyService.GetAllProprtyTypes();
            var propertyHomeValueTypes = PropertyService.GetAllProprtyHomeValueTypes();
            ViewBag.Frequencies = freqs;
            ViewBag.PropertyTypes = propertyTypes;
            ViewBag.PropertyHomeValueTypes = propertyHomeValueTypes;
            ViewBag.PathAndQuery = HttpContext.Request.UrlReferrer.PathAndQuery;
            TempData["CurrentLink"] = "Properties";
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> AddNewProperty(PropertyMyOnboardModel model)
        {
            var files = Request.Files;
            var status = true;
            var message = "Record added successfully";
            var data = model;
            AddTenantToPropertyModel tenant = new AddTenantToPropertyModel();
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            var newProp = PropertyOwnerService.AddOnboardProperty(login, model);
            var newRepayment = new PropertyRepayment();
            if (newProp == null)
            {
                return Json(new { Success = false, message = "Cannot add the property!" });
            }
            else
            {
                newRepayment = PropertyOwnerService.AddOnboardRepayment(login, model.Repayments, newProp.Id);
                decimal _totalRepayment = 0;
                int _nosWeeks = 0;
                int _nosFortnights = 0;
                int _nosMonthly = 0;
                if (newRepayment != null)
                {
                    foreach (Service.Models.RepaymentViewModel repayment in model.Repayments)
                    {
                        switch (repayment.FrequencyType)
                        {
                            case 1: // Weekly
                                    // find the nos of weeks in datediff(StartDate, EndDate)
                                _nosWeeks = ((newRepayment.EndDate - newRepayment.StartDate) ?? TimeSpan.Zero).Days / 7;
                                // _totalAmount = nos weeks * amount
                                _totalRepayment = _nosWeeks * newRepayment.Amount;
                                break;
                            case 2:   // Fortnightly
                                      // find the nos of Fortnights in datediff(StartDate, EndDate)
                                _nosFortnights = ((newRepayment.EndDate - newRepayment.StartDate) ?? TimeSpan.Zero).Days / 14;
                                // _totalAmount = nos weeks * amount
                                _totalRepayment = _nosFortnights * newRepayment.Amount;
                                break;
                            case 3: //Monthly
                                    // find the nos of Monthls in datediff(StartDate, EndDate)
                                _nosMonthly = ((newRepayment.EndDate - newRepayment.StartDate) ?? TimeSpan.Zero).Days / 30;
                                _totalRepayment = _nosMonthly * newRepayment.Amount;
                                // _totalAmount = nos Monthls * amount
                                break;
                        }
                        actualTotalRepayment += _totalRepayment;
                    }
                }
                //*****AddExpenses
                var newExpense = new PropertyExpense();
                newExpense = PropertyOwnerService.AddOnboardExpense(login, model.Expenses, newProp.Id);
                //******AddFinancial
                var newFinancial = new PropertyFinance();
                newFinancial = PropertyOwnerService.AddOnboardFinance(login, model, newProp.Id, actualTotalRepayment);
                var ownerPerson = AccountService.GetPersonByLoginId(login.Id);
                if (!model.IsOwnerOccupied)
                {

                    var ten = AccountService.GetExistingLogin(model.TenantToPropertyModel.TenantEmail);
                    if (ten == null)
                    {
                        var sendEmail = false;
                        var temPass = UtilService.GeneraterRandomKey(8);
                        var createRes = AccountService.CreateTenantAccount(model.TenantToPropertyModel, login, temPass);

                        if (createRes.IsSuccess)
                        {
                            ten = createRes.NewObject as Login;
                            sendEmail = true;
                        }

                        if (sendEmail)
                        {
                            var emailRes = await EmailService.SendCreateAccountToTenant(model, temPass, ownerPerson);
                        }
                        else
                        {
                            return Json(new { Success = false, NewPropId = newProp.Id });
                        }
                        //return Json(new { Success = false, NewPropId = newProp.Id, Todo = "Send email", ErrorMsg = "Cannot find person in login table!" });
                    }
                    else // ten not null
                    {
                        if (!ten.IsActive)
                        {
                            var resultTenantActive = PropertyService.ActivateTenant(login, ten.Id);
                            if (resultTenantActive.IsSuccess)
                            {

                                await EmailService.SendActivationEmailToTenant(model, ownerPerson);
                            }
                        }

                    }
                    var person = AccountService.GetPersonByLoginId(ten.Id);
                    //var result = PropertyService.AddTenantToProperty(login, person.Id, newProp.Id, model.TenantToPropertyModel.StartDate,
                    //    model.TenantToPropertyModel.EndDate, model.TenantToPropertyModel.PaymentFrequencyId, model.TenantToPropertyModel.PaymentAmount);
                    model.TenantToPropertyModel.Liabilities = model.LiabilityValues;
                    model.TenantToPropertyModel.PropertyId = newProp.Id;
                    var result = PropertyService.AddTenant(login, ten.Id, model.TenantToPropertyModel);
                    if (result.IsSuccess)
                    {
                        return Json(new { Sucess = true, Msg = "Added!", NewPropId = newProp.Id, result = "Redirect", url = Url.Action("Index", "PropertyOwners") });
                    }
                    else
                    {
                        return Json(new { Sucess = false, NewPropId = newProp.Id, Msg = result.ErrorMessage, redirect = "Redirect", url = Url.Action("Index", "PropertyOwners") });
                    }
                }
            }
            return Json(new { Success = status, NewPropId = newProp.Id, message = message, data = tenant });
        }

        public ActionResult PropertyRequestsCount()
        {
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            var requestCount = PropertyService.GetRequestsCount(login);
            return PartialView(requestCount);
        }

        public ActionResult RequestsByTenants(string searchString, string sortOrder, string currentFilter, int page = 1)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = searchString;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            var propertyRequest = PropertyService.GetAllPropertiesWithRequests(login).ToPagedList(page, 10);
            propertyRequest.ForEach(x => x.TenantJobRequests.ForEach(y => y.MediaFiles.ForEach(z => z.Data = Url.Content("~/images/" + z.Data))));
            propertyRequest.ForEach(x => x.PropertyImages.ForEach(y => y.Data = Url.Content("~/images/" + y.NewFileName)));
            TempData["CurrentLink"] = "TenantRequests";
            return View(propertyRequest);
        }

        public JsonResult GetSpecificProperties(int pid)
        {
            var user = User.Identity.Name;
            var id = AccountService.GetLoginByEmail(user).Id;
            var allProperties = new List<PropertyViewModel>();
            var property = PropertyService.GetPropertyById(pid);
            var address = PropertyService.GetAddressById(property.AddressId);
            if (address != null)
            {
                if (address.Street != "" && address.City != "")
                {
                    var propertyVm = new PropertyViewModel
                    {
                        AddressString = address.Number + " " + address.Street + ", " + address.Suburb + ", " + address.City + ", " + address.PostCode
                    };
                    allProperties.Add(propertyVm);
                }
            }
            var jsonProperties = new JavaScriptSerializer().Serialize(allProperties);
            return Json(jsonProperties, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllProperties()
        {
            var user = User.Identity.Name;
            var id = AccountService.GetLoginByEmail(user).Id;
            var properties = PropertyService.GetAllPropertiesByOwner(id);
            var allProperties = new List<PropertyViewModel>();
            foreach (var property in properties)
            {
                var address = PropertyService.GetAddressById(property.AddressId);
                if (address != null)
                {
                    if (address.Street != "" && address.Suburb != "" && address.City != "")
                    {
                        var propertyVm = new PropertyViewModel
                        {
                            Id = property.Id,
                            Name = property.Name,
                            AddressString = address.Number + " " + address.Street + ", " + address.Suburb + ", " + address.City + ", " + address.PostCode
                        };
                        allProperties.Add(propertyVm);
                    }
                }
                else
                {
                    continue;
                }
            }
            var jsonProperties = new JavaScriptSerializer().Serialize(allProperties);
            return Json(jsonProperties, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddJobRequest(MarketJobModel model)
        {
            var user = User.Identity.Name;
            var files = Request.Files;
            var login = AccountService.GetLoginByEmail(user);
            var request = PropertyService.GetPropertyRequestById(model.RequestId);
            if (request == null) return Json(new { Success = false, Message = "No record found!" });
            if (request.RequestStatusId == (int)JobRequestStatus.Accepted) return Json(new { Success = true, Message = "Job Already Created !!", Posted = false });
            //request.RequestStatusId = (int)JobRequestStatus.Accepted;
            //request.IsUpdated = true;
            if (ModelState.IsValid)
            {
                var result = RentalService.AddTenantJobRequest(model, login, Request.Files);
                if (result.IsSuccess)
                {
                    return Json(new { Success = true, Message = "Job Created And Updated", Posted = true });
                }
                else
                {
                    return Json(new { Success = false, ErrorMsg = result.ErrorMessage });
                }
            }
            return Json(new { Success = false, ErrorMsg = "Invalid fields" });
        }

        // For Property owner After viewed the TenantJobRequest  
        [HttpPost]
        public JsonResult RequestViewed(int id)
        {
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            var request = PropertyService.GetPropertyRequestById(id);
            if (request == null) return Json(new { Success = false, Message = "No record found!" });
            if (request.IsViewed) return Json(new { Success = true, Message = "Record has been viewed!", Updated = false });
            request.IsViewed = true;
            request.IsUpdated = true;
            var result = PropertyService.UpdateRequest(request, login, Request.Files, Server.MapPath("~/images"));
            if (result.IsSuccess)
            {
                return Json(new
                {
                    Success = true,
                    Message = "IsViewed",
                    Updated = true,
                    //data = currentTenantJobRequestRecord
                });
            }
            else
            {
                return Json(new { Success = false, Message = result.ErrorMessage });
            }
        }

        [HttpPost]
        public JsonResult ApplicationViewed(int id)
        {
            var result = RentalService.UpdateApplicationView(id);
            if (result.IsSuccess)
            {
                return Json(new
                {
                    Success = true,
                    Message = "Application is Viewed By Owner",
                    Updated = true,
                });
            }
            else
            {
                return Json(new { Success = false, Message = result.ErrorMessage, Updated = false });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuoteViewed(int quoteId)
        {
            var user = User.Identity.Name;
            if (String.IsNullOrEmpty(user))
            {
                return Json(new { Success = false, Msg = "User not found!" });
            }
            var result = JobService.QuoteViewed(quoteId);

            return Json(new { Success = result.IsSuccess, Msg = result.ErrorMessage });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AccceptQuote(JobAcceptedModel jobModel)
        {
            var user = User.Identity.Name;
            if (String.IsNullOrEmpty(user))
            {
                return Json(new { Success = false, Msg = "User not found!" });
            }
            var login = AccountService.GetLoginByEmail(user);
            var result = JobService.AcceptQuote(jobModel, login);
            if (result.IsSuccess)
            {

                var serviceProviderPersonDetails = JobService.GetPersonByJobQuoteId(jobModel);
                var serviceProviderLoginDetails = AccountService.GetLoginById(serviceProviderPersonDetails.Id);
                var jobDetails = JobService.GetMarketJobById(jobModel.JobRequestId);
                var nvc = new NameValueCollection();
                nvc.Add("userAction", "2");

                string url = UtilService.UrlGenerator(System.Web.HttpContext.Current.Request, "/Jobs/Home", UtilService.ToQueryString(nvc));
                SendGridEmailModel mail = new SendGridEmailModel
                {
                    RecipentName = serviceProviderPersonDetails.FirstName,
                    ButtonText = "",
                    ButtonUrl = url,
                    RecipentEmail = serviceProviderLoginDetails.Email,
                    JobTitle = jobDetails.Title ?? "No Title",
                };
                await EmailService.SendEmailWithSendGrid(EmailType.AcceptQuote, mail);
                return Json(new { Success = result.IsSuccess, Msg = result.ErrorMessage });
            }


            return Json(new { Success = false });


        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<JsonResult> ApplicationAccepted(AcceptAndDeclineRentalApplicationModel model)
        {
            if (!ModelState.IsValid) return Json(new { Success = false });
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            var result = PropertyService.AcceptApplication(model, login);
           

            // send email here
            if (result.IsSuccess)
            {
                var tenantPersonDetails = AccountService.GetPersonById(model.TenantId);
                var tenatLoginDetails = AccountService.GetLoginById(model.TenantId);


                var property = db.Property.FirstOrDefault(x => x.Id == model.PropertyId);
                var addressString = "";
                if (property != null)
                {
                    var address = PropertyService.GetAddressById(property.AddressId);
                    if (address != null)
                    {
                        if (address.Street != "" && address.City != "")
                        {
                            addressString = address.Number + " " + address.Street + ", " + address.Suburb + ", " + address.City + ", " + address.PostCode;
                        }
                    }
                }

                string url = UtilService.UrlGenerator(System.Web.HttpContext.Current.Request, "Tenants/Home/MyRentals");
                SendGridEmailModel mail = new SendGridEmailModel
                {
                    RecipentName = tenantPersonDetails.FirstName,
                    ButtonText = "",
                    ButtonUrl = url,
                    RecipentEmail = tenatLoginDetails.Email,
                    Address = addressString,
                };
                await EmailService.SendEmailWithSendGrid(EmailType.AcceptRentalApplication, mail);
                return Json(new { Success = result.IsSuccess });
            }
            return Json(new { Success = result.IsSuccess, Msg = result.ErrorMessage });
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public JsonResult ApplicationDeclined(AcceptAndDeclineRentalApplicationModel model)
        {
            if (!ModelState.IsValid) return Json(new { Success = false });
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            var result = PropertyService.DeclineApplication(model, login);
            if (result.IsSuccess)
            {
                return Json(new { Success = result.IsSuccess });
            }
            else
            {
                return Json(new { Success = result.IsSuccess, Msg = result.ErrorMessage });
            }
        }

        [HttpPost]
        public JsonResult EditRentalListing(RentListingModel model)
        {
            var user = User.Identity.Name;
            if (String.IsNullOrEmpty(user))
            {
                return Json(new { Success = false });
            }
            var login = AccountService.GetLoginByEmail(user);
            if (ModelState.IsValid)
            {
                var files = Request.Files;
                var result = RentalService.EditRentalListing(model, files, login);
                return Json(new { Success = result.IsSuccess , MediaFiles = result.Result});
            }
            return Json(new { Success = false });
        }

        [HttpPost]
        public ActionResult AddRentalListing(RentListingModel model)
        {
            var user = User.Identity.Name;
            if (String.IsNullOrEmpty(user))
            {
                return Json(new { Success = false });
            }
            var login = AccountService.GetLoginByEmail(user);
            if (ModelState.IsValid)
            {
                var files = Request.Files;
                var result = RentalService.AddRentalListing(model, files, login);
                return Json(new { Success = result.IsSuccess });
            }
            return Json(new { Success = false });
        }

        [HttpGet]
        public ActionResult RentalProperties(PORentalListingSearchModel model)
        {
           
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            if (String.IsNullOrWhiteSpace(model.SortOrder))
            {
                model.SortOrder = "New Application(Desc)";
            }
            var res = PropertyService.GetRentalListings(model, login);
            model.PagedInput = new PagedInput
            {
                ActionName = "RentalProperties",
                ControllerName = "Property",
                PagedLinkValues = new RouteValueDictionary(new { SortOrder = model.SortOrder, SearchString = model.SearchString })
            };
            var rvr = new RouteValueDictionary(new {SearchString = model.SearchString});
            var sortOrders = new List<SortOrderModel>();
            sortOrders.Add(new SortOrderModel { SortOrder = "New Application(Desc)", ActionName = "RentalProperties", RouteValues = rvr.AddRouteValue("SortOrder", "New Application(Desc)") });
            sortOrders.Add(new SortOrderModel { SortOrder = "New Application", ActionName = "RentalProperties", RouteValues = rvr.AddRouteValue("SortOrder", "New Application") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Earliest Date", ActionName = "RentalProperties", RouteValues = rvr.AddRouteValue("SortOrder", "Earliest Date") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Latest Date", ActionName = "RentalProperties", RouteValues = rvr.AddRouteValue("SortOrder", "Latest Date") });
            sortOrders.Add(new SortOrderModel { SortOrder = "TitLe", ActionName = "RentalProperties", RouteValues = rvr.AddRouteValue("SortOrder", "TitLe") });
            sortOrders.Add(new SortOrderModel { SortOrder = "TitLe(Desc)", ActionName = "RentalProperties", RouteValues = rvr.AddRouteValue("SortOrder", "TitLe(Desc)") });
            model.SortOrders = sortOrders;
            model.SearchCount = res.SearchCount;
            if (String.IsNullOrWhiteSpace(model.SearchString)) model.Page = 1;
            model.PageCount = res.Items.PageCount;
            model.Items = res.Items;
            model.EditUrl = "/PropertyOwners/Property/EditRentalListing";
            model.DeleteUrl = "/PropertyOwners/Property/DeleteRentalListing";
            TempData["CurrentLink"] = "RentalApplications";
            return View(model);
        }

        public ActionResult NewApplicationCount()
        {
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            var applicationsCount = PropertyService.GetNewApplicationCount(login);
            return PartialView(applicationsCount);
        }

        public ActionResult AllRentalApplications(PORentAppsSearchModelModel model)
        {
            if (model.RentalListingId.HasValue)
            {
                var add = PropertyService.GetAddressByRentalListing(model.RentalListingId.Value);
                var address = new AddressViewModel
                {
                    Number = add.Number.Replace(" ", ""),
                    Street = add.Street.Trim(),
                    City = add.City?.Trim(),
                    Suburb = add.Suburb?.Trim() ?? "",
                    PostCode = add.PostCode.Replace(" ", ""),
                };
                model.Address = address;
            }
            
            var apps = PropertyService.GetRentalApplications(model);
            model.PagedInput = new PagedInput
            {
                ActionName = "AllRentalApplications",
                ControllerName = "Property",
                PagedLinkValues = new RouteValueDictionary(new { SortOrder = model.SortOrder, SearchString = model.SearchString, RentalListingId = model.RentalListingId })
            };
            model.InputValues = new List<SearchInput>() {
                new SearchInput { Name = "RentalListingId", Value = model.RentalListingId.ToString()}
            };
            var rvr = new RouteValueDictionary(new { SearchString = model.SearchString , RentalListingId = model.RentalListingId.ToString() });
            var sortOrders = new List<SortOrderModel>();
            sortOrders.Add(new SortOrderModel { SortOrder = "Lowest Tenants Count", ActionName = "AllRentalApplications", RouteValues = rvr.AddRouteValue("SortOrder", "Lowest Tenants Count") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Highest Tenants Count", ActionName = "AllRentalApplications", RouteValues = rvr.AddRouteValue("SortOrder", "Highest Tenants Count") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Earliest Date", ActionName = "AllRentalApplications", RouteValues = rvr.AddRouteValue("SortOrder", "Earliest Date") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Latest Date", ActionName = "AllRentalApplications", RouteValues = rvr.AddRouteValue("SortOrder", "Latest Date") });
            model.SortOrders = sortOrders;
            model.SearchCount = apps.SearchCount;
            if (String.IsNullOrWhiteSpace(model.SearchString)) model.Page = 1;
            model.PageCount = apps.Items.PageCount;
            model.Items = apps.Items;
            return View(model);
        }

        public ActionResult RequestForProperty(int propId, string returnUrl, int page = 1)
        {
            var user = User.Identity.Name;
            var property = PropertyService.GetPropertyById(propId);
            var address = PropertyService.GetAddressById(property.AddressId);
            var requests = db.PropertyRequest.Where(x => x.PropertyId == propId).ToList();

            var reqs = PropertyService.GetRequestByProperty(propId);
            var tenantJobRequests = reqs.AsQueryable().ToPagedList(page, 10);
            var persons = db.Person.Where(x => x.IsActive).ToList();

            var tenantsId = tenantJobRequests.Select(x => x.PersonId).ToList();
            var tenants = db.Person.Where(x => tenantsId.Contains(x.Id)).ToList();

            tenantJobRequests.ForEach(x => x.PersonName = tenants.FirstOrDefault(y => y.Id == x.PersonId).FirstName);

            var model = new RequestForPropertyModel { Address = address, Requests = tenantJobRequests };
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        public ActionResult ListRental(int? propId, string returnUrl)
        {
            ViewBag.PropId = propId;
            ViewBag.PathAndQuery = HttpContext.Request.UrlReferrer.PathAndQuery; 
            var user = User.Identity.Name;
            var id = AccountService.GetLoginByEmail(user).Id;
            var allProperties = new List<PropertyViewModel>();
            if (propId == null || propId == (-1))
            {
                var properties = PropertyService.GetAllPropertiesByOwner(id);
                foreach (var property in properties)
                {
                    var address = PropertyService.GetAddressById(property.AddressId);
                    if (address != null)
                    {
                        if (address.Street != "" && address.Suburb != "" && address.City != "")
                        {
                            var propertyVm = new PropertyViewModel
                            {
                                Id = property.Id,
                                Name = property.Name,
                                AddressString = address.Number + " " + address.Street + ", " + address.Suburb + ", " + address.City + ", " + address.PostCode
                            };
                            allProperties.Add(propertyVm);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                var property = PropertyService.GetPropertyById(Convert.ToInt32(propId));
                var address = PropertyService.GetAddressById(property.AddressId);
                if (address != null)
                {
                    if (address.Street != "" && address.City != "")
                    {
                        // Edited by David - 19/10/2017
                        // Missing property Id. Adding property Id and Name
                        var propertyVm = new PropertyViewModel
                        {
                            Id = property.Id,
                            Name = property.Name,
                            AddressString = address.Number + " " + address.Street + ", " + address.Suburb + ", " + address.City + ", " + address.PostCode
                        };
                        allProperties.Add(propertyVm);
                    }
                }
            }
            TempData["CurrentLink"] = "RentalApplications";
            ViewBag.Properties = allProperties;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult PropertyTenants(int propId, string returnUrl, int? page)
        {
            return View();
        }

       
        [HttpPost]
        public JsonResult AddTenantLiability(TenantPropertyLiability model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false });
            }
            else
            {
                var result = PropertyService.AddTenantLiabilities(model);
                return Json(new { Success = true });
            }
        }

        
        [HttpGet]
        public ActionResult GetPropertyFinanceDetails(int propId)
        {
            var prop = db.Property.FirstOrDefault(x => x.Id == propId);
            var propFinance = db.PropertyFinance.FirstOrDefault(x => x.PropertyId == propId);
            var propRepayments = PropertyService.GetRepayments(propId);
            var propExpense = PropertyService.GetExpenses(propId);
            var homeValues = PropertyService.GetHomeValues(propId);
            var rentalPayments = PropertyService.GetRentalPayments(propId);
            var financeReport = DataService.ExecuteStoredProcedure<PropertyFinanceResultModel>("sp_FinanceReport", new GetFinanceReportModel { PropertyId = propId });
            var model = new FinancialModel
            {
                PropId = propId,
                YearBuilt = prop.YearBuilt ?? 1900,
                PurchasePrice = propFinance?.PurchasePrice ??  0 ,
                Mortgage = propFinance?.Mortgage ?? 0,
                CurrentHomeValue = propFinance?.CurrentHomeValue ?? 0,
                HomeValueType = prop.PropertyHomeValue.FirstOrDefault(x => x.PropertyId == propId)?.PropertyHomeValueType?.Id ?? 0,
                PropertyValueType = prop.PropertyHomeValue.FirstOrDefault(x => x.PropertyId == propId)?.PropertyHomeValueType?.HomeValueType,
                HomeValues = homeValues,
                Repayments = propRepayments.ToList(),
                Expenses = propExpense.ToList(),
                RentalPayments = rentalPayments.ToList(),
                FinanceReport = financeReport.First()
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SavePropertyHomeValue( HomeValueViewModel model)
        {
           
            if (ModelState.IsValid)
            {
                var result = Json(new { Success = false });
                if (model.IsActive)
                {
                    var activeValue = db.PropertyHomeValue.FirstOrDefault(x => x.PropertyId == model.PropertyId && (x.IsActive ?? false));
                    if (activeValue != null) activeValue.IsActive = false;
                }
                if (model.Id > 0)
                {
                    var value = db.PropertyHomeValue.FirstOrDefault(x => x.Id == model.Id);
                    if (value == null) return Json(new { Success = false });
                    value.Value = model.Value;
                    value.HomeValueTypeId = model.TypeId;
                    value.Date = model.Date;
                    value.IsActive = model.IsActive;
                    db.SaveChanges();
                    result = Json(new { Success = true, IsActive = model.IsActive });
                }
                else
                {
                    var newValue = new PropertyHomeValue
                    {
                      
                        PropertyId = model.PropertyId,
                        Value = model.Value,
                        HomeValueTypeId = model.TypeId,
                        Date = model.Date,
                        IsActive = model.IsActive
                    };
                    db.PropertyHomeValue.Add(newValue);
                    db.SaveChanges();
                    result = Json(new { Success = true, NewId = newValue.Id, IsActive = model.IsActive });
                }
                try
                {
                    return result;
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false });
                }
            }
            else
            {
                return Json(new { Success = false });
            }

        }

        [HttpPost]
        public ActionResult DeleteHomeValue(int homeValueId)
        {
            var homeValue = db.PropertyHomeValue.FirstOrDefault(x => x.Id == homeValueId);
            if (homeValue == null) return Json(new { Success = false });
            db.PropertyHomeValue.Remove(homeValue);
            db.SaveChanges();
            return Json(new { Success = true });
        }

        [HttpPost]
        public ActionResult DeleteRepayment(int repaymentId)
        {
            var repayment = db.PropertyRepayment.FirstOrDefault(x => x.Id == repaymentId);
            if (repayment == null) return Json(new { Success = false });
            db.PropertyRepayment.Remove(repayment);
            db.SaveChanges();
            return Json(new { Success = true });
        }

        [HttpPost]
        public ActionResult DeleteExpense(int expenseId)
        {
            var expense = db.PropertyExpense.FirstOrDefault(x => x.Id == expenseId);
            if (expense == null) return Json(new { Success = false });
            db.PropertyExpense.Remove(expense);
            db.SaveChanges();
            return Json(new { Success = true });
        }

        [HttpPost]
        public ActionResult DeleteRentalPayments(int rentalpaymentId)
        {
            var rentalpayment = db.PropertyRentalPayment.FirstOrDefault(x => x.Id == rentalpaymentId);
            if (rentalpayment == null) return Json(new { Success = false });
            db.PropertyRentalPayment.Remove(rentalpayment);
            db.SaveChanges();
            return Json(new { Success = true });
        }

        [HttpPost]
        public ActionResult SaveRepayment(RepaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    var repayment = db.PropertyRepayment.FirstOrDefault(x => x.Id == model.Id);
                    if (repayment == null) return Json(new { Success = false });
                    repayment.Amount = model.Amount;
                    repayment.FrequencyType = model.FrequencyType;
                    repayment.StartDate = model.StartDate;
                    repayment.EndDate = model.EndDate;
                    db.SaveChanges();
                    return Json(new { Success = true });
                }
                else
                {
                    var newRepayment = new PropertyRepayment
                    {
                        PropertyId = model.PropertyId,
                        Amount = model.Amount,
                        FrequencyType = model.FrequencyType,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate
                    };
                    db.PropertyRepayment.Add(newRepayment);
                    db.SaveChanges();
                    return Json(new { Success = true, NewId = newRepayment.Id });
                }
            }
            else
            {
                return Json(new { Success = false });
            }
        }

        [HttpPost]
        public ActionResult SaveExspense(ExpenseViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    var expense = db.PropertyExpense.FirstOrDefault(x => x.Id == model.Id);
                    if (expense == null) return Json(new { Success = false });
                    expense.Amount = model.Amount;
                    expense.Description = model.Description;
                    expense.Date = model.ExpenseDate;
                    db.SaveChanges();
                    return Json(new { Success = true });
                }
                else
                {
                    var newExpense = new PropertyExpense
                    {
                        PropertyId = model.PropertyId,
                        Amount = model.Amount,
                        Description = model.Description,
                        Date = model.ExpenseDate,
                    };
                    db.PropertyExpense.Add(newExpense);
                    db.SaveChanges();
                    return Json(new { Success = true, NewId = newExpense.Id });
                }
            }
            else
            {
                return Json(new { Success = false });
            }
        }

        [HttpPost]
        public ActionResult SaveRentalPayments(RentalPaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    var rentalpayment = db.PropertyRentalPayment.FirstOrDefault(x => x.Id == model.Id);
                    if (rentalpayment == null) return Json(new { Success = false });
                    rentalpayment.Amount = model.Amount;
                    rentalpayment.FrequencyType = model.FrequencyType;
                    rentalpayment.Date = model.Date;
                    db.SaveChanges();
                    return Json(new { Success = true , Id= rentalpayment.PropertyId });
                }
                else
                {
                    var newrentalpayment = new PropertyRentalPayment
                    {
                        PropertyId = model.PropertyId,
                        Amount = model.Amount,
                        FrequencyType = model.FrequencyType,
                        Date = model.Date,        
                    };
                    db.PropertyRentalPayment.Add(newrentalpayment);
                    db.SaveChanges();
                    return Json(new { Success = true, NewId = newrentalpayment.Id });
                }
            }
            else
            {
                return Json(new { Success = false });
            }
        }

        [HttpPost]
        public ActionResult DeleteRentalListing(int id)
        {
            var rl = db.RentalListing.First(p => p.Id == id);
            if (rl == null)
            {
                return Json(new
                {
                    Success = false,
                    Message = "The selected Rental Listing can't be found in the database."
                });
            }
            else
            {
                rl.IsActive = false;
                db.SaveChanges();

                return Json(new
                {
                    Success = true,
                    Message = "Rental Listing deleted successfully",
                    id = id
                });
            }

        }

        [HttpGet]
        public ActionResult TenantRequests()
        {
            
            return View();
        }

        public ActionResult MyRequests(POMyRequestsSearchModel model)
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
            var res = PropertyOwnerService.GetMyRequests(model, login);
            model.PagedInput = new PagedInput
            {
                ActionName = "MyRequests",
                ControllerName = "Property",
                PagedLinkValues = new RouteValueDictionary(new
                {
                    SortOrder = model.SortOrder,
                    SearchString = model.SearchString,
                    ReturnUrl = model.ReturnUrl,
                    RequestStatus = model.RequestStatus.ToString()
                }),

            };
            model.InputValues = new List<SearchInput>() {
                new SearchInput { Name = "RequestStatus", Value = model.RequestStatus.ToString()}
            };
            var rvr = new RouteValueDictionary(new { SearchString = model.SearchString, ReturnUrl = model.ReturnUrl , RequestStatus = model.RequestStatus.ToString()});
            var sortOrders = new List<SortOrderModel>();
            sortOrders.Add(new SortOrderModel { SortOrder = "Latest First", ActionName = "MyRequests", RouteValues = rvr.AddRouteValue("SortOrder", "Latest First") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Earliest First", ActionName = "MyRequests", RouteValues = rvr.AddRouteValue("SortOrder", "Earliest First") });
            
            model.SortOrders = sortOrders;
            model.SearchCount = res.SearchCount;
            if (String.IsNullOrWhiteSpace(model.SearchString)) model.Page = 1;
            model.PageCount = res.Items.PageCount;
            model.Items = res.Items;
            model.EditUrl = "/PropertyOwners/Property/EditMyRequest";
            model.DeleteUrl = "/PropertyOwners/Property/DeleteMyRequest";
            TempData["CurrentLink"] = "POMyRequests";
            return View(model);
            
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditMyRequest(RequestModel model)
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
        public JsonResult DeleteMyRequest(int Id)
        {
            var request = db.PropertyRequest.FirstOrDefault(x => x.Id == Id);
            if (request == null) return Json(new { Success = false, Message = "No record found!" });

            if (ModelState.IsValid)
            {
                try
                {
                    request.IsActive = false;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, ErrorMsg = ex.ToString() });
                }
            }
            else
            {
                return Json(new { Success = false, ErrorMsg = "Invalid Fields." });
            }

            return Json(new { Success = true, Message = "Request Deleted Successfully" });
        }

        //check add tenant
         [HttpPost]
         public JsonResult AddTenantToProperty(AddTenantToPropertyModel model)
         {
           
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);          
            var tenant = AccountService.GetLoginByEmail(model.TenantEmail);
            if (tenant == null)
            {               
                var temPass = UtilService.GeneraterRandomKey(8);
                var createRes = AccountService.CreateTenantAccount(model, login, temPass);

                if (!createRes.IsSuccess)
                {
                    tenant = createRes.NewObject as Login;
                    return Json(new { Success = false, NewPropId = model.Id });
                }
            }
            var newTenantProperty = new TenantProperty
            {
                Id = model.Id,
                TenantId = tenant.Id,
                CreatedBy = User.Identity.Name,
                CreatedOn = DateTime.Now,
                UpdatedBy = User.Identity.Name,
                UpdatedOn = DateTime.Now,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                PaymentAmount = model.PaymentAmount,
                PaymentFrequencyId = model.PaymentFrequencyId,
                PropertyId = model.PropertyId,
                PaymentStartDate = model.PaymentStartDate,
                PaymentDueDate = model.PaymentDueDate,
                IsMainTenant = model.IsMainTenant,
                IsActive= tenant.IsActive
            };
            if (model.Liabilities != null)
            {
                model.Liabilities.ForEach(x =>
                {
                    db.TenantPropertyLiability.Add(new TenantPropertyLiability
                    {
                        LiabilityName = x.Name,
                        Amount = x.Amount,
                        TenantProperty = newTenantProperty
                    });
                });
            }
            
            db.TenantProperty.Add(newTenantProperty);
            db.SaveChanges();
            return Json(new { Success = true, NewId = newTenantProperty.Id });           
         }


        protected override void Dispose(bool disposing)
        {
            db?.Dispose();
            base.Dispose(disposing);
        }
    }
}

