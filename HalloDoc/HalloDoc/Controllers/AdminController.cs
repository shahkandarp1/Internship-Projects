using HalloDoc.Repository.Interface;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdmin _admin;
        private readonly ApplicationDbContext _db;

        public AdminController(IAdmin admin,ApplicationDbContext db)
        {
            _admin = admin;
            _db = db;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            var count_new = _db.Requests.Count(r => r.Status == 1);
            var count_pending = _db.Requests.Count(r => r.Status == 2);
            var count_active = _db.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _db.Requests.Count(r => r.Status == 5);
            var count_toclose = _db.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _db.Requests.Count(r => r.Status == 9);

            IQueryable<Request> _new = _db.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 1).OrderByDescending(e => e.CreatedDate);

        AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                requests = _new.ToList(),
                regions = _db.Regions.ToList(),
                status = "New",
            };
            
            return View(adminDashboardViewModel);
        }

        public async Task<IActionResult> New(string? search,string ?requestor,int? region)
        {

            IQueryable<Request> _new = _db.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 1).OrderByDescending(e => e.CreatedDate);

            var count_new = _db.Requests.Count(r => r.Status == 1);
            var count_pending = _db.Requests.Count(r => r.Status == 2);
            var count_active = _db.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _db.Requests.Count(r => r.Status == 5);
            var count_toclose = _db.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _db.Requests.Count(r => r.Status == 9);

            if(search!=null)
            {
                _new = _new.Where(r => r.RequestClient.FirstName.Contains(search) || r.RequestClient.LastName.Contains(search)); 
            }

            if(requestor == "Family")
            {
                _new = _new.Where(r => r.RequestTypeId == 3 );
            }

            if(requestor == "Business")
            {
                _new = _new.Where(r => r.RequestTypeId == 1 );
            }

            if(requestor == "Concierge")
            {
                _new = _new.Where(r => r.RequestTypeId == 4);
            }

            if(requestor == "Patient")
            {
                _new = _new.Where(r => r.RequestTypeId == 2);
            }
            if(region!=null && region!=-1)
            {
                _new = _new.Where(r => r.RequestClient.RegionId == region);

            }

            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                requests = await _new.ToListAsync(),
                regions = _db.Regions.ToList(),
                status = "New",
            };
            return PartialView("_AdminDashboardTable",adminDashboardViewModel);
        }

        public IActionResult Pending(string? search, string? requestor, int? region)
        {
            var count_new = _db.Requests.Count(r => r.Status == 1);
            var count_pending = _db.Requests.Count(r => r.Status == 2);
            var count_active = _db.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _db.Requests.Count(r => r.Status == 5);
            var count_toclose = _db.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _db.Requests.Count(r => r.Status == 9);

            IQueryable<Request> _pending;
            _pending = _db.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 2).OrderByDescending(e => e.CreatedDate);

            if (search != null)
            {
                _pending = _pending.Where(r => r.RequestClient.FirstName.Contains(search) || r.RequestClient.LastName.Contains(search));
            }

            if (requestor == "Family")
            {
                _pending = _pending.Where(r => r.RequestTypeId == 3);
            }

            if (requestor == "Business")
            {
                _pending = _pending.Where(r => r.RequestTypeId == 1);
            }

            if (requestor == "Concierge")
            {
                _pending = _pending.Where(r => r.RequestTypeId == 4);
            }

            if (requestor == "Patient")
            {
                _pending = _pending.Where(r => r.RequestTypeId == 2);
            }
            if (region != null && region != -1)
            {
                _pending = _pending.Where(r => r.RequestClient.RegionId == region);

            }


            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                requests = _pending.ToList(),
                regions = _db.Regions.ToList(),
                status = "Pending",
            };
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }

        public IActionResult Active(string? search, string? requestor, int? region)
        {
            IQueryable<Request> _active;
            _active = _db.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 3 || r.Status == 4).OrderByDescending(e => e.CreatedDate);
            var count_new = _db.Requests.Count(r => r.Status == 1);
            var count_pending = _db.Requests.Count(r => r.Status == 2);
            var count_active = _db.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _db.Requests.Count(r => r.Status == 5);
            var count_toclose = _db.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _db.Requests.Count(r => r.Status == 9);

            if (search != null)
            {
                _active = _active.Where(r => r.RequestClient.FirstName.Contains(search) || r.RequestClient.LastName.Contains(search));
            }

            if (requestor == "Family")
            {
                _active = _active.Where(r => r.RequestTypeId == 3);
            }

            if (requestor == "Business")
            {
                _active = _active.Where(r => r.RequestTypeId == 1);
            }

            if (requestor == "Concierge")
            {
                _active = _active.Where(r => r.RequestTypeId == 4);
            }

            if (requestor == "Patient")
            {
                _active = _active.Where(r => r.RequestTypeId == 2);
            }
            if (region != null && region != -1)
            {
                _active = _active.Where(r => r.RequestClient.RegionId == region);

            }


            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                requests = _active.ToList(),
                regions = _db.Regions.ToList(),
                status = "Active",
            };
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }

        public IActionResult Conclude(string? search, string? requestor, int? region)
        {
            var count_new = _db.Requests.Count(r => r.Status == 1);
            var count_pending = _db.Requests.Count(r => r.Status == 2);
            var count_active = _db.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _db.Requests.Count(r => r.Status == 5);
            var count_toclose = _db.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _db.Requests.Count(r => r.Status == 9);

            IQueryable<Request> _conclude;
            _conclude = _db.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 5).OrderByDescending(e => e.CreatedDate);

            if (search != null)
            {
                _conclude = _conclude.Where(r => r.RequestClient.FirstName.Contains(search) || r.RequestClient.LastName.Contains(search));
            }

            if (requestor == "Family")
            {
                _conclude = _conclude.Where(r => r.RequestTypeId == 3);
            }

            if (requestor == "Business")
            {
                _conclude = _conclude.Where(r => r.RequestTypeId == 1);
            }

            if (requestor == "Concierge")
            {
                _conclude = _conclude.Where(r => r.RequestTypeId == 4);
            }

            if (requestor == "Patient")
            {
                _conclude = _conclude.Where(r => r.RequestTypeId == 2);
            }
            if (region != null && region != -1)
            {
                _conclude = _conclude.Where(r => r.RequestClient.RegionId == region);

            }

            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                requests = _conclude.ToList(),
                regions = _db.Regions.ToList(),
                status = "Conclude",
            };
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }

        public IActionResult Close(string? search, string? requestor, int? region)
        {
            var count_new = _db.Requests.Count(r => r.Status == 1);
            var count_pending = _db.Requests.Count(r => r.Status == 2);
            var count_active = _db.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _db.Requests.Count(r => r.Status == 5);
            var count_toclose = _db.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _db.Requests.Count(r => r.Status == 9);

            IQueryable<Request> _toclose;
            _toclose = _db.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 6 || r.Status == 7 || r.Status == 8).OrderByDescending(e => e.CreatedDate);

            if (search != null)
            {
                _toclose = _toclose.Where(r => r.RequestClient.FirstName.Contains(search) || r.RequestClient.LastName.Contains(search));
            }

            if (requestor == "Family")
            {
                _toclose = _toclose.Where(r => r.RequestTypeId == 3);
            }

            if (requestor == "Business")
            {
                _toclose = _toclose.Where(r => r.RequestTypeId == 1);
            }

            if (requestor == "Concierge")
            {
                _toclose = _toclose.Where(r => r.RequestTypeId == 4);
            }

            if (requestor == "Patient")
            {
                _toclose = _toclose.Where(r => r.RequestTypeId == 2);
            }
            if (region != null && region != -1)
            {
                _toclose = _toclose.Where(r => r.RequestClient.RegionId == region);

            }

            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                requests = _toclose.ToList(),
                regions = _db.Regions.ToList(),
                status = "ToClose",
            };
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }

        public IActionResult Unpaid(string? search, string? requestor, int? region)
        {
            var count_new = _db.Requests.Count(r => r.Status == 1);
            var count_pending = _db.Requests.Count(r => r.Status == 2);
            var count_active = _db.Requests.Count(r => r.Status == 3 || r.Status == 4);
            var count_conclude = _db.Requests.Count(r => r.Status == 5);
            var count_toclose = _db.Requests.Count(r => r.Status == 6 || r.Status == 7 || r.Status == 8);
            var count_unpaid = _db.Requests.Count(r => r.Status == 9);

            IQueryable<Request> _unpaid = _db.Requests.Include(r => r.RequestClient).Include(r => r.Physician).Include(r => r.RequestStatusLogs).Where(r => r.Status == 9).OrderByDescending(e => e.CreatedDate);

            if (search != null)
            {
                _unpaid = _unpaid.Where(r => r.RequestClient.FirstName.Contains(search) || r.RequestClient.LastName.Contains(search));
            }

            if (requestor == "Family")
            {
                _unpaid = _unpaid.Where(r => r.RequestTypeId == 3);
            }

            if (requestor == "Business")
            {
                _unpaid = _unpaid.Where(r => r.RequestTypeId == 1);
            }

            if (requestor == "Concierge")
            {
                _unpaid = _unpaid.Where(r => r.RequestTypeId == 4);
            }

            if (requestor == "Patient")
            {
                _unpaid = _unpaid.Where(r => r.RequestTypeId == 2);
            }
            if (region != null && region != -1)
            {
                _unpaid = _unpaid.Where(r => r.RequestClient.RegionId == region);

            }

            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel
            {
                new_count = count_new,
                pending_count = count_pending,
                active_count = count_active,
                conclude_count = count_conclude,
                toclose_count = count_toclose,
                requests = _unpaid.ToList(),
                regions = _db.Regions.ToList(),
                status = "Unpaid",
            };
            return PartialView("_AdminDashboardTable", adminDashboardViewModel);
        }

    }
}
