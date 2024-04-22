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
        /// <summary>
        /// It is Get Method for Patient Site Page
        /// </summary>
        /// <returns></returns>
        public IActionResult PatientSite()
        {
            return View();
        }
        /// <summary>
        /// It is Get Method for Submit Request Page
        /// </summary>
        /// <returns></returns>
        public IActionResult SubmitRequest()
        {
            return View();
        }
        /// <summary>
        /// It is Get Method for Patient Request Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult PatientForm()
        {
            return View();
        }
        /// <summary>
        /// It is post Method for Patient Request Form
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PatientForm(PatientRequestViewModel modal)
        {
            if (ModelState.IsValid)
            {
                bool isVerified = _admin.VerifyRegion(modal.State);
                if (!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(modal);
                }

                bool isValidRole = _admin.CheckUserRole(modal.Email);
                if(!isValidRole)
                {
                    TempData["error"] = "Only patients can create request!!!";
                    return View(modal);
                }

                bool isBlocked = _admin.VerifyBlock(modal.Email);
                if (isBlocked)
                {
                    TempData["error"] = "Patient with this email is blocked!!!";
                    return View(modal);
                }

                var isCreated = _patient.PatientRequest(modal);
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
        /// <summary>
        /// It will check whether the specified Email already exists in AspNetUsers Table
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public IActionResult PatientCheck(string email)
        {
            if (email == null)
            {
                return View();
            }
            var existingUser = _patient.GetAspNetUser(email);
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
        /// <summary>
        /// It is Get Method for Business Request Page
        /// </summary>
        /// <returns></returns>
        public IActionResult BusinessForm()
        {
            return View();
        }
        /// <summary>
        /// It is Get Method for Family Request Form
        /// </summary>
        /// <returns></returns>
        public IActionResult FamilyForm()
        {
            return View();
        }
        /// <summary>
        /// It is Post Request for Family Request Form
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FamilyForm(FamilyRequestViewModel modal)
        {
            if (ModelState.IsValid)
            {
                bool isVerified = _admin.VerifyRegion(modal.State);
                if (!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(modal);
                }

                bool isValidRole = _admin.CheckUserRole(modal.Email);
                if (!isValidRole)
                {
                    TempData["error"] = "Only patients can create request!!!";
                    return View(modal);
                }

                bool isBlocked = _admin.VerifyBlock(modal.Email);
                if (isBlocked)
                {
                    TempData["error"] = "Patient with this email is blocked!!!";
                    return View(modal);
                }

                var isCreated = _patient.FamilyRequest(modal);
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

        /// <summary>
        /// It is Post Request for Concierge Request Form
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConciergeForm(ConciergeRequestViewModel modal)
        {
            if (ModelState.IsValid)
            {
                bool isVerified = _admin.VerifyRegion(modal.ConciergeState);
                if (!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(modal);
                }

                bool isValidRole = _admin.CheckUserRole(modal.Email);
                if (!isValidRole)
                {
                    TempData["error"] = "Only patients can create request!!!";
                    return View(modal);
                }

                bool isBlocked = _admin.VerifyBlock(modal.Email);
                if (isBlocked)
                {
                    TempData["error"] = "Patient with this email is blocked!!!";
                    return View(modal);
                }

                var isCreated = _patient.ConciergeRequest(modal);
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
        /// <summary>
        /// It is Post Request for Business Request Form
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BusinessForm(BusinessRequestViewModel modal)
        {
            if (ModelState.IsValid)
            {
                bool isVerified = _admin.VerifyRegion(modal.State);
                if (!isVerified)
                {
                    TempData["error"] = "We are currently not serving this region!!!";
                    return View(modal);
                }

                bool isValidRole = _admin.CheckUserRole(modal.Email);
                if (!isValidRole)
                {
                    TempData["error"] = "Only patients can create request!!!";
                    return View(modal);
                }

                bool isBlocked = _admin.VerifyBlock(modal.Email);
                if (isBlocked)
                {
                    TempData["error"] = "Patient with this email is blocked!!!";
                    return View(modal);
                }

                var isCreated = _patient.BusinessRequest(modal);
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
        /// <summary>
        /// It is Get method for Concierge Form
        /// </summary>
        /// <returns></returns>
        public IActionResult ConciergeForm()
        {
            return View();
        }
    }
}
