using ClosedXML.Excel;
using HalloDoc.Repository.Interface;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace HalloDoc.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdmin _admin;

        public AdminController(IAdmin admin)
        {
            _admin = admin;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("New",null,null,-1);
            return View(adminDashboardViewModel);
        }

        public async Task<IActionResult> New(string? search,string ?requestor,int? region)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("New", search, requestor, region);
            return PartialView("_AdminDashboardTable",adminDashboardViewModel);
        }

        public IActionResult Pending(string? search, string? requestor, int? region)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("Pending", search, requestor, region);
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }

        public IActionResult Active(string? search, string? requestor, int? region)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("Active", search, requestor, region);
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }

        public IActionResult Conclude(string? search, string? requestor, int? region)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("Conclude", search, requestor, region);
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }

        public IActionResult Close(string? search, string? requestor, int? region)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("ToClose", search, requestor, region);
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }

        public IActionResult Unpaid(string? search, string? requestor, int? region)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("Unpaid", search, requestor, region);
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }

        public IActionResult ExportAll()
        {
            MemoryStream memoryStream = _admin.exportAll();
             return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "All Data.xlsx");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Dashboard(AdminDashboardViewModel model)
        {
            MemoryStream memoryStream = _admin.export(model);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Data-{model.status}.xlsx");
        }

        public IActionResult ViewCase(int id)
        {
            
            ViewCaseViewModel viewCaseViewModel= _admin.viewCase(id);
            return View(viewCaseViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewCase(ViewCaseViewModel model)
        {
            bool status = _admin.viewCase(model);
            if(status)
            {
                TempData["success"] = "Data Editted Successfully!!";
            }
            else
            {
                TempData["error"] = "Data could not be editted!!";
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelRequest(ViewCaseViewModel viewCaseViewModel)
        {
            bool isUpdated = _admin.cancelRequest(viewCaseViewModel.RequestId,viewCaseViewModel.Admin_notes,viewCaseViewModel.CaseTag);
            if (isUpdated)
            {
                TempData["success"] = "Request Cancelled Successfully!!";
            }
            else
            {
                TempData["error"] = "Request could not be Cancelled!!";
            }
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("New", null, null, -1);
            return View("Dashboard",adminDashboardViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendLink(AdminDashboardViewModel dashboardViewModel)
        {
            bool isSent = _admin.sendLink(dashboardViewModel);
            if (isSent)
            {
                TempData["success"] = "Link Sent Successfully!!";
            }
            else
            {
                TempData["error"] = "Link could not be Sent!!";
            }
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("New", null, null, -1);
            return View("Dashboard", adminDashboardViewModel);
        }

        public IActionResult CreateRequest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRequest(PatientRequestViewModel? patientRequestViewModel)
        {
            if(ModelState.IsValid)
            {
                bool isVerified = _admin.verifyRegion(patientRequestViewModel.State);
                if(!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(patientRequestViewModel);
                }

                bool isBlocked = _admin.verifyBlock(patientRequestViewModel.Email);
                if (isBlocked)
                {
                    TempData["error"] = "Patient with this email is blocked!!!";
                    return View(patientRequestViewModel);
                }

                bool requestCreated = _admin.createRequest(patientRequestViewModel);
                if(requestCreated)
                {
                    TempData["success"] = "Request created Successfully!!";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    TempData["error"] = "Request Could not be created!!";
                    return View(patientRequestViewModel);
                }
            }
            return View(patientRequestViewModel);
        }

        [HttpPost]
        public IActionResult VerifyRegion(string region)
        {
            if(region == null)
            {
                return Json(new { isVerified=2 });
            }
            bool isVerified = _admin.verifyRegion(region);
            if (isVerified)
            {
                return Json(new { isVerified = 1 });
            }
            else
            {
                return Json(new { isVerified = 3 });
            }
        }

        public IActionResult ViewNotes(int id)
        {
            ViewNotesViewModel viewNotesViewModel = _admin.viewNotes(id);
            return View(viewNotesViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewNotes(ViewNotesViewModel viewNotesViewModel)
        {
            bool isUpdated = _admin.updateAdminNotes(viewNotesViewModel);
            if(isUpdated)
            {
                TempData["success"] = "Admin Note Updated Successfully!!";
            }
            else
            {
                TempData["error"] = "Admin Note Could not be updated!!";
            }
            ViewNotesViewModel viewNoteViewModel = _admin.viewNotes(viewNotesViewModel.RequestId);
            return View(viewNoteViewModel);
        }
    }
}
