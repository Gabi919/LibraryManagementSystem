using System;
using System.Collections.Generic;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using Npgsql;

namespace LibraryManagementSystem.Services
{
    public class LoanService : ILoanService
    {
        private readonly string _connectionString;

        public LoanService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Loan> GetActiveLoans()
        {
            var list = new List<Loan>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT 
                        l.LoanId, l.LoanDate, l.DueDate, 
                        u.FirstName, u.LastName, 
                        b.Title
                    FROM Loans l
                    INNER JOIN Users u ON l.UserId = u.UserId
                    INNER JOIN Books b ON l.BookId = b.BookId
                    WHERE l.ReturnDate IS NULL
                    ORDER BY l.DueDate";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Loan
                        {
                            LoanId = reader.GetInt32(0),
                            LoanDate = reader.GetDateTime(1),
                            DueDate = reader.GetDateTime(2),
                            ReturnDate = null,
                            UserFullName = $"{reader.GetString(3)} {reader.GetString(4)}",
                            BookTitle = reader.GetString(5)
                        });
                    }
                }
            }
            return list;
        }

        public void BorrowBook(int userId, int bookId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlInsert = @"
                            INSERT INTO Loans (UserId, BookId, DueDate) 
                            VALUES (@Uid, @Bid, @Due)";

                        using (var cmd1 = new NpgsqlCommand(sqlInsert, conn))
                        {
                            cmd1.Parameters.AddWithValue("@Uid", userId);
                            cmd1.Parameters.AddWithValue("@Bid", bookId);
                            cmd1.Parameters.AddWithValue("@Due", DateTime.Now.AddDays(14));
                            cmd1.ExecuteNonQuery();
                        }

                        string sqlUpdate = "UPDATE Books SET CurrentStock = CurrentStock - 1 WHERE BookId = @Bid";
                        using (var cmd2 = new NpgsqlCommand(sqlUpdate, conn))
                        {
                            cmd2.Parameters.AddWithValue("@Bid", bookId);
                            cmd2.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void ReturnBook(int loanId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int bookId = 0;
                        string sqlGetId = "SELECT BookId FROM Loans WHERE LoanId = @Lid";
                        using (var cmdGet = new NpgsqlCommand(sqlGetId, conn))
                        {
                            cmdGet.Parameters.AddWithValue("@Lid", loanId);
                            var result = cmdGet.ExecuteScalar();
                            if (result != null) bookId = (int)result;
                        }

                        string sqlUpdateLoan = "UPDATE Loans SET ReturnDate = @Now WHERE LoanId = @Lid";
                        using (var cmd1 = new NpgsqlCommand(sqlUpdateLoan, conn))
                        {
                            cmd1.Parameters.AddWithValue("@Now", DateTime.Now);
                            cmd1.Parameters.AddWithValue("@Lid", loanId);
                            cmd1.ExecuteNonQuery();
                        }

                        string sqlUpdateStock = "UPDATE Books SET CurrentStock = CurrentStock + 1 WHERE BookId = @Bid";
                        using (var cmd2 = new NpgsqlCommand(sqlUpdateStock, conn))
                        {
                            cmd2.Parameters.AddWithValue("@Bid", bookId);
                            cmd2.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}