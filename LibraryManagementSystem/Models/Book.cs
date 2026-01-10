namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Isbn { get; set; }
        public string Publisher { get; set; }
        public int PublishedYear { get; set; }
        public int TotalStock { get; set; }
        public int CurrentStock { get; set; }
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public bool IsAvailable => CurrentStock > 0;
        public string FullDisplay => $"{Title} - {Author}";
    }
}