using System;
using System.Collections.Generic;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using Npgsql;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LibraryManagementSystem.Services
{
    public class ReportService : IReportService
    {
        private readonly string _connectionString;

        public ReportService(string connectionString)
        {
            _connectionString = connectionString;

            QuestPDF.Settings.License = LicenseType.Community;
        }

        public List<OverdueReportItem> GetOverdueLoans()
        {
            var list = new List<OverdueReportItem>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT u.FirstName || ' ' || u.LastName, u.Phone, b.Title, l.DueDate
                    FROM Loans l
                    JOIN Users u ON l.UserId = u.UserId
                    JOIN Books b ON l.BookId = b.BookId
                    WHERE l.DueDate < NOW() AND l.ReturnDate IS NULL
                    ORDER BY l.DueDate ASC";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dueDate = reader.GetDateTime(3);
                        list.Add(new OverdueReportItem
                        {
                            UserName = reader.GetString(0),
                            Phone = reader.IsDBNull(1) ? "-" : reader.GetString(1),
                            BookTitle = reader.GetString(2),
                            DueDate = dueDate,
                            DaysOverdue = (DateTime.Now - dueDate).Days
                        });
                    }
                }
            }
            return list;
        }

        public List<PopularityReportItem> GetTopBooks()
        {
            var list = new List<PopularityReportItem>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT b.Title, b.Author, COUNT(l.LoanId) as BorrowCount, 
                           b.CurrentStock, b.TotalStock
                    FROM Books b
                    LEFT JOIN Loans l ON b.BookId = l.BookId
                    GROUP BY b.BookId
                    ORDER BY BorrowCount DESC
                    LIMIT 20";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PopularityReportItem
                        {
                            BookTitle = reader.GetString(0),
                            Author = reader.GetString(1),
                            BorrowCount = reader.GetInt32(2),
                            CurrentStock = reader.GetInt32(3),
                            TotalStock = reader.GetInt32(4)
                        });
                    }
                }
            }
            return list;
        }

        public List<LowStockReportItem> GetLowStockBooks()
        {
            var list = new List<LowStockReportItem>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT Title, Author, CurrentStock, TotalStock
                    FROM Books
                    WHERE CurrentStock < 2
                    ORDER BY CurrentStock ASC";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LowStockReportItem
                        {
                            BookTitle = reader.GetString(0),
                            Author = reader.GetString(1),
                            CurrentStock = reader.GetInt32(2),
                            TotalStock = reader.GetInt32(3)
                        });
                    }
                }
            }
            return list;
        }

        public void ExportToPdf<T>(List<T> data, string reportTitle, string filePath)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));
                    page.Header()
                        .Text(reportTitle)
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            var props = typeof(T).GetProperties();

                            table.ColumnsDefinition(columns =>
                            {
                                foreach (var p in props)
                                {
                                    columns.RelativeColumn();
                                }
                            });

                            table.Header(header =>
                            {
                                foreach (var prop in props)
                                {
                                    header.Cell().Element(CellStyle).Text(prop.Name).SemiBold();
                                }
                            });

                            foreach (var item in data)
                            {
                                foreach (var prop in props)
                                {
                                    var value = prop.GetValue(item);

                                    string textValue = "-";
                                    if (value != null)
                                    {
                                        if (value is DateTime date)
                                            textValue = date.ToString("dd.MM.yyyy");
                                        else
                                            textValue = value.ToString();
                                    }

                                    table.Cell().Element(CellStyle).Text(textValue);
                                }
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Pagina ");
                            x.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf(filePath);
        }

        static IContainer CellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(5)
                .PaddingHorizontal(2);
        }
    }
}