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
            AdminDashboardViewModel viewmodel = _admin.adminDashboardContent(model.status, model.search, model.requestor, model.RegionId, (int)model.CurrentPage, (int)model.PageSize);
            MemoryStream memoryStream = _admin.export(viewmodel);
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
            return RedirectToAction("ViewUploads",new { id = requestid });
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
            return Json(new { isSent = sentMail.Result });
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
            return RedirectToAction("Dashboard");
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

        public IActionResult EncounterForm(int id)
        {
            EncounterFormViewModel encounterFormViewModel = _admin.getEncounterFormDetails(id);
            return View(encounterFormViewModel);
        }

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

        public IActionResult CloseCase(int id)
        {
            CloseCaseViewModel closeCaseViewModel = _admin.getCloseCase(id);
            return View(closeCaseViewModel);
        }

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

        public IActionResult Provider()
        {
            ProviderViewModel providerViewModel = _admin.getProviderPageDetails(-1);
            return View(providerViewModel);
        }

        public IActionResult ProviderTable(int id = -1,int page=1,int pageSize = 10)
        {
            ProviderViewModel providerViewModel = _admin.getProviderPageDetails(id,page,pageSize);
            return PartialView("_AdminProviderTable", providerViewModel);
        }

        public IActionResult ChangeNotification(int id,bool update)
        {
            bool isUpdated = _admin.changeNotification(id,update);
            return Json(new {isUpdated = isUpdated });
        }
        
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
            if(physicianAccountViewModel.Signature == null || physicianAccountViewModel.Photo == null)
            {
                TempData["error"] = "Please upload neccessarry documents!!";
                return View(physicianAccountViewModel);
            }

            if(ModelState.IsValid)
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

    }
}
