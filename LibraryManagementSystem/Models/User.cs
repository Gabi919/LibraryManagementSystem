using System;

namespace LibraryManagementSystem.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime RegisteredDate { get; set; }
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}