using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Repository.Interface
{
    public interface IPatient
    {
        public int Login(LoginViewModel loginViewModel);

        public PasswordReset GetResetPassword(string Token);

        public bool ResetPassword(ResetPasswordViewModel modal);

        public Task<bool> SendResetLink(string email);

        public Task<bool> PatientRequest(PatientRequestViewModel modal);

        public Task<bool> BusinessRequest(BusinessRequestViewModel modal);

        public Task<bool> FamilyRequest(FamilyRequestViewModel modal);

        public Task<bool> ConciergeRequest(ConciergeRequestViewModel modal);

        public AspNetUser GetAspNetUser(string email);
        public AspNetUser GetAspNetUserLogin(string email);

        public DashboardViewModel GetDashboardData(int page = 1, int pageSize = 10);

        public AspNetUser GetAspNetUserById(int id);

        public bool Register(RegisterViewModel modal);

        public FamilyRequestViewModel GetFamilyRequest();

        public PatientRequestViewModel GetPatientRequest();

        public Task<bool> SomeoneElseRequest(FamilyRequestViewModel familyRequestViewModel);

        public Task<bool> SelfRequest(PatientRequestViewModel patientRequestViewModel);

        public PatientRequestViewModel GetPatientProfile();

        public int UpdatePatientProfile(PatientRequestViewModel patientRequestViewModel);

        public ViewDocumentModal GetViewDocument(int id);

        public Task<bool> FileUpload(IFormFile file, int id);
    }
}
