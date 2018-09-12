using BartPortfolio.EMailing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BartPortfolio.Controllers
{
    public class homeController : Controller
    {
        // GET: home
        public ActionResult Index()
        {
            ViewBag.Msg = false;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string name, string email, string subject, string message)
        {
            EmailClass Obj = new EmailClass();
            Obj.SendContact(name, email, subject, message);
            ViewBag.Msg = true;
            return View();
        }

        public ActionResult dompiggame()
        {
            return View();
        }

        public ActionResult budgety()
        {
            return View();
        }
    }
}