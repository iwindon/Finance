using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DebtPayoffCalculator.Models;
using DebtPayoffCalculator.Services;

namespace DebtPayoffCalculator.ViewModels
{
    public class DebtViewModel : INotifyPropertyChanged
    {
        private readonly DataService _dataService;
        private string _cardName = string.Empty;
        private decimal _balance;
        private decimal _interestRate;
        private decimal _minimumPayment;
        private DateTime _dueDate = DateTime.Today;
        private string _payoffMethod = "Avalanche";
        private decimal _extraPayment;
        private string _resultsText = string.Empty;
        private bool _isSnowballSelected = false;
        private bool _isAvalancheSelected = true;

        public ObservableCollection<CreditCard> CreditCards { get; set; }

        public string CardName
        {
            get => _cardName;
            set { _cardName = value; OnPropertyChanged(); }
        }

        public decimal Balance
        {
            get => _balance;
            set { _balance = value; OnPropertyChanged(); }
        }

        public decimal InterestRate
        {
            get => _interestRate;
            set { _interestRate = value; OnPropertyChanged(); }
        }

        public decimal MinimumPayment
        {
            get => _minimumPayment;
            set { _minimumPayment = value; OnPropertyChanged(); }
        }

        public DateTime DueDate
        {
            get => _dueDate;
            set { _dueDate = value; OnPropertyChanged(); }
        }

        public string PayoffMethod
        {
            get => _payoffMethod;
            set
            {
                _payoffMethod = value;
                OnPropertyChanged();
                IsSnowballSelected = value == "Snowball";
                IsAvalancheSelected = value == "Avalanche";
            }
        }

        public bool IsSnowballSelected
        {
            get => _isSnowballSelected;
            set { _isSnowballSelected = value; OnPropertyChanged(); }
        }

        public bool IsAvalancheSelected
        {
            get => _isAvalancheSelected;
            set { _isAvalancheSelected = value; OnPropertyChanged(); }
        }

        public decimal ExtraPayment
        {
            get => _extraPayment;
            set { _extraPayment = value; OnPropertyChanged(); }
        }

        public string ResultsText
        {
            get => _resultsText;
            set { _resultsText = value; OnPropertyChanged(); }
        }

        public ICommand AddCardCommand { get; }
        public ICommand RemoveCardCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand ViewScheduleCommand { get; }

        private List<PayoffScheduleEntry> _payoffSchedule = new List<PayoffScheduleEntry>();
        private int _totalMonths;
        private decimal _totalInterest;
        private decimal _totalPaid;

        public DebtViewModel()
        {
            _dataService = new DataService();
            CreditCards = new ObservableCollection<CreditCard>();
            AddCardCommand = new RelayCommand(AddCard, CanAddCard);
            RemoveCardCommand = new RelayCommand<CreditCard>(RemoveCard);
            CalculateCommand = new RelayCommand(Calculate, CanCalculate);
            ViewScheduleCommand = new RelayCommand(ViewSchedule, CanViewSchedule);

            // Load saved credit cards
            LoadData();
        }

        private void LoadData()
        {
            var savedCards = _dataService.LoadCreditCards();
            foreach (var card in savedCards)
            {
                CreditCards.Add(card);
            }
        }

        private void SaveData()
        {
            _dataService.SaveCreditCards(CreditCards);
        }

        private bool CanAddCard()
        {
            return !string.IsNullOrWhiteSpace(CardName) && Balance > 0 && MinimumPayment > 0;
        }

        private void AddCard()
        {
            var card = new CreditCard
            {
                Name = CardName,
                Balance = Balance,
                InterestRate = InterestRate,
                MinimumPayment = MinimumPayment,
                DueDate = DueDate
            };

            CreditCards.Add(card);

            // Save data after adding
            SaveData();

            // Clear input fields
            CardName = string.Empty;
            Balance = 0;
            InterestRate = 0;
            MinimumPayment = 0;
            DueDate = DateTime.Today;
        }

        private void RemoveCard(CreditCard? card)
        {
            if (card != null)
            {
                CreditCards.Remove(card);

                // Save data after removing
                SaveData();
            }
        }

