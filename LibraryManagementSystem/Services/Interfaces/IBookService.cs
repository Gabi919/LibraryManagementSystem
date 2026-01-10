using System.Collections.Generic;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IBookService
    {
        List<Book> GetAllBooks();
        void AddBook(Book book);
        void DeleteBook(int bookId);
        List<Category> GetAllCategories();
    }
}