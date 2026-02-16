using LibraryApp.Core.Base;
using LibraryApp.ViewModels.Base;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using System;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IUserService _userService;

        public event Action<User> LoginSuccess;
        public event Action GoToRegisterRequested;

        public string Email { get; set; }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;
            LoginCommand = new RelayCommand(ExecuteLogin);
            GoToRegisterCommand = new RelayCommand(_ => GoToRegisterRequested?.Invoke());
        }

        private void ExecuteLogin(object parameter)
        {
            var passBox = parameter as System.Windows.Controls.PasswordBox;
            var password = passBox?.Password;

            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(password))
            {
                ErrorMessage = "Introdu email și parola.";
                return;
            }

            try
            {
                var user = _userService.Authenticate(Email, password);
                if (user != null)
                {
                    LoginSuccess?.Invoke(user);
                }
                else
                {
                    ErrorMessage = "Email sau parolă incorectă.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Eroare DB: " + ex.Message;
            }
        }
    }
}