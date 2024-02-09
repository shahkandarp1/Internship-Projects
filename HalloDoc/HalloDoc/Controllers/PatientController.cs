using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HalloDoc.ViewModels;
using HalloDoc;
using System.Collections;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace HaloDocMVC.NET.Controllers
{
    public class PatientController : Controller
    {
        private readonly ILogger<PatientController> _logger;
        private readonly ApplicationDbContext _db;

        public PatientController(ILogger<PatientController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult PatientSite()
        {
            return View();
        }

        public IActionResult SubmitRequest()
        {
            return View();
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
                var user = _db.AspNetUsers.FirstOrDefault(u => u.UserName == model.Username);
                if (user != null)
                {
                    if (model.Password == user.PasswordHash)
                    {
                        return RedirectToAction("PatientDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Incorrect Password");
                    }
                }
                else
                {
                    ModelState.AddModelError("Username", "Incorrect Username");
                }
            }
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PatientForm()
        {
            ViewData["isRegistered"] = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PatientForm(PatientRequestViewModel modal)
        {
            if (ModelState.IsValid)
            {
                var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == modal.Email);
                if (modal.ImageContent != null && modal.ImageContent.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", modal.ImageContent.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await modal.ImageContent.CopyToAsync(stream);
                    }
                }
                
                var region = _db.Regions.FirstOrDefault(u => u.Name == modal.State.Trim().ToLower().Replace(" ", ""));
                if(region == null)
                {
                    ModelState.AddModelError("ImageContent", "Currently we are not serving in this region");
                    return View(modal);
                }
                if (user != null)
                {
                    var curr_user = _db.Users.FirstOrDefault(u => u.AspNetUserId == user.Id);
                    var block = _db.BlockRequests.FirstOrDefault(u=>u.Email == user.Email);
                    if(block != null)
                    {
                        ModelState.AddModelError("ImageContent", "This request is blocked");
                        return View(modal);
                    }

                    RequestClient rc = new RequestClient
                    {
                        FirstName = modal.FirstName,
                        LastName = modal.LastName,
                        PhoneNumber = modal.Phone,
                        Email = modal.Email,
                        State = modal.State,
                        Street = modal.Street,
                        City = modal.City,
                        RegionId = region.RegionId,
                        ZipCode = modal.ZipCode,
                        Notes = modal.Symptoms,
                        NotiEmail = modal.Email,
                        NotiMobile = modal.Phone,
                        StrMonth = modal.DateOfBirth.Month.ToString(),
                        IntYear = modal.DateOfBirth.Year,
                        IntDate = modal.DateOfBirth.Day
                    };

                    _db.RequestClients.Add(rc);
                    _db.SaveChanges();

                    int requests = _db.Requests.Where(u => u.CreatedDate == DateTime.Now.Date).Count();

                    Request req = new Request
                    {
                        FirstName = modal.FirstName,
                        LastName = modal.LastName,
                        PhoneNumber = modal.Phone,
                        Email = modal.Email,
                        RequestClientId = rc.RequestClientId,
                        RequestTypeId = 2,
                        UserId = curr_user.UserId,
                        Status = 1,
                        CreatedDate = DateTime.Now.Date,
                        IsUrgentEmailSent = new BitArray(1),
                        ConfirmationNumber = string.Concat(region.Abbreviation, modal.FirstName.Substring(0, 2).ToUpper(), modal.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4)),

                    };

                    _db.Requests.Add(req);
                    _db.SaveChanges();

                    if (modal.ImageContent != null)
                    {
                        RequestWiseFile rfile = new RequestWiseFile
                        {
                            RequestId = req.RequestId,
                            FileName = modal.ImageContent.FileName,
                            CreatedDate = DateTime.Now.Date
                        };
                        _db.RequestWiseFiles.Add(rfile);
                        _db.SaveChanges();
                    }


                    RequestStatusLog rst = new RequestStatusLog
                    {
                        RequestId = req.RequestId,
                        Status = 1,
                        CreatedDate = DateTime.Now.Date
                    };

                    _db.RequestStatusLogs.Add(rst);
                    _db.SaveChanges();

                    return RedirectToAction("PatientSite");

                }
                else
                {

                    AspNetUser aspuser = new AspNetUser
                    {
                        UserName = modal.Email,
                        Email = modal.Email,
                        PhoneNumber = modal.Phone,
                        CreatedDate = DateTime.Now.Date,
                        PasswordHash = modal.Password  ,
                    };


                    _db.AspNetUsers.Add(aspuser);
                    _db.SaveChanges();


                    User us = new User
                    {
                        AspNetUserId = aspuser.Id,
                        FirstName = modal.FirstName,
                        LastName = modal.LastName,
                        Email = modal.Email,
                        Mobile = modal.Phone,
                        Street = modal.Street,
                        City = modal.City,
                        State = modal.State,
                        RegionId = region.RegionId,
                        ZipCode = modal.ZipCode,
                        StrMonth = modal.DateOfBirth.Month.ToString(),
                        IntYear = modal.DateOfBirth.Year,
                        IntDate = modal.DateOfBirth.Day,
                        CreatedBy = aspuser.Id,
                        CreatedDate = DateTime.Now.Date,

                    };

                    _db.Users.Add(us);
                    _db.SaveChanges();

                    RequestClient rc = new RequestClient
                    {
                        FirstName = modal.FirstName,
                        LastName = modal.LastName,
                        PhoneNumber = modal.Phone,
                        Email = modal.Email,
                        State = modal.State,
                        Street = modal.Street,
                        City = modal.City,
                        RegionId = region.RegionId,
                        ZipCode = modal.ZipCode,
                        Notes = modal.Symptoms,
                        NotiEmail = modal.Email,
                        NotiMobile = modal.Phone,
                        StrMonth = modal.DateOfBirth.Month.ToString(),
                        IntYear = modal.DateOfBirth.Year,
                        IntDate = modal.DateOfBirth.Day
                    };

                    _db.RequestClients.Add(rc);
                    _db.SaveChanges();

                    int requests = _db.Requests.Where(u => u.CreatedDate == DateTime.Now.Date).Count();

                    Request req = new Request
                    {
                        FirstName = modal.FirstName,
                        LastName = modal.LastName,
                        PhoneNumber = modal.Phone,
                        Email = modal.Email,
                        RequestClientId = rc.RequestClientId,
                        RequestTypeId = 2,
                        UserId = us.UserId,
                        Status = 1,
                        CreatedDate = DateTime.Now.Date,
                        IsUrgentEmailSent = new BitArray(1),
                        ConfirmationNumber = string.Concat(region.Abbreviation, modal.FirstName.Substring(0, 2).ToUpper(), modal.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4)),

                    };

                    _db.Requests.Add(req);
                    _db.SaveChanges();
                    if (modal.ImageContent != null)
                    {
                        RequestWiseFile rfile = new RequestWiseFile
                        {
                            RequestId = req.RequestId,
                            FileName = modal.ImageContent.FileName,
                            CreatedDate = DateTime.Now.Date
                        };
                        _db.RequestWiseFiles.Add(rfile);
                        _db.SaveChanges();
                    }
                    
                    RequestStatusLog rst = new RequestStatusLog
                    {
                        RequestId = req.RequestId,
                        Status = 1,
                        CreatedDate = DateTime.Now.Date
                    };

                    _db.RequestStatusLogs.Add(rst);
                    _db.SaveChanges();

                    return RedirectToAction("PatientSite");

                }
            }
            return View(modal);
        }

        public IActionResult PatientCheck(string email)
        {
            if(email == null)
            {
                return View();
            }
            var existingUser = _db.AspNetUsers.SingleOrDefault(u => u.Email == email);
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

        public IActionResult PaswordCheck(string password,string confirmpassword)
        {
            bool isPasswordSame;
            if(password == confirmpassword)
            {
                isPasswordSame = true;
            }
            else
            {
                isPasswordSame = false;
            }
            return Json(new { isPasswordValid = isPasswordSame });
        }

        public IActionResult BusinessForm()
        {
            return View();
        }

        public IActionResult FamilyForm()
        {
            return View();
        }

        public IActionResult ConciergeForm()
        {
            return View();
        }

        public IActionResult PatientDashboard()
        {
            return View();
        }

        public IActionResult SubmitSomeoneElse()
        {
            return View();
        }

        public IActionResult SubmitForMe()
        {
            return View();
        }

        public IActionResult PatientProfile()
        {
            return View();
        }

        public IActionResult ViewDocument()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}