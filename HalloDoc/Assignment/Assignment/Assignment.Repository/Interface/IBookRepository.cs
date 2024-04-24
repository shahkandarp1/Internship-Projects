using Assignment.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Repository.Interface
{
    public interface IBookRepository
    {
        public bool CreateBook(AddBookViewModel addBookViewModel);
        public IndexViewModel GetAllBookData(string? search, int page = 1, int pageSize = 10);
        public AddBookViewModel GetBookDetails(int? id);
        public bool EditBook(AddBookViewModel addBookViewModel);
        public bool DeleteBook(int? id);
    }
}
