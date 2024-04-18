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
        private readonly IAdmin _admin;
        private readonly IHttpContextAccessor _context;

        public AgreementController(ApplicationDbContext db, IHttpContextAccessor context, IAdmin admin)
        {
            _context = context;
            _admin = admin;
        }
        /// <summary>
        /// It is a Get method of Agreement Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Index(int id)
        {
            var request_status = _admin.GetRequest(id);
            if (request_status == null || request_status.Status != 2 )
            {
                return NotFound();
            }

            return View(request_status);
            
        }
        /// <summary>
        /// It will agree the agreement
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Agree(int id)
        {
            bool isAgreed = _admin.Agree(id);
            if (isAgreed)
            {
                TempData["success"] = "Agreement Agreed Successfully!!";
            }
            else
            {
                TempData["error"] = "Agreement could not be agreed!!";
            }
            return Json(new { isAgreed = isAgreed });
        }
        /// <summary>
        /// It will disagree the agreement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public IActionResult Disagree(int id,string notes)
        {
            bool isDiasagreed = _admin.Disagree(id,notes);
            if (isDiasagreed)
            {
                TempData["success"] = "Agreement Disagreed!!";
            }
            else
            {
                TempData["error"] = "Agreement could not be disagreed!!";
            }
            return Json(new { isAgreed = isDiasagreed });
        }
    }
}
