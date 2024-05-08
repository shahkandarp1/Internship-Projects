using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using HalloDoc.Repository.Auth;
using HalloDoc.Repository.Interface;
using HalloDoc.ViewModels;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Rotativa.AspNetCore;

namespace HalloDoc.Controllers
{
    [CustomAuthorize("Admin,Provider")]
    public class DoctorController : Controller
    {
        private readonly IAdmin _admin;
        private readonly IPatient _patient;
        private readonly IJwtService _jwt;
        private readonly IHttpContextAccessor _context;
        private readonly IDoctor _doctor;

        public DoctorController(IAdmin admin, IJwtService jwt, IPatient patient, IHttpContextAccessor context,IDoctor doctor)
        {
            _admin = admin;
            _jwt = jwt;
            _patient = patient;
            _context = context;
            _doctor = doctor;
        }

        [CustomAuthorize("Provider", "My Profile")]
        /// <summary>
        /// It is the get method of Doctor Profile Page
        /// </summary>
        /// <returns></returns>
        public IActionResult DoctorProfile()
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "DoctorProfile",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            PhysicianAccountViewModel physicianAccountViewModel = _admin.GetPhysicianDetails(cookieModel.userId, adminNavbarViewModel);
            return View("/Views/Admin/EditPhysician.cshtml", physicianAccountViewModel);
        }
        /// <summary>
        /// This method will accept the case of the specified request Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult AcceptCase(int id)
        {
            bool isAccepted = _doctor.AcceptCase(id);
            if (isAccepted)
            {
                TempData["success"] = "Case Accepted Successfully!!";
            }
            else
            {
                TempData["error"] = "Case Could not be Accepted!!";
            }
            return RedirectToAction("Dashboard", "Admin");
        }
        /// <summary>
        /// This method will send the mail to all the admins for editing physicians profile
        /// </summary>
        /// <param name="physicianAccountViewModel"></param>
        /// <returns></returns>
        public IActionResult RequestAdmin(PhysicianAccountViewModel physicianAccountViewModel)
        {
            Task<bool> isSent = _doctor.RequestAdmin(physicianAccountViewModel);
            if (isSent.Result)
            {
                TempData["success"] = "Mail Sent Successfully!!";
            }
            else
            {
                TempData["error"] = "Mail Could not be sent!!";
            }
            return RedirectToAction("Dashboard","Admin");
        }
        /// <summary>
        /// This method will update type of care specified by physician
        /// </summary>
        /// <param name="adminDashboardViewModel"></param>
        /// <returns></returns>
        public IActionResult TypeOfCare(AdminDashboardViewModel adminDashboardViewModel)
        {
            bool isAssigned = _doctor.TypeOfCare(adminDashboardViewModel);
            if (isAssigned)
            {
                TempData["success"] = "Type Of Care Updated!!";
            }
            else
            {
                TempData["error"] = "Type Of Care could not be Updated!!";
            }
            return RedirectToAction("Dashboard","Admin");
        }
        /// <summary>
        /// This method will change status from MDOnSite to Conclude
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult HouseCall(int? id)
        {
            bool isUpdated = _doctor.HouseCall(id);
            if (isUpdated)
            {
                TempData["success"] = "Status Updated Successfully!!";
            }
            else
            {
                TempData["error"] = "Status not be Updated!!";
            }
            return RedirectToAction("Dashboard", "Admin");
        }
        /// <summary>
        /// It is get method for Encounter Form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult EncounterForm(int? id)
        {
            EncounterFormViewModel encounterFormViewModel = _admin.GetEncounterFormDetails((int)id);
            return View(encounterFormViewModel);
        }
        /// <summary>
        /// This method is the POST method for Encounter Form and will update Encounter Form
        /// </summary>
        /// <param name="encounterFormViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EncounterForm(EncounterFormViewModel encounterFormViewModel)
        {
            bool isUpdated = _admin.UpdateEncounterForm(encounterFormViewModel);
            if (isUpdated && encounterFormViewModel.isFinalized == true)
            {
                TempData["success"] = "Encounter Form Finalized Successfully!!";
                return RedirectToAction("Dashboard", "Admin");
            }
            else if(isUpdated)
            {
                TempData["success"] = "Encounter Form Updated Successfully!!";
            }
            else
            {
                TempData["error"] = "Encounter Form could not be Updated!!";
            }
            return RedirectToAction("EncounterForm", new { id = encounterFormViewModel.RequestId });
        }
        [CustomAuthorize("Provider", "My Schedule")]
        /// <summary>
        /// This method is get method for My Schedule Page
        /// </summary>
        /// <returns></returns>
        public IActionResult MySchedule()
        {
            SchedulingViewModel schedulingViewModel = _admin.GetAllShiftDetails(-1);
            return View(schedulingViewModel);
        }
        /// <summary>
        /// It is get method for Conclude Care Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ConcludeCare(int id)
        {
            ConcludeCareViewModel concludeCareViewModel = _doctor.GetConcludeCare(id);
            if (concludeCareViewModel == null)
            {
                return NotFound();
            }
            return View(concludeCareViewModel);
        }
        /// <summary>
        /// This method is the POST method for Conclude Care page
        /// </summary>
        /// <param name="concludeCareViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConcludeCare(ConcludeCareViewModel concludeCareViewModel)
        {
            if(ModelState.IsValid)
            {
                int isConcluded = _doctor.ConcludeCare(concludeCareViewModel);
                if (isConcluded == 1)
                {
                    TempData["success"] = "Case Concluded Successfully!!";
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if(isConcluded == 2)
                {
                    TempData["error"] = "Encounter Form is not finalized yet!!";
                }
                else
                {
                    TempData["error"] = "Case could not be concluded!!";
                }
            }
            return RedirectToAction("ConcludeCare", new { id = concludeCareViewModel.RequestId });
        }
        /// <summary>
        /// This method will transfer case back from physician to admin
        /// </summary>
        /// <param name="adminDashboardViewModel"></param>
        /// <returns></returns>
        public IActionResult TransferCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            bool isTransfered = _doctor.TransferCase(adminDashboardViewModel);
            if (isTransfered)
            {
                TempData["success"] = "Request Transferred Successfully!!";
            }
            else
            {
                TempData["error"] = "Request could not be transferred!!";
            }
            return RedirectToAction("Dashboard","Admin");
        }
        /// <summary>
        /// This method will download encounter form once it is finalized
        /// </summary>
        /// <param name="adminDashboardViewModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> DownloadEncounterForm(AdminDashboardViewModel adminDashboardViewModel)
        {
            var model =  _admin.GetEncounterFormDetails((int)adminDashboardViewModel.RequestId);

            var request = _admin.GetRequest((int)adminDashboardViewModel.RequestId);

            return new ViewAsPdf("../Shared/_EncounterForm", model)
            {
                FileName = $"EncounterReport-{request.ConfirmationNumber}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageMargins = { Left = 20, Right = 20 }
            };
        }
        /// <summary>
        /// This method will update doctors recent location i.e. lat and long once he/she logs into the system
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public IActionResult UpdatePhysicianLatitudeLongitude(decimal lat, decimal lng,string address)
        {
            bool isUpdated = _doctor.UpdatePhysicianLatitudeLongitude(lat, lng, address);
            return Json(new { isUpdated = isUpdated });
        }
        public IActionResult Invoicing()
        {
            PhysicianInvoicingViewModel physicianInvoicingViewModel = _doctor.GetPhysicianInvoicingDetails(null, null);
            return View(physicianInvoicingViewModel);
        }

