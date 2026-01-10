using System;

namespace LibraryManagementSystem.Models
{
    public class Loan
    {
        public int LoanId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public string BookTitle { get; set; }
        public string UserFullName { get; set; }

        public bool IsOverdue
        {
            get
            {
                if (ReturnDate.HasValue) return false;
                return DateTime.Now > DueDate;
            }
        }
    }
}