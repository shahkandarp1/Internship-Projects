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

namespace HalloDoc.Controllers
{

    [CustomAuthorize("Admin")]
    public class AdminController : Controller
    {
        private readonly IAdmin _admin;
        private readonly IPatient _patient;
        private readonly IJwtService _jwt;
        

        public AdminController(IAdmin admin,IJwtService jwt, IPatient patient)
        {
            _admin = admin;
            _jwt = jwt;
            _patient = patient;
        }
        /// <summary>
        /// Get Method for Admin Dashboard
        /// </summary>
        /// <returns></returns>
        public IActionResult Dashboard()
        {
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("New",null,null,-1);
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
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("New", search, requestor, region,page,pageSize);
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
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("Pending", search, requestor, region,page,pageSize);
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
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("Active", search, requestor, region, page, pageSize);
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
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("Conclude", search, requestor, region, page, pageSize);
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
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("ToClose", search, requestor, region,page,pageSize);
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
            AdminDashboardViewModel adminDashboardViewModel = _admin.adminDashboardContent("Unpaid", search, requestor, region, page, pageSize);
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }
        /// <summary>
        /// Downloads Excel File with all request data
        /// </summary>
        /// <returns></returns>
        public IActionResult ExportAll()
        {
            MemoryStream memoryStream = _admin.exportAll();
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
            AdminDashboardViewModel viewmodel = _admin.adminDashboardContent(model.status, model.search, model.requestor, model.RegionId, (int)model.CurrentPage, (int)model.PageSize);
            MemoryStream memoryStream = _admin.export(viewmodel);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Data-{model.status}.xlsx");
        }
        /// <summary>
        /// Get Method for View Case Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ViewCase(int id)
        {
            
            ViewCaseViewModel viewCaseViewModel= _admin.viewCase(id);
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
            bool status = _admin.viewCase(model);
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
            bool isUpdated = _admin.cancelRequest(viewCaseViewModel.RequestId,viewCaseViewModel.Admin_notes,viewCaseViewModel.CaseTag);
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
            Task<bool> isSent = _admin.sendLink(dashboardViewModel);
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
            PatientRequestViewModel patientRequestViewModel = _admin.createRequest();
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

                Task<bool> requestCreated = _admin.createRequest(patientRequestViewModel);
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
        /// <summary>
        /// Get Method of View Note Action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ViewNotes(int id)
        {
            ViewNotesViewModel viewNotesViewModel = _admin.viewNotes(id);
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
            bool isUpdated = _admin.updateAdminNotes(viewNotesViewModel);
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
            ViewDocumentModal viewDocumentModal = _admin.viewUploads(id);
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
            Task<bool> isFileUploaded = _admin.fileUpload(file,id);
            return Json(new { isFileUploaded = isFileUploaded });
        }
        /// <summary>
        /// This method will delete single file in View Uploads Action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteSingle(int id)
        {
            int requestid = _admin.deleteSingleFile(id);
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
            var result = _admin.downloadMultipleFiles(viewDocumentModal);
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
            int requestid = _admin.deleteAllFile(filename);
            return Json(new { isDeleted = true });
        }
        /// <summary>
        /// This method will send selected file as a attatchment in the mail to the patient in View Uploads Action
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public IActionResult SendMailDocuments([FromForm] string filename)
        {
            Task<bool> sentMail = _admin.sendDocumentsMail(filename);
            return Json(new { isSent = sentMail.Result });
        }
        /// <summary>
        /// This method will return all the physician present in a specific region in json format
        /// </summary>
        /// <param name="regionid"></param>
        /// <returns></returns>
        public IActionResult GetPhysician(int regionid)
        {
            List<Physician> physician = _admin.getPhysician(regionid);
            return Json(new { data=physician});
        }
        /// <summary>
        /// It is a post method that will assign case of a specific request to the specified physician
        /// </summary>
        /// <param name="adminDashboardViewModel"></param>
        /// <returns></returns>
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
            return RedirectToAction("Dashboard");
        }
        /// <summary>
        /// It is a post method that will transfer case of a specific request to the specified physician
        /// </summary>
        /// <param name="adminDashboardViewModel"></param>
        /// <returns></returns>
        public IActionResult TransferCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            bool isExists = _admin.isSamePhysician(adminDashboardViewModel);
            if(isExists)
            {
                TempData["error"] = "This doctor already has the same case!!";
                return RedirectToAction("Dashboard");
            }

            bool isTransfered = _admin.transferCase(adminDashboardViewModel);
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
            Task<bool> isSent = _admin.sendAgreement(adminDashboardViewModel);
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
            bool isBlocked = _admin.blockCase(adminDashboardViewModel);
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
        /// <summary>
        /// This is the Get method of Orders Action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Orders(int id)
        {
            OrdersViewModel ordersViewModel = _admin.orders(id);
            return View(ordersViewModel);
        }
        /// <summary>
        /// It will return all Health Professionals of the specified Health Professional Type and will return data in Json 
        /// </summary>
        /// <param name="professionid"></param>
        /// <returns></returns>
        public IActionResult GetBusiness(int professionid)
        {
            var business = _admin.getBusiness(professionid);
            return Json(new { data=business });
        }
        /// <summary>
        /// It will return data of a specific Health Professionals and will return data in Json
        /// </summary>
        /// <param name="businessid"></param>
        /// <returns></returns>
        public IActionResult GetBusinessData(int businessid)
        {
            var business = _admin.getBusinessData(businessid);
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
        /// <summary>
        /// It is a Get Method for Admin Profile Page
        /// </summary>
        /// <returns></returns>
        public IActionResult Profile()
        {
            AdminProfileViewModel adminProfileViewModel = _admin.getAdmin();
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
        /// <summary>
        /// This method will reset password for specific admin
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
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
        /// <summary>
        /// It is a Get method for Encounter Form for specific request
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult EncounterForm(int id)
        {
            EncounterFormViewModel encounterFormViewModel = _admin.getEncounterFormDetails(id);
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
            bool isUpdated = _admin.updateEncounterForm(encounterFormViewModel);
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
            CloseCaseViewModel closeCaseViewModel = _admin.getCloseCase(id);
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
            bool isUpdated = _admin.updateCloseCase(closeCaseViewModel);
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
            bool isClosed = _admin.closeCase(id);
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
        /// <summary>
        /// It is a get method for Provider Page
        /// </summary>
        /// <returns></returns>
        public IActionResult Provider()
        {
            ProviderViewModel providerViewModel = _admin.getProviderPageDetails(-1);
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
            ProviderViewModel providerViewModel = _admin.getProviderPageDetails(id,page,pageSize);
            return PartialView("_AdminProviderTable", providerViewModel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public IActionResult ChangeNotification(int id,bool update)
        {
            bool isUpdated = _admin.changeNotification(id,update);
            return Json(new {isUpdated = isUpdated });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerViewModel"></param>
        /// <returns></returns>
        public IActionResult ContactProvider(ProviderViewModel providerViewModel)
        {
            Task<bool> isSent = _admin.contactProvider(providerViewModel);
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

        public IActionResult CreatePhysician()
        {
            PhysicianAccountViewModel physicianAccountViewModel = _admin.getCreatePhysicianDetails();
            return View(physicianAccountViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePhysician(PhysicianAccountViewModel physicianAccountViewModel)
        {
            List<Role> roles = _admin.getPhysicianRoles();
            physicianAccountViewModel.roles = roles;
            if (physicianAccountViewModel.Password == null || physicianAccountViewModel.Password == "")
            {
                ModelState.AddModelError("Password", "Please Enter Password");
                return View(physicianAccountViewModel);
            }
            if(physicianAccountViewModel.role_id == -1)
            {
                ModelState.AddModelError("role_id", "Please Select Role");
                return View(physicianAccountViewModel);
            }
            if (physicianAccountViewModel.Signature == null || physicianAccountViewModel.Photo == null)
            {
                TempData["error"] = "Please upload neccessarry documents!!";
                return View(physicianAccountViewModel);
            }

            if (ModelState.IsValid)
            {
                if(_patient.getAspNetUser(physicianAccountViewModel.Email) != null)
                {
                    TempData["error"] = "This Email Id Already Exists!!";
                    return View(physicianAccountViewModel);
                }

                Task<bool> isCreated = _admin.createPhysician(physicianAccountViewModel);
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

        public IActionResult EditPhysician(int id)
        {
            PhysicianAccountViewModel physicianAccountViewModel = _admin.getPhysicianDetails(id);
            return View(physicianAccountViewModel);
        }

        public IActionResult FileUploadPhysician([FromForm] IFormFile file, [FromForm] int id, [FromForm] string name)
        {
            Task<bool> isUploaded = _admin.fileUploadPhysician(file, id, name);
            return Json(new { isUploaded = isUploaded.Result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPhysician(PhysicianAccountViewModel physicianAccountViewModel)
        {
            Task<bool> isUpdated = _admin.updatePhysician(physicianAccountViewModel);
            if (isUpdated.Result)
            {
                TempData["success"] = "Information Updated Successfully!!";
            }
            else
            {
                TempData["error"] = "Information could not be Updated!!";
            }
            return RedirectToAction("EditPhysician",new { id=physicianAccountViewModel.PhysicianId });
        }

        public IActionResult ResetPasswordPhysician(string password, int id)
        {
            int isreset = 0;
            if(password == null)
            {
                isreset = 1;
                return Json(new { isReseted = isreset });
            }

            bool isReseted = _admin.resetPasswordPhysician(password, id);
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

        public IActionResult DeletePhysician(int id)
        {
            bool isDeleted = _admin.deletePhysician(id);
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

        public IActionResult PatientHistory()
        {
            PatientHistoryViewModel patientHistoryViewModel = _admin.getAllPatients(null,null,null,null);
            return View(patientHistoryViewModel);
        }

        public IActionResult PatientHistoryTable(string? firstname,string? lastname,string? email,string? phone,int page=1,int pageSize = 10)
        {
            PatientHistoryViewModel patientHistoryViewModel = _admin.getAllPatients(firstname,lastname,email,phone,page,pageSize);
            return PartialView("_PatientHistoryTable", patientHistoryViewModel);
        }

        public IActionResult PatientRecord(int id, int page = 1, int pageSize = 10)
        {
            PatientHistoryViewModel patientHistoryViewModel = _admin.getAllPatientRecords(id,page,pageSize);
            return View(patientHistoryViewModel);
        }
        public IActionResult PatientRecordTable(int id, int page = 1, int pageSize = 10)
        {
            PatientHistoryViewModel patientHistoryViewModel = _admin.getAllPatientRecords(id,page,pageSize);
            return PartialView("_PatientRecordTable", patientHistoryViewModel);
        }

        public IActionResult BlockHistory()
        {
            BlockHistoryViewModel blockHistoryViewModel = _admin.getBlockHistoryData(null, null, null, null);
            return View(blockHistoryViewModel);
        }

        public IActionResult BlockHistoryData(string? name, DateTime? date, string? email, string? phone, int page = 1, int pageSize = 10)
        {
            BlockHistoryViewModel blockHistoryViewModel = _admin.getBlockHistoryData(name, date, email, phone, page, pageSize);
            return PartialView("_RequestBlockHistory", blockHistoryViewModel);
        }

        public IActionResult ToggleActive(int blockrequestid,bool value)
        {
            bool isToggled = _admin.toggleActive(blockrequestid, value);
            return Json(new { isToggled = isToggled });
        }

        public IActionResult RestoreBlock(int blockrequestid)
        {
            bool isRestored = _admin.restoreBlock(blockrequestid);
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

        public IActionResult SearchRecord()
        {
            SearchRecordViewModel searchRecordViewModel = _admin.getSearchedData(null, null, null, null, null, null, null, null);
            return View(searchRecordViewModel);
        }

        public IActionResult SearchRecordTable(int? status, string? name, int? requesttypeid, DateTime? fromdos, DateTime? todos, string? providername, string? email, string? phonenumber, int page = 1, int pageSize = 10)
        {
            SearchRecordViewModel searchRecordViewModel = _admin.getSearchedData(status, name, requesttypeid, fromdos, todos, providername, email, phonenumber, page, pageSize);
            return PartialView("_SearchRecordTable",searchRecordViewModel);
        }

        public IActionResult DeleteRequest(int id)
        {
            bool isDeleted = _admin.deleteRequest(id);
            if(isDeleted)
            {
                TempData["success"] = "Request Deleted Successfully!!";
            }
            else{
                TempData["error"] = "Request could not be Deleted!!";
            }
            return RedirectToAction("SearchRecord");
        }

        public IActionResult ExportSearchedData(SearchRecordViewModel model)
        {
            SearchRecordViewModel searchRecordViewModel = _admin.getSearchedData(model.status,  model.name, model.requesttypeid, model.fromdos, model.todos, model.providername, model.email, model.phonenumber, (int)model.CurrentPage, (int)model.PageSize);
            MemoryStream memoryStream = _admin.exportSearchedData(searchRecordViewModel);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Filtered-Data.xlsx");
        }

        public IActionResult AccountAccess()
        {
            AccountAccessViewModel accountAccessViewModel = _admin.getAllRolesDetails();
            return View(accountAccessViewModel);
        }

        public IActionResult AccountAccessTable(int page = 1, int pageSize = 10)
        {
            AccountAccessViewModel accountAccessViewModel = _admin.getAllRolesDetails();
            return PartialView("_AccountAccessTable",accountAccessViewModel);
        }

        public IActionResult CreateAccess()
        {
            AdminNavbarViewModel adminNavbarViewModel = _admin.getCreateAccessNavbar();
            return View(adminNavbarViewModel);
        }

        public IActionResult GetMenus(int? id)
        {
            List<Menu> menu = _admin.getMenus(id);
            return Json(new { data = menu });
        }

        public IActionResult CreateRole(string? menus,string? role_name,int? account_type)
        {
            bool isCreated = _admin.createRole(menus, role_name, account_type);
            if(isCreated)
            {
                TempData["success"] = "Role Created Successfully!!";
            }
            else
            {
                TempData["error"] = "Role could not be Created!!";
            }
            return Json(new { isCreated  = isCreated });
        }

        public IActionResult DeleteRole(int? id)
        {
            bool isDeleted = _admin.deleteRole(id);
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

        public IActionResult EditRole(int? id)
        {
            EditAccessViewModel editAccessViewModel = _admin.getRoleDetails(id);
            return View(editAccessViewModel);
        }

        [HttpPost]
        public IActionResult EditRole(int? id, string? menus, string? role_name, int? account_type)
        {
            bool isEditted = _admin.editRoleDetails(id,menus,role_name,account_type);
            if(isEditted)
            {
                TempData["success"] = "Role Editted Successfully!!";
            }
            else
            {
                TempData["error"] = "Role could not be Editted!!";
            }
            return Json(new { isEditted = isEditted });
        }

        public IActionResult EmailLog()
        {
            EmailLogViewModel emailLogViewModel = _admin.getEmailLogDetails(-1,null,null,null,null);
            return View(emailLogViewModel);
        }
        
        public IActionResult EmailLogTable(int? roleid, string? name, string? email, DateTime? createddate, DateTime? sentdate, int page = 1, int pageSize = 10)
        {
            EmailLogViewModel emailLogViewModel = _admin.getEmailLogDetails(roleid,name,email,createddate,sentdate,page,pageSize);
            return PartialView("_EmailLogTable",emailLogViewModel);
        }

        public IActionResult SMSLog()
        {
            EmailLogViewModel emailLogViewModel = _admin.getSMSLogDetails(-1, null, null, null, null);
            return View(emailLogViewModel);
        }

        public IActionResult SMSLogTable(int? roleid, string? name, string? phonenumber, DateTime? createddate, DateTime? sentdate, int page = 1, int pageSize = 10)
        {
            EmailLogViewModel emailLogViewModel = _admin.getSMSLogDetails(roleid, name, phonenumber, createddate, sentdate, page, pageSize);
            return PartialView("_SMSLogTable", emailLogViewModel);
        }

        public IActionResult Partners()
        {
            PartnerViewModal partnerViewModal = _admin.getPartnerDetails(null,-1);
            return View(partnerViewModal);
        }

        public IActionResult PartnersTable(string? name,int? id,int page = 1,int pageSize = 10)
        {
            PartnerViewModal partnerViewModal = _admin.getPartnerDetails(name,id,page,pageSize);
            return PartialView("_PartnersTable", partnerViewModal);
        }

        public IActionResult CreateBusiness()
        {
            BusinessViewModel businessViewModel = _admin.getBusinessNavbar();
            return View("Business",businessViewModel);
        }

        [HttpPost]
        public IActionResult AddBusiness(BusinessViewModel businessViewModel)
        {
            if(ModelState.IsValid)
            {

                bool isCreated = _admin.createBusiness(businessViewModel);
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

        public IActionResult EditBusiness(int id)
        {
            BusinessViewModel businessViewModel = _admin.getBusinessDetails(id);
            return View("Business", businessViewModel);
        }

        [HttpPost]
        public IActionResult EditBusiness(BusinessViewModel businessViewModel)
        {
            if (ModelState.IsValid)
            {

                bool isEditted = _admin.editBusiness(businessViewModel);
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

        public IActionResult DeleteBusiness(int id)
        {
            bool isDeleted = _admin.deleteBusiness(id);
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

        public IActionResult ProviderLocation()
        {
            ProviderLocationViewModel providerLocationViewModel = _admin.getProviderLocation();
            return View(providerLocationViewModel);
        }

        public IActionResult CreateAdmin()
        {
            AdminProfileViewModel adminProfileViewModel = _admin.getCreateAdminProfilePageDetails();
            return View(adminProfileViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAdmin(AdminProfileViewModel adminProfileViewModel)
        {
            List<Role> roles = _admin.getAdminRoles();
            adminProfileViewModel.roles = roles;
            if (adminProfileViewModel.Password == null || adminProfileViewModel.Password == "")
            {
                ModelState.AddModelError("Password", "Please Enter Password");
                return View(adminProfileViewModel);
            }
            if (adminProfileViewModel.role_id == -1)
            {
                ModelState.AddModelError("role_id", "Please Select Role");
                return View(adminProfileViewModel);
            }
            if(ModelState.IsValid)
            {
                if (_patient.getAspNetUser(adminProfileViewModel.Email) != null)
                {
                    TempData["error"] = "This Email Id Already Exists!!";
                    return View(adminProfileViewModel);
                }
                Task<bool> isCreated = _admin.createAdmin(adminProfileViewModel);
                if(isCreated.Result)
                {
                    TempData["success"] = "Admin Created Successfully!!";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    TempData["error"] = "Admin could not be Created!!";
                }
            }
            return View(adminProfileViewModel);

        }

        public IActionResult UserAccess()
        {
            var useraccess = _admin.GetUserAccessDetails(-1);
            return View(useraccess);
        }
        
        public IActionResult UserAccessTable(int? roleid,int page=1,int pageSize=10)
        {
            var useraccess = _admin.GetUserAccessDetails(roleid,page,pageSize);
            return PartialView("_UserAccessTable",useraccess);
        }

    }
}
