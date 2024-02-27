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

namespace HalloDoc.Repository.Repository
{

    public class Admin:IAdmin
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _context;
        public Admin(ApplicationDbContext db, IHttpContextAccessor context)
        {
            _db = db;
            _context = context;
        }

        AdminDashboardViewModel IAdmin.adminDashboardContent(string status, string? search, string? requestor, int? region)
        {
            Expression<Func<Request, bool>> exp;
            if(status == "New")
            {
                exp = r => r.Status == 1;
            }
            else if(status=="Penidng")
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

            IQueryable<Request> _query = _db.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(exp).OrderByDescending(e => e.CreatedDate);

            var count_new = _db.Requests.Count(r => r.Status == 1);
            var count_pending = _db.Requests.Count(r => r.Status == 2);
            var count_active = _db.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _db.Requests.Count(r => r.Status == 5);
            var count_toclose = _db.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _db.Requests.Count(r => r.Status == 9);
            var casetag = _db.CaseTags.ToList();

            if (search != null)
            {
                _query = _query.Where(r => r.RequestClient.FirstName.Contains(search) || r.RequestClient.LastName.Contains(search));
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

            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                requests = _query.ToList(),
                regions = _db.Regions.ToList(),
                status = status,
                caseTags = casetag,
            };
            return adminDashboardViewModel;
        }

        MemoryStream IAdmin.exportAll()
        {
            try
            {
                List<Request> data = new List<Request>();
                data = _db.Requests.Include(r => r.RequestClient).Include(r => r.Physician).ToList();
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

        MemoryStream IAdmin.export(AdminDashboardViewModel model)
        {
            try
            {
                List<Request> data = new List<Request>();
                data = model.requests;
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Export All");

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
                            notes = stat.Notes ?? "";
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
                        worksheet.Cell(row, count++).Value = "Admin transferred to Dr.AGOLA on 10\\10\\2023 at 4:11:38 AM:test";
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
                caseTags = caseTags
            };
            return viewCaseViewModel;
        }

        public bool viewCase(ViewCaseViewModel model)
        {
            
            try
            {
                RequestClient requestClient = _db.RequestClients.FirstOrDefault(r=>r.RequestClientId == model.RequestClientId);
                requestClient.FirstName = model.FirstName;
                requestClient.LastName = model.LastName;
                requestClient.PhoneNumber = model.PhoneNumber;
                requestClient.Email = model.Email;
                requestClient.StrMonth = model.DateOfBirth.Month.ToString();
                requestClient.IntYear = model.DateOfBirth.Year;
                requestClient.IntDate = model.DateOfBirth.Day;
                _db.RequestClients.Update(requestClient);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

         bool IAdmin.cancelRequest(int id,string notes,string request)
        {
            try
            {
                Request req = _db.Requests.FirstOrDefault(r => r.RequestId == id);
                req.CaseTag = request;
                req.Status = 6;
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

        bool IAdmin.sendLink(AdminDashboardViewModel dashboardViewModel)
        {
            try
            {
                string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                string senderPassword = "shahkandarp2430";
                var platformTitle = "HalloDoc";
                var inviteLink = "https://localhost:7088/Patient/SubmitRequest";
                var subject = "Register - HalloDoc";
                var body = $"Hello {dashboardViewModel.Mail_FirstName} {dashboardViewModel.Mail_LastName},<br />Click the following link to create new request in our portal,<br /><br /><a href='{inviteLink}'>Create Request</a><br /><br />Regards,<br/>{platformTitle}<br/>";

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

                client.SendMailAsync(mailMessage);
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        bool IAdmin.verifyRegion(string region)
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

        bool IAdmin.verifyBlock(string Email)
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

        bool IAdmin.createRequest(PatientRequestViewModel modal)
        {
            try
            {
                var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == modal.Email);

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
                        IsUrgentEmailSent = new BitArray(1),
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

                    RequestNote requestNote = new RequestNote
                    {
                        RequestId = req.RequestId,
                        AdminNotes = modal.Admin_notes,
                        CreatedDate = DateTime.Now,
                        CreatedBy = 2,
                    };

                    _db.RequestNotes.Add(requestNote);
                    _db.SaveChanges();

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
                            CreatedDate = DateTime.Now
                        };
                        _db.RequestWiseFiles.Add(rfile);
                        _db.SaveChanges();
                    }

                    RequestStatusLog rst = new RequestStatusLog
                    {
                        RequestId = req.RequestId,
                        Status = 1,
                        CreatedDate = DateTime.Now
                    };

                    _db.RequestStatusLogs.Add(rst);
                    _db.SaveChanges();

                    RequestNote requestNote = new RequestNote
                    {
                        RequestId = req.RequestId,
                        AdminNotes = modal.Admin_notes,
                        CreatedDate = DateTime.Now,
                        CreatedBy = 2,
                    };

                    _db.RequestNotes.Add(requestNote);
                    _db.SaveChanges();

                    string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                    string senderPassword = "shahkandarp2430";
                    var platformTitle = "HalloDoc";
                    var inviteLink = $"https://localhost:7088/Patient/Register/{aspuser.Id}";
                    var subject = "Register - HalloDoc";
                    var body = $"Hello <br />Click the following link to register to our portal,<br /><br /><a href='{inviteLink}'>Register</a><br /><br />Regards,<br/>{platformTitle}<br/>";

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

                    client.SendMailAsync(mailMessage);

                    return true;

                }
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        ViewNotesViewModel IAdmin.viewNotes(int id)
        {
            RequestStatusLog patientcancel = _db.RequestStatusLogs.FirstOrDefault(r=>r.RequestId == id && r.Status == 7);
            RequestStatusLog admincancel = _db.RequestStatusLogs.FirstOrDefault(r => r.RequestId == id && r.Status == 6);
            List<RequestStatusLog> transfernotes = _db.RequestStatusLogs.Where(r => r.RequestId == id && r.Status == 2).ToList();
            RequestNote requestNotes = _db.RequestNotes.FirstOrDefault(r=>r.RequestId == id);
            ViewNotesViewModel viewNotesViewModel = new ViewNotesViewModel
            {
                RequestId = id,
                Admin_Note = requestNotes?.AdminNotes ?? "-",
                Physician_Note = requestNotes?.PhysicianNotes ?? "-",
                Admin_Cancellation_Note = admincancel?.Notes,
                Cancellation_Note = patientcancel?.Notes,
                Transfer_Notes = transfernotes
            };
            return viewNotesViewModel;
        }

        bool IAdmin.updateAdminNotes(ViewNotesViewModel viewNotesViewModel)
        {
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
                        CreatedBy = 2,
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

    }
}
