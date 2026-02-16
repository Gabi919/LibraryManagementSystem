using System;
using System.IO;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TotalStock { get; set; }
        public int CurrentStock { get; set; }
        public string CoverImage { get; set; }
        public string Isbn { get; set; }
        public string Publisher { get; set; }
        public int PublishedYear { get; set; }

        public string FullImagePath
        {
            get
            {
                string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CoverImages");
                string imageFile = string.IsNullOrEmpty(CoverImage) ? "default.jpg" : CoverImage;
                string fullPath = Path.Combine(folder, imageFile);

                if (File.Exists(fullPath)) return fullPath;
                return Path.Combine(folder, "default.jpg");
            }
        }

        public bool IsAvailable => CurrentStock > 0;
    }
}