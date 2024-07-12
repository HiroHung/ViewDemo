using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ViewDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //ViewData["Name"] = "5566得第一";
            //ViewBag.Name = "F4得第二";

            //TempData["Name"] = "EG得第三";
            return View();
            //return RedirectToAction("About");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}