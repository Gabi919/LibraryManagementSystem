using System.Configuration;
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

            string connString = ConfigurationManager.ConnectionStrings["LibraryConnection"].ConnectionString;

            var bookService = new BookService(connString);
            var mainViewModel = new MainViewModel(bookService);

            MainWindow window = new MainWindow();
            window.DataContext = mainViewModel;
            window.Show();
        }
    }
}