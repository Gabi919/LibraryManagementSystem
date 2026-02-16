using LibraryApp.Core.Base;
using LibraryApp.ViewModels.Base;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class ReportsViewModel : ViewModelBase
    {
        private readonly IReportService _reportService;

        public Dictionary<string, string> ReportTypes { get; } = new Dictionary<string, string>
        {
            { "Overdue", "1. Cărți Nereturnate (Întârzieri)" },
            { "Popular", "2. Top Cele Mai Populare" },
            { "LowStock", "3. Stoc Critic (Aprovizionare)" }
        };

        private string _selectedReportKey;
        public string SelectedReportKey
        {
            get => _selectedReportKey;
            set
            {
                SetProperty(ref _selectedReportKey, value);
                LoadData();
            }
        }

        private ObservableCollection<object> _reportData;
        public ObservableCollection<object> ReportData
        {
            get => _reportData;
            set => SetProperty(ref _reportData, value);
        }

        public ICommand ExportPdfCommand { get; }

        public ReportsViewModel(IReportService reportService)
        {
            _reportService = reportService;
            ExportPdfCommand = new RelayCommand(ExecuteExport);

            ReportData = new ObservableCollection<object>();
            SelectedReportKey = "Overdue";
        }

        private void LoadData()
        {
            var newData = new ObservableCollection<object>();

            if (SelectedReportKey == "Overdue")
            {
                var list = _reportService.GetOverdueLoans();
                foreach (var item in list) newData.Add(item);
            }
            else if (SelectedReportKey == "Popular")
            {
                var list = _reportService.GetTopBooks();
                foreach (var item in list) newData.Add(item);
            }
            else if (SelectedReportKey == "LowStock")
            {
                var list = _reportService.GetLowStockBooks();
                foreach (var item in list) newData.Add(item);
            }

            ReportData = newData;
        }

        private void ExecuteExport(object obj)
        {
            if (ReportData == null || ReportData.Count == 0)
            {
                MessageBox.Show("Nu există date de exportat.");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "PDF Files|*.pdf",
                FileName = $"Raport_{SelectedReportKey}_{DateTime.Now:yyyyMMdd}.pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    if (SelectedReportKey == "Overdue")
                    {
                        var list = new List<OverdueReportItem>();
                        foreach (var item in ReportData) list.Add((OverdueReportItem)item);
                        _reportService.ExportToPdf(list, "Raport Întârzieri", saveDialog.FileName);
                    }
                    else if (SelectedReportKey == "Popular")
                    {
                        var list = new List<PopularityReportItem>();
                        foreach (var item in ReportData) list.Add((PopularityReportItem)item);
                        _reportService.ExportToPdf(list, "Top Cărți Populare", saveDialog.FileName);
                    }
                    else if (SelectedReportKey == "LowStock")
                    {
                        var list = new List<LowStockReportItem>();
                        foreach (var item in ReportData) list.Add((LowStockReportItem)item);
                        _reportService.ExportToPdf(list, "Raport Stoc Critic", saveDialog.FileName);
                    }

                    MessageBox.Show("PDF generat cu succes!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la export: " + ex.Message);
                }
            }
        }
    }
}