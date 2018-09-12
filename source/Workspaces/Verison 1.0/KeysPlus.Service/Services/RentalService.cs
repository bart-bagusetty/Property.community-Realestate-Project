using KeysPlus.Data;
using KeysPlus.Service.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KeysPlus.Service.Services
{
    public static class RentalService
    {
        static string _error = "Something went wrong please try again later!";
        public static IEnumerable<RentalListing> GetRentalProperties(string searchString, string sortOrder)
        {
            using (var db = new KeysEntities())
            {
                return db.RentalListing.Include("Property").Include("RentalListingMedia").Include("Property.Address").Include("Property.PropertyType").Include("Property.TargetRentType").Include("Property.PropertyMedia").Where(x => x.IsActive == true).ToList();
            }
        }
        public static SearchResult GetAllRentalProperties(RentalListingSearchModel model)
        {
            var result = new SearchResult { SearchCount = 0 };
            return result;
        }

        public static ServiceResponseResult AddTenantJobRequest(MarketJobModel model, Login login, HttpFileCollectionBase files = null)
        {
            using (var db = new KeysEntities())
            {
                try
                {
                    var request = db.PropertyRequest.Where(x => x.Id == model.RequestId).FirstOrDefault(); 
                    if (request == null) return new ServiceResponseResult{ IsSuccess = false, ErrorMessage = "No record found!" };
                    request.RequestStatusId = (int)JobRequestStatus.Accepted;
                    request.IsUpdated = true;
                    //var tenant = TenantService.GetTenantByLoginId(login.Id);
                    var jobRequest = new TenantJobRequest
                    {
                        PropertyId = model.PropertyId,
                        OwnerId = login.Id,
                        JobDescription = model.JobDescription,
                        JobStatusId = 1,  //Bug #2031 (Part 3)
                        MaxBudget = model.MaxBudget,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = login.Email,
                        UpdatedOn = DateTime.UtcNow,
                        UpdatedBy = login.Email,
                        Title = model.Title
                    };
                    db.TenantJobRequest.Add(jobRequest);
                    //foreach (int fileId in model.FilesCopy)
                    //{
                    //    var propertyRequestMedia1 = db.PropertyRequestMedia.Where(x => x.Id == fileId).FirstOrDefault();
                    //    var newMarketJob = new TenantJobRequestMedia();

                    //    newMarketJob.TenantJobRequestId = jobRequest.Id;
                    //    newMarketJob.NewFileName = propertyRequestMedia1.NewFileName;
                    //    newMarketJob.OldFileName = propertyRequestMedia1.OldFileName;
                    //    newMarketJob.EntityGuid = new Guid();
                    //    db.TenantJobRequestMedia.Add(newMarketJob);
                    //}
                    var mediaFiles = MediaService.SaveFiles(files, 5, AllowedFileType.Images).NewObject as List<MediaModel>;
                    mediaFiles.ForEach(x => jobRequest.TenantJobRequestMedia.Add(new TenantJobRequestMedia
                    {
                        NewFileName = x.NewFileName,
                        OldFileName = x.OldFileName,
                    }));
                    db.SaveChanges();
                    return new ServiceResponseResult { IsSuccess = true };
                                                
                }
                catch (Exception ex)
                {
                    return new ServiceResponseResult { IsSuccess = false, ErrorMessage = _error };
                }
            }
        }

        public static ServiceResponseResult AddRentallApllication(RentalApplicationModel model, Login login, HttpFileCollectionBase files = null)
        {
            return new ServiceResponseResult { IsSuccess = true };
        }
        
        public static ServiceResponseResult EditRentallApllication(RentalApplicationModel model, Login login, HttpFileCollectionBase files = null)
        {
            using (var db = new KeysEntities())
            {
                var result = new ServiceResponseResult { IsSuccess = false };
                if (!TenantService.IsLoginATenant(login))
                {
                    var errorMsg = "Account not tenant!";
                    result.ErrorMessage = errorMsg;
                    return result;
                }
                var foundRentalApplication = db.RentalApplication.Where(x => x.Id == model.Id).FirstOrDefault();
                if (foundRentalApplication == null)
                {
                    var errorMsg = "Cannot locate the Rental application";
                    result.ErrorMessage = errorMsg;
                    return result;
                }
                else
                {
                    foundRentalApplication.RentalListingId = model.RentalListingId;
                    foundRentalApplication.PersonId = login.Id;
                    foundRentalApplication.TenantsCount = model.TenantsCount;
                    foundRentalApplication.Note = model.Note;
                    foundRentalApplication.ApplicationStatusId = 1;
                    foundRentalApplication.CreatedBy = login.Email;
                    foundRentalApplication.CreatedOn = DateTime.UtcNow;
                    foundRentalApplication.UpdatedBy = login.Email;
                    foundRentalApplication.UpdatedOn = DateTime.UtcNow;
                    foundRentalApplication.IsActive = true;
                    
                    model.FilesRemoved.ForEach( x => {
                        var media = db.RentalApplicationMedia.FirstOrDefault( y => y.Id == x);
                        if (media != null) { db.RentalApplicationMedia.Remove(media); MediaService.RemoveMediaFile(media.NewFileName); }
                    });
                    var mediaFiles = MediaService.SaveFiles(files, 5-foundRentalApplication.RentalApplicationMedia.Count, AllowedFileType.AllFiles).NewObject as List<MediaModel>;
                    if (mediaFiles != null)
                    {
                        mediaFiles.ForEach(x => foundRentalApplication.RentalApplicationMedia.Add(new RentalApplicationMedia
                        {
                            NewFileName = x.NewFileName,
                            OldFileName = x.OldFileName,
                        }));
                    }
                };
                try
                {

                    db.SaveChanges();
                    var mFiles = new List<MediaModel>();
                    var medias = foundRentalApplication.RentalApplicationMedia
                                .Select(x => MediaService.GenerateViewProperties(new MediaModel { Id = x.Id, NewFileName = x.NewFileName, OldFileName = x.OldFileName })).ToList();
                    return new ServiceResponseResult { IsSuccess = true , NewObject = mFiles};

                }
                catch (Exception ex)
                {
                    return new ServiceResponseResult { IsSuccess = false, ErrorMessage = _error };
                }
            }
        }

        public static ServiceResponseResult DeleteRentallApllication(int rentalApplicationId)
        {
            var result = new ServiceResponseResult { IsSuccess = false };
            using (var db = new KeysEntities())
            {
                var deleteRentallApllication = db.RentalApplication.Where(x => x.Id == rentalApplicationId).First();
                if (deleteRentallApllication == null)
                {
                    var errorMsg = "Cannot locate the Rental application";
                    result.ErrorMessage = errorMsg;
                    return result;
                }
                else
                {
                    deleteRentallApllication.IsActive = false;
                };
                try
                {
                    db.SaveChanges();
                    return new ServiceResponseResult { IsSuccess = true };
                }
                catch (Exception ex)
                {
                    return new ServiceResponseResult { IsSuccess = false, ErrorMessage = _error };
                }

            }
        }

        public static  Person GetOwnerDetails(RentalApplicationModel model)
        {
            using (var db = new KeysEntities())
            {
                var ownerId = db.RentalListing.Where(x => x.Id == model.RentalListingId).Select(y => y.Property.OwnerProperty.FirstOrDefault().Person.Id).FirstOrDefault();
                return db.Person.FirstOrDefault(x => x.Id == ownerId);
            }
           
        }
        
        public static IEnumerable<RequestType> GetRequestTypes()
        {
            using (var db = new KeysEntities())
            {
                return db.RequestType.ToList();
            }
        }

        public static RentalApplication GetRentalApplicationById(int id)
        {
            using (var db = new KeysEntities())
            {
                return db.RentalApplication.FirstOrDefault(x => x.Id == id);
            }
        }

        public static ServiceResponseResult UpdateApplicationView(int appId)
        {
            using (var db = new KeysEntities())
            {

                var foundApplication = db.RentalApplication.Where(x => x.Id == appId).FirstOrDefault();
                if (foundApplication == null) return new ServiceResponseResult { IsSuccess = false, ErrorMessage = "Item not found!" };
                if (foundApplication.IsViewedByOwner == true) { return new ServiceResponseResult { IsSuccess = false, ErrorMessage = "Already Viewed" }; }
                if (foundApplication.IsViewedByOwner ?? false) return new ServiceResponseResult { IsSuccess = false };
                foundApplication.IsViewedByOwner = true;
                try
                {
                    db.SaveChanges();
                    return new ServiceResponseResult { IsSuccess = true };
                }
                catch (Exception ex)
                {
                    return new ServiceResponseResult { IsSuccess = false, ErrorMessage = _error };
                }
            }
        }
        public static ServiceResponseResult<List<MediaModel>> EditRentalListing(RentListingModel model, HttpFileCollectionBase files, Login user)
        {
            using (var db = new KeysEntities())
            {
                var foundProp = db.OwnerProperty.FirstOrDefault(x => x.PropertyId == model.PropertyId && x.OwnerId == user.Id);
                if (foundProp == null)
                {
                    return new ServiceResponseResult<List<MediaModel>> { IsSuccess = false, ErrorMessage = "Can not find property." };
                }
                var oldRentalListing = db.RentalListing.Find(model.Id);
                oldRentalListing.Description = model.Description;
                oldRentalListing.Title = model.Title;
                oldRentalListing.MovingCost = model.MovingCost;
                oldRentalListing.TargetRent = model.TargetRent;
                oldRentalListing.AvailableDate = model.AvailableDate;
                oldRentalListing.Furnishing = model.Furnishing;
                oldRentalListing.IdealTenant = model.IdealTenant;
                oldRentalListing.OccupantCount = model.OccupantCount;
                oldRentalListing.PetsAllowed = model.PetsAllowed;
                oldRentalListing.UpdatedBy = user.Email;
                oldRentalListing.UpdatedOn = DateTime.UtcNow;
                if (model.FilesRemoved != null)
                {
                    model.FilesRemoved.ToList().ForEach(x =>
                     {
                         var media = oldRentalListing.RentalListingMedia.FirstOrDefault(y => y.Id == x);
                         if (media != null)
                         {
                             db.RentalListingMedia.Remove(media);
                             MediaService.RemoveMediaFile(media.NewFileName);
                         }
                     });
                }


                var fileList = MediaService.SaveMediaFiles(files, 5, null).NewObject as List<MediaModel>;
                if (fileList != null)
                {
                    fileList.ForEach(x => oldRentalListing.RentalListingMedia.Add(new RentalListingMedia { NewFileName = x.NewFileName, OldFileName = x.OldFileName }));
                }

                try
                {
                    db.SaveChanges();
                    var medias = oldRentalListing.RentalListingMedia
                                .Select(x => MediaService.GenerateViewProperties( new MediaModel { Id = x.Id, NewFileName = x.NewFileName, OldFileName = x.OldFileName})).ToList();
                    return new ServiceResponseResult<List<MediaModel>> { IsSuccess = true, Result = medias };
                }
                catch (Exception ex)
                {
                    return new ServiceResponseResult<List<MediaModel>> { IsSuccess = true, ErrorMessage = _error };
                }
            }
        }

        public static ServiceResponseResult AddRentalListing(RentListingModel model, HttpFileCollectionBase files, Login user)
        {
            using (var db = new KeysEntities())
            {
                var foundProp = db.OwnerProperty.FirstOrDefault(x => x.PropertyId == model.PropertyId && x.OwnerId == user.Id);
                if (foundProp == null)
                {
                    return new ServiceResponseResult { IsSuccess = false, ErrorMessage = "Can not find property." };
                }
                var newRentalListing = new RentalListing
                {
                    PropertyId = model.PropertyId,
                    Description = model.Description,
                    Title = model.Title = model.Title,
                    MovingCost = model.MovingCost,
                    TargetRent = model.TargetRent,
                    AvailableDate = model.AvailableDate,
                    Furnishing = model.Furnishing,
                    IdealTenant = model.IdealTenant,
                    OccupantCount = model.OccupantCount,
                    PetsAllowed = model.PetsAllowed,
                    CreatedBy = user.Email,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedBy = user.Email,
                    IsActive = true,
                    RentalStatusId = 1
                };
                var fileList = MediaService.SaveMediaFiles(files, 5, null).NewObject as List<MediaModel>;
                if (fileList != null)
                {
                    fileList.ForEach(x => newRentalListing.RentalListingMedia.Add(new RentalListingMedia { NewFileName = x.NewFileName, OldFileName = x.OldFileName }));
                }
                db.RentalListing.Add(newRentalListing);
                try
                {
                    db.SaveChanges();
                    return new ServiceResponseResult { IsSuccess = true };
                }
                catch (Exception ex)
                {
                    return new ServiceResponseResult { IsSuccess = true, ErrorMessage = _error };
                }
            }

        }
        
        
    }
}
