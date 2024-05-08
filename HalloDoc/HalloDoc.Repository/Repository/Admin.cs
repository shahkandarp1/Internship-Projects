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
using System.Diagnostics.Metrics;
using DocumentFormat.OpenXml.EMMA;

namespace HalloDoc.Repository.Repository
{

    public class Admin : IAdmin
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

        public AdminDashboardViewModel AdminDashboardContent(string status, string? search, string? requestor, int? region, int page = 1, int pageSize = 10)
        {


            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            Expression<Func<Request, bool>> exp;
            if (status == "New")
            {
                exp = r => r.Status == 1;
            }
            else if (status == "Pending")
            {
                exp = r => r.Status == 2;
            }
            else if (status == "Active")
            {
                exp = r => r.Status == 3 || r.Status == 4;
            }
            else if (status == "Conclude")
            {
                exp = r => r.Status == 5;
            }
            else if (status == "ToClose")
            {
                exp = r => r.Status == 6 || r.Status == 7 || r.Status == 8;
            }
            else
            {
                exp = r => r.Status == 9;
            }

            IQueryable<Request> _query = _db.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Include(r => r.EncounterForms).Where(exp).Where(r => r.IsDeleted == new BitArray(new[] { false })).OrderByDescending(e => e.CreatedDate);

            if (_query == null)
            {
                return null;
            }

            var count_new = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false })).Count(r => r.Status == 1);
            var count_pending = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false })).Count(r => r.Status == 2);
            var count_active = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false })).Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false })).Count(r => r.Status == 5);
            var count_toclose = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false })).Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false })).Count(r => r.Status == 9);
            var casetag = _db.CaseTags.ToList();

            if (cookieModel.role == "Provider")
            {
                count_new = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false }) && r.PhysicianId == cookieModel.userId).Count(r => r.Status == 1);
                count_pending = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false }) && r.PhysicianId == cookieModel.userId).Count(r => r.Status == 2);
                count_active = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false }) && r.PhysicianId == cookieModel.userId).Count(r => r.Status == 3 || r.Status == 4);
                count_conclude = _db.Requests.Where(r => r.IsDeleted == new BitArray(new[] { false }) && r.PhysicianId == cookieModel.userId).Count(r => r.Status == 5);
                _query = _query.Where(r => r.PhysicianId == cookieModel.userId);
            }

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

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus,
                role = cookieModel.role
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

        public MemoryStream ExportAll()
        {
            try
            {
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

        public MemoryStream Export(AdminDashboardViewModel model)
        {
            try
            {
                var data = model.requests;
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

        public ViewCaseViewModel ViewCase(int id)
        {
            var req = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(r => r.RequestId == id);
            if (req == null)
            {
                return null;
            }
            var region = _db.Regions.FirstOrDefault(r => r.RegionId == req.RequestClient.RegionId);
            var caseTags = _db.CaseTags.ToList();

            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            var regions = _db.Regions.ToList();

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus,
                role = cookieModel.role
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

        public bool ViewCase(ViewCaseViewModel model)
        {

            try
            {
                RequestClient requestClient = _db.RequestClients.FirstOrDefault(r => r.RequestClientId == model.RequestClientId);
                if (requestClient == null)
                {
                    return false;
                }
                requestClient.PhoneNumber = model?.PhoneNumber ?? requestClient.PhoneNumber;
                requestClient.Email = model?.Email ?? requestClient.Email;
                _db.RequestClients.Update(requestClient);
                _db.SaveChanges();
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public bool CancelRequest(int id, string notes, string request)
        {
            try
            {
                Request req = _db.Requests.FirstOrDefault(r => r.RequestId == id);
                if (req == null)
                {
                    return false;
                }
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
            catch (Exception exp)
            {
                return false;
            }
        }

        public async Task<bool> SendLink(AdminDashboardViewModel dashboardViewModel)
        {
            int retryCount = 1;
            bool success = false;

            while (retryCount <= 3 && !success) // Set retry limit
            {

                string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                string senderPassword = "shahkandarp2430";
                var platformTitle = "HalloDoc";
                var inviteLink = $"https://localhost:{_configuration["Port:number"]}/CreateRequest/SubmitRequest";
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
                    LogEmail(body, subject, dashboardViewModel.Mail_Email, null, -1, -1, -1, true, retryCount, -1, 9);
                    break;
                }
                catch (Exception ex)
                {

                    if (retryCount >= 3)
                    {
                        LogEmail(body, subject, dashboardViewModel.Mail_Email, null, -1, -1, -1, false, retryCount, -1, 9);
                    }
                    retryCount++;
                }
            }

            retryCount = 1;
            success = false;

            while (retryCount <= 3 && !success) // Set retry limit
            {
                var platformTitle = "HalloDoc";
                var inviteLink = $"https://localhost:{_configuration["Port:number"]}/CreateRequest/SubmitRequest";

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
                        to: new Twilio.Types.PhoneNumber(dashboardViewModel.Mail_PhoneNumber[0] == '+' && dashboardViewModel.Mail_PhoneNumber[1] == '9' && dashboardViewModel.Mail_PhoneNumber[2] == '1' ? dashboardViewModel.Mail_PhoneNumber : "+91" + dashboardViewModel.Mail_PhoneNumber)
                    );


                    success = true;
                    LogSMS(messageBody, dashboardViewModel.Mail_PhoneNumber, null, -1, -1, -1, true, retryCount, -1, 3);
                    break;
                }
                catch (Exception ex)
                {

                    if (retryCount >= 3)
                    {
                        LogSMS(messageBody, dashboardViewModel.Mail_PhoneNumber, null, -1, -1, -1, false, retryCount, -1, 3);
                    }
                    retryCount++;
                }
            }

            return success;
        }

        public bool VerifyRegion(string region)
        {
            var region_check = _db.Regions.FirstOrDefault(u => u.Name == region.Trim().ToLower().Replace(" ", ""));
            if (region_check != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool VerifyBlock(string Email)
        {
            var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == Email);
            if (user != null)
            {
                var block = _db.BlockRequests.FirstOrDefault(u => u.Email == user.Email);
                if (block != null)
                {
                    return true;
                }
            }
            return false;
        }

        public PatientRequestViewModel CreateRequest()
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            PatientRequestViewModel patientRequestViewModel = new PatientRequestViewModel()
            {
                adminNavbarViewModel = adminNavbarViewModel
            };
            return patientRequestViewModel;
        }

        public async Task<bool> CreateRequest(PatientRequestViewModel modal)
        {
            try
            {
                var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == modal.Email);
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

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
                    if (cookieModel.role == "Provider")
                    {
                        req.PhysicianId = cookieModel.userId;
                        req.Status = 2;
                    }
                    _db.Requests.Add(req);
                    _db.SaveChanges();

                    if (cookieModel.role == "Provider")
                    {
                        RequestStatusLog requestStatusLog = new RequestStatusLog
                        {
                            RequestId = req.RequestId,
                            Status = 2,
                            CreatedDate = DateTime.Now
                        };
                        _db.RequestStatusLogs.Add(requestStatusLog);
                        _db.SaveChanges();
                    }

                    if (modal.Admin_notes != null && cookieModel.role == "Admin")
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

                    if (modal.Physician_notes != null && cookieModel.role == "Provider")
                    {
                        RequestNote requestNote = new RequestNote
                        {
                            RequestId = req.RequestId,
                            PhysicianNotes = modal.Physician_notes,
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

                    if (cookieModel.role == "Provider")
                    {
                        req.PhysicianId = cookieModel.userId;
                        req.Status = 2;
                    }
                    _db.Requests.Add(req);
                    _db.SaveChanges();

                    if (cookieModel.role == "Provider")
                    {
                        RequestStatusLog requestStatusLog = new RequestStatusLog
                        {
                            RequestId = req.RequestId,
                            Status = 2,
                            CreatedDate = DateTime.Now
                        };
                        _db.RequestStatusLogs.Add(requestStatusLog);
                        _db.SaveChanges();
                    }

                    if (modal.Admin_notes != null && cookieModel.role == "Admin")
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

                    if (modal.Physician_notes != null && cookieModel.role == "Provider")
                    {
                        RequestNote requestNote = new RequestNote
                        {
                            RequestId = req.RequestId,
                            PhysicianNotes = modal.Physician_notes,
                            CreatedDate = DateTime.Now,
                            CreatedBy = cookieModel.aspId,
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
                        var inviteLink = $"https://localhost:{_configuration["Port:number"]}/Login/Register/{aspuser.Id}";
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
                            LogEmail(body, subject, modal.Email, req.ConfirmationNumber, req.RequestId, -1, -1, true, retryCount, 1, 2);
                            break;
                        }
                        catch (Exception ex)
                        {

                            if (retryCount >= 3)
                            {
                                LogEmail(body, subject, modal.Email, req.ConfirmationNumber, req.RequestId, -1, -1, false, retryCount, 1, 2);
                            }
                            retryCount++;
                        }
                    }

                    return success;

                }
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public ViewNotesViewModel ViewNotes(int id)
        {
            Request request = _db.Requests.FirstOrDefault(r => r.RequestId == id);
            if (request == null)
            {
                return null;
            }
            RequestStatusLog patientcancel = _db.RequestStatusLogs.FirstOrDefault(r => r.RequestId == id && r.Status == 7);
            RequestStatusLog admincancel = _db.RequestStatusLogs.FirstOrDefault(r => r.RequestId == id && r.Status == 6);
            List<RequestStatusLog> transfernotes = _db.RequestStatusLogs.Where(r => r.RequestId == id && (r.Status == 1)).ToList();
            RequestNote requestNotes = _db.RequestNotes.FirstOrDefault(r => r.RequestId == id);

            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus,
                role = cookieModel.role
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

        public bool UpdateAdminNotes(ViewNotesViewModel viewNotesViewModel)
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);
            try
            {
                RequestNote requestNote = _db.RequestNotes.FirstOrDefault(r => r.RequestId == viewNotesViewModel.RequestId);
                if (requestNote != null)
                {
                    if (cookieModel.role == "Admin")
                    {
                        requestNote.AdminNotes = viewNotesViewModel.Admin_Note;
                    }
                    else
                    {
                        requestNote.PhysicianNotes = viewNotesViewModel.Physician_Note;
                    }
                    requestNote.ModifiedDate = DateTime.Now;
                    _db.RequestNotes.Update(requestNote);
                    _db.SaveChanges();
                }
                else
                {
                    RequestNote newRequestNote = new RequestNote
                    {
                        RequestId = viewNotesViewModel.RequestId,
                        CreatedDate = DateTime.Now,
                        CreatedBy = cookieModel.aspId,
                    };
                    if (cookieModel.role == "Admin")
                    {
                        newRequestNote.AdminNotes = viewNotesViewModel.Admin_Note;
                    }
                    else
                    {
                        newRequestNote.PhysicianNotes = viewNotesViewModel.Physician_Note;
                    }
                    _db.RequestNotes.Add(newRequestNote);
                    _db.SaveChanges();
                }
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public int Login(LoginViewModel loginViewModel)
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
            catch (Exception exp)
            {
                return 5;
            }
        }

        public int ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            var admin = _db.AspNetUsers.FirstOrDefault(a => a.Email == forgotPasswordViewModel.email);
            if (admin == null)
            {
                return 1;
            }

            var role = _db.AspNetUserRoles.FirstOrDefault(a => a.UserId == admin.Id);
            if (role.RoleId == 1)
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
                var inviteLink = $"https://localhost:{_configuration["Port:number"]}/Login/ResetPassword/?token={Token}";
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
            catch (Exception ex)
            {
                return 2;
            }
        }

        public bool Logout()
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ViewDocumentModal ViewUploads(int id)
        {
            var request = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id);
            if (request == null)
            {
                return null;
            }
            var documents = _db.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == id && u.IsDeleted.Equals(new BitArray(new[] { false }))).ToList();
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus,
                role = cookieModel.role
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

        public async Task<bool> FileUpload(IFormFile file, int id)
        {
            try
            {
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);
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
                if (cookieModel.role == "Provider")
                {
                    requestWiseFile.AdminId = null;
                    requestWiseFile.PhysicianId = cookieModel.userId;
                }
                _db.RequestWiseFiles.Add(requestWiseFile);
                _db.SaveChanges();
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public int DeleteSingleFile(int id)
        {
            RequestWiseFile requestWiseFile = _db.RequestWiseFiles.FirstOrDefault(r => r.RequestWiseFileId == id);
            if (requestWiseFile == null)
            {
                return -1;
            }
            requestWiseFile.IsDeleted = new BitArray(new[] { true });
            _db.RequestWiseFiles.Update(requestWiseFile);
            _db.SaveChanges();
            return requestWiseFile.RequestId;
        }

        public async Task<Tuple<MemoryStream, string>> DownloadMultipleFiles(ViewDocumentModal viewDocumentModal)
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

        public int DeleteAllFile(string filename)
        {
            string[] documentid = filename.Split(",");
            int requestid = 0;
            for (int i = 0; i < documentid.Length - 1; ++i)
            {
                var document = _db.RequestWiseFiles.FirstOrDefault(r => r.RequestWiseFileId == int.Parse(documentid[i]));
                if (document == null)
                {
                    return -1;
                }
                document.IsDeleted = new BitArray(new[] { true });
                _db.RequestWiseFiles.Update(document);
                _db.SaveChanges();
                requestid = document.RequestId;
            }
            return requestid;
        }

        public async Task<bool> SendDocumentsMail(string filename)
        {

            int retryCount = 1;
            bool success = false;

            while (retryCount <= 3 && !success) // Set retry limit
            {
                string[] documentid = filename.Split(",");
                var document = _db.RequestWiseFiles.Include(r => r.Request).FirstOrDefault(r => r.RequestWiseFileId == int.Parse(documentid[0]));
                if (document == null)
                {
                    return false;
                }
                var user = _db.RequestClients.FirstOrDefault(u => u.RequestClientId == document.Request.RequestClientId);
                string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                string senderPassword = "shahkandarp2430"; // Replace with your actual password (store securely)
                var platformTitle = "HalloDoc";
                var subject = "Documents - HalloDoc";
                var body = $"Hello {user.FirstName} {user.LastName},<br />We have attached few important documents in order to update about you with the progress of your request.<br /><br />Regards,<br/>{platformTitle}<br/>";
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);
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
                    LogEmail(body, subject, user.Email, document.Request.ConfirmationNumber, document.Request.RequestId, -1, -1, true, retryCount, 1, 3);
                    break;
                }
                catch (Exception ex)
                {

                    if (retryCount >= 3)
                    {
                        LogEmail(body, subject, user.Email, document.Request.ConfirmationNumber, document.Request.RequestId, -1, -1, false, retryCount, 1, 3);
                    }
                    retryCount++;
                }
            }

            return success;

        }

        public void LogEmail(string emailTemplate, string subject, string userEmail, string confirmation_no, int request_id, int admin_id, int physician_id, bool success, int retryCount, int role_id, int action)
        {
            if (role_id == 1)
            {
                var emailLog = new EmailLog
                {
                    EmailTemplate = emailTemplate,
                    SubjectName = subject,
                    EmailId = userEmail,
                    ConfirmationNumber = confirmation_no,
                    RequestId = request_id == -1 ? null : request_id,
                    IsEmailSent = new BitArray(new[] { success }),
                    SentTries = retryCount,
                    CreateDate = DateTime.Now,
                    RoleId = role_id,
                    SentDate = DateTime.Now,
                    Action = action
                };
                _db.EmailLogs.Add(emailLog);
                _db.SaveChanges();
            }
            else if (role_id == 3)
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
                    SentDate = DateTime.Now,
                    Action = action

                };
                _db.EmailLogs.Add(emailLog);
                _db.SaveChanges();
            }
            else if (role_id == 2)
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
                    SentDate = DateTime.Now,
                    Action = action

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
                    SentDate = DateTime.Now,
                    Action = action

                };
                _db.EmailLogs.Add(emailLog);
                _db.SaveChanges();
            }

        }

        public PasswordReset GetPasswordReset(string token)
        {
            return _db.PasswordResets.FirstOrDefault(u => u.Token == token) ?? null;
        }

        public bool ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
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
            catch (Exception exp)
            {
                return false;
            }
        }

        public List<RegionSpecificPhysician> GetPhysician(int regionid)
        {
            var query = from p in _db.Physicians
                        join pr in _db.PhysicianRegions on p.PhysicianId equals pr.PhysicianId
                        where pr.RegionId == regionid && p.IsDeleted == new BitArray(new[] { false })
                        select new RegionSpecificPhysician
                        {
                            PhysicianId = p.PhysicianId,
                            FirstName = p.FirstName,
                            LastName = p.LastName
                        };

            return query.ToList();
        }

        public bool AssignCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            try
            {

                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                Request request = _db.Requests.FirstOrDefault(r => r.RequestId == adminDashboardViewModel.RequestId);
                if (request == null)
                {
                    return false;
                }
                request.ModifiedDate = DateTime.Now;
                request.PhysicianId = adminDashboardViewModel.PhysicianId;
                _db.Requests.Update(request);

                Physician physician = _db.Physicians.FirstOrDefault(p => p.PhysicianId == adminDashboardViewModel.PhysicianId);
                if (physician == null)
                {
                    return false;
                }
                RequestStatusLog requestStatusLog = new RequestStatusLog
                {
                    RequestId = (int)adminDashboardViewModel.RequestId,
                    Status = 1,
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
            catch (Exception exp)
            {
                return false;
            }
        }
        
        public bool SamePhysicianAssignCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            try
            {
                Request request = _db.Requests.FirstOrDefault(r => r.RequestId == adminDashboardViewModel.RequestId);
                if (request == null)
                {
                    return true;
                }
                if(request.PhysicianId ==  adminDashboardViewModel.PhysicianId)
                {
                    return false;
                }
                return true;
            }
            catch (Exception exp)
            {
                return true;
            }
        }

        public bool TransferCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            try
            {
                Request request = _db.Requests.FirstOrDefault(r => r.RequestId == adminDashboardViewModel.RequestId);
                if (request == null)
                {
                    return false;
                }
                request.Status = 1;
                request.ModifiedDate = DateTime.Now;
                request.PhysicianId = adminDashboardViewModel.PhysicianId;
                _db.Requests.Update(request);

                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                Physician physician = _db.Physicians.FirstOrDefault(p => p.PhysicianId == adminDashboardViewModel.PhysicianId);
                if (physician == null)
                {
                    return false;
                }
                RequestStatusLog requestStatusLog = new RequestStatusLog
                {
                    RequestId = (int)adminDashboardViewModel.RequestId,
                    Status = 1,
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
            catch (Exception exp)
            {
                return false;
            }
        }

        public bool IsSamePhysician(AdminDashboardViewModel adminDashboardViewModel)
        {
            Request request = _db.Requests.FirstOrDefault(r => r.RequestId == adminDashboardViewModel.RequestId);

            if (request == null || request.PhysicianId == adminDashboardViewModel.PhysicianId)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> SendAgreement(AdminDashboardViewModel adminDashboardViewModel)
        {
            int retryCount = 1;
            bool success = false;

            while (retryCount <= 3 && !success) // Set retry limit
            {

                var user = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestClientId == adminDashboardViewModel.RequestId);
                if (user == null)
                {
                    return false;
                }
                string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                string senderPassword = "shahkandarp2430"; // Replace with your actual password (store securely)
                var platformTitle = "HalloDoc";
                var subject = "Agreement - HalloDoc";
                var inviteLink = $"https://localhost:{_configuration["Port:number"]}/Agreement/Index/{adminDashboardViewModel.RequestId}";
                var body = $"Hello {user.RequestClient.FirstName} {user.RequestClient.LastName},<br />Please review agreement and accept it so that we can start your treatment,<br /><br /><a href='{inviteLink}'>Review Agreement</a><br /><br />Regards,<br/>{platformTitle}<br/>";
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);
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
                    LogEmail(body, subject, adminDashboardViewModel.Mail_Email, user.ConfirmationNumber, user.RequestId, -1, -1, true, retryCount, 1, 4);
                    break;
                }
                catch (Exception ex)
                {

                    if (retryCount >= 3)
                    {
                        LogEmail(body, subject, adminDashboardViewModel.Mail_Email, user.ConfirmationNumber, user.RequestId, -1, -1, false, retryCount, 1, 4);
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
                var inviteLink = $"https://localhost:{_configuration["Port:number"]}/Agreement/Index/{adminDashboardViewModel.RequestId}";

                var accountSid = _configuration["Twilio:accountSid"];
                var authToken = _configuration["Twilio:authToken"];
                var twilionumber = _configuration["Twilio:twilioNumber"];
                var messageBody = $"Hello {user.RequestClient.FirstName} {user.RequestClient.LastName},\nPlease review agreement and accept it so that we can start your treatment,\n\n{inviteLink}\n\nRegards,\n{platformTitle}";

                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);
                try
                {

                    TwilioClient.Init(accountSid, authToken);



                    var message = MessageResource.Create(
                        from: new Twilio.Types.PhoneNumber(twilionumber),
                        body: messageBody,
                        to: new Twilio.Types.PhoneNumber(adminDashboardViewModel.Mail_PhoneNumber[0] == '+' && adminDashboardViewModel.Mail_PhoneNumber[1] == '9' && adminDashboardViewModel.Mail_PhoneNumber[2] == '1' ? adminDashboardViewModel.Mail_PhoneNumber : "+91" + adminDashboardViewModel.Mail_PhoneNumber)
                    );


                    success = true;
                    LogSMS(messageBody, adminDashboardViewModel.Mail_PhoneNumber, user.ConfirmationNumber, user.RequestId, -1, -1, true, retryCount, 1, 1);
                    break;
                }
                catch (Exception ex)
                {

                    if (retryCount >= 3)
                    {
                        LogSMS(messageBody, adminDashboardViewModel.Mail_PhoneNumber, user.ConfirmationNumber, user.RequestId, -1, -1, false, retryCount, 1, 1);
                    }
                    retryCount++;
                }
            }

            return success;
        }

        public void LogSMS(string SmsTemplate, string userPhone, string confirmation_no, int request_id, int admin_id, int physician_id, bool success, int retryCount, int role_id, int action)
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
                    SentDate = DateTime.Now,
                    Action = action

                };
                _db.Smslogs.Add(smslog);
                _db.SaveChanges();
            }
            else if (role_id == 3)
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
                    SentDate = DateTime.Now,
                    Action = action

                };
                _db.Smslogs.Add(smslog);
                _db.SaveChanges();
            }
            else if (role_id == 2)
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
                    SentDate = DateTime.Now,
                    Action = action

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
                    SentDate = DateTime.Now,
                    Action = action

                };
                _db.Smslogs.Add(smslog);
                _db.SaveChanges();
            }


        }

        public bool BlockCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            try
            {
                Request request = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(b => b.RequestId == adminDashboardViewModel.RequestId);
                if (request == null)
                {
                    return false;
                }
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
                    Name = string.Concat(request.RequestClient.FirstName, " ", request.RequestClient.LastName)
                };
                _db.BlockRequests.Add(blockRequest);
                _db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ClearCase(AdminDashboardViewModel adminDashboardViewModel)
        {
            try
            {
                Request request = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(b => b.RequestId == adminDashboardViewModel.RequestId);
                if (request == null)
                {
                    return false;
                }
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

        public OrdersViewModel Orders(int id)
        {

            Request request = _db.Requests.FirstOrDefault(r => r.RequestId == id);
            if (request == null)
            {
                return null;
            }

            var healthProfessionals = _db.HealthProfessionalTypes.Where(h => h.IsDeleted == new BitArray(new[] { false })).ToList();
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            OrdersViewModel ordersViewModel = new OrdersViewModel()
            {
                RequestId = id,
                healthProfessionalTypes = healthProfessionals,
                adminNavbarViewModel = adminNavbarViewModel
            };
            return ordersViewModel;
        }

        public List<HealthProfessional> GetBusiness(int professionid)
        {
            return _db.HealthProfessionals.Where(h => h.Profession == professionid && h.IsDeleted == new BitArray(new[] { false })).ToList();
        }

        public HealthProfessional GetBusinessData(int businessid)
        {
            return _db.HealthProfessionals.FirstOrDefault(h => h.VendorId == businessid);
        }

        public bool PlaceOrder(OrdersViewModel ordersViewModel)
        {
            try
            {
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                OrderDetail orderDetail = new OrderDetail()
                {
                    VendorId = ordersViewModel.business_id,
                    RequestId = ordersViewModel.RequestId,
                    FaxNumber = ordersViewModel.Business_fax,
                    Email = ordersViewModel.Business_email,
                    BusinessContact = ordersViewModel.Business_contact,
                    Prescription = ordersViewModel.prescription,
                    NoOfRefill = ordersViewModel.numberOfRefills == -1 ? null : ordersViewModel.numberOfRefills,
                    CreatedDate = DateTime.Now,
                    CreatedBy = cookieModel.name
                };
                _db.OrderDetails.Add(orderDetail);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public AdminProfileViewModel GetAdmin(int id, string active)
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            HalloDoc.Admin admin = _db.Admins.Include(a => a.AspNetUser).FirstOrDefault(a => a.AdminId == (id == -1 ? cookieModel.userId : id));
            if (admin == null)
            {
                return null;
            }
            List<Region> regions = _db.Regions.ToList();
            IQueryable<AdminRegion> adminRegions = _db.AdminRegions.Where(a => a.AdminId == (id == -1 ? cookieModel.userId : id));
            List<CheckboxViewModel> checkboxViewModels = new List<CheckboxViewModel>();
            List<Role> roles = _db.Roles.Where(r => r.AccountType == 1 && r.IsDeleted == new BitArray(new[] { false })).ToList();
            for (var i = 0; i < regions.Count; i++)
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
                curr_active = active,
                menus = cookieModel.menus,
                role = cookieModel.role,
                userId = cookieModel.userId
            };

            AdminProfileViewModel adminProfile = new AdminProfileViewModel()
            {
                UserName = admin.AspNetUser.UserName,
                status = admin.Status,
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
                roles = roles,
                admin_id = (id == -1 ? cookieModel.userId : id)
            };

            return adminProfile;

        }

        public int CheckAdminEmail(AdminProfileViewModel adminProfileViewModel)
        {
            try
            {
                HalloDoc.Admin admin = _db.Admins.FirstOrDefault(p => p.AdminId == (int)adminProfileViewModel.admin_id);
                if (admin == null)
                {
                    return 1;
                }
                if (adminProfileViewModel.Email != null && adminProfileViewModel.Email != admin.Email)
                {
                    HalloDoc.Admin admin1 = _db.Admins.FirstOrDefault(p => p.Email == adminProfileViewModel.Email);
                    if (admin1 != null)
                    {
                        return 3;
                    }
                }
                return 2;
            }
            catch (Exception exp)
            {
                return 1;
            }
        }

        public bool UpdateProfile(AdminProfileViewModel adminProfileViewModel)
        {
            try
            {
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                HalloDoc.Admin admin = _db.Admins.Include(a => a.AspNetUser).FirstOrDefault(a => a.AdminId == adminProfileViewModel.admin_id);
                if (admin == null)
                {
                    return false;
                }
                IQueryable<AdminRegion> adminRegions = _db.AdminRegions.Where(a => a.AdminId == adminProfileViewModel.admin_id);
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
                admin.Status = adminProfileViewModel?.status ?? admin.Status;
                admin.RoleId = adminProfileViewModel?.role_id ?? admin.RoleId;


                _db.Admins.Update(admin);

                for (var i = 0; i < adminProfileViewModel.checkboxViewModels.Count; ++i)
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
                    else if (adminProfileViewModel.checkboxViewModels[i].isChecked == false && adminRegions.FirstOrDefault(a => a.RegionId == adminProfileViewModel.checkboxViewModels[i].Id) != null)
                    {
                        AdminRegion adminRegion = adminRegions.FirstOrDefault(a => a.RegionId == adminProfileViewModel.checkboxViewModels[i].Id);
                        _db.AdminRegions.Remove(adminRegion);
                    }
                }

                _db.SaveChanges();

                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public bool ResetPasswordProfile(string password, int id)
        {
            try
            {
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);
                if (id != -1)
                {
                    id = _db.Admins.FirstOrDefault(a => a.AdminId == id).AspNetUserId;
                }
                AspNetUser aspNetUser = _db.AspNetUsers.FirstOrDefault(a => a.Id == (id == -1 ? cookieModel.aspId : id));
                if (aspNetUser == null)
                {
                    return false;
                }
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

        public Request GetRequest(int id)
        {
            return _db.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id) ?? null;
        }

        public bool Agree(int id)
        {
            try
            {
                var request = _db.Requests.FirstOrDefault(u => u.RequestId == id);
                if (request == null)
                {
                    return false;
                }
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
            catch (Exception exp)
            {
                return false;
            }
        }

        public bool Disagree(int id, string notes)
        {
            try
            {
                var request = _db.Requests.FirstOrDefault(u => u.RequestId == id);
                if (request == null)
                {
                    return false;
                }
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
            catch (Exception exp)
            {
                return false;
            }
        }

        public EncounterFormViewModel GetEncounterFormDetails(int id)
        {
            Request request = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(r => r.RequestId == id);
            if (request == null)
            {
                return null;
            }
            EncounterForm encounterForm = _db.EncounterForms.FirstOrDefault(r => r.RequestId == id);
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus,
                role = cookieModel.role
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
                Medications = encounterForm?.Medications,
                Allergies = encounterForm?.Allergies,
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

        public bool UpdateEncounterForm(EncounterFormViewModel encounterFormViewModel)
        {
            try
            {
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

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

                    if (cookieModel.role == "Provider")
                    {
                        encounter.IsFinalized = encounterFormViewModel.isFinalized == true ? new BitArray(new[] { true }) : new BitArray(new[] { false });
                    }

                    _db.EncounterForms.Add(encounter);

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

                    if (cookieModel.role == "Provider")
                    {
                        encounterForm.IsFinalized = encounterFormViewModel.isFinalized == true ? new BitArray(new[] { true }) : new BitArray(new[] { false });
                    }

                    _db.EncounterForms.Update(encounterForm);
                }

                if (cookieModel.role == "Provider")
                {
                    Request request = _db.Requests.FirstOrDefault(r => r.RequestId == encounterFormViewModel.RequestId);
                    if (request == null)
                    {
                        return false;
                    }
                    RequestClient requestClient = _db.RequestClients.FirstOrDefault(r => r.RequestClientId == request.RequestClientId);
                    requestClient.FirstName = encounterFormViewModel?.FirstName ?? requestClient.FirstName;
                    requestClient.LastName = encounterFormViewModel?.LastName ?? requestClient.LastName;
                    requestClient.Street = encounterFormViewModel?.Location.Split(",")[0] ?? requestClient.Street;
                    requestClient.City = encounterFormViewModel?.Location.Split(",")[1] ?? requestClient.City;
                    requestClient.State = encounterFormViewModel?.Location.Split(",")[2] ?? requestClient.State;
                    requestClient.ZipCode = encounterFormViewModel?.Location.Split(",")[3] ?? requestClient.ZipCode;
                    requestClient.IntDate = encounterFormViewModel?.DateOfBirth.Value.Day ?? requestClient.IntDate;
                    requestClient.IntYear = encounterFormViewModel?.DateOfBirth.Value.Year ?? requestClient.IntYear;
                    requestClient.StrMonth = encounterFormViewModel?.DateOfBirth.Value.Month.ToString() ?? requestClient.StrMonth;
                    requestClient.Email = encounterFormViewModel?.Email ?? requestClient.Email;
                    requestClient.PhoneNumber = encounterFormViewModel?.PhoneNumber ?? requestClient.PhoneNumber;
                    _db.RequestClients.Update(requestClient);
                }
                _db.SaveChanges();

                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public CloseCaseViewModel GetCloseCase(int id)
        {
            var request = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id);
            if (request == null)
            {
                return null;
            }
            var documents = _db.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == id && u.IsDeleted.Equals(new BitArray(new[] { false }))).ToList();
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Dashboard",
                menus = cookieModel.menus,
                role = cookieModel.role
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

        public bool UpdateCloseCase(CloseCaseViewModel closeCaseViewModel)
        {
            try
            {
                Request request = _db.Requests.FirstOrDefault(r => r.RequestId == closeCaseViewModel.RequestId);
                if (request == null)
                {
                    return false;
                }
                RequestClient requestClient = _db.RequestClients.FirstOrDefault(r => r.RequestClientId == request.RequestId);
                requestClient.PhoneNumber = closeCaseViewModel.PhoneNumber;
                requestClient.Email = closeCaseViewModel.Email;
                _db.RequestClients.Update(requestClient);
                _db.SaveChanges();
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public bool CloseCase(int id)
        {
            try
            {
                Request request = _db.Requests.FirstOrDefault(r => r.RequestId == id);
                if (request == null)
                {
                    return false;
                }
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
            catch (Exception exp)
            {
                return false;
            }
        }

        public ProviderViewModel GetProviderPageDetails(int id = -1, int page = 1, int pageSize = 10)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            IQueryable<Physician> physicians = _db.Physicians.Include(p => p.PhysicianRegions).Where(p => p.IsDeleted == new BitArray(new[] { false })).OrderByDescending(e => e.CreatedDate);

            if (id != -1 && id != null)
            {
                physicians = physicians.Where(p => p.PhysicianRegions.Any(pr => pr.RegionId == id));
            }

            List<Physician> physician = physicians.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            List<PhysicianProvider> physicianProviders = new List<PhysicianProvider>();

            var currentDate = DateTime.Now.Date;
            var currentTime = DateTime.Now.TimeOfDay;

            var active = from p in _db.Physicians
                         join pr in _db.PhysicianRegions on p.PhysicianId equals pr.PhysicianId
                         where _db.Shifts.Any(s => s.PhysicianId == p.PhysicianId &&
                                                          _db.ShiftDetails.Any(sd => s.ShiftId == sd.ShiftId &&
                                                                                        sd.ShiftDate.Date == currentDate &&
                                                                                        new TimeOnly(currentTime.Hours, currentTime.Minutes, currentTime.Seconds) >= sd.StartTime &&
                                                                                        new TimeOnly(currentTime.Hours, currentTime.Minutes, currentTime.Seconds) <= sd.EndTime && sd.Status == 1)) &&
                               p.IsDeleted == new BitArray(new[] { false })
                         select new MDOnCallPhysicians
                         {
                             PhysicianId = p.PhysicianId,
                             Name = p.FirstName + ", " + p.LastName.ToUpper()[0],
                             Photo = p.Photo,
                             RegionId = pr.RegionId,
                             Email = p.Email
                         };

            for (var i = 0; i < physician.Count; ++i)
            {
                PhysicianNotification physicianNotification = _db.PhysicianNotifications.FirstOrDefault(p => p.PhysicianId == physician[i].PhysicianId);
                Role role = _db.Roles.FirstOrDefault(r => r.RoleId == physician[i].RoleId);
                bool isActive = false;
                

                if (active.Where(a=>a.PhysicianId == physician[i].PhysicianId).Count() > 0)
                {
                    isActive = true;
                }
                physicianProviders.Add(new PhysicianProvider()
                {
                    isStopNotification = physicianNotification == null ? false : physicianNotification.IsNotificationStopped[0],
                    name = string.Concat(physician[i].FirstName, ", ", physician[i].LastName),
                    status = physician[i].Status,
                    role = role?.Name ?? "-",
                    physicianId = physician[i].PhysicianId,
                    oncallstatus = isActive == true ? "On Call" : "Unavailable",
                });
            }

            List<Region> regions = _db.Regions.ToList();

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Provider",
                menus = cookieModel.menus,
                role = cookieModel.role
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

        public bool ChangeNotification(int id, bool update)
        {
            try
            {
                PhysicianNotification physicianNotification = _db.PhysicianNotifications.FirstOrDefault(p => p.PhysicianId == id);
                if (physicianNotification == null)
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
            catch (Exception exp)
            {
                return false;
            }
        }

        public async Task<bool> ContactProvider(ProviderViewModel providerViewModel)
        {
            int retryCount = 1;
            bool success = false;

            if (providerViewModel.communication_type == "Email" || providerViewModel.communication_type == "Both")
            {
                while (retryCount <= 3 && !success) // Set retry limit
                {

                    var physician = _db.Physicians.FirstOrDefault(p => p.PhysicianId == providerViewModel.ProviderId);
                    if (physician == null)
                    {
                        return false;
                    }
                    string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                    string senderPassword = "shahkandarp2430"; // Replace with your actual password (store securely)
                    var platformTitle = "HalloDoc";
                    var subject = "Contact - HalloDoc";
                    var body = $"Hello {physician.FirstName} {physician.LastName},<br />{providerViewModel.message}<br /><br />Regards,<br/>{platformTitle}<br/>";
                    var request = _context.HttpContext.Request;
                    var token = request.Cookies["jwt"];
                    CookieModel cookieModel = _jwt.GetDetails(token);
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
                        LogEmail(body, subject, physician.Email, null, -1, -1, physician.PhysicianId, true, retryCount, 3, 5);
                        break;
                    }
                    catch (Exception ex)
                    {

                        if (retryCount >= 3)
                        {
                            LogEmail(body, subject, physician.Email, null, -1, -1, physician.PhysicianId, false, retryCount, 3, 5);
                        }
                        retryCount++;
                    }
                }
            }

            if (providerViewModel.communication_type == "SMS" || providerViewModel.communication_type == "Both")
            {
                retryCount = 1;
                success = false;

                while (retryCount <= 3 && !success) // Set retry limit
                {

                    var physician = _db.Physicians.FirstOrDefault(p => p.PhysicianId == providerViewModel.ProviderId);
                    if (physician == null)
                    {
                        return false;
                    }
                    string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                    string senderPassword = "shahkandarp2430"; // Replace with your actual password (store securely)
                    var platformTitle = "HalloDoc";

                    var accountSid = _configuration["Twilio:accountSid"];
                    var authToken = _configuration["Twilio:authToken"];
                    var twilionumber = _configuration["Twilio:twilioNumber"];
                    var messageBody = $"Hello {physician.FirstName} {physician.LastName},\n{providerViewModel.message}\n\nRegards,\n{platformTitle}";

                    var request = _context.HttpContext.Request;
                    var token = request.Cookies["jwt"];
                    CookieModel cookieModel = _jwt.GetDetails(token);
                    try
                    {

                        TwilioClient.Init(accountSid, authToken);

                        var message = MessageResource.Create(
                            from: new Twilio.Types.PhoneNumber(twilionumber),
                            body: messageBody,
                            to: new Twilio.Types.PhoneNumber(physician.Mobile[0] == '+' && physician.Mobile[1] == '9' && physician.Mobile[2] == '1' ? physician.Mobile : "+91" + physician.Mobile)
                        );


                        success = true;
                        LogSMS(messageBody, physician.Mobile, null, -1, -1, physician.PhysicianId, true, retryCount, 3, 2);
                        break;
                    }
                    catch (Exception ex)
                    {

                        if (retryCount >= 3)
                        {
                            LogSMS(messageBody, physician.Mobile, null, -1, -1, physician.PhysicianId, false, retryCount, 3, 2);
                        }
                        retryCount++;
                    }
                }
            }


            return success;
        }

        public PhysicianAccountViewModel GetCreatePhysicianDetails()
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Provider",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            List<Region> regions = _db.Regions.ToList();

            List<Role> roles = _db.Roles.Where(r => r.AccountType == 2 && r.IsDeleted == new BitArray(new[] { false })).ToList();

            List<CheckboxViewModel> checkboxViewModels = new List<CheckboxViewModel>();

            for (var i = 0; i < regions.Count; ++i)
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

        public async Task<bool> CreatePhysician(PhysicianAccountViewModel physicianAccountViewModel)
        {
            var passwordHasher = new PasswordHasher<AspNetUser>();
            Region region = _db.Regions.FirstOrDefault(r => r.RegionId == physicianAccountViewModel.RegionId);
            if (region == null)
            {
                return false;
            }
            AspNetUser aspNetUser = new AspNetUser()
            {
                Email = physicianAccountViewModel.Email,
                UserName = string.Concat("MD.", physicianAccountViewModel.LastName.Substring(0, 1).ToUpper(), physicianAccountViewModel.LastName.Substring(1).ToLower(), ".", physicianAccountViewModel.FirstName.Substring(0, 1).ToUpper()),
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
            CookieModel cookieModel = _jwt.GetDetails(token);

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
                physician.Signature = physicianAccountViewModel.Signature.FileName;
            }

            if (physicianAccountViewModel.Photo != null && physicianAccountViewModel.Photo.Length > 0)
            {
                var filePathh = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\provider_documents\\{physician.PhysicianId}", physicianAccountViewModel.Photo.FileName);
                using (var stream = System.IO.File.Create(filePathh))
                {
                    await physicianAccountViewModel.Photo.CopyToAsync(stream);
                }
                physician.Photo = physicianAccountViewModel.Signature.FileName;
            }

            _db.Physicians.Update(physician);
            _db.SaveChanges();

            for (var i = 0; i < physicianAccountViewModel.checkboxViewModels.Count; ++i)
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

            if (physicianAccountViewModel.IsAgreementDoc)
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
                    LogEmail(body, subject, physician.Email, null, -1, -1, physician.PhysicianId, true, retryCount, 3, 6);
                    break;
                }
                catch (Exception ex)
                {

                    if (retryCount >= 3)
                    {
                        LogEmail(body, subject, physician.Email, null, -1, -1, physician.PhysicianId, false, retryCount, 3, 6);
                    }
                    retryCount++;
                }
            }

            return success;
        }

        public List<Role> GetPhysicianRoles()
        {
            return _db.Roles.Where(r => r.AccountType == 2 && r.IsDeleted == new BitArray(new[] { false })).ToList();
        }

        public PhysicianAccountViewModel GetPhysicianDetails(int id, AdminNavbarViewModel adminNavbarViewModel)
        {
            Physician physician = _db.Physicians.Include(p => p.AspNetUser).FirstOrDefault(p => p.PhysicianId == id);
            if (physician == null)
            {
                return null;
            }
            PhysicianLocation physicianLocation = _db.PhysicianLocations.FirstOrDefault(p => p.PhysicianId == id);
            IQueryable<PhysicianRegion> physicianRegion = _db.PhysicianRegions.Where(p => p.PhysicianId == id);
            List<Region> regions = _db.Regions.ToList();

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

        public async Task<bool> FileUploadPhysician(IFormFile file, int id, string name)
        {
            try
            {
                Physician physician = _db.Physicians.FirstOrDefault(p => p.PhysicianId == id);
                if (physician == null)
                {
                    return false;
                }
                var filePathh = "";

                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

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
            catch (Exception exp)
            {
                return false;
            }
        }

        public int CheckPhysicianEmail(PhysicianAccountViewModel physicianAccountViewModel)
        {
            try
            {
                Physician physician = _db.Physicians.FirstOrDefault(p => p.PhysicianId == physicianAccountViewModel.PhysicianId);
                if (physician == null)
                {
                    return 1;
                }
                if (physicianAccountViewModel.Email != null && physicianAccountViewModel.Email != physician.Email)
                {
                    Physician physician1 = _db.Physicians.FirstOrDefault(p => p.Email == physicianAccountViewModel.Email);
                    if (physician1 != null)
                    {
                        return 3;
                    }
                }
                return 2;
            }
            catch (Exception exp)
            {
                return 1;
            }
        }

        public async Task<bool> UpdatePhysician(PhysicianAccountViewModel physicianAccountViewModel)
        {
            try
            {

                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                Physician physician = _db.Physicians.FirstOrDefault(p => p.PhysicianId == physicianAccountViewModel.PhysicianId);
                if (physician == null)
                {
                    return false;
                }
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
                physician.Address2 = physicianAccountViewModel?.Address2 ?? physician.Address2;
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

                IQueryable<PhysicianRegion> physicianRegions = _db.PhysicianRegions.Where(p => p.PhysicianId == physicianAccountViewModel.PhysicianId);

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
            catch (Exception exp)
            {
                return false;
            }
        }

        public bool ResetPasswordPhysician(string password, int id)
        {
            try
            {
                AspNetUser aspNetUser = _db.AspNetUsers.FirstOrDefault(a => a.Id == id);
                if (aspNetUser == null)
                {
                    return false;
                }
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

        public bool DeletePhysician(int id)
        {
            try
            {
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                Physician physician = _db.Physicians.FirstOrDefault(p => p.PhysicianId == id);
                if (physician == null)
                {
                    return false;
                }
                physician.IsDeleted = new BitArray(new[] { true });
                physician.ModifiedDate = DateTime.Now;
                physician.ModifiedBy = cookieModel.aspId;

                _db.Physicians.Update(physician);
                _db.SaveChanges();
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public PatientHistoryViewModel GetAllPatients(string? firstname, string? lastname, string? email, string? phone, int page = 1, int pageSize = 10)
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Record",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            IQueryable<User> users = _db.Users;

            if (firstname != null)
            {
                users = users.Where(r => r.FirstName.ToLower().Contains(firstname.ToLower()));
            }
            if (lastname != null)
            {
                users = users.Where(r => r.LastName.ToLower().Contains(lastname.ToLower()));
            }
            if (email != null)
            {
                users = users.Where(r => r.Email.ToLower().Contains(email.ToLower()));
            }
            if (phone != null)
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

        public PatientHistoryViewModel GetAllPatientRecords(int id, int page = 1, int pageSize = 10)
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            int count = _db.Requests.Where(u => u.UserId == id).Count();
            List<RequestViewModel> data = _db.RequestViewModels.FromSqlRaw($"SELECT * FROM PatientDashboardData({id},{pageSize},{((page - 1) * pageSize)})").ToList();
            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Record",
                menus = cookieModel.menus,
                role = cookieModel.role
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

        public BlockHistoryViewModel GetBlockHistoryData(string? name, DateTime? date, string? email, string? phone, int page = 1, int pageSize = 10)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Record",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            IQueryable<BlockRequest> blockRequests = _db.BlockRequests;

            if (name != null)
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

        public bool ToggleActive(int blockrequestid, bool value)
        {
            try
            {
                BlockRequest blockRequest = _db.BlockRequests.FirstOrDefault(b => b.BlockRequestId == blockrequestid);
                if (blockRequest == null)
                {
                    return false;
                }
                blockRequest.IsActive = new BitArray(new[] { value });
                blockRequest.ModifiedDate = DateTime.Now;
                _db.BlockRequests.Update(blockRequest);
                _db.SaveChanges();
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public bool RestoreBlock(int blockrequestid)
        {
            try
            {
                BlockRequest blockRequest = _db.BlockRequests.FirstOrDefault(b => b.BlockRequestId == blockrequestid);
                if (blockRequest == null)
                {
                    return false;
                }
                var requestid = blockRequest.RequestId;
                _db.BlockRequests.Remove(blockRequest);

                var status = 1;

                Request request = _db.Requests.FirstOrDefault(r => r.RequestId == int.Parse(requestid));
                if (request == null)
                {
                    return false;
                }
                request.Status = (short)status;
                request.ModifiedDate = DateTime.Now;
                _db.Requests.Update(request);

                RequestStatusLog requestStatusLog = new RequestStatusLog
                {
                    Status = (short)status,
                    CreatedDate = DateTime.Now,
                    RequestId = int.Parse(requestid),
                };
                _db.RequestStatusLogs.Add(requestStatusLog);
                _db.SaveChanges();

                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public SearchRecordViewModel GetSearchedData(int? status, string? name, int? requesttypeid, DateTime? fromdos, DateTime? todos, string? providername, string? email, string? phonenumber, int page = 1, int pageSize = 10)
        {

            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Record",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            List<RequestType> requestTypes = _db.RequestTypes.ToList();

            IQueryable<Request> requests = _db.Requests.Include(r => r.RequestClient).Include(r => r.RequestNotes).Include(r => r.RequestStatusLogs).Include(r => r.Physician).Where(r => r.IsDeleted == new BitArray(new[] { false }));
            if (status != null && status != -1)
            {
                requests = requests.Where(r => r.Status == status);
            }
            if (name != null)
            {
                requests = requests.Where(r => r.RequestClient.FirstName.ToLower().Contains(name.ToLower()) || r.RequestClient.LastName.ToLower().Contains(name.ToLower()));
            }
            if (requesttypeid != null && requesttypeid != -1)
            {
                requests = requests.Where(r => r.RequestTypeId == requesttypeid);
            }
            if (fromdos != null && todos == null)
            {
                requests = requests.Where(r => r.AcceptedDate.Value.Date >= fromdos.Value.Date);
            }
            if (fromdos == null && todos != null)
            {
                requests = requests.Where(r => r.AcceptedDate.Value.Date <= todos.Value.Date);
            }
            if (fromdos != null && todos != null)
            {
                requests = requests.Where(r => r.AcceptedDate.Value.Date >= fromdos.Value.Date && r.AcceptedDate.Value.Date <= todos.Value.Date);
            }
            if (providername != null)
            {
                requests = requests.Where(r => r.Physician.FirstName.ToLower().Contains(providername.ToLower()) || r.Physician.LastName.ToLower().Contains(providername.ToLower()));
            }
            if (email != null)
            {
                requests = requests.Where(r => r.RequestClient.Email.ToLower().Contains(email.ToLower()));
            }
            if (phonenumber != null)
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

        public bool DeleteRequest(int id)
        {
            try
            {
                Request req = _db.Requests.FirstOrDefault(r => r.RequestId == id);
                if (req == null)
                {
                    return false;
                }
                req.IsDeleted = new BitArray(new[] { true });
                req.ModifiedDate = DateTime.Now;
                _db.Requests.Update(req);
                _db.SaveChanges();
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }

        }

        public MemoryStream ExportSearchedData(SearchRecordViewModel model)
        {
            try
            {
                var data = model.alldata;
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
                    if (item.RequestTypeId == 1)
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

        public AccountAccessViewModel GetAllRolesDetails(int page = 1, int pageSize = 10)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Access",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            IQueryable<Role> roles = _db.Roles.Where(r => r.IsDeleted == new BitArray(new[] { false }));

            AccountAccessViewModel accountAccessViewModel = new AccountAccessViewModel
            {
                roles = roles.Skip((page - 1) * pageSize).Take(pageSize).OrderByDescending(r => r.CreatedBy).ToList(),
                adminNavbarViewModel = adminNavbarViewModel,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = roles.Count(),
                TotalPages = (int)Math.Ceiling((double)roles.Count() / pageSize)
            };

            return accountAccessViewModel;

        }

        public AdminNavbarViewModel GetCreateAccessNavbar()
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Access",
                menus = cookieModel.menus,
                role = cookieModel.role
            };
            return adminNavbarViewModel;
        }

        public List<Menu> GetMenus(int? id)
        {
            if (id == -1)
            {
                return _db.Menus.ToList();
            }

            return _db.Menus.Where(m => m.AccountType == id).ToList();
        }

        public bool CheckRole(string? role_name)
        {
            return _db.Roles.Where(r=>r.Name.ToLower().Replace(" ","") == role_name.ToLower().Replace(" ", "") && r.IsDeleted == new BitArray(new[] { false })).Count() > 0 ? true : false;
        }

        public bool CreateRole(string? menus, string? role_name, int? account_type)
        {
            try
            {
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

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


                for (var i = 0; i < menu.Length - 1; ++i)
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

        public bool DeleteRole(int? id)
        {
            try
            {
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                Role role = _db.Roles.FirstOrDefault(r => r.RoleId == id);
                if (role == null)
                {
                    return false;
                }
                role.IsDeleted = new BitArray(new[] { true });
                role.ModifiedDate = DateTime.Now;
                role.ModifiedBy = cookieModel.name;

                _db.Roles.Update(role);
                _db.SaveChanges();

                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public EditAccessViewModel GetRoleDetails(int? id)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Access",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            Role role = _db.Roles.FirstOrDefault(r => r.RoleId == id);
            if (role == null)
            {
                return null;
            }
            IQueryable<RoleMenu> roleMenus = _db.RoleMenus.Where(r => r.RoleId == id);

            List<Menu> menus = _db.Menus.Where(r => r.AccountType == role.AccountType).ToList();

            List<CheckboxViewModel> checkboxViewModels = new List<CheckboxViewModel>();

            string roles = "";

            for (var i = 0; i < roleMenus.Count(); ++i)
            {
                roles += roleMenus.ToList()[i].MenuId + ",";
            }

            for (var i = 0; i < menus.Count; i++)
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

        public bool EditRoleDetails(int? id, string? menus, string? role_name, int? account_type)
        {
            try
            {
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                string[] menu = menus.Split(",");

                Role role = _db.Roles.FirstOrDefault(r => r.RoleId == id);
                if (role == null)
                {
                    return false;
                }

                if (role.AccountType == account_type)
                {

                    IQueryable<RoleMenu> roleMenus = _db.RoleMenus.Where(r => r.RoleId == id);
                    for (var i = 0; i < menu.Length - 1; ++i)
                    {
                        if (roleMenus.FirstOrDefault(r => r.MenuId == int.Parse(menu[i])) == null)
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

                    for (var i = 0; i < roleMenus.Count(); ++i)
                    {
                        keyValuePairs[roleMenus.ToList()[i].MenuId] = false;
                    }

                    for (var i = 0; i < menu.Length - 1; ++i)
                    {
                        keyValuePairs[int.Parse(menu[i])] = true;
                    }

                    for (var i = 0; i < roleMenus.Count(); ++i)
                    {
                        if (keyValuePairs[roleMenus.ToList()[i].MenuId] == false)
                        {
                            _db.RoleMenus.Remove(roleMenus.FirstOrDefault(r => r.MenuId == roleMenus.ToList()[i].MenuId));
                        }
                    }

                }
                else
                {
                    IQueryable<RoleMenu> roleMenus = _db.RoleMenus.Where(r => r.RoleId == id);
                    for (var i = 0; i < roleMenus.Count(); ++i)
                    {
                        _db.RoleMenus.Remove(roleMenus.FirstOrDefault(r => r.MenuId == roleMenus.ToList()[i].MenuId));
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
            catch (Exception exp)
            {
                return false;
            }
        }

        public EmailLogViewModel GetEmailLogDetails(int? roleid, string? name, string? email, DateTime? createddate, DateTime? sentdate, int page = 1, int pageSize = 10)
        {

            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Record",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            IQueryable<ELogViewModel> emailLogs = _db.ELogViewModels.FromSqlRaw($"SELECT * FROM EmailLog()");


            if (roleid != null && roleid != -1)
            {
                emailLogs = emailLogs.Where(r => r.roleid == roleid);
            }
            if (email != null)
            {
                emailLogs = emailLogs.Where(r => r.emailid.ToLower().Contains(email.ToLower()));
            }
            if (createddate != null)
            {
                emailLogs = emailLogs.Where(r => r.createddate.Value.Date == createddate.Value.Date);
            }
            if (sentdate != null)
            {
                emailLogs = emailLogs.Where(r => r.sentdate.Value.Date == sentdate.Value.Date);
            }
            if (name != null)
            {
                emailLogs = emailLogs.Where(r => r.name.ToLower().Contains(name.ToLower()));
            }

            List<AspNetRole> roles = _db.AspNetRoles.ToList();

            EmailLogViewModel emailLogViewModel = new EmailLogViewModel
            {
                roles = roles,
                adminNavbarViewModel = adminNavbarViewModel,
                logViewModels = emailLogs.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = emailLogs.Count(),
                TotalPages = (int)Math.Ceiling((double)emailLogs.Count() / pageSize)
            };
            return emailLogViewModel;
        }


        public EmailLogViewModel GetSMSLogDetails(int? roleid, string? name, string? phonenumber, DateTime? createddate, DateTime? sentdate, int page = 1, int pageSize = 10)
        {

            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Record",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            IQueryable<SMSLogViewModel> smslogs = _db.SMSLogViewModels.FromSqlRaw($"SELECT * FROM SmsLog()");

            if (roleid != null && roleid != -1)
            {
                smslogs = smslogs.Where(r => r.roleid == roleid);
            }
            if (phonenumber != null)
            {
                smslogs = smslogs.Where(r => r.mobilenumber.ToLower().Contains(phonenumber.ToLower()));
            }
            if (createddate != null)
            {
                smslogs = smslogs.Where(r => r.createddate.Value.Date == createddate.Value.Date);
            }
            if (sentdate != null)
            {
                smslogs = smslogs.Where(r => r.sentdate.Value.Date == sentdate.Value.Date);
            }
            if (name != null)
            {
                smslogs = smslogs.Where(r => r.name.ToLower().Contains(name.ToLower()));
            }

            List<AspNetRole> roles = _db.AspNetRoles.ToList();

            EmailLogViewModel emailLogViewModel = new EmailLogViewModel
            {
                roles = roles,
                adminNavbarViewModel = adminNavbarViewModel,
                smsLogViewModels = smslogs.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = smslogs.Count(),
                TotalPages = (int)Math.Ceiling((double)smslogs.Count() / pageSize)
            };
            return emailLogViewModel;
        }

        public PartnerViewModal GetPartnerDetails(string? name, int? id, int page = 1, int pageSize = 10)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Partner",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            List<HealthProfessionalType> healthProfessionalTypes = _db.HealthProfessionalTypes.Where(r => r.IsDeleted == new BitArray(new[] { false })).ToList();
            IQueryable<HealthProfessional> healthProfessionals = _db.HealthProfessionals.Include(r => r.ProfessionNavigation).Where(r => r.IsDeleted == new BitArray(new[] { false }));

            if (name != null)
            {
                healthProfessionals = healthProfessionals.Where(r => r.VendorName.ToLower().Contains(name.ToLower()));
            }
            if (id != null && id != -1)
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

        public BusinessViewModel GetBusinessNavbar()
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            List<HealthProfessionalType> healthProfessionalTypes = _db.HealthProfessionalTypes.Where(r => r.IsDeleted == new BitArray(new[] { false })).ToList();

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Partner",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            BusinessViewModel businessViewModel = new BusinessViewModel
            {
                adminNavbarViewModel = adminNavbarViewModel,
                page = "Add Business",
                healthProfessionalTypes = healthProfessionalTypes
            };
            return businessViewModel;
        }

        public bool CreateBusiness(BusinessViewModel businessViewModel)
        {
            try
            {
                Region region = new Region();
                if (businessViewModel.State != null)
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
            catch (Exception exp)
            {
                return false;
            }
        }

        public BusinessViewModel GetBusinessDetails(int id)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            List<HealthProfessionalType> healthProfessionalTypes = _db.HealthProfessionalTypes.Where(r => r.IsDeleted == new BitArray(new[] { false })).ToList();

            HealthProfessional healthProfessional = _db.HealthProfessionals.FirstOrDefault(h => h.VendorId == id);
            if (healthProfessional == null)
            {
                return null;
            }

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Partner",
                menus = cookieModel.menus,
                role = cookieModel.role
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

        public bool EditBusiness(BusinessViewModel businessViewModel)
        {
            try
            {
                Region region = new Region();
                if (businessViewModel.State != null)
                {
                    region = _db.Regions.FirstOrDefault(u => u.Name == businessViewModel.State.Trim().ToLower().Replace(" ", ""));
                }

                HealthProfessional healthProfessional = _db.HealthProfessionals.FirstOrDefault(h => h.VendorId == businessViewModel.BusinessId);

                if (healthProfessional == null)
                {
                    return false;
                }

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

        public bool DeleteBusiness(int id)
        {
            try
            {
                HealthProfessional healthProfessional = _db.HealthProfessionals.FirstOrDefault(h => h.VendorId == id);
                if (healthProfessional == null)
                {
                    return false;
                }
                healthProfessional.IsDeleted = new BitArray(new[] { true });
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

        public ProviderLocationViewModel GetProviderLocation()
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "ProviderLocation",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            List<PhysicianLocation> physicianLocations = _db.PhysicianLocations.ToList();

            ProviderLocationViewModel providerLocationViewModel = new ProviderLocationViewModel
            {
                adminNavbarViewModel = adminNavbarViewModel,
                physicianLocations = physicianLocations
            };

            return providerLocationViewModel;

        }

        public AdminProfileViewModel GetCreateAdminProfilePageDetails()
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Access",
                menus = cookieModel.menus,
                role = cookieModel.role
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

        public List<Role> GetAdminRoles()
        {
            return _db.Roles.Where(r => r.AccountType == 1 && r.IsDeleted == new BitArray(new[] { false })).ToList();
        }

        public async Task<bool> CreateAdmin(AdminProfileViewModel adminProfileViewModel)
        {
            try
            {
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

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
                        LogEmail(body, subject, admin.Email, null, -1, admin.AdminId, -1, true, retryCount, 2, 7);
                        break;
                    }
                    catch (Exception ex)
                    {

                        if (retryCount >= 3)
                        {
                            LogEmail(body, subject, admin.Email, null, -1, admin.AdminId, -1, false, retryCount, 2, 7);
                        }
                        retryCount++;
                    }
                }

                return success;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public UserAccessViewModel GetUserAccessDetails(int? roleid, int page = 1, int pageSize = 10)
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
                                          anr.RoleId == 3 ? _db.Requests.Count(r => (r.Status == 1 || r.Status == 2 || r.Status == 3 || r.Status == 4 || r.Status == 5) && r.IsDeleted == new BitArray(new[] { false }) && r.PhysicianId == ph.PhysicianId) : 0,
                            PhysicianId = ph.PhysicianId == null ? -1 : ph.PhysicianId,
                            AdminId = ad.AdminId == null ? -1 : ad.AdminId,
                        };


            if (roleid == 1)
            {
                query = query.Where(r => r.AccountType == "Admin");
            }
            else if (roleid == 2)
            {
                query = query.Where(r => r.AccountType == "Provider");
            }

            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Access",
                menus = cookieModel.menus,
                role = cookieModel.role
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

        public bool DeleteAdmin(int id)
        {
            try
            {
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                HalloDoc.Admin admin = _db.Admins.FirstOrDefault(a => a.AdminId == id);
                if (admin == null)
                {
                    return false;
                }
                admin.IsDeleted = true;
                admin.ModifiedDate = DateTime.Now;
                admin.ModifiedBy = cookieModel.aspId;
                _db.Admins.Update(admin);
                _db.SaveChanges();
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public SchedulingViewModel GetAllShiftDetails(int? regionid)
        {
            List<Region> regions = _db.Regions.ToList();

            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            if (cookieModel.role == "Provider")
            {
                var regionss = from r in _db.Regions
                               where _db.PhysicianRegions
                                             .Where(pr => pr.PhysicianId == cookieModel.userId)
                                             .Select(pr => pr.RegionId)
                                             .Contains(r.RegionId)
                               select r;

                regions = regionss.ToList();
            }

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Provider",
                menus = cookieModel.menus,
                role = cookieModel.role
            };
            if (cookieModel.role == "Provider")
            {
                adminNavbarViewModel.curr_active = "DoctorSchedule";
            }

            List<Physician> physicians = _db.Physicians.Where(p => p.IsDeleted == new BitArray(new[] { false })).ToList();

            var query = from s in _db.Shifts
                        join sd in _db.ShiftDetails on s.ShiftId equals sd.ShiftId
                        join r in _db.Regions on sd.RegionId equals r.RegionId into regionGroup
                        from r in regionGroup.DefaultIfEmpty()
                        join p in _db.Physicians on s.PhysicianId equals p.PhysicianId
                        where sd.IsDeleted == new BitArray(new[] { false })
                        select new ShiftViewModel
                        {
                            PhysicianId = s.PhysicianId,
                            RegionId = (int)sd.RegionId,
                            PhysicianName = p.LastName.ToUpper() + ", " + p.FirstName.ToUpper()[0] + ".",
                            RegionAbbreviation = r.Abbreviation,
                            ShiftDate = sd.ShiftDate,
                            StartTime = sd.StartTime,
                            EndTime = sd.EndTime,
                            Status = sd.Status,
                            ShiftDetailId = sd.ShiftDetailId
                        };

            if (cookieModel.role == "Provider")
            {
                query = query.Where(q => q.PhysicianId == cookieModel.userId);
            }

            if (regionid != null && regionid != -1)
            {
                query = query.Where(r => r.RegionId == regionid);
            }


            string[] days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

            List<CheckboxViewModel> checkboxViewModels = new List<CheckboxViewModel>();

            for (var i = 0; i < days.Length; ++i)
            {
                checkboxViewModels.Add(new CheckboxViewModel
                {
                    Id = i,
                    Name = days[i],
                    isChecked = false
                });
            }

            SchedulingViewModel schedulingViewModel = new SchedulingViewModel()
            {
                adminNavbarViewModel = adminNavbarViewModel,
                regions = regions,
                checkboxViewModels = checkboxViewModels,
                IsRepeat = false,
                physicians = physicians,
                shiftViewModels = query.ToList()
            };

            return schedulingViewModel;

        }

        public int CreateShift(SchedulingViewModel schedulingViewModel)
        {
            try
            {
                var count = _db.Shifts
                .Join(_db.ShiftDetails,
                      s => s.ShiftId,
                      sd => sd.ShiftId,
                      (s, sd) => new { Shift = s, ShiftDetail = sd })
                .Where(x => x.Shift.PhysicianId == schedulingViewModel.PhysicianId && x.ShiftDetail.ShiftDate == schedulingViewModel.StartDate && ((x.ShiftDetail.StartTime <= schedulingViewModel.StartTime && x.ShiftDetail.EndTime > schedulingViewModel.StartTime) || (x.ShiftDetail.StartTime <= schedulingViewModel.EndTime && x.ShiftDetail.EndTime > schedulingViewModel.EndTime) || (schedulingViewModel.StartTime < x.ShiftDetail.StartTime && schedulingViewModel.EndTime > x.ShiftDetail.EndTime)) && x.ShiftDetail.IsDeleted == new BitArray(new[] { false }))
                .Count();

                if(count > 0)
                {
                    return 1;
                }

                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                var weekDays = "";

                for (var i = 0; i < schedulingViewModel.checkboxViewModels.Count; ++i)
                {
                    if (schedulingViewModel.checkboxViewModels[i].isChecked)
                    {
                        weekDays += "1";
                    }
                    else
                    {
                        weekDays += "0";
                    }
                }

                short status = 1;
                if (cookieModel.role == "Provider")
                {
                    status = 0;
                }

                Shift shift = new Shift
                {
                    PhysicianId = (int)schedulingViewModel.PhysicianId,
                    StartDate = DateOnly.FromDateTime(schedulingViewModel.StartDate),
                    IsRepeat = new BitArray(new[] { (bool)schedulingViewModel.IsRepeat }),
                    RepeatUpto = schedulingViewModel.Repeat,
                    CreatedDate = DateTime.Now,
                    CreatedBy = cookieModel.aspId,
                    WeekDays = weekDays
                };

                _db.Shifts.Add(shift);

                ShiftDetail shiftDetail = new ShiftDetail
                {
                    Shift = shift,
                    ShiftDate = schedulingViewModel.StartDate,
                    RegionId = schedulingViewModel.RegionId,
                    StartTime = schedulingViewModel.StartTime,
                    EndTime = schedulingViewModel.EndTime,
                    Status = status,
                    IsDeleted = new BitArray(new[] { false }),
                };
                _db.ShiftDetails.Add(shiftDetail);

                ShiftDetailRegion shiftDetailRegion = new ShiftDetailRegion
                {
                    ShiftDetail = shiftDetail,
                    RegionId = (int)schedulingViewModel.RegionId
                };
                _db.ShiftDetailRegions.Add(shiftDetailRegion);

                for (var i = 0; i < schedulingViewModel.checkboxViewModels.Count; ++i)
                {
                    var flag = 0;
                    if (schedulingViewModel.checkboxViewModels[i].isChecked)
                    {
                        for (var j = 0; j < schedulingViewModel.Repeat; ++j)
                        {
                            DateTime? shiftdate = new DateTime();
                            if ((int)schedulingViewModel.StartDate.DayOfWeek < (int)schedulingViewModel.checkboxViewModels[i].Id && flag == 0)
                            {
                                shiftdate = schedulingViewModel.StartDate.AddDays((int)schedulingViewModel.checkboxViewModels[i].Id - (int)schedulingViewModel.StartDate.DayOfWeek);
                                flag = 1;
                            }
                            else if ((int)schedulingViewModel.StartDate.DayOfWeek == (int)schedulingViewModel.checkboxViewModels[i].Id && flag == 0)
                            {
                                shiftdate = null;
                                flag = 1;
                            }
                            else
                            {
                                shiftdate = schedulingViewModel.StartDate.AddDays(7 * (flag == 1 ? j : j + 1) - (int)schedulingViewModel.StartDate.DayOfWeek + (int)schedulingViewModel.checkboxViewModels[i].Id);
                            }

                            if (shiftdate != null)
                            {
                                ShiftDetail shiftDetail1 = new ShiftDetail
                                {
                                    Shift = shift,
                                    ShiftDate = shiftdate.Value,
                                    RegionId = schedulingViewModel.RegionId,
                                    StartTime = schedulingViewModel.StartTime,
                                    EndTime = schedulingViewModel.EndTime,
                                    Status = status,
                                    IsDeleted = new BitArray(new[] { false }),
                                };
                                _db.ShiftDetails.Add(shiftDetail1);

                                ShiftDetailRegion shiftDetailRegion1 = new ShiftDetailRegion
                                {
                                    ShiftDetail = shiftDetail1,
                                    RegionId = (int)schedulingViewModel.RegionId
                                };
                                _db.ShiftDetailRegions.Add(shiftDetailRegion1);

                                var countt = _db.Shifts
                                .Join(_db.ShiftDetails,
                                      s => s.ShiftId,
                                      sd => sd.ShiftId,
                                      (s, sd) => new { Shift = s, ShiftDetail = sd })
                                .Where(x => x.Shift.PhysicianId == schedulingViewModel.PhysicianId && x.ShiftDetail.ShiftDate == shiftdate.Value && ((x.ShiftDetail.StartTime <= schedulingViewModel.StartTime && x.ShiftDetail.EndTime > schedulingViewModel.StartTime) || (x.ShiftDetail.StartTime <= schedulingViewModel.EndTime && x.ShiftDetail.EndTime > schedulingViewModel.EndTime) || (schedulingViewModel.StartTime < x.ShiftDetail.StartTime && schedulingViewModel.EndTime > x.ShiftDetail.EndTime)) && x.ShiftDetail.IsDeleted == new BitArray(new[] { false }))
                                .Count();

                                if (countt > 0)
                                {
                                    return 1;
                                }
                            }

                        }
                        flag = 0;
                    }
                }

                _db.SaveChanges();

                return 3;
            }
            catch (Exception exp)
            {
                return 2;
            }

        }

        public int EditShift(DateTime shiftdate, TimeOnly starttime, TimeOnly endtime, int physicianid, int shiftdetailid)
        {
            try
            {
                var count = _db.Shifts
                .Join(_db.ShiftDetails,
                      s => s.ShiftId,
                      sd => sd.ShiftId,
                      (s, sd) => new { Shift = s, ShiftDetail = sd })
                .Where(x => x.ShiftDetail.ShiftDetailId != shiftdetailid && x.Shift.PhysicianId == physicianid && x.ShiftDetail.ShiftDate == shiftdate && ((x.ShiftDetail.StartTime <= starttime && x.ShiftDetail.EndTime > starttime) || (x.ShiftDetail.StartTime <= endtime && x.ShiftDetail.EndTime > endtime) || (starttime < x.ShiftDetail.StartTime && endtime > x.ShiftDetail.EndTime)) && x.ShiftDetail.IsDeleted == new BitArray(new[] { false }))
                .Count();

                if (count > 0)
                {
                    return 1;
                }

                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                ShiftDetail shiftDetail = _db.ShiftDetails.FirstOrDefault(s => s.ShiftDetailId == shiftdetailid);
                if (shiftDetail == null)
                {
                    return 2;
                }
                shiftDetail.StartTime = starttime;
                shiftDetail.EndTime = endtime;
                shiftDetail.ShiftDate = shiftdate;
                shiftDetail.ModifiedBy = cookieModel.aspId;
                shiftDetail.ModifiedDate = DateTime.Now;

                _db.ShiftDetails.Update(shiftDetail);
                _db.SaveChanges();
                return 3;
            }
            catch (Exception exp)
            {
                return 2;
            }
        }

        public bool DeleteShift(int? id)
        {
            try
            {
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                ShiftDetail shiftDetail = _db.ShiftDetails.FirstOrDefault(s => s.ShiftDetailId == id);
                if (shiftDetail == null)
                {
                    return false;
                }
                shiftDetail.IsDeleted = new BitArray(new[] { true });
                shiftDetail.ModifiedBy = cookieModel.aspId;
                shiftDetail.ModifiedDate = DateTime.Now;

                _db.ShiftDetails.Update(shiftDetail);
                _db.SaveChanges();
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public MDOnCallViewModel GetMdOnCallDetails(int regionid = -1)
        {

            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Provider",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            List<Region> regions = _db.Regions.ToList();

            var currentDate = DateTime.Now.Date;
            var currentTime = DateTime.Now.TimeOfDay;

            var notactive = from p in _db.Physicians
                            join pr in _db.PhysicianRegions on p.PhysicianId equals pr.PhysicianId
                            where !_db.Shifts.Any(s => s.PhysicianId == p.PhysicianId &&
                                                             _db.ShiftDetails.Any(sd => s.ShiftId == sd.ShiftId &&
                                                                                           sd.ShiftDate.Date == currentDate &&
                                                                                           new TimeOnly(currentTime.Hours, currentTime.Minutes, currentTime.Seconds) >= sd.StartTime &&
                                                                                           new TimeOnly(currentTime.Hours, currentTime.Minutes, currentTime.Seconds) <= sd.EndTime && sd.Status == 1)) &&
                                  p.IsDeleted == new BitArray(new[] { false })
                            select new MDOnCallPhysicians
                            {
                                PhysicianId = p.PhysicianId,
                                Name = p.FirstName + ", " + p.LastName.ToUpper()[0],
                                Photo = p.Photo,
                                RegionId = pr.RegionId,
                                Email = p.Email
                            };

            if (regionid != -1)
            {
                notactive = notactive.Where(r => r.RegionId == regionid);
            }
            else
            {
                notactive = notactive.GroupBy(r => r.PhysicianId).Select(r => r.FirstOrDefault());
            }

            var active = from p in _db.Physicians
                         join pr in _db.PhysicianRegions on p.PhysicianId equals pr.PhysicianId
                         where _db.Shifts.Any(s => s.PhysicianId == p.PhysicianId &&
                                                          _db.ShiftDetails.Any(sd => s.ShiftId == sd.ShiftId &&
                                                                                        sd.ShiftDate.Date == currentDate &&
                                                                                        new TimeOnly(currentTime.Hours, currentTime.Minutes, currentTime.Seconds) >= sd.StartTime &&
                                                                                        new TimeOnly(currentTime.Hours, currentTime.Minutes, currentTime.Seconds) <= sd.EndTime && sd.Status == 1)) &&
                               p.IsDeleted == new BitArray(new[] { false })
                         select new MDOnCallPhysicians
                         {
                             PhysicianId = p.PhysicianId,
                             Name = p.FirstName + ", " + p.LastName.ToUpper()[0],
                             Photo = p.Photo,
                             RegionId = pr.RegionId,
                             Email = p.Email
                         };

            if (regionid != -1)
            {
                active = active.Where(r => r.RegionId == regionid);
            }
            else
            {
                active = active.GroupBy(r => r.PhysicianId).Select(r => r.FirstOrDefault());
            }

            MDOnCallViewModel mDOnCallViewModel = new MDOnCallViewModel
            {
                notActivePhysicians = notactive.ToList(),
                activePhysicians = active.ToList(),
                adminNavbarViewModel = adminNavbarViewModel,
                regions = regions
            };

            return mDOnCallViewModel;
        }

        public ShiftsForReviewViewModel GetRequestedShifts(int regionid = -1, int page = 1, int pageSize = 10)
        {

            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Provider",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            List<Region> regions = _db.Regions.ToList();

            var result = from s in _db.Shifts
                         join sd in _db.ShiftDetails on s.ShiftId equals sd.ShiftId
                         join p in _db.Physicians on s.PhysicianId equals p.PhysicianId
                         join r in _db.Regions on sd.RegionId equals r.RegionId
                         where sd.Status == 0 && sd.IsDeleted == new BitArray(new[] { false })
                         select new RequestedShifts
                         {
                             FirstName = p.FirstName,
                             LastName = p.LastName,
                             ShiftDate = sd.ShiftDate,
                             StartTime = sd.StartTime,
                             EndTime = sd.EndTime,
                             RegionId = r.RegionId,
                             RegionName = r.Name,
                             ShiftDetailId = sd.ShiftDetailId
                         };

            if (regionid != -1)
            {
                result = result.Where(r => r.RegionId == regionid);
            }

            ShiftsForReviewViewModel shiftsForReviewViewModel = new ShiftsForReviewViewModel
            {
                adminNavbarViewModel = adminNavbarViewModel,
                requestedShifts = result.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                regions = regions,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = result.Count(),
                TotalPages = (int)Math.Ceiling((double)result.Count() / pageSize)
            };

            return shiftsForReviewViewModel;

        }

        public bool AprooveShifts(ShiftsForReviewViewModel shiftsForReviewViewModel)
        {
            try
            {

                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                string[] shiftDetailIds = shiftsForReviewViewModel.ShiftDetailIds.Split(",");
                for (var i = 0; i < shiftDetailIds.Length - 1; ++i)
                {
                    ShiftDetail shiftDetail = _db.ShiftDetails.FirstOrDefault(s => s.ShiftDetailId == int.Parse(shiftDetailIds[i]));
                    if (shiftDetail == null)
                    {
                        return false;
                    }
                    shiftDetail.Status = 1;
                    shiftDetail.ModifiedDate = DateTime.Now;
                    shiftDetail.ModifiedBy = cookieModel.aspId;
                    _db.ShiftDetails.Update(shiftDetail);
                }
                _db.SaveChanges();
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public bool DeleteShifts(ShiftsForReviewViewModel shiftsForReviewViewModel)
        {
            try
            {

                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                string[] shiftDetailIds = shiftsForReviewViewModel.ShiftDetailIds.Split(",");
                for (var i = 0; i < shiftDetailIds.Length - 1; ++i)
                {
                    ShiftDetail shiftDetail = _db.ShiftDetails.FirstOrDefault(s => s.ShiftDetailId == int.Parse(shiftDetailIds[i]));
                    if (shiftDetail == null)
                    {
                        return false;
                    }
                    shiftDetail.IsDeleted = new BitArray(new[] { true });
                    shiftDetail.ModifiedDate = DateTime.Now;
                    shiftDetail.ModifiedBy = cookieModel.aspId;
                    _db.ShiftDetails.Update(shiftDetail);
                }
                _db.SaveChanges();
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public bool ToggleShiftStatus(int? id)
        {
            try
            {
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                ShiftDetail shiftDetail = _db.ShiftDetails.FirstOrDefault(s => s.ShiftDetailId == id);
                if (shiftDetail == null)
                {
                    return false;
                }
                int status = shiftDetail.Status == 0 ? 1 : 0;
                shiftDetail.Status = (short)status;
                shiftDetail.ModifiedDate = DateTime.Now;
                shiftDetail.ModifiedBy = cookieModel.aspId;
                _db.ShiftDetails.Update(shiftDetail);

                _db.SaveChanges();
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public async Task<bool> RequestDTYSupport(AdminDashboardViewModel adminDashboardViewModel)
        {
            try
            {
                var currentDate = DateTime.Now.Date;
                var currentTime = DateTime.Now.TimeOfDay;

                var notactive = from p in _db.Physicians
                                where !_db.Shifts.Any(s => s.PhysicianId == p.PhysicianId &&
                                                                 _db.ShiftDetails.Any(sd => s.ShiftId == sd.ShiftId &&
                                                                                               sd.ShiftDate.Date == currentDate &&
                                                                                               new TimeOnly(currentTime.Hours, currentTime.Minutes, currentTime.Seconds) >= sd.StartTime &&
                                                                                               new TimeOnly(currentTime.Hours, currentTime.Minutes, currentTime.Seconds) <= sd.EndTime && sd.Status == 1)) &&
                                      p.IsDeleted == new BitArray(new[] { false })
                                select new MDOnCallPhysicians
                                {
                                    PhysicianId = p.PhysicianId,
                                    Name = p.FirstName + ", " + p.LastName.ToUpper()[0],
                                    Photo = p.Photo,
                                    Email = p.Email
                                };

                int retryCount = 1;
                bool success = false;

                for (var i = 0; i < notactive.Count(); ++i)
                {
                    while (retryCount <= 3 && !success) // Set retry limit
                    {

                        string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                        string senderPassword = "shahkandarp2430"; // Replace with your actual password (store securely)
                        var platformTitle = "HalloDoc";
                        var subject = "Request Support - HalloDoc";
                        var body = $"Hello {notactive.ToList()[i].Name},<br />{adminDashboardViewModel.BlockReason}<br /><br />Regards,<br/>{platformTitle}<br/>";
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

                            mailMessage.To.Add(notactive.ToList()[i].Email);


                            await client.SendMailAsync(mailMessage);


                            success = true;
                            LogEmail(body, subject, notactive.ToList()[i].Email, null, -1, -1, notactive.ToList()[i].PhysicianId, true, retryCount, 3, 8);
                            break;
                        }
                        catch (Exception ex)
                        {

                            if (retryCount >= 3)
                            {
                                LogEmail(body, subject, notactive.ToList()[i].Email, null, -1, -1, notactive.ToList()[i].PhysicianId, false, retryCount, 3, 8);
                            }
                            retryCount++;
                        }
                    }
                    retryCount = 1;
                    success = false;
                }

                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public PayRateViewModel GetPayRate(int id)
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            Physician physician = _db.Physicians.FirstOrDefault(p => p.PhysicianId == id);
            if (physician == null)
            {
                return null;
            }

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "Provider",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            Payrate payrate = _db.Payrates.FirstOrDefault(p => p.PhysicianId == id);

            PayRateViewModel payRateViewModel = new PayRateViewModel
            {
                adminNavbarViewModel = adminNavbarViewModel,
                NightShiftWeekend = payrate?.NightShiftWeekend ?? 0,
                Shift = payrate?.Shift ?? 0,
                HouseCalls_Night_Weekend = payrate?.HousecallNightWeekend ?? 0,
                PhoneConsult_Night_Weekend = payrate?.PhoneconsultNightWeekend ?? 0,
                BatchTesting = payrate?.BatchTesting ?? 0,
                PhoneConsult = payrate?.Phoneconsult ?? 0,
                HouseCall = payrate?.Housecall ?? 0,
                PhysicianId = id
            };
            return payRateViewModel;

        }

        public bool CheckUserRole(string email)
        {
            try
            {
                var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == email);
                if(user!=null)
                {
                    AspNetUserRole aspNetUserRole = _db.AspNetUserRoles.FirstOrDefault(a => a.UserId == user.Id);
                    if ((aspNetUserRole == null) || (aspNetUserRole != null && aspNetUserRole.RoleId != 1))
                    {
                        return false;
                    }
                }
                return true; 
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public bool UpdatePayRate(PayRateViewModel payRateViewModel)
        {
            try
            {
                Payrate payrate = _db.Payrates.FirstOrDefault(p => p.PhysicianId == payRateViewModel.PhysicianId);
                if (payrate == null)
                {
                    Payrate payrate1 = new Payrate
                    {
                        PhysicianId = (int)payRateViewModel.PhysicianId,
                        NightShiftWeekend = payRateViewModel?.NightShiftWeekend ?? 0,
                        Shift = payRateViewModel?.Shift ?? 0,
                        HousecallNightWeekend = payRateViewModel?.HouseCalls_Night_Weekend ?? 0,
                        Phoneconsult = payRateViewModel?.PhoneConsult ?? 0,
                        PhoneconsultNightWeekend = payRateViewModel?.PhoneConsult_Night_Weekend ?? 0,
                        BatchTesting = payRateViewModel?.BatchTesting ?? 0,
                        Housecall = payRateViewModel?.HouseCall ?? 0,
                        CreatedDate = DateTime.Now
                    };
                    _db.Payrates.Add(payrate1);
                }
                else
                {
                    payrate.Housecall = payRateViewModel?.HouseCall ?? payrate.Housecall;
                    payrate.BatchTesting = payRateViewModel?.BatchTesting ?? payrate.BatchTesting;
                    payrate.PhoneconsultNightWeekend = payRateViewModel?.PhoneConsult_Night_Weekend ?? payrate.PhoneconsultNightWeekend;
                    payrate.Phoneconsult = payRateViewModel?.PhoneConsult ?? payrate.Phoneconsult;
                    payrate.HousecallNightWeekend = payRateViewModel?.HouseCalls_Night_Weekend ?? payrate.HousecallNightWeekend;
                    payrate.Shift = payRateViewModel?.Shift ?? payrate.Shift;
                    payrate.NightShiftWeekend = payRateViewModel?.NightShiftWeekend ?? payrate.NightShiftWeekend;
                    payrate.ModifiedDate = DateTime.Now;
                    _db.Payrates.Update(payrate);
                }
                _db.SaveChanges();
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public bool ApproveTimesheet(decimal totalamount, decimal bonusamount, string desc, int id)
        {
            try
            {
                Timesheet timesheet = _db.Timesheets.FirstOrDefault(t=>t.TimesheetId == id);
                if(timesheet == null)
                {
                    return false;
                }
                timesheet.TotalAmount = totalamount;
                timesheet.BonusAmount = bonusamount;
                timesheet.AdminDescription = desc;
                timesheet.Status = "Accepted";
                timesheet.ModifiedDate = DateTime.Now;
                _db.Timesheets.Update(timesheet);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

    }
}
