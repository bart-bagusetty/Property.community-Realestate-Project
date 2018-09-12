using KeysPlus.Data;
using KeysPlus.Service.Models;
using KeysPlus.Service.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KeysPlus.Website.Areas.Personal.Controllers
{
    public class WatchlistController : Controller
    {
        KeysEntities db = new KeysEntities();
        // GET: Personal/Watchlist
        public ActionResult Index(WatchlistDisplayModel model)
        {
            var login = AccountService.GetLoginByEmail(User.Identity.Name);
            var loginRole = (from r in db.Role
                             join lr in db.LoginRole on r.Id equals lr.RoleId
                             where lr.PersonId == login.Id
                             select r).FirstOrDefault().Name;

            using (var db = new KeysEntities())
            {
                //IPagedList res;

                if (model.ItemType == ItemType.RentalListing || model.ItemType == 0)
                {
                    model.ItemType = ItemType.RentalListing;
                    model = getRentalWatchlist(model, login);
                }
                else if (model.ItemType == ItemType.MarketJob)
                {
                    model = getMarketJobWatchlist(model, login);
                }

                return View(model);
            }

            switch (loginRole)
            {
                case "Tenant":
                    model.ItemType = ItemType.RentalListing;
                    break;

                case "Service Suppliers":
                    model.ItemType = ItemType.MarketJob;
                    break;

                default:
                    model.ItemType = ItemType.RentalListing;
                    break;
            }

        }

        public WatchlistDisplayModel getRentalWatchlist(WatchlistDisplayModel model, Login login)
        {            
            var data = db.RentalWatchList.Where(x => x.PersonId == login.Id && x.IsActive)
                    .Select(x => new WatctlistItem<RentListingModel>
                    {
                        Model = new RentListingModel
                        {
                            Id = x.RentalListing.Id,
                            MovingCost = x.RentalListing.MovingCost,
                            TargetRent = x.RentalListing.TargetRent,
                            AvailableDate = x.RentalListing.AvailableDate,
                            Furnishing = x.RentalListing.Furnishing,
                            OccupantCount = x.RentalListing.OccupantCount,
                            PetsAllowed = x.RentalListing.PetsAllowed,
                            Title = x.RentalListing.Title,
                            Description = x.RentalListing.Description,

                            PropertyId = x.RentalListing.PropertyId,
                            IdealTenant = x.RentalListing.IdealTenant,
                            IsActive = x.RentalListing.IsActive,
                            RentalStatusId = x.RentalListing.RentalStatusId,
                            MediaFiles = x.RentalListing.RentalListingMedia.Select(y => new MediaModel { Id = y.Id, NewFileName = y.NewFileName, OldFileName = y.OldFileName }).ToList()

                        },
                        Address = new AddressViewModel
                        {
                            Street = x.RentalListing.Property.Address.Street,
                            Suburb = x.RentalListing.Property.Address.Suburb,

                            AddressId = x.RentalListing.Property.Address.AddressId,
                            CountryId = x.RentalListing.Property.Address.AddressId,
                            Number = x.RentalListing.Property.Address.Number,
                            Region = x.RentalListing.Property.Address.Region,
                            City = x.RentalListing.Property.Address.City,
                            PostCode = x.RentalListing.Property.Address.PostCode
                        },
                        Property = new PropertyViewModel
                        {
                            Bedroom = x.RentalListing.Property.Bedroom,
                            Bathroom = x.RentalListing.Property.Bathroom,
                            FloorArea = x.RentalListing.Property.FloorArea,
                            LandArea = x.RentalListing.Property.LandSqm,
                            ParkingSpace = x.RentalListing.Property.ParkingSpace,
                            CreatedDate = x.RentalListing.Property.CreatedOn,
                            PropertyType = x.RentalListing.Property.PropertyType.Name,
                            RentalPaymentType = x.RentalListing.Property.TargetRentType.Name
                        }
                    });

            var allItems = data.OrderBy(x => x.Model.Title).ToPagedList(model.Page, 2);
            allItems.ToList().ForEach(x => x.Model.MediaFiles.ToList().ForEach(y => y.InjectMediaModelViewProperties()));

            if (string.IsNullOrWhiteSpace(model.SortOrder))
            {
                model.SortOrder = "Latest Listing";
            }
            switch (model.SortOrder)
            {
                case "Title":
                    data = data.OrderBy(x => x.Model.Title);
                    break;
                case "Title_Desc":
                    data = data.OrderByDescending(x => x.Model.Title);
                    break;
                case "Highest Rent":
                    data = data.OrderByDescending(x => x.Model.TargetRent);
                    break;
                case "Lowest Rent":
                    data = data.OrderBy(x => x.Model.TargetRent);
                    break;
                case "Latest Avaible":
                    data = data.OrderByDescending(x => x.Model.AvailableDate);
                    break;
                case "Earliest Avaible":
                    data = data.OrderBy(x => x.Model.AvailableDate);
                    break;
                case "Latest Listing":
                    data = data.OrderByDescending(x => x.Property.CreatedDate);
                    break;
                case "Earliest Listing":
                    data = data.OrderBy(x => x.Property.CreatedDate);
                    break;
                default:
                    data = data.OrderByDescending(x => x.Property.CreatedDate);
                    break;
            }
            if (!String.IsNullOrWhiteSpace(model.SearchString))
            {
                SearchUtil searchTool = new SearchUtil();
                int searchType = searchTool.CheckDisplayType(model.SearchString);
                string formatString = searchTool.ConvertString(model.SearchString);
                data = data.Where(x => x.Model.Title.ToLower().Contains(formatString)
                                       );
            };

            var items = data.ToPagedList(model.Page, 10);
            items.ToList().ForEach(x => x.Model.MediaFiles.ToList().ForEach(y => y.InjectMediaModelViewProperties()));
            if (String.IsNullOrWhiteSpace(model.SearchString)) model.Page = 1;
            var sortOrders = new List<SortOrderModel>();
            var rvr = new RouteValueDictionary(new { SearchString = model.SearchString });
            sortOrders.Add(new SortOrderModel { SortOrder = "Title", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Title") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Title_Desc", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Title_Desc") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Highest Rent", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Highest Rent") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Lowest Rent", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Lowest Rent") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Latest Available", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Latest Available") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Earliest Available", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Earliest Available") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Latest Listing", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Latest Listing") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Earliest Listing", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Earliest Listing") });
            model.SortOrders = sortOrders;
            model.PagedInput = new PagedInput
            {
                ActionName = "Index",
                ControllerName = "Watchlist",
                PagedLinkValues = new RouteValueDictionary(new { SortOrder = model.SortOrder, SearchString = model.SearchString })
            };
            model.PageCount = items.Count == 0 ? allItems.PageCount : items.PageCount;
            model.SearchCount = items.Count;
            model.Items = items.Count == 0 ? allItems : items;

            return model;
        }

        public WatchlistDisplayModel getMarketJobWatchlist(WatchlistDisplayModel model, Login login)
        {
            var data = db.JobWatchList.Where(x => x.PersonId == login.Id && x.IsActive)
                    .Select(x => new WatctlistItem<JobMarketModel>
                    {
                        Model = new JobMarketModel
                        {
                            Id = x.TenantJobRequest.Id,
                            Title = x.TenantJobRequest.Title,
                            MaxBudget = x.TenantJobRequest.MaxBudget,
                            JobDescription = x.TenantJobRequest.JobDescription,
                            PostedDate = x.TenantJobRequest.CreatedOn,
                            MediaFiles = x.TenantJobRequest.TenantJobRequestMedia.Select(y => new MediaModel { Id = y.Id, NewFileName = y.NewFileName, OldFileName = y.OldFileName }).ToList()

                        },
                        Address = new AddressViewModel
                        {
                            AddressId = x.TenantJobRequest.Property.Address.AddressId,
                            CountryId = x.TenantJobRequest.Property.Address.AddressId,
                            Number = x.TenantJobRequest.Property.Address.Number,
                            Street = x.TenantJobRequest.Property.Address.Street,
                            Suburb = x.TenantJobRequest.Property.Address.Suburb,
                            Region = x.TenantJobRequest.Property.Address.Region,
                            City = x.TenantJobRequest.Property.Address.City,
                            PostCode = x.TenantJobRequest.Property.Address.PostCode
                        }
                    });

            var allItems = data.OrderBy(x => x.Model.Title).ToPagedList(model.Page, 2);
            allItems.ToList().ForEach(x => x.Model.MediaFiles.ToList().ForEach(y => y.InjectMediaModelViewProperties()));

            if (string.IsNullOrWhiteSpace(model.SortOrder))
            {
                model.SortOrder = "Title";
            }
            switch (model.SortOrder)
            {
                case "Title":
                    data = data.OrderBy(x => x.Model.Title);
                    break;
                case "Title_Desc":
                    data = data.OrderByDescending(x => x.Model.Title);
                    break;
                case "MaxBudget":
                    data = data.OrderBy(x => x.Model.MaxBudget);
                    break;
                case "MaxBudget_Desc":
                    data = data.OrderByDescending(x => x.Model.MaxBudget);
                    break;
                case "Date_Desc":
                    data = data.OrderByDescending(x => x.Model.PostedDate);
                    break;
                case "Date":
                    data = data.OrderBy(x => x.Model.PostedDate);
                    break;
                default:
                    data = data.OrderByDescending(x => x.Model.Title);
                    break;
            }
            if (!String.IsNullOrWhiteSpace(model.SearchString))
            {
                SearchUtil searchTool = new SearchUtil();
                int searchType = searchTool.CheckDisplayType(model.SearchString);
                string formatString = searchTool.ConvertString(model.SearchString);
                data = data.Where(x => x.Model.Title.ToLower().Contains(formatString)
                                       );
            };

            var items = data.ToPagedList(model.Page, 10);
            items.ToList().ForEach(x => x.Model.MediaFiles.ToList().ForEach(y => y.InjectMediaModelViewProperties()));
            if (String.IsNullOrWhiteSpace(model.SearchString)) model.Page = 1;
            var sortOrders = new List<SortOrderModel>();
            var rvr = new RouteValueDictionary(new { SearchString = model.SearchString });
            sortOrders.Add(new SortOrderModel { SortOrder = "Title", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Title") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Title_Desc", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Title_Desc") });
            sortOrders.Add(new SortOrderModel { SortOrder = "MaxBudget", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "MaxBudget") });
            sortOrders.Add(new SortOrderModel { SortOrder = "MaxBudget_Desc", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "MaxBudget_Desc") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Date_Desc", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Date_Desc") });
            sortOrders.Add(new SortOrderModel { SortOrder = "Date", ActionName = "Index", RouteValues = rvr.AddRouteValue("SortOrder", "Date") });
            model.SortOrders = sortOrders;
            model.PagedInput = new PagedInput
            {
                ActionName = "Index",
                ControllerName = "Watchlist",
                PagedLinkValues = new RouteValueDictionary(new { SortOrder = model.SortOrder, SearchString = model.SearchString })
            };
            model.PageCount = items.Count == 0 ? allItems.PageCount : items.PageCount;
            model.SearchCount = items.Count;
            model.Items = items.Count == 0 ? allItems : items;

            return model;
        }
        
        [HttpPost]
        public ActionResult SaveToWatchlist(SaveWatchlistModel model)
        {
            var user = AccountService.GetLoginByEmail(User.Identity.Name);
            using (var db = new KeysEntities())
            {
                if (model.ItemType == ItemType.RentalListing)
                {
                    var rentalWatchlist = new RentalWatchList
                    {
                        RentalListingId = model.Id,
                        PersonId = user.Id,
                        IsActive = true,
                        CreatedBy = user.UserName,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                    };
                    db.RentalWatchList.Add(rentalWatchlist);
                }
                if (model.ItemType == ItemType.MarketJob)
                {
                    var jobWatchlist = new JobWatchList
                    {
                        MarketJobId = model.Id,
                        PersonId = user.Id,
                        IsActive = true,
                        CreatedBy = user.UserName,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                    };
                    db.JobWatchList.Add(jobWatchlist);
                }
                try
                {
                    db.SaveChanges();
                    return Json(new { Success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false });
                }
            }
        }
    }
}