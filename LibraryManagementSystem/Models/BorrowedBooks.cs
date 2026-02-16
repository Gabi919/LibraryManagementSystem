using System;

namespace LibraryManagementSystem.Models
{
    public class BorrowedBook
    {
        public int LoanId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public string FullImagePath { get; set; }
    }
}