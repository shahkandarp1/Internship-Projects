using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HalloDoc.ViewModels;
using HalloDoc;

namespace HaloDocMVC.NET.Controllers
{
    public class PatientController : Controller
    {
        private readonly ILogger<PatientController> _logger;
        private readonly ApplicationDbContext _db;

        public PatientController(ILogger<PatientController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult PatientSite()
        {
            return View();
        }

        public IActionResult SubmitRequest()
        {
            return View();
        }

        public IActionResult PatientLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PatientLogin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.AspNetUsers.FirstOrDefault(u => u.UserName == model.Username);
                if (user != null)
                {
                    if (model.Password == user.PasswordHash)
                    {
                        return RedirectToAction("PatientDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Incorrect Password");
                    }
                }
                else
                {
                    ModelState.AddModelError("Username", "Incorrect Username");
                }
            }
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult PatientForm()
        {
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
            return View();
        }

        public IActionResult SubmitSomeoneElse()
        {
            return View();
        }

        public IActionResult SubmitForMe()
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