using System.Collections.ObjectModel;
using LibraryApp.Core.Base;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;

namespace LibraryManagementSystem.ViewModels
{
    public class UserLoansViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;
        private readonly int _userId;

        public ObservableCollection<BorrowedBook> Loans { get; set; }

        public UserLoansViewModel(IBookService bookService, int userId)
        {
            _bookService = bookService;
            _userId = userId;
            Loans = new ObservableCollection<BorrowedBook>();
            LoadLoans();
        }

        private void LoadLoans()
        {
            var items = _bookService.GetUserLoans(_userId);
            Loans.Clear();
            foreach (var item in items) Loans.Add(item);
        }
    }
}