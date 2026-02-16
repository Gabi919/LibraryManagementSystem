using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using Npgsql;

namespace LibraryManagementSystem.Services
{
    public class BookService : IBookService
    {
        private readonly string _connectionString;

        public BookService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ObservableCollection<Book> GetAllBooks()
        {
            var list = new ObservableCollection<Book>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT b.BookId, b.Title, b.Author, b.CategoryId, c.CategoryName, 
                           b.TotalStock, b.CurrentStock, b.CoverImage,
                           b.Isbn, b.Publisher, b.PublishedYear
                    FROM Books b
                    JOIN Categories c ON b.CategoryId = c.CategoryId
                    ORDER BY b.BookId";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Book
                        {
                            BookId = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Author = reader.GetString(2),
                            CategoryId = reader.GetInt32(3),
                            CategoryName = reader.GetString(4),
                            TotalStock = reader.GetInt32(5),
                            CurrentStock = reader.GetInt32(6),
                            CoverImage = reader.IsDBNull(7) ? "default.jpg" : reader.GetString(7),
                            Isbn = reader.IsDBNull(8) ? "" : reader.GetString(8),
                            Publisher = reader.IsDBNull(9) ? "" : reader.GetString(9),
                            PublishedYear = reader.IsDBNull(10) ? 0 : reader.GetInt32(10)
                        });
                    }
                }
            }
            return list;
        }

        public List<Category> GetAllCategories()
        {
            var list = new List<Category>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT CategoryId, CategoryName FROM Categories", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Category
                        {
                            CategoryId = reader.GetInt32(0),
                            CategoryName = reader.GetString(1)
                        });
                    }
                }
            }
            return list;
        }

        public bool AddBook(Book book)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    INSERT INTO Books (Title, Author, CategoryId, TotalStock, CurrentStock, CoverImage, Isbn, Publisher, PublishedYear)
                    VALUES (@Title, @Author, @CatId, @Total, @Current, @Cover, @Isbn, @Pub, @Year)";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", book.Title);
                    cmd.Parameters.AddWithValue("@Author", book.Author);
                    cmd.Parameters.AddWithValue("@CatId", book.CategoryId);
                    cmd.Parameters.AddWithValue("@Total", book.TotalStock);
                    cmd.Parameters.AddWithValue("@Current", book.CurrentStock);
                    cmd.Parameters.AddWithValue("@Cover", string.IsNullOrEmpty(book.CoverImage) ? "default.jpg" : book.CoverImage);
                    cmd.Parameters.AddWithValue("@Isbn", string.IsNullOrEmpty(book.Isbn) ? (object)DBNull.Value : book.Isbn);
                    cmd.Parameters.AddWithValue("@Pub", string.IsNullOrEmpty(book.Publisher) ? (object)DBNull.Value : book.Publisher);
                    cmd.Parameters.AddWithValue("@Year", book.PublishedYear == 0 ? (object)DBNull.Value : book.PublishedYear);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateBook(Book book)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    UPDATE Books 
                    SET Title = @Title, 
                        Author = @Author, 
                        CategoryId = @CatId, 
                        TotalStock = @Total, 
                        CurrentStock = @Current,
                        CoverImage = @Cover,
                        Isbn = @Isbn,
                        Publisher = @Pub,
                        PublishedYear = @Year
                    WHERE BookId = @Id";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", book.Title);
                    cmd.Parameters.AddWithValue("@Author", book.Author);
                    cmd.Parameters.AddWithValue("@CatId", book.CategoryId);
                    cmd.Parameters.AddWithValue("@Total", book.TotalStock);
                    cmd.Parameters.AddWithValue("@Current", book.CurrentStock);
                    cmd.Parameters.AddWithValue("@Cover", string.IsNullOrEmpty(book.CoverImage) ? "default.jpg" : book.CoverImage);
                    cmd.Parameters.AddWithValue("@Isbn", string.IsNullOrEmpty(book.Isbn) ? (object)DBNull.Value : book.Isbn);
                    cmd.Parameters.AddWithValue("@Pub", string.IsNullOrEmpty(book.Publisher) ? (object)DBNull.Value : book.Publisher);
                    cmd.Parameters.AddWithValue("@Year", book.PublishedYear == 0 ? (object)DBNull.Value : book.PublishedYear);
                    cmd.Parameters.AddWithValue("@Id", book.BookId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteBook(int bookId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM Books WHERE BookId = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", bookId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool BorrowBook(int userId, int bookId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (var checkCmd = new NpgsqlCommand("SELECT CurrentStock FROM Books WHERE BookId = @Id", conn))
                        {
                            checkCmd.Parameters.AddWithValue("@Id", bookId);
                            int stock = (int)checkCmd.ExecuteScalar();
                            if (stock <= 0) return false;
                        }

                        using (var updateCmd = new NpgsqlCommand("UPDATE Books SET CurrentStock = CurrentStock - 1 WHERE BookId = @Id", conn))
                        {
                            updateCmd.Parameters.AddWithValue("@Id", bookId);
                            updateCmd.ExecuteNonQuery();
                        }

                        using (var insertCmd = new NpgsqlCommand(@"
                            INSERT INTO Loans (UserId, BookId, LoanDate, DueDate) 
                            VALUES (@Uid, @Bid, @Date, @Due)", conn))
                        {
                            insertCmd.Parameters.AddWithValue("@Uid", userId);
                            insertCmd.Parameters.AddWithValue("@Bid", bookId);
                            insertCmd.Parameters.AddWithValue("@Date", DateTime.Now);
                            insertCmd.Parameters.AddWithValue("@Due", DateTime.Now.AddDays(14));
                            insertCmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                        return true;
                    }
                    catch
                    {
                        trans.Rollback();
                        return false;
                    }
                }
            }
        }

        public ObservableCollection<BorrowedBook> GetUserLoans(int userId)
        {
            var list = new ObservableCollection<BorrowedBook>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT l.LoanId, b.Title, b.Author, l.LoanDate, l.DueDate, b.CoverImage 
                    FROM Loans l
                    JOIN Books b ON l.BookId = b.BookId
                    WHERE l.UserId = @Uid AND l.ReturnDate IS NULL
                    ORDER BY l.LoanDate DESC";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Uid", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                        string folder = Path.Combine(baseDir, "CoverImages");

                        while (reader.Read())
                        {
                            string coverImg = reader.IsDBNull(5) ? "default.jpg" : reader.GetString(5);
                            string fullPath = Path.Combine(folder, coverImg);
                            if (!File.Exists(fullPath)) fullPath = Path.Combine(folder, "default.jpg");

                            list.Add(new BorrowedBook
                            {
                                LoanId = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Author = reader.GetString(2),
                                LoanDate = reader.GetDateTime(3),
                                DueDate = reader.GetDateTime(4),
                                FullImagePath = fullPath
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}