using Microsoft.AspNetCore.Mvc;
using HalloDoc.ViewModels;
using HalloDoc;
using HaloDocMVC.NET.Controllers;
using Microsoft.EntityFrameworkCore;
using HalloDoc.Repository.Interface;

namespace HalloDoc.Controllers
{
    public class AgreementController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IAdmin _admin;
        private readonly IHttpContextAccessor _context;

        public AgreementController( ApplicationDbContext db, IHttpContextAccessor context, IAdmin admin)
        {
            _db = db;
            _context = context;
            _admin = admin;
        }
        public IActionResult Index(int id)
        {
            var request_status = _admin.getRequest(id);
            if(request_status.Status != 2)
            {
                return NotFound();
            }
            var request = _db.Requests.Include(u=>u.RequestClient).FirstOrDefault(u => u.RequestId == id);

            return View(request);
            
        }

        public IActionResult Agree(int id)
        {
            bool isAgreed = _admin.agree(id);
            return Json(new { isAgreed = isAgreed });
        }

        public IActionResult Disagree(int id,string notes)
        {
            bool isDiasagreed = _admin.disagree(id,notes);
            return Json(new { isAgreed = isDiasagreed });
        }
    }
}
