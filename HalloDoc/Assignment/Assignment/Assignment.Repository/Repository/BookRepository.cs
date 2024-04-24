using Assignment.Data;
using Assignment.Models;
using Assignment.Models.ViewModels;
using Assignment.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Assignment.Repository.Repository
{
    public class BookRepository: IBookRepository
    {
        private readonly ApplicationDbContext _db;
        public BookRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public IndexViewModel GetAllBookData(string? search, int page = 1, int pageSize = 10)
        {
            IQueryable<Book> books = _db.Books.OrderBy(r=>r.Id);

            if(search!= null)
            {
                books = books.Where(b=>b.BookName.ToLower().Replace(" ", "").Contains(search.ToLower().Replace(" ", "")));
            }

            IndexViewModel indexViewModel = new IndexViewModel
            {
                books = books.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = books.Count(),
                TotalPages = (int)Math.Ceiling((double)books.Count() / pageSize)
            };
            return indexViewModel;

        }
        public bool CreateBook(AddBookViewModel addBookViewModel)
        {
            try
            {
                Book book = _db.Books.FirstOrDefault(b=>b.BorrowerName.ToLower().Replace(" ", "") == addBookViewModel.BorrowerName.ToLower().Replace(" ", ""));
                if(book == null)
                {
                    Borrower borrower = new Borrower
                    {
                        City = addBookViewModel.City,
                    };

                    _db.Borrowers.Add(borrower);
                    _db.SaveChanges();

                    Book bookk = new Book
                    {
                        BorrowerId = borrower.Id,
                        BookName = addBookViewModel.BookName,
                        Author = addBookViewModel.Author,
                        BorrowerName = addBookViewModel.BorrowerName,
                        DateOfIssue = addBookViewModel.DateOfIssue,
                        Genre = addBookViewModel.Genre ,
                        City = addBookViewModel.City,
                    };

                    _db.Books.Add(bookk);
                }
                else
                {

                    Borrower borrower = _db.Borrowers.FirstOrDefault(b=>b.Id == book.BorrowerId);

                    Book bookk = new Book
                    {
                        BorrowerId = borrower.Id,
                        BookName = addBookViewModel.BookName,
                        Author = addBookViewModel.Author,
                        BorrowerName = addBookViewModel.BorrowerName,
                        DateOfIssue = addBookViewModel.DateOfIssue,
                        Genre = addBookViewModel.Genre,
                        City = addBookViewModel.City,
                    };

                    _db.Books.Add(bookk);
                }
                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public AddBookViewModel GetBookDetails(int? id)
        {
            Book book = _db.Books.FirstOrDefault(b=>b.Id == id);
            if(book == null)
            {
                return null;
            }
            AddBookViewModel bookViewModel = new AddBookViewModel
            {
                BookName = book.BookName,
                Author = book.Author,
                BorrowerName = book.BorrowerName,
                DateOfIssue = book.DateOfIssue,
                Genre = book.Genre,
                City = book.City,
            };
            return bookViewModel;
        }

        public bool EditBook(AddBookViewModel addBookViewModel)
        {
            try
            {
                Book book = _db.Books.FirstOrDefault(b => b.Id == addBookViewModel.BookId);
                if (book == null)
                {
                    return false;
                }
                if(book.BorrowerName.ToLower().Replace(" ", "") != addBookViewModel.BorrowerName.ToLower().Replace(" ", ""))
                {
                    Book bookk = _db.Books.FirstOrDefault(b => b.BorrowerName.ToLower().Replace(" ", "") == addBookViewModel.BorrowerName.ToLower().Replace(" ", ""));
                    if (bookk == null)
                    {
                        Borrower borrower = new Borrower
                        {
                            City = addBookViewModel.City,
                        };

                        _db.Borrowers.Add(borrower);
                        _db.SaveChanges();

                        book.BorrowerId = borrower.Id;
                    }
                    else
                    {

                        Borrower borrower = _db.Borrowers.FirstOrDefault(b => b.Id == bookk.BorrowerId);

                        book.BorrowerId = borrower.Id;

                    }
                }

                book.BorrowerName = addBookViewModel.BorrowerName;
                book.BookName = addBookViewModel.BookName;
                book.Author = addBookViewModel.Author;
                book.Genre = addBookViewModel.Genre;
                book.City = addBookViewModel.City;
                _db.Books.Update(book);

                _db.SaveChanges();
                return true;
            }
            catch(Exception exp)
            {
                return false;
            }
        }

        public bool DeleteBook(int? id)
        {
            try
            {
                Book book = _db.Books.FirstOrDefault(b => b.Id == id);
                if (book == null)
                {
                    return false;
                }
                _db.Books.Remove(book);
                _db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