        private bool CanCalculate()
        {
            return CreditCards.Count > 0;
        }

        private void Calculate()
        {
            if (CreditCards.Count == 0)
            {
                ResultsText = "Please add at least one credit card.";
                return;
            }

            // Create working copies of the cards
            var workingCards = CreditCards.Select(c => new
            {
                Name = c.Name,
                Balance = c.Balance,
                InterestRate = c.InterestRate,
                MinimumPayment = c.MinimumPayment
            }).ToList();

            int months = 0;
            decimal totalInterestPaid = 0;
            decimal totalPaid = 0;
            _payoffSchedule = new List<PayoffScheduleEntry>();
            DateTime currentDate = new DateTime(2026, 8, 1); // Start in August 2026
            var paidOffCards = new HashSet<string>();
            decimal rolloverAmount = 0;

            // Calculate payoff
            while (workingCards.Any(c => c.Balance > 0))
            {
                months++;
                currentDate = new DateTime(2026, 8, 1).AddMonths(months - 1);

                // Safety check to prevent infinite loop
                if (months > 1200) // 100 years
                {
                    ResultsText = "Error: Unable to pay off debts with current payment plan. Please increase payments.";
                    return;
                }

                // Calculate total minimum payment
                decimal totalMinimumPayment = workingCards.Where(c => c.Balance > 0).Sum(c => c.MinimumPayment);

                // Track rollover from cards paid off this month
                decimal rolloverThisMonth = 0;

                // Track available extra funds this month (starts with extra payment + any rollover from previous months)
                decimal availableExtraFunds = ExtraPayment + rolloverAmount;

                // Apply payments to all cards with balance, allowing cascading extra payments
                while (availableExtraFunds > 0.01m && workingCards.Any(c => c.Balance > 0))
                {
                    // Determine which card to prioritize based on method
                    var priorityCard = PayoffMethod == "Snowball"
                        ? workingCards.Where(c => c.Balance > 0).OrderBy(c => c.Balance).FirstOrDefault()
                        : workingCards.Where(c => c.Balance > 0).OrderByDescending(c => c.InterestRate).FirstOrDefault();

                    if (priorityCard == null) break;

                    int priorityIndex = workingCards.FindIndex(c => c.Name == priorityCard.Name);
                    var card = workingCards[priorityIndex];
                    decimal startingBalance = card.Balance;

                    // Calculate monthly interest on current balance
                    decimal monthlyInterestRate = card.InterestRate / 100 / 12;
                    decimal interestCharge = card.Balance * monthlyInterestRate;
                    totalInterestPaid += interestCharge;

                    // Determine total payment: minimum + extra funds
                    decimal totalPayment = card.MinimumPayment + availableExtraFunds;
                    decimal balanceWithInterest = card.Balance + interestCharge;
                    decimal actualPayment = Math.Min(totalPayment, balanceWithInterest);

                    decimal newBalance = balanceWithInterest - actualPayment;
                    decimal principalPaid = actualPayment - interestCharge;
                    totalPaid += actualPayment;

                    bool isPaidOff = newBalance <= 0.01m;

                    // Track if this card is being paid off
                    if (isPaidOff && !paidOffCards.Contains(card.Name))
                    {
                        paidOffCards.Add(card.Name);
                        rolloverThisMonth += card.MinimumPayment;
                    }

                    // Create single schedule entry showing total payment with correct interest
                    var entry = new PayoffScheduleEntry
                    {
                        MonthNumber = months,
                        Date = currentDate,
                        CardName = card.Name,
                        StartingBalance = startingBalance,
                        Payment = actualPayment,
                        InterestCharged = interestCharge,
                        PrincipalPaid = principalPaid,
                        EndingBalance = Math.Max(0, newBalance),
                        IsPaidOff = isPaidOff,
                        IsRolloverMonth = isPaidOff,
                        RolloverAmount = isPaidOff ? card.MinimumPayment : 0,
                        IsPriorityCard = true // This card is getting extra payment
                    };
                    _payoffSchedule.Add(entry);

                    // Update balance
                    workingCards[priorityIndex] = new
                    {
                        card.Name,
                        Balance = Math.Max(0, newBalance),
                        card.InterestRate,
                        card.MinimumPayment
                    };

                    // Reduce available funds by the extra amount actually used (payment - minimum)
                    decimal extraUsed = actualPayment - card.MinimumPayment;
                    availableExtraFunds -= extraUsed;

                    // If card is paid off and there's leftover money, it cascades to the next priority card
                    // Otherwise, we've applied all the extra funds to this one card, so break
                    if (!isPaidOff)
                    {
                        break; // Extra funds exhausted on this card
                    }
                }

                // Apply minimum payments to remaining cards that didn't get extra payments
                for (int i = 0; i < workingCards.Count; i++)
                {
                    var card = workingCards[i];
                    if (card.Balance <= 0) continue;

                    // Check if we already created an entry for this card (it got extra payment)
                    bool alreadyProcessed = _payoffSchedule.Any(e => 
                        e.MonthNumber == months && 
                        e.CardName == card.Name);

                    if (alreadyProcessed) continue;

                    decimal startingBalance = card.Balance;

                    // Calculate monthly interest
                    decimal monthlyInterestRate = card.InterestRate / 100 / 12;
                    decimal interestCharge = card.Balance * monthlyInterestRate;
                    totalInterestPaid += interestCharge;

                    // Apply minimum payment only
                    decimal payment = card.MinimumPayment;
                    decimal balanceWithInterest = card.Balance + interestCharge;
                    decimal actualPayment = Math.Min(payment, balanceWithInterest);
                    decimal newBalance = balanceWithInterest - actualPayment;
                    decimal principalPaid = actualPayment - interestCharge;
                    totalPaid += actualPayment;

                    // Create schedule entry for minimum payment only
                    var entry = new PayoffScheduleEntry
                    {
                        MonthNumber = months,
                        Date = currentDate,
                        CardName = card.Name,
                        StartingBalance = startingBalance,
                        Payment = actualPayment,
                        InterestCharged = interestCharge,
                        PrincipalPaid = principalPaid,
                        EndingBalance = Math.Max(0, newBalance),
                        IsPaidOff = false,
                        IsRolloverMonth = false,
                        RolloverAmount = 0,
                        IsPriorityCard = false // Just getting minimum payment
                    };
                    _payoffSchedule.Add(entry);

                    // Update balance
                    workingCards[i] = new
                    {
                        card.Name,
                        Balance = Math.Max(0, newBalance),
                        card.InterestRate,
                        card.MinimumPayment
                    };
                }

                // Apply rollover to next month
                rolloverAmount += rolloverThisMonth;
            }

            _totalMonths = months;
            _totalInterest = totalInterestPaid;
            _totalPaid = totalPaid;

            int years = months / 12;
            int remainingMonths = months % 12;

            string timeString = years > 0
                ? $"{years} year(s) and {remainingMonths} month(s)"
                : $"{remainingMonths} month(s)";

            ResultsText = $"Payoff Method: {PayoffMethod}\n\n" +
                         $"Time to Pay Off: {timeString} ({months} months)\n" +
                         $"Total Interest Paid: {totalInterestPaid:C2}\n" +
                         $"Total Amount Paid: {totalPaid:C2}\n" +
                         $"Monthly Payment (avg): {(totalPaid / months):C2}\n\n" +
                         $"Click 'View Detailed Schedule' to see the complete month-by-month breakdown.";
        }

        private bool CanViewSchedule()
        {
            return _payoffSchedule.Count > 0;
        }

        private void ViewSchedule()
        {
            var viewModel = new PayoffScheduleViewModel(
                _payoffSchedule,
                PayoffMethod,
                ExtraPayment,
                _totalInterest,
                _totalPaid,
                _totalMonths
            );

            var window = new Views.PayoffScheduleWindow(viewModel);
            window.Show();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute();

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecute;

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke((T?)parameter) ?? true;

        public void Execute(object? parameter) => _execute((T?)parameter);

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
