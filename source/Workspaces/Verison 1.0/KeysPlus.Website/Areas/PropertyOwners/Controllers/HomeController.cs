using KeysPlus.Data;
using KeysPlus.Service.Models;
using KeysPlus.Service.Services;
using KeysPlus.Website.Areas.PropertyOwners.Models;
using KeysPlus.Website.Areas.Tools;
using KeysPlus.Website.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KeysPlus.Website.Areas.PropertyOwners.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        KeysEntities db = new KeysEntities();

        public ActionResult Index(POPropSearchModel model)
        {
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            if (String.IsNullOrWhiteSpace(model.SortOrder))
            {
                model.SortOrder = "Latest Date";
            }
            var prop = db.OwnerProperty.Where(x => x.PropertyId == 2678).First();
            var data = db.OwnerProperty.Where(x => x.Person.Login.UserName == User.Identity.Name && x.Property.IsActive)
                .Select( x => new PropViewModel {
                    Model = new PropertyModel {
                        Id = x.PropertyId,
                        PropertyTypeId = x.Property.PropertyTypeId,
                        Name = x.Property.Name,
                        Description = x.Property.Description,
                        Bedroom = x.Property.Bedroom,
                        Bathroom = x.Property.Bathroom,
                        LandSqm = x.Property.LandSqm,
                        ParkingSpace = x.Property.ParkingSpace,
                        FloorArea = x.Property.FloorArea,
                        TargetRent = x.Property.TargetRent,
                        TargetRentTypeId = x.Property.TargetRentTypeId,
                        YearBuilt = x.Property.YearBuilt,
                        IsOwnerOccupied = x.Property.IsOwnerOccupied,
                        Address = new AddressViewModel
                        {
                            AddressId = x.Property.Address.AddressId,
                            CountryId = x.Property.Address.CountryId,
                            Number = x.Property.Address.Number.Replace(" ", ""),
                            Street = x.Property.Address.Street.Trim(),
                            City = x.Property.Address.City.Trim(),
                            Suburb = x.Property.Address.Suburb.Trim() ?? "",
                            PostCode = x.Property.Address.PostCode.Replace(" ", ""),
                            Latitude = x.Property.Address.Lat,
                            Longitude = x.Property.Address.Lng,
                        },
                        MediaFiles = x.Property.PropertyMedia.Select( y => new MediaModel {Id=y.Id, NewFileName =y.NewFileName, OldFileName = y.OldFileName }).ToList()
                    },
                    PropertyTypeName = x.Property.PropertyType.Name,
                    CreatedOn = x.Property.CreatedOn,
                    PurchasePrice = x.Property.PropertyFinance.PurchasePrice,
                    CurrentHomeValue = x.Property.PropertyFinance.CurrentHomeValue,
                    TenantCount = x.Property.TenantProperty.Where(y => y.IsActive ?? true).Count(),
                });
            var allItems = data.OrderByDescending(x => x.CreatedOn).ToPagedList(model.Page, 10);
            switch (model.SortOrder)
            {
                case "Name":
                    data = data.OrderBy(s => s.Model.Name);
                    break;
                case "Name(Desc)":
                    data = data.OrderByDescending(s => s.Model.Name);
                    break;
                case "Latest Date":
                    data = data.OrderByDescending(s => s.CreatedOn);
                    break;
                case "Earliest Date":
                    data = data.OrderBy(s => s.CreatedOn);
                    break;
                default:
                    data = data.OrderByDescending(s => s.CreatedOn);
                    break;
            }
            if (!String.IsNullOrWhiteSpace(model.SearchString))
            {
                SearchTool searchTool = new SearchTool();
                int searchType = searchTool.CheckDisplayType(model.SearchString);
                string formatString = searchTool.ConvertString(model.SearchString);
                switch (searchType)
                {
                    case 1:
                        data = data.Where(x => x.Model.Name.ToLower().EndsWith(formatString)
                                             || x.Model.Address.City.ToLower().EndsWith(formatString)
                                             || x.Model.Address.Suburb.ToLower().EndsWith(formatString));
                        break;
                    case 2:
                        data = data.Where(x => x.Model.Name.ToLower().StartsWith(formatString)
                                              || x.Model.Address.City.ToLower().StartsWith(formatString)
                                              || x.Model.Address.Suburb.ToLower().StartsWith(formatString));
                        break;
                    case 3:
                        data = data.Where(x => x.Model.Name.ToLower().Contains(formatString)
                                                || x.Model.Address.City.ToLower().Contains(formatString)
                                               || x.Model.Address.Suburb.ToLower().Contains(formatString));
                        break;
                }
            }
            var items = data.ToPagedList(model.Page, 10);
            var count = items.Count;
            items = count == 0 ? allItems : items;
            items.ToList().ForEach(x => x.Model.MediaFiles.ForEach(y => y.InjectMediaModelViewProperties()));
            var result = new SearchResult { SearchCount = items.Count, Items = count == 0 ? allItems : items };
            model.PagedInput = new PagedInput
            {
                ActionName = "Index",
                ControllerName = "Home",
                PagedLinkValues = new RouteValueDictionary(new { SortOrder = model.SortOrder, SearchString = model.SearchString })
            };
            var rvr = new RouteValueDictionary(new { SearchString = model.SearchString });
            var sortOrders = new List<SortOrderModel>();
            sortOrders.Add(new SortOrderModel { SortOrder = "Name", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Name") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Name(Desc)", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Name(Desc)") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Latest Date", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Latest Date") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Earliest Date", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Earliest Date") });
            model.SortOrders = sortOrders;
            model.SearchCount = result.SearchCount;
            if (String.IsNullOrWhiteSpace(model.SearchString)) model.Page = 1;
            model.PageCount = result.Items.PageCount;
            model.Items = result.Items;
            model.EditUrl = "/PropertyOwners/Home/Edit";
            model.DeleteUrl = "/PropertyOwners/Home/Delete";
            model.CanListRental = allItems.Any();
            ViewBag.Frequencies = PropertyOwnerService.GetAllPaymentFrequencies();
            ViewBag.PropertyTypes = PropertyService.GetAllProprtyTypes();
            return View(model);
        }
        //public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        //{
        //    var freqs = PropertyOwnerService.GetAllPaymentFrequencies();
        //    var propertyTypes = PropertyService.GetAllProprtyTypes();
        //    var propertyHomeValueTypes = PropertyService.GetAllProprtyHomeValueTypes();
        //    ViewBag.Frequencies = freqs;
        //    ViewBag.PropertyTypes = propertyTypes;
        //    ViewBag.PropertyHomeValueTypes = propertyHomeValueTypes;
        //    var dbContext = db.Property;
        //    ViewBag.SearchCount = 1;
        //    ViewBag.CurrentSort = sortOrder;
        //    ViewBag.CurrentFilter = searchString;
        //    ViewBag.NameSortParm = sortOrder == "Name" ? "Name_Desc" : "Name";
        //    ViewBag.DateSortParm = sortOrder == "Date" ? "Date_Desc" : "Date";
        //    ViewBag.FirstNameSortParm = sortOrder == "Fname_asc" ? "Fname" : "Fname_asc";
        //    ViewBag.MiddleNameSortParm = sortOrder == "Mname_asc" ? "Mname" : "Mname_asc";

        //    if (searchString != null)
        //    {
        //        page = 1;
        //    }
        //    else
        //    {
        //        searchString = currentFilter;
        //    }

        //    var propertyContext = db.OwnerProperty.Where(p => p.Person.Login.UserName == User.Identity.Name && p.Property.IsActive)
        //        .Select(p => new PropertyViewModel
        //        {
        //            Id = p.Property.Id,
        //            PropertyTypeId = p.Property.PropertyTypeId,
        //            IsActive = p.Property.IsActive,
        //            Name = p.Property.Name,
        //            Description = p.Property.Description,
        //            CreatedOn = p.Property.CreatedOn.ToString(),
        //            CreatedDate = p.Property.CreatedOn,
        //            FloorArea = p.Property.FloorArea,
        //            ParkingSpace = p.Property.ParkingSpace,
        //            TargetRent = p.Property.TargetRent,
        //            TargetRentType = p.Property.TargetRentTypeId,
        //            Bedroom = p.Property.Bedroom,
        //            Bathroom = p.Property.Bathroom,
        //            LandArea = p.Property.LandSqm,
        //            YearBuilt = (int)p.Property.YearBuilt,
        //            ResidentialType = p.Property.PropertyType.PropertyTypeId,
        //            CommercialType = p.Property.PropertyType.PropertyTypeId,
        //            Mortgage = p.Property.PropertyFinance.Mortgage,
        //            PurchasePrice = p.Property.PropertyFinance.PurchasePrice,
        //            CapitalGain = p.Property.PropertyFinance.CapitalGain,
        //            CurrentHomeValue = p.Property.PropertyFinance.CurrentHomeValue,
        //            TotalExpenses = p.Property.PropertyFinance.TotalExpense,
        //            TotalRepayment = p.Property.PropertyFinance.TotalRepayment,
        //            Yield = p.Property.PropertyFinance.Yield,
        //            ActualRentalPayments = p.Property.PropertyFinance.ActualRentalPayments,
        //            TenantCount = p.Property.TenantProperty.Where(x => x.IsActive ?? true).Count(),
        //            IsOwnerOccupied = p.Property.IsOwnerOccupied ?? false,
        //            Address = new AddressViewModel
        //            {
        //                AddressId = p.Property.Address.AddressId,
        //                CountryId = p.Property.Address.CountryId,
        //                Number = p.Property.Address.Number.Replace(" ", ""),
        //                Street = p.Property.Address.Street.Trim(),
        //                City = p.Property.Address.City.Trim(),
        //                Suburb = p.Property.Address.Suburb.Trim() ?? "",
        //                PostCode = p.Property.Address.PostCode.Replace(" ", ""),
        //                Latitude = p.Property.Address.Lat,
        //                Longitude = p.Property.Address.Lng,
        //            }
        //        }).AsQueryable();

        //    var login = AccountService.GetLoginByEmail(User.Identity.Name);

        //    var propertyContextfull = propertyContext.OrderBy(s => s.Name);
        //    switch (sortOrder)
        //    {
        //        case "Name_Desc":
        //            propertyContext = propertyContext.OrderByDescending(s => s.Name);
        //            break;
        //        case "Name":
        //            propertyContext = propertyContext.OrderBy(s => s.Name);
        //            break;
        //        case "Date":
        //            propertyContext = propertyContext.OrderBy(s => s.CreatedDate);
        //            break;
        //        case "Date_Desc":
        //            propertyContext = propertyContext.OrderByDescending(s => s.CreatedDate);
        //            break;
        //        default:
        //            propertyContext = propertyContext.OrderByDescending(s => s.CreatedDate);
        //            break;
        //    }
        //    if (!String.IsNullOrWhiteSpace(searchString))
        //    {
        //        SearchTool searchTool = new SearchTool();
        //        int searchType = searchTool.CheckDisplayType(searchString);
        //        string formatString = searchTool.ConvertString(searchString);
        //        switch (searchType)
        //        {
        //            case 1:
                        
        //                propertyContext = propertyContext.Where(property => property.Name.ToLower().EndsWith(formatString)
        //                                     || property.Address.City.ToLower().EndsWith(formatString)
        //                                     || property.Address.Suburb.ToLower().EndsWith(formatString));

        //                break;
        //            case 2:
                       
        //                propertyContext = propertyContext.Where(property => property.Name.ToLower().StartsWith(formatString)
        //                                      || property.Address.City.ToLower().StartsWith(formatString)
        //                                      || property.Address.Suburb.ToLower().StartsWith(formatString));

        //                break;
        //            case 3:
        //                propertyContext = propertyContext.Where(property => property.Name.ToLower().Contains(formatString)
        //                                       || property.Address.City.ToLower().Contains(formatString)
        //                                       || property.Address.Suburb.ToLower().Contains(formatString));

        //                break;
        //        }

        //        if (propertyContext.ToList().Count() == 0)
        //        {
        //            ViewBag.SearchCount = 0;
        //            TempData["search"] = searchString ?? "";
        //            ViewBag.CurrentFilter = "";
        //            propertyContext = propertyContextfull;
        //        }
        //    }
        //    int pageNumber = (page ?? 1);
        //    var properties = propertyContext.ToPagedList(pageNumber, 10);
        //    foreach (var item in properties)
        //    {
        //        item.PhotoFiles = new List<MediaModel>();
        //        var property = dbContext.Include("PropertyMedia").Single(p => p.Id == item.Id);
        //        item.CreatedOn = property.CreatedOn.ToString("s", CultureInfo.InvariantCulture);
        //        var photos = property.PropertyMedia;
        //        var hasPhotos = photos.AsEnumerable().Count() == 0 ? true : false;
        //        foreach (var photo in photos)
        //        {
        //            item.PhotoFiles.Add(new MediaModel()
        //            {
        //                Id = photo.Id,
        //                Data = Url.Content("~/images/" + photo.NewFileName),
        //                Status = "load",
        //                MediaType = MediaService.GetMediaType(photo.OldFileName),
        //            });
        //        }               
        //    }
        //    if (Request.IsAjaxRequest())
        //    {
        //        return PartialView("_PagedProperties", properties);
        //    }
        //    TempData["CurrentLink"] = "Properties";
        //    return View(properties);
        //}


        [HttpPost]
        public ActionResult Create(PropertyViewModel model)
        {
            var status = true;
            var message = "";
            var data = model;
            var currentUser = db.Person.Where(x => x.Login.UserName == User.Identity.Name).FirstOrDefault();
            var newOwner = new Owners();
            // model.PropertyTypeId = 1;
            var OwnerPerson = db.Owners.Any(n => n.Person.Login.UserName == User.Identity.Name);
            if (OwnerPerson == false)
            {
                newOwner.Person = currentUser;
                db.Owners.Add(newOwner);
                db.SaveChanges();
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                status = false;
                message = "Something went wrong, perhaps a field is invalid.";
            }
            else
            {
                var address = model.Address;
                var newProperty = new Property();
                // var newFinancial = new PropertyFinance();
                var newOwnerProperty = new OwnerProperty();
                var newpropModel = new PropertyViewModel();
                var newAddressModel = new AddressViewModel();
                var newAddress = new Address
                {
                    CountryId = address.CountryId,
                    Number = address.Number.Replace(" ", ""),
                    Street = address.Street.Trim(),
                    City = address.City.Trim(),
                    PostCode = address.PostCode.Replace(" ", ""),
                    Suburb = address.Suburb,
                    CreatedBy = HttpContext.User.Identity.Name,
                    CreatedOn = DateTime.Today,
                    UpdatedBy = HttpContext.User.Identity.Name,
                    UpdatedOn = DateTime.Today,
                    Lat = address.Latitude,
                    Lng = address.Longitude,
                    IsActive = true
                };

                db.Address.Add(newAddress);
                db.SaveChanges();

                newAddressModel.AddressId = newAddress.AddressId;
                newAddressModel.CountryId = newAddress.CountryId;
                newAddressModel.Number = newAddress.Number.Replace(" ", "");
                newAddressModel.Street = newAddress.Street.Trim();
                newAddressModel.City = newAddress.City.Trim();
                newAddressModel.Suburb = newAddress.Suburb;
                newAddressModel.PostCode = newAddress.PostCode.Replace(" ", "");
                newAddressModel.Latitude = newAddress.Lat;
                newAddressModel.Longitude = newAddress.Lng;

                newProperty.Address = newAddress;
                newProperty.Name = model.Name;
                newProperty.IsActive = model.IsActive;
                newProperty.Bathroom = model.Bathroom;
                newProperty.Bedroom = model.Bedroom;
                newProperty.CreatedBy = User.Identity.Name;
                newProperty.CreatedOn = DateTime.Today;
                newProperty.UpdatedBy = User.Identity.Name;
                newProperty.UpdatedOn = DateTime.Today;
                newProperty.Description = model.Description;
                newProperty.FloorArea = model.FloorArea;
                newProperty.LandSqm = model.LandArea;
                newProperty.ParkingSpace = model.ParkingSpace;
                newProperty.YearBuilt = model.YearBuilt;
                newProperty.PropertyTypeId = model.PropertyTypeId;
                newProperty.TargetRent = model.PaymentAmount;
        newProperty.TargetRentTypeId = model.TargetRentType;
                newProperty.IsOwnerOccupied = model.IsOwnerOccupied;
                db.Property.Add(newProperty);

                /*
                newFinancial.Property = newProperty;
                newFinancial.PurchasePrice = model.PurchasePrice;
                newFinancial.TotalRepayment = model.TotalRepayment;
                newFinancial.Mortgage = model.Mortgage;
                db.PropertyFinance.Add(newFinancial);
                */

                newOwnerProperty.Property = newProperty;
                newOwnerProperty.OwnershipStatusId = 1;
                newOwnerProperty.Person = currentUser;
                newOwnerProperty.OwnerId = currentUser.Id;
                newOwnerProperty.CreatedBy = User.Identity.Name;
                newOwnerProperty.UpdatedBy = User.Identity.Name;
                newOwnerProperty.PurchaseDate = DateTime.Today;  // Should Create a feild in the Add property page and get value from there
                newOwnerProperty.UpdatedOn = DateTime.Today;
                newOwnerProperty.CreatedOn = DateTime.Today;

                db.OwnerProperty.Add(newOwnerProperty);

                db.SaveChanges();

                newpropModel.Id = newProperty.Id;
                newpropModel.Address = newAddressModel;
                newpropModel.IsActive = newProperty.IsActive;
                newpropModel.CreatedOn = newProperty.CreatedOn.ToString("s", CultureInfo.InvariantCulture);
                newpropModel.Name = newProperty.Name;
                newpropModel.Bathroom = newProperty.Bathroom ?? 0;
                newpropModel.Bedroom = newProperty.Bedroom ?? 0;
                newpropModel.FloorArea = newProperty.FloorArea;
                newpropModel.LandArea = newProperty.LandSqm;
                newpropModel.ParkingSpace = newProperty.ParkingSpace;
                newpropModel.PropertyTypeId = newProperty.PropertyTypeId;
                newpropModel.PropertyType = (newProperty.PropertyTypeId >= 7) ? "Commercial" : "Residential";
                newpropModel.TargetRent = newProperty.TargetRent ?? 0;
                newpropModel.TargetRentType = model.TargetRentType;
                newpropModel.YearBuilt = newProperty.YearBuilt ?? 0;
                newpropModel.Description = newProperty.Description;
                newpropModel.RentalApplications = new List<RentalApplicationsViewModel> { };
                data = newpropModel;
            }

            return Json(new
            {
                success = status,
                message = message,
                data = data
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PropertyModel model)
        {
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            if (String.IsNullOrEmpty(user))
            {
                return Json(new { Success = false });
            }
            var files = Request.Files;
            var result = PropertyOwnerService.EditProperty(model, files, login);
            return Json(new
            {
                Success = result.IsSuccess,
                Message = result.ErrorMessage,
            });
        }

        public ActionResult Delete(int id,string returnUrl)
        {
            var property = db.Property.First(p => p.Id == id);
            if (property == null)
            {
                return Json(new
                {
                    Success = false,
                    message = "The selected property can't be found in the database."
                });
            }
            else
            {
                property.IsActive = false;
                foreach (var item in property.RentalListing) item.IsActive = false;
                db.SaveChanges();

                // return Json(new
                //   {
                //     Success = true,
                //   message = "Property deleted successfully",
                // id = id
                //  });
                return Redirect(returnUrl);
            }
        }

        public JsonResult UpdatePhotos(int id)
        {
            var allowedPhotos = 10;
            var message = "Photo added successfully";
            var status = true;
            var photos = Request.Files;
            //var deleted = Request.Form.AllKeys;
            var propId = id;

            if (photos.Count > allowedPhotos)
            {
                status = false;
                message = $"You can't add more than {allowedPhotos} photos";
            }

            //Remove all images in database with particular property id
            //var delmedia = db.PropertyMedia.Where(x => x.PropertyId == propId).ToList();
            //if (delmedia != null)
            //{
            //    foreach (var dm in delmedia)
            //    {
            //        db.PropertyMedia.Remove(dm);
            //    }
            //    db.SaveChanges();
            //}

            if (photos != null && photos.Count > 0)
            {

                var numberOfPhotos = photos.Count < allowedPhotos ? photos.Count : allowedPhotos;

                List<string> acceptedExtensions = new List<string>
                {
                    ".jpg",
                    ".png",
                    ".gif",
                    ".jpeg"
                };

                for (int i = 0; i < numberOfPhotos; i++)
                {
                    var file = photos[i];

                    var fileExtension = Path.GetExtension(file.FileName);

                    if (fileExtension != null && !acceptedExtensions.Contains(fileExtension.ToLower()))
                    {
                        message = "Supported file types are *.jpg, *.png, *.gif, *.jpeg";
                        status = false;
                        break;
                    }
                    else
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var index = fileName.LastIndexOf(".");
                        var newFileName = fileName.Insert(index, $"{Guid.NewGuid()}");
                        var physicalPath = Path.Combine(Server.MapPath("~/images"), newFileName);

                        db.PropertyMedia.Add(new PropertyMedia()
                        {
                            PropertyId = id,
                            NewFileName = newFileName,
                            OldFileName = fileName
                        });

                        file.SaveAs(physicalPath);
                    }
                }
                db.SaveChanges();
            }
            else
            {
                message = "You have not specified a file.";
                status = false;
            }

            return Json(new
            {
                success = status,
                message = message
            });
        }
        [HttpPost]
        public JsonResult AddTenant(FinancialModel model)
        {
            AddTenantToPropertyModel tenant = new AddTenantToPropertyModel();
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            var ten = AccountService.GetLoginByEmail(model.TenantToPropertyModel.TenantEmail);
            if (ten == null)
            {
                tenant.TenantEmail = model.TenantToPropertyModel.TenantEmail;
                tenant.StartDate = model.TenantToPropertyModel.StartDate;
                tenant.EndDate = model.TenantToPropertyModel.EndDate;
                tenant.PaymentAmount = model.TenantToPropertyModel.PaymentAmount;
                tenant.PaymentFrequencyId = model.TenantToPropertyModel.PaymentFrequencyId;
                tenant.PropertyId = model.PropId;

                //var TenantEmailResult = SendInvitationEmailToTenant(tenant);

                return Json(new { Success = false, NewPropId = model.PropId, Todo = "Send email", ErrorMsg = "Cannot find person in login table!" });
            }
            else
            {
                var person = AccountService.GetPersonByLoginId(ten.Id);
                var result = PropertyService.AddTenantToProperty(login, person.Id, model.PropId, model.TenantToPropertyModel.StartDate,
                    model.TenantToPropertyModel.EndDate, model.TenantToPropertyModel.PaymentFrequencyId, model.TenantToPropertyModel.PaymentAmount);
                if (result.IsSuccess)
                {
                    return Json(new { Sucess = true, Msg = "Added!", result = "Redirect", url = Url.Action("Index", "PropertyOwners") });

                }
                else
                {
                    return Json(new { Sucess = false, Msg = result.ErrorMessage, redirect = "Redirect", url = Url.Action("Index", "PropertyOwners") });
                }
            }
        }

        [HttpPost]
        public JsonResult UpdateFinance (FinancialModel model)
        {
            var status = true;
            decimal actualTotalRepayment = 0;
            var message = "Record added successfully";
            var data = model;
            // AddTenantToPropertyModel tenant = new AddTenantToPropertyModel();
            //*********** AddNewProperty
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            //var newProp = PropertyOwnerService.AddOnboardProperty(login, model);

            ////*********** AddRepayments

            var newRepayment = new PropertyRepayment();



            newRepayment = PropertyOwnerService.AddOnboardRepayment(login, model.Repayments, model.PropId);
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
        
            //*****AddExpenses
            var newExpense = new PropertyExpense();
                newExpense = PropertyOwnerService.AddOnboardExpense(login, model.Expenses, model.PropId);
                //******AddFinancial
                var newFinancial = new PropertyFinance();
                newFinancial = PropertyOwnerService.AddPropertyFinance(login, model, model.PropId, actualTotalRepayment);

                var newRentalPayment= new PropertyRentalPayment();
          //      newRentalPayment = PropertyOwnerService.AddOnboardRentalPayment(login, model.RentalPayments, model.PropId);
            }

            return Json(new { success = status, message = message });

        }

        [HttpGet]
        public ActionResult GetRepayments(int id)
        {

            var data = PropertyService.GetRepayments(id).ToList();
            //var TotalRepaymentsForPeriod = PropertyService.GetTotalIdvRepayment(.Amount, x.StartDate, x.EndDate ?? DateTime.Now, x.FrequencyType);
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetExpenses(int id)
        {
            var data = PropertyService.GetExpenses(id).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetRentalPayments(int id)
        {
            var data = PropertyService.GetRentalPayments(id).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddTenantDashBoard(int? propId, string returnUrl)
        {
            var freqs = PropertyOwnerService.GetAllPaymentFrequencies();
            ViewBag.Frequencies = freqs;
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            var properties = PropertyService.GetPropertiesAndAddress(login.Id, propId).ToList();
            properties.ForEach(x => x.AddressString = x.Address.ToAddressString());
            var model = new PropDataModel
            {
                ReturnUrl = returnUrl,
                Properties = properties
            };
            ViewBag.ReturnUrl = returnUrl ?? "/PropertyOwners/Home/Dashboard";
            return View(model); 
        }

        [HttpGet]
        public ActionResult EditTenantInProperty(int tenantId, int propId, string returnUrl)
        {
            var freqs = PropertyOwnerService.GetAllPaymentFrequencies();
            var user = User.Identity.Name;
            var id = AccountService.GetLoginByEmail(user).Id;
            var property = PropertyService.GetPropertyById(propId);

            ViewBag.Frequencies = freqs;
            ViewBag.ReturnUrl = returnUrl;

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

                    ViewBag.Property = propertyVm;
                }
            }

            var propertyTenant = PropertyService.GetTenantInProperty(tenantId, propId);
         
            return View(propertyTenant);

        }


        protected override void Dispose(bool disposing)
        {
            db?.Dispose();
            base.Dispose(disposing);
        }
        
    }
}