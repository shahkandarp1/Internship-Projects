using Assignment.Models.ViewModels;
using Assignment.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookRepository _book;
        public BookController(IBookRepository book)
        {
            _book = book;
        } 
        /// <summary>
        /// It is a get method of Index page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// It will return filtered and paginated data
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult GetAllBookData(string? search,int page = 1,int pageSize = 10)
        {
            IndexViewModel indexViewModel = _book.GetAllBookData(search,page,pageSize);
            return PartialView("_BookTable", indexViewModel);
        }
        /// <summary>
        /// It will open addbook pop up
        /// </summary>
        /// <returns></returns>
        public IActionResult AddBook()
        {
            AddBookViewModel addBookViewModel = new AddBookViewModel();
            return PartialView("_CreateBookModel", addBookViewModel);
        }
        /// <summary>
        /// It is post method of add book and will create new book
        /// </summary>
        /// <param name="addBookViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        
        public IActionResult AddBook(AddBookViewModel addBookViewModel)
        {
            bool isAddded = _book.CreateBook(addBookViewModel);
            if(isAddded)
            {
                TempData["success"] = "New Entry Created Successfully!!";
            }
            else
            {
                TempData["error"] = "Entry could not be Created!!";
            }
            return RedirectToAction("Index");
        }
        /// <summary>
        /// It will update details specified in edit book pop up
        /// </summary>
        /// <param name="addBookViewModel"></param>
        /// <returns></returns>
        public IActionResult EditBook(AddBookViewModel addBookViewModel)
        {
            bool isEditted = _book.EditBook(addBookViewModel);
            if (isEditted)
            {
                TempData["success"] = "Entry Editted Successfully!!";
            }
            else
            {
                TempData["error"] = "Entry could not be Editted!!";
            }
            return RedirectToAction("Index");
        }
        /// <summary>
        /// It will open edit book pop up
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetBookDetails(int? id)
        {
            AddBookViewModel addBookViewModel = _book.GetBookDetails(id);
            if(addBookViewModel == null)
            {
                return Json(new { canOpen = false });
            }
            return PartialView("_CreateBookModel", addBookViewModel);
        }
        /// <summary>
        /// it will delete specified book entry
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteBook(int? id)
        {
            bool isDeleted = _book.DeleteBook(id);
            if (isDeleted)
            {
                TempData["success"] = "Entry Deleted Successfully!!";
            }
            else
            {
                TempData["error"] = "Entry could not be Deleted!!";
            }
            return RedirectToAction("Index");
        }
    }
}
