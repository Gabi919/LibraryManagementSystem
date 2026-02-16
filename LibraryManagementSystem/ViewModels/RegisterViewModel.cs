using LibraryApp.Core.Base;
using LibraryApp.ViewModels.Base;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using System;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly IUserService _userService;

        public event Action<User> RegistrationSuccess;
        public event Action RequestNavigateToLogin;

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private string _successMessage;
        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }

        public RegisterViewModel(IUserService userService)
        {
            _userService = userService;
            RegisterCommand = new RelayCommand(ExecuteRegister);
            GoToLoginCommand = new RelayCommand(_ => RequestNavigateToLogin?.Invoke());
        }

        private void ExecuteRegister(object parameter)
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Completează toate câmpurile obligatorii!";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Parolele nu coincid!";
                return;
            }

            try
            {
                bool success = _userService.RegisterUser(FirstName, LastName, Email, Password, Phone);

                if (success)
                {
                    var user = _userService.Authenticate(Email, Password);

                    if (user != null)
                    {
                        SuccessMessage = "Cont creat! Te logăm automat...";
                        RegistrationSuccess?.Invoke(user);
                    }
                }
                else
                {
                    ErrorMessage = "Acest email este deja înregistrat.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Eroare: " + ex.Message;
            }
        }
    }
}