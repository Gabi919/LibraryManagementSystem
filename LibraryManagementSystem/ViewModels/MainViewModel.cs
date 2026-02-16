using LibraryApp.Core.Base;
using LibraryApp.ViewModels.Base;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Services.Interfaces;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly string _connectionString;
        private User _currentUser;

        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetProperty(ref _isLoggedIn, value);
        }

        public bool IsUserAdmin => _currentUser != null && _currentUser.IsAdmin;

        public ICommand GoToHomeCommand { get; }
        public ICommand GoToBooksCommand { get; }
        public ICommand GoToMyLoansCommand { get; }
        public ICommand GoToReportsCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel(IBookService bookService, IUserService userService, string connectionString)
        {
            _bookService = bookService;
            _userService = userService;
            _connectionString = connectionString;

            GoToHomeCommand = new RelayCommand(_ => CurrentViewModel = new HomeViewModel(_currentUser, _bookService));
            GoToBooksCommand = new RelayCommand(_ => ShowBooksList());
            GoToMyLoansCommand = new RelayCommand(_ => ShowUserLoans());
            GoToReportsCommand = new RelayCommand(_ => ShowReports());
            LogoutCommand = new RelayCommand(_ => ShowLogin());

            ShowLogin();
        }

        private void ShowUserLoans()
        {
            if (_currentUser == null) return;
            CurrentViewModel = new UserLoansViewModel(_bookService, _currentUser.UserId);
        }

        private void ShowReports()
        {
            var reportService = new ReportService(_connectionString);
            CurrentViewModel = new ReportsViewModel(reportService);
        }

        private void ShowBooksList()
        {
            bool isAdmin = _currentUser != null && _currentUser.IsAdmin;
            var booksVm = new BooksViewModel(_bookService, isAdmin);

            booksVm.OpenDetailsRequested += (selectedBook) => ShowBookDetails(selectedBook);
            booksVm.RequestAddBook += () => ShowBookEditor(null);

            CurrentViewModel = booksVm;
        }

        private void ShowBookDetails(Book book)
        {
            var detailsVm = new BookDetailViewModel(_bookService, book, _currentUser);

            detailsVm.RequestGoBack += () => ShowBooksList();
            detailsVm.RequestEditBook += (bookToEdit) => ShowBookEditor(bookToEdit);

            CurrentViewModel = detailsVm;
        }

        private void ShowBookEditor(Book bookToEdit)
        {
            var editorVm = new BookEditorViewModel(_bookService, bookToEdit);
            editorVm.RequestGoBack += () => ShowBooksList();
            CurrentViewModel = editorVm;
        }

        private void ShowLogin()
        {
            IsLoggedIn = false;
            _currentUser = null;
            OnPropertyChanged(nameof(IsUserAdmin));

            var loginVm = new LoginViewModel(_userService);

            loginVm.LoginSuccess += (user) =>
            {
                _currentUser = user;
                IsLoggedIn = true;
                OnPropertyChanged(nameof(IsUserAdmin));
                CurrentViewModel = new HomeViewModel(user, _bookService);
            };

            loginVm.GoToRegisterRequested += () => ShowRegister();
            CurrentViewModel = loginVm;
        }

        private void ShowRegister()
        {
            IsLoggedIn = false;
            var regVm = new RegisterViewModel(_userService);
            regVm.RequestNavigateToLogin += () => ShowLogin();
            regVm.RegistrationSuccess += (user) =>
            {
                _currentUser = user;
                IsLoggedIn = true;
                OnPropertyChanged(nameof(IsUserAdmin));
                CurrentViewModel = new HomeViewModel(user, _bookService);
            };
            CurrentViewModel = regVm;
        }
    }
}