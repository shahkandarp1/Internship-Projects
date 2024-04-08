using HalloDoc.Repository.Interface;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

        public void LogEmail(string emailTemplate, string subject, string userEmail, string confirmation_no, int request_id, int admin_id, int physician_id, bool success, int retryCount, int role_id)
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
                    SentDate = DateTime.Now

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

        public void LogSMS(string SmsTemplate, string userPhone, string confirmation_no, int request_id, int admin_id, int physician_id, bool success, int retryCount, int role_id)
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
                    SentDate = DateTime.Now

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
                        var body = $"Hello {admin[i].FirstName} {admin[i].LastName},<br />{physicianAccountViewModel.editReason}<br /><br />Regards,<br/>{platformTitle}<br/>";
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
                            LogEmail(body, subject, admin[i].Email, null, -1, admin[i].AdminId, -1, true, retryCount, 2);
                            break;
                        }
                        catch (Exception ex)
                        {

                            if (retryCount >= 3)
                            {
                                LogEmail(body, subject, admin[i].Email, null, -1, admin[i].AdminId, -1, false, retryCount, 2);
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

                        var messageBody = $"Hello {admin[i].FirstName} {admin[i].LastName},\n{physicianAccountViewModel.editReason}\n\nRegards,\n{platformTitle}";
                        try
                        {

                            TwilioClient.Init(accountSid, authToken);

                            var message = MessageResource.Create(
                                from: new Twilio.Types.PhoneNumber(twilionumber),
                                body: messageBody,
                                to: new Twilio.Types.PhoneNumber("+91" + admin[i].Mobile)
                            );


                            success = true;
                            LogSMS(messageBody, admin[i].Mobile, null, -1, admin[i].AdminId, -1, true, retryCount, 2);
                            break;
                        }
                        catch (Exception ex)
                        {

                            if (retryCount >= 3)
                            {
                                LogSMS(messageBody, admin[i].Mobile, null, -1, admin[i].AdminId, -1, false, retryCount, 2);
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
                if (adminDashboardViewModel.typeOfCare == "Consult")
                {
                    status = 5;
                }
                else
                {
                    status = 4;
                }

                Request request = _db.Requests.FirstOrDefault(r => r.RequestId == adminDashboardViewModel.RequestId);
                if (request == null)
                {
                    return false;
                }

                request.Status = (short)status;
                request.ModifiedDate = DateTime.Now;

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


    }
}
