using KeysPlus.Data;
using System;
using System.Linq;
using System.Web.Mvc;
using KeysPlus.Service.Models;
using KeysPlus.Service.Services;
using PagedList;
using System.Collections.Generic;


namespace KeysPlus.Website.Areas.Rental.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        KeysEntities db = new KeysEntities();

        public ActionResult Index(RentalListingSearchModel model)
        {
            
            TempData["CurrentLink"] = "RentallListing";
            return View(model);
        }

        public ActionResult AddRentalApplication(RentalApplicationModel model)
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
            }
            return Json(new { Success = false });
        }

        public ActionResult SendRequest(string returnUrl, int? propId, int? type)
        {
            var user = User.Identity.Name;
            var login = AccountService.GetLoginByEmail(user);
            var properties = PropertyService.GetPropertiesAndAddress(login.Id, propId, true).ToList();
            properties.ForEach(x => x.AddressString = x.Address.ToAddressString());
            var requestTypes = PropertyService.GetRequestTypes().Where(x => x.Id != 1).ToList();
            if (type.HasValue)
            {
                requestTypes = requestTypes.Where( x => x.Id == type).ToList();
            }
            var model = new PropertyRequestModel
            {
                AvalableProperties = properties,
                RequestTypes = requestTypes

            };
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SubmitPropertyRequest(RequestModel model)
        {

            if (ModelState.IsValid)
            {
                var files = Request.Files;//.MediaFiles;// Request.Files;
                var userName = User.Identity.Name;
                if (String.IsNullOrEmpty(userName))
                {
                    return Json(new { Success = false, ErrorMsg = "User not exixt!" });
                }
                var login = AccountService.GetLoginByEmail(userName);
                var result = PropertyService.AddPropertyRequest(login, model, Request.Files);
                return result.IsSuccess ? Json(new { Success = true }) : Json(new { Success = false, ErrorMsg = result.ErrorMessage });
            } else
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return Json(new { Success = false, Model = allErrors });
            }
            
            
        }

        [HttpGet]
        public ActionResult AdvancedSearch()
        {
            GetPropertyTypes();
            return View();
        }

        private void GetPropertyTypes()
        {
            var propertyType = PropertyService.GetAllProprtyTypes().ToList();
            ViewBag.TypeOfProperty = propertyType;
        }

        [HttpPost]
        public ActionResult AdvancedSearch(string sortOrder, int? page, RentalAdvancedSearchViewModel advancedSearch)
        {
            TempData["AdvancedSearch"] = advancedSearch;
            return RedirectToAction("AdvanceSearchResult", new { sortOrder = sortOrder ?? "Title" });
        }

        [HttpPost]
        public ActionResult AdvanceSearchResult(string sortOrder, int? page, RentalAdvancedSearchViewModel advancedSearch)
        {
            TempData["AdvancedSearch"] = advancedSearch;
            return RedirectToAction("AdvanceSearchResult", new { sortOrder = sortOrder ?? "Title" });
        }
        
        private static string CreateFilterForPaging(string currentFilter, RentalAdvancedSearchViewModel advancedSearch)
        {
            var newFilter = currentFilter;

            if (!String.IsNullOrEmpty(currentFilter))
            {
                Dictionary<string, string> keyValuePairs = currentFilter.Split(',')
                                    .Select(value => value.Split('='))
                                    .ToDictionary(pair => pair[0], pair => pair[1]);
                if (keyValuePairs["BedroomMax"] != "")
                {
                    advancedSearch.BedroomMax = Convert.ToInt32(keyValuePairs["BedroomMax"]);
                }
                if (keyValuePairs["BedroomMin"] != "")
                {
                    advancedSearch.BedroomMin = Convert.ToInt32(keyValuePairs["BedroomMin"]);
                }
                if (keyValuePairs["BathroomMax"] != "")
                {
                    advancedSearch.BathroomMax = Convert.ToInt32(keyValuePairs["BathroomMax"]);
                }
                if (keyValuePairs["BathroomMin"] != "")
                {
                    advancedSearch.BathroomMin = Convert.ToInt32(keyValuePairs["BathroomMin"]);
                }
                if (keyValuePairs["RentMax"] != "")
                {
                    advancedSearch.RentMax = Convert.ToDecimal(keyValuePairs["RentMax"]);
                }
                if (keyValuePairs["RentMin"] != "")
                {
                    advancedSearch.RentMin = Convert.ToDecimal(keyValuePairs["RentMin"]);
                }
                if (keyValuePairs["LandSqmMax"] != "")
                {
                    advancedSearch.LandSqmMax = Convert.ToInt32(keyValuePairs["LandSqmMax"]);
                }
                if (keyValuePairs["LandSqmMin"] != "")
                {
                    advancedSearch.LandSqmMin = Convert.ToInt32(keyValuePairs["LandSqmMin"]);
                }
                if (keyValuePairs["Title"] != "")
                {
                    advancedSearch.Title = keyValuePairs["Title"];
                }
                for (int j = 0; keyValuePairs.ContainsKey(("Suburb" + j)); j++)
                {
                    advancedSearch.Address.SuburbList.Add(keyValuePairs["Suburb" + j]);
                }
                for (int j = 0; keyValuePairs.ContainsKey(("PropertyType" + j)); j++)
                {
                    advancedSearch.PropertyType.Add(keyValuePairs["PropertyType" + j]);
                }
            }
            else
            {
                newFilter = "BedroomMax=" + advancedSearch.BedroomMax +
                            ",BedroomMin=" + advancedSearch.BedroomMin +
                            ",BathroomMax=" + advancedSearch.BathroomMax +
                            ",BathroomMin=" + advancedSearch.BathroomMin +
                            ",RentMax=" + advancedSearch.RentMax +
                            ",RentMin=" + advancedSearch.RentMin +
                            ",LandSqmMax=" + advancedSearch.LandSqmMax +
                            ",LandSqmMin=" + advancedSearch.LandSqmMin +
                            ",Title=" + advancedSearch.Title;
                for (int i = 0; i < advancedSearch.Address.SuburbList.Count; i++)
                {
                    newFilter = newFilter + ",Suburb" + i + "=" + advancedSearch.Address.SuburbList[i];
                }
                for (int i = 0; i < advancedSearch.PropertyType.Count; i++)
                {
                    newFilter = newFilter + ",PropertyType" + i + "=" + advancedSearch.PropertyType[i];
                }
            }

            return newFilter;
        }

        private RentalAdvancedSearchViewModel InitialiazeSuburbListAndPropertyType()
        {
            RentalAdvancedSearchViewModel advancedSearch = (RentalAdvancedSearchViewModel)TempData["AdvancedSearch"] ?? new RentalAdvancedSearchViewModel();
            if (advancedSearch.Address == null) { advancedSearch.Address = new AddressViewModel(); }
            if (advancedSearch.Address.SuburbList == null) { advancedSearch.Address.SuburbList = new List<string>(); }
            if (advancedSearch.Address.Suburb != null) { advancedSearch.Address.Suburb = advancedSearch.Address.Suburb.ToLower(); }
            if (advancedSearch.Title != null) { advancedSearch.Title = advancedSearch.Title.ToLower(); }
            if (advancedSearch.PropertyType == null) { advancedSearch.PropertyType = new List<String>(); }

            return advancedSearch;
        }

       
        public JsonResult GetTenantsByPropertyId(int propId)
        {
            var result = PropertyService.GetTenantsByPropId(propId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRequestTypes()
        {
            var result = PropertyService.GetRequestTypes();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }

}