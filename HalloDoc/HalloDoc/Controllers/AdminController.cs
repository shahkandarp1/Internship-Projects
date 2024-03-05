using ClosedXML.Excel;
using HalloDoc.Repository.Interface;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Presentation;
using System.Collections;
using Irony.Parsing;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel loginViewModel)
        {
            if(ModelState.IsValid)
            {
                int result = _admin.login(loginViewModel);
                if(result == 1)
                {
                    TempData["error"] = "Username is incorrect!!";
                }
                else if(result == 2)
                {
                    TempData["error"] = "Password is incorrect!!";
                }
                else if(result == 3)
                {
                    TempData["error"] = "You dont have rights to login into this website!!";
                }
                else if (result == 5)
                {
                    TempData["error"] = "There was some issue in Login!!";
                }
                else
                {
                    TempData["success"] = "Loged In Successfully!!";
                    return RedirectToAction("Dashboard");
                }
            }
            return View(loginViewModel);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if(ModelState.IsValid)
            {
                int result = _admin.forgotPassword(forgotPasswordViewModel);
                if(result == 1)
                {
                    TempData["error"] = "This email does not exists!!";
                }
                else if(result == 2)
                {
                    TempData["error"] = "Email could not be sent!!";
                }
                else
                {
                    TempData["success"] = "Email sent successfully!!";
                }
            }
            return View(forgotPasswordViewModel);
        }

        public IActionResult Logout()
        {

            bool isLogout = _admin.logout();
            return Json(new { isLogout = isLogout });
        }

        public IActionResult Dashboard()
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("New",null,null,-1);
            return View(adminDashboardViewModel);
        }

        public IActionResult New(string? search,string ?requestor,int? region,int page=1,int pageSize = 10)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("New", search, requestor, region,page,pageSize);
            return PartialView("_AdminDashboardTable",adminDashboardViewModel);
        }

        public IActionResult Pending(string? search, string? requestor, int? region, int page = 1, int pageSize = 10)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("Pending", search, requestor, region,page,pageSize);
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }

        public IActionResult Active(string? search, string? requestor, int? region, int page = 1, int pageSize = 10)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("Active", search, requestor, region, page, pageSize);
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }

        public IActionResult Conclude(string? search, string? requestor, int? region, int page = 1, int pageSize = 10)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("Conclude", search, requestor, region, page, pageSize);
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }

        public IActionResult Close(string? search, string? requestor, int? region, int page = 1, int pageSize = 10)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("ToClose", search, requestor, region,page,pageSize);
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }

        public IActionResult Unpaid(string? search, string? requestor, int? region, int page = 1, int pageSize = 10)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("Unpaid", search, requestor, region, page, pageSize);
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
            PatientRequestViewModel patientRequestViewModel = _admin.createRequest();
            return View(patientRequestViewModel);
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

        public IActionResult ViewUploads(int id)
        {
            ViewDocumentModal viewDocumentModal = _admin.viewUploads(id);
            return View(viewDocumentModal);
        }

        [HttpPost]
        public IActionResult FileUpload([FromForm] IFormFile file, [FromForm] int id)
        {
            Task<bool> isFileUploaded = _admin.fileUpload(file,id);
            return Json(new { isFileUploaded = isFileUploaded });
        }

        public IActionResult DeleteSingle(int id)
        {
            int requestid = _admin.deleteSingleFile(id);
            ViewDocumentModal viewDocumentModal = _admin.viewUploads(requestid);
            return View("ViewUploads",viewDocumentModal);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewUploads(ViewDocumentModal viewDocumentModal)
        {
            var result = _admin.downloadMultipleFiles(viewDocumentModal);
            await result;
            Response.ContentType = "application/zip";
            Response.Headers.Add("Content-Disposition", $"attachment; filename={result.Result.Item2}");
            return File(result.Result.Item1.ToArray(), "application/zip", result.Result.Item2);
        }

        public IActionResult DeleteAll([FromForm]string filename)
        {
            int requestid = _admin.deleteAllFile(filename);
            return Json(new { isDeleted = true });
        }

        public IActionResult SendMailDocuments([FromForm] string filename)
        {
            Task<bool> sentMail = _admin.sendDocumentsMail(filename);
            return Json(new { isSent = sentMail });
        }

        public IActionResult ResetPassword(string token)
        {
            PasswordReset passwordReset = _admin.getPasswordReset(token);
            if (passwordReset == null)
            {
                return NotFound();
            }
            if (passwordReset.IsUpdated == true)
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
            resetPasswordViewModel.Token = token;
            return View(resetPasswordViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordViewModel modal)
        {
            if(ModelState.IsValid)
            {
                bool isReseted = _admin.resetPassword(modal);
                if(isReseted)
                {
                    TempData["success"] = "Password reseted Successfully!!";
                }
                else
                {
                    TempData["error"] = "Password could not be reseted!!";
                }
            }
            return View(modal);
        }

        public IActionResult GetPhysician(int regionid)
        {
            List<Physician> physician = _admin.getPhysician(regionid);
            return Json(new { data=physician});
        }

        public IActionResult AssignCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            bool isAssigned = _admin.assignCase(adminDashboardViewModel);
            if(isAssigned)
            {
                TempData["success"] = "Request Assigned Successfully!!";
            }
            else
            {
                TempData["error"] = "Request could not be assigned!!";
            }
            AdminDashboardViewModel newAdminDashboardViewModel = _admin.adminDashboardContent("New", null, null, -1);
            return View("Dashboard", newAdminDashboardViewModel);
        }

        public IActionResult TransferCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            bool isTransfered = _admin.transferCase(adminDashboardViewModel);
            if (isTransfered)
            {
                TempData["success"] = "Request Transferred Successfully!!";
            }
            else
            {
                TempData["error"] = "Request could not be transferred!!";
            }
            AdminDashboardViewModel newAdminDashboardViewModel = _admin.adminDashboardContent("New", null, null, -1);
            return View("Dashboard", newAdminDashboardViewModel);
        }

        public IActionResult SendAgreement(AdminDashboardViewModel adminDashboardViewModel)
        {
            Task<bool> isSent = _admin.sendAgreement(adminDashboardViewModel);
            if (isSent.Result)
            {
                TempData["success"] = "Agreement Sent Successfully!!";
            }
            else
            {
                TempData["error"] = "Agreement could not be sent!!";
            }
            AdminDashboardViewModel newAdminDashboardViewModel = _admin.adminDashboardContent("New", null, null, -1);
            return View("Dashboard", newAdminDashboardViewModel);
        }

        public IActionResult BlockCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            bool isBlocked = _admin.blockCase(adminDashboardViewModel);
            if (isBlocked)
            {
                TempData["success"] = "Request Blocked Successfully!!";
            }
            else
            {
                TempData["error"] = "Request could not be Blocked!!";
            }
            AdminDashboardViewModel newAdminDashboardViewModel = _admin.adminDashboardContent("New", null, null, -1);
            return View("Dashboard", newAdminDashboardViewModel);
        }

        public IActionResult ClearCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            bool isCleared = _admin.clearCase(adminDashboardViewModel);
            if (isCleared)
            {
                TempData["success"] = "Request Cleared Successfully!!";
            }
            else
            {
                TempData["error"] = "Request could not be Cleared!!";
            }
            AdminDashboardViewModel newAdminDashboardViewModel = _admin.adminDashboardContent("New", null, null, -1);
            return View("Dashboard", newAdminDashboardViewModel);
        }

        public IActionResult Orders(int id)
        {
            OrdersViewModel ordersViewModel = _admin.orders(id);
            return View(ordersViewModel);
        }

        public IActionResult GetBusiness(int professionid)
        {
            var business = _admin.getBusiness(professionid);
            return Json(new { data=business });
        }
        
        public IActionResult GetBusinessData(int businessid)
        {
            var business = _admin.getBusinessData(businessid);
            return Json(new { data=business });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Orders(OrdersViewModel ordersViewModel)
        {
            if(ModelState.IsValid)
            {

                if(ordersViewModel.business_id == -1)
                {
                    ModelState.AddModelError("business_id", "Please select Vendor!!");
                    return View(ordersViewModel);
                }

                bool isOrdered = _admin.placeOrder(ordersViewModel);
                if(isOrdered)
                {
                    TempData["success"] = "Order Placed Successfully!!";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    TempData["error"] = "Order could not be placed!!";
                }
            }
            return View(ordersViewModel);
        }

        public IActionResult Profile()
        {
            AdminProfileViewModel adminProfileViewModel = _admin.getAdmin();
            return View(adminProfileViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(AdminProfileViewModel adminProfileViewModel)
        {
            
            bool isUpdated = _admin.updateProfile(adminProfileViewModel);
            if (isUpdated)
            {
                TempData["success"] = "Information Updated Successfully!!";
            }
            else
            {
                TempData["error"] = "Information could not be Updated!!";
            }
            return RedirectToAction("Profile");
        }

        public IActionResult ResetPasswordProfile(string password)
        {
            if(password == null)
            {
                return Json(new { isReseted = 1 });
            }

            bool isReseted = _admin.resetPasswordProfile(password);
            if (isReseted)
            {
                return Json(new { isReseted = 2 });
            }
            else
            {
                return Json(new { isReseted = 3 });
            }
        }
    }
}
