using HaloDocMVC.NET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HaloDocMVC.NET.Controllers
{
    public class PatientController : Controller
    {
        private readonly ILogger<PatientController> _logger;

        public PatientController(ILogger<PatientController> logger)
        {
            _logger = logger;
        }

        public IActionResult PatientSite()
        {
            return View();
        }

        public IActionResult SubmitRequest()
        {
            TempData["return_page"] = "Site";
            return View();
        }

        public IActionResult PatientLogin()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult PatientForm()
        {
            ViewData["redirect_page"] = TempData["return_page"];
            TempData.Keep("return_page");
            return View();
        }

        public IActionResult BusinessForm()
        {
            return View();
        }

        public IActionResult FamilyForm()
        {
            return View();
        }

        public IActionResult ConciergeForm()
        {
            return View();
        }

        public IActionResult PatientDashboard()
        {
            TempData["return_page"] = "Dashboard";
            System.Diagnostics.Debug.WriteLine(ViewData["redirect-request-page"] as string);
            return View();
        }

        public IActionResult SubmitSomeoneElse()
        {
            return View();
        }

        public IActionResult PatientProfile()
        {
            return View();
        }

        public IActionResult ViewDocument()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}