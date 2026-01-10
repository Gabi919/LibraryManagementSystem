using System.Collections.Generic;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface ILoanService
    {
        List<Loan> GetActiveLoans();
        void BorrowBook(int userId, int bookId);
        void ReturnBook(int loanId);
    }
}