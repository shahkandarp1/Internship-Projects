using HalloDoc.ViewModels;
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
    }
}
