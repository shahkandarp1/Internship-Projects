using HalloDoc.Repository.Auth;
using HalloDoc.Repository.Interface;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;

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

        public DoctorController(IAdmin admin, IJwtService jwt, IPatient patient, IHttpContextAccessor context,IDoctor doctor)
        {
            _admin = admin;
            _jwt = jwt;
            _patient = patient;
            _context = context;
            _doctor = doctor;
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
    }
}
