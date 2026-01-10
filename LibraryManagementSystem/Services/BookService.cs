using System;
using System.Collections.Generic;
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

        public List<Book> GetAllBooks()
        {
            var list = new List<Book>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT 
                        b.BookId, b.Title, b.Author, b.Isbn, b.Publisher, 
                        b.PublishedYear, b.TotalStock, b.CurrentStock, 
                        b.CategoryId, c.CategoryName
                    FROM Books b
                    INNER JOIN Categories c ON b.CategoryId = c.CategoryId
                    ORDER BY b.Title";

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
                            Isbn = reader.GetString(3),
                            Publisher = reader.IsDBNull(4) ? null : reader.GetString(4),
                            PublishedYear = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                            TotalStock = reader.GetInt32(6),
                            CurrentStock = reader.GetInt32(7),
                            CategoryId = reader.GetInt32(8),
                            CategoryName = reader.GetString(9)
                        });
                    }
                }
            }
            return list;
        }

        public void AddBook(Book book)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    INSERT INTO Books 
                    (Title, Author, Isbn, Publisher, PublishedYear, TotalStock, CurrentStock, CategoryId)
                    VALUES 
                    (@Title, @Author, @Isbn, @Publisher, @Year, @Stock, @Stock, @CatId)";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", book.Title);
                    cmd.Parameters.AddWithValue("@Author", book.Author);
                    cmd.Parameters.AddWithValue("@Isbn", book.Isbn);
                    cmd.Parameters.AddWithValue("@Publisher", (object)book.Publisher ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Year", book.PublishedYear);
                    cmd.Parameters.AddWithValue("@Stock", book.TotalStock);
                    cmd.Parameters.AddWithValue("@CatId", book.CategoryId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteBook(int bookId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Books WHERE BookId = @Id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", bookId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Category> GetAllCategories()
        {
            var list = new List<Category>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT CategoryId, CategoryName FROM Categories ORDER BY CategoryName";
                using (var cmd = new NpgsqlCommand(sql, conn))
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
    }
}