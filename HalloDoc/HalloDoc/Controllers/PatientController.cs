using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HalloDoc.ViewModels;
using HalloDoc;
using System.Collections;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.IO.Compression;
using System.IO;

namespace HaloDocMVC.NET.Controllers
{
    public class PatientController : Controller
    {
        private readonly ILogger<PatientController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _context;

        public PatientController(ILogger<PatientController> logger, ApplicationDbContext db,IHttpContextAccessor context)
        {
            _logger = logger;
            _db = db;
            _context = context;
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
                        var role = _db.AspNetUserRoles.FirstOrDefault(u=>u.UserId == user.Id);
                        if(role.RoleId != 1)
                        {
                            ModelState.AddModelError("Password", "You are not having rights of patient site");
                            return View();
                        }
                        var curr_user = _db.Users.FirstOrDefault(u=>u.AspNetUserId == user.Id);
                        _context.HttpContext.Session.SetInt32("AspNetUserId", user.Id);
                        _context.HttpContext.Session.SetInt32("UserId", curr_user.UserId);
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

                    AspNetUserRole aspnr = new AspNetUserRole
                    {
                        UserId = aspuser.Id,
                        RoleId = 1
                    };

                    _db.AspNetUserRoles.Add(aspnr);
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

        public IActionResult Logout()
        {

            _context.HttpContext.Session.Clear();
            return Json(new { isLogout = true});
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
                if (region == null)
                {
                    ModelState.AddModelError("ImageContent", "Currently we are not serving in this region");
                    return View(modal);
                }
                if(user != null)
                {
                    var curr_user = _db.Users.FirstOrDefault(u => u.AspNetUserId == user.Id);
                    var block = _db.BlockRequests.FirstOrDefault(u => u.Email == user.Email);
                    if (block != null)
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
                        FirstName = modal.FamilyFirstName,
                        LastName = modal.FamilyLastName,
                        PhoneNumber = modal.FamilyPhoneNumber,
                        Email = modal.FamilyEmail,
                        RequestClientId = rc.RequestClientId,
                        RequestTypeId = 3,
                        UserId = curr_user.UserId,
                        Status = 1,
                        CreatedDate = DateTime.Now.Date,
                        IsUrgentEmailSent = new BitArray(1),
                        ConfirmationNumber = string.Concat(region.Abbreviation, modal.FirstName.Substring(0, 2).ToUpper(), modal.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4)),
                        RelationName = modal.FamilyRelation,

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

                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ConciergeForm(ConciergeRequestViewModel modal)
        {
            if (ModelState.IsValid)
            {
                var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == modal.Email);

                var region = _db.Regions.FirstOrDefault(u => u.Name == modal.ConciergeState.Trim().ToLower().Replace(" ", ""));
                if (region == null)
                {
                    ModelState.AddModelError("Room", "Currently we are not serving in this region");
                    return View(modal);
                }
                if (user != null)
                {
                    var curr_user = _db.Users.FirstOrDefault(u => u.AspNetUserId == user.Id);
                    var block = _db.BlockRequests.FirstOrDefault(u => u.Email == user.Email);
                    if (block != null)
                    {
                        ModelState.AddModelError("Room", "This request is blocked");
                        return View(modal);
                    }
                    RequestClient rc = new RequestClient
                    {
                        FirstName = modal.FirstName,
                        LastName = modal.LastName,
                        PhoneNumber = modal.Phone,
                        Email = modal.Email,
                        State = modal.ConciergeState,
                        Street = modal.ConciergeStreet,
                        City = modal.ConciergeCity,
                        RegionId = region.RegionId,
                        ZipCode = modal.ConciergeZipcode,
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
                        FirstName = modal.ConciergeFirstName,
                        LastName = modal.ConciergeLastName,
                        PhoneNumber = modal.ConciergePhoneNumber,
                        Email = modal.ConciergeEmail,
                        RequestClientId = rc.RequestClientId,
                        RequestTypeId = 4,
                        UserId = curr_user.UserId,
                        Status = 1,
                        CreatedDate = DateTime.Now.Date,
                        IsUrgentEmailSent = new BitArray(1),
                        ConfirmationNumber = string.Concat(region.Abbreviation, modal.FirstName.Substring(0, 2).ToUpper(), modal.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4)),

                    };

                    _db.Requests.Add(req);
                    _db.SaveChanges();

                    RequestStatusLog rst = new RequestStatusLog
                    {
                        RequestId = req.RequestId,
                        Status = 1,
                        CreatedDate = DateTime.Now.Date
                    };

                    _db.RequestStatusLogs.Add(rst);
                    _db.SaveChanges();

                    Concierge concierge = new Concierge
                    {
                        ConciergeName = string.Concat(modal.ConciergeFirstName,' ',modal.ConciergeLastName),
                        RegionId = region.RegionId,
                        CreatedDate = DateTime.Now.Date,
                        Street = modal.ConciergeStreet,
                        City = modal.ConciergeCity,
                        State = modal.ConciergeState,
                        ZipCode = modal.ConciergeZipcode
                    };

                    _db.Concierges.Add(concierge); 
                    _db.SaveChanges();

                    RequestConcierge requestconcierge = new RequestConcierge
                    {
                        ConciergeId = concierge.ConciergeId,
                        RequestId = req.RequestId
                    };

                    _db.RequestConcierges.Add(requestconcierge);
                    _db.SaveChanges();

                    return RedirectToAction("PatientSite");
                }
                else
                {

                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> BusinessForm(BusinessRequestViewModel modal)
        {
            if(ModelState.IsValid)
            {
                var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == modal.Email);

                var region = _db.Regions.FirstOrDefault(u => u.Name == modal.State.Trim().ToLower().Replace(" ", ""));
                if (region == null)
                {
                    ModelState.AddModelError("Room", "Currently we are not serving in this region");
                    return View(modal);
                }
                if(user!=null)
                {
                    var curr_user = _db.Users.FirstOrDefault(u => u.AspNetUserId == user.Id);
                    var block = _db.BlockRequests.FirstOrDefault(u => u.Email == user.Email);
                    if (block != null)
                    {
                        ModelState.AddModelError("Room", "This request is blocked");
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
                        FirstName = modal.BusinessFirstName,
                        LastName = modal.BusinessLastName,
                        PhoneNumber = modal.BusinessPhoneNumber,
                        Email = modal.BusinessEmail,
                        RequestClientId = rc.RequestClientId,
                        RequestTypeId = 1,
                        UserId = curr_user.UserId,
                        Status = 1,
                        CreatedDate = DateTime.Now.Date,
                        IsUrgentEmailSent = new BitArray(1),
                        ConfirmationNumber = string.Concat(region.Abbreviation, modal.FirstName.Substring(0, 2).ToUpper(), modal.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4)),
                        CaseNumber = modal.BusinessCaseNumber
                    };

                    _db.Requests.Add(req);
                    _db.SaveChanges();

                    RequestStatusLog rst = new RequestStatusLog
                    {
                        RequestId = req.RequestId,
                        Status = 1,
                        CreatedDate = DateTime.Now.Date
                    };

                    _db.RequestStatusLogs.Add(rst);
                    _db.SaveChanges();

                    Business business = new Business
                    {
                        Name = modal.BusinessPropertyName,
                        CreatedDate = DateTime.Now.Date,
                        RegionId = region.RegionId,

                    };

                    _db.Businesses.Add(business);
                    _db.SaveChanges();

                    RequestBusiness requestbusiness = new RequestBusiness
                    {
                        RequestId = req.RequestId,
                        BusinessId = business.BusinessId
                    };

                    _db.RequestBusinesses.Add(requestbusiness);
                    _db.SaveChanges();

                    return RedirectToAction("PatientSite");
                }
                else
                {

                }
            }

            return View();
        }

        public IActionResult ConciergeForm()
        {
            return View();
        }

        public async Task<IActionResult> PatientDashboard()
        {
            var id = _context.HttpContext.Session.GetInt32("UserId");
            var data = _db.RequestViewModels.FromSqlRaw(
    $"SELECT * FROM PatientDashboardData({id})"
).ToList();
            var curr_user = _db.Users.FirstOrDefault(u=>u.UserId == id);
            DashboardViewModel dashboardViewModel = new DashboardViewModel
            {
                requests = data,
                name = string.Concat(curr_user.FirstName,' ',curr_user.LastName)
            };

            return View(dashboardViewModel);
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

        [HttpGet]
        public IActionResult ViewDocument(int id)
        {
            var user_id = _context.HttpContext.Session.GetInt32("UserId");
            var request = _db.Requests.Include(r=>r.RequestClient).FirstOrDefault(u=>u.RequestId == id);
            var documents = _db.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u=>u.RequestId == id).ToList();
            var user = _db.Users.FirstOrDefault(u=>u.UserId == user_id);
            ViewDocumentModal viewDocumentModal = new ViewDocumentModal()
            {
                patient_name = string.Concat(request.RequestClient.FirstName,' ', request.RequestClient.LastName),
                name = string.Concat(user.FirstName, ' ', user.LastName),
                confirmation_number = request.ConfirmationNumber,
                requestWiseFiles = documents,
                uploader_name = string.Concat(request.FirstName, ' ', request.LastName)
            };
            return View(viewDocumentModal);
        }

        [HttpPost]
        public async Task<IActionResult> FileUpload([FromForm]IFormFile file, [FromForm] int id)
        {
            if (file != null && file.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", file.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
            }
            RequestWiseFile requestWiseFile = new RequestWiseFile
            {
                RequestId = id,
                FileName = file.FileName,
                CreatedDate = DateTime.Now.Date,

            };
            _db.RequestWiseFiles.Add(requestWiseFile);
            _db.SaveChanges();
            return Json(new { isFileUploaded = true });
        }

        public IActionResult DownloadAll(string filename)
        {
            var zipName = $"TestFiles-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";
            string[] filenames = filename.Split(',') ;
            using (MemoryStream ms = new MemoryStream())
            {
                //required: using System.IO.Compression;
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    //QUery the Products table and get all image content

                    for(var i=0;i< filenames.Length - 1; ++i)
                    {
                        var entry = zip.CreateEntry(filenames[i]);
                        byte[] bytes = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", filenames[i]));
                        using (MemoryStream fileStream = new MemoryStream(bytes))
                        using (var entryStream = entry.Open())
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }
                return File(ms.ToArray(), "application/zip", zipName);
            }
            return Json(new { isDownloaded = true });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}