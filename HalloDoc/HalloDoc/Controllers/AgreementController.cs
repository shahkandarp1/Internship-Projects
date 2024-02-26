using Microsoft.AspNetCore.Mvc;
using HalloDoc.ViewModels;
using HalloDoc;
using HaloDocMVC.NET.Controllers;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.Controllers
{
    public class AgreementController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _context;

        public AgreementController( ApplicationDbContext db, IHttpContextAccessor context)
        {
            _db = db;
            _context = context;
        }
        public IActionResult Index(int id)
        {
            var request_status = _db.Requests.FirstOrDefault(u=>u.RequestId == id);
            if(request_status.Status != 2)
            {
                return NotFound();
            }
            var request = _db.Requests.Include(u=>u.RequestClient).FirstOrDefault(u => u.RequestId == id);

            return View(request);
            
        }

        public IActionResult Agree(int id)
        {
            var request = _db.Requests.FirstOrDefault(u => u.RequestId == id);
            request.Status = 3;
            _db.Requests.Update(request);

            RequestStatusLog requestStatusLog = new RequestStatusLog
            {
                Status = 3,
                RequestId = id,
                CreatedDate = DateTime.Now,
            };
            _db.RequestStatusLogs.Add(requestStatusLog);
            _db.SaveChanges();
            return Json(new { isAgreed = true });
        }

        public IActionResult Disagree(int id,string notes)
        {
            var request = _db.Requests.FirstOrDefault(u => u.RequestId == id);
            request.Status = 7;
            _db.Requests.Update(request);

            RequestStatusLog requestStatusLog = new RequestStatusLog
            {
                Status = 7,
                RequestId = id,
                CreatedDate = DateTime.Now,
                Notes = notes
            };
            _db.RequestStatusLogs.Add(requestStatusLog);
            _db.SaveChanges();
            return Json(new { isAgreed = true });
        }
    }
}
