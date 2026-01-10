using LibraryApp.ViewModels.Base;
using LibraryApp.Core.Base;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;

        // Collection for the UI list
        public ObservableCollection<Book> Books { get; set; }

        private Book _selectedBook;
        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                SetProperty(ref _selectedBook, value);
                // Notify the command to re-evaluate if it can execute
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        // Commands
        public ICommand DeleteCommand { get; }

        // Constructor requiring the Service (Dependency Injection)
        public MainViewModel(IBookService bookService)
        {
            _bookService = bookService;
            Books = new ObservableCollection<Book>();

            // Initialize Command: Action to execute, Condition to execute
            DeleteCommand = new RelayCommand(ExecuteDeleteBook, CanDeleteBook);

            LoadData();
        }

        private void LoadData()
        {
            var booksFromDb = _bookService.GetAllBooks();
            Books.Clear();
            foreach (var book in booksFromDb)
            {
                Books.Add(book);
            }
        }

        private bool CanDeleteBook(object parameter)
        {
            return SelectedBook != null;
        }

        private void ExecuteDeleteBook(object parameter)
        {
            if (SelectedBook != null)
            {
                // 1. Delete from Database
                _bookService.DeleteBook(SelectedBook.BookId);

                // 2. Remove from UI List
                Books.Remove(SelectedBook);

                // 3. Reset Selection
                SelectedBook = null;
            }
        }
    }
}