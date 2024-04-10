using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
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
    [CustomAuthorize("Provider")]
    public class DoctorController : Controller
    {
        private readonly IAdmin _admin;
        private readonly IPatient _patient;
        private readonly IJwtService _jwt;
        private readonly IHttpContextAccessor _context;
        private readonly IDoctor _doctor;
        private readonly IViewRenderService _viewRender;

        public DoctorController(IAdmin admin, IJwtService jwt, IPatient patient, IHttpContextAccessor context,IDoctor doctor, IViewRenderService viewRender)
        {
            _admin = admin;
            _jwt = jwt;
            _patient = patient;
            _context = context;
            _doctor = doctor;
            _viewRender = viewRender;
        }

        [CustomAuthorize("Provider", "My Profile")]
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

        public IActionResult EncounterForm(int? id)
        {
            EncounterFormViewModel encounterFormViewModel = _admin.GetEncounterFormDetails((int)id);
            return View(encounterFormViewModel);
        }

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

        public IActionResult MySchedule()
        {
            SchedulingViewModel schedulingViewModel = _admin.GetAllShiftDetails(-1);
            return View(schedulingViewModel);
        }
        /// <summary>
        /// 
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

        public IActionResult VModel(int id)
        {
            EncounterFormViewModel encounterFormViewModel = _admin.GetEncounterFormDetails((int)id);
            return PartialView("_EncounterForm",encounterFormViewModel);
        }
    }
}
