using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using PersonalFinancePlanner.Models;
using PersonalFinancePlanner.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace PersonalFinancePlanner.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly NavigationViewModel _navigationViewModel;
        private readonly TransactionService _transactionService;
        private readonly BudgetService _budgetService;
        private readonly SavingsService _savingsService;
        private readonly DataService _debtService;

        private decimal _monthlyIncome;
        private decimal _monthlyExpenses;
        private decimal _budgetRemaining;
        private decimal _currentSavings;
        private decimal _totalDebt;

        public decimal MonthlyIncome
        {
            get => _monthlyIncome;
            set { _monthlyIncome = value; OnPropertyChanged(); }
        }

        public decimal MonthlyExpenses
        {
            get => _monthlyExpenses;
            set { _monthlyExpenses = value; OnPropertyChanged(); }
        }

        public decimal BudgetRemaining
        {
            get => _budgetRemaining;
            set { _budgetRemaining = value; OnPropertyChanged(); }
        }

        public decimal CurrentSavings
        {
            get => _currentSavings;
            set { _currentSavings = value; OnPropertyChanged(); }
        }

        public decimal TotalDebt
        {
            get => _totalDebt;
            set { _totalDebt = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ISeries> SpendingByCategorySeries { get; set; }
        public ObservableCollection<ISeries> MonthlyTrendSeries { get; set; }

        public DashboardViewModel(NavigationViewModel navigationViewModel)
        {
            _navigationViewModel = navigationViewModel;
            _transactionService = new TransactionService();
            _budgetService = new BudgetService();
            _savingsService = new SavingsService();
            _debtService = new DataService();

            SpendingByCategorySeries = new ObservableCollection<ISeries>();
            MonthlyTrendSeries = new ObservableCollection<ISeries>();

            RefreshData();
        }

        public void RefreshData()
        {
            var transactions = _transactionService.LoadTransactions();
            var budget = _budgetService.LoadBudgetCategories();
            var savings = _savingsService.LoadSavingsEntries();
            var creditCards = _debtService.LoadCreditCards();

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            // Calculate monthly income and expenses
            var currentMonthTransactions = transactions
                .Where(t => t.Date.Month == currentMonth && t.Date.Year == currentYear)
                .ToList();

            MonthlyIncome = currentMonthTransactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            MonthlyExpenses = currentMonthTransactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            var totalBudgeted = budget.Sum(b => b.BudgetedAmount);
            BudgetRemaining = totalBudgeted - MonthlyExpenses;

            // Current savings
            CurrentSavings = savings.OrderByDescending(s => s.Date).FirstOrDefault()?.Balance ?? 0;

            // Total debt
            TotalDebt = creditCards.Sum(c => c.Balance);

            // Update charts
            UpdateSpendingByCategory(currentMonthTransactions);
            UpdateMonthlyTrend(transactions);
        }

        private void UpdateSpendingByCategory(List<Transaction> transactions)
        {
            var expensesByCategory = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
                .OrderByDescending(x => x.Total)
                .Take(6)
                .ToList();

            SpendingByCategorySeries.Clear();

            if (expensesByCategory.Any())
            {
                var colors = new[] 
                { 
                    SKColors.DodgerBlue, 
                    SKColors.SteelBlue, 
                    SKColors.CornflowerBlue,
                    SKColors.LightSkyBlue,
                    SKColors.DeepSkyBlue,
                    SKColors.RoyalBlue
                };

                for (int i = 0; i < expensesByCategory.Count; i++)
                {
                    var item = expensesByCategory[i];
                    SpendingByCategorySeries.Add(new PieSeries<decimal>
                    {
                        Values = new[] { item.Total },
                        Name = item.Category,
                        DataLabelsPaint = new SolidColorPaint(SKColors.White),
                        DataLabelsSize = 14,
                        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
                    });
                }
            }

            OnPropertyChanged(nameof(SpendingByCategorySeries));
        }

        private void UpdateMonthlyTrend(List<Transaction> transactions)
        {
            var last6Months = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .OrderBy(d => d)
                .ToList();

            var incomeData = new List<decimal>();
            var expenseData = new List<decimal>();

            foreach (var month in last6Months)
            {
                var monthTransactions = transactions
                    .Where(t => t.Date.Month == month.Month && t.Date.Year == month.Year)
                    .ToList();

                incomeData.Add(monthTransactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount));
                expenseData.Add(monthTransactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount));
            }

            MonthlyTrendSeries.Clear();
            MonthlyTrendSeries.Add(new LineSeries<decimal>
            {
                Name = "Income",
                Values = incomeData,
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.Green, 3),
                GeometryStroke = new SolidColorPaint(SKColors.Green, 3),
                GeometrySize = 8
            });

            MonthlyTrendSeries.Add(new LineSeries<decimal>
            {
                Name = "Expenses",
                Values = expenseData,
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.Red, 3),
                GeometryStroke = new SolidColorPaint(SKColors.Red, 3),
                GeometrySize = 8
            });

            OnPropertyChanged(nameof(MonthlyTrendSeries));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
