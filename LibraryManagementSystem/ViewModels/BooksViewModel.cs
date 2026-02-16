using LibraryApp.Core.Base;
using LibraryApp.ViewModels.Base;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class BooksViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;

        public bool IsAdmin { get; }
        public event Action<Book> OpenDetailsRequested;
        public event Action RequestAddBook;

        public ObservableCollection<Book> Books { get; set; }

        public ICommand OpenDetailsCommand { get; }
        public ICommand AddBookCommand { get; }
        public ICommand DeleteBookCommand { get; }

        public BooksViewModel(IBookService bookService, bool isAdmin)
        {
            _bookService = bookService;
            IsAdmin = isAdmin;
            Books = new ObservableCollection<Book>();

            OpenDetailsCommand = new RelayCommand(param =>
            {
                if (param is Book book) OpenDetailsRequested?.Invoke(book);
            });

            AddBookCommand = new RelayCommand(_ => RequestAddBook?.Invoke());

            DeleteBookCommand = new RelayCommand(param =>
            {
                if (param is Book book)
                {
                    _bookService.DeleteBook(book.BookId);
                    LoadData();
                }
            });

            LoadData();
        }

        private void LoadData()
        {
            var list = _bookService.GetAllBooks();
            Books.Clear();
            foreach (var item in list) Books.Add(item);
        }
    }
}