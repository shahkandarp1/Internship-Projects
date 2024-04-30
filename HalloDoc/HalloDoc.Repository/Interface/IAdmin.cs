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
        public AdminDashboardViewModel AdminDashboardContent(string? status, string? search, string? requestor, int? region,int page = 1,int pageSize = 10);

        public MemoryStream ExportAll();

        public MemoryStream Export(AdminDashboardViewModel adminDashboardViewModel);

        public ViewCaseViewModel ViewCase(int id);

        public bool ViewCase(ViewCaseViewModel model);

        public bool CancelRequest(int id, string notes, string select);

        public Task<bool> SendLink(AdminDashboardViewModel dashboardViewModel);

        public bool VerifyRegion(string region);

        public bool VerifyBlock(string Email);

        public PatientRequestViewModel CreateRequest();

        public Task<bool> CreateRequest(PatientRequestViewModel model);

        public ViewNotesViewModel ViewNotes(int id);

        public bool UpdateAdminNotes(ViewNotesViewModel viewNotesViewModel);

        public int Login(LoginViewModel loginViewModel);

        public int ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel);

        public bool Logout();

        public ViewDocumentModal ViewUploads(int id);

        public Task<bool> FileUpload(IFormFile file,int id);

        public int DeleteSingleFile(int id);

        public Task<Tuple<MemoryStream, string>> DownloadMultipleFiles(ViewDocumentModal viewDocumentModal);

        public int DeleteAllFile(string filename);

        public Task<bool> SendDocumentsMail(string filename);

        public PasswordReset GetPasswordReset(string token);

        public bool ResetPassword(ResetPasswordViewModel resetPasswordViewModel);

        public List<RegionSpecificPhysician> GetPhysician(int regionid);

        public bool AssignCase(AdminDashboardViewModel adminDashboardViewModel);
        public bool SamePhysicianAssignCase(AdminDashboardViewModel adminDashboardViewModel);

        public bool TransferCase(AdminDashboardViewModel adminDashboardViewModel);

        public bool IsSamePhysician(AdminDashboardViewModel adminDashboardViewModel);

        public Task<bool> SendAgreement(AdminDashboardViewModel adminDashboardViewModel);

        public bool BlockCase(AdminDashboardViewModel adminDashboardViewModel);

        public bool ClearCase(AdminDashboardViewModel adminDashboardViewModel);

        public OrdersViewModel Orders(int id);

        public List<HealthProfessional> GetBusiness(int professionid);

        public HealthProfessional GetBusinessData(int businessid);

        public bool PlaceOrder(OrdersViewModel ordersViewModel);

        public AdminProfileViewModel GetAdmin(int id,string active);

        public bool UpdateProfile(AdminProfileViewModel adminProfileViewModel);
        public int CheckAdminEmail(AdminProfileViewModel adminProfileViewModel);

        public bool ResetPasswordProfile(string password,int id);

        public Request GetRequest(int id);

        public bool Agree(int id);

        public bool Disagree(int id,string notes);

        public EncounterFormViewModel GetEncounterFormDetails(int id);

        public bool UpdateEncounterForm(EncounterFormViewModel encounterFormViewModel);

        public CloseCaseViewModel GetCloseCase(int id);

        public bool UpdateCloseCase(CloseCaseViewModel closeCaseViewModel);

        public bool CloseCase(int id);

        public ProviderViewModel GetProviderPageDetails(int id=-1, int page = 1, int pageSize = 10);

        public bool ChangeNotification(int id, bool update);

        public Task<bool> ContactProvider(ProviderViewModel providerViewModel);

        public PhysicianAccountViewModel GetCreatePhysicianDetails();
        public List<Role> GetPhysicianRoles();

        public Task<bool> CreatePhysician(PhysicianAccountViewModel physicianAccountViewModel);

        public PhysicianAccountViewModel GetPhysicianDetails(int id,AdminNavbarViewModel adminNavbarViewModel);

        public Task<bool> FileUploadPhysician(IFormFile file, int id, string name);

        public Task<bool> UpdatePhysician(PhysicianAccountViewModel physicianAccountViewModel);
        public int CheckPhysicianEmail(PhysicianAccountViewModel physicianAccountViewModel);

        public bool ResetPasswordPhysician(string password, int id);

        public bool DeletePhysician(int id);

        public PatientHistoryViewModel GetAllPatients(string? firstname, string? lastname, string? email, string? phone, int page = 1, int pageSize = 10);

        public PatientHistoryViewModel GetAllPatientRecords(int id,int page = 1, int pageSize = 10);

        public BlockHistoryViewModel GetBlockHistoryData(string? name, DateTime? date, string? email, string? phone, int page = 1, int pageSize = 10);

        public bool ToggleActive(int blockrequestid, bool value);

        public bool RestoreBlock(int blockrequestid);

        public SearchRecordViewModel GetSearchedData(int? status,string? name,int? requesttypeid,DateTime? fromdos,DateTime? todos,string? providername,string? email,string? phonenumber, int page = 1, int pageSize = 10);

        public bool DeleteRequest(int id);

        public MemoryStream ExportSearchedData(SearchRecordViewModel searchRecordViewModel);

        public AccountAccessViewModel GetAllRolesDetails(int page = 1, int pageSize = 10);

        public AdminNavbarViewModel GetCreateAccessNavbar();

        public List<Menu> GetMenus(int? id);

        public bool CreateRole(string? menus, string? role_name, int? account_type);
        public bool CheckRole(string? role_name);

        public bool DeleteRole(int? id);

        public EditAccessViewModel GetRoleDetails(int? id);

        public bool EditRoleDetails(int? id, string? menus, string? role_name, int? account_type);

        public EmailLogViewModel GetEmailLogDetails(int? roleid,string? name,string? email,DateTime? createddate,DateTime? sentdate,int page = 1,int pageSize = 10);
        public EmailLogViewModel GetSMSLogDetails(int? roleid,string? name,string? phonenumber, DateTime? createddate,DateTime? sentdate,int page = 1,int pageSize = 10);

        public PartnerViewModal GetPartnerDetails(string? name, int? id, int page = 1, int pageSize = 10);

        public BusinessViewModel GetBusinessNavbar();
        public BusinessViewModel GetBusinessDetails(int id);
        public bool CreateBusiness(BusinessViewModel businessViewModel);
        public bool EditBusiness(BusinessViewModel businessViewModel);
        public bool DeleteBusiness(int id);
        public ProviderLocationViewModel GetProviderLocation();

        public AdminProfileViewModel GetCreateAdminProfilePageDetails();
        public List<Role> GetAdminRoles();
        public Task<bool> CreateAdmin(AdminProfileViewModel adminProfileViewModel);

        public UserAccessViewModel GetUserAccessDetails(int? roleid, int page = 1, int pageSize = 10);
        public bool DeleteAdmin(int id);

        public SchedulingViewModel GetAllShiftDetails(int? regionid);
        public int CreateShift(SchedulingViewModel schedulingViewModel);

        public int EditShift(DateTime shiftdate, TimeOnly starttime, TimeOnly endtime,int physicianid,int shiftdetailid);
        public bool DeleteShift(int? id);
        public MDOnCallViewModel GetMdOnCallDetails(int regionid = -1);
        public ShiftsForReviewViewModel GetRequestedShifts(int regionid = -1, int page = 1, int pageSize = 10);
        public bool AprooveShifts(ShiftsForReviewViewModel shiftsForReviewViewModel);
        public bool DeleteShifts(ShiftsForReviewViewModel shiftsForReviewViewModel);
        public bool ToggleShiftStatus(int? id);
        public Task<bool> RequestDTYSupport(AdminDashboardViewModel adminDashboardViewModel);
        public PayRateViewModel GetPayRate(int id);
        public bool CheckUserRole(string email);
        public bool UpdatePayRate(PayRateViewModel payRateViewModel);

    }
}
