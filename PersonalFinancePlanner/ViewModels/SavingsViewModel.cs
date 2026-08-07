using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PersonalFinancePlanner.Models;
using PersonalFinancePlanner.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace PersonalFinancePlanner.ViewModels
{
    public class SavingsViewModel : INotifyPropertyChanged
    {
        private readonly SavingsService _savingsService;

        private DateTime _entryDate = DateTime.Today;
        private decimal _balance;
        private string _notes = string.Empty;
        private decimal _currentBalance;
        private decimal _totalChange;

        public ObservableCollection<SavingsEntry> SavingsEntries { get; set; }
        public ObservableCollection<ISeries> SavingsChartSeries { get; set; }

        public DateTime EntryDate
        {
            get => _entryDate;
            set { _entryDate = value; OnPropertyChanged(); }
        }

        public decimal Balance
        {
            get => _balance;
            set { _balance = value; OnPropertyChanged(); }
        }

        public string Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(); }
        }

        public decimal CurrentBalance
        {
            get => _currentBalance;
            set { _currentBalance = value; OnPropertyChanged(); }
        }

        public decimal TotalChange
        {
            get => _totalChange;
            set { _totalChange = value; OnPropertyChanged(); }
        }

        public ICommand AddEntryCommand { get; }
        public ICommand DeleteEntryCommand { get; }

        public SavingsViewModel()
        {
            _savingsService = new SavingsService();

            SavingsEntries = new ObservableCollection<SavingsEntry>();
            SavingsChartSeries = new ObservableCollection<ISeries>();

            AddEntryCommand = new RelayCommand(AddEntry, CanAddEntry);
            DeleteEntryCommand = new RelayCommand<SavingsEntry>(DeleteEntry);

            LoadData();
        }

        private void LoadData()
        {
            SavingsEntries.Clear();
            var entries = _savingsService.LoadSavingsEntries();
            foreach (var entry in entries.OrderByDescending(e => e.Date))
            {
                SavingsEntries.Add(entry);
            }

            UpdateSummary();
            UpdateChart();
        }

        private void UpdateSummary()
        {
            CurrentBalance = SavingsEntries.OrderByDescending(e => e.Date).FirstOrDefault()?.Balance ?? 0;

            var oldestEntry = SavingsEntries.OrderBy(e => e.Date).FirstOrDefault();
            var newestEntry = SavingsEntries.OrderByDescending(e => e.Date).FirstOrDefault();

            if (oldestEntry != null && newestEntry != null)
            {
                TotalChange = newestEntry.Balance - oldestEntry.Balance;
            }
            else
            {
                TotalChange = 0;
            }
        }

        private void UpdateChart()
        {
            var chartData = SavingsEntries
                .OrderBy(e => e.Date)
                .Select(e => e.Balance)
                .ToList();

            SavingsChartSeries.Clear();

            if (chartData.Any())
            {
                SavingsChartSeries.Add(new LineSeries<decimal>
                {
                    Name = "Savings Balance",
                    Values = chartData,
                    Fill = new SolidColorPaint(SKColors.LightBlue.WithAlpha(50)),
                    Stroke = new SolidColorPaint(SKColors.DodgerBlue, 3),
                    GeometryStroke = new SolidColorPaint(SKColors.DodgerBlue, 3),
                    GeometrySize = 10,
                    LineSmoothness = 0.5
                });
            }

            OnPropertyChanged(nameof(SavingsChartSeries));
        }

        private bool CanAddEntry()
        {
            return Balance >= 0;
        }

        private void AddEntry()
        {
            var entry = new SavingsEntry
            {
                Date = EntryDate,
                Balance = Balance,
                Notes = Notes
            };

            SavingsEntries.Insert(0, entry);
            _savingsService.SaveSavingsEntries(SavingsEntries);

            Balance = 0;
            Notes = string.Empty;
            EntryDate = DateTime.Today;

            UpdateSummary();
            UpdateChart();
        }

        private void DeleteEntry(SavingsEntry? entry)
        {
            if (entry != null)
            {
                SavingsEntries.Remove(entry);
                _savingsService.SaveSavingsEntries(SavingsEntries);
                UpdateSummary();
                UpdateChart();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
