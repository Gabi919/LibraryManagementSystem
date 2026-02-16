using LibraryApp.Core.Base;
using LibraryApp.ViewModels.Base;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class BookDetailViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;
        private readonly User _currentUser;

        public Book SelectedBook { get; }
        public bool IsAdmin => _currentUser.IsAdmin;

        public event Action RequestGoBack;
        public event Action<Book> RequestEditBook;

        public ICommand GoBackCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand BorrowCommand { get; }

        public BookDetailViewModel(IBookService bookService, Book book, User currentUser)
        {
            _bookService = bookService;
            SelectedBook = book;
            _currentUser = currentUser;

            GoBackCommand = new RelayCommand(_ => RequestGoBack?.Invoke());
            EditBookCommand = new RelayCommand(_ => RequestEditBook?.Invoke(SelectedBook));
            BorrowCommand = new RelayCommand(ExecuteBorrow);
        }

        private void ExecuteBorrow(object obj)
        {
            if (SelectedBook.CurrentStock <= 0)
            {
                MessageBox.Show("Stoc epuizat!");
                return;
            }

            bool success = _bookService.BorrowBook(_currentUser.UserId, SelectedBook.BookId);
            if (success)
            {
                MessageBox.Show("Carte împrumutată cu succes! Verificați secțiunea 'Cărțile mele'.");
                SelectedBook.CurrentStock--;
                OnPropertyChanged(nameof(SelectedBook));
                RequestGoBack?.Invoke();
            }
            else
            {
                MessageBox.Show("Eroare la împrumut.");
            }
        }
    }
}