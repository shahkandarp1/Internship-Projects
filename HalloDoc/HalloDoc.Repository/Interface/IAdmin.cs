using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Repository.Interface
{
    public interface IAdmin
    {
        public AdminDashboardViewModel adminDashboardContent(string? status, string? search, string? requestor, int? region);

        public MemoryStream exportAll();

        public MemoryStream export(AdminDashboardViewModel adminDashboardViewModel);

        public ViewCaseViewModel viewCase(int id);

        public bool viewCase(ViewCaseViewModel model);

        public bool cancelRequest(int id, string notes, string select);

        public bool sendLink(AdminDashboardViewModel dashboardViewModel);

        public bool verifyRegion(string region);

        public bool verifyBlock(string Email);

        public PatientRequestViewModel createRequest();

        public bool createRequest(PatientRequestViewModel model);

        public ViewNotesViewModel viewNotes(int id);

        public bool updateAdminNotes(ViewNotesViewModel viewNotesViewModel);

        public int login(LoginViewModel loginViewModel);

        public int forgotPassword(ForgotPasswordViewModel forgotPasswordViewModel);

        public bool logout();

        public ViewDocumentModal viewUploads(int id);

        public Task<bool> fileUpload(IFormFile file,int id);

        public int deleteSingleFile(int id);

        public Task<Tuple<MemoryStream, string>> downloadMultipleFiles(ViewDocumentModal viewDocumentModal);

        public int deleteAllFile(string filename);

        public Task<bool> sendDocumentsMail(string filename);

        public PasswordReset getPasswordReset(string token);

        public bool resetPassword(ResetPasswordViewModel resetPasswordViewModel);

        public List<Physician> getPhysician(int regionid);

        public bool assignCase(AdminDashboardViewModel adminDashboardViewModel);

        public Task<bool> sendAgreement(AdminDashboardViewModel adminDashboardViewModel);

        public bool blockCase(AdminDashboardViewModel adminDashboardViewModel);
    }
}
