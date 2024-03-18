using DocumentFormat.OpenXml.Spreadsheet;
using HalloDoc.Models;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Repository.Interface
{
    public interface IAdmin
    {
        public AdminDashboardViewModel adminDashboardContent(string? status, string? search, string? requestor, int? region,int page = 1,int pageSize = 10);

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

        public bool transferCase(AdminDashboardViewModel adminDashboardViewModel);

        public bool isSamePhysician(AdminDashboardViewModel adminDashboardViewModel);

        public Task<bool> sendAgreement(AdminDashboardViewModel adminDashboardViewModel);

        public bool blockCase(AdminDashboardViewModel adminDashboardViewModel);

        public bool clearCase(AdminDashboardViewModel adminDashboardViewModel);

        public OrdersViewModel orders(int id);

        public List<HealthProfessional> getBusiness(int professionid);

        public HealthProfessional getBusinessData(int businessid);

        public bool placeOrder(OrdersViewModel ordersViewModel);

        public AdminProfileViewModel getAdmin();

        public bool updateProfile(AdminProfileViewModel adminProfileViewModel);

        public bool resetPasswordProfile(string password);

        public Request getRequest(int id);

        public bool agree(int id);

        public bool disagree(int id,string notes);

        public EncounterFormViewModel getEncounterFormDetails(int id);

        public bool updateEncounterForm(EncounterFormViewModel encounterFormViewModel);

        public CloseCaseViewModel getCloseCase(int id);

        public bool updateCloseCase(CloseCaseViewModel closeCaseViewModel);

        public bool closeCase(int id);

        public ProviderViewModel getProviderPageDetails(int id=-1, int page = 1, int pageSize = 10);

        public bool changeNotification(int id, bool update);

        public Task<bool> contactProvider(ProviderViewModel providerViewModel);

        public PhysicianAccountViewModel getCreatePhysicianDetails();

        public Task<bool> createPhysician(PhysicianAccountViewModel physicianAccountViewModel);

        public PhysicianAccountViewModel getPhysicianDetails(int id);

        public Task<bool> fileUploadPhysician(IFormFile file, int id, string name);

        public Task<bool> updatePhysician(PhysicianAccountViewModel physicianAccountViewModel);

        public bool resetPasswordPhysician(string password, int id);

        public bool deletePhysician(int id);

        public PatientHistoryViewModel getAllPatients(string? firstname, string? lastname, string? email, string? phone, int page = 1, int pageSize = 10);

        public PatientHistoryViewModel getAllPatientRecords(int id,int page = 1, int pageSize = 10);

        public BlockHistoryViewModel getBlockHistoryData(string? name, DateTime? date, string? email, string? phone, int page = 1, int pageSize = 10);

        public bool toggleActive(int blockrequestid, bool value);

        public bool restoreBlock(int blockrequestid);

        public SearchRecordViewModel getSearchedData(int? status,string? name,int? requesttypeid,DateTime? fromdos,DateTime? todos,string? providername,string? email,string? phonenumber, int page = 1, int pageSize = 10);

        public bool deleteRequest(int id);

        public MemoryStream exportSearchedData(SearchRecordViewModel searchRecordViewModel);

        public AccountAccessViewModel getAllRolesDetails(int page = 1, int pageSize = 10);

        public AdminNavbarViewModel getCreateAccessNavbar();

        public List<Menu> getMenus(int? id);

        public bool createRole(string? menus, string? role_name, int? account_type);

        public bool deleteRole(int? id);

        public EditAccessViewModel getRoleDetails(int? id);

    }
}
