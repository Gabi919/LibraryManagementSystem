using LibraryApp.Core.Base;
using LibraryApp.ViewModels.Base;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.Win32; 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class BookEditorViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;
        private readonly bool _isEditMode;

        public event Action RequestGoBack;

        public string PageTitle => _isEditMode ? "Editare Carte" : "Adăugare Carte Nouă";

        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int TotalStock { get; set; }
        public int CurrentStock { get; set; }

        private string _coverImage;
        public string CoverImage
        {
            get => _coverImage;
            set
            {
                if (SetProperty(ref _coverImage, value))
                {
                    OnPropertyChanged(nameof(PreviewImagePath));
                }
            }
        }

        public string PreviewImagePath
        {
            get
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string folder = Path.Combine(baseDir, "CoverImages");
                string fileName = string.IsNullOrEmpty(CoverImage) ? "default.jpg" : CoverImage;
                string fullPath = Path.Combine(folder, fileName);

                if (!File.Exists(fullPath)) return Path.Combine(folder, "default.jpg");
                return fullPath;
            }
        }

        public string Isbn { get; set; }
        public string Publisher { get; set; }
        public int PublishedYear { get; set; }

        public List<Category> Categories { get; set; }
        public Category SelectedCategory { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectImageCommand { get; }

        public BookEditorViewModel(IBookService bookService, Book bookToEdit = null)
        {
            _bookService = bookService;
            Categories = _bookService.GetAllCategories();

            if (bookToEdit != null)
            {
                _isEditMode = true;
                BookId = bookToEdit.BookId;
                Title = bookToEdit.Title;
                Author = bookToEdit.Author;
                TotalStock = bookToEdit.TotalStock;
                CurrentStock = bookToEdit.CurrentStock;
                CoverImage = bookToEdit.CoverImage;
                Isbn = bookToEdit.Isbn;
                Publisher = bookToEdit.Publisher;
                PublishedYear = bookToEdit.PublishedYear;
                SelectedCategory = Categories.FirstOrDefault(c => c.CategoryId == bookToEdit.CategoryId);
            }
            else
            {
                _isEditMode = false;
                CoverImage = "default.jpg";
                SelectedCategory = Categories.FirstOrDefault();
            }

            SaveCommand = new RelayCommand(ExecuteSave);
            CancelCommand = new RelayCommand(_ => RequestGoBack?.Invoke());
            SelectImageCommand = new RelayCommand(ExecuteSelectImage);
        }

        private void ExecuteSelectImage(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Imagini|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Selectează Coperta Cărții"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string sourceFilePath = openFileDialog.FileName;
                string fileExtension = Path.GetExtension(sourceFilePath);

                string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                string destFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CoverImages");
                if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);

                string destPath = Path.Combine(destFolder, uniqueFileName);

                try
                {
                    File.Copy(sourceFilePath, destPath, true);

                    CoverImage = uniqueFileName;
                }
                catch (Exception)
                {
                    
                }
            }
        }

        private void ExecuteSave(object obj)
        {
            var book = new Book
            {
                BookId = BookId,
                Title = Title,
                Author = Author,
                TotalStock = TotalStock,
                CurrentStock = CurrentStock,
                CoverImage = CoverImage,
                Isbn = Isbn,
                Publisher = Publisher,
                PublishedYear = PublishedYear,
                CategoryId = SelectedCategory?.CategoryId ?? 1
            };

            if (!_isEditMode)
            {
                if (book.CurrentStock == 0) book.CurrentStock = book.TotalStock;
                _bookService.AddBook(book);
            }
            else
            {
                _bookService.UpdateBook(book);
            }

            RequestGoBack?.Invoke();
        }
    }
}