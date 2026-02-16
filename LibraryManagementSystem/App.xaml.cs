using System.Windows;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.ViewModels;
using LibraryManagementSystem.Views;

namespace LibraryManagementSystem
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string connectionString = "Host=localhost;Database=LibraryDB;Username=postgres;Password=123456";

            var bookService = new BookService(connectionString);
            var userService = new UserService(connectionString);

            var mainViewModel = new MainViewModel(bookService, userService, connectionString);

            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            mainWindow.Show();
        }
    }
}