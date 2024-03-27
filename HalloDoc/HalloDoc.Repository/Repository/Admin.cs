using ClosedXML.Excel;
using DocumentFormat.OpenXml.VariantTypes;
using HalloDoc.Repository.Interface;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Collections;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.IO.Compression;
using DocumentFormat.OpenXml.Office2010.Word;
using Microsoft.AspNetCore.Identity;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Drawing.Charts;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Irony.Parsing;
using System.Collections.Immutable;
using DocumentFormat.OpenXml.Wordprocessing;
using static HalloDoc.ViewModels.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HalloDoc.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;
using Twilio.Http;
using Twilio.Types;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Twilio.Rest.Trusthub.V1.TrustProducts;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;

namespace HalloDoc.Repository.Repository
{

    public class Admin:IAdmin
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _context;
        private readonly IJwtService _jwt;
        private readonly IConfiguration _configuration;
        public Admin(ApplicationDbContext db, IHttpContextAccessor context, IJwtService jwt, IConfiguration configuration)
        {
            _db = db;
            _context = context;
            _jwt = jwt;
            _configuration = configuration;
        }

        public AdminDashboardViewModel adminDashboardContent(string status, string? search, string? requestor, int? region,int page=1,int pageSize = 10)
        {
            Expression<Func<Request, bool>> exp;
            if(status == "New")
            {
                exp = r => r.Status == 1;
            }
            else if(status=="Pending")
            {
                exp = r => r.Status == 2;
            }
            else if(status == "Active")
            {
                exp = r => r.Status == 3 || r.Status == 4;
            }
            else if(status=="Conclude")
            {
                exp = r => r.Status == 5;
            }
            else if(status=="ToClose")
            {
                exp = r => r.Status == 6 || r.Status == 7 || r.Status == 8;
            }
            else
            {
                exp = r => r.Status == 9;
            }

            IQueryable<Request> _query = _db.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(exp).Where(r=>r.IsDeleted == new BitArray(new[] { false })).OrderByDescending(e => e.CreatedDate);

            var count_new = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false })).Count(r => r.Status == 1);
            var count_pending = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false })).Count(r => r.Status == 2);
            var count_active = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false })).Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false })).Count(r => r.Status == 5);
            var count_toclose = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false })).Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false })).Count(r => r.Status == 9);
            var casetag = _db.CaseTags.ToList();

            if (search != null)
            {
                _query = _query.Where(r => r.RequestClient.FirstName.ToLower().Contains(search.ToLower()) || r.RequestClient.LastName.ToLower().Contains(search.ToLower()));
            }

            if (requestor == "Family")
            {
                _query = _query.Where(r => r.RequestTypeId == 3);
            }

            if (requestor == "Business")
            {
                _query = _query.Where(r => r.RequestTypeId == 1);
            }

            if (requestor == "Concierge")
            {
                _query = _query.Where(r => r.RequestTypeId == 4);
            }

            if (requestor == "Patient")
            {
                _query = _query.Where(r => r.RequestTypeId == 2);
            }
            if (region != null && region != -1)
            {
                _query = _query.Where(r => r.RequestClient.RegionId == region);
            }


            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus
            };


            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                unpaid_count = count_unpaid,
                requests = _query.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                regions = _db.Regions.ToList(),
                status = status,
                caseTags = casetag,
                adminNavbarViewModel = adminNavbarViewModel,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = _query.Count(),
                TotalPages = (int)Math.Ceiling((double)_query.Count() / pageSize)
            };
            return adminDashboardViewModel;
        }

        public MemoryStream exportAll()
        {
            try
            {
                //List<Request> data = new List<Request>();
                var data = _db.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Where(r => r.IsDeleted == new BitArray(new[] { false })).ToList();
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Export All");


                worksheet.Cell(1, 1).Value = "Name";
                worksheet.Cell(1, 2).Value = "Date Of Birth";
                worksheet.Cell(1, 3).Value = "Requestor";
                worksheet.Cell(1, 4).Value = "Physician Name";
                worksheet.Cell(1, 5).Value = "Date of Service";
                worksheet.Cell(1, 6).Value = "Requested Date";
                worksheet.Cell(1, 7).Value = "Phone Number";
                worksheet.Cell(1, 8).Value = "Address";
                worksheet.Cell(1, 9).Value = "Notes";

                int row = 2;
                foreach (var item in data)
                {
                    var statusClass = "";
                    var dos = "";
                    var notes = "";
                    if (item.RequestTypeId == 1)
                    {
                        statusClass = "business";
                    }
                    else if (item.RequestTypeId == 3)
                    {
                        statusClass = "family";
                    }
                    else if (item.RequestTypeId == 2)
                    {
                        statusClass = "patient";
                    }
                    else
                    {
                        statusClass = "concierge";
                    }
                    foreach (var stat in item.RequestStatusLogs)
                    {
                        if (stat.Status == 2)
                        {
                            dos = stat.CreatedDate.ToString("MMMM dd,yyyy");
                            notes = stat.Notes ?? "";
                        }
                    }
                    worksheet.Cell(row, 1).Value = string.Concat(item.RequestClient.FirstName + ',' + item.RequestClient.LastName);
                    worksheet.Cell(row, 2).Value = DateTime.Parse($"{item.RequestClient.IntYear}-{item.RequestClient.StrMonth}-{item.RequestClient.IntDate}").ToString("MMMM dd,yyyy");
                    worksheet.Cell(row, 3).Value = statusClass.Substring(0, 1).ToUpper() + statusClass.Substring(1).ToLower() + " " + item.FirstName + item.LastName;
                    worksheet.Cell(row, 4).Value = "Dr." + item?.Physician == null ? "" : item?.Physician?.FirstName;
                    worksheet.Cell(row, 5).Value = item.CreatedDate.ToString("MMMM dd,yyyy");
                    worksheet.Cell(row, 6).Value = item.AcceptedDate?.ToString("MMMM dd,yyyy");
                    worksheet.Cell(row, 7).Value = item.RequestClient.PhoneNumber + "(Patient)" + (item.RequestTypeId != 2 ? item.PhoneNumber + "(" + statusClass.Substring(0, 1).ToUpper() + statusClass.Substring(1).ToLower() + ")" : "");
                    worksheet.Cell(row, 8).Value = (item.RequestClient.Address != null ? string.Concat(item.RequestClient.Address, ',', item.RequestClient.Street, ',', item.RequestClient.City, ',', item.RequestClient.State, ',', item.RequestClient.ZipCode) : string.Concat(item.RequestClient.Street, ',', item.RequestClient.City, ',', item.RequestClient.State, ',', item.RequestClient.ZipCode));
                    worksheet.Cell(row, 9).Value = notes;
                    row++;
                }
                worksheet.Columns().AdjustToContents();

                var memoryStream = new MemoryStream();
                workbook.SaveAs(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        public MemoryStream export(AdminDashboardViewModel model)
        {
            try
            {
                List<Request> data = new List<Request>();
                data = model.requests;
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Export");

                int count = 1;
                worksheet.Cell(1, count++).Value = "Name";
                if (model.status != "Unpaid")
                {
                    worksheet.Cell(1, count++).Value = "Date Of Birth";
                }
                if (model.status == "New" || model.status == "Pending" || model.status == "Active")
                {
                    worksheet.Cell(1, count++).Value = "Requestor";
                }
                if (model.status != "New")
                {
                    worksheet.Cell(1, count++).Value = "Physician Name";
                    worksheet.Cell(1, count++).Value = "Date Of Service";
                }
                if (model.status == "New")
                {
                    worksheet.Cell(1, count++).Value = "Requested Date";
                }
                if (model.status != "ToClose")
                {
                    worksheet.Cell(1, count++).Value = "Phone";
                }
                worksheet.Cell(1, count++).Value = "Address";
                if (model.status != "Conclude" || model.status != "Unpaid")
                {
                    worksheet.Cell(1, count++).Value = "Notes";
                }

                int row = 2;
                foreach (var item in data)
                {
                    count = 1;
                    var statusClass = "";
                    var dos = "";
                    var notes = "";
                    if (item.RequestTypeId == 1)
                    {
                        statusClass = "business";
                    }
                    else if (item.RequestTypeId == 3)
                    {
                        statusClass = "family";
                    }
                    else if (item.RequestTypeId == 2)
                    {
                        statusClass = "patient";
                    }
                    else
                    {
                        statusClass = "concierge";
                    }
                    foreach (var stat in item.RequestStatusLogs)
                    {
                        if (stat.Status == 2)
                        {
                            dos = stat.CreatedDate.ToString("MMMM dd,yyyy");
                            notes += stat.Notes + Environment.NewLine;
                        }
                    }
                    worksheet.Cell(row, count++).Value = string.Concat(item.RequestClient.FirstName, ',', item.RequestClient.LastName);
                    if (model.status != "Unpaid")
                    {

                        DateTime now = DateTime.Today;
                        int age = now.Year - DateTime.Parse($"{item.RequestClient.IntYear}-{item.RequestClient.StrMonth}-{item.RequestClient.IntDate}").Year;
                        if (DateTime.Parse($"{item.RequestClient.IntYear}-{item.RequestClient.StrMonth}-{item.RequestClient.IntDate}") > now.AddYears(-age))
                            age--;

                        worksheet.Cell(row, count++).Value = DateTime.Parse($"{item.RequestClient.IntYear}-{item.RequestClient.StrMonth}-{item.RequestClient.IntDate}").ToString("MMMM dd,yyyy") + "(" + age + ")";
                    }
                    if (model.status == "New" || model.status == "Pending" || model.status == "Active")
                    {
                        worksheet.Cell(row, count++).Value = statusClass.Substring(0, 1).ToUpper() + statusClass.Substring(1).ToLower() + " " + string.Concat(item.FirstName, ',', item.LastName);
                    }
                    if (model.status != "New")
                    {
                        worksheet.Cell(row, count++).Value = "Dr." + item.Physician == null ? "" : item.Physician.FirstName;
                    }
                    if (model.status == "New")
                    {

                        int hoursDifference = (int)(DateTime.Now - item.CreatedDate).TotalHours;
                        int minutesDifference = (DateTime.Now - item.CreatedDate).Minutes;

                        worksheet.Cell(row, count++).Value = item.CreatedDate.ToString("MMMM dd,yyyy") + " " + hoursDifference + "H " + minutesDifference + "M";
                    }
                    if (model.status != "New")
                    {
                        worksheet.Cell(row, count++).Value = item.AcceptedDate?.ToString("MMMM dd,yyyy"); 
                    }
                    if (model.status != "ToClose")
                    {
                        worksheet.Cell(row, count++).Value = item.RequestClient.PhoneNumber + "(Patient)" + (item.RequestTypeId != 2 ? item.PhoneNumber + "(" + statusClass.Substring(0, 1).ToUpper() + statusClass.Substring(1).ToLower() + ")" : "");
                    }
                    worksheet.Cell(row, count++).Value = (item.RequestClient.Address != null ? string.Concat(item.RequestClient.Address, ',', item.RequestClient.Street, ',', item.RequestClient.City, ',', item.RequestClient.State, ',', item.RequestClient.ZipCode) : string.Concat(item.RequestClient.Street, ',', item.RequestClient.City, ',', item.RequestClient.State, ',', item.RequestClient.ZipCode));
                    if (model.status != "Conclude" || model.status != "Unpaid")
                    {
                        worksheet.Cell(row, count++).Value = notes;
                    }
                    row++;
                }
                worksheet.Columns().AdjustToContents();

                var memoryStream = new MemoryStream();
                workbook.SaveAs(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        public ViewCaseViewModel viewCase(int id)
        {
            var req = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(r => r.RequestId == id);
            var region = _db.Regions.FirstOrDefault(r => r.RegionId == req.RequestClient.RegionId);
            var caseTags = _db.CaseTags.ToList();

            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            var admin = _db.Admins.FirstOrDefault(a => a.AdminId == cookieModel.userId);
            var regions = _db.Regions.ToList();

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus
            };

            ViewCaseViewModel viewCaseViewModel = new ViewCaseViewModel()
            {
                id = id,
                FirstName = req.RequestClient.FirstName,
                LastName = req.RequestClient.LastName,
                DateOfBirth = DateTime.Parse($"{req.RequestClient.IntYear}-{req.RequestClient.StrMonth}-{req.RequestClient.IntDate}"),
                PhoneNumber = req.RequestClient.PhoneNumber,
                Email = req.RequestClient.Email,
                Region = region.Name,
                Address = string.Concat(req.RequestClient.Street, ',', req.RequestClient.City, ',', req.RequestClient.State, ',', req.RequestClient.ZipCode),
                RequestId = req.RequestId,
                RequestClientId = req.RequestClientId,
                Status = req.Status,
                RequestTypeId = req.RequestTypeId,
                ConfirmationNumber = req.ConfirmationNumber,
                Notes = req.RequestClient.Notes,
                Room = req.RequestClient.Address,
                caseTags = caseTags,
                adminNavbarViewModel = adminNavbarViewModel,
                regions = regions,
                RegionId = region.RegionId
            };
            return viewCaseViewModel;
        }

        public bool viewCase(ViewCaseViewModel model)
        {
            
            try
            {
                RequestClient requestClient = _db.RequestClients.FirstOrDefault(r=>r.RequestClientId == model.RequestClientId);
                requestClient.PhoneNumber = model?.PhoneNumber ?? requestClient.PhoneNumber;
                requestClient.Email = model?.Email ?? requestClient.Email;
                _db.RequestClients.Update(requestClient);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

         public bool cancelRequest(int id,string notes,string request)
        {
            try
            {
                Request req = _db.Requests.FirstOrDefault(r => r.RequestId == id);
                req.CaseTag = request;
                req.Status = 6;
                req.ModifiedDate = DateTime.Now;
                _db.Requests.Update(req);

                RequestStatusLog requestStatusLog = new RequestStatusLog()
                {
                    RequestId = id,
                    Status = 6,
                    Notes = notes,
                    CreatedDate = DateTime.Now,
                };
                _db.RequestStatusLogs.Add(requestStatusLog);
                _db.SaveChanges();

                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public async Task<bool> sendLink(AdminDashboardViewModel dashboardViewModel)
        {
            int retryCount = 1;
            bool success = false;

            while (retryCount <= 3 && !success) // Set retry limit
            {

                string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                string senderPassword = "shahkandarp2430";
                var platformTitle = "HalloDoc";
                var inviteLink = "https://localhost:7088/CreateRequest/SubmitRequest";
                var subject = "Register - HalloDoc";
                var body = $"Hello {dashboardViewModel.Mail_FirstName} {dashboardViewModel.Mail_LastName},<br />Click the following link to create new request in our portal,<br /><br /><a href='{inviteLink}'>Create Request</a><br /><br />Regards,<br/>{platformTitle}<br/>";
                try
                {

                    SmtpClient client = new SmtpClient("smtp.office365.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(senderEmail, senderPassword),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false
                    };
                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, "HalloDoc"),
                        Subject = "Create New Request",
                        IsBodyHtml = true,
                        Body = body
                    };

                    mailMessage.To.Add(dashboardViewModel.Mail_Email);

                    await client.SendMailAsync(mailMessage);


                    success = true;
                    LogEmail(body, subject, dashboardViewModel.Mail_Email, null, -1, -1, -1, true, retryCount, -1);
                    break;
                }
                catch (Exception ex)
                {

                    if (retryCount >= 3)
                    {
                        LogEmail(body, subject, dashboardViewModel.Mail_Email, null, -1, -1, -1, false, retryCount, -1);
                    }
                    retryCount++;
                }
            }

            retryCount = 1;
            success = false;

            while (retryCount <= 3 && !success) // Set retry limit
            {
                var platformTitle = "HalloDoc";
                var inviteLink = "https://localhost:7088/CreateRequest/SubmitRequest";

                var accountSid = _configuration["Twilio:accountSid"];
                var authToken = _configuration["Twilio:authToken"];
                var twilionumber = _configuration["Twilio:twilioNumber"];

                var messageBody = $"Hello {dashboardViewModel.Mail_FirstName} {dashboardViewModel.Mail_LastName},\nClick the following link to create new request in our portal,\n\n{inviteLink}\n\nRegards,\n{platformTitle}";
                try
                {

                    TwilioClient.Init(accountSid, authToken);

                    var message = MessageResource.Create(
                        from: new Twilio.Types.PhoneNumber(twilionumber),
                        body: messageBody,
                        to: new Twilio.Types.PhoneNumber("+91" + dashboardViewModel.Mail_PhoneNumber)
                    );


                    success = true;
                    LogSMS(messageBody, dashboardViewModel.Mail_PhoneNumber, null, -1, -1, -1, true, retryCount, -1);
                    break;
                }
                catch (Exception ex)
                {

                    if (retryCount >= 3)
                    {
                        LogSMS(messageBody, dashboardViewModel.Mail_PhoneNumber, null, -1, -1, -1, false, retryCount, -1);
                    }
                    retryCount++;
                }
            }

            return success;
        }

        public bool verifyRegion(string region)
        {
            var region_check = _db.Regions.FirstOrDefault(u => u.Name == region.Trim().ToLower().Replace(" ", ""));
            if(region_check != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool verifyBlock(string Email)
        {
            var user = _db.AspNetUsers.FirstOrDefault(u=>u.Email == Email); 
            if(user != null)
            {
                var block = _db.BlockRequests.FirstOrDefault(u => u.Email == user.Email);
                if(block != null)
                {
                    return true;
                }
            }
            return false;
        }

        public PatientRequestViewModel createRequest()
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus
            };

            PatientRequestViewModel patientRequestViewModel = new PatientRequestViewModel()
            {
                adminNavbarViewModel = adminNavbarViewModel
            };
            return patientRequestViewModel;
        }

        public async Task<bool> createRequest(PatientRequestViewModel modal)
        {
            try
            {
                var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == modal.Email);
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);

                var region = _db.Regions.FirstOrDefault(u => u.Name == modal.State.Trim().ToLower().Replace(" ", ""));
                if (user != null)
                {
                    var curr_user = _db.Users.FirstOrDefault(u => u.AspNetUserId == user.Id);

                    RequestClient rc = new RequestClient
                    {
                        FirstName = modal.FirstName,
                        LastName = modal.LastName,
                        PhoneNumber = modal.Phone,
                        Email = modal.Email,
                        State = modal.State,
                        Street = modal.Street,
                        City = modal.City,
                        Address = modal.Room,
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

                    int requests = _db.Requests.Where(u => u.CreatedDate.Date == DateTime.Now.Date).Count();

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
                        CreatedDate = DateTime.Now,
                        IsUrgentEmailSent = new BitArray(new[] { false }),
                        ConfirmationNumber = string.Concat(region.Abbreviation, modal.FirstName.Substring(0, 2).ToUpper(), modal.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4)),

                    };

                    _db.Requests.Add(req);
                    _db.SaveChanges();


                    RequestStatusLog rst = new RequestStatusLog
                    {
                        RequestId = req.RequestId,
                        Status = 1,
                        CreatedDate = DateTime.Now
                    };

                    _db.RequestStatusLogs.Add(rst);
                    _db.SaveChanges();

                    if(modal.Admin_notes != null)
                    {
                        RequestNote requestNote = new RequestNote
                        {
                            RequestId = req.RequestId,
                            AdminNotes = modal.Admin_notes,
                            CreatedDate = DateTime.Now,
                            CreatedBy = cookieModel.aspId,
                        };

                        _db.RequestNotes.Add(requestNote);
                        _db.SaveChanges();
                    }

                    return true;

                }
                else
                {

                    AspNetUser aspuser = new AspNetUser
                    {
                        UserName = modal.Email,
                        Email = modal.Email,
                        PhoneNumber = modal.Phone,
                        CreatedDate = DateTime.Now
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
                        CreatedDate = DateTime.Now,

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
                        Address = modal.Room,
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

                    int requests = _db.Requests.Where(u => u.CreatedDate.Date == DateTime.Now.Date).Count();

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
                        CreatedDate = DateTime.Now,
                        IsUrgentEmailSent = new BitArray(new[] { false }),
                        ConfirmationNumber = string.Concat(region.Abbreviation, modal.FirstName.Substring(0, 2).ToUpper(), modal.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4)),

                    };

                    _db.Requests.Add(req);
                    _db.SaveChanges();

                    RequestStatusLog rst = new RequestStatusLog
                    {
                        RequestId = req.RequestId,
                        Status = 1,
                        CreatedDate = DateTime.Now
                    };

                    _db.RequestStatusLogs.Add(rst);
                    _db.SaveChanges();

                    if (modal.Admin_notes != null)
                    {
                        RequestNote requestNote = new RequestNote
                        {
                            RequestId = req.RequestId,
                            AdminNotes = modal.Admin_notes,
                            CreatedDate = DateTime.Now,
                            CreatedBy = cookieModel.userId,
                        };

                        _db.RequestNotes.Add(requestNote);
                        _db.SaveChanges();
                    }

                    int retryCount = 1;
                    bool success = false;

                    while (retryCount <= 3 && !success) // Set retry limit
                    {

                        string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                        string senderPassword = "shahkandarp2430";
                        var platformTitle = "HalloDoc";
                        var inviteLink = $"https://localhost:7088/Login/Register/{aspuser.Id}";
                        var subject = "Register - HalloDoc";
                        var body = $"Hello <br />Click the following link to register to our portal,<br /><br /><a href='{inviteLink}'>Register</a><br /><br />Regards,<br/>{platformTitle}<br/>";
                        try
                        {

                            SmtpClient client = new SmtpClient("smtp.office365.com")
                            {
                                Port = 587,
                                Credentials = new NetworkCredential(senderEmail, senderPassword),
                                EnableSsl = true,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                UseDefaultCredentials = false
                            };
                            MailMessage mailMessage = new MailMessage
                            {
                                From = new MailAddress(senderEmail, "HalloDoc"),
                                Subject = "Set up your Account",
                                IsBodyHtml = true,
                                Body = body
                            };

                            mailMessage.To.Add(modal.Email);

                            await client.SendMailAsync(mailMessage);


                            success = true;
                            LogEmail(body, subject, modal.Email, req.ConfirmationNumber,req.RequestId , -1, -1, true, retryCount, 1);
                            break;
                        }
                        catch (Exception ex)
                        {

                            if (retryCount >= 3)
                            {
                                LogEmail(body, subject, modal.Email, req.ConfirmationNumber, req.RequestId, -1, -1, false, retryCount, 1);
                            }
                            retryCount++;
                        }
                    }

                    return success;

                }
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public ViewNotesViewModel viewNotes(int id)
        {
            RequestStatusLog patientcancel = _db.RequestStatusLogs.FirstOrDefault(r=>r.RequestId == id && r.Status == 7);
            RequestStatusLog admincancel = _db.RequestStatusLogs.FirstOrDefault(r => r.RequestId == id && r.Status == 6);
            List<RequestStatusLog> transfernotes = _db.RequestStatusLogs.Where(r => r.RequestId == id && r.Status == 2).ToList();
            RequestNote requestNotes = _db.RequestNotes.FirstOrDefault(r=>r.RequestId == id);

            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus
            };

            ViewNotesViewModel viewNotesViewModel = new ViewNotesViewModel
            {
                RequestId = id,
                Admin_Note = requestNotes?.AdminNotes ?? "-",
                Physician_Note = requestNotes?.PhysicianNotes ?? "-",
                Admin_Cancellation_Note = admincancel?.Notes,
                Cancellation_Note = patientcancel?.Notes,
                Transfer_Notes = transfernotes,
                adminNavbarViewModel = adminNavbarViewModel
            };
            return viewNotesViewModel;
        }

        public bool updateAdminNotes(ViewNotesViewModel viewNotesViewModel)
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);
            try
            {
                RequestNote requestNote = _db.RequestNotes.FirstOrDefault(r => r.RequestId == viewNotesViewModel.RequestId);
                if (requestNote != null)
                {
                    requestNote.AdminNotes = viewNotesViewModel.Admin_Note;
                    requestNote.ModifiedDate = DateTime.Now;
                    _db.RequestNotes.Update(requestNote);
                    _db.SaveChanges();
                }
                else
                {
                    RequestNote newRequestNote = new RequestNote
                    {
                        RequestId = viewNotesViewModel.RequestId,
                        AdminNotes = viewNotesViewModel.Admin_Note,
                        CreatedDate = DateTime.Now,
                        CreatedBy = cookieModel.aspId,
                    };

                    _db.RequestNotes.Add(newRequestNote);
                    _db.SaveChanges();
                }
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public int login(LoginViewModel loginViewModel)
        {
            try
            {
                AspNetUser user = _db.AspNetUsers.FirstOrDefault(a => a.Email == loginViewModel.Email);
                if (user == null)
                {
                    return 1;
                }
                else
                {
                    var passwordHasher = new PasswordHasher<AspNetUser>();
                    var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginViewModel.Password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        var role = _db.AspNetUserRoles.FirstOrDefault(u => u.UserId == user.Id);
                        if (role.RoleId == 1)
                        {
                            return 3;
                        }
                        return 4;

                    }
                    else
                    {
                        return 2;
                    }
                }
            }
            catch(Exception exp)
            {
                return 5;
            }
        }

        public int forgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            var admin = _db.AspNetUsers.FirstOrDefault(a=>a.Email == forgotPasswordViewModel.email);
            if(admin == null)
            {
                return 1;
            }

            var role = _db.AspNetUserRoles.FirstOrDefault(a=>a.UserId == admin.Id);
            if(role.RoleId == 1)
            {
                return 1;
            }

            try
            {
                string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                string senderPassword = "shahkandarp2430";
                string platformTitle = "HalloDoc";
                string Token = Guid.NewGuid().ToString();
                PasswordReset passwordReset = new PasswordReset
                {
                    Token = Token,
                    Email = forgotPasswordViewModel.email,
                    CreatedDate = DateTime.Now
                };
                _db.PasswordResets.Add(passwordReset);
                _db.SaveChanges();
                var inviteLink = $"https://localhost:7088/Login/ResetPassword/?token={Token}";
                var subject = "Reset Password - HalloDoc";
                var body = $"Hello <br />Click the following link to change your password,<br /><br /><a href='{inviteLink}'>Change Password</a><br /><br />Regards,<br/>{platformTitle}<br/>";
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, "HalloDoc"),
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = body
                };

                SmtpClient client = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };
                mailMessage.To.Add(forgotPasswordViewModel.email);

                client.SendMailAsync(mailMessage);
                return 3;
            }
            catch(Exception ex)
            {
                return 2;
            }
        }

        public bool logout()
        {
            try
            {
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public ViewDocumentModal viewUploads(int id)
        {
            var request = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id);
            var documents = _db.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == id && u.IsDeleted.Equals(new BitArray(new[] { false }))).ToList();
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus
            };

            ViewDocumentModal viewDocumentModal = new ViewDocumentModal()
            {
                patient_name = string.Concat(request.RequestClient.FirstName, ' ', request.RequestClient.LastName),
                confirmation_number = request.ConfirmationNumber,
                requestWiseFiles = documents,
                uploader_name = string.Concat(request.FirstName, ' ', request.LastName),
                adminNavbarViewModel = adminNavbarViewModel,
            };
            return viewDocumentModal;
        }

        public async Task<bool> fileUpload(IFormFile file, int id)
        {
            try
            {
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);
                if (file != null && file.Length > 0)
                {
                    var filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", file.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                RequestWiseFile requestWiseFile = new RequestWiseFile
                {
                    RequestId = id,
                    FileName = file.FileName,
                    CreatedDate = DateTime.Now,
                    IsDeleted = new BitArray(new[] { false }),
                    AdminId = cookieModel.userId,
                };
                _db.RequestWiseFiles.Add(requestWiseFile);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public int deleteSingleFile(int id)
        {
            RequestWiseFile requestWiseFile = _db.RequestWiseFiles.FirstOrDefault(r=>r.RequestWiseFileId == id);
            requestWiseFile.IsDeleted = new BitArray(new[] { true });
            _db.RequestWiseFiles.Update(requestWiseFile);
            _db.SaveChanges();
            return requestWiseFile.RequestId;
        }

        public async Task<Tuple<MemoryStream, string>> downloadMultipleFiles(ViewDocumentModal viewDocumentModal)
        {
            var zipName = $"{viewDocumentModal.patient_name}-documents.zip";
            string[] filenames = viewDocumentModal.filename.Split(',');
            using (MemoryStream ms = new MemoryStream())
            {
                //required: using System.IO.Compression;
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    //QUery the Products table and get all image content

                    for (var i = 0; i < filenames.Length - 1; ++i)
                    {
                        var entry = zip.CreateEntry(filenames[i]);
                        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                        byte[] imageBytes = await client.GetByteArrayAsync($"https://localhost:7088/uploads/{filenames[i]}");
                        using (MemoryStream fileStream = new MemoryStream(imageBytes))
                        using (var entryStream = entry.Open())
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                    var result = System.Tuple.Create(ms, zipName);
                    return result;
                }
            }
        }

        public int deleteAllFile(string filename)
        {
            string[] documentid = filename.Split(",");
            int requestid = 0;
            for(int i=0;i<documentid.Length-1;++i)
            {
                var document = _db.RequestWiseFiles.FirstOrDefault(r=>r.RequestWiseFileId == int.Parse(documentid[i]));
                document.IsDeleted = new BitArray(new[] { true });
                _db.RequestWiseFiles.Update(document);
                _db.SaveChanges();
                requestid = document.RequestId;
            }
            return requestid;
        }

        public async Task<bool> sendDocumentsMail(string filename)
        {

            int retryCount = 1;
            bool success = false;

            while (retryCount <= 3 && !success) // Set retry limit
            {
                string[] documentid = filename.Split(",");
                var document = _db.RequestWiseFiles.Include(r => r.Request).FirstOrDefault(r => r.RequestWiseFileId == int.Parse(documentid[0]));
                var user = _db.RequestClients.FirstOrDefault(u => u.RequestClientId == document.Request.RequestClientId);
                string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                string senderPassword = "shahkandarp2430"; // Replace with your actual password (store securely)
                var platformTitle = "HalloDoc";
                var subject = "Documents - HalloDoc";
                var body = $"Hello {user.FirstName} {user.FirstName},<br />We have attached few important documents in order to update about you with the progress of your request.<br /><br />Regards,<br/>{platformTitle}<br/>";
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);
                try
                {

                    SmtpClient client = new SmtpClient("smtp.office365.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(senderEmail, senderPassword),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false
                    };

                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, "HalloDoc"),
                        Subject = subject,
                        IsBodyHtml = true,
                        Body = body
                    };

                    mailMessage.To.Add(user.Email);

                    for (var i = 0; i < documentid.Length - 1; ++i)
                    {
                        var doc = _db.RequestWiseFiles.Include(r => r.Request).FirstOrDefault(r => r.RequestWiseFileId == int.Parse(documentid[i]));
                        string filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", doc.FileName);
                        var fileInfo = new FileInfo(filePath);
                        var memoryStream = new MemoryStream();
                        using (var stream = fileInfo.OpenRead())
                        {
                            stream.CopyTo(memoryStream);
                        }
                        memoryStream.Position = 0;
                        string fileName = fileInfo.Name;
                        mailMessage.Attachments.Add(new Attachment(memoryStream, fileName));
                    }
                    await client.SendMailAsync(mailMessage);

                    
                    success = true;
                    LogEmail(body, subject, user.Email, document.Request.ConfirmationNumber, document.Request.RequestId, -1, -1, true, retryCount,1);
                    break;
                }
                catch (Exception ex)
                {

                    if (retryCount >= 3) 
                    {
                        LogEmail(body,subject, user.Email,document.Request.ConfirmationNumber, document.Request.RequestId, -1, -1, false, retryCount,1);
                    }
                    retryCount++;
                }
            }

            return success;
            
        }

        public void LogEmail(string emailTemplate,string subject,string userEmail,string confirmation_no,int request_id,int admin_id,int physician_id , bool success, int retryCount,int role_id)
        {
            if(role_id == 1)
            {
                var emailLog = new EmailLog
                {
                    EmailTemplate = emailTemplate,
                    SubjectName = subject,
                    EmailId = userEmail,
                    ConfirmationNumber = confirmation_no,
                    RequestId = request_id == -1 ? null : request_id,
                    IsEmailSent = new BitArray(new[] { success }) ,
                    SentTries = retryCount,
                    CreateDate = DateTime.Now,
                    RoleId = role_id,
                    SentDate = DateTime.Now,

                };
                _db.EmailLogs.Add(emailLog);
                _db.SaveChanges();
            }
            else if(role_id == 3)
            {
                var emailLog = new EmailLog
                {
                    EmailTemplate = emailTemplate,
                    SubjectName = subject,
                    EmailId = userEmail,
                    ConfirmationNumber = confirmation_no,
                    PhysicianId = physician_id == -1 ? null : physician_id,
                    IsEmailSent = new BitArray(new[] { success }),
                    SentTries = retryCount,
                    CreateDate = DateTime.Now,
                    RoleId = role_id,
                    SentDate = DateTime.Now

                };
                _db.EmailLogs.Add(emailLog);
                _db.SaveChanges();
            }
            else if(role_id == 2)
            {
                var emailLog = new EmailLog
                {
                    EmailTemplate = emailTemplate,
                    SubjectName = subject,
                    EmailId = userEmail,
                    ConfirmationNumber = confirmation_no,
                    AdminId = admin_id == -1 ? null : admin_id,
                    IsEmailSent = new BitArray(new[] { success }),
                    SentTries = retryCount,
                    CreateDate = DateTime.Now,
                    RoleId = role_id,
                    SentDate = DateTime.Now

                };
                _db.EmailLogs.Add(emailLog);
                _db.SaveChanges();
            }
            else
            {
                var emailLog = new EmailLog
                {
                    EmailTemplate = emailTemplate,
                    SubjectName = subject,
                    EmailId = userEmail,
                    ConfirmationNumber = confirmation_no,
                    IsEmailSent = new BitArray(new[] { success }),
                    SentTries = retryCount,
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now

                };
                _db.EmailLogs.Add(emailLog);
                _db.SaveChanges();
            }
            
        }

        public PasswordReset getPasswordReset(string token)
        {
            return _db.PasswordResets.FirstOrDefault(u => u.Token == token); 
        }

        public bool resetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            try
            {
                PasswordReset passwordReset = _db.PasswordResets.FirstOrDefault(u => u.Token == resetPasswordViewModel.Token);
                AspNetUser aspNetUser = _db.AspNetUsers.FirstOrDefault(u => u.Email == passwordReset.Email);
                var passwordHasher = new PasswordHasher<AspNetUser>();
                aspNetUser.PasswordHash = passwordHasher.HashPassword(aspNetUser, resetPasswordViewModel.Password);
                _db.AspNetUsers.Update(aspNetUser);
                _db.SaveChanges();
                passwordReset.IsUpdated = true;
                _db.PasswordResets.Update(passwordReset);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public List<Physician> getPhysician(int regionid)
        {
            return _db.Physicians.Where( p=>p.RegionId == regionid && p.IsDeleted == new BitArray(new[] { false }) ).ToList();
        }

        public bool assignCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            try
            {

                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);

                Request request = _db.Requests.FirstOrDefault(r=>r.RequestId == adminDashboardViewModel.RequestId);
                request.Status = 2;
                request.ModifiedDate = DateTime.Now;
                request.PhysicianId = adminDashboardViewModel.PhysicianId;
                request.AcceptedDate = DateTime.Now;
                _db.Requests.Update(request);

                Physician physician = _db.Physicians.FirstOrDefault(p=>p.PhysicianId == adminDashboardViewModel.PhysicianId);

                RequestStatusLog requestStatusLog = new RequestStatusLog
                {
                    RequestId = (int)adminDashboardViewModel.RequestId,
                    Status = 2,
                    Notes = $"Admin transferred to Dr. {physician.FirstName} on {DateTime.Now.ToString("MMMM dd,yyyy")} at {string.Format("{0:hh:mm:ss tt}", DateTime.Now)} : {adminDashboardViewModel.Description}",
                    CreatedDate = DateTime.Now,
                    TransToPhysicianId = adminDashboardViewModel.PhysicianId,
                    PhysicianId = adminDashboardViewModel.PhysicianId,
                    AdminId = cookieModel.userId
                };
                _db.RequestStatusLogs.Add(requestStatusLog);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }
        public bool transferCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            try
            {
                Request request = _db.Requests.FirstOrDefault(r=>r.RequestId == adminDashboardViewModel.RequestId);
                request.Status = 2;
                request.ModifiedDate = DateTime.Now;
                request.PhysicianId = adminDashboardViewModel.PhysicianId;
                _db.Requests.Update(request);

                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);

                Physician physician = _db.Physicians.FirstOrDefault(p=>p.PhysicianId == adminDashboardViewModel.PhysicianId);

                RequestStatusLog requestStatusLog = new RequestStatusLog
                {
                    RequestId = (int)adminDashboardViewModel.RequestId,
                    Status = 2,
                    Notes = $"Admin transferred to Dr. {physician.FirstName} on {DateTime.Now.ToString("MMMM dd,yyyy")} at {string.Format("{0:hh:mm:ss tt}", DateTime.Now)} : {adminDashboardViewModel.Description}",
                    CreatedDate = DateTime.Now,
                    TransToPhysicianId = adminDashboardViewModel.PhysicianId,
                    PhysicianId = adminDashboardViewModel.PhysicianId,
                    AdminId = cookieModel.userId
                };
                _db.RequestStatusLogs.Add(requestStatusLog);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public bool isSamePhysician(AdminDashboardViewModel adminDashboardViewModel)
        {
            Request request = _db.Requests.FirstOrDefault(r=>r.RequestId == adminDashboardViewModel.RequestId);
            if(request.PhysicianId == adminDashboardViewModel.PhysicianId)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> sendAgreement(AdminDashboardViewModel adminDashboardViewModel)
        {
            int retryCount = 1;
            bool success = false;

            while (retryCount <= 3 && !success) // Set retry limit
            {
                
                var user = _db.Requests.Include(r=>r.RequestClient).FirstOrDefault(u => u.RequestClientId == adminDashboardViewModel.RequestId);
                string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                string senderPassword = "shahkandarp2430"; // Replace with your actual password (store securely)
                var platformTitle = "HalloDoc";
                var subject = "Agreement - HalloDoc";
                var inviteLink = $"https://localhost:7088/Agreement/Index/{adminDashboardViewModel.RequestId}";
                var body = $"Hello {user.RequestClient.FirstName} {user.RequestClient.LastName},<br />Please review agreement and accept it so that we can start your treatment,<br /><br /><a href='{inviteLink}'>Review Agreement</a><br /><br />Regards,<br/>{platformTitle}<br/>";
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);
                try
                {

                    SmtpClient client = new SmtpClient("smtp.office365.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(senderEmail, senderPassword),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false
                    };

                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, "HalloDoc"),
                        Subject = subject,
                        IsBodyHtml = true,
                        Body = body
                    };

                    mailMessage.To.Add(adminDashboardViewModel.Mail_Email);

                    
                    await client.SendMailAsync(mailMessage);


                    success = true;
                    LogEmail(body, subject, adminDashboardViewModel.Mail_Email, user.ConfirmationNumber, user.RequestId,-1, -1, true, retryCount,1);
                    break;
                }
                catch (Exception ex)
                {

                    if (retryCount >= 3)
                    {
                        LogEmail(body, subject, adminDashboardViewModel.Mail_Email, user.ConfirmationNumber, user.RequestId, -1, -1, false, retryCount,1);
                    }
                    retryCount++;
                }
            }

            retryCount = 1;
            success = false;

            while (retryCount <= 3 && !success) // Set retry limit
            {

                var user = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestClientId == adminDashboardViewModel.RequestId);
                var platformTitle = "HalloDoc";
                var inviteLink = $"https://localhost:7088/Agreement/Index/{adminDashboardViewModel.RequestId}";

                var accountSid = _configuration["Twilio:accountSid"];
                var authToken = _configuration["Twilio:authToken"];
                var twilionumber = _configuration["Twilio:twilioNumber"];
                var messageBody = $"Hello {user.RequestClient.FirstName} {user.RequestClient.LastName},\nPlease review agreement and accept it so that we can start your treatment,\n\n{inviteLink}\n\nRegards,\n{platformTitle}";

                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);
                try
                {

                    TwilioClient.Init(accountSid, authToken);

                    var message = MessageResource.Create(
                        from: new Twilio.Types.PhoneNumber(twilionumber),
                        body: messageBody,
                        to: new Twilio.Types.PhoneNumber("+91" + adminDashboardViewModel.Mail_PhoneNumber)
                    );


                    success = true;
                    LogSMS(messageBody, adminDashboardViewModel.Mail_PhoneNumber, user.ConfirmationNumber, user.RequestId, -1, -1, true, retryCount,1);
                    break;
                }
                catch (Exception ex)
                {

                    if (retryCount >= 3)
                    {
                        LogSMS(messageBody, adminDashboardViewModel.Mail_PhoneNumber, user.ConfirmationNumber, user.RequestId, -1, -1, false, retryCount,1);
                    }
                    retryCount++;
                }
            }

            return success;
        }

        public void LogSMS(string SmsTemplate, string userPhone, string confirmation_no, int request_id, int admin_id, int physician_id, bool success, int retryCount,int role_id)
        {
            if (role_id == 1)
            {
                var smslog = new Smslog
                {
                    Smstemplate = SmsTemplate,
                    MobileNumber = userPhone,
                    ConfirmationNumber = confirmation_no,
                    RequestId = request_id,
                    IsSmssent = new BitArray(new[] { success }),
                    SentTries = retryCount,
                    CreateDate = DateTime.Now,
                    RoleId = role_id,
                    SentDate = DateTime.Now

                };
                _db.Smslogs.Add(smslog);
                _db.SaveChanges();
            }
            else if(role_id == 3)
            {
                var smslog = new Smslog
                {
                    Smstemplate = SmsTemplate,
                    MobileNumber = userPhone,
                    ConfirmationNumber = confirmation_no,
                    PhysicianId = physician_id,
                    IsSmssent = new BitArray(new[] { success }),
                    SentTries = retryCount,
                    CreateDate = DateTime.Now,
                    RoleId = role_id,
                    SentDate = DateTime.Now

                };
                _db.Smslogs.Add(smslog);
                _db.SaveChanges();
            }
            else if(role_id == 2)
            {
                var smslog = new Smslog
                {
                    Smstemplate = SmsTemplate,
                    MobileNumber = userPhone,
                    ConfirmationNumber = confirmation_no,
                    AdminId = admin_id,
                    IsSmssent = new BitArray(new[] { success }),
                    SentTries = retryCount,
                    CreateDate = DateTime.Now,
                    RoleId = role_id,
                    SentDate = DateTime.Now

                };
                _db.Smslogs.Add(smslog);
                _db.SaveChanges();
            }
            else
            {
                var smslog = new Smslog
                {
                    Smstemplate = SmsTemplate,
                    MobileNumber = userPhone,
                    ConfirmationNumber = confirmation_no,
                    IsSmssent = new BitArray(new[] { success }),
                    SentTries = retryCount,
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now

                };
                _db.Smslogs.Add(smslog);
                _db.SaveChanges();
            }


        }

        public bool blockCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            try
            {
                Request request = _db.Requests.Include(r=>r.RequestClient).FirstOrDefault(b=>b.RequestId == adminDashboardViewModel.RequestId);
                request.Status = 11;
                request.ModifiedDate = DateTime.Now;
                _db.Requests.Update(request);

                RequestStatusLog requestStatusLog = new RequestStatusLog()
                {
                    RequestId = (int)adminDashboardViewModel.RequestId,
                    Status = 11,
                    CreatedDate = DateTime.Now
                };
                _db.RequestStatusLogs.Add(requestStatusLog);

                BlockRequest blockRequest = new BlockRequest()
                {
                    PhoneNumber = request.RequestClient.PhoneNumber,
                    Email = request.RequestClient.Email,
                    Reason = adminDashboardViewModel.BlockReason,
                    CreatedDate = DateTime.Now,
                    RequestId = adminDashboardViewModel.RequestId.ToString(),
                    IsActive = new BitArray(new[] { false }),
                    Name = string.Concat(request.RequestClient.FirstName , " " , request.RequestClient.LastName)
                };
                _db.BlockRequests.Add(blockRequest);
                _db.SaveChanges();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool clearCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            try
            {
                Request request = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(b => b.RequestId == adminDashboardViewModel.RequestId);
                request.Status = 10;
                request.ModifiedDate = DateTime.Now;
                _db.Requests.Update(request);

                RequestStatusLog requestStatusLog = new RequestStatusLog()
                {
                    RequestId = (int)adminDashboardViewModel.RequestId,
                    Status = 10,
                    CreatedDate = DateTime.Now
                };
                _db.RequestStatusLogs.Add(requestStatusLog);
                _db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public OrdersViewModel orders(int id)
        {
            var healthProfessionals = _db.HealthProfessionalTypes.Where(h=>h.IsDeleted == new BitArray(new[] { false })).ToList();
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus
            };

            OrdersViewModel ordersViewModel = new OrdersViewModel()
            {
                RequestId = id,
                healthProfessionalTypes = healthProfessionals,
                adminNavbarViewModel = adminNavbarViewModel
            };
            return ordersViewModel;
        }

        public List<HealthProfessional> getBusiness(int professionid)
        {
            return _db.HealthProfessionals.Where(h=>h.Profession == professionid && h.IsDeleted == new BitArray(new[] { false })).ToList();
        }
        
        public HealthProfessional getBusinessData(int businessid)
        {
            return _db.HealthProfessionals.FirstOrDefault(h=>h.VendorId == businessid);
        }

        public bool placeOrder(OrdersViewModel ordersViewModel)
        {
            try
            {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            var admin = _db.Admins.FirstOrDefault(a=>a.AdminId == cookieModel.userId);
                OrderDetail orderDetail = new OrderDetail()
                {
                    VendorId = ordersViewModel.business_id,
                    RequestId = ordersViewModel.RequestId,
                    FaxNumber = ordersViewModel.Business_fax,
                    Email = ordersViewModel.Business_email,
                    BusinessContact = ordersViewModel.Business_contact,
                    Prescription = ordersViewModel.prescription,
                    NoOfRefill = ordersViewModel.numberOfRefills == -1 ? null:ordersViewModel.numberOfRefills,
                    CreatedDate = DateTime.Now,
                    CreatedBy = string.Concat(admin.FirstName," ",admin.LastName)
                };
                _db.OrderDetails.Add(orderDetail);
                _db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public AdminProfileViewModel getAdmin()
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            HalloDoc.Admin admin = _db.Admins.Include(a => a.AspNetUser).FirstOrDefault(a => a.AdminId == cookieModel.userId);
            List<Region> regions = _db.Regions.ToList();
            IQueryable<AdminRegion> adminRegions = _db.AdminRegions.Where(a=>a.AdminId == cookieModel.userId);
            List<CheckboxViewModel> checkboxViewModels = new List<CheckboxViewModel>();
            List<Role> roles = _db.Roles.Where(r=>r.AccountType == 1 && r.IsDeleted == new BitArray(new[] { false })).ToList();
            for(var i=0;i<regions.Count;i++)
            {
                checkboxViewModels.Add(new CheckboxViewModel()
                {
                    Id = regions[i].RegionId,
                    Name = regions[i].Name,
                    isChecked = adminRegions.FirstOrDefault(a => a.RegionId == regions[i].RegionId) == null ? false : true
                });
            }


            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Profile",
                menus = cookieModel.menus
            };

            AdminProfileViewModel adminProfile = new AdminProfileViewModel()
            { 
                UserName = admin.AspNetUser.UserName,
                status = 1,
                role_id = admin.RoleId,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.Email,
                ConfirmEmail = admin.Email,
                adminNavbarViewModel = adminNavbarViewModel,
                PhoneNumber = admin.Mobile,
                Alt_PhoneNumber = admin.AltPhone,
                checkboxViewModels = checkboxViewModels,
                Address1 = admin.Address1,
                Address2 = admin.Address2,
                City = admin.City,
                RegionId = admin.RegionId,
                ZipCode = admin.Zip,
                roles = roles
            };

            return adminProfile;

        }

        public bool updateProfile(AdminProfileViewModel adminProfileViewModel)
        {
            try
            {
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);

                HalloDoc.Admin admin = _db.Admins.Include(a => a.AspNetUser).FirstOrDefault(a => a.AdminId == cookieModel.userId);
                IQueryable<AdminRegion> adminRegions = _db.AdminRegions.Where(a => a.AdminId == cookieModel.userId);
                admin.FirstName = adminProfileViewModel.FirstName ?? admin.FirstName;
                admin.LastName = adminProfileViewModel.LastName ?? admin.LastName;
                admin.Email = adminProfileViewModel.Email ?? admin.Email;
                admin.Mobile = adminProfileViewModel.PhoneNumber ?? admin.Mobile;
                admin.Address1 = adminProfileViewModel.Address1 ?? admin.Address1;
                admin.Address2 = adminProfileViewModel.Address2 ?? admin.Address2;
                admin.City = adminProfileViewModel.City ?? admin.City;
                admin.RegionId = adminProfileViewModel.RegionId ?? admin.RegionId;
                admin.Zip = adminProfileViewModel.ZipCode ?? admin.Zip;
                admin.AltPhone = adminProfileViewModel.Alt_PhoneNumber ?? admin.AltPhone;
                admin.ModifiedDate = DateTime.Now;
                admin.ModifiedBy = cookieModel.aspId;

                _db.Admins.Update(admin);

                for(var i=0;i<adminProfileViewModel.checkboxViewModels.Count;++i)
                {
                    if (adminProfileViewModel.checkboxViewModels[i].isChecked == true && adminRegions.FirstOrDefault(a => a.RegionId == adminProfileViewModel.checkboxViewModels[i].Id) == null)
                    {
                        AdminRegion adminRegion = new AdminRegion()
                        {
                            AdminId = cookieModel.userId,
                            RegionId = (int)adminProfileViewModel.checkboxViewModels[i].Id
                        };
                        _db.AdminRegions.Add(adminRegion);
                    }
                    else if(adminProfileViewModel.checkboxViewModels[i].isChecked == false && adminRegions.FirstOrDefault(a => a.RegionId == adminProfileViewModel.checkboxViewModels[i].Id) != null)
                    {
                        AdminRegion adminRegion = adminRegions.FirstOrDefault(a => a.RegionId == adminProfileViewModel.checkboxViewModels[i].Id);
                        _db.AdminRegions.Remove(adminRegion);
                    }
                }

                _db.SaveChanges();

                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public bool resetPasswordProfile(string password)
        {
            try
            {
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);

                AspNetUser aspNetUser = _db.AspNetUsers.FirstOrDefault(a=>a.Id == cookieModel.aspId);
                var passwordHasher = new PasswordHasher<AspNetUser>();
                aspNetUser.PasswordHash = passwordHasher.HashPassword(aspNetUser, password);
                _db.AspNetUsers.Update(aspNetUser);
                _db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public Request getRequest(int id)
        {
            return _db.Requests.FirstOrDefault(u => u.RequestId == id);
        }

        public bool agree(int id)
        {
            try
            {
                var request = _db.Requests.FirstOrDefault(u => u.RequestId == id);
                request.Status = 3;
                request.ModifiedDate = DateTime.Now;
                _db.Requests.Update(request);

                RequestStatusLog requestStatusLog = new RequestStatusLog
                {
                    Status = 3,
                    RequestId = id,
                    CreatedDate = DateTime.Now,
                };
                _db.RequestStatusLogs.Add(requestStatusLog);
                _db.SaveChanges();

                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public bool disagree(int id, string notes)
        {
            try
            {
                var request = _db.Requests.FirstOrDefault(u => u.RequestId == id);
                request.Status = 7;
                request.ModifiedDate = DateTime.Now;
                _db.Requests.Update(request);

                RequestStatusLog requestStatusLog = new RequestStatusLog
                {
                    Status = 7,
                    RequestId = id,
                    CreatedDate = DateTime.Now,
                    Notes = notes
                };
                _db.RequestStatusLogs.Add(requestStatusLog);
                _db.SaveChanges();

                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public EncounterFormViewModel getEncounterFormDetails(int id)
        {
            Request request = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(r=>r.RequestId == id);
            EncounterForm encounterForm = _db.EncounterForms.FirstOrDefault(r => r.RequestId == id);
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus
            };

            EncounterFormViewModel encounterFormViewModel = new EncounterFormViewModel()
            {
                RequestId = id,
                adminNavbarViewModel = adminNavbarViewModel,
                FirstName = request.RequestClient.FirstName,
                LastName = request.RequestClient.LastName,
                Email = request.RequestClient.Email,
                PhoneNumber = request.RequestClient.PhoneNumber,
                DateOfBirth = DateTime.Parse($"{request.RequestClient.IntYear}-{request.RequestClient.StrMonth}-{request.RequestClient.IntDate}"),
                Location = string.Concat(request.RequestClient.Street, ',', request.RequestClient.City, ',', request.RequestClient.State, ',', request.RequestClient.ZipCode),
                Date = encounterForm?.Date ?? DateTime.Now,
                Illness_history = encounterForm?.HistoryIllness,
                Medical_history = encounterForm?.MedicalHistory,
                Medications = encounterForm?.Medications ,
                Allergies = encounterForm?.Allergies ,
                Temp = encounterForm?.Temp,
                Hr = encounterForm?.Hr,
                Rr = encounterForm?.Rr,
                BpS = encounterForm?.BpS,
                BpD = encounterForm?.BpD,
                O2 = encounterForm?.O2,
                Pain = encounterForm?.Pain,
                Heent = encounterForm?.Heent,
                Cv = encounterForm?.Cv,
                Chest = encounterForm?.Chest,
                Abd = encounterForm?.Abd,
                Extr = encounterForm?.Extr,
                Skin = encounterForm?.Skin,
                Neuro = encounterForm?.Neuro,
                Other = encounterForm?.Other,
                Diagnosis = encounterForm?.Diagnosis,
                TreatmentPlan = encounterForm?.TreatmentPlan,
                MedicationDispensed = encounterForm?.MedicationDispensed,
                Procedures = encounterForm?.Procedures,
                FollowUp = encounterForm?.FollowUp,
            };
            return encounterFormViewModel;
        }

        public bool updateEncounterForm(EncounterFormViewModel encounterFormViewModel)
        {
            try
            {
                EncounterForm encounterForm = _db.EncounterForms.FirstOrDefault(r => r.RequestId == encounterFormViewModel.RequestId);
                if (encounterForm == null)
                {
                    EncounterForm encounter = new EncounterForm()
                    {
                        Date = DateTime.Now,
                        RequestId = (int)encounterFormViewModel.RequestId,
                        HistoryIllness = encounterFormViewModel?.Illness_history,
                        MedicalHistory = encounterFormViewModel?.Medical_history,
                        Medications = encounterFormViewModel?.Medications,
                        Allergies = encounterFormViewModel?.Allergies,
                        Temp = encounterFormViewModel?.Temp,
                        Hr = encounterFormViewModel?.Hr,
                        Rr = encounterFormViewModel?.Rr,
                        BpS = encounterFormViewModel?.BpS,
                        BpD = encounterFormViewModel?.BpD,
                        O2 = encounterFormViewModel?.O2,
                        Pain = encounterFormViewModel?.Pain,
                        Heent = encounterFormViewModel?.Heent,
                        Cv = encounterFormViewModel?.Cv,
                        Chest = encounterFormViewModel?.Chest,
                        Abd = encounterFormViewModel?.Abd,
                        Extr = encounterFormViewModel?.Extr,
                        Skin = encounterFormViewModel?.Skin,
                        Neuro = encounterFormViewModel?.Neuro,
                        Other = encounterFormViewModel?.Other,
                        Diagnosis = encounterFormViewModel?.Diagnosis,
                        TreatmentPlan = encounterFormViewModel?.TreatmentPlan,
                        MedicationDispensed = encounterFormViewModel?.MedicationDispensed,
                        Procedures = encounterFormViewModel?.Procedures,
                        FollowUp = encounterFormViewModel?.FollowUp,
                    };

                    _db.EncounterForms.Add(encounter);
                    _db.SaveChanges();

                }
                else
                {
                    encounterForm.Date = encounterFormViewModel.Date;
                    encounterForm.HistoryIllness = encounterFormViewModel?.Illness_history;
                    encounterForm.MedicalHistory = encounterFormViewModel?.Medical_history;
                    encounterForm.Medications = encounterFormViewModel?.Medications;
                    encounterForm.Allergies = encounterFormViewModel?.Allergies;
                    encounterForm.Temp = encounterFormViewModel?.Temp;
                    encounterForm.Hr = encounterFormViewModel?.Hr;
                    encounterForm.Rr = encounterFormViewModel?.Rr;
                    encounterForm.BpS = encounterFormViewModel?.BpS;
                    encounterForm.BpD = encounterFormViewModel?.BpD;
                    encounterForm.O2 = encounterFormViewModel?.O2;
                    encounterForm.Pain = encounterFormViewModel?.Pain;
                    encounterForm.Heent = encounterFormViewModel?.Heent;
                    encounterForm.Cv = encounterFormViewModel?.Cv;
                    encounterForm.Chest = encounterFormViewModel?.Chest;
                    encounterForm.Abd = encounterFormViewModel?.Abd;
                    encounterForm.Extr = encounterFormViewModel?.Extr;
                    encounterForm.Skin = encounterFormViewModel?.Skin;
                    encounterForm.Neuro = encounterFormViewModel?.Neuro;
                    encounterForm.Other = encounterFormViewModel?.Other;
                    encounterForm.Diagnosis = encounterFormViewModel?.Diagnosis;
                    encounterForm.TreatmentPlan = encounterFormViewModel?.TreatmentPlan;
                    encounterForm.MedicationDispensed = encounterFormViewModel?.MedicationDispensed;
                    encounterForm.Procedures = encounterFormViewModel?.Procedures;
                    encounterForm.FollowUp = encounterFormViewModel?.FollowUp;

                    _db.EncounterForms.Update(encounterForm);
                    _db.SaveChanges();
                }

                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public CloseCaseViewModel getCloseCase(int id)
        {
            var request = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id);
            var documents = _db.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == id && u.IsDeleted.Equals(new BitArray(new[] { false }))).ToList();
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus
            };

            CloseCaseViewModel closeCaseViewModel = new CloseCaseViewModel()
            {
                RequestId = id,
                patient_name = string.Concat(request.RequestClient.FirstName, ' ', request.RequestClient.LastName),
                confirmation_number = request.ConfirmationNumber,
                requestWiseFiles = documents,
                uploader_name = string.Concat(request.FirstName, ' ', request.LastName),
                adminNavbarViewModel = adminNavbarViewModel,
                FirstName = request.RequestClient.FirstName,
                LastName = request.RequestClient.LastName,
                PhoneNumber = request.RequestClient.PhoneNumber,
                Email = request.RequestClient.Email,
                DateOfBirth = DateTime.Parse($"{request.RequestClient.IntYear}-{request.RequestClient.StrMonth}-{request.RequestClient.IntDate}")
            };
            return closeCaseViewModel;
        }

        public bool updateCloseCase(CloseCaseViewModel closeCaseViewModel)
        {
            try
            {
                Request request = _db.Requests.FirstOrDefault(r=>r.RequestId == closeCaseViewModel.RequestId);
                RequestClient requestClient = _db.RequestClients.FirstOrDefault(r=>r.RequestClientId == request.RequestId);
                requestClient.PhoneNumber = closeCaseViewModel.PhoneNumber;
                requestClient.Email = closeCaseViewModel.Email;
                _db.RequestClients.Update(requestClient);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public bool closeCase(int id)
        {
            try
            {
                Request request = _db.Requests.FirstOrDefault(r=>r.RequestId == id);
                request.Status = 9;
                request.ModifiedDate = DateTime.Now;
                _db.Requests.Update(request);

                RequestStatusLog requestStatusLog = new RequestStatusLog()
                {
                    RequestId = request.RequestId,
                    Status = 9,
                    CreatedDate = DateTime.Now,
                };
                _db.RequestStatusLogs.Add(requestStatusLog);
                _db.SaveChanges();

                RequestClosed requestClosed = new RequestClosed()
                {
                    RequestId = request.RequestId,
                    RequestStatusLogId = requestStatusLog.RequestStatusLogId
                };

                _db.RequestCloseds.Add(requestClosed);
                _db.SaveChanges();

                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public ProviderViewModel getProviderPageDetails(int id=-1,int page=1,int pageSize=10)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            IQueryable<Physician> physicians = _db.Physicians.Where(p=>p.IsDeleted == new BitArray(new[] { false })).OrderByDescending(e => e.CreatedDate);

            if(id != -1 && id!=null)
            {
                physicians = physicians.Where(p => p.RegionId == id);
            }

            List<Physician> physician = physicians.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            List<PhysicianProvider> physicianProviders = new List<PhysicianProvider>();

            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);

            for (var i=0;i< physician.Count;++i)
            {
                PhysicianNotification physicianNotification = _db.PhysicianNotifications.FirstOrDefault(p=>p.PhysicianId == physician[i].PhysicianId);
                Role role = _db.Roles.FirstOrDefault(r => r.RoleId == physician[i].RoleId);
                List<Shift> shift = _db.Shifts.Where(s=>s.PhysicianId == physician[i].PhysicianId && s.StartDate <= DateOnly.FromDateTime(DateTime.Now)).ToList();
                List<ShiftDetail> shiftDetails = new List<ShiftDetail>();
                bool isActive = false;
                foreach(var sh in shift)
                {
                    List<ShiftDetail> shiftDetail = _db.ShiftDetails.Where(p => p.ShiftId == sh.ShiftId && p.ShiftDate.Date == DateTime.Now.Date).ToList();
                    if(shiftDetail.Count>0)
                    {
                        shiftDetails.AddRange(shiftDetail);
                    }
                }
                foreach(var sd in shiftDetails)
                {
                    if(currentTime.IsBetween(sd.StartTime, sd.EndTime))
                    {
                        isActive = true;
                        break;
                    }
                }
                physicianProviders.Add(new PhysicianProvider()
                {
                    isStopNotification = physicianNotification == null?false: physicianNotification.IsNotificationStopped[0],
                    name = string.Concat(physician[i].FirstName,", ", physician[i].LastName),
                    status = physician[i].Status,
                    role = role?.Name ?? "-",
                    physicianId = physician[i].PhysicianId,
                    oncallstatus = isActive == true? "On Call":"Unavailable",
                });
            }

            List<Region> regions = _db.Regions.ToList();

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Provider",
                menus = cookieModel.menus
            };

            ProviderViewModel providerViewModel = new ProviderViewModel()
            {
                adminNavbarViewModel = adminNavbarViewModel,
                physicianproviders = physicianProviders,
                regions = regions,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = physicians.Count(),
                TotalPages = (int)Math.Ceiling((double)physicians.Count() / pageSize)
            };

            return providerViewModel;

        }

        public bool changeNotification(int id,bool update)
        {
            try
            {
                PhysicianNotification physicianNotification = _db.PhysicianNotifications.FirstOrDefault(p=>p.PhysicianId == id);
                if(physicianNotification == null)
                {
                    PhysicianNotification physicianNotification1 = new PhysicianNotification()
                    {
                        PhysicianId = id,
                        IsNotificationStopped = new BitArray(new[] { update })
                    };
                    _db.PhysicianNotifications.Add(physicianNotification1);
                }
                else
                {
                    physicianNotification.IsNotificationStopped = new BitArray(new[] { update });
                    _db.PhysicianNotifications.Update(physicianNotification);
                }
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public async Task<bool> contactProvider(ProviderViewModel providerViewModel)
        {
            int retryCount = 1;
            bool success = false;

            if (providerViewModel.communication_type == "Email" || providerViewModel.communication_type == "Both")
            {
                while (retryCount <= 3 && !success) // Set retry limit
                {

                    var physician = _db.Physicians.FirstOrDefault(p => p.PhysicianId == providerViewModel.ProviderId);
                    string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                    string senderPassword = "shahkandarp2430"; // Replace with your actual password (store securely)
                    var platformTitle = "HalloDoc";
                    var subject = "Contact - HalloDoc";
                    var body = $"Hello {physician.FirstName} {physician.LastName},<br />{providerViewModel.message}<br /><br />Regards,<br/>{platformTitle}<br/>";
                    var request = _context.HttpContext.Request;
                    var token = request.Cookies["jwt"];
                    CookieModel cookieModel = _jwt.getDetails(token);
                    try
                    {

                        SmtpClient client = new SmtpClient("smtp.office365.com")
                        {
                            Port = 587,
                            Credentials = new NetworkCredential(senderEmail, senderPassword),
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false
                        };

                        MailMessage mailMessage = new MailMessage
                        {
                            From = new MailAddress(senderEmail, "HalloDoc"),
                            Subject = subject,
                            IsBodyHtml = true,
                            Body = body
                        };

                        mailMessage.To.Add(physician.Email);


                        await client.SendMailAsync(mailMessage);


                        success = true;
                        LogEmail(body, subject, physician.Email, null, -1, -1, physician.PhysicianId, true, retryCount, 3);
                        break;
                    }
                    catch (Exception ex)
                    {

                        if (retryCount >= 3)
                        {
                            LogEmail(body, subject, physician.Email, null, -1, -1, physician.PhysicianId, false, retryCount, 3);
                        }
                        retryCount++;
                    }
                }
            }

            if(providerViewModel.communication_type == "SMS" || providerViewModel.communication_type == "Both")
            {
                retryCount = 1;
                success = false;

                while (retryCount <= 3 && !success) // Set retry limit
                {

                    var physician = _db.Physicians.FirstOrDefault(p => p.PhysicianId == providerViewModel.ProviderId);
                    string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                    string senderPassword = "shahkandarp2430"; // Replace with your actual password (store securely)
                    var platformTitle = "HalloDoc";

                    var accountSid = _configuration["Twilio:accountSid"];
                    var authToken = _configuration["Twilio:authToken"];
                    var twilionumber = _configuration["Twilio:twilioNumber"];
                    var messageBody = $"Hello {physician.FirstName} {physician.LastName},\n{providerViewModel.message}\n\nRegards,\n{platformTitle}";

                    var request = _context.HttpContext.Request;
                    var token = request.Cookies["jwt"];
                    CookieModel cookieModel = _jwt.getDetails(token);
                    try
                    {

                        TwilioClient.Init(accountSid, authToken);

                        var message = MessageResource.Create(
                            from: new Twilio.Types.PhoneNumber(twilionumber),
                            body: messageBody,
                            to: new Twilio.Types.PhoneNumber("+91" + physician.Mobile)
                        );


                        success = true;
                        LogSMS(messageBody, physician.Mobile , null, -1, -1, physician.PhysicianId, true, retryCount, 3);
                        break;
                    }
                    catch (Exception ex)
                    {

                        if (retryCount >= 3)
                        {
                            LogSMS(messageBody, physician.Mobile, null, -1, -1, physician.PhysicianId, false, retryCount, 3);
                        }
                        retryCount++;
                    }
                }
            }


            return success;
        }

        public PhysicianAccountViewModel getCreatePhysicianDetails()
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Provider",
                menus = cookieModel.menus
            };

            List<Region> regions = _db.Regions.ToList();

            List<Role> roles = _db.Roles.Where(r => r.AccountType == 2 && r.IsDeleted == new BitArray(new[] { false })).ToList();

            List<CheckboxViewModel> checkboxViewModels = new List<CheckboxViewModel>();

            for(var i=0;i<regions.Count;++i)
            {
                checkboxViewModels.Add(new CheckboxViewModel()
                {
                    Name = regions[i].Name,
                    Id = regions[i].RegionId,
                    isChecked = false
                }); 
            }

            PhysicianAccountViewModel physicianAccountViewModel = new PhysicianAccountViewModel()
            {
                adminNavbarViewModel = adminNavbarViewModel,
                checkboxViewModels = checkboxViewModels,
                roles = roles
            };
            return physicianAccountViewModel;
        }

        public async Task<bool> createPhysician(PhysicianAccountViewModel physicianAccountViewModel)
        {
            var passwordHasher = new PasswordHasher<AspNetUser>();
            Region region = _db.Regions.FirstOrDefault(r=>r.RegionId == physicianAccountViewModel.RegionId);
            AspNetUser aspNetUser = new AspNetUser()
            {
                Email = physicianAccountViewModel.Email,
                UserName = string.Concat("MD.", physicianAccountViewModel.LastName.Substring(0, 1).ToUpper() , physicianAccountViewModel.LastName.Substring(1).ToLower() , "." , physicianAccountViewModel.FirstName.Substring(0, 1).ToUpper()),
                CreatedDate = DateTime.Now
            };
            aspNetUser.PasswordHash = passwordHasher.HashPassword(aspNetUser, physicianAccountViewModel.Password);
            _db.AspNetUsers.Add(aspNetUser);
            _db.SaveChanges();

            AspNetUserRole aspNetUserRole = new AspNetUserRole
            {
                UserId = aspNetUser.Id,
                RoleId = 3
            };

            _db.AspNetUserRoles.Add(aspNetUserRole);
            _db.SaveChanges();

            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            Physician physician = new Physician()
            {
                AspNetUserId = aspNetUser.Id,
                FirstName = physicianAccountViewModel.FirstName,
                LastName = physicianAccountViewModel.LastName,
                Email = physicianAccountViewModel.Email,
                Mobile = physicianAccountViewModel.PhoneNumber,
                AltPhone = physicianAccountViewModel.AltPhone,
                MedicalLicense = physicianAccountViewModel.MedicalLicense,
                Npinumber = physicianAccountViewModel.NPI_Number,
                SyncEmailAddress = physicianAccountViewModel.Sync_Email,
                Address1 = physicianAccountViewModel.Address1,
                Address2 = physicianAccountViewModel.Address2,
                City = physicianAccountViewModel.City,
                RegionId = physicianAccountViewModel.RegionId,
                Zip = physicianAccountViewModel.Zipcode,
                BusinessName = physicianAccountViewModel.BusinessName,
                BusinessWebsite = physicianAccountViewModel.BusinessWebsite,
                AdminNotes = physicianAccountViewModel.Admin_Notes,
                IsAgreementDoc = new BitArray(new[] { physicianAccountViewModel.IsAgreementDoc }),
                IsBackgroundDoc = new BitArray(new[] { physicianAccountViewModel.IsBackgroundDoc }),
                IsLicenseDoc = new BitArray(new[] { physicianAccountViewModel.IsLicenseDoc }),
                IsNonDisclosureDoc = new BitArray(new[] { physicianAccountViewModel.IsNonDisclosureDoc }),
                IsCredentialDoc = new BitArray(new[] { physicianAccountViewModel.IsCredentialDoc }),
                IsDeleted = new BitArray(new[] { false }),
                Status = 2,
                RoleId = physicianAccountViewModel.role_id,
                CreatedDate = DateTime.Now,
                CreatedBy = cookieModel.aspId
            };

            _db.Physicians.Add(physician);
            _db.SaveChanges();

            var filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}");
            Directory.CreateDirectory(filePath);

            if (physicianAccountViewModel.Signature != null && physicianAccountViewModel.Signature.Length > 0)
            {
                var filePathh = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}", physicianAccountViewModel.Signature.FileName);
                using (var stream = System.IO.File.Create(filePathh))
                {
                    await physicianAccountViewModel.Signature.CopyToAsync(stream);
                }
            }

            if (physicianAccountViewModel.Photo != null && physicianAccountViewModel.Photo.Length > 0)
            {
                var filePathh = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}", physicianAccountViewModel.Photo.FileName);
                using (var stream = System.IO.File.Create(filePathh))
                {
                    await physicianAccountViewModel.Photo.CopyToAsync(stream);
                }
            }

            physician.Signature = physicianAccountViewModel.Signature.FileName;
            physician.Photo = physicianAccountViewModel.Signature.FileName;

            _db.Physicians.Update(physician);
            _db.SaveChanges();

            for(var i=0;i<physicianAccountViewModel.checkboxViewModels.Count;++i)
            {
                if (physicianAccountViewModel.checkboxViewModels[i].isChecked == true)
                {
                    PhysicianRegion physicianRegion = new PhysicianRegion()
                    {
                        PhysicianId = physician.PhysicianId,
                        RegionId = (int)physicianAccountViewModel.checkboxViewModels[i].Id
                    };
                    _db.PhysicianRegions.Add(physicianRegion);
                    _db.SaveChanges();
                }
            }

            if(physicianAccountViewModel.IsAgreementDoc)
            {
                if (physicianAccountViewModel.AgreementDoc != null && physicianAccountViewModel.AgreementDoc.Length > 0)
                {
                    var filePathh = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}\\Agreement.pdf");
                    using (var stream = System.IO.File.Create(filePathh))
                    {
                        await physicianAccountViewModel.AgreementDoc.CopyToAsync(stream);
                    }
                }
            }

            if (physicianAccountViewModel.IsBackgroundDoc)
            {
                if (physicianAccountViewModel.BackgroundDoc != null && physicianAccountViewModel.BackgroundDoc.Length > 0)
                {
                    var filePathh = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}\\Background.pdf");
                    using (var stream = System.IO.File.Create(filePathh))
                    {
                        await physicianAccountViewModel.BackgroundDoc.CopyToAsync(stream);
                    }
                }
            }

            if (physicianAccountViewModel.IsLicenseDoc)
            {
                if (physicianAccountViewModel.LicenseDoc != null && physicianAccountViewModel.LicenseDoc.Length > 0)
                {
                    var filePathh = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}\\License.pdf");
                    using (var stream = System.IO.File.Create(filePathh))
                    {
                        await physicianAccountViewModel.LicenseDoc.CopyToAsync(stream);
                    }
                }
            }

            if (physicianAccountViewModel.IsNonDisclosureDoc)
            {
                if (physicianAccountViewModel.NonDisclosureDoc != null && physicianAccountViewModel.NonDisclosureDoc.Length > 0)
                {
                    var filePathh = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}\\NonDiscolsure.pdf");
                    using (var stream = System.IO.File.Create(filePathh))
                    {
                        await physicianAccountViewModel.NonDisclosureDoc.CopyToAsync(stream);
                    }
                }
            }

            if (physicianAccountViewModel.IsCredentialDoc)
            {
                if (physicianAccountViewModel.CredentialDoc != null && physicianAccountViewModel.CredentialDoc.Length > 0)
                {
                    var filePathh = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}\\HIPAA.pdf");
                    using (var stream = System.IO.File.Create(filePathh))
                    {
                        await physicianAccountViewModel.CredentialDoc.CopyToAsync(stream);
                    }
                }
            }

            PhysicianLocation physicianLocation = new PhysicianLocation()
            {
                PhysicianId = physician.PhysicianId,
                Latitude = physicianAccountViewModel?.lat != null ? physicianAccountViewModel?.lat : 0,
                Longitude = physicianAccountViewModel?.lng != null ? physicianAccountViewModel?.lng : 0,
                CreatedDate = DateTime.Now,
                PhysicianName = string.Concat(physician.FirstName, ' ', physician.LastName),
                Address = string.Concat(physician.Address1, ", ", physician.City, ", ", region.Name, " ", physician.Zip)
            };
            _db.PhysicianLocations.Add(physicianLocation);
            _db.SaveChanges();

            int retryCount = 1;
            bool success = false;

            while (retryCount <= 3 && !success) // Set retry limit
            {
                string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                string senderPassword = "shahkandarp2430"; // Replace with your actual password (store securely)
                var platformTitle = "HalloDoc";
                var subject = "Account Credentials - HalloDoc";
                var body = $"Hello {physician.FirstName} {physician.LastName},<br />We welcome you onboard on HalloDoc, Here are your credentials to login,<br />Email : {physician.Email}<br />Password : {physicianAccountViewModel.Password}<br />Username : {aspNetUser.UserName}<br /><br />Regards,<br/>{platformTitle}<br/>";
                try
                {

                    SmtpClient client = new SmtpClient("smtp.office365.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(senderEmail, senderPassword),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false
                    };

                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, "HalloDoc"),
                        Subject = subject,
                        IsBodyHtml = true,
                        Body = body
                    };

                    mailMessage.To.Add(physician.Email);


                    await client.SendMailAsync(mailMessage);


                    success = true;
                    LogEmail(body, subject, physician.Email, null, -1, -1, physician.PhysicianId, true, retryCount,3);
                    break;
                }
                catch (Exception ex)
                {

                    if (retryCount >= 3)
                    {
                        LogEmail(body, subject, physician.Email, null, -1, -1, physician.PhysicianId, false, retryCount,3);
                    }
                    retryCount++;
                }
            }

            return success;
        }

        public List<Role> getPhysicianRoles()
        {
            return _db.Roles.Where(r => r.AccountType == 2 && r.IsDeleted == new BitArray(new[] { false })).ToList();
        }

        public PhysicianAccountViewModel getPhysicianDetails(int id)
        {
            Physician physician = _db.Physicians.Include(p=>p.AspNetUser).FirstOrDefault(p=>p.PhysicianId == id);
            PhysicianLocation physicianLocation = _db.PhysicianLocations.FirstOrDefault(p => p.PhysicianId == id);
            IQueryable<PhysicianRegion> physicianRegion = _db.PhysicianRegions.Where(p => p.PhysicianId == id);
            List<Region> regions = _db.Regions.ToList();

            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            List<CheckboxViewModel> checkboxViewModels = new List<CheckboxViewModel>();

            List<Role> roles = _db.Roles.Where(r => r.AccountType == 2 && r.IsDeleted == new BitArray(new[] { false })).ToList();

            for (var i = 0; i < regions.Count; ++i)
            {
                checkboxViewModels.Add(new CheckboxViewModel()
                {
                    Name = regions[i].Name,
                    Id = regions[i].RegionId,
                    isChecked = physicianRegion.FirstOrDefault(a => a.RegionId == regions[i].RegionId) == null ? false : true
                });
            }

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Provider",
                menus = cookieModel.menus
            };


            PhysicianAccountViewModel physicianAccountViewModel = new PhysicianAccountViewModel()
            {
                PhysicianId = id,
                AspId = physician.AspNetUser.Id,
                adminNavbarViewModel = adminNavbarViewModel,
                checkboxViewModels = checkboxViewModels,
                UserName = physician.AspNetUser?.UserName,
                roles = roles,
                role_id = physician?.RoleId,
                FirstName = physician?.FirstName,
                LastName = physician?.LastName,
                Email = physician?.Email,
                PhoneNumber = physician?.Mobile,
                NPI_Number = physician?.Npinumber,
                MedicalLicense = physician?.MedicalLicense,
                Sync_Email = physician?.SyncEmailAddress,
                Address1 = physician?.Address1,
                Address2 = physician?.Address2,
                City = physician?.City,
                RegionId = (int)physician?.RegionId,
                Zipcode = physician?.Zip,
                AltPhone = physician?.AltPhone,
                BusinessName = physician?.BusinessName,
                BusinessWebsite = physician?.BusinessWebsite,
                signature_name = physician?.Signature,
                Admin_Notes = physician?.AdminNotes,
                IsAgreementDoc = physician.IsAgreementDoc[0],
                IsBackgroundDoc = physician.IsBackgroundDoc[0],
                IsCredentialDoc = physician.IsCredentialDoc[0],
                IsLicenseDoc = physician.IsLicenseDoc[0],
                IsNonDisclosureDoc = physician.IsNonDisclosureDoc[0],
                Status = physician.Status
            };

            return physicianAccountViewModel;

        }

        public async Task<bool> fileUploadPhysician(IFormFile file, int id, string name)
        {
            try
            {
                Physician physician = _db.Physicians.FirstOrDefault(p => p.PhysicianId == id);
                var filePathh = "";

                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);

                if (name == "license")
                {
                    filePathh = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}\\License.pdf");
                    if (System.IO.File.Exists(filePathh))
                    {
                        System.IO.File.Delete(filePathh);
                    }
                    physician.IsLicenseDoc = new BitArray(new[] { true });
                }
                else if (name == "nondisclosure")
                {
                    filePathh = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}\\NonDiscolsure.pdf");
                    if (System.IO.File.Exists(filePathh))
                    {
                        System.IO.File.Delete(filePathh);
                    }
                    physician.IsNonDisclosureDoc = new BitArray(new[] { true });
                }
                else if (name == "hipaa")
                {
                    filePathh = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}\\HIPAA.pdf");
                    if (System.IO.File.Exists(filePathh))
                    {
                        System.IO.File.Delete(filePathh);
                    }
                    physician.IsCredentialDoc = new BitArray(new[] { true });
                }
                else if (name == "background")
                {
                    filePathh = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}\\Background.pdf");
                    if (System.IO.File.Exists(filePathh))
                    {
                        System.IO.File.Delete(filePathh);
                    }
                    physician.IsBackgroundDoc = new BitArray(new[] { true });
                }
                else if (name == "agreement")
                {
                    filePathh = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}\\Agreement.pdf");
                    if (System.IO.File.Exists(filePathh))
                    {
                        System.IO.File.Delete(filePathh);
                    }
                    physician.IsAgreementDoc = new BitArray(new[] { true });
                }


                using (var stream = System.IO.File.Create(filePathh))
                {
                    await file.CopyToAsync(stream);
                }
                physician.ModifiedDate = DateTime.Now;
                physician.ModifiedBy = cookieModel.aspId;
                _db.Physicians.Update(physician);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public async Task<bool> updatePhysician(PhysicianAccountViewModel physicianAccountViewModel)
        {
            try
            {

                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);

                Physician physician = _db.Physicians.FirstOrDefault(p=>p.PhysicianId == physicianAccountViewModel.PhysicianId);
                physician.Status = (short?)(physicianAccountViewModel?.Status ?? physician.Status);
                physician.RoleId = physicianAccountViewModel?.role_id ?? physician.RoleId;
                physician.FirstName = physicianAccountViewModel?.FirstName ?? physician.FirstName;
                physician.LastName = physicianAccountViewModel?.LastName ?? physician.LastName;
                physician.Email = physicianAccountViewModel?.Email ?? physician.Email;
                physician.Mobile = physicianAccountViewModel?.PhoneNumber ?? physician.Mobile;
                physician.MedicalLicense = physicianAccountViewModel?.MedicalLicense ?? physician.MedicalLicense;
                physician.Npinumber = physicianAccountViewModel?.NPI_Number ?? physician.Npinumber;
                physician.SyncEmailAddress = physicianAccountViewModel?.Sync_Email ?? physician.SyncEmailAddress;
                physician.Address1 = physicianAccountViewModel?.Address1 ?? physician.Address1;
                physician.City = physicianAccountViewModel?.City ?? physician.City;
                physician.RegionId = physicianAccountViewModel?.RegionId ?? physician.RegionId;
                physician.Zip = physicianAccountViewModel?.Zipcode ?? physician.Zip;
                physician.AltPhone = physicianAccountViewModel?.AltPhone ?? physician.AltPhone;
                physician.BusinessName = physicianAccountViewModel?.BusinessName ?? physician.BusinessName;
                physician.BusinessWebsite = physicianAccountViewModel?.BusinessWebsite ?? physician.BusinessWebsite;
                physician.AdminNotes = physicianAccountViewModel?.Admin_Notes ?? physician.AdminNotes;
                physician.ModifiedDate = DateTime.Now;
                physician.ModifiedBy = cookieModel.aspId;

                if (physicianAccountViewModel.Signature != null && physicianAccountViewModel.Signature.Length > 0)
                {
                    var filePathh = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}", physicianAccountViewModel.Signature.FileName);
                    using (var stream = System.IO.File.Create(filePathh))
                    {
                        await physicianAccountViewModel.Signature.CopyToAsync(stream);
                    }
                    physician.Signature = physicianAccountViewModel.Signature.FileName;
                }

                if (physicianAccountViewModel.Photo != null && physicianAccountViewModel.Photo.Length > 0)
                {
                    var filePathh = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}", physicianAccountViewModel.Photo.FileName);
                    using (var stream = System.IO.File.Create(filePathh))
                    {
                        await physicianAccountViewModel.Photo.CopyToAsync(stream);
                    }
                    physician.Photo = physicianAccountViewModel.Photo.FileName;
                }

                _db.Physicians.Update(physician);

                IQueryable<PhysicianRegion> physicianRegions = _db.PhysicianRegions.Where(p=>p.PhysicianId == physicianAccountViewModel.PhysicianId);

                for (var i = 0; i < physicianAccountViewModel.checkboxViewModels.Count; ++i)
                {
                    if (physicianAccountViewModel.checkboxViewModels[i].isChecked == true && physicianRegions.FirstOrDefault(a => a.RegionId == physicianAccountViewModel.checkboxViewModels[i].Id) == null)
                    {
                        PhysicianRegion physicianRegion = new PhysicianRegion()
                        {
                            PhysicianId = (int)physicianAccountViewModel.PhysicianId,
                            RegionId = (int)physicianAccountViewModel.checkboxViewModels[i].Id
                        };
                        _db.PhysicianRegions.Add(physicianRegion);
                    }
                    else if (physicianAccountViewModel.checkboxViewModels[i].isChecked == false && physicianRegions.FirstOrDefault(a => a.RegionId == physicianAccountViewModel.checkboxViewModels[i].Id) != null)
                    {
                        PhysicianRegion physicianRegion = physicianRegions.FirstOrDefault(a => a.RegionId == physicianAccountViewModel.checkboxViewModels[i].Id);
                        _db.PhysicianRegions.Remove(physicianRegion);
                    }
                }

                _db.SaveChanges();

                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public bool resetPasswordPhysician(string password,int id)
        {
            try
            {
                AspNetUser aspNetUser = _db.AspNetUsers.FirstOrDefault(a => a.Id == id);
                var passwordHasher = new PasswordHasher<AspNetUser>();
                aspNetUser.PasswordHash = passwordHasher.HashPassword(aspNetUser, password);
                _db.AspNetUsers.Update(aspNetUser);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool deletePhysician(int id)
        {
            try
            {
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);

                Physician physician = _db.Physicians.FirstOrDefault(p=>p.PhysicianId == id);
                physician.IsDeleted = new BitArray(new[] { true });
                physician.ModifiedDate = DateTime.Now;
                physician.ModifiedBy = cookieModel.aspId;

                _db.Physicians.Update(physician);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public PatientHistoryViewModel getAllPatients(string? firstname, string? lastname, string? email, string? phone, int page = 1, int pageSize = 10)
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Record",
                menus = cookieModel.menus
            };

            IQueryable<User> users = _db.Users;

            if(firstname!=null)
            {
                users = users.Where(r => r.FirstName.ToLower().Contains(firstname.ToLower()));
            }
            if(lastname!=null)
            {
                users = users.Where(r => r.LastName.ToLower().Contains(lastname.ToLower()));
            }
            if(email!=null)
            {
                users = users.Where(r => r.Email.ToLower().Contains(email.ToLower()));
            }
            if(phone!=null)
            {
                users = users.Where(r => r.Mobile.Contains(phone));
            }

            PatientHistoryViewModel patientHistoryViewModel = new PatientHistoryViewModel
            { 
                adminNavbarViewModel = adminNavbarViewModel,
                users = users.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = users.Count(),
                TotalPages = (int)Math.Ceiling((double)users.Count() / pageSize)
            };

            return patientHistoryViewModel;

        }

        public PatientHistoryViewModel getAllPatientRecords(int id,int page = 1, int pageSize = 10)
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            int count = _db.Requests.Where(u => u.UserId == id).Count();
            List<RequestViewModel> data = _db.RequestViewModels.FromSqlRaw($"SELECT * FROM PatientDashboardData({id},{pageSize},{((page - 1) * pageSize)})").ToList();

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Record",
                menus = cookieModel.menus
            };

            PatientHistoryViewModel patientHistoryViewModel = new PatientHistoryViewModel
            {
                adminNavbarViewModel = adminNavbarViewModel,
                requestViewModels = data,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = count,
                TotalPages = (int)Math.Ceiling((double)count / pageSize)
            };

            return patientHistoryViewModel;

        }

        public BlockHistoryViewModel getBlockHistoryData(string? name, DateTime? date, string? email, string? phone, int page = 1, int pageSize = 10)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Record",
                menus = cookieModel.menus
            };

            IQueryable<BlockRequest> blockRequests = _db.BlockRequests;

            if(name!=null)
            {
                blockRequests = blockRequests.Where(r => r.Name.ToLower().Contains(name.ToLower()));
            }
            if (date != null)
            {
                blockRequests = blockRequests.Where(r => r.CreatedDate.Value.Date == date.Value.Date);
            }
            if (email != null)
            {
                blockRequests = blockRequests.Where(r => r.Email.ToLower().Contains(email.ToLower()));
            }
            if (phone != null)
            {
                blockRequests = blockRequests.Where(r => r.PhoneNumber.Contains(phone));
            }

            BlockHistoryViewModel blockHistoryViewModel = new BlockHistoryViewModel
            {
                adminNavbarViewModel = adminNavbarViewModel,
                blockRequests = blockRequests.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = blockRequests.Count(),
                TotalPages = (int)Math.Ceiling((double)blockRequests.Count() / pageSize)
            };

            return blockHistoryViewModel;
        }

        public bool toggleActive(int blockrequestid, bool value)
        {
            try
            {
                BlockRequest blockRequest = _db.BlockRequests.FirstOrDefault(b=>b.BlockRequestId == blockrequestid);
                blockRequest.IsActive = new BitArray(new[] { value });
                blockRequest.ModifiedDate = DateTime.Now;
                _db.BlockRequests.Update(blockRequest);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public bool restoreBlock(int blockrequestid)
        {
            try
            {
                BlockRequest blockRequest = _db.BlockRequests.FirstOrDefault(b => b.BlockRequestId == blockrequestid);
                var requestid = blockRequest.RequestId;
                _db.BlockRequests.Remove(blockRequest);

                List<RequestStatusLog> requestStatusLogs = _db.RequestStatusLogs.Where(r=>r.RequestId == int.Parse(requestid)).OrderBy(r=>r.CreatedDate).ToList();

                var status = requestStatusLogs[requestStatusLogs.Count - 2].Status;

                Request request = _db.Requests.FirstOrDefault(r=>r.RequestId == int.Parse(requestid));
                request.Status = status;
                request.ModifiedDate = DateTime.Now;
                _db.Requests.Update(request);

                RequestStatusLog requestStatusLog = new RequestStatusLog
                {
                    Status = status,
                    CreatedDate = DateTime.Now,
                    RequestId = int.Parse(requestid),
                };
                _db.RequestStatusLogs.Add(requestStatusLog);
                _db.SaveChanges();

                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public SearchRecordViewModel getSearchedData(int? status, string? name, int? requesttypeid, DateTime? fromdos, DateTime? todos, string? providername, string? email, string? phonenumber, int page = 1, int pageSize = 10)
        {

            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Record",
                menus = cookieModel.menus
            };

            List<RequestType> requestTypes = _db.RequestTypes.ToList();

            IQueryable<Request> requests = _db.Requests.Include(r => r.RequestClient).Include(r => r.RequestNotes).Include(r => r.RequestStatusLogs).Include(r => r.Physician).Where(r => r.IsDeleted == new BitArray(new[] { false }));
            if(status!=null && status != -1)
            {
                requests = requests.Where(r=>r.Status == status);
            }
            if(name!=null)
            {
                requests = requests.Where(r => r.RequestClient.FirstName.ToLower().Contains(name.ToLower()) || r.RequestClient.LastName.ToLower().Contains(name.ToLower()));
            }
            if(requesttypeid!=null && requesttypeid !=-1)
            {
                requests = requests.Where(r => r.RequestTypeId == requesttypeid);
            }
            if(fromdos!=null && todos == null)
            {
                requests = requests.Where(r => r.AcceptedDate.Value.Date >= fromdos.Value.Date);
            }
            if(fromdos == null && todos != null)
            {
                requests = requests.Where(r => r.AcceptedDate.Value.Date <= todos.Value.Date);
            }
            if(fromdos != null && todos != null)
            {
                requests = requests.Where(r => r.AcceptedDate.Value.Date >= fromdos.Value.Date && r.AcceptedDate.Value.Date <= todos.Value.Date);
            }
            if(providername!=null)
            {
                requests = requests.Where(r => r.Physician.FirstName.ToLower().Contains(providername.ToLower()) || r.Physician.LastName.ToLower().Contains(providername.ToLower()));
            }
            if(email!=null)
            {
                requests = requests.Where(r => r.RequestClient.Email.ToLower().Contains(email.ToLower()));
            }
            if(phonenumber!=null)
            {
                requests = requests.Where(r => r.RequestClient.PhoneNumber.ToLower().Contains(phonenumber.ToLower()));
            }

            SearchRecordViewModel searchRecordViewModel = new SearchRecordViewModel
            {
                requests = requests.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                alldata = requests.ToList(),
                adminNavbarViewModel = adminNavbarViewModel,
                requestTypes = requestTypes,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = requests.Count(),
                TotalPages = (int)Math.Ceiling((double)requests.Count() / pageSize)
            };

            return searchRecordViewModel;

        }

        public bool deleteRequest(int id)
        {
            try
            {
                Request req = _db.Requests.FirstOrDefault(r=>r.RequestId == id);
                req.IsDeleted = new BitArray(new[] { true });
                req.ModifiedDate = DateTime.Now;
                _db.Requests.Update(req);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }

        }

        public MemoryStream exportSearchedData(SearchRecordViewModel model)
        {
            try
            {
                List<Request> data = new List<Request>();
                data = model.alldata;
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Export All");

                worksheet.Cell(1, 1).Value = "Patient Name";
                worksheet.Cell(1, 2).Value = "Requestor";
                worksheet.Cell(1, 3).Value = "Date Of Service";
                worksheet.Cell(1, 4).Value = "Close Case Date";
                worksheet.Cell(1, 5).Value = "Email";
                worksheet.Cell(1, 6).Value = "Phone Number";
                worksheet.Cell(1, 7).Value = "Address";
                worksheet.Cell(1, 8).Value = "Zip";
                worksheet.Cell(1, 9).Value = "Request Status";
                worksheet.Cell(1, 10).Value = "Physician Name";
                worksheet.Cell(1, 11).Value = "Physician Notes";
                worksheet.Cell(1, 12).Value = "Admin Note";
                worksheet.Cell(1, 13).Value = "Patient Note";

                int row = 2;
                foreach (var item in data)
                {
                    var closecasedate = "-";
                    string cancelprovicernote = "";
                    foreach (var requeststatuslog in item.RequestStatusLogs)
                    {
                        if (requeststatuslog.Status == 9)
                        {
                            closecasedate = requeststatuslog.CreatedDate.ToString("MMMM dd,yyyy");
                        }
                        if (requeststatuslog.Status == 2 && requeststatuslog?.TransToAdmin == new BitArray(new[] { true }))
                        {
                            cancelprovicernote += requeststatuslog.Notes + Environment.NewLine;
                        }
                    }
                    var requestor = "";
                    if(item.RequestTypeId == 1)
                    {
                        requestor = "Business";
                    }
                    else if (item.RequestTypeId == 2)
                    {
                        requestor = "Patient";
                    }
                    else if (item.RequestTypeId == 3)
                    {
                        requestor = "Family";
                    }
                    else
                    {
                        requestor = "Concierge"; 
                    }
                    worksheet.Cell(row, 1).Value = string.Concat(item.RequestClient.FirstName, ", ", item.RequestClient.LastName);
                    worksheet.Cell(row, 2).Value = requestor;
                    worksheet.Cell(row, 3).Value = item.AcceptedDate == null ? "-" : item.AcceptedDate?.ToString("MMMM dd,yyyy");
                    worksheet.Cell(row, 4).Value = closecasedate;
                    worksheet.Cell(row, 5).Value = item.RequestClient.Email;
                    worksheet.Cell(row, 6).Value = item.RequestClient.PhoneNumber;
                    worksheet.Cell(row, 7).Value = string.Concat(item.RequestClient.Street, ", ", item.RequestClient.City, ", ", item.RequestClient.State);
                    worksheet.Cell(row, 8).Value = item.RequestClient.ZipCode;
                    worksheet.Cell(row, 9).Value = Enum.GetName(typeof(Status), @item.Status);
                    worksheet.Cell(row, 10).Value = item?.Physician?.FirstName == null ? "-" : "Dr. " + item?.Physician?.FirstName;
                    worksheet.Cell(row, 11).Value = item?.RequestNotes.FirstOrDefault()?.PhysicianNotes == null ? "-" : item?.RequestNotes.FirstOrDefault()?.PhysicianNotes;
                    worksheet.Cell(row, 12).Value = item?.RequestNotes.FirstOrDefault()?.AdminNotes == null ? "-" : item?.RequestNotes.FirstOrDefault()?.AdminNotes;
                    worksheet.Cell(row, 13).Value = item.RequestClient?.Notes == null ? "-" : item.RequestClient?.Notes;
                    row++;
                }
                worksheet.Columns().AdjustToContents();

                var memoryStream = new MemoryStream();
                workbook.SaveAs(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        public AccountAccessViewModel getAllRolesDetails(int page=1,int pageSize=10)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Access",
                menus = cookieModel.menus
            };

            IQueryable<Role> roles = _db.Roles.Where(r => r.IsDeleted == new BitArray(new[] { false }));

            AccountAccessViewModel accountAccessViewModel = new AccountAccessViewModel
            {
                roles = roles.Skip((page - 1) * pageSize).Take(pageSize).OrderByDescending(r=>r.CreatedBy).ToList(),
                adminNavbarViewModel = adminNavbarViewModel,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = roles.Count(),
                TotalPages = (int)Math.Ceiling((double)roles.Count() / pageSize)
            };

            return accountAccessViewModel;

        }

        public AdminNavbarViewModel getCreateAccessNavbar()
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Access",
                menus = cookieModel.menus
            };
            return adminNavbarViewModel;
        }

        public List<Menu> getMenus(int? id)
        {
            if(id==-1)
            {
                return _db.Menus.ToList();
            }

            return _db.Menus.Where(m => m.AccountType == id).ToList();
        }

        public bool createRole(string? menus, string? role_name, int? account_type)
        {
            try
            {
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);

                string[] menu = menus.Split(",");

                Role role = new Role
                {
                    Name = role_name,
                    AccountType = (short)account_type,
                    CreatedDate = DateTime.Now,
                    CreatedBy = cookieModel.name,
                    IsDeleted = new BitArray(new[] { false })
                };
                _db.Roles.Add(role);
                _db.SaveChanges();


                for(var i=0;i< menu.Length - 1;++i)
                {
                    RoleMenu roleMenu = new RoleMenu
                    {
                        RoleId = role.RoleId,
                        MenuId = int.Parse(menu[i])
                    };
                    _db.RoleMenus.Add(roleMenu);
                }

                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool deleteRole(int? id)
        {
            try
            {
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);

                Role role = _db.Roles.FirstOrDefault(r=>r.RoleId == id);
                role.IsDeleted = new BitArray(new[] { true });
                role.ModifiedDate = DateTime.Now;
                role.ModifiedBy = cookieModel.name;

                _db.Roles.Update(role);
                _db.SaveChanges();

                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public EditAccessViewModel getRoleDetails(int? id)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Access",
                menus = cookieModel.menus
            };

            Role role = _db.Roles.FirstOrDefault(r=>r.RoleId == id);
            IQueryable<RoleMenu> roleMenus = _db.RoleMenus.Where(r=>r.RoleId == id);

            List<Menu> menus = _db.Menus.Where(r=>r.AccountType == role.AccountType).ToList();

            List<CheckboxViewModel> checkboxViewModels = new List<CheckboxViewModel>();

            string roles = "";

            for(var i=0;i< roleMenus.Count();++i)
            {
                roles += roleMenus.ToList()[i].MenuId + ",";
            }

            for(var i=0; i<menus.Count; i++)
            {
                checkboxViewModels.Add(new CheckboxViewModel
                {
                    Name = menus[i].Name,
                    Id = menus[i].MenuId,
                    isChecked = roleMenus.FirstOrDefault(r => r.MenuId == menus[i].MenuId) == null ? false : true
                });
            }

            EditAccessViewModel editAccessViewModel = new EditAccessViewModel
            {
                Name = role.Name,
                Account_type = role.AccountType,
                adminNavbarViewModel = adminNavbarViewModel,
                checkboxViewModels = checkboxViewModels,
                menus = roles,
            };

            return editAccessViewModel;

        }

        public bool editRoleDetails(int? id, string? menus, string? role_name, int? account_type)
        {
            try
            {
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);

                string[] menu = menus.Split(",");

                Role role = _db.Roles.FirstOrDefault(r => r.RoleId == id);

                if (role.AccountType == account_type)
                {
                    
                    IQueryable<RoleMenu> roleMenus = _db.RoleMenus.Where(r => r.RoleId == id);
                    for (var i = 0; i < menu.Length - 1; ++i)
                    {
                        if(roleMenus.FirstOrDefault(r=>r.MenuId == int.Parse(menu[i])) == null)
                        {
                            RoleMenu roleMenu = new RoleMenu
                            {
                                RoleId = role.RoleId,
                                MenuId = int.Parse(menu[i])
                            };
                            _db.RoleMenus.Add(roleMenu);
                        }
                    }

                    Dictionary<int, bool> keyValuePairs = new Dictionary<int, bool>();

                    for(var i=0;i< roleMenus.Count();++i)
                    {
                        keyValuePairs[roleMenus.ToList()[i].MenuId] = false;
                    }

                    for(var i=0;i< menu.Length - 1;++i)
                    {
                        keyValuePairs[int.Parse(menu[i])] = true;
                    }

                    for(var i=0;i< roleMenus.Count(); ++i)
                    {
                        if(keyValuePairs[roleMenus.ToList()[i].MenuId] == false)
                        {
                            _db.RoleMenus.Remove(roleMenus.FirstOrDefault(r => r.MenuId == roleMenus.ToList()[i].MenuId));
                        }
                    }

                }
                else
                {
                    IQueryable<RoleMenu> roleMenus = _db.RoleMenus.Where(r => r.RoleId == id);
                    for(var i=0;i< roleMenus.Count();++i)
                    {
                        _db.RoleMenus.Remove(roleMenus.FirstOrDefault(r=>r.MenuId == roleMenus.ToList()[i].MenuId));
                    }

                    for (var i = 0; i < menu.Length - 1; ++i)
                    {
                        RoleMenu roleMenu = new RoleMenu
                        {
                            RoleId = role.RoleId,
                            MenuId = int.Parse(menu[i])
                        };
                        _db.RoleMenus.Add(roleMenu);
                    }

                }

                role.Name = role_name;
                role.AccountType = (short)account_type;
                role.ModifiedDate = DateTime.Now;
                role.ModifiedBy = cookieModel.name;
                _db.Roles.Update(role);
                _db.SaveChanges();


                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public EmailLogViewModel getEmailLogDetails(int? roleid, string? name, string? email, DateTime? createddate, DateTime? sentdate, int page = 1, int pageSize = 10)
        {

            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Record",
                menus = cookieModel.menus
            };

            IQueryable<EmailLog> emailLogs = _db.EmailLogs;

            if(roleid!=null && roleid!=-1)
            {
                emailLogs = emailLogs.Where(r => r.RoleId == roleid);
            }
            if(email!=null)
            {
                emailLogs = emailLogs.Where(r=>r.EmailId.ToLower().Contains(email.ToLower()));
            }
            if(createddate != null)
            {
                emailLogs = emailLogs.Where(r => r.CreateDate.Date == createddate.Value.Date);
            }
            if(sentdate != null)
            {
                emailLogs = emailLogs.Where(r => r.SentDate.Value.Date == sentdate.Value.Date);
            }

            List<LogViewModel> logViewModels = new List<LogViewModel>();

            for(var i=0;i< emailLogs.Count();++i)
            {
                var namee = "-";
                if (emailLogs.ToList()[i].RoleId == 1)
                {
                    if (emailLogs.ToList()[i].RequestId != null)
                    {
                        Request request = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(r => r.RequestId == emailLogs.ToList()[i].RequestId);
                        namee = string.Concat(request.RequestClient.FirstName, ", ", request.RequestClient.LastName);
                    }
                    else
                    {
                        AspNetUser aspNetUser = _db.AspNetUsers.FirstOrDefault(a => a.Email == emailLogs.ToList()[i].EmailId);
                        User user = _db.Users.FirstOrDefault(u=>u.AspNetUserId == aspNetUser.Id);
                        namee = string.Concat(user.FirstName, ", ", user.LastName);
                    }
                }
                else if(emailLogs.ToList()[i].RoleId == 3)
                {
                    if(emailLogs.ToList()[i].PhysicianId != null)
                    {
                        Physician physician = _db.Physicians.FirstOrDefault(r => r.PhysicianId == emailLogs.ToList()[i].PhysicianId);
                        namee = string.Concat(physician.FirstName, ", ", physician.LastName);
                    }
                    else
                    {
                        AspNetUser aspNetUser = _db.AspNetUsers.FirstOrDefault(a => a.Email == emailLogs.ToList()[i].EmailId);
                        Physician physician = _db.Physicians.FirstOrDefault(r => r.AspNetUserId == aspNetUser.Id);
                        namee = string.Concat(physician.FirstName, ", ", physician.LastName);
                    }
                }
                else if(emailLogs.ToList()[i].RoleId == 2)
                {
                    if(emailLogs.ToList()[i].AdminId != null)
                    {
                        HalloDoc.Admin admin = _db.Admins.FirstOrDefault(r => r.AdminId == emailLogs.ToList()[i].AdminId);
                        namee = string.Concat(admin.FirstName, ", ", admin.LastName);
                    }
                    else
                    {
                        AspNetUser aspNetUser = _db.AspNetUsers.FirstOrDefault(a => a.Email == emailLogs.ToList()[i].EmailId);
                        HalloDoc.Admin admin = _db.Admins.FirstOrDefault(r => r.AspNetUserId == aspNetUser.Id);
                        namee = string.Concat(admin.FirstName, ", ", admin.LastName);
                    }
                }
                AspNetRole role = _db.AspNetRoles.FirstOrDefault(r=>r.Id == emailLogs.ToList()[i].RoleId);
                logViewModels.Add(new LogViewModel
                {
                    Name = namee,
                    EmailId = emailLogs.ToList()[i].EmailId,
                    Action = "-",
                    RoleName = role?.Name ?? "-",
                    CreatedDate = emailLogs.ToList()[i].CreateDate,
                    SentDate = emailLogs.ToList()[i].SentDate,
                    Sent = emailLogs.ToList()[i].IsEmailSent[0] ? "Yes" : "No",
                    SentTries = emailLogs.ToList()[i].SentTries,
                    ConfirmationNumber = emailLogs.ToList()[i].ConfirmationNumber ?? "-"
                });
            }

            List<LogViewModel> filteredlogViewModels = new List<LogViewModel>();

            if(name!=null)
            {
                for (var i = 0; i < logViewModels.Count; ++i)
                {
                    if (logViewModels[i].Name.ToLower().Contains(name.ToLower()))
                    {
                        filteredlogViewModels.Add(logViewModels[i]);
                    }
                }
            }
            else
            {
                filteredlogViewModels = logViewModels;
            }

            List<AspNetRole> roles = _db.AspNetRoles.ToList();

            EmailLogViewModel emailLogViewModel = new EmailLogViewModel
            {
                roles = roles,
                adminNavbarViewModel = adminNavbarViewModel,
                logViewModels = filteredlogViewModels.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = filteredlogViewModels.Count,
                TotalPages = (int)Math.Ceiling((double)filteredlogViewModels.Count / pageSize)
            };
            return emailLogViewModel;
        }

        public EmailLogViewModel getSMSLogDetails(int? roleid, string? name, string? phonenumber, DateTime? createddate, DateTime? sentdate, int page = 1, int pageSize = 10)
        {

            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Record",
                menus = cookieModel.menus
            };

            IQueryable<Smslog> smslogs = _db.Smslogs;

            if(roleid!=null && roleid!=-1)
            {
                smslogs = smslogs.Where(r => r.RoleId == roleid);
            }
            if(phonenumber != null)
            {
                smslogs = smslogs.Where(r=>r.MobileNumber.ToLower().Contains(phonenumber.ToLower()));
            }
            if(createddate != null)
            {
                smslogs = smslogs.Where(r => r.CreateDate.Date == createddate.Value.Date);
            }
            if(sentdate != null)
            {
                smslogs = smslogs.Where(r => r.SentDate.Value.Date == sentdate.Value.Date);
            }

            List<LogViewModel> logViewModels = new List<LogViewModel>();

            for(var i=0;i< smslogs.Count();++i)
            {
                var namee = "-";
                if (smslogs.ToList()[i].RoleId == 1)
                {
                    Request request = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(r => r.RequestId == smslogs.ToList()[i].RequestId);
                    namee = string.Concat(request.RequestClient.FirstName, ", ", request.RequestClient.LastName);

                }
                else if (smslogs.ToList()[i].RoleId == 3)
                {
                    Physician physician = _db.Physicians.FirstOrDefault(r => r.PhysicianId == smslogs.ToList()[i].PhysicianId);
                    namee = string.Concat(physician.FirstName, ", ", physician.LastName);
                }
                else if (smslogs.ToList()[i].RoleId == 2)
                {
                    HalloDoc.Admin admin = _db.Admins.FirstOrDefault(r => r.AdminId == smslogs.ToList()[i].AdminId);
                    namee = string.Concat(admin.FirstName, ", ", admin.LastName);
                }
                AspNetRole role = _db.AspNetRoles.FirstOrDefault(r=>r.Id == smslogs.ToList()[i].RoleId);
                logViewModels.Add(new LogViewModel
                {
                    Name = namee,
                    PhoneNumber = smslogs.ToList()[i].MobileNumber,
                    Action = "-",
                    RoleName = role?.Name ?? "-",
                    CreatedDate = smslogs.ToList()[i].CreateDate,
                    SentDate = smslogs.ToList()[i].SentDate,
                    Sent = smslogs.ToList()[i].IsSmssent[0] ? "Yes" : "No",
                    SentTries = smslogs.ToList()[i].SentTries,
                    ConfirmationNumber = smslogs.ToList()[i].ConfirmationNumber ?? "-"
                });
            }

            List<LogViewModel> filteredlogViewModels = new List<LogViewModel>();

            if(name!=null)
            {
                for (var i = 0; i < logViewModels.Count; ++i)
                {
                    if (logViewModels[i].Name.ToLower().Contains(name.ToLower()))
                    {
                        filteredlogViewModels.Add(logViewModels[i]);
                    }
                }
            }
            else
            {
                filteredlogViewModels = logViewModels;
            }

            List<AspNetRole> roles = _db.AspNetRoles.ToList();

            EmailLogViewModel emailLogViewModel = new EmailLogViewModel
            {
                roles = roles,
                adminNavbarViewModel = adminNavbarViewModel,
                logViewModels = filteredlogViewModels.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = filteredlogViewModels.Count,
                TotalPages = (int)Math.Ceiling((double)filteredlogViewModels.Count / pageSize)
            };
            return emailLogViewModel;
        }

        public PartnerViewModal getPartnerDetails(string? name, int? id, int page = 1, int pageSize = 10)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Partner",
                menus = cookieModel.menus
            };

            List<HealthProfessionalType> healthProfessionalTypes = _db.HealthProfessionalTypes.Where(r => r.IsDeleted == new BitArray(new[] { false })).ToList();
            IQueryable<HealthProfessional> healthProfessionals = _db.HealthProfessionals.Include(r=>r.ProfessionNavigation).Where(r => r.IsDeleted == new BitArray(new[] { false }));

            if(name!=null)
            {
                healthProfessionals = healthProfessionals.Where(r=>r.VendorName.ToLower().Contains(name.ToLower()));
            }
            if(id!=null && id!=-1)
            {
                healthProfessionals = healthProfessionals.Where(r => r.Profession == id);
            }

            PartnerViewModal partnerViewModal = new PartnerViewModal
            {
                adminNavbarViewModel = adminNavbarViewModel,
                healthProfessionalTypes = healthProfessionalTypes,
                healthProfessionals = healthProfessionals.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = healthProfessionals.Count(),
                TotalPages = (int)Math.Ceiling((double)healthProfessionals.Count() / pageSize)
            };
            return partnerViewModal;
        }

        public BusinessViewModel getBusinessNavbar()
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            List<HealthProfessionalType> healthProfessionalTypes = _db.HealthProfessionalTypes.Where(r => r.IsDeleted == new BitArray(new[] { false })).ToList();

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Partner",
                menus = cookieModel.menus
            };

            BusinessViewModel businessViewModel = new BusinessViewModel
            {
                adminNavbarViewModel = adminNavbarViewModel,
                page = "Add Business",
                healthProfessionalTypes = healthProfessionalTypes
            };
            return businessViewModel;
        }

        public bool createBusiness(BusinessViewModel businessViewModel)
        {
            try
            {
                Region region = new Region();
                if(businessViewModel.State != null)
                {
                    region = _db.Regions.FirstOrDefault(u => u.Name == businessViewModel.State.Trim().ToLower().Replace(" ", ""));
                }

                HealthProfessional healthProfessional = new HealthProfessional
                {
                    VendorName = businessViewModel.Name,
                    IsDeleted = new BitArray(new[] { false }),
                    Profession = businessViewModel.ProfessionId == -1 ? null : businessViewModel.ProfessionId,
                    FaxNumber = businessViewModel.FaxNumber,
                    PhoneNumber = businessViewModel.PhoneNumber,
                    Email = businessViewModel.Email,
                    BusinessContact = businessViewModel?.BusinessContact,
                    State = businessViewModel?.State,
                    Address = businessViewModel?.Street,
                    Zip = businessViewModel?.ZipCode,
                    City = businessViewModel?.City,
                    RegionId = region?.RegionId,
                    CreatedDate = DateTime.Now,
                };
                _db.HealthProfessionals.Add(healthProfessional);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public BusinessViewModel getBusinessDetails(int id)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            List<HealthProfessionalType> healthProfessionalTypes = _db.HealthProfessionalTypes.Where(r => r.IsDeleted == new BitArray(new[] { false })).ToList();

            HealthProfessional healthProfessional = _db.HealthProfessionals.FirstOrDefault(h=>h.VendorId == id);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Partner",
                menus = cookieModel.menus
            };

            BusinessViewModel businessViewModel = new BusinessViewModel
            {
                adminNavbarViewModel = adminNavbarViewModel,
                page = "Edit Business",
                healthProfessionalTypes = healthProfessionalTypes,
                Name = healthProfessional.VendorName,
                BusinessId = healthProfessional.VendorId,
                ProfessionId = healthProfessional.Profession,
                FaxNumber = healthProfessional.FaxNumber,
                PhoneNumber = healthProfessional.PhoneNumber,
                Email = healthProfessional.Email,
                BusinessContact = healthProfessional?.BusinessContact,
                Street = healthProfessional?.Address,
                City = healthProfessional?.City,
                State = healthProfessional?.State,
                ZipCode = healthProfessional?.Zip
            };

            return businessViewModel;
        }

        public bool editBusiness(BusinessViewModel businessViewModel)
        {
            try
            {
                Region region = new Region();
                if (businessViewModel.State != null)
                {
                    region = _db.Regions.FirstOrDefault(u => u.Name == businessViewModel.State.Trim().ToLower().Replace(" ", ""));
                }

                HealthProfessional healthProfessional = _db.HealthProfessionals.FirstOrDefault(h=>h.VendorId == businessViewModel.BusinessId);

                healthProfessional.VendorName = businessViewModel.Name;
                healthProfessional.Profession = businessViewModel.ProfessionId == -1 ? null : businessViewModel.ProfessionId;
                healthProfessional.FaxNumber = businessViewModel.FaxNumber;
                healthProfessional.PhoneNumber = businessViewModel.PhoneNumber;
                healthProfessional.Email = businessViewModel.Email;
                healthProfessional.BusinessContact = businessViewModel?.BusinessContact;
                healthProfessional.State = businessViewModel?.State;
                healthProfessional.Address = businessViewModel?.Street;
                healthProfessional.Zip = businessViewModel?.ZipCode;
                healthProfessional.City = businessViewModel?.City;
                healthProfessional.RegionId = region?.RegionId;
                healthProfessional.ModifiedDate = DateTime.Now;

                _db.HealthProfessionals.Update(healthProfessional);
                _db.SaveChanges();
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public bool deleteBusiness(int id)
        {
            try
            {
                HealthProfessional healthProfessional = _db.HealthProfessionals.FirstOrDefault(h => h.VendorId == id);
                healthProfessional.IsDeleted = new BitArray(new[] { true });
                healthProfessional.ModifiedDate = DateTime.Now;
                _db.HealthProfessionals.Update(healthProfessional);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public ProviderLocationViewModel getProviderLocation()
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "ProviderLocation",
                menus = cookieModel.menus
            };

            List<PhysicianLocation> physicianLocations = _db.PhysicianLocations.ToList();

            ProviderLocationViewModel providerLocationViewModel = new ProviderLocationViewModel
            {
                adminNavbarViewModel = adminNavbarViewModel,
                physicianLocations = physicianLocations
            };

            return providerLocationViewModel;

        }

        public AdminProfileViewModel getCreateAdminProfilePageDetails()
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Access",
                menus = cookieModel.menus
            };

            List<Region> regions = _db.Regions.ToList();

            List<CheckboxViewModel> checkboxViewModels = new List<CheckboxViewModel>();
            List<Role> roles = _db.Roles.Where(r => r.AccountType == 1 && r.IsDeleted == new BitArray(new[] { false })).ToList();
            for (var i = 0; i < regions.Count; i++)
            {
                checkboxViewModels.Add(new CheckboxViewModel()
                {
                    Id = regions[i].RegionId,
                    Name = regions[i].Name,
                    isChecked = false
                });
            }

            AdminProfileViewModel adminProfileViewModel = new AdminProfileViewModel
            {
                adminNavbarViewModel = adminNavbarViewModel,
                checkboxViewModels = checkboxViewModels,
                roles = roles
            };
            return adminProfileViewModel;
        }

        public List<Role> getAdminRoles()
        {
            return _db.Roles.Where(r => r.AccountType == 1 && r.IsDeleted == new BitArray(new[] { false })).ToList();
        }

        public async Task<bool> createAdmin(AdminProfileViewModel adminProfileViewModel)
        {
            try
            {
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);

                var passwordHasher = new PasswordHasher<AspNetUser>();
                Region region = _db.Regions.FirstOrDefault(r => r.RegionId == adminProfileViewModel.RegionId);

                AspNetUser aspNetUser = new AspNetUser()
                {
                    Email = adminProfileViewModel.Email,
                    UserName = string.Concat(adminProfileViewModel.LastName.Substring(0, 1).ToUpper(), adminProfileViewModel.LastName.Substring(1).ToLower(), adminProfileViewModel.FirstName.Substring(0, 1).ToUpper()),
                    CreatedDate = DateTime.Now
                };
                aspNetUser.PasswordHash = passwordHasher.HashPassword(aspNetUser, adminProfileViewModel.Password);
                _db.AspNetUsers.Add(aspNetUser);
                _db.SaveChanges();

                AspNetUserRole aspNetUserRole = new AspNetUserRole
                {
                    UserId = aspNetUser.Id,
                    RoleId = 2
                };

                _db.AspNetUserRoles.Add(aspNetUserRole);
                _db.SaveChanges();

                HalloDoc.Admin admin = new HalloDoc.Admin
                {
                    AspNetUserId = aspNetUser.Id,
                    RoleId = adminProfileViewModel.role_id,
                    FirstName = adminProfileViewModel.FirstName,
                    LastName = adminProfileViewModel.LastName,
                    Email = adminProfileViewModel.Email,
                    Mobile = adminProfileViewModel.PhoneNumber,
                    Address1 = adminProfileViewModel.Address1,
                    Address2 = adminProfileViewModel.Address2,
                    City = adminProfileViewModel.City,
                    RegionId = adminProfileViewModel.RegionId,
                    Zip = adminProfileViewModel.ZipCode,
                    AltPhone = adminProfileViewModel.Alt_PhoneNumber,
                    Status = 2,
                    CreatedDate = DateTime.Now,
                    CreatedBy = cookieModel.aspId,
                    IsDeleted = false,
                };

                _db.Admins.Add(admin);
                _db.SaveChanges();

                for (var i = 0; i < adminProfileViewModel.checkboxViewModels.Count; ++i)
                {
                    if (adminProfileViewModel.checkboxViewModels[i].isChecked == true)
                    {
                        AdminRegion adminRegion = new AdminRegion()
                        {
                            AdminId = admin.AdminId,
                            RegionId = (int)adminProfileViewModel.checkboxViewModels[i].Id
                        };
                        _db.AdminRegions.Add(adminRegion);
                        _db.SaveChanges();
                    }
                }

                int retryCount = 1;
                bool success = false;

                while (retryCount <= 3 && !success) // Set retry limit
                {

                    string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                    string senderPassword = "shahkandarp2430"; // Replace with your actual password (store securely)
                    var platformTitle = "HalloDoc";
                    var subject = "Account Credentials - HalloDoc";
                    var body = $"Hello {admin.FirstName} {admin.LastName},<br />We welcome you onboard on HalloDoc, Here are your credentials to login,<br />Email : {admin.Email}<br />Password : {adminProfileViewModel.Password}<br />Username : {aspNetUser.UserName}<br /><br />Regards,<br/>{platformTitle}<br/>";
                    try
                    {


                        SmtpClient client = new SmtpClient("smtp.office365.com")
                        {
                            Port = 587,
                            Credentials = new NetworkCredential(senderEmail, senderPassword),
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false
                        };

                        MailMessage mailMessage = new MailMessage
                        {
                            From = new MailAddress(senderEmail, "HalloDoc"),
                            Subject = subject,
                            IsBodyHtml = true,
                            Body = body
                        };

                        mailMessage.To.Add(admin.Email);


                        await client.SendMailAsync(mailMessage);


                        success = true;
                        LogEmail(body, subject, admin.Email, null, -1, admin.AdminId, -1, true, retryCount, 2);
                        break;
                    }
                    catch (Exception ex)
                    {

                        if (retryCount >= 3)
                        {
                            LogEmail(body, subject, admin.Email, null, -1, admin.AdminId, -1, false, retryCount, 2);
                        }
                        retryCount++;
                    }
                }

                return success;

            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public UserAccessViewModel GetUserAccessDetails(int? roleid,int page = 1,int pageSize = 10)
        {
            var query = from an in _db.AspNetUsers
                                    join anr in _db.AspNetUserRoles on an.Id equals anr.UserId
                                    join ar in _db.AspNetRoles on anr.RoleId equals ar.Id
                                    join ph in _db.Physicians on an.Id equals ph.AspNetUserId into phJoin
                                    from ph in phJoin.DefaultIfEmpty()
                                    join ad in _db.Admins on an.Id equals ad.AspNetUserId into adJoin
                                    from ad in adJoin.DefaultIfEmpty()
                                    where anr.RoleId != 1 &&
                                          (ad.AdminId == null || (ad.AdminId != null && ad.IsDeleted == false)) &&
                                          (ph.PhysicianId == null || (ph.PhysicianId != null && ph.IsDeleted == new BitArray(new[] { false })))
                                    select new UserAccessData
                                    {
                                        AccountType = ar.Name,
                                        Name = anr.RoleId == 2 ? ad.FirstName + ", " + ad.LastName :
                                               anr.RoleId == 3 ? ph.FirstName + ", " + ph.LastName : null,
                                        PhoneNumber = anr.RoleId == 2 ? ad.Mobile :
                                                       anr.RoleId == 3 ? ph.Mobile : null,
                                        Status = anr.RoleId == 2 ? ad.Status :
                                                 anr.RoleId == 3 ? ph.Status : null,
                                        OpenRequest = anr.RoleId == 2 ? _db.Requests.Count(r => r.Status != 10 && r.Status != 11 && r.IsDeleted == new BitArray(new[] { false })) :
                                                      anr.RoleId == 3 ? _db.Requests.Count(r => r.Status != 10 && r.Status != 11 && r.IsDeleted == new BitArray(new[] { false }) && r.PhysicianId == ph.PhysicianId) : 0,
                                        PhysicianId = ph.PhysicianId == null ? -1 : ph.PhysicianId,
                                        AdminId = ad.AdminId == null ? -1 : ad.AdminId,
                                    };

        
            if(roleid==1)
            {
                query = query.Where(r=>r.AccountType == "Admin");
            }
            else if(roleid == 2)
            {
                query = query.Where(r => r.AccountType == "Provider");
            }

            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Access",
                menus = cookieModel.menus
            };

            UserAccessViewModel userAccessViewModel = new UserAccessViewModel
            {
                adminNavbarViewModel = adminNavbarViewModel,
                userAccessData = query.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = query.Count(),
                TotalPages = (int)Math.Ceiling((double)query.Count() / pageSize)
            };

            return userAccessViewModel;
        }


    }
}
