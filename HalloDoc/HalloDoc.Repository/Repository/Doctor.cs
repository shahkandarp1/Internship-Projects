using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using HalloDoc.Repository.Interface;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using static HalloDoc.ViewModels.Enums;

namespace HalloDoc.Repository.Repository
{
    public class Doctor:IDoctor
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _context;
        private readonly IJwtService _jwt;
        private readonly IConfiguration _configuration;
        public Doctor(ApplicationDbContext db, IHttpContextAccessor context, IJwtService jwt, IConfiguration configuration)
        {
            _db = db;
            _context = context;
            _jwt = jwt;
            _configuration = configuration;
        }

        public void LogEmail(string emailTemplate, string subject, string userEmail, string confirmation_no, int request_id, int admin_id, int physician_id, bool success, int retryCount, int role_id,int action)
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

        public void LogSMS(string SmsTemplate, string userPhone, string confirmation_no, int request_id, int admin_id, int physician_id, bool success, int retryCount, int role_id,int action)
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

        public bool AcceptCase(int? id)
        {
            try
            {
                Request request = _db.Requests.FirstOrDefault(r => r.RequestId == id);
                if (request == null)
                {
                    return false;
                }
                request.Status = 2;
                request.ModifiedDate = DateTime.Now;
                request.AcceptedDate = DateTime.Now;
                _db.Requests.Update(request);

                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                RequestStatusLog requestStatusLog = new RequestStatusLog
                {
                    RequestId = (int)id,
                    Status = 2,
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

        public async Task<bool> RequestAdmin(PhysicianAccountViewModel physicianAccountViewModel)
        {
            try
            {
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                AspNetUser aspNetUser = _db.AspNetUsers.FirstOrDefault(r=>r.Id == cookieModel.aspId);

                List<HalloDoc.Admin> admin = _db.Admins.Where(a => a.IsDeleted == false).ToList();
                var retryCount = 1;
                var success = false;
                for (var i = 0; i < admin.Count; i++)
                {
                    while (retryCount <= 3 && !success) // Set retry limit
                    {

                        string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                        string senderPassword = "shahkandarp2430"; // Replace with your actual password (store securely)
                        var platformTitle = "HalloDoc";
                        var subject = "Request Profile Edit - HalloDoc";
                        var body = $"Hello {admin[i].FirstName} {admin[i].LastName},<br />Doctor's Name: {cookieModel.name}<br/>Doctor's UserName: {aspNetUser.UserName}<br/>Doctor's Message: {physicianAccountViewModel.editReason}<br /><br />Regards,<br/>{platformTitle}<br/>";
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

                            mailMessage.To.Add(admin[i].Email);


                            await client.SendMailAsync(mailMessage);


                            success = true;
                            LogEmail(body, subject, admin[i].Email, null, -1, admin[i].AdminId, -1, true, retryCount, 2,10);
                            break;
                        }
                        catch (Exception ex)
                        {

                            if (retryCount >= 3)
                            {
                                LogEmail(body, subject, admin[i].Email, null, -1, admin[i].AdminId, -1, false, retryCount, 2,10);
                            }
                            retryCount++;
                        }
                    }
                    retryCount = 1;
                    success = false;

                    while (retryCount <= 3 && !success) // Set retry limit
                    {
                        var platformTitle = "HalloDoc";

                        var accountSid = _configuration["Twilio:accountSid"];
                        var authToken = _configuration["Twilio:authToken"];
                        var twilionumber = _configuration["Twilio:twilioNumber"];

                        var messageBody = $"Hello {admin[i].FirstName} {admin[i].LastName},\nDoctor's Name: {cookieModel.name}\nDoctor's UserName: {aspNetUser.UserName}\nDoctor's Message: {physicianAccountViewModel.editReason}\n\nRegards,\n{platformTitle}";
                        try
                        {

                            TwilioClient.Init(accountSid, authToken);

                            var message = MessageResource.Create(
                                from: new Twilio.Types.PhoneNumber(twilionumber),
                                body: messageBody,
                                to: new Twilio.Types.PhoneNumber(admin[i].Mobile[0] == '+' && admin[i].Mobile[1] == '9' && admin[i].Mobile[2] == '1' ? admin[i].Mobile : "+91" + admin[i].Mobile)
                            );


                            success = true;
                            LogSMS(messageBody, admin[i].Mobile, null, -1, admin[i].AdminId, -1, true, retryCount, 2,4);
                            break;
                        }
                        catch (Exception ex)
                        {

                            if (retryCount >= 3)
                            {
                                LogSMS(messageBody, admin[i].Mobile, null, -1, admin[i].AdminId, -1, false, retryCount, 2,4);
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

        public bool TypeOfCare(AdminDashboardViewModel adminDashboardViewModel)
        {
            try
            {
                var status = 0;
                short calltype = 0;
                if (adminDashboardViewModel.typeOfCare == "Consult")
                {
                    status = 5;
                    calltype = 2;
                }
                else
                {
                    status = 4;
                    calltype = 1;
                }

                Request request = _db.Requests.FirstOrDefault(r => r.RequestId == adminDashboardViewModel.RequestId);
                if (request == null)
                {
                    return false;
                }

                request.Status = (short)status;
                request.ModifiedDate = DateTime.Now;
                request.CallType = calltype;

                _db.Requests.Update(request);

                RequestStatusLog requestStatusLog = new RequestStatusLog
                {
                    RequestId = request.RequestId,
                    Status = (short)status,
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

        public bool HouseCall(int? id)
        {
            try
            {
                Request request = _db.Requests.FirstOrDefault(r => r.RequestId == id);
                if (request == null)
                {
                    return false;
                }

                request.Status = 5;
                request.ModifiedDate = DateTime.Now;

                _db.Requests.Update(request);

                RequestStatusLog requestStatusLog = new RequestStatusLog
                {
                    RequestId = request.RequestId,
                    Status = 5,
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

        public ConcludeCareViewModel GetConcludeCare(int id)
        {
            var request = _db.Requests.Include(r => r.RequestClient).Include(r=>r.RequestNotes).FirstOrDefault(u => u.RequestId == id);
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

            ConcludeCareViewModel concludeCareViewModel = new ConcludeCareViewModel()
            {
                RequestId = id,
                patient_name = string.Concat(request.RequestClient.FirstName, ' ', request.RequestClient.LastName),
                confirmation_number = request.ConfirmationNumber,
                requestWiseFiles = documents,
                uploader_name = string.Concat(request.FirstName, ' ', request.LastName),
                adminNavbarViewModel = adminNavbarViewModel,
                ProviderNotes = request.RequestNotes.ToList().Count > 0 ? request.RequestNotes.ToList()[0].PhysicianNotes : null
            };
            return concludeCareViewModel;
        }

        public int ConcludeCare(ConcludeCareViewModel concludeCareViewModel)
        {
            try
            {
                Request request = _db.Requests.Include(r => r.EncounterForms).FirstOrDefault(r => r.RequestId == concludeCareViewModel.RequestId);
                if (request == null)
                {
                    return 3;
                }
                if (request.EncounterForms.ToList().Count == 0 || request.EncounterForms.ToList()[0].IsFinalized[0] == false)
                {
                    return 2;
                }
                request.Status = 8;
                request.ModifiedDate = DateTime.Now;
                request.CompletedByPhysician = new BitArray(new[] { true });
                _db.Requests.Update(request);

                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                RequestStatusLog requestStatusLog = new RequestStatusLog
                {
                    RequestId = request.RequestId,
                    Status = 8,
                    CreatedDate = DateTime.Now,
                };
                _db.RequestStatusLogs.Add(requestStatusLog);

                RequestNote requestNote = _db.RequestNotes.FirstOrDefault(r=>r.RequestId == request.RequestId);
                if(requestNote == null)
                {
                    RequestNote requestNote1 = new RequestNote
                    {
                        RequestId = request.RequestId,
                        CreatedDate = DateTime.Now,
                        CreatedBy = cookieModel.aspId,
                        PhysicianNotes = concludeCareViewModel.ProviderNotes
                    };
                    _db.RequestNotes.Add(requestNote1);
                }
                else
                {
                    requestNote.PhysicianNotes = concludeCareViewModel.ProviderNotes;
                    requestNote.ModifiedBy = cookieModel.aspId;
                    requestNote.ModifiedDate = DateTime.Now;
                    _db.RequestNotes.Update(requestNote);
                }
                _db.SaveChanges();
                return 1;
            }
            catch(Exception ex)
            {
                return 3;
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
                request.PhysicianId = null;
                _db.Requests.Update(request);

                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                Physician physician = _db.Physicians.FirstOrDefault(p => p.PhysicianId == cookieModel.userId);
                if (physician == null)
                {
                    return false;
                }
                RequestStatusLog requestStatusLog = new RequestStatusLog
                {
                    RequestId = (int)adminDashboardViewModel.RequestId,
                    Status = 1,
                    Notes = $"Dr. {physician.FirstName} transferred to Admin on {DateTime.Now.ToString("MMMM dd,yyyy")} at {string.Format("{0:hh:mm:ss tt}", DateTime.Now)} : {adminDashboardViewModel.BlockReason}",
                    CreatedDate = DateTime.Now,
                    TransToAdmin = new BitArray(new[] { true }),
                    PhysicianId = adminDashboardViewModel.PhysicianId,
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

        public bool UpdatePhysicianLatitudeLongitude(decimal lat, decimal lng, string address)
        {
            try
            {
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                PhysicianLocation physicianLocation = _db.PhysicianLocations.FirstOrDefault(r=>r.PhysicianId == cookieModel.userId);
                physicianLocation.Longitude = lng;
                physicianLocation.Latitude = lat;
                physicianLocation.Address = address;

                _db.PhysicianLocations.Update(physicianLocation);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public PhysicianInvoicingViewModel GetPhysicianInvoicingDetails(DateTime? startdate, DateTime? enddate)
        {

            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "DoctorInvoice",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            PhysicianInvoicingViewModel physicianInvoicingViewModel = new PhysicianInvoicingViewModel
            {
                adminNavbarViewModel = adminNavbarViewModel,
            };

            if (startdate != null)
            {
                physicianInvoicingViewModel.timesheetDetails = _db.Timesheets.Include(t => t.TimesheetDetails).FirstOrDefault(t => t.Startdate.Value.Date == startdate.Value.Date && t.Enddate.Value.Date == enddate.Value.Date && t.PhysicianId == cookieModel.userId);
            }
            if (enddate != null)
            {
                physicianInvoicingViewModel.timesheetReimbursement = _db.Timesheets.Include(t => t.TimesheetReimbursements.Where(tr => tr.IsDeleted == false)).FirstOrDefault(t => t.Startdate.Value.Date == startdate.Value.Date && t.Enddate.Value.Date == enddate.Value.Date && t.PhysicianId == cookieModel.userId);
            }

            return physicianInvoicingViewModel;
        }

        public PhysicianTimesheetViewModel GetTimesheetDetails(DateTime? startdate, DateTime? enddate)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            AdminNavbarViewModel adminNavbarViewModel = new AdminNavbarViewModel
            {
                Name = cookieModel.name,
                curr_active = "DoctorInvoice",
                menus = cookieModel.menus,
                role = cookieModel.role
            };

            List<TimeSheetViewModel> timesheetDetails = new List<TimeSheetViewModel>();
            List<TimeSheetReimbursementViewModel> timeSheetReimbursementViewModels = new List<TimeSheetReimbursementViewModel>();

            Timesheet timesheet = _db.Timesheets.Include(t => t.TimesheetDetails).Include(t => t.TimesheetReimbursements.Where(tr => tr.IsDeleted == false)).FirstOrDefault(t => t.Startdate.Value.Date == startdate.Value.Date && t.Enddate.Value.Date == enddate.Value.Date && t.PhysicianId == cookieModel.userId);
            if(timesheet!=null && timesheet?.TimesheetDetails.Count != 0)
            {
                for(var i=0;i< timesheet.TimesheetDetails.ToList().Count;++i)
                {
                    timesheetDetails.Add(new TimeSheetViewModel()
                    {
                        Shiftdate = timesheet.TimesheetDetails.ToList()[i].Shiftdate,
                        ShiftHours = timesheet.TimesheetDetails.ToList()[i].ShiftHours,
                        Housecall = timesheet.TimesheetDetails.ToList()[i].Housecall,
                        PhoneConsult = timesheet.TimesheetDetails.ToList()[i].PhoneConsult,
                        IsWeekend = timesheet.TimesheetDetails.ToList()[i].IsWeekend[0],
                        TimesheetId = timesheet.TimesheetDetails.ToList()[i].TimesheetId,
                        TimesheetDetailId = timesheet.TimesheetDetails.ToList()[i].TimesheetDetailId,
                    });
                }                
            }
            else
            {
                for(var i= startdate.Value.Date;i<= enddate.Value.Date;i= i.AddDays(1))
                {
                    var hours = (from s in _db.Shifts
                                              join sd in _db.ShiftDetails on s.ShiftId equals sd.ShiftId
                                              where s.PhysicianId == cookieModel.userId && sd.ShiftDate == i && sd.IsDeleted == new BitArray(new[] { false })
                                              select Math.Ceiling((sd.EndTime - sd.StartTime).TotalSeconds / 3600)).Sum();

                    timesheetDetails.Add(new TimeSheetViewModel()
                    {
                        Shiftdate = i,
                        ShiftHours = (int)hours,
                        IsWeekend = false
                    });

                }

            }

            for (var i = startdate.Value.Date; i <= enddate.Value.Date; i = i.AddDays(1))
            {
                var timesheetreimbursement = timesheet?.TimesheetReimbursements.FirstOrDefault(t => t.Date.Value.Date == i);

                if (timesheetreimbursement != null)
                {
                    timeSheetReimbursementViewModels.Add(new TimeSheetReimbursementViewModel()
                    {
                        TimesheetId = timesheetreimbursement.TimesheetId,
                        TimesheetReimbursementId = timesheetreimbursement.TimesheetReimbursementId,
                        IsDeleted = timesheetreimbursement.IsDeleted,
                        Item = timesheetreimbursement.Item,
                        Amount = timesheetreimbursement.Amount,
                        Bill = timesheetreimbursement.Bill,
                        Date = timesheetreimbursement.Date,
                    });
                }
                else
                {
                    timeSheetReimbursementViewModels.Add(new TimeSheetReimbursementViewModel()
                    {
                        IsDeleted = false,
                        Amount = 0,
                        Date = i,
                        TimesheetReimbursementId = 0
                    });
                }
            }

            PhysicianTimesheetViewModel physicianTimesheetViewModel = new PhysicianTimesheetViewModel
            {
                adminNavbarViewModel = adminNavbarViewModel,
                timesheetDetails = timesheetDetails,
                startDate = startdate,
                endDate = enddate,
                timeSheetReimbursementViewModels = timeSheetReimbursementViewModels
            };
            return physicianTimesheetViewModel;
        }

        public bool UpdateTimeSheet(PhysicianTimesheetViewModel physicianTimesheetViewModel)
        {
            try
            {
                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                Timesheet timesheet = _db.Timesheets.Include(t => t.TimesheetDetails).Include(t => t.TimesheetReimbursements.Where(tr => tr.IsDeleted == false)).FirstOrDefault(t => t.Startdate.Value.Date == physicianTimesheetViewModel.startDate.Value.Date && t.Enddate.Value.Date == physicianTimesheetViewModel.endDate.Value.Date && t.PhysicianId == cookieModel.userId);

                if (physicianTimesheetViewModel.timesheetDetails[0].TimesheetDetailId <= 0)
                {
                    if (timesheet != null)
                    {
                        for (var i = 0; i < physicianTimesheetViewModel.timesheetDetails.Count; ++i)
                        {
                            _db.TimesheetDetails.Add(new TimesheetDetail()
                            {
                                Shiftdate = physicianTimesheetViewModel.timesheetDetails[i].Shiftdate,
                                ShiftHours = physicianTimesheetViewModel.timesheetDetails[i].ShiftHours,
                                Housecall = physicianTimesheetViewModel.timesheetDetails[i].Housecall,
                                PhoneConsult = physicianTimesheetViewModel.timesheetDetails[i].PhoneConsult,
                                IsWeekend = new BitArray(new[] { physicianTimesheetViewModel.timesheetDetails[i]?.IsWeekend ?? false }),
                                TimesheetId = timesheet.TimesheetId
                            });
                        }
                    }
                    else
                    {
                        Timesheet timesheet1 = new Timesheet
                        {
                            PhysicianId = cookieModel.userId,
                            Startdate = physicianTimesheetViewModel.startDate,
                            Enddate = physicianTimesheetViewModel.endDate,
                            Status = "Not Accepted",
                            IsFinalized = new BitArray(new[] { false })
                        };
                        _db.Timesheets.Add(timesheet1);
                        for (var i = 0; i < physicianTimesheetViewModel.timesheetDetails.Count; ++i)
                        {
                            _db.TimesheetDetails.Add(new TimesheetDetail()
                            {
                                Shiftdate = physicianTimesheetViewModel.timesheetDetails[i].Shiftdate,
                                ShiftHours = physicianTimesheetViewModel.timesheetDetails[i].ShiftHours,
                                Housecall = physicianTimesheetViewModel.timesheetDetails[i].Housecall,
                                PhoneConsult = physicianTimesheetViewModel.timesheetDetails[i].PhoneConsult,
                                IsWeekend = new BitArray(new[] { physicianTimesheetViewModel.timesheetDetails[i]?.IsWeekend ?? false }),
                                Timesheet = timesheet1
                            });
                        }
                    }
                    
                }
                else
                {
                    for (var i = 0; i < physicianTimesheetViewModel.timesheetDetails.Count; ++i)
                    {
                        TimesheetDetail timesheetDetail = _db.TimesheetDetails.FirstOrDefault(t=>t.TimesheetDetailId == physicianTimesheetViewModel.timesheetDetails[i].TimesheetDetailId);
                        timesheetDetail.Shiftdate = physicianTimesheetViewModel.timesheetDetails[i].Shiftdate;
                        timesheetDetail.ShiftHours = physicianTimesheetViewModel.timesheetDetails[i].ShiftHours;
                        timesheetDetail.Housecall = physicianTimesheetViewModel.timesheetDetails[i].Housecall;
                        timesheetDetail.PhoneConsult = physicianTimesheetViewModel.timesheetDetails[i].PhoneConsult;
                        timesheetDetail.IsWeekend = new BitArray(new[] { physicianTimesheetViewModel.timesheetDetails[i]?.IsWeekend ?? false });

                        _db.TimesheetDetails.Update(timesheetDetail);
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

        public async Task<bool> UpdateTimeSheetReimbursement(IFormFile file, DateTime? date, int? id, string? item, int? amount, DateTime? startdate, DateTime? enddate)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", file.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                var requestt = _context.HttpContext.Request;
                var token = requestt.Cookies["jwt"];
                CookieModel cookieModel = _jwt.GetDetails(token);

                if (id <= 0)
                {
                    Timesheet timesheet = _db.Timesheets.Include(t => t.TimesheetDetails).Include(t => t.TimesheetReimbursements.Where(tr => tr.IsDeleted == false)).FirstOrDefault(t => t.Startdate.Value.Date == startdate.Value.Date && t.Enddate.Value.Date == enddate.Value.Date && t.PhysicianId == cookieModel.userId);
                    if(timesheet != null)
                    {
                        TimesheetReimbursement timesheetReimbursement = new TimesheetReimbursement
                        {
                            Item = item,
                            Amount = (int)amount,
                            Bill = file!=null ? file.FileName : null,
                            TimesheetId = timesheet.TimesheetId,
                            IsDeleted = false,
                            Date = date
                        };
                        _db.TimesheetReimbursements.Add(timesheetReimbursement);
                    }
                    else
                    {
                        Timesheet timesheet1 = new Timesheet
                        {
                            PhysicianId = cookieModel.userId,
                            Startdate = startdate,
                            Enddate = enddate,
                            Status = "Not Accepted",
                            IsFinalized = new BitArray(new[] { false })
                        };
                        _db.Timesheets.Add(timesheet1);

                        TimesheetReimbursement timesheetReimbursement = new TimesheetReimbursement
                        {
                            Item = item,
                            Amount = (int)amount,
                            Bill = file != null ? file.FileName : null,
                            Timesheet = timesheet1,
                            IsDeleted = false,
                            Date = date
                        };
                        _db.TimesheetReimbursements.Add(timesheetReimbursement);
                    }
                }
                else
                {
                    TimesheetReimbursement timesheetReimbursement = _db.TimesheetReimbursements.FirstOrDefault(t => t.TimesheetReimbursementId == id);
                    timesheetReimbursement.Amount = (int)amount;
                    timesheetReimbursement.Item = item;
                    timesheetReimbursement.Bill = file != null ? file.FileName : timesheetReimbursement.Bill;

                    _db.TimesheetReimbursements.Update(timesheetReimbursement);
                }
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public bool DeleteTimesheetReimbursement(int id)
        {
            try
            {
                TimesheetReimbursement timesheetReimbursement = _db.TimesheetReimbursements.FirstOrDefault(t=>t.TimesheetReimbursementId == id);
                if(timesheetReimbursement == null)
                {
                    return false;
                }
                timesheetReimbursement.IsDeleted = true;
                _db.TimesheetReimbursements.Update(timesheetReimbursement);
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }
        public bool FinalizeTimesheet(int id)
        {
            try
            {
                Timesheet timesheet = _db.Timesheets.FirstOrDefault(t=>t.TimesheetId == id);
                if(timesheet == null)
                {
                    return false;
                }
                timesheet.IsFinalized = new BitArray(new[] { true });
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
