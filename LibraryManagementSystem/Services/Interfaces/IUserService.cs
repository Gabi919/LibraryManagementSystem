using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
        User Authenticate(string email, string password);

        bool RegisterUser(string firstName, string lastName, string email, string password, string phone);
    }
}