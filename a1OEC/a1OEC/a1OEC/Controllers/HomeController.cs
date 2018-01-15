using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using a1OEC.Models;
using Microsoft.AspNetCore.Http; // required for GetString() & SetString()

namespace a1OEC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // session
            HttpContext.Session.SetString("SessionFirstName", "Hyerim");

            // cookie
            Response.Cookies.Append("CookiesFirstName", "Hyerim");

            // tempdata
            TempData["message"] = "Hyerim's website !";

            // set viewbag to session name
            ViewBag.SessionFirstName = HttpContext.Session.GetString("SessionFirstName");

            // set viewbag to cookie name
            ViewBag.CookiesFirstName = Request.Cookies["CookiesFirstName"];

            //  set viewbag to tempdata name
            ViewBag.TempDataFirstName = TempData["message"];


            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
