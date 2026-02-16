using System.Collections.Generic;
using System.Collections.ObjectModel;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IBookService
    {
        ObservableCollection<Book> GetAllBooks();
        List<Category> GetAllCategories();
        bool AddBook(Book book);
        bool UpdateBook(Book book);
        bool DeleteBook(int bookId);

        bool BorrowBook(int userId, int bookId);
        ObservableCollection<BorrowedBook> GetUserLoans(int userId);
    }
}