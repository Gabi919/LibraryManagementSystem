using System;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using Npgsql;

namespace LibraryManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;

        public UserService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User Authenticate(string email, string password)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT UserId, FirstName, LastName, Email, RoleId FROM Users WHERE Email = @Email AND Password = @Pass";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Pass", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Email = reader.GetString(3),
                                RoleId = reader.GetInt32(4)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool RegisterUser(string firstName, string lastName, string email, string password, string phone)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email", conn))
                {
                    checkCmd.Parameters.AddWithValue("@Email", email);
                    long count = (long)checkCmd.ExecuteScalar();
                    if (count > 0) return false;
                }

                string insertSql = @"
                    INSERT INTO Users (FirstName, LastName, Email, Password, Phone, RoleId, RegisteredDate)
                    VALUES (@Fn, @Ln, @Email, @Pass, @Phone, 2, CURRENT_TIMESTAMP)";

                using (var cmd = new NpgsqlCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@Fn", firstName);
                    cmd.Parameters.AddWithValue("@Ln", lastName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Pass", password);

                    if (string.IsNullOrEmpty(phone))
                        cmd.Parameters.AddWithValue("@Phone", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@Phone", phone);

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }
    }
}