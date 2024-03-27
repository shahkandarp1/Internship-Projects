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
        public int login(LoginViewModel loginViewModel);

        public PasswordReset getResetPassword(string Token);

        public bool resetPassword(ResetPasswordViewModel modal);

        public Task<bool> sendResetLink(string email);

        public Task<bool> patientRequest(PatientRequestViewModel modal);

        public Task<bool> businessRequest(BusinessRequestViewModel modal);

        public Task<bool> familyRequest(FamilyRequestViewModel modal);

        public Task<bool> conciergeRequest(ConciergeRequestViewModel modal);

        public AspNetUser getAspNetUser(string email);
        public AspNetUser getAspNetUserLogin(string email);

        public DashboardViewModel getDashboardData(int page = 1, int pageSize = 10);

        public AspNetUser getAspNetUserById(int id);

        public bool register(RegisterViewModel modal);

        public FamilyRequestViewModel getFamilyRequest();

        public PatientRequestViewModel getPatientRequest();

        public Task<bool> someoneElseRequest(FamilyRequestViewModel familyRequestViewModel);

        public Task<bool> selfRequest(PatientRequestViewModel patientRequestViewModel);

        public PatientRequestViewModel getPatientProfile();

        public int updatePatientProfile(PatientRequestViewModel patientRequestViewModel);

        public ViewDocumentModal getViewDocument(int id);

        public Task<bool> fileUpload(IFormFile file, int id);
    }
}
