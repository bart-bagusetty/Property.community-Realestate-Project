using KeysPlus.Data;
using KeysPlus.Website.Areas.Companies.Models;
using KeysPlus.Website.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Runtime.Remoting.Channels;
using KeysPlus.Website.Areas.Tools;
using KeysPlus.Service.Models;
using KeysPlus.Service.Services;
//using CompanyViewModel = KeysPlus.Service.Models.CompanyViewModel;

namespace KeysPlus.Website.Areas.Companies.Controllers
{
    [Authorize]
    public class OnboardingController : Controller
    {
        // GET: Companies/Onboarding
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewServiceProvider (CompanyViewModel model)
        {

            return View();
        }
        
    }
}