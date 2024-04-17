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
        private readonly Dictionary<string, string> mappingDictionary = new Dictionary<string, string>();
        public LoginController(IJwtService jwt,IAdmin admin,IPatient patient)
        {
            _jwt = jwt;
            _admin = admin;
            _patient = patient;
            mappingDictionary.Add("Provider Location", "ProviderLocation");
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
        /// <summary>
        /// It is Get Method for Login Page
        /// </summary>
        /// <returns></returns>
        public IActionResult PatientLogin()
        {
            return View();
        }
        /// <summary>
        /// It is post Method for Login Page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PatientLogin(LoginViewModel model)
        {
            

            if (ModelState.IsValid)
            {
                var result = _patient.Login(model);
                if (result == 2)
                {
                    TempData["success"] = "Loged in Successfully!!";
                    AspNetUser user = _patient.GetAspNetUserLogin(model.Email);
                    var jwtToken = _jwt.GenerateJWTAuthetication(user);
                    CookieModel cookieModel = _jwt.GetDetails(jwtToken);
                    Response.Cookies.Append("jwt", jwtToken);
                    if(cookieModel.role == "Patient")
                    {
                        return RedirectToAction("PatientDashboard","Patient");
                    }
                    else if(cookieModel.role == "Admin")
                    {
                        mappingDictionary.Add("My Profile","Profile");
                        if(cookieModel.menus.Contains("Dashboard"))
                        {
                            return RedirectToAction("Dashboard", "Admin");
                        }
                        else
                        {
                            return RedirectToAction(mappingDictionary[cookieModel.menus.Split(",")[0]], "Admin");
                        }
                    }
                    else if(cookieModel.role == "Provider")
                    {
                        mappingDictionary.Add("My Profile", "DoctorProfile");
                        if (cookieModel.menus.Contains("Dashboard"))
                        {
                            return RedirectToAction("Dashboard", "Admin");
                        }
                        else
                        {
                            return RedirectToAction(mappingDictionary[cookieModel.menus.Split(",")[0]], "Doctor");
                        }
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
                else if (result == 6)
                {
                    TempData["error"] = "Your account is currently is not active!! Please Contact Your Admin.";
                }
                else if (result == 7)
                {
                    TempData["error"] = "Your account is deleted!! Please Contact Your Admin.";
                }
                else
                {
                    TempData["error"] = "There was some issue in Log in!!";
                }
            }
            return View();
        }
        /// <summary>
        /// It is Get method for Forgot Password Page
        /// </summary>
        /// <returns></returns>
        public IActionResult ForgotPassword()
        {
            return View();
        }
        /// <summary>
        /// It is Post Method for Forgot Password Page
        /// </summary>
        /// <param name="forgotPasswordViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            Task<bool> isSent = _patient.SendResetLink(forgotPasswordViewModel.email);
            if(isSent.Result)
            {
                TempData["success"] = "Email Sent Successfully!!";
            }
            else
            {
                TempData["error"] = "This email does not exists!!";
            }
            return View();
        }
        /// <summary>
        /// It is Get Method for Reset Password Page
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public IActionResult ResetPassword(string Token)
        {
            PasswordReset passwordReset = _patient.GetResetPassword(Token);
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
        /// <summary>
        /// It is Post Method for Reset Password Page
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel modal)
        {
            if (ModelState.IsValid)
            {
                bool isReseted = _patient.ResetPassword(modal);
                if (isReseted)
                {
                    TempData["success"] = "Password Reseted successfully!!!";
                    return RedirectToAction("PatientLogin");

                }
                else
                {
                    TempData["error"] = "Password could not be Reseted!!!";
                }
            }
            return View(modal);
        }

        /// <summary>
        /// It is method for logging out from an account
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            if (Request.Cookies["jwt"] != null)
            {
                var myCookie = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(-1d) // Set the expiry date to yesterday
                };
                Response.Cookies.Append("jwt", "", myCookie);
            }
            return Json(new { isLogout = true });
        }
        /// <summary>
        /// It is Get method of Register Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Register(int id)
        {
            RegisterViewModel modal = new RegisterViewModel();
            AspNetUser aspNetUser = _patient.GetAspNetUserById(id);
            modal.Id = id;
            modal.Email = aspNetUser.Email;
            return View(modal);
        }
        /// <summary>
        /// It is post method for Register Page
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel modal)
        {
            if (ModelState.IsValid)
            {
                var isRegistered = _patient.Register(modal);
                if (isRegistered)
                {
                    TempData["success"] = "Registered successfully!!!";
                    return RedirectToAction("PatientLogin");
                }
                else
                {
                    TempData["error"] = "Patient could not be registered!!!";
                }
                return View();
            }
            return View(modal);
        }
        /// <summary>
        /// It is Get method for AccessDenied Page
        /// </summary>
        /// <returns></returns>
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