        public IActionResult InvoicingTimesheetTable(DateTime startdate, DateTime enddate)
        {
            PhysicianInvoicingViewModel physicianInvoicingViewModel = _doctor.GetPhysicianInvoicingDetails(startdate, enddate);
            return PartialView("_PhysicianInvoicingDetailTable", physicianInvoicingViewModel);
        }

        public IActionResult InvoicingReimbursementTable(DateTime startdate, DateTime enddate)
        {
            PhysicianInvoicingViewModel physicianInvoicingViewModel = _doctor.GetPhysicianInvoicingDetails(startdate, enddate);
            return PartialView("_PhysicianInvoicingReimbursementTable", physicianInvoicingViewModel);
        }

        public IActionResult CheckFinalize(DateTime startdate, DateTime enddate)
        {
            PhysicianInvoicingViewModel physicianInvoicingViewModel = _doctor.GetPhysicianInvoicingDetails(startdate, enddate);
            return Json(new { isFinalized = physicianInvoicingViewModel.timesheetReimbursement?.IsFinalized[0] ?? false });
        }
        public IActionResult Timesheet(DateTime startdate, DateTime enddate)
        {
            if(startdate.Date.ToString("MM/dd/yyyy") == "01/01/0001" || enddate.Date.ToString("MM/dd/yyyy") == "01/01/0001")
            {
                return NotFound();
            }
            if(startdate.Date.ToString("yyyy") != DateTime.Today.ToString("yyyy") || enddate.Date.ToString("yyyy") != DateTime.Today.ToString("yyyy"))
            {
                return NotFound();
            }
            PhysicianTimesheetViewModel physicianTimesheetViewModel = _doctor.GetTimesheetDetails(startdate, enddate);
            if (physicianTimesheetViewModel == null)
            {
                return NotFound();
            }
            return View(physicianTimesheetViewModel);
        }
        [HttpPost]
        public IActionResult TimeSheet(PhysicianTimesheetViewModel physicianTimesheetViewModel)
        {
            bool isUpdated = _doctor.UpdateTimeSheet(physicianTimesheetViewModel);
            if(isUpdated)
            {
                TempData["success"] = "Timesheet updated Successfully!!";
            }
            else
            {
                TempData["error"] = "Timesheet could not be updated!!";
            }
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);
            if (cookieModel.role == "Admin")
            {
                return RedirectToAction("Invoicing","Admin");
            }
            else
            {
                return RedirectToAction("Invoicing", "Doctor");
            }
        }

        public IActionResult TimeSheetReimbursement(IFormFile file,DateTime? date,int? id,string? item,int? amount, DateTime? startdate, DateTime? enddate,int? physicianId)
        {
            Task<bool> isCreated = _doctor.UpdateTimeSheetReimbursement(file, date, id, item, amount,startdate,enddate,physicianId);
            if (isCreated.Result)
            {
                TempData["success"] = "Timesheet updated Successfully!!";
            }
            else
            {
                TempData["error"] = "Timesheet could not be updated!!";
            }
            return RedirectToAction("TimeSheet",new { startdate = startdate,enddate = enddate });
        }

        public IActionResult DeleteTimesheetReimbursement(int id)
        {
            bool isDeleted = _doctor.DeleteTimesheetReimbursement(id);
            if(isDeleted)
            {
                TempData["success"] = "Timesheet Deleted Successfully!!";
            }
            else
            {
                TempData["error"] = "Timesheet could not be deleted!!";
            }
            return Json(new { isDeleted });
        }

        public IActionResult FinalizeTimesheet(int id)
        {
            bool isFinalized = _doctor.FinalizeTimesheet(id);
            if(isFinalized)
            {
                TempData["success"] = "Timesheet Finalized Successfully!!";
            }
            else
            {
                TempData["error"] = "Timesheet could not be Finalized!!";
            }
            return Json(new { isFinalized });
        }
    }
}
