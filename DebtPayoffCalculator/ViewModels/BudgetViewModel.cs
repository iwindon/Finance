using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DebtPayoffCalculator.Models;
using DebtPayoffCalculator.Services;

namespace DebtPayoffCalculator.ViewModels
{
    public class BudgetViewModel : INotifyPropertyChanged
    {
        private readonly BudgetService _budgetService;
        private readonly TransactionService _transactionService;

        private string _newCategoryName = string.Empty;
        private decimal _totalBudgeted;
        private decimal _totalSpent;
        private decimal _remaining;

        public ObservableCollection<BudgetCategory> Categories { get; set; }
        public ObservableCollection<BudgetComparison> BudgetComparisons { get; set; }

        public string NewCategoryName
        {
            get => _newCategoryName;
            set { _newCategoryName = value; OnPropertyChanged(); }
        }

        public decimal TotalBudgeted
        {
            get => _totalBudgeted;
            set { _totalBudgeted = value; OnPropertyChanged(); }
        }

        public decimal TotalSpent
        {
            get => _totalSpent;
            set { _totalSpent = value; OnPropertyChanged(); }
        }

        public decimal Remaining
        {
            get => _remaining;
            set { _remaining = value; OnPropertyChanged(); }
        }

        public ICommand AddCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand SaveBudgetCommand { get; }

        public BudgetViewModel()
        {
            _budgetService = new BudgetService();
            _transactionService = new TransactionService();

            Categories = new ObservableCollection<BudgetCategory>();
            BudgetComparisons = new ObservableCollection<BudgetComparison>();

            AddCategoryCommand = new RelayCommand(AddCategory, CanAddCategory);
            DeleteCategoryCommand = new RelayCommand<BudgetCategory>(DeleteCategory);
            SaveBudgetCommand = new RelayCommand(SaveBudget);

            LoadData();
        }

        private void LoadData()
        {
            Categories.Clear();
            var categories = _budgetService.LoadBudgetCategories();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }

            UpdateComparisons();
        }

        private void UpdateComparisons()
        {
            var transactions = _transactionService.LoadTransactions();
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var currentMonthExpenses = transactions
                .Where(t => t.Date.Month == currentMonth && 
                           t.Date.Year == currentYear && 
                           t.Type == TransactionType.Expense)
                .ToList();

            BudgetComparisons.Clear();
            TotalBudgeted = 0;
            TotalSpent = 0;

            foreach (var category in Categories)
            {
                var spent = currentMonthExpenses
                    .Where(t => t.Category == category.Name)
                    .Sum(t => t.Amount);

                var comparison = new BudgetComparison
                {
                    CategoryName = category.Name,
                    Budgeted = category.BudgetedAmount,
                    Spent = spent,
                    Remaining = category.BudgetedAmount - spent
                };

                BudgetComparisons.Add(comparison);

                TotalBudgeted += category.BudgetedAmount;
                TotalSpent += spent;
            }

            Remaining = TotalBudgeted - TotalSpent;
        }

        private bool CanAddCategory()
        {
            return !string.IsNullOrWhiteSpace(NewCategoryName);
        }

        private void AddCategory()
        {
            var category = new BudgetCategory
            {
                Name = NewCategoryName,
                BudgetedAmount = 0,
                IsCustom = true
            };

            Categories.Add(category);
            _budgetService.SaveBudgetCategories(Categories);

            NewCategoryName = string.Empty;
            UpdateComparisons();
        }

        private void DeleteCategory(BudgetCategory? category)
        {
            if (category != null && category.IsCustom)
            {
                Categories.Remove(category);
                _budgetService.SaveBudgetCategories(Categories);
                UpdateComparisons();
            }
        }

        private void SaveBudget()
        {
            _budgetService.SaveBudgetCategories(Categories);
            UpdateComparisons();
            System.Windows.MessageBox.Show("Budget saved successfully!", 
                "Success", 
                System.Windows.MessageBoxButton.OK, 
                System.Windows.MessageBoxImage.Information);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BudgetComparison
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Budgeted { get; set; }
        public decimal Spent { get; set; }
        public decimal Remaining { get; set; }
        public double PercentUsed => Budgeted > 0 ? (double)(Spent / Budgeted * 100) : 0;
    }
}
