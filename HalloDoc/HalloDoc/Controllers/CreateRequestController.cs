using HalloDoc.Repository.Interface;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.Controllers
{
    public class CreateRequestController : Controller
    {

        private readonly IJwtService _jwt;
        private readonly IAdmin _admin;
        private readonly IPatient _patient;

        public CreateRequestController(IJwtService jwt, IAdmin admin, IPatient patient)
        {
            _jwt = jwt;
            _admin = admin;
            _patient = patient;
        }

        public IActionResult PatientSite()
        {
            return View();
        }

        public IActionResult SubmitRequest()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PatientForm()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PatientForm(PatientRequestViewModel modal)
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

                var isCreated = _patient.patientRequest(modal);
                if (isCreated.Result)
                {
                    TempData["success"] = "Request Created Successfully!!!";
                    return RedirectToAction("PatientSite");
                }
                else
                {
                    TempData["error"] = "Request could not be Created!!!";
                }
            }
            return View(modal);
        }

        public IActionResult PatientCheck(string email)
        {
            if (email == null)
            {
                return View();
            }
            var existingUser = _patient.getAspNetUser(email);
            bool isValidEmail;
            if (existingUser == null)
            {
                isValidEmail = false;
            }
            else
            {
                isValidEmail = true;
            }
            return Json(new { isValid = isValidEmail });
        }

        public IActionResult BusinessForm()
        {
            return View();
        }

        public IActionResult FamilyForm()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FamilyForm(FamilyRequestViewModel modal)
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

                var isCreated = _patient.familyRequest(modal);
                if (isCreated.Result)
                {
                    TempData["success"] = "Request Created Successfully!!!";
                    return RedirectToAction("PatientSite");
                }
                else
                {
                    TempData["error"] = "Request could not be Created!!!";
                }
            }
            return View(modal);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ConciergeForm(ConciergeRequestViewModel modal)
        {
            if (ModelState.IsValid)
            {
                bool isVerified = _admin.verifyRegion(modal.ConciergeState);
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

                var isCreated = _patient.conciergeRequest(modal);
                if (isCreated.Result)
                {
                    TempData["success"] = "Request Created Successfully!!!";
                    return RedirectToAction("PatientSite");
                }
                else
                {
                    TempData["error"] = "Request could not be Created!!!";
                }
            }
            return View(modal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> BusinessForm(BusinessRequestViewModel modal)
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

                var isCreated = _patient.businessRequest(modal);
                if (isCreated.Result)
                {
                    TempData["success"] = "Request Created Successfully!!!";
                    return RedirectToAction("PatientSite");
                }
                else
                {
                    TempData["error"] = "Request could not be Created!!!";
                }
            }

            return View(modal);
        }

        public IActionResult ConciergeForm()
        {
            return View();
        }
    }
}
