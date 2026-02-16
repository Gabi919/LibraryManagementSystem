using System;

namespace LibraryManagementSystem.Models
{
    public class OverdueReportItem
    {
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string BookTitle { get; set; }
        public DateTime DueDate { get; set; }
        public int DaysOverdue { get; set; }
    }

    public class PopularityReportItem
    {
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public int BorrowCount { get; set; }
        public int CurrentStock { get; set; }
        public int TotalStock { get; set; }
    }

    public class LowStockReportItem
    {
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public int CurrentStock { get; set; }
        public int TotalStock { get; set; }
    }
}