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
using HalloDoc.Repository.Auth;

namespace HaloDocMVC.NET.Controllers
{
    [CustomAuthorize("Patient")]

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


        

        public async Task<IActionResult> PatientDashboard()
        {
            DashboardViewModel dashboardViewModel = _patient.getDashboardData(1,10);
            return View(dashboardViewModel);
        }

        public async Task<IActionResult> PatientDashboardTable(int page=1,int pageSize=10)
        {
            DashboardViewModel dashboardViewModel = _patient.getDashboardData(page,pageSize);
            return PartialView("_PatientDashboard",dashboardViewModel);
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