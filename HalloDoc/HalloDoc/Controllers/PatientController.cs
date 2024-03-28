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
        private readonly IHttpContextAccessor _context;
        private readonly IPatient _patient;
        private readonly IAdmin _admin;

        public PatientController(ILogger<PatientController> logger,IHttpContextAccessor context,IPatient patient,IAdmin admin)
        {
            _logger = logger;
            _context = context;
            _patient = patient;
            _admin = admin;
        }
        /// <summary>
        /// It is Get Method for Patient Dashboard Page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> PatientDashboard()
        {
            DashboardViewModel dashboardViewModel = _patient.GetDashboardData(1,10);
            return View(dashboardViewModel);
        }
        /// <summary>
        /// It will return filtered and paginated data for Patient Dashboard Page as Partial View
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IActionResult> PatientDashboardTable(int page=1,int pageSize=10)
        {
            DashboardViewModel dashboardViewModel = _patient.GetDashboardData(page,pageSize);
            return PartialView("_PatientDashboard",dashboardViewModel);
        }
        /// <summary>
        /// It is Get method for Submit Request for Someone else Page
        /// </summary>
        /// <returns></returns>
        public IActionResult SubmitSomeoneElse()
        {

            FamilyRequestViewModel familyRequestViewModel = _patient.GetFamilyRequest();
            return View(familyRequestViewModel);
        }
        /// <summary>
        /// It is post method for Submit Request for Someone else Page
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitSomeoneElse(FamilyRequestViewModel modal)
        {
            if(ModelState.IsValid)
            {
                bool isVerified = _admin.VerifyRegion(modal.State);
                if (!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(modal);
                }

                bool isBlocked = _admin.VerifyBlock(modal.Email);
                if (isBlocked)
                {
                    TempData["error"] = "Patient with this email is blocked!!!";
                    return View(modal);
                }

                var isCreated = _patient.SomeoneElseRequest(modal);
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
        /// <summary>
        /// It is Get method for Submit Request for Me Page
        /// </summary>
        /// <returns></returns>
        public IActionResult SubmitForMe()
        {
            PatientRequestViewModel patientRequestViewModel = _patient.GetPatientRequest();
            return View(patientRequestViewModel);
        }
        /// <summary>
        /// It is Post method for Submit for Me Page
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitForMe(PatientRequestViewModel modal)
        {
            if (ModelState.IsValid)
            {
                bool isVerified = _admin.VerifyRegion(modal.State);
                if (!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(modal);
                }

                bool isBlocked = _admin.VerifyBlock(modal.Email);
                if (isBlocked)
                {
                    TempData["error"] = "Patient with this email is blocked!!!";
                    return View(modal);
                }

                var isCreated = _patient.SelfRequest(modal);
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
        /// <summary>
        /// It is Get method for Patient Profile Page
        /// </summary>
        /// <returns></returns>
        public IActionResult PatientProfile()
        {
            PatientRequestViewModel patientRequestViewModel = _patient.GetPatientProfile();
            if(patientRequestViewModel == null)
            {
                return NotFound();
            }
            return View(patientRequestViewModel);
        }
        /// <summary>
        /// It is Post method for Patient Profile Page
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PatientProfile(PatientRequestViewModel modal)
        {
            if (ModelState.IsValid)
            {
                bool isVerified = _admin.VerifyRegion(modal.State);
                if (!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(modal);
                }

                int isUpdated = _patient.UpdatePatientProfile(modal);
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
                else if(isUpdated == 4)
                {
                    return NotFound();
                }
                else
                {
                    TempData["error"] = "Information could not be updated!!!";
                }

                return RedirectToAction("PatientDashboard");

            }
            return View(modal);
        }
        /// <summary>
        /// It is Get method for View Document Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ViewDocument(int id)
        {
            ViewDocumentModal viewDocumentModal = _patient.GetViewDocument(id);
            if(viewDocumentModal == null)
            {
                return NotFound();
            }
            return View(viewDocumentModal);
        }
        /// <summary>
        /// It will upload file for specified request
        /// </summary>
        /// <param name="file"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FileUpload([FromForm]IFormFile file, [FromForm] int id)
        {
            Task<bool> isFileUploaded = _patient.FileUpload(file, id);
            return Json(new { isFileUploaded = isFileUploaded });
        }
        /// <summary>
        /// It will download all the files selected in the form of zip file
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewDocument(ViewDocumentModal modal)
        {
            var result = _admin.DownloadMultipleFiles(modal);
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