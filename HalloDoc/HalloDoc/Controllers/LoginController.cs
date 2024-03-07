using DocumentFormat.OpenXml.Spreadsheet;
using HalloDoc.Repository.Interface;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.Controllers
{
    public class LoginController : Controller
    {
        private readonly IJwtService _jwt;
        private readonly IAdmin _admin;
        private readonly IPatient _patient;
        public LoginController(IJwtService jwt,IAdmin admin,IPatient patient)
        {
            _jwt = jwt;
            _admin = admin;
            _patient = patient;
        }

        public IActionResult PatientLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PatientLogin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _patient.login(model);
                if (result == 2)
                {
                    TempData["success"] = "Loged in Successfully!!";
                    AspNetUser user = _patient.getAspNetUser(model.Email);
                    var jwtToken = _jwt.GenerateJWTAuthetication(user);
                    CookieModel cookieModel = _jwt.getDetails(jwtToken);
                    Response.Cookies.Append("jwt", jwtToken);
                    if(cookieModel.role == "Patient")
                    {
                        return RedirectToAction("PatientDashboard","Patient");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                }
                else if (result == 3)
                {
                    TempData["error"] = "Incorrect Password!!";
                }
                else if (result == 4)
                {
                    TempData["error"] = "Incorrect Username!!";
                }
                else
                {
                    TempData["error"] = "There was some issue in Log in!!";
                }
            }
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            bool isSent = _patient.sendResetLink(forgotPasswordViewModel.email);
            if(isSent)
            {
                TempData["success"] = "Email Sent Successfully!!";
            }
            else
            {
                TempData["error"] = "This email does not exists!!";
            }
            return View();
        }

        public IActionResult ResetPassword(string Token)
        {
            PasswordReset passwordReset = _patient.getResetPassword(Token);
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
            resetPasswordViewModel.Token = Token;
            return View(resetPasswordViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel modal)
        {
            if (ModelState.IsValid)
            {
                bool isReseted = _patient.resetPassword(modal);
                if (isReseted)
                {
                    TempData["success"] = "Password Reseted successfully!!!";
                }
                else
                {
                    TempData["error"] = "Password could not be Reseted!!!";
                }
            }
            return View(modal);
        }


        public IActionResult Logout()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                IsEssential = true
            };
            Response.Cookies.Delete("jwt", cookieOptions);
            return Json(new { isLogout = true });
        }

        public IActionResult Register(int id)
        {
            RegisterViewModel modal = new RegisterViewModel();
            AspNetUser aspNetUser = _patient.getAspNetUserById(id);
            modal.Id = id;
            modal.Email = aspNetUser.Email;
            return View(modal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel modal)
        {
            if (ModelState.IsValid)
            {
                var isRegistered = _patient.register(modal);
                if (isRegistered)
                {
                    TempData["success"] = "Registered successfully!!!";
                }
                else
                {
                    TempData["error"] = "Patient could not be registered!!!";
                }
                return View();
            }
            return View(modal);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
