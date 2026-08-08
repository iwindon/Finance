using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CsvHelper;
using CsvHelper.Configuration;
using PersonalFinancePlanner.Models;
using PersonalFinancePlanner.Services;
using Microsoft.Win32;

namespace PersonalFinancePlanner.ViewModels
{
    public class TransactionsViewModel : INotifyPropertyChanged
    {
        private readonly TransactionService _transactionService;
        private readonly BudgetService _budgetService;
        private readonly CreditCardPaymentService _paymentService;

        private DateTime _transactionDate = DateTime.Today;
        private string _description = string.Empty;
        private decimal _amount;
        private string _selectedCategory = string.Empty;
        private TransactionType _transactionType = TransactionType.Expense;
        private string _filterCategory = "All";
        private string _selectedCreditCard = string.Empty;

        public ObservableCollection<Transaction> Transactions { get; set; }
        public ObservableCollection<Transaction> FilteredTransactions { get; set; }
        public ObservableCollection<string> Categories { get; set; }
        public ObservableCollection<string> CreditCardNames { get; set; }

        public DateTime TransactionDate
        {
            get => _transactionDate;
            set { _transactionDate = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public decimal Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }

        public TransactionType TransactionType
        {
            get => _transactionType;
            set { _transactionType = value; OnPropertyChanged(); }
        }

        public string SelectedCreditCard
        {
            get => _selectedCreditCard;
            set { _selectedCreditCard = value; OnPropertyChanged(); }
        }

        public string FilterCategory
        {
            get => _filterCategory;
            set
            {
                _filterCategory = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public ICommand AddTransactionCommand { get; }
        public ICommand EditTransactionCommand { get; }
        public ICommand DeleteTransactionCommand { get; }
        public ICommand ImportCsvCommand { get; }

        private Transaction? _editingTransaction;

        public TransactionsViewModel()
        {
            _transactionService = new TransactionService();
            _budgetService = new BudgetService();
            _paymentService = new CreditCardPaymentService();

            Transactions = new ObservableCollection<Transaction>();
            FilteredTransactions = new ObservableCollection<Transaction>();
            Categories = new ObservableCollection<string>();
            CreditCardNames = new ObservableCollection<string>();

            AddTransactionCommand = new RelayCommand(AddTransaction, CanAddTransaction);
            EditTransactionCommand = new RelayCommand<Transaction>(EditTransaction);
            DeleteTransactionCommand = new RelayCommand<Transaction>(DeleteTransaction);
            ImportCsvCommand = new RelayCommand(ImportCsv);

            LoadData();
        }

        private void LoadData()
        {
            Transactions.Clear();
            var transactions = _transactionService.LoadTransactions();
            foreach (var transaction in transactions)
            {
                Transactions.Add(transaction);
            }

            Categories.Clear();
            Categories.Add("All");
            var budgetCategories = _budgetService.LoadBudgetCategories();
            foreach (var category in budgetCategories.Select(c => c.Name))
            {
                Categories.Add(category);
            }

            // Load credit card names
            CreditCardNames.Clear();
            var creditCards = _paymentService.LoadCreditCards();
            foreach (var card in creditCards)
            {
                CreditCardNames.Add(card.Name);
            }

            if (Categories.Count > 1)
            {
                SelectedCategory = Categories[1];
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            FilteredTransactions.Clear();
            var filtered = FilterCategory == "All"
                ? Transactions
                : Transactions.Where(t => t.Category == FilterCategory);

            foreach (var transaction in filtered.OrderByDescending(t => t.Date))
            {
                FilteredTransactions.Add(transaction);
            }
        }

        private bool CanAddTransaction()
        {
            return !string.IsNullOrWhiteSpace(Description) && Amount > 0 && !string.IsNullOrWhiteSpace(SelectedCategory);
        }

        private void AddTransaction()
        {
            if (_editingTransaction != null)
            {
                // Update existing transaction
                _editingTransaction.Date = TransactionDate;
                _editingTransaction.Description = Description;
                _editingTransaction.Amount = Amount;
                _editingTransaction.Category = SelectedCategory;
                _editingTransaction.Type = TransactionType;

                // Handle debt payment updates
                if (TransactionType == TransactionType.DebtPayment && !string.IsNullOrWhiteSpace(SelectedCreditCard))
                {
                    _editingTransaction.CreditCardName = SelectedCreditCard;
                    // Note: Editing a payment doesn't re-apply it to avoid double-counting
                }

                _editingTransaction = null;
            }
            else
            {
                // Add new transaction
                var transaction = new Transaction
                {
                    Date = TransactionDate,
                    Description = Description,
                    Amount = Amount,
                    Category = SelectedCategory,
                    Type = TransactionType
                };

                // Handle debt payments
                if (TransactionType == TransactionType.DebtPayment && !string.IsNullOrWhiteSpace(SelectedCreditCard))
                {
                    transaction.CreditCardName = SelectedCreditCard;
                    ApplyPaymentToCreditCard(transaction);
                }

                Transactions.Add(transaction);
            }

            _transactionService.SaveTransactions(Transactions);

            // If it's a savings transaction, sync to savings
            if (TransactionType == TransactionType.Savings)
            {
                SyncSavingsTransaction(TransactionDate, Amount, Description);
            }

            // Clear form
            Description = string.Empty;
            Amount = 0;
            TransactionDate = DateTime.Today;
            TransactionType = TransactionType.Expense;
            SelectedCreditCard = string.Empty;

            ApplyFilter();
        }

        private void EditTransaction(Transaction? transaction)
        {
            if (transaction != null)
            {
                _editingTransaction = transaction;
                TransactionDate = transaction.Date;
                Description = transaction.Description;
                Amount = transaction.Amount;
                SelectedCategory = transaction.Category;
                TransactionType = transaction.Type;
                SelectedCreditCard = transaction.CreditCardName ?? string.Empty;
            }
        }

        private void ApplyPaymentToCreditCard(Transaction transaction)
        {
            var creditCards = _paymentService.LoadCreditCards();
            var card = creditCards.FirstOrDefault(c => c.Name == transaction.CreditCardName);

            if (card != null)
            {
                var breakdown = _paymentService.ApplyPayment(
                    card,
                    transaction.Amount,
                    transaction.Date,
                    transaction.Description
                );

                // Update transaction with payment breakdown
                transaction.InterestPaid = breakdown.InterestPaid;
                transaction.PrincipalPaid = breakdown.PrincipalPaid;

                // Save updated credit cards
                _paymentService.SaveCreditCards(creditCards);
            }
        }

        private void SyncSavingsTransaction(DateTime date, decimal amount, string description)
        {
            var savingsService = new SavingsService();
            var entries = savingsService.LoadSavingsEntries();

            var entry = new SavingsEntry
            {
                Date = date,
                Balance = amount,
                Notes = description
            };

            entries.Add(entry);
            savingsService.SaveSavingsEntries(entries);
        }

        private void DeleteTransaction(Transaction? transaction)
        {
            if (transaction != null)
            {
                Transactions.Remove(transaction);
                _transactionService.SaveTransactions(Transactions);
                ApplyFilter();
            }
        }

        private void ImportCsv()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Select CSV file to import"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using var reader = new StreamReader(openFileDialog.FileName);
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HeaderValidated = null,
                        MissingFieldFound = null
                    };
                    using var csv = new CsvReader(reader, config);

                    var records = csv.GetRecords<CsvTransaction>().ToList();
                    int imported = 0;

                    foreach (var record in records)
                    {
                        var transaction = new Transaction
                        {
                            Date = record.Date,
                            Description = record.Description ?? "Imported Transaction",
                            Amount = Math.Abs(record.Amount),
                            Category = string.IsNullOrWhiteSpace(record.Category) ? "Other" : record.Category,
                            Type = record.Amount < 0 ? TransactionType.Expense : TransactionType.Income
                        };

                        Transactions.Add(transaction);
                        imported++;
                    }

                    _transactionService.SaveTransactions(Transactions);
                    ApplyFilter();

                    System.Windows.MessageBox.Show($"Successfully imported {imported} transactions!", 
                        "Import Complete", 
                        System.Windows.MessageBoxButton.OK, 
                        System.Windows.MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error importing CSV: {ex.Message}", 
                        "Import Error", 
                        System.Windows.MessageBoxButton.OK, 
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }

        // Helper class for CSV mapping
        private class CsvTransaction
        {
            public DateTime Date { get; set; }
            public string? Description { get; set; }
            public decimal Amount { get; set; }
            public string? Category { get; set; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
