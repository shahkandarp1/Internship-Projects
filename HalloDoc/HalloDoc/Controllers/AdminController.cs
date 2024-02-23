using ClosedXML.Excel;
using HalloDoc.Repository.Interface;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

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

        public IActionResult ExportAll()
        {
            try
            {
                List<Request> data = new List<Request>();
                data = _db.Requests.Include(r=>r.RequestClient).Include(r=>r.Physician).ToList();
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Export All");


                worksheet.Cell(1, 1).Value = "Name";
                worksheet.Cell(1, 2).Value = "Date Of Birth";
                worksheet.Cell(1, 3).Value = "Requestor";
                worksheet.Cell(1, 4).Value = "Physician Name";
                worksheet.Cell(1, 5).Value = "Date of Service";
                worksheet.Cell(1, 6).Value = "Requested Date";
                worksheet.Cell(1, 7).Value = "Phone Number";
                worksheet.Cell(1, 8).Value = "Address";
                worksheet.Cell(1, 9).Value = "Notes";

                int row = 2;
                foreach (var item in data)
                {
                    var statusClass = "";
                    var dos = "";
                    var notes = "";
                    if (item.RequestTypeId == 1)
                    {
                        statusClass = "business";
                    }
                    else if (item.RequestTypeId == 3)
                    {
                        statusClass = "family";
                    }
                    else if (item.RequestTypeId == 2)
                    {
                        statusClass = "patient";
                    }
                    else
                    {
                        statusClass = "concierge";
                    }
                    foreach (var stat in item.RequestStatusLogs)
                    {
                        if (stat.Status == 2)
                        {
                            dos = stat.CreatedDate.ToString("MMMM dd,yyyy");
                            notes = stat.Notes ?? "";
                        }
                    }
                    worksheet.Cell(row, 1).Value = string.Concat(item.RequestClient.FirstName + ',' + item.RequestClient.LastName);
                    worksheet.Cell(row, 2).Value = DateTime.Parse($"{item.RequestClient.IntYear}-{item.RequestClient.StrMonth}-{item.RequestClient.IntDate}").ToString("MMMM dd,yyyy");
                    worksheet.Cell(row, 3).Value = statusClass.Substring(0, 1).ToUpper() + statusClass.Substring(1).ToLower() + " " + item.FirstName + item.LastName;
                    worksheet.Cell(row, 4).Value = ("Dr." + item?.Physician == null ? "" : item?.Physician?.FirstName);
                    worksheet.Cell(row, 5).Value = item.CreatedDate.ToString("MMMM dd,yyyy");
                    worksheet.Cell(row, 6).Value = dos;
                    worksheet.Cell(row, 7).Value = item.RequestClient.PhoneNumber + "(Patient)" + (item.RequestTypeId != 2 ? item.PhoneNumber + "(" + statusClass.Substring(0, 1).ToUpper() + statusClass.Substring(1).ToLower() + ")" : "") ;
                    worksheet.Cell(row, 8).Value = (item.RequestClient.Address != null ? string.Concat(item.RequestClient.Address, ',', item.RequestClient.Street, ',', item.RequestClient.City, ',', item.RequestClient.State, ',', item.RequestClient.ZipCode) : string.Concat(item.RequestClient.Street, ',', item.RequestClient.City, ',', item.RequestClient.State, ',', item.RequestClient.ZipCode));
                    worksheet.Cell(row, 9).Value = notes;
                    row++;
                }
                worksheet.Columns().AdjustToContents();

                var memoryStream = new MemoryStream();
                workbook.SaveAs(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "All Data.xlsx");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Dashboard(AdminDashboardViewModel model)
        {
            try
            {
                List<Request> data = new List<Request>();
                data = model.requests;
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Export All");

                int count = 1;
            worksheet.Cell(1, count++).Value = "Name";
            if(model.status != "Unpaid")
            {
                    worksheet.Cell(1, count++).Value = "Date Of Birth";
            }
            if(model.status == "New" || model.status == "Pending" || model.status == "Active")
            {
                    worksheet.Cell(1, count++).Value = "Requestor";
            }
            if(model.status != "New")
            {
                    worksheet.Cell(1, count++).Value = "Physician Name";
                    worksheet.Cell(1, count++).Value = "Date Of Service";
            }
            if(model.status == "New")
            {
                    worksheet.Cell(1, count++).Value = "Requested Date";
            }
            if(model.status != "ToClose")
            {
                    worksheet.Cell(1, count++).Value = "Phone";
            }
                worksheet.Cell(1, count++).Value = "Address";
            if(model.status != "Conclude" || model.status != "Unpaid")
            {
                    worksheet.Cell(1, count++).Value = "Notes";
            }

                int row = 2;
                foreach (var item in data)
                {
                    count = 1;
                    var statusClass = "";
                    var dos = "";
                    var notes = "";
                    if (item.RequestTypeId == 1)
                    {
                        statusClass = "business";
                    }
                    else if (item.RequestTypeId == 3)
                    {
                        statusClass = "family";
                    }
                    else if (item.RequestTypeId == 2)
                    {
                        statusClass = "patient";
                    }
                    else
                    {
                        statusClass = "concierge";
                    }
                    foreach (var stat in item.RequestStatusLogs)
                    {
                        if (stat.Status == 2)
                        {
                            dos = stat.CreatedDate.ToString("MMMM dd,yyyy");
                            notes = stat.Notes ?? "";
                        }
                    }
                    worksheet.Cell(row, count++).Value = string.Concat(item.RequestClient.FirstName, ',', item.RequestClient.LastName);
                if(model.status != "Unpaid")
                {

                    DateTime now = DateTime.Today;
                    int age = now.Year - DateTime.Parse($"{item.RequestClient.IntYear}-{item.RequestClient.StrMonth}-{item.RequestClient.IntDate}").Year;
                    if (DateTime.Parse($"{item.RequestClient.IntYear}-{item.RequestClient.StrMonth}-{item.RequestClient.IntDate}") > now.AddYears(-age))
                        age--;

                        worksheet.Cell(row, count++).Value = DateTime.Parse($"{item.RequestClient.IntYear}-{item.RequestClient.StrMonth}-{item.RequestClient.IntDate}").ToString("MMMM dd,yyyy") + "(" + age + ")"; 
                }
                if(model.status == "New" || model.status == "Pending" || model.status == "Active")
                {
                        worksheet.Cell(row, count++).Value = statusClass.Substring(0, 1).ToUpper() + statusClass.Substring(1).ToLower() + " " + string.Concat(item.FirstName, ',', item.LastName);
                }
                if(model.status != "New")
                {
                        worksheet.Cell(row, count++).Value = "Dr." + item.Physician == null ? "" : item.Physician.FirstName; 
                }
                if(model.status == "New")
                {

                    int hoursDifference = (int)(DateTime.Now - item.CreatedDate).TotalHours;
                    int minutesDifference = (DateTime.Now - item.CreatedDate).Minutes;

                    worksheet.Cell(row, count++).Value = item.CreatedDate.ToString("MMMM dd,yyyy") + " " + hoursDifference + "H " + minutesDifference + "M"; 
                }
                if(model.status != "New")
                {
                        worksheet.Cell(row, count++).Value = dos; 
                }
                if(model.status != "ToClose")
                {
                        worksheet.Cell(row, count++).Value = item.RequestClient.PhoneNumber + "(Patient)" + (item.RequestTypeId != 2 ? item.PhoneNumber + "(" + statusClass.Substring(0, 1).ToUpper() + statusClass.Substring(1).ToLower() + ")" : "");
                }
                worksheet.Cell(row, count++).Value = (item.RequestClient.Address != null ? string.Concat(item.RequestClient.Address, ',', item.RequestClient.Street, ',', item.RequestClient.City, ',', item.RequestClient.State, ',', item.RequestClient.ZipCode) : string.Concat(item.RequestClient.Street, ',', item.RequestClient.City, ',', item.RequestClient.State, ',', item.RequestClient.ZipCode));
                if(model.status != "Conclude" || model.status != "Unpaid")
                {
                        worksheet.Cell(row, count++).Value = "Admin transferred to Dr.AGOLA on 10\\10\\2023 at 4:11:38 AM:test";
                }
                    row++;
                }
                worksheet.Columns().AdjustToContents();

                var memoryStream = new MemoryStream();
                workbook.SaveAs(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Data-{model.status}.xlsx");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        public IActionResult ViewCase(int id)
        {
            var req = _db.Requests.Include(r => r.RequestClient).FirstOrDefault(r=>r.RequestId == id);
            var region = _db.Regions.FirstOrDefault(r => r.RegionId == req.RequestClient.RegionId);
            ViewCaseViewModel viewCaseViewModel = new ViewCaseViewModel()
            {
                id = id,
                FirstName = req.RequestClient.FirstName,
                LastName = req.RequestClient.LastName,
                DateOfBirth = DateTime.Parse($"{req.RequestClient.IntYear}-{req.RequestClient.StrMonth}-{req.RequestClient.IntDate}"),
                PhoneNumber = req.RequestClient.PhoneNumber,
                Email = req.RequestClient.Email,
                Region = region.Name,
                Address = string.Concat(req.RequestClient.Street, ',', req.RequestClient.City, ',', req.RequestClient.State, ',', req.RequestClient.ZipCode),
                requests = req
            };

            return View(viewCaseViewModel);
        }

    }
}
