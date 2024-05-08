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
using HalloDoc.Repository.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime.CompilerServices;
using HalloDoc.Models;
using Newtonsoft.Json.Linq;
using Rotativa.AspNetCore;
using DocumentFormat.OpenXml.Wordprocessing;
using HalloDoc.Repository.Repository;

namespace HalloDoc.Controllers
{

    [CustomAuthorize("Admin,Provider")]
    public class AdminController : Controller
    {
        private readonly IAdmin _admin;
        private readonly IPatient _patient;
        private readonly IJwtService _jwt;
        private readonly IDoctor _doctor;
        private readonly IHttpContextAccessor _context;
        private readonly Dictionary<string, string> mappingDictionary = new Dictionary<string, string>();

        public AdminController(IAdmin admin,IJwtService jwt, IPatient patient, IHttpContextAccessor context, IDoctor doctor)
        {
            _admin = admin;
            _jwt = jwt;
            _patient = patient;
            _context = context;
            _doctor = doctor;
            mappingDictionary.Add("Provider Location", "ProviderLocation");
            mappingDictionary.Add("Dashboard", "Dashboard");
            mappingDictionary.Add("Providerr", "Provider");
            mappingDictionary.Add("Scheduling", "Scheduling");
            mappingDictionary.Add("Invoicing", "Invoicing");
            mappingDictionary.Add("Partners", "Partners");
            mappingDictionary.Add("Account Access", "AccountAccess");
            mappingDictionary.Add("User Access", "UserAccess");
            mappingDictionary.Add("Search Records", "SearchRecord");
            mappingDictionary.Add("Email Logs", "EmailLog");
            mappingDictionary.Add("SMS Logs", "SMSLog");
            mappingDictionary.Add("Patient History", "PatientHistory");
            mappingDictionary.Add("Block History", "BlockHistory");
            mappingDictionary.Add("Create Admin Account", "CreateAdmin");
            mappingDictionary.Add("My Schedule", "MySchedule");
        }
        [CustomAuthorize("Admin,Provider", "Dashboard")]
        /// <summary>
        /// Get Method for Admin Dashboard
        /// </summary>
        /// <returns></returns>
        public IActionResult Dashboard()
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.AdminDashboardContent("New",null,null,-1);
            if(adminDashboardViewModel == null)
            {
                return NotFound();
            }
            return View(adminDashboardViewModel);
        }
        /// <summary>
        /// Filter Method for Status New
        /// </summary>
        /// <param name="search"></param>
        /// <param name="requestor"></param>
        /// <param name="region"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult New(string? search,string ?requestor,int? region,int page=1,int pageSize = 10)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.AdminDashboardContent("New", search, requestor, region,page,pageSize);
            if (adminDashboardViewModel == null)
            {
                return NotFound();
            }
            return PartialView("_AdminDashboardTable",adminDashboardViewModel);
        }
        /// <summary>
        /// Filter Method for Status Pending
        /// </summary>
        /// <param name="search"></param>
        /// <param name="requestor"></param>
        /// <param name="region"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult Pending(string? search, string? requestor, int? region, int page = 1, int pageSize = 10)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.AdminDashboardContent("Pending", search, requestor, region,page,pageSize);
            if (adminDashboardViewModel == null)
            {
                return NotFound();
            }
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }
        /// <summary>
        /// Filter Method for Status Active
        /// </summary>
        /// <param name="search"></param>
        /// <param name="requestor"></param>
        /// <param name="region"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult Active(string? search, string? requestor, int? region, int page = 1, int pageSize = 10)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.AdminDashboardContent("Active", search, requestor, region, page, pageSize);
            if (adminDashboardViewModel == null)
            {
                return NotFound();
            }
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }
        /// <summary>
        /// Filter Method for Status Conclude
        /// </summary>
        /// <param name="search"></param>
        /// <param name="requestor"></param>
        /// <param name="region"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult Conclude(string? search, string? requestor, int? region, int page = 1, int pageSize = 10)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.AdminDashboardContent("Conclude", search, requestor, region, page, pageSize);
            if (adminDashboardViewModel == null)
            {
                return NotFound();
            }
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }
        /// <summary>
        /// Filter Method for Status To Close
        /// </summary>
        /// <param name="search"></param>
        /// <param name="requestor"></param>
        /// <param name="region"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult Close(string? search, string? requestor, int? region, int page = 1, int pageSize = 10)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.AdminDashboardContent("ToClose", search, requestor, region,page,pageSize);
            if (adminDashboardViewModel == null)
            {
                return NotFound();
            }
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }
        /// <summary>
        /// Filter Method for Status Unpaid
        /// </summary>
        /// <param name="search"></param>
        /// <param name="requestor"></param>
        /// <param name="region"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult Unpaid(string? search, string? requestor, int? region, int page = 1, int pageSize = 10)
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.AdminDashboardContent("Unpaid", search, requestor, region, page, pageSize);
            if (adminDashboardViewModel == null)
            {
                return NotFound();
            }
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }
        /// <summary>
        /// Downloads Excel File with all request data
        /// </summary>
        /// <returns></returns>
        public IActionResult ExportAll()
        {
            MemoryStream memoryStream = _admin.ExportAll();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "All Data.xlsx");
        }

        /// <summary>
        /// Downloads Excel File of the filtered data present in current page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Dashboard(AdminDashboardViewModel model)
        {
            AdminDashboardViewModel viewmodel = _admin.AdminDashboardContent(model.status, model.search, model.requestor, model.RegionId, (int)model.CurrentPage, (int)model.PageSize);
            MemoryStream memoryStream = _admin.Export(viewmodel);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Data-{model.status}.xlsx");
        }
        /// <summary>
        /// Get Method for View Case Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ViewCase(int id)
        {
            
            ViewCaseViewModel viewCaseViewModel= _admin.ViewCase(id);
            if (viewCaseViewModel == null)
            {
                return NotFound();
            }
            return View(viewCaseViewModel);
        }
        /// <summary>
        /// Post Method for View Case that updates request client details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewCase(ViewCaseViewModel model)
        {
            bool status = _admin.ViewCase(model);
            if(status)
            {
                TempData["success"] = "Data Editted Successfully!!";
                return RedirectToAction("ViewCase", new { id = model.RequestId });
            }
            else
            {
                TempData["error"] = "Data could not be editted!!";
            }
            return View(model);
        }
        /// <summary>
        /// This Method will cancel the request from view case page
        /// </summary>
        /// <param name="viewCaseViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelRequest(ViewCaseViewModel viewCaseViewModel)
        {
            bool isUpdated = _admin.CancelRequest(viewCaseViewModel.RequestId,viewCaseViewModel.Admin_notes,viewCaseViewModel.CaseTag);
            if (isUpdated)
            {
                TempData["success"] = "Request Cancelled Successfully!!";
            }
            else
            {
                TempData["error"] = "Request could not be Cancelled!!";
            }
            return RedirectToAction("Dashboard");
        }
        /// <summary>
        /// This will send link of submit request page in email and sms to the email and phonenumber specified by the admin in the sendlink modal present in admin dashboard 
        /// </summary>
        /// <param name="dashboardViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendLink(AdminDashboardViewModel dashboardViewModel)
        {
            Task<bool> isSent = _admin.SendLink(dashboardViewModel);
            if (isSent.Result)
            {
                TempData["success"] = "Link Sent Successfully!!";
            }
            else
            {
                TempData["error"] = "Link could not be Sent!!";
            }
            return RedirectToAction("Dashboard");
        }
        /// <summary>
        /// This is get method for create request page in admin dashboard page
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateRequest()
        {
            PatientRequestViewModel patientRequestViewModel = _admin.CreateRequest();
            return View(patientRequestViewModel);
        }
        /// <summary>
        /// This is a post method which will create patient request from admin dashboard
        /// </summary>
        /// <param name="patientRequestViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRequest(PatientRequestViewModel? patientRequestViewModel)
        {
            if(ModelState.IsValid)
            {
                bool isVerified = _admin.VerifyRegion(patientRequestViewModel.State);
                if(!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(patientRequestViewModel);
                }

                bool isValidRole = _admin.CheckUserRole(patientRequestViewModel.Email);
                if (!isValidRole)
                {
                    TempData["error"] = "Only patients can create request!!!";
                    return View(patientRequestViewModel);
                }

                bool isBlocked = _admin.VerifyBlock(patientRequestViewModel.Email);
                if (isBlocked)
                {
                    TempData["error"] = "Patient with this email is blocked!!!";
                    return View(patientRequestViewModel);
                }

                Task<bool> requestCreated = _admin.CreateRequest(patientRequestViewModel);
                if(requestCreated.Result)
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
        /// <summary>
        /// This will verify whether state specified in create request form is there in the region where currently this service is available 
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult VerifyRegion(string region)
        {
            if(region == null)
            {
                return Json(new { isVerified=2 });
            }
            bool isVerified = _admin.VerifyRegion(region);
            if (isVerified)
            {
                return Json(new { isVerified = 1 });
            }
            else
            {
                return Json(new { isVerified = 3 });
            }
        }
        /// <summary>
        /// Get Method of View Note Action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ViewNotes(int id)
        {
            ViewNotesViewModel viewNotesViewModel = _admin.ViewNotes(id);
            if(viewNotesViewModel == null)
            {
                return NotFound();
            }
            return View(viewNotesViewModel);
        }
        /// <summary>
        /// Post Method of View Notes which will update admin note
        /// </summary>
        /// <param name="viewNotesViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewNotes(ViewNotesViewModel viewNotesViewModel)
        {
            bool isUpdated = _admin.UpdateAdminNotes(viewNotesViewModel);
            if(isUpdated)
            {
                TempData["success"] = "Admin Note Updated Successfully!!";
            }
            else
            {
                TempData["error"] = "Admin Note Could not be updated!!";
            }
            return RedirectToAction("ViewNotes",new { id = viewNotesViewModel.RequestId });
        }
        /// <summary>
        /// Get method of View Uploads Action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ViewUploads(int id)
        {
            ViewDocumentModal viewDocumentModal = _admin.ViewUploads(id);
            if(viewDocumentModal == null)
            {
                return NotFound();
            }
            return View(viewDocumentModal);
        }
        /// <summary>
        /// This method is used for uploading file in View Uploads action
        /// </summary>
        /// <param name="file"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult FileUpload([FromForm] IFormFile file, [FromForm] int id)
        {
            Task<bool> isFileUploaded = _admin.FileUpload(file,id);
            return Json(new { isFileUploaded = isFileUploaded });
        }
        /// <summary>
        /// This method will delete single file in View Uploads Action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteSingle(int id)
        {
            int requestid = _admin.DeleteSingleFile(id);
            if(requestid == -1)
            {
                TempData["error"] = "File could not be Deleted!!";
            }
            return RedirectToAction("ViewUploads",new { id = requestid });
        }
        /// <summary>
        /// This method is used for downloading multiple files in View Uploads Action
        /// </summary>
        /// <param name="viewDocumentModal"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewUploads(ViewDocumentModal viewDocumentModal)
        {
            var result = _admin.DownloadMultipleFiles(viewDocumentModal);
            await result;
            Response.ContentType = "application/zip";
            Response.Headers.Add("Content-Disposition", $"attachment; filename={result.Result.Item2}");
            return File(result.Result.Item1.ToArray(), "application/zip", result.Result.Item2);
        }
        /// <summary>
        /// This method will delete selected files in View Uploads Action
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public IActionResult DeleteAll([FromForm]string filename)
        {
            int requestid = _admin.DeleteAllFile(filename);
            if (requestid == -1)
            {
                TempData["error"] = "Files could not be Deleted!!";
            }
            return Json(new { isDeleted = true });
        }
        /// <summary>
        /// This method will send selected file as a attatchment in the mail to the patient in View Uploads Action
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public IActionResult SendMailDocuments([FromForm] string filename)
        {
            Task<bool> sentMail = _admin.SendDocumentsMail(filename);
            return Json(new { isSent = sentMail.Result });
        }
        /// <summary>
        /// This method will return all the physician present in a specific region in json format
        /// </summary>
        /// <param name="regionid"></param>
        /// <returns></returns>
        public IActionResult GetPhysician(int regionid)
        {
            List<RegionSpecificPhysician> physician = _admin.GetPhysician(regionid);
            return Json(new { data=physician});
        }
        /// <summary>
        /// It is a post method that will assign case of a specific request to the specified physician
        /// </summary>
        /// <param name="adminDashboardViewModel"></param>
        /// <returns></returns>
        public IActionResult AssignCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            bool isSame = _admin.SamePhysicianAssignCase(adminDashboardViewModel);
            if(!isSame)
            {
                TempData["error"] = "Request is already assigned to this physician!!";
                return RedirectToAction("Dashboard");
            }
            bool isAssigned = _admin.AssignCase(adminDashboardViewModel);
            if(isAssigned)
            {
                TempData["success"] = "Request Assigned Successfully!!";
            }
            else
            {
                TempData["error"] = "Request could not be assigned!!";
            }
            return RedirectToAction("Dashboard");
        }
        /// <summary>
        /// It is a post method that will transfer case of a specific request to the specified physician
        /// </summary>
        /// <param name="adminDashboardViewModel"></param>
        /// <returns></returns>
        public IActionResult TransferCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            bool isExists = _admin.IsSamePhysician(adminDashboardViewModel);
            if(isExists)
            {
                TempData["error"] = "This doctor already has the same case!!";
                return RedirectToAction("Dashboard");
            }

            bool isTransfered = _admin.TransferCase(adminDashboardViewModel);
            if (isTransfered)
            {
                TempData["success"] = "Request Transferred Successfully!!";
            }
            else
            {
                TempData["error"] = "Request could not be transferred!!";
            }
            return RedirectToAction("Dashboard");
        }
        /// <summary>
        /// This will send link of agreement to the patient in email and message
        /// </summary>
        /// <param name="adminDashboardViewModel"></param>
        /// <returns></returns>
        public IActionResult SendAgreement(AdminDashboardViewModel adminDashboardViewModel)
        {
            Task<bool> isSent = _admin.SendAgreement(adminDashboardViewModel);
            if (isSent.Result)
            {
                TempData["success"] = "Agreement Sent Successfully!!";
            }
            else
            {
                TempData["error"] = "Agreement could not be sent!!";
            }
            return RedirectToAction("Dashboard");
        }
        /// <summary>
        /// This will block the specified request i.e. its status will be set to 11 and entry corresponding to that request will be added in BlockRequests table
        /// </summary>
        /// <param name="adminDashboardViewModel"></param>
        /// <returns></returns>
        public IActionResult BlockCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            bool isBlocked = _admin.BlockCase(adminDashboardViewModel);
            if (isBlocked)
            {
                TempData["success"] = "Request Blocked Successfully!!";
            }
            else
            {
                TempData["error"] = "Request could not be Blocked!!";
            }
            return RedirectToAction("Dashboard");
        }
        /// <summary>
        /// This will clear the specified request i.e. its status will be set to 10
        /// </summary>
        /// <param name="adminDashboardViewModel"></param>
        /// <returns></returns>
        public IActionResult ClearCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            bool isCleared = _admin.ClearCase(adminDashboardViewModel);
            if (isCleared)
            {
                TempData["success"] = "Request Cleared Successfully!!";
            }
            else
            {
                TempData["error"] = "Request could not be Cleared!!";
            }
            return RedirectToAction("Dashboard");
        }
        /// <summary>
        /// This is the Get method of Orders Action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Orders(int id)
        {
            OrdersViewModel ordersViewModel = _admin.Orders(id);
            if(ordersViewModel == null)
            {
                return NotFound();
            }
            return View(ordersViewModel);
        }
        /// <summary>
        /// It will return all Health Professionals of the specified Health Professional Type and will return data in Json 
        /// </summary>
        /// <param name="professionid"></param>
        /// <returns></returns>
        public IActionResult GetBusiness(int professionid)
        {
            var business = _admin.GetBusiness(professionid);
            return Json(new { data=business });
        }
        /// <summary>
        /// It will return data of a specific Health Professionals and will return data in Json
        /// </summary>
        /// <param name="businessid"></param>
        /// <returns></returns>
        public IActionResult GetBusinessData(int businessid)
        {
            var business = _admin.GetBusinessData(businessid);
            return Json(new { data=business });
        }
        /// <summary>
        /// This is a post method of Orders action and it will place order for the specific request
        /// </summary>
        /// <param name="ordersViewModel"></param>
        /// <returns></returns>
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

                bool isOrdered = _admin.PlaceOrder(ordersViewModel);
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
        [CustomAuthorize("Admin,Provider", "My Profile")]
        /// <summary>
        /// It is a Get Method for Admin Profile Page
        /// </summary>
        /// <returns></returns>
        public IActionResult Profile()
        {
            AdminProfileViewModel adminProfileViewModel = _admin.GetAdmin(-1,"Profile");
            if(adminProfileViewModel == null)
            {
                return NotFound();
            }
            return View(adminProfileViewModel);
        }
        /// <summary>
        /// It is a post method that will update details in the admin profile page
        /// </summary>
        /// <param name="adminProfileViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(AdminProfileViewModel adminProfileViewModel)
        {
            
            bool isUpdated = _admin.UpdateProfile(adminProfileViewModel);
            if (isUpdated)
            {
                TempData["success"] = "Information Updated Successfully!!";

                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                AspNetUser aspNetUser = _patient.GetAspNetUserById(cookieModel.aspId);
                string jwtToken = _jwt.GenerateJWTAuthetication(aspNetUser);
                Response.Cookies.Append("jwt", jwtToken);
            }
            else
            {
                TempData["error"] = "Information could not be Updated!!";
            }
            return RedirectToAction("Profile");
        }
        /// <summary>
        /// This method will reset password for specific admin
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public IActionResult ResetPasswordProfile(string password,int id=-1)
        {
            if(password == null)
            {
                return Json(new { isReseted = 1 });
            }

            bool isReseted = _admin.ResetPasswordProfile(password,id);
            if (isReseted)
            {
                return Json(new { isReseted = 2 });
            }
            else
            {
                return Json(new { isReseted = 3 });
            }
        }
        /// <summary>
        /// It is a Get method for Encounter Form for specific request
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult EncounterForm(int id)
        {
            EncounterFormViewModel encounterFormViewModel = _admin.GetEncounterFormDetails(id);
            if(encounterFormViewModel == null)
            {
                return NotFound();
            }
            return View(encounterFormViewModel);
        }

        /// <summary>
        /// Encounter Form Post Method
        /// </summary>
        /// <param name="encounterFormViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EncounterForm(EncounterFormViewModel encounterFormViewModel)
        {
            bool isUpdated = _admin.UpdateEncounterForm(encounterFormViewModel);
            if(isUpdated)
            {
                TempData["success"] = "Encounter Form Updated Successfully!!";
            }
            else
            {
                TempData["error"] = "Encounter Form could not be Updated!!";
            }
            return RedirectToAction("EncounterForm",new { id = encounterFormViewModel.RequestId});
        }
        /// <summary>
        /// It is a Get Method for Close Case Action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult CloseCase(int id)
        {
            CloseCaseViewModel closeCaseViewModel = _admin.GetCloseCase(id);
            if(closeCaseViewModel == null)
            {
                return NotFound();
            }
            return View(closeCaseViewModel);
        }
        /// <summary>
        /// It is a Post Method of Close Case Action that will update Request Client details
        /// </summary>
        /// <param name="closeCaseViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CloseCase(CloseCaseViewModel closeCaseViewModel)
        {
            bool isUpdated = _admin.UpdateCloseCase(closeCaseViewModel);
            if (isUpdated)
            {
                TempData["success"] = "Information Updated Successfully!!";
            }
            else
            {
                TempData["error"] = "Information could not be Updated!!";
            }
            return RedirectToAction("CloseCase",new { id = closeCaseViewModel.RequestId});
        }
        /// <summary>
        /// It will close the case i.e. the status will be set to unpaid(9)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult FinalCloseCase(int id)
        {
            bool isClosed = _admin.CloseCase(id);
            if (isClosed)
            {
                TempData["success"] = "Case Closed!!";
            }
            else
            {
                TempData["error"] = "Case could not be Closed!!";
            }
            return RedirectToAction("Dashboard");
        }
        [CustomAuthorize("Admin,Provider", "Providerr")]
        /// <summary>
        /// It is a get method for Provider Page
        /// </summary>
        /// <returns></returns>
        public IActionResult Provider()
        {
            ProviderViewModel providerViewModel = _admin.GetProviderPageDetails(-1);
            return View(providerViewModel);
        }
        /// <summary>
        /// It will return the filtered and paginated result in the provider page in the form of partial view
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult ProviderTable(int id = -1,int page=1,int pageSize = 10)
        {
            ProviderViewModel providerViewModel = _admin.GetProviderPageDetails(id,page,pageSize);
            return PartialView("_AdminProviderTable", providerViewModel);
        }
        /// <summary>
        /// It is used to toggle change notification field in Providers page
        /// </summary>
        /// <param name="id"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public IActionResult ChangeNotification(int id,bool update)
        {
            bool isUpdated = _admin.ChangeNotification(id,update);
            return Json(new {isUpdated = isUpdated });
        }
        /// <summary>
        /// It will either mail or message or both to provider
        /// </summary>
        /// <param name="providerViewModel"></param>
        /// <returns></returns>
        public IActionResult ContactProvider(ProviderViewModel providerViewModel)
        {
            Task<bool> isSent = _admin.ContactProvider(providerViewModel);
            if (isSent.Result)
            {
                TempData["success"] = "Your Message Sent Successfully!!";
            }
            else
            {
                TempData["error"] = "Your Message Could not be sent!!";
            }
            return RedirectToAction("Provider");
        }
        [CustomAuthorize("Admin,Provider", "Providerr")]
        /// <summary>
        /// It is Get method for CreatePhysician
        /// </summary>
        /// <returns></returns>
        public IActionResult CreatePhysician()
        {
            PhysicianAccountViewModel physicianAccountViewModel = _admin.GetCreatePhysicianDetails();
            return View(physicianAccountViewModel);
        }
        /// <summary>
        /// It is a post method of Create Physician that will Create Physician
        /// </summary>
        /// <param name="physicianAccountViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePhysician(PhysicianAccountViewModel physicianAccountViewModel)
        {
            List<Role> roles = _admin.GetPhysicianRoles();
            physicianAccountViewModel.roles = roles;
            if (ModelState.IsValid)
            {
                if(_patient.GetAspNetUser(physicianAccountViewModel.Email) != null)
                {
                    TempData["error"] = "This Email Id Already Exists!!";
                    return View(physicianAccountViewModel);
                }

                Task<bool> isCreated = _admin.CreatePhysician(physicianAccountViewModel);
                if(isCreated.Result)
                {
                    TempData["success"] = "Account Created Successfully!!";
                    return RedirectToAction("Provider");
                }
                else
                {
                    TempData["error"] = "Account Could not be Created!!";
                }
            }
            return View(physicianAccountViewModel);
        }
        [CustomAuthorize("Admin,Provider", "Providerr")]
        /// <summary>
        /// It is Get method of Edit Physician
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult EditPhysician(int id)
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Provider",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            PhysicianAccountViewModel physicianAccountViewModel = _admin.GetPhysicianDetails(id,adminNavbarViewModel);
            if(physicianAccountViewModel == null)
            {
                return NotFound();
            }
            return View(physicianAccountViewModel);
        }
        /// <summary>
        /// It is Get method of Edit Physician From user access page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult EditPhysicianUserAccess(int id)
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Access",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            PhysicianAccountViewModel physicianAccountViewModel = _admin.GetPhysicianDetails(id,adminNavbarViewModel);
            if(physicianAccountViewModel == null)
            {
                return NotFound();
            }

            return View("EditPhysician",physicianAccountViewModel);
        }
        /// <summary>
        /// It will upload files that are present in onboarding section in Edit Physician Page
        /// </summary>
        /// <param name="file"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IActionResult FileUploadPhysician([FromForm] IFormFile file, [FromForm] int id, [FromForm] string name)
        {
            Task<bool> isUploaded = _admin.FileUploadPhysician(file, id, name);
            return Json(new { isUploaded = isUploaded.Result });
        }
        /// <summary>
        /// It is post method of Edit Physician Page
        /// </summary>
        /// <param name="physicianAccountViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPhysician(PhysicianAccountViewModel physicianAccountViewModel)
        {
            int isAvailable = _admin.CheckPhysicianEmail(physicianAccountViewModel);
            if(isAvailable == 3)
            {
                TempData["error"] = "This Email Already Exists!!";
                return RedirectToAction("EditPhysician", new { id = physicianAccountViewModel.PhysicianId });
            }
            else if(isAvailable == 1)
            {
                TempData["error"] = "This Physician Does Not Exists!!";
                return RedirectToAction("EditPhysician", new { id = physicianAccountViewModel.PhysicianId });
            }
            Task<bool> isUpdated = _admin.UpdatePhysician(physicianAccountViewModel);
            if (isUpdated.Result)
            {
                TempData["success"] = "Information Updated Successfully!!";
            }
            else
            {
                TempData["error"] = "Information could not be Updated!!";
            }
            if(physicianAccountViewModel.adminNavbarViewModel.curr_active == "Access")
            {
                return RedirectToAction("EditPhysicianUserAccess", new { id=physicianAccountViewModel.PhysicianId });
            }
            else
            {
                return RedirectToAction("EditPhysician", new { id = physicianAccountViewModel.PhysicianId });
            }
        }
        /// <summary>
        /// It will reset password in Edit Physician Page
        /// </summary>
        /// <param name="password"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ResetPasswordPhysician(string password, int id)
        {
            int isreset = 0;
            if(password == null)
            {
                isreset = 1;
                return Json(new { isReseted = isreset });
            }

            bool isReseted = _admin.ResetPasswordPhysician(password, id);
            if(isReseted)
            {
                isreset = 2;
            }
            else
            {
                isreset = 3;
            }
            return Json(new { isReseted  = isreset });
        }
        [CustomAuthorize("Admin,Provider", "Providerr")]
        /// <summary>
        /// It will delete the specified physician from edit physician page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeletePhysician(int id)
        {
            bool isDeleted = _admin.DeletePhysician(id);
            if (isDeleted)
            {
                TempData["success"] = "Account Deleted!!";
            }
            else
            {
                TempData["error"] = "Account could not be deleted!!";
            }
            return RedirectToAction("Provider");
        }
        [CustomAuthorize("Admin,Provider", "Patient History")]
        /// <summary>
        /// It is Get Method of Patient History Page
        /// </summary>
        /// <returns></returns>
        public IActionResult PatientHistory()
        {
            PatientHistoryViewModel patientHistoryViewModel = _admin.GetAllPatients(null,null,null,null);
            return View(patientHistoryViewModel);
        }
        /// <summary>
        /// It will return filtered and paginated data for Patient History Page as Partial View
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="email"></param>
        /// <param name="phone"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult PatientHistoryTable(string? firstname,string? lastname,string? email,string? phone,int page=1,int pageSize = 10)
        {
            PatientHistoryViewModel patientHistoryViewModel = _admin.GetAllPatients(firstname,lastname,email,phone,page,pageSize);
            return PartialView("_PatientHistoryTable", patientHistoryViewModel);
        }
        [CustomAuthorize("Admin,Provider", "Patient History")]
        /// <summary>
        /// It is a Get method for Patient Record Page
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult PatientRecord(int id, int page = 1, int pageSize = 10)
        {
            PatientHistoryViewModel patientHistoryViewModel = _admin.GetAllPatientRecords(id,page,pageSize);
            return View(patientHistoryViewModel);
        }
        /// <summary>
        /// It will return filtered and paginated data for Patient Record Page as Partial View
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult PatientRecordTable(int id, int page = 1, int pageSize = 10)
        {
            PatientHistoryViewModel patientHistoryViewModel = _admin.GetAllPatientRecords(id,page,pageSize);
            return PartialView("_PatientRecordTable", patientHistoryViewModel);
        }
        [CustomAuthorize("Admin,Provider", "Block History")]
        /// <summary>
        /// It is a Get method for Block History Page
        /// </summary>
        /// <returns></returns>
        public IActionResult BlockHistory()
        {
            BlockHistoryViewModel blockHistoryViewModel = _admin.GetBlockHistoryData(null, null, null, null);
            return View(blockHistoryViewModel);
        }
        /// <summary>
        /// It will return filtered and paginated data for Block History Page as Partial View
        /// </summary>
        /// <param name="name"></param>
        /// <param name="date"></param>
        /// <param name="email"></param>
        /// <param name="phone"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult BlockHistoryData(string? name, DateTime? date, string? email, string? phone, int page = 1, int pageSize = 10)
        {
            BlockHistoryViewModel blockHistoryViewModel = _admin.GetBlockHistoryData(name, date, email, phone, page, pageSize);
            return PartialView("_RequestBlockHistory", blockHistoryViewModel);
        }
        /// <summary>
        /// It will toggle is active checkbox in Block Request page
        /// </summary>
        /// <param name="blockrequestid"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IActionResult ToggleActive(int blockrequestid,bool value)
        {
            bool isToggled = _admin.ToggleActive(blockrequestid, value);
            return Json(new { isToggled = isToggled });
        }
        /// <summary>
        /// It will Unblock the Blocked Request from Block Request Page
        /// </summary>
        /// <param name="blockrequestid"></param>
        /// <returns></returns>
        public IActionResult RestoreBlock(int blockrequestid)
        {
            bool isRestored = _admin.RestoreBlock(blockrequestid);
            if(isRestored)
            {
                TempData["success"] = "Request Restored Successfully!!";
            }
            else
            {
                TempData["error"] = "Request could not be Restored!!";
            }
            return RedirectToAction("Dashboard");
        }
        [CustomAuthorize("Admin,Provider", "Search Records")]
        /// <summary>
        /// It is Get method for Search Record Page
        /// </summary>
        /// <returns></returns>
        public IActionResult SearchRecord()
        {
            SearchRecordViewModel searchRecordViewModel = _admin.GetSearchedData(null, null, null, null, null, null, null, null);
            return View(searchRecordViewModel);
        }
        /// <summary>
        /// It will return filtered and paginated data for Search Record Page as Partial View
        /// </summary>
        /// <param name="status"></param>
        /// <param name="name"></param>
        /// <param name="requesttypeid"></param>
        /// <param name="fromdos"></param>
        /// <param name="todos"></param>
        /// <param name="providername"></param>
        /// <param name="email"></param>
        /// <param name="phonenumber"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult SearchRecordTable(int? status, string? name, int? requesttypeid, DateTime? fromdos, DateTime? todos, string? providername, string? email, string? phonenumber, int page = 1, int pageSize = 10)
        {
            SearchRecordViewModel searchRecordViewModel = _admin.GetSearchedData(status, name, requesttypeid, fromdos, todos, providername, email, phonenumber, page, pageSize);
            return PartialView("_SearchRecordTable",searchRecordViewModel);
        }
        /// <summary>
        /// It will delete specific request from Search Records Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteRequest(int id)
        {
            bool isDeleted = _admin.DeleteRequest(id);
            if(isDeleted)
            {
                TempData["success"] = "Request Deleted Successfully!!";
            }
            else{
                TempData["error"] = "Request could not be Deleted!!";
            }
            return RedirectToAction("SearchRecord");
        }
        /// <summary>
        /// It will Export Filtered Data into Excel in Seacxrh Record Page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IActionResult ExportSearchedData(SearchRecordViewModel model)
        {
            SearchRecordViewModel searchRecordViewModel = _admin.GetSearchedData(model.status,  model.name, model.requesttypeid, model.fromdos, model.todos, model.providername, model.email, model.phonenumber, (int)model.CurrentPage, (int)model.PageSize);
            MemoryStream memoryStream = _admin.ExportSearchedData(searchRecordViewModel);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Filtered-Data.xlsx");
        }
        [CustomAuthorize("Admin,Provider", "Account Access")]
        /// <summary>
        /// It is Get Method of Account Access Page
        /// </summary>
        /// <returns></returns>
        public IActionResult AccountAccess()
        {
            AccountAccessViewModel accountAccessViewModel = _admin.GetAllRolesDetails();
            return View(accountAccessViewModel);
        }
        /// <summary>
        /// It will return filtered and paginated data for Account Access Page as Partial View
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult AccountAccessTable(int page = 1, int pageSize = 10)
        {
            AccountAccessViewModel accountAccessViewModel = _admin.GetAllRolesDetails(page,pageSize);
            return PartialView("_AccountAccessTable",accountAccessViewModel);
        }
        [CustomAuthorize("Admin,Provider", "Account Access")]
        /// <summary>
        /// It is a get method for Create Access Page
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateAccess()
        {
            AdminNavbarViewModel adminNavbarViewModel = _admin.GetCreateAccessNavbar();
            return View(adminNavbarViewModel);
        }
        /// <summary>
        /// It will return Menus for the specified Account Type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetMenus(int? id)
        {
            List<Menu> menu = _admin.GetMenus(id);
            return Json(new { data = menu });
        }
        /// <summary>
        /// It is a post method for Create Role Page
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="role_name"></param>
        /// <param name="account_type"></param>
        /// <returns></returns>
        public IActionResult CreateRole(string? menus,string? role_name,int? account_type)
        {
            bool isExist = _admin.CheckRole(role_name);
            if(isExist)
            {
                return Json(new { isCreated = 1 });
            }
            bool isCreated = _admin.CreateRole(menus, role_name, account_type);
            if(isCreated)
            {
                TempData["success"] = "Role Created Successfully!!";
            }
            else
            {
                TempData["error"] = "Role could not be Created!!";
            }
            return Json(new { isCreated  = isCreated == true ? 2 : 3 });
        }
        /// <summary>
        /// It will delete specific role from Account Access Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteRole(int? id)
        {
            bool isDeleted = _admin.DeleteRole(id);
            if(isDeleted)
            {
                TempData["success"] = "Role Deleted Successfully!!";
            }
            else
            {
                TempData["error"] = "Role could not be Deleted!!";
            }
            return RedirectToAction("AccountAccess");
        }
        [CustomAuthorize("Admin,Provider", "Account Access")]
        /// <summary>
        /// It is a Get method for Edit Role page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult EditRole(int? id)
        {
            EditAccessViewModel editAccessViewModel = _admin.GetRoleDetails(id);
            if(editAccessViewModel == null)
            {
                return NotFound();
            }
            return View(editAccessViewModel);
        }
        /// <summary>
        /// It is a post method for Edit Role Page
        /// </summary>
        /// <param name="id"></param>
        /// <param name="menus"></param>
        /// <param name="role_name"></param>
        /// <param name="account_type"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult EditRole(int? id, string? menus, string? role_name, int? account_type)
        {
            bool isExist = _admin.CheckRole(role_name);
            if (isExist)
            {
                return Json(new { isEditted = 4 });
            }
            bool isEditted = _admin.EditRoleDetails(id,menus,role_name,account_type);
            if(isEditted)
            {
                TempData["success"] = "Role Editted Successfully!!";
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);
                AspNetUser aspNetUser = _patient.GetAspNetUserById(cookieModel.aspId);
                string jwtToken = _jwt.GenerateJWTAuthetication(aspNetUser);
                Response.Cookies.Append("jwt", jwtToken);

                mappingDictionary.Add("My Profile", "Profile");
                CookieModel cookieModelupdated = _jwt.GetDetails(jwtToken);
                if (!cookieModelupdated.menus.Contains("Account Access"))
                {
                    return Json(new { isEditted = 1,url = $"/Admin/{mappingDictionary[cookieModelupdated.menus.Split(",")[0]]}" });
                }

            }
            else
            {
                TempData["error"] = "Role could not be Editted!!";
            }
            return Json(new { isEditted = isEditted == true ? 2 : 3 });
        }
        [CustomAuthorize("Admin,Provider", "Email Logs")]
        /// <summary>
        /// It is a Get method for Email Log Page
        /// </summary>
        /// <returns></returns>
        public IActionResult EmailLog()
        {
            EmailLogViewModel emailLogViewModel = _admin.GetEmailLogDetails(-1,null,null,null,null);
            return View(emailLogViewModel);
        }
        /// <summary>
        /// It will return filtered and paginated data for Email Log Page as Partial View
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="createddate"></param>
        /// <param name="sentdate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult EmailLogTable(int? roleid, string? name, string? email, DateTime? createddate, DateTime? sentdate, int page = 1, int pageSize = 10)
        {
            EmailLogViewModel emailLogViewModel = _admin.GetEmailLogDetails(roleid,name,email,createddate,sentdate,page,pageSize);
            return PartialView("_EmailLogTable",emailLogViewModel);
        }
        [CustomAuthorize("Admin,Provider", "SMS Logs")]
        /// <summary>
        /// It is a Get method for SMS Log Page
        /// </summary>
        /// <returns></returns>
        public IActionResult SMSLog()
        {
            EmailLogViewModel emailLogViewModel = _admin.GetSMSLogDetails(-1, null, null, null, null);
            return View(emailLogViewModel);
        }
        /// <summary>
        /// It will return filtered and paginated data for SMS Log Page as Partial View
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="name"></param>
        /// <param name="phonenumber"></param>
        /// <param name="createddate"></param>
        /// <param name="sentdate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult SMSLogTable(int? roleid, string? name, string? phonenumber, DateTime? createddate, DateTime? sentdate, int page = 1, int pageSize = 10)
        {
            EmailLogViewModel emailLogViewModel = _admin.GetSMSLogDetails(roleid, name, phonenumber, createddate, sentdate, page, pageSize);
            return PartialView("_SMSLogTable", emailLogViewModel);
        }
        [CustomAuthorize("Admin,Provider", "Partners")]
        /// <summary>
        /// It is a Get method for Partners Page
        /// </summary>
        /// <returns></returns>
        public IActionResult Partners()
        {
            PartnerViewModal partnerViewModal = _admin.GetPartnerDetails(null,-1);
            return View(partnerViewModal);
        }
        /// <summary>
        /// It will return filtered and paginated data for Partners Page as Partial View
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult PartnersTable(string? name,int? id,int page = 1,int pageSize = 10)
        {
            PartnerViewModal partnerViewModal = _admin.GetPartnerDetails(name,id,page,pageSize);
            return PartialView("_PartnersTable", partnerViewModal);
        }
        [CustomAuthorize("Admin,Provider", "Partners")]
        /// <summary>
        /// It is a Get Method for Create Business
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateBusiness()
        {
            BusinessViewModel businessViewModel = _admin.GetBusinessNavbar();
            return View("Business",businessViewModel);
        }
        /// <summary>
        /// It is a Post method for Create Business
        /// </summary>
        /// <param name="businessViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddBusiness(BusinessViewModel businessViewModel)
        {
            if(ModelState.IsValid)
            {

                bool isCreated = _admin.CreateBusiness(businessViewModel);
                if(isCreated)
                {
                    TempData["success"] = "Vendor Added Successfully!!";
                    return RedirectToAction("Partners");
                }
                else
                {
                    TempData["error"] = "Vendor could not be Added!!";
                }
            }
            return View("Business", businessViewModel);
        }
        [CustomAuthorize("Admin,Provider", "Partners")]
        /// <summary>
        /// It is a Get method for Edit Business
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult EditBusiness(int id)
        {
            BusinessViewModel businessViewModel = _admin.GetBusinessDetails(id);
            if(businessViewModel == null)
            {
                return NotFound();
            }
            return View("Business", businessViewModel);
        }
        /// <summary>
        /// It is a Post method for Edit Business
        /// </summary>
        /// <param name="businessViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult EditBusiness(BusinessViewModel businessViewModel)
        {
            if (ModelState.IsValid)
            {

                bool isEditted = _admin.EditBusiness(businessViewModel);
                if (isEditted)
                {
                    TempData["success"] = "Vendor Updated Successfully!!";
                    return RedirectToAction("Partners");
                }
                else
                {
                    TempData["error"] = "Vendor could not be Updated!!";
                }
            }
            return View("Business", businessViewModel);
        }
        /// <summary>
        /// It will delete specified Business from Partners Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteBusiness(int id)
        {
            bool isDeleted = _admin.DeleteBusiness(id);
            if(isDeleted)
            {
                TempData["success"] = "Vendor Deleted Successfully!!";
            }
            else
            {
                TempData["error"] = "Vendor could not be Deleted!!";
            }
            return RedirectToAction("Partners");
        }
        [CustomAuthorize("Admin", "Provider Location")]
        /// <summary>
        /// It is a get method for Provider Location Page
        /// </summary>
        /// <returns></returns>
        public IActionResult ProviderLocation()
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            if(token == null)
            {
                return RedirectToAction("PatientLogin","Login");
            }
            ProviderLocationViewModel providerLocationViewModel = _admin.GetProviderLocation();
            return View(providerLocationViewModel);
        }
        [CustomAuthorize("Admin", "Create Admin Account")]
        /// <summary>
        /// It is a Get Method for Create Admin
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateAdmin()
        {
            AdminProfileViewModel adminProfileViewModel = _admin.GetCreateAdminProfilePageDetails();
            return View(adminProfileViewModel);
        }
        /// <summary>
        /// It is a post method for Create Admin Page 
        /// </summary>
        /// <param name="adminProfileViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAdmin(AdminProfileViewModel adminProfileViewModel)
        {
            List<Role> roles = _admin.GetAdminRoles();
            adminProfileViewModel.roles = roles;
            if(ModelState.IsValid)
            {
                if (_patient.GetAspNetUser(adminProfileViewModel.Email) != null)
                {
                    TempData["error"] = "This Email Id Already Exists!!";
                    return View(adminProfileViewModel);
                }
                Task<bool> isCreated = _admin.CreateAdmin(adminProfileViewModel);
                if(isCreated.Result)
                {
                    TempData["success"] = "Admin Created Successfully!!";
                    return RedirectToAction("UserAccess");
                }
                else
                {
                    TempData["error"] = "Admin could not be Created!!";
                }
            }
            return View(adminProfileViewModel);

        }
        [CustomAuthorize("Admin,Provider", "User Access")]
        /// <summary>
        /// It is a Get method for User Access Page
        /// </summary>
        /// <returns></returns>
        public IActionResult UserAccess()
        {
            var useraccess = _admin.GetUserAccessDetails(-1);
            return View(useraccess);
        }
        /// <summary>
        /// It will return filtered and paginated data for User Access Page as Partial View
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult UserAccessTable(int? roleid,int page=1,int pageSize=10)
        {
            var useraccess = _admin.GetUserAccessDetails(roleid,page,pageSize);
            return PartialView("_UserAccessTable",useraccess);
        }
        [CustomAuthorize("Admin,Provider", "User Access")]
        /// <summary>
        /// It is a Get method for Edit Admin Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult EditAdmin(int id)
        {
            AdminProfileViewModel adminProfileViewModel = _admin.GetAdmin(id, "Access");
            if(adminProfileViewModel == null)
            {
                return NotFound();
            }
            return View(adminProfileViewModel);
        }
        /// <summary>
        /// It is a Post Method for Edit Admin Page
        /// </summary>
        /// <param name="adminProfileViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAdmin(AdminProfileViewModel adminProfileViewModel) {
            int isAvailable = _admin.CheckAdminEmail(adminProfileViewModel);
            if (isAvailable == 3)
            {
                TempData["error"] = "This Email Already Exists!!";
                return RedirectToAction("EditAdmin", new { id = adminProfileViewModel.admin_id });
            }
            else if (isAvailable == 1)
            {
                TempData["error"] = "This Admin Does Not Exists!!";
                return RedirectToAction("EditAdmin", new { id = adminProfileViewModel.admin_id });
            }
            bool isUpdated = _admin.UpdateProfile(adminProfileViewModel);
            if (isUpdated)
            {
                TempData["success"] = "Information Updated Successfully!!";
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);
                AspNetUser aspNetUser = _patient.GetAspNetUserById(cookieModel.aspId);
                string jwtToken = _jwt.GenerateJWTAuthetication(aspNetUser);
                Response.Cookies.Append("jwt", jwtToken);
            }
            else
            {
                TempData["error"] = "Information could not be Updated!!";
            }
            return RedirectToAction("EditAdmin", new { id = adminProfileViewModel.admin_id });
        }
        /// <summary>
        /// It will delete specified Admin from Edit Admin Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteAdmin(int id)
        {
            bool isDeleted = _admin.DeleteAdmin(id);
            if (isDeleted)
            {
                TempData["success"] = "Admin Deleted Successfully!!";
            }
            else
            {
                TempData["error"] = "Information could not be Deleted!!";
            }
            return RedirectToAction("UserAccess");
        }
        [CustomAuthorize("Admin,Provider", "Scheduling")]
        /// <summary>
        /// It is a get method for Scheduling
        /// </summary>
        /// <returns></returns>
        public IActionResult Scheduling()
        {
            SchedulingViewModel schedulingViewModel = _admin.GetAllShiftDetails(-1);
            return View(schedulingViewModel);
        }
        /// <summary>
        /// It will return filtered shifts based on region id as Json
        /// </summary>
        /// <param name="regionid"></param>
        /// <returns></returns>
        public IActionResult SchedulingTable(int regionid = -1)
        {
            SchedulingViewModel schedulingViewModel = _admin.GetAllShiftDetails(regionid);
            return Json(new { data = schedulingViewModel.shiftViewModels });
        }

        /// <summary>
        /// It is a post method for create shift which will create shift for the specified physician if he/she is not already having any shift at that time
        /// </summary>
        /// <param name="schedulingViewModel"></param>
        /// <returns></returns>
        public IActionResult CreateShift(SchedulingViewModel schedulingViewModel)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);
            if(ModelState.IsValid)
            {

                if(cookieModel.role == "Provider")
                {
                    schedulingViewModel.PhysicianId = cookieModel.userId;
                }

                int isCreated = _admin.CreateShift(schedulingViewModel);
                if(isCreated == 1)
                {
                    TempData["error"] = "Physician is already scheduled in this slot!!";
                }
                else if(isCreated == 2)
                {
                    TempData["error"] = "Shift could not be created!!";
                }
                else
                {
                    TempData["success"] = "Shift Created Successfully!!";
                }
                if (cookieModel.role == "Provider")
                {
                    return RedirectToAction("MySchedule","Doctor");
                }
                return RedirectToAction("Scheduling");
            }
            if (cookieModel.role == "Provider")
            {
                return View("/Views/Doctor/MySchedule.cshtml", schedulingViewModel);
            }
            return View("Scheduling", schedulingViewModel);
        }
        /// <summary>
        /// It is a post method for editting shift
        /// </summary>
        /// <param name="schedulingViewModel"></param>
        /// <returns></returns>
        public IActionResult EditShift(DateTime shiftdate,TimeOnly starttime,TimeOnly endtime, int physicianid, int shiftdetailid)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            int isEditted = _admin.EditShift(shiftdate, starttime, endtime, physicianid, shiftdetailid);
            return Json(new { isEditted = isEditted });
        }
        /// <summary>
        /// This method will delete shift of the specified Shift Detail Id
        /// </summary>
        /// <param name="schedulingViewModel"></param>
        /// <returns></returns>
        public IActionResult DeleteShift(int id)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            bool isDeleted = _admin.DeleteShift(id);
            return Json(new { isDeleted = isDeleted });
        }
        [CustomAuthorize("Admin,Provider", "Scheduling")]
        /// <summary>
        /// It is a get method for Providers on call page
        /// </summary>
        /// <returns></returns>
        public IActionResult MdOnCall()
        {
            MDOnCallViewModel mDOnCallViewModel = _admin.GetMdOnCallDetails();
            return View(mDOnCallViewModel);
        }
        /// <summary>
        /// It will return filtered physicians that are on and off duty based on region id as Partial View
        /// </summary>
        /// <param name="regionid"></param>
        /// <returns></returns>
        public IActionResult MdOnCallTable(int regionid = -1)
        {
            MDOnCallViewModel mDOnCallViewModel = _admin.GetMdOnCallDetails(regionid);
            return PartialView("_MDOnCallPhysicians", mDOnCallViewModel);
        }
        [CustomAuthorize("Admin,Provider", "Scheduling")]
        /// <summary>
        /// It is a get method for Shifts for Review Page
        /// </summary>
        /// <returns></returns>
        public IActionResult ShiftsForReview()
        {
            ShiftsForReviewViewModel shiftsForReviewViewModel = _admin.GetRequestedShifts();
            return View(shiftsForReviewViewModel);
        }
        /// <summary>
        /// It will return filtered and paginated data for Shifts For Review Page as Partial View
        /// </summary>
        /// <param name="regionid"></param>
        /// <param name="currMonth"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult ShiftsForReviewTable(int regionid = -1, int page = 1, int pageSize = 10)
        {
            ShiftsForReviewViewModel shiftsForReviewViewModel = _admin.GetRequestedShifts(regionid,page,pageSize);
            return PartialView("_ShiftsForReviewTable", shiftsForReviewViewModel);
        }
        /// <summary>
        /// It will aproove all the selected shifts based on shiftdetailid
        /// </summary>
        /// <param name="shiftsForReviewViewModel"></param>
        /// <returns></returns>
        public IActionResult AprooveShifts(ShiftsForReviewViewModel shiftsForReviewViewModel)
        {
            bool isApproved = _admin.AprooveShifts(shiftsForReviewViewModel);
            if(isApproved)
            {
                TempData["success"] = "Shifts Aprooved Successfully!!";
            }
            else
            {
                TempData["error"] = "Shift could not be Aprooved!!";
            }
            return RedirectToAction("ShiftsForReview");
        }
        /// <summary>
        /// It will deleted all the selected shifts based on shiftdetailid
        /// </summary>
        /// <param name="shiftsForReviewViewModel"></param>
        /// <returns></returns>
        public IActionResult DeleteShifts(ShiftsForReviewViewModel shiftsForReviewViewModel)
        {
            bool isDeleted = _admin.DeleteShifts(shiftsForReviewViewModel);
            if (isDeleted)
            {
                TempData["success"] = "Shifts Deleted Successfully!!";
            }
            else
            {
                TempData["error"] = "Shift could not be Deleted!!";
            }
            return RedirectToAction("ShiftsForReview");
        }
        /// <summary>
        /// It will toggle shift status when admin clicks return button from view shift modal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ToggleShiftStatus(int? id)
        {
            bool isToggled = _admin.ToggleShiftStatus(id);
            return Json(new { isToggled = isToggled });
        }
        /// <summary>
        /// This method will send message specified by admin via email to all the unscheduled physicians
        /// </summary>
        /// <param name="adminDashboardViewModel"></param>
        /// <returns></returns>
        public IActionResult RequestDTYSupport(AdminDashboardViewModel adminDashboardViewModel)
        {
            Task<bool> isSent = _admin.RequestDTYSupport(adminDashboardViewModel);
            if(isSent.Result)
            {
                TempData["success"] = "Mail Sent Successfully!!";
            }
            else
            {
                TempData["error"] = "Mail Could not be sent!!";
            }
            return RedirectToAction("Dashboard");
        }
        /// <summary>
        /// This method will redirect from Requested shifts page to Scheduling page with default view of Month Calendar
        /// </summary>
        /// <returns></returns>
        public IActionResult ViewCurrentMonthShift()
        {
            TempData["Shift"] = "Month";
            return RedirectToAction("Scheduling");
        }
        /// <summary>
        /// This method will download encounter form for the specified request id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DownloadEncounterForm(int id)
        {
            var model = _admin.GetEncounterFormDetails(id);

            var request = _admin.GetRequest(id);

            return new ViewAsPdf("../Shared/_EncounterForm", model)
            {
                FileName = $"EncounterReport-{request.ConfirmationNumber}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageMargins = { Left = 20, Right = 20 }
            };
        }

        public IActionResult PayRate(int id)
        {
            PayRateViewModel payRateViewModel = _admin.GetPayRate(id);
            if(payRateViewModel == null)
            {
                return NotFound();
            }
            return View(payRateViewModel);
        }
        [HttpPost]
        public IActionResult UpdatePayRate(PayRateViewModel payRateViewModel)
        {
            bool isUpdated = _admin.UpdatePayRate(payRateViewModel);
            if (isUpdated)
            {
                TempData["success"] = "Pay Rate Updated Successfully!!";
            }
            else
            {
                TempData["error"] = "Pay Rate could not be Updated!!";
            }
            return RedirectToAction("PayRate", new { id = payRateViewModel.PhysicianId });
        }
        public IActionResult Invoicing()
        {
            PhysicianInvoicingViewModel physicianInvoicingViewModel = _doctor.GetPhysicianInvoicingDetails(null, null);
            return View(physicianInvoicingViewModel);
        }
        public IActionResult InvoicingTable(DateTime startdate, DateTime enddate,int id)
        {
            PhysicianInvoicingViewModel physicianInvoicingViewModel = _doctor.GetPhysicianInvoicingDetails(startdate, enddate,id);
            return PartialView("_AdminInvoicingTable", physicianInvoicingViewModel);
        }

        public IActionResult TimeSheet(DateTime startdate, DateTime enddate,int id)
        {
            if (startdate.Date.ToString("MM/dd/yyyy") == "01/01/0001" || enddate.Date.ToString("MM/dd/yyyy") == "01/01/0001")
            {
                return NotFound();
            }
            if (startdate.Date.ToString("yyyy") != DateTime.Today.ToString("yyyy") || enddate.Date.ToString("yyyy") != DateTime.Today.ToString("yyyy"))
            {
                return NotFound();
            }
            PhysicianTimesheetViewModel physicianTimesheetViewModel = _doctor.GetTimesheetDetails(startdate, enddate,id);
            if (physicianTimesheetViewModel == null)
            {
                return NotFound();
            }
            return View("/Views/Doctor/TimeSheet.cshtml", physicianTimesheetViewModel);
        }
        public IActionResult ApproveTimesheet(decimal totalamount,decimal bonusamount,string desc, int id)
        {
            bool isApproved = _admin.ApproveTimesheet(totalamount, bonusamount, desc, id);
            if(isApproved)
            {
                TempData["success"] = "Timesheet Aprooved Successfully!!";
            }
            else
            {
                TempData["error"] = "Timesheet could not be Aprooved!!";
            }
            return Json(new { isApproved = isApproved });
        }
    }
}
