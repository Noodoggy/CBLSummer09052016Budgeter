using CBLSummer09052016Budgeter.Models;
using CBLSummer09052016Budgeter.Models.CodeFirst.Extensions;
using CBLSummer09052016Budgeter.Models.CodeFirst.Helpers;
using Microsoft.AspNet.Identity;
using MvcBreadCrumbs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CBLSummer09052016Budgeter.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewIndex()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }



    }
}

