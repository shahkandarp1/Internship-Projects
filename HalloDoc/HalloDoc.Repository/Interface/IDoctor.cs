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
    public interface IDoctor
    {
        public bool AcceptCase(int? id);
        public Task<bool> RequestAdmin(PhysicianAccountViewModel physicianAccountViewModel);
        public bool TypeOfCare(AdminDashboardViewModel adminDashboardViewModel);
        public bool HouseCall(int? id);
        public ConcludeCareViewModel GetConcludeCare(int id);
        public int ConcludeCare(ConcludeCareViewModel concludeCareViewModel);
        public bool TransferCase(AdminDashboardViewModel adminDashboardViewModel);
        public bool UpdatePhysicianLatitudeLongitude(decimal lat, decimal lng, string address);
        public PhysicianInvoicingViewModel GetPhysicianInvoicingDetails(DateTime? startdate, DateTime? enddate);
        public PhysicianTimesheetViewModel GetTimesheetDetails(DateTime? startdate, DateTime? enddate);
        public bool UpdateTimeSheet(PhysicianTimesheetViewModel physicianTimesheetViewModel);
        public Task<bool> UpdateTimeSheetReimbursement(IFormFile file, DateTime? date, int? id, string? item, int? amount, DateTime? startdate, DateTime? enddate);
        public bool DeleteTimesheetReimbursement(int id);
        public bool FinalizeTimesheet(int id);
    }
}
