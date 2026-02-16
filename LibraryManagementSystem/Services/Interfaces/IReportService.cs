using System.Collections.Generic;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IReportService
    {
        List<OverdueReportItem> GetOverdueLoans();
        List<PopularityReportItem> GetTopBooks();
        List<LowStockReportItem> GetLowStockBooks();
        void ExportToPdf<T>(List<T> data, string reportTitle, string filePath);
    }
}