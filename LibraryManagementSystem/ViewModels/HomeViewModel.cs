using System;
using System.Collections.Generic;
using System.Windows.Threading;
using LibraryApp.Core.Base;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;

namespace LibraryManagementSystem.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;
        private readonly User _currentUser;
        private DispatcherTimer _timer;

        public string UserName => _currentUser?.FirstName ?? "Utilizator";
        public string UserRole => _currentUser?.IsAdmin == true ? "Administrator" : "Membru";

        private string _currentDate;
        public string CurrentDate
        {
            get => _currentDate;
            set => SetProperty(ref _currentDate, value);
        }

        private string _currentTime;
        public string CurrentTime
        {
            get => _currentTime;
            set => SetProperty(ref _currentTime, value);
        }

        public int ActiveLoansCount { get; set; }
        public int TotalBooksCount { get; set; }
        public string RandomQuote { get; set; }
        public string QuoteAuthor { get; set; }

        public HomeViewModel(User user, IBookService bookService)
        {
            _currentUser = user;
            _bookService = bookService;

            LoadStats();
            LoadRandomQuote();
            StartClock();
        }

        private void StartClock()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (sender, args) =>
            {
                CurrentDate = DateTime.Now.ToString("dd MMMM yyyy");
                CurrentTime = DateTime.Now.ToString("HH:mm");
            };

            CurrentDate = DateTime.Now.ToString("dd MMMM yyyy");
            CurrentTime = DateTime.Now.ToString("HH:mm");

            _timer.Start();
        }

        private void LoadStats()
        {
            if (_currentUser != null)
            {
                var loans = _bookService.GetUserLoans(_currentUser.UserId);
                ActiveLoansCount = loans.Count;
            }

            var books = _bookService.GetAllBooks();
            TotalBooksCount = books.Count;
        }

        private void LoadRandomQuote()
        {
            var quotes = new List<(string q, string a)>
            {
                ("O carte este un vis pe care îl ții în mâini.", "Neil Gaiman"),
                ("Cărțile sunt o magie unică și portabilă.", "Stephen King"),
                ("Nu există prieten mai loial ca o carte.", "Ernest Hemingway"),
                ("Camera fără cărți e ca trupul fără suflet.", "Cicero"),
                ("Citesc pentru a trăi.", "Gustave Flaubert"),
                ("O bibliotecă este un spital pentru minte.", "Anonim")
            };

            var rand = new Random();
            var selected = quotes[rand.Next(quotes.Count)];
            RandomQuote = $"„{selected.q}”";
            QuoteAuthor = selected.a;
        }
    }
}