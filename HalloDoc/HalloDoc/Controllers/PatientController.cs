using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HalloDoc.ViewModels;
using HalloDoc;
using System.Collections;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.IO.Compression;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting.Server;
using System.Net.Mail;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Identity;
using HalloDoc.Repository.Interface;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using DocumentFormat.OpenXml.Drawing;

namespace HaloDocMVC.NET.Controllers
{
    public class PatientController : Controller
    {
        private readonly ILogger<PatientController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _context;
        private readonly IPatient _patient;
        private readonly IAdmin _admin;

        public PatientController(ILogger<PatientController> logger, ApplicationDbContext db,IHttpContextAccessor context,IPatient patient,IAdmin admin)
        {
            _logger = logger;
            _db = db;
            _context = context;
            _patient = patient;
            _admin = admin;
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
                var result = _patient.login(model);
                if(result == 1)
                {
                    ModelState.AddModelError("Password", "You are not having rights of patient site");
                    return View();
                }
                else if(result == 2)
                {
                    TempData["success"] = "Loged in Successfully!!";
                    return RedirectToAction("PatientDashboard");
                }
                else if(result == 3)
                {
                    ModelState.AddModelError("Password", "Incorrect Password");
                }
                else if(result == 4)
                {
                    ModelState.AddModelError("Username", "Incorrect Username");
                }
                else
                {
                    TempData["error"] = "There was some issue in Log in!!";
                }
            }
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ResetPassword(string Token)
        {
            PasswordReset passwordReset = _patient.getResetPassword(Token);
            if(passwordReset == null)
            {
                return NotFound();
            }
            if(passwordReset.IsUpdated == true)
            {
                return NotFound();
            }
            TimeSpan difference = DateTime.Now.Subtract(passwordReset.CreatedDate);
            double hours = difference.TotalHours;
            if (hours > 24)
            {
                return NotFound();
            }
            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel();
            resetPasswordViewModel.Token = Token;
            return View(resetPasswordViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel modal)
        {
            if(ModelState.IsValid)
            {
                bool isReseted = _patient.resetPassword(modal);
                if (isReseted)
                {
                    ViewData["Message"] = "Password Reseted successfully!!!";
                }
            }
            return View(modal);
        }

        public IActionResult EmailCheck(string email)
        {
            bool isSent = _patient.sendResetLink(email);
            return Json(new { isValid = isSent });
        }

        [HttpGet]
        public IActionResult PatientForm()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PatientForm(PatientRequestViewModel modal)
        {
            if (ModelState.IsValid)
            {
                bool isVerified = _admin.verifyRegion(modal.State);
                if (!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(modal);
                }

                bool isBlocked = _admin.verifyBlock(modal.Email);
                if (isBlocked)
                {
                    TempData["error"] = "Patient with this email is blocked!!!";
                    return View(modal);
                }

                var isCreated = _patient.patientRequest(modal);
                if(isCreated.Result)
                {
                    TempData["success"] = "Request Created Successfully!!!";
                    return RedirectToAction("PatientSite");
                }
                else
                {
                    TempData["error"] = "Request could not be Created!!!";
                }
            }
            return View(modal);
        }

        public IActionResult PatientCheck(string email)
        {
            if(email == null)
            {
                return View();
            }
            var existingUser = _patient.getAspNetUser(email);
            bool isValidEmail;
            if (existingUser == null)
            {
                isValidEmail = false;
            }
            else
            {
                isValidEmail = true;
            }
            return Json(new { isValid = isValidEmail });
        }

        public IActionResult Logout()
        {

            _context.HttpContext.Session.Clear();
            return Json(new { isLogout = true});
        }

        public IActionResult BusinessForm()
        {
            return View();
        }

        public IActionResult FamilyForm()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FamilyForm(FamilyRequestViewModel modal)
        {
            if (ModelState.IsValid)
            {
                bool isVerified = _admin.verifyRegion(modal.State);
                if (!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(modal);
                }

                bool isBlocked = _admin.verifyBlock(modal.Email);
                if (isBlocked)
                {
                    TempData["error"] = "Patient with this email is blocked!!!";
                    return View(modal);
                }

                var isCreated = _patient.familyRequest(modal);
                if (isCreated.Result)
                {
                    TempData["success"] = "Request Created Successfully!!!";
                    return RedirectToAction("PatientSite");
                }
                else
                {
                    TempData["error"] = "Request could not be Created!!!";
                }
            }
            return View(modal);
        }


        public IActionResult Register(int id)
        {
            RegisterViewModel modal = new RegisterViewModel();
            AspNetUser aspNetUser = _patient.getAspNetUserById(id);
            modal.Id = id;
            modal.Email = aspNetUser.Email;
            return View(modal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel modal)
        {
            if (ModelState.IsValid)
            {
                var isRegistered = _patient.register(modal);
                if(isRegistered)
                {
                    ViewData["Message"] = "Registered successfully!!!";
                }
                return View();
            }
            return View(modal);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ConciergeForm(ConciergeRequestViewModel modal)
        {
            if (ModelState.IsValid)
            {
                bool isVerified = _admin.verifyRegion(modal.ConciergeState);
                if (!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(modal);
                }

                bool isBlocked = _admin.verifyBlock(modal.Email);
                if (isBlocked)
                {
                    TempData["error"] = "Patient with this email is blocked!!!";
                    return View(modal);
                }

                var isCreated = _patient.conciergeRequest(modal);
                if (isCreated.Result)
                {
                    TempData["success"] = "Request Created Successfully!!!";
                    return RedirectToAction("PatientSite");
                }
                else
                {
                    TempData["error"] = "Request could not be Created!!!";
                }
            }
            return View(modal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> BusinessForm(BusinessRequestViewModel modal)
        {
            if(ModelState.IsValid)
            {
                bool isVerified = _admin.verifyRegion(modal.State);
                if (!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(modal);
                }

                bool isBlocked = _admin.verifyBlock(modal.Email);
                if (isBlocked)
                {
                    TempData["error"] = "Patient with this email is blocked!!!";
                    return View(modal);
                }

                var isCreated = _patient.businessRequest(modal);
                if (isCreated.Result)
                {
                    TempData["success"] = "Request Created Successfully!!!";
                    return RedirectToAction("PatientSite");
                }
                else
                {
                    TempData["error"] = "Request could not be Created!!!";
                }
            }

            return View(modal);
        }

        public IActionResult ConciergeForm()
        {
            return View();
        }

        public async Task<IActionResult> PatientDashboard()
        {
            DashboardViewModel dashboardViewModel = _patient.getDashboardData();
            return View(dashboardViewModel);
        }

        public IActionResult SubmitSomeoneElse()
        {

            FamilyRequestViewModel familyRequestViewModel = _patient.getFamilyRequest();
            return View(familyRequestViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitSomeoneElse(FamilyRequestViewModel modal)
        {
            if(ModelState.IsValid)
            {
                bool isVerified = _admin.verifyRegion(modal.State);
                if (!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(modal);
                }

                bool isBlocked = _admin.verifyBlock(modal.Email);
                if (isBlocked)
                {
                    TempData["error"] = "Patient with this email is blocked!!!";
                    return View(modal);
                }

                var isCreated = _patient.someoneElseRequest(modal);
                if (isCreated.Result)
                {
                    TempData["success"] = "Request Created Successfully!!!";
                    return RedirectToAction("PatientDashboard");
                }
                else
                {
                    TempData["error"] = "Request could not be Created!!!";
                }
            }
            
            return View(modal);
        }

        public IActionResult SubmitForMe()
        {
            PatientRequestViewModel patientRequestViewModel = _patient.getPatientRequest();
            return View(patientRequestViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitForMe(PatientRequestViewModel modal)
        {
            if (ModelState.IsValid)
            {
                bool isVerified = _admin.verifyRegion(modal.State);
                if (!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(modal);
                }

                bool isBlocked = _admin.verifyBlock(modal.Email);
                if (isBlocked)
                {
                    TempData["error"] = "Patient with this email is blocked!!!";
                    return View(modal);
                }

                var isCreated = _patient.selfRequest(modal);
                if (isCreated.Result)
                {
                    TempData["success"] = "Request Created Successfully!!!";
                    return RedirectToAction("PatientDashboard");
                }
                else
                {
                    TempData["error"] = "Request could not be Created!!!";
                }
            }
            return View(modal);
        }

        public IActionResult PatientProfile()
        {
            PatientRequestViewModel patientRequestViewModel = _patient.getPatientProfile();
            return View(patientRequestViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PatientProfile(PatientRequestViewModel modal)
        {
            if (ModelState.IsValid)
            {
                bool isVerified = _admin.verifyRegion(modal.State);
                if (!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(modal);
                }

                int isUpdated = _patient.updatePatientProfile(modal);
                if(isUpdated == 1)
                {
                    TempData["success"] = "Information updated successfully!!!";
                    return RedirectToAction("PatientDashboard");
                }
                else if(isUpdated == 2)
                {
                    TempData["error"] = "This email already exists!!!";
                    return View(modal);
                }
                else
                {
                    TempData["error"] = "Information could not be updated!!!";
                }

                return RedirectToAction("PatientDashboard");

            }
            return View(modal);
        }

        [HttpGet]
        public IActionResult ViewDocument(int id)
        {
            ViewDocumentModal viewDocumentModal = _patient.getViewDocument(id);
            return View(viewDocumentModal);
        }

        [HttpPost]
        public async Task<IActionResult> FileUpload([FromForm]IFormFile file, [FromForm] int id)
        {
            Task<bool> isFileUploaded = _patient.fileUpload(file, id);
            return Json(new { isFileUploaded = isFileUploaded });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewDocument(ViewDocumentModal modal)
        {
            var result = _admin.downloadMultipleFiles(modal);
            await result;
            Response.ContentType = "application/zip";
            Response.Headers.Add("Content-Disposition", $"attachment; filename={result.Result.Item2}");
            return File(result.Result.Item1.ToArray(), "application/zip", result.Result.Item2);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}