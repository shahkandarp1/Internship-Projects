using HalloDoc.Repository.Interface;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Presentation;
using System.Collections;
using DocumentFormat.OpenXml.Spreadsheet;
using Irony.Parsing;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.IO.Compression;

namespace HalloDoc.Repository.Repository
{
    public class Patient:IPatient
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _context;
        private readonly IJwtService _jwt;

        public Patient(ApplicationDbContext db, IHttpContextAccessor context, IJwtService jwt)
        {
            _db = db;
            _context = context;
            _jwt = jwt;
        }

        int IPatient.login(LoginViewModel model)
        {
            try
            {
                var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == model.Email);
                if (user != null)
                {
                    var role = _db.AspNetUserRoles.FirstOrDefault(u=>u.UserId == user.Id);
                    if(role.RoleId != 1)
                    {
                        return 4;
                    }
                    var passwordHasher = new PasswordHasher<AspNetUser>();
                    var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
                    if (result == PasswordVerificationResult.Success)
                    { 
                        return 2;
                    }
                    else
                    {
                        return 3;
                    }
                }
                else
                {
                    var aspid = _db.AspNetUsers.FirstOrDefault(u => u.UserName == model.Email);
                    if(aspid != null)
                    {
                        var role = _db.AspNetUserRoles.FirstOrDefault(u => u.UserId == aspid.Id);
                        if (role.RoleId != 2 && role.RoleId != 3)
                        {
                            return 4;
                        }
                        if(role.RoleId == 2)
                        {
                            HalloDoc.Admin admin = _db.Admins.FirstOrDefault(a=>a.AspNetUserId == aspid.Id);
                            if(admin?.Status == null || admin?.Status !=2)
                            {
                                return 6;
                            }
                        }
                        if(role.RoleId == 3)
                        {
                            HalloDoc.Physician physician = _db.Physicians.FirstOrDefault(a=>a.AspNetUserId == aspid.Id);
                            if(physician?.Status == null || physician?.Status != 2)
                            {
                                return 6;
                            }
                        }
                        var passwordHasher = new PasswordHasher<AspNetUser>();
                        var result = passwordHasher.VerifyHashedPassword(aspid, aspid.PasswordHash, model.Password);
                        if (result == PasswordVerificationResult.Success)
                        {
                            return 2;
                        }
                        else
                        {
                            return 3;
                        }

                    }
                    else
                    {
                        return 4;
                    }
                }
            }
            catch(Exception ex)
            {
                return 5;
            }
        }

        PasswordReset IPatient.getResetPassword(string Token)
        {
            return _db.PasswordResets.FirstOrDefault(u => u.Token == Token); 
        }

        bool IPatient.resetPassword(ResetPasswordViewModel modal)
        {
            try
            {
                PasswordReset passwordReset = _db.PasswordResets.FirstOrDefault(u => u.Token == modal.Token);
                AspNetUser aspNetUser = _db.AspNetUsers.FirstOrDefault(u => u.Email == passwordReset.Email);
                var passwordHasher = new PasswordHasher<AspNetUser>();
                aspNetUser.PasswordHash = passwordHasher.HashPassword(aspNetUser, modal.Password);
                _db.AspNetUsers.Update(aspNetUser);
                _db.SaveChanges();
                passwordReset.IsUpdated = true;
                _db.PasswordResets.Update(passwordReset);
                _db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        bool IPatient.sendResetLink(string email)
        {
            var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return false;
            }
            string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
            string senderPassword = "shahkandarp2430";
            string platformTitle = "HalloDoc";
            string Token = Guid.NewGuid().ToString();
            PasswordReset passwordReset = new PasswordReset
            {
                Token = Token,
                Email = email,
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
            mailMessage.To.Add(email);

            client.SendMailAsync(mailMessage);

            return true;
        }


        async Task<bool> IPatient.patientRequest(PatientRequestViewModel modal)
        {
            try
            {
                var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == modal.Email);
                if (modal.ImageContent != null && modal.ImageContent.Length > 0)
                {
                    var filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", modal.ImageContent.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await modal.ImageContent.CopyToAsync(stream);
                    }
                }

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

                    if (modal.ImageContent != null)
                    {
                        RequestWiseFile rfile = new RequestWiseFile
                        {
                            RequestId = req.RequestId,
                            FileName = modal.ImageContent.FileName,
                            CreatedDate = DateTime.Now,
                            IsDeleted = new BitArray(new[] { false })
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

                }
                else
                {

                    AspNetUser aspuser = new AspNetUser
                    {
                        UserName = modal.Email,
                        Email = modal.Email,
                        PhoneNumber = modal.Phone,
                        CreatedDate = DateTime.Now,
                        PasswordHash = modal.Password,
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
                    if (modal.ImageContent != null)
                    {
                        RequestWiseFile rfile = new RequestWiseFile
                        {
                            RequestId = req.RequestId,
                            FileName = modal.ImageContent.FileName,
                            CreatedDate = DateTime.Now,
                            IsDeleted = new BitArray(new[] { false })
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

                }
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        async Task<bool> IPatient.businessRequest(BusinessRequestViewModel modal)
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
                        RegionId = region.RegionId,
                        ZipCode = modal.ZipCode,
                        Address = modal.Room,
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
                        FirstName = modal.BusinessFirstName,
                        LastName = modal.BusinessLastName,
                        PhoneNumber = modal.BusinessPhoneNumber,
                        Email = modal.BusinessEmail,
                        RequestClientId = rc.RequestClientId,
                        RequestTypeId = 1,
                        UserId = curr_user.UserId,
                        Status = 1,
                        CreatedDate = DateTime.Now,
                        IsUrgentEmailSent = new BitArray(new[] { false }),
                        ConfirmationNumber = string.Concat(region.Abbreviation, modal.FirstName.Substring(0, 2).ToUpper(), modal.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4)),
                        CaseNumber = modal.BusinessCaseNumber
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

                    Business business = new Business
                    {
                        Name = modal.BusinessPropertyName,
                        CreatedDate = DateTime.Now,
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
                        ZipCode = modal.ZipCode,
                        Address = modal.Room,
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
                        FirstName = modal.BusinessFirstName,
                        LastName = modal.BusinessLastName,
                        PhoneNumber = modal.BusinessPhoneNumber,
                        Email = modal.BusinessEmail,
                        RequestClientId = rc.RequestClientId,
                        RequestTypeId = 1,
                        UserId = us.UserId,
                        Status = 1,
                        CreatedDate = DateTime.Now,
                        IsUrgentEmailSent = new BitArray(new[] { false }),
                        ConfirmationNumber = string.Concat(region.Abbreviation, modal.FirstName.Substring(0, 2).ToUpper(), modal.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4)),
                        CaseNumber = modal.BusinessCaseNumber
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

                    Business business = new Business
                    {
                        Name = modal.BusinessPropertyName,
                        CreatedDate = DateTime.Now,
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

                    string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                    string senderPassword = "shahkandarp2430";
                    var platformTitle = "HalloDoc";
                    var inviteLink = $"https://localhost:7088/Login/Register/{aspuser.Id}";
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
                }
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }
        
        async Task<bool> IPatient.familyRequest(FamilyRequestViewModel modal)
        {
            try
            {
                var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == modal.Email);
                if (modal.ImageContent != null && modal.ImageContent.Length > 0)
                {
                    var filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", modal.ImageContent.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await modal.ImageContent.CopyToAsync(stream);
                    }
                }

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
                        FirstName = modal.FamilyFirstName,
                        LastName = modal.FamilyLastName,
                        PhoneNumber = modal.FamilyPhoneNumber,
                        Email = modal.FamilyEmail,
                        RequestClientId = rc.RequestClientId,
                        RequestTypeId = 3,
                        UserId = curr_user.UserId,
                        Status = 1,
                        CreatedDate = DateTime.Now,
                        IsUrgentEmailSent = new BitArray(new[] { false }),
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
                            CreatedDate = DateTime.Now,
                            IsDeleted = new BitArray(new[] { false })
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
                        FirstName = modal.FamilyFirstName,
                        LastName = modal.FamilyLastName,
                        PhoneNumber = modal.FamilyPhoneNumber,
                        Email = modal.FamilyEmail,
                        RequestClientId = rc.RequestClientId,
                        RequestTypeId = 3,
                        UserId = us.UserId,
                        Status = 1,
                        CreatedDate = DateTime.Now,
                        IsUrgentEmailSent = new BitArray(new[] { false }),
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
                            CreatedDate = DateTime.Now,
                            IsDeleted = new BitArray(new[] { false })
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

                    string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                    string senderPassword = "shahkandarp2430";
                    var platformTitle = "HalloDoc";
                    var inviteLink = $"https://localhost:7088/Login/Register/{aspuser.Id}";
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

                }
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }
        
        async Task<bool> IPatient.conciergeRequest(ConciergeRequestViewModel modal)
        {
            try
            {
                var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == modal.Email);

                var region = _db.Regions.FirstOrDefault(u => u.Name == modal.ConciergeState.Trim().ToLower().Replace(" ", ""));
                if (user != null)
                {
                    var curr_user = _db.Users.FirstOrDefault(u => u.AspNetUserId == user.Id);
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
                        Address = modal.Room,
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
                        FirstName = modal.ConciergeFirstName,
                        LastName = modal.ConciergeLastName,
                        PhoneNumber = modal.ConciergePhoneNumber,
                        Email = modal.ConciergeEmail,
                        RequestClientId = rc.RequestClientId,
                        RequestTypeId = 4,
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

                    Concierge concierge = new Concierge
                    {
                        ConciergeName = string.Concat(modal.ConciergeFirstName, ' ', modal.ConciergeLastName),
                        RegionId = region.RegionId,
                        CreatedDate = DateTime.Now,
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
                        Street = modal.ConciergeStreet,
                        City = modal.ConciergeCity,
                        State = modal.ConciergeState,
                        RegionId = region.RegionId,
                        ZipCode = modal.ConciergeZipcode,
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
                        State = modal.ConciergeState,
                        Street = modal.ConciergeStreet,
                        City = modal.ConciergeCity,
                        RegionId = region.RegionId,
                        ZipCode = modal.ConciergeZipcode,
                        Address = modal.Room,
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
                        FirstName = modal.ConciergeFirstName,
                        LastName = modal.ConciergeLastName,
                        PhoneNumber = modal.ConciergePhoneNumber,
                        Email = modal.ConciergeEmail,
                        RequestClientId = rc.RequestClientId,
                        RequestTypeId = 4,
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

                    Concierge concierge = new Concierge
                    {
                        ConciergeName = string.Concat(modal.ConciergeFirstName, ' ', modal.ConciergeLastName),
                        RegionId = region.RegionId,
                        CreatedDate = DateTime.Now,
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

                    string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                    string senderPassword = "shahkandarp2430";
                    var platformTitle = "HalloDoc";
                    var inviteLink = $"https://localhost:7088/Login/Register/{aspuser.Id}";   
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
                }
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        AspNetUser IPatient.getAspNetUser(string email)
        {
            return _db.AspNetUsers.SingleOrDefault(u => u.Email == email); 
        }
        AspNetUser IPatient.getAspNetUserLogin(string email)
        {
            if(_db.AspNetUsers.SingleOrDefault(u => u.Email == email) == null)
            {
                return _db.AspNetUsers.SingleOrDefault(u => u.UserName == email);
            }
            return _db.AspNetUsers.SingleOrDefault(u => u.Email == email); 
        }

        DashboardViewModel IPatient.getDashboardData(int page = 1, int pageSize = 10)
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);
            int count = _db.Requests.Where(u => u.UserId == cookieModel.userId).Count();
            List<RequestViewModel> data = _db.RequestViewModels.FromSqlRaw($"SELECT * FROM PatientDashboardData({cookieModel.userId},{pageSize},{((page - 1) * pageSize)})").ToList();
            var curr_user = _db.Users.FirstOrDefault(u => u.UserId == cookieModel.userId);
            DashboardViewModel dashboardViewModel = new DashboardViewModel
            {
                requests = data,
                name = string.Concat(curr_user.FirstName, ' ', curr_user.LastName),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = count,
                TotalPages = (int)Math.Ceiling((double)count / pageSize)
            };
            return dashboardViewModel;
        }

        AspNetUser IPatient.getAspNetUserById(int id)
        {
            return _db.AspNetUsers.FirstOrDefault(u => u.Id == id);
        }

        bool IPatient.register(RegisterViewModel modal)
        {
            try
            {
                AspNetUser aspNetUser = _db.AspNetUsers.FirstOrDefault(u => u.Id == modal.Id);
                aspNetUser.Email = modal.Email;
                aspNetUser.UserName = modal.Email;
                var passwordHasher = new PasswordHasher<AspNetUser>();
                aspNetUser.PasswordHash = passwordHasher.HashPassword(aspNetUser, modal.Password);
                _db.AspNetUsers.Update(aspNetUser);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        FamilyRequestViewModel IPatient.getFamilyRequest()
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);
            var session_user = _db.Users.FirstOrDefault(u => u.UserId == cookieModel.userId);
            FamilyRequestViewModel familyRequestViewModel = new FamilyRequestViewModel()
            {
                FamilyFirstName = session_user.FirstName,
                FamilyLastName = session_user.LastName,
                FamilyEmail = session_user.Email,
                FamilyPhoneNumber = session_user.Mobile,
            };
            return familyRequestViewModel;
        }

        async Task<bool> IPatient.someoneElseRequest(FamilyRequestViewModel modal)
        {
            try
            {
                var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == modal.Email);
                if (modal.ImageContent != null && modal.ImageContent.Length > 0)
                {
                    var filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", modal.ImageContent.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await modal.ImageContent.CopyToAsync(stream);
                    }
                }

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
                        RegionId = region.RegionId,
                        ZipCode = modal.ZipCode,
                        Address = modal.Room,
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
                        FirstName = modal.FamilyFirstName,
                        LastName = modal.FamilyLastName,
                        PhoneNumber = modal.FamilyPhoneNumber,
                        Email = modal.FamilyEmail,
                        RequestClientId = rc.RequestClientId,
                        RequestTypeId = 3,
                        UserId = curr_user.UserId,
                        Status = 1,
                        CreatedDate = DateTime.Now,
                        IsUrgentEmailSent = new BitArray(new[] { false }),
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
                            CreatedDate = DateTime.Now,
                            IsDeleted = new BitArray(new[] { false })
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
                        ZipCode = modal.ZipCode,
                        Notes = modal.Symptoms,
                        NotiEmail = modal.Email,
                        Address = modal.Room,
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
                        FirstName = modal.FamilyFirstName,
                        LastName = modal.FamilyLastName,
                        PhoneNumber = modal.FamilyPhoneNumber,
                        Email = modal.FamilyEmail,
                        RequestClientId = rc.RequestClientId,
                        RequestTypeId = 3,
                        UserId = us.UserId,
                        Status = 1,
                        CreatedDate = DateTime.Now,
                        IsUrgentEmailSent = new BitArray(new[] { false }),
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
                            CreatedDate = DateTime.Now,
                            IsDeleted = new BitArray(new[] { false })
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

                    string senderEmail = "tatva.dotnet.kandarpshah@outlook.com";
                    string senderPassword = "shahkandarp2430";
                    var platformTitle = "HalloDoc";
                    var inviteLink = $"https://localhost:7088/Login/Register/{aspuser.Id}";   
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
                }
                return true;
            }
            catch(Exception  ex)
            {
                return false;
            }
        }

        PatientRequestViewModel IPatient.getPatientRequest()
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);
            var user = _db.Users.FirstOrDefault(u => u.UserId == cookieModel.userId);
            PatientRequestViewModel patientRequestViewModel = new PatientRequestViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Mobile,
                DateOfBirth = DateTime.Parse($"{user.IntYear}-{user.StrMonth}-{user.IntDate}")

            };
            return patientRequestViewModel;
        }

        async Task<bool> IPatient.selfRequest(PatientRequestViewModel modal)
        {
            try
            {
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);
                var user = _db.AspNetUsers.FirstOrDefault(u => u.Id == cookieModel.aspId);
                if (modal.ImageContent != null && modal.ImageContent.Length > 0)
                {
                    var filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", modal.ImageContent.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await modal.ImageContent.CopyToAsync(stream);
                    }
                }

                var region = _db.Regions.FirstOrDefault(u => u.Name == modal.State.Trim().ToLower().Replace(" ", ""));

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
                    RegionId = region.RegionId,
                    ZipCode = modal.ZipCode,
                    Address = modal.Room,
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

                if (modal.ImageContent != null)
                {
                    RequestWiseFile rfile = new RequestWiseFile
                    {
                        RequestId = req.RequestId,
                        FileName = modal.ImageContent.FileName,
                        CreatedDate = DateTime.Now,
                        IsDeleted = new BitArray(new[] { false })
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
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        PatientRequestViewModel IPatient.getPatientProfile()
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);
            var curr_user = _db.Users.FirstOrDefault(u => u.UserId == cookieModel.userId);
            PatientRequestViewModel patientRequestViewModel = new PatientRequestViewModel
            {
                FirstName = curr_user.FirstName,
                LastName = curr_user.LastName,
                Email = curr_user.Email,
                Phone = curr_user.Mobile,
                DateOfBirth = DateTime.Parse($"{curr_user.IntYear}-{curr_user.StrMonth}-{curr_user.IntDate}"),
                Street = curr_user.Street,
                State = curr_user.State,
                City = curr_user.City,
                ZipCode = curr_user.ZipCode
            };
            return patientRequestViewModel;
        }

        int IPatient.updatePatientProfile(PatientRequestViewModel modal)
        {
            try
            {
                var region = _db.Regions.FirstOrDefault(u => u.Name == modal.State.Trim().ToLower().Replace(" ", ""));
                var request = _context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                CookieModel cookieModel = _jwt.getDetails(token);

                User user = _db.Users.FirstOrDefault(u => u.UserId == cookieModel.userId);

                if (user.Email != modal.Email)
                {
                    var check_user_count = _db.AspNetUsers.Where(u => u.Email == modal.Email).Count();
                    if (check_user_count == 1)
                    {
                        return 2;
                    }
                }

                user.FirstName = modal.FirstName;
                user.LastName = modal.LastName;
                user.Mobile = modal.Phone;
                user.Street = modal.Street;
                user.City = modal.City;
                user.State = modal.State;
                user.RegionId = region.RegionId;
                user.ZipCode = modal.ZipCode;
                user.StrMonth = modal.DateOfBirth.Month.ToString();
                user.IntYear = modal.DateOfBirth.Year;
                user.IntDate = modal.DateOfBirth.Day;
                user.ModifiedBy = cookieModel.aspId;
                user.ModifiedDate = DateTime.Now;

                _db.Users.Update(user);
                _db.SaveChanges();
                return 1;
            }
            catch(Exception exp)
            {
                return 3;
            }
        }

        ViewDocumentModal IPatient.getViewDocument(int id)
        {
            var requestt = _context.HttpContext.Request;
            var token = requestt.Cookies["jwt"];
            CookieModel cookieModel = _jwt.getDetails(token);
            var request = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(u => u.RequestId == id);
            var documents = _db.RequestWiseFiles.Include(u => u.Admin).Include(u => u.Physician).Where(u => u.RequestId == id && u.IsDeleted.Equals(new BitArray(new[] { false }))).ToList();
            var user = _db.Users.FirstOrDefault(u => u.UserId == cookieModel.userId);
            ViewDocumentModal viewDocumentModal = new ViewDocumentModal()
            {
                patient_name = string.Concat(request.RequestClient.FirstName, ' ', request.RequestClient.LastName),
                name = string.Concat(user.FirstName, ' ', user.LastName),
                confirmation_number = request.ConfirmationNumber,
                requestWiseFiles = documents,
                uploader_name = string.Concat(request.FirstName, ' ', request.LastName)
            };
            return viewDocumentModal;
        }

        async Task<bool> IPatient.fileUpload(IFormFile file, int id)
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
                RequestWiseFile requestWiseFile = new RequestWiseFile
                {
                    RequestId = id,
                    FileName = file.FileName,
                    CreatedDate = DateTime.Now,
                    IsDeleted = new BitArray(new[] { false }),
                };
                _db.RequestWiseFiles.Add(requestWiseFile);
                _db.SaveChanges();
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

    }
}
