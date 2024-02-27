using HalloDoc.ViewModels;
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

        public bool createRequest(PatientRequestViewModel model);

        public ViewNotesViewModel viewNotes(int id);

        public bool updateAdminNotes(ViewNotesViewModel viewNotesViewModel);
    }
}
