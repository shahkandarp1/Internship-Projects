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
            var request_status = _db.RequestStatusLogs.FirstOrDefault(u=>u.RequestId == id);
            if(request_status.Status != 2)
            {
                return NotFound();
            }
            var request = _db.Requests.Include(u=>u.RequestClient).FirstOrDefault(u => u.RequestId == id);

            return View(request);
            
        }

        public IActionResult Agree(int id)
        {
            var request_status = _db.RequestStatusLogs.FirstOrDefault(u => u.RequestId == id);
            request_status.Status = 3;
            _db.RequestStatusLogs.Update(request_status);
            _db.SaveChanges();
            return Json(new { isAgreed = true });
        }

        public IActionResult Disagree(int id,string notes)
        {
            var request_status = _db.RequestStatusLogs.FirstOrDefault(u => u.RequestId == id);
            request_status.Status = 5;
            request_status.Notes = notes;
            _db.RequestStatusLogs.Update(request_status);
            _db.SaveChanges();
            return Json(new { isAgreed = true });
        }
    }
}
