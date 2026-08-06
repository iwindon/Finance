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
        private readonly DebtPlanService _planService;
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
        private DebtPlan? _currentPlan;
        private string _planName = string.Empty;

        public ObservableCollection<CreditCard> CreditCards { get; set; }
        public ObservableCollection<DebtPlan> SavedPlans { get; set; }

        public DebtPlan? CurrentPlan
        {
            get => _currentPlan;
            set { _currentPlan = value; OnPropertyChanged(); }
        }

        public string PlanName
        {
            get => _planName;
            set { _planName = value; OnPropertyChanged(); }
        }

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
        public ICommand SavePlanCommand { get; }
        public ICommand LoadPlanCommand { get; }
        public ICommand NewPlanCommand { get; }
        public ICommand AdjustPlanCommand { get; }
        public ICommand DeletePlanCommand { get; }

        private List<PayoffScheduleEntry> _payoffSchedule = new List<PayoffScheduleEntry>();
        private int _totalMonths;
        private decimal _totalInterest;
        private decimal _totalPaid;

        public DebtViewModel()
        {
            _dataService = new DataService();
            _planService = new DebtPlanService();
            CreditCards = new ObservableCollection<CreditCard>();
            SavedPlans = new ObservableCollection<DebtPlan>();
            AddCardCommand = new RelayCommand(AddCard, CanAddCard);
            RemoveCardCommand = new RelayCommand<CreditCard>(RemoveCard);
            CalculateCommand = new RelayCommand(Calculate, CanCalculate);
            ViewScheduleCommand = new RelayCommand(ViewSchedule, CanViewSchedule);
            SavePlanCommand = new RelayCommand(SavePlan, CanSavePlan);
            LoadPlanCommand = new RelayCommand<DebtPlan>(LoadPlan);
            NewPlanCommand = new RelayCommand(NewPlan);
            AdjustPlanCommand = new RelayCommand(AdjustPlan, CanAdjustPlan);
            DeletePlanCommand = new RelayCommand<DebtPlan>(DeletePlan);

            // Load saved credit cards
            LoadData();
            LoadSavedPlans();
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

            // Determine start date: use CurrentPlan's StartDate if loaded, otherwise default
            DateTime startDate = CurrentPlan?.StartDate ?? new DateTime(2026, 8, 1);
            int startMonthNumber = CurrentPlan?.CurrentMonthNumber ?? 1;

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
            DateTime currentDate = startDate; // Use plan's start date
            var paidOffCards = new HashSet<string>();
            decimal rolloverAmount = 0;

            // Calculate payoff
            while (workingCards.Any(c => c.Balance > 0))
            {
                months++;
                currentDate = startDate.AddMonths(months - 1);

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

            // If we have a plan with completed months, prepend them to the schedule
            if (CurrentPlan != null && CurrentPlan.CompletedMonths.Count > 0)
            {
                var historicalEntries = new List<PayoffScheduleEntry>();

                foreach (var completedMonth in CurrentPlan.CompletedMonths.OrderBy(m => m.MonthNumber))
                {
                    foreach (var payment in completedMonth.Payments)
                    {
                        historicalEntries.Add(new PayoffScheduleEntry
                        {
                            MonthNumber = completedMonth.MonthNumber,
                            Date = completedMonth.Date,
                            CardName = payment.CardName,
                            StartingBalance = payment.StartingBalance,
                            Payment = payment.PaymentAmount,
                            InterestCharged = payment.InterestCharged,
                            PrincipalPaid = payment.PaymentAmount - payment.InterestCharged,
                            EndingBalance = payment.EndingBalance,
                            IsPaidOff = payment.PaidOff,
                            IsRolloverMonth = false,
                            RolloverAmount = 0,
                            IsPriorityCard = false, // Historical months don't need priority highlighting
                            IsHistorical = true // Mark as historical data
                        });
                    }
                }

                // Combine historical and projected entries
                _payoffSchedule = historicalEntries.Concat(_payoffSchedule).ToList();
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

        private void LoadSavedPlans()
        {
            SavedPlans.Clear();
            var plans = _planService.LoadAllPlans();
            foreach (var plan in plans)
            {
                SavedPlans.Add(plan);
            }
        }

        private bool CanSavePlan()
        {
            return !string.IsNullOrWhiteSpace(PlanName) && CreditCards.Count > 0;
        }

        private void SavePlan()
        {
            if (CurrentPlan == null)
            {
                // Create new plan
                CurrentPlan = new DebtPlan
                {
                    Name = PlanName,
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    StartDate = new DateTime(2026, 8, 1), // Default start date
                    PayoffMethod = PayoffMethod,
                    OriginalExtraPayment = ExtraPayment,
                    CurrentExtraPayment = ExtraPayment,
                    CurrentMonthNumber = 1
                };

                // Snapshot current cards
                foreach (var card in CreditCards)
                {
                    var savedCard = new SavedCreditCard
                    {
                        Name = card.Name,
                        Balance = card.Balance,
                        InterestRate = card.InterestRate,
                        MinimumPayment = card.MinimumPayment,
                        DueDate = card.DueDate.Day
                    };
                    CurrentPlan.OriginalCards.Add(savedCard);
                    CurrentPlan.CurrentCards.Add(savedCard);
                }
            }
            else
            {
                // Update existing plan
                CurrentPlan.Name = PlanName;
                CurrentPlan.LastModifiedDate = DateTime.Now;
            }

            _planService.SavePlan(CurrentPlan);
            LoadSavedPlans();
            ResultsText = $"Plan '{PlanName}' saved successfully!";
        }

        private void LoadPlan(DebtPlan? plan)
        {
            if (plan == null) return;

            CurrentPlan = plan;
            PlanName = plan.Name;
            PayoffMethod = plan.PayoffMethod;
            ExtraPayment = plan.CurrentExtraPayment;

            // Load cards from the current plan state
            CreditCards.Clear();
            foreach (var savedCard in plan.CurrentCards)
            {
                CreditCards.Add(new CreditCard
                {
                    Name = savedCard.Name,
                    Balance = savedCard.Balance,
                    InterestRate = savedCard.InterestRate,
                    MinimumPayment = savedCard.MinimumPayment,
                    DueDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, savedCard.DueDate)
                });
            }

            ResultsText = $"Plan '{plan.Name}' loaded. Last modified: {plan.LastModifiedDate:g}";
        }

        private void NewPlan()
        {
            CurrentPlan = null;
            PlanName = string.Empty;
            CreditCards.Clear();
            ResultsText = string.Empty;
            _payoffSchedule.Clear();
        }

        private bool CanAdjustPlan()
        {
            return CurrentPlan != null;
        }

        private void AdjustPlan()
        {
            if (CurrentPlan == null) return;

            // Create an adjustment record
            var adjustment = new PlanAdjustment
            {
                AdjustmentDate = DateTime.Now,
                EffectiveMonthNumber = CurrentPlan.CurrentMonthNumber,
                AdjustmentType = "ExtraPaymentChange",
                OldExtraPayment = CurrentPlan.CurrentExtraPayment,
                NewExtraPayment = ExtraPayment,
                Description = $"Extra payment adjusted from {CurrentPlan.CurrentExtraPayment:C} to {ExtraPayment:C}",
                Reason = "User adjustment"
            };

            CurrentPlan.Adjustments.Add(adjustment);
            CurrentPlan.CurrentExtraPayment = ExtraPayment;

            // Update current card balances
            CurrentPlan.CurrentCards.Clear();
            foreach (var card in CreditCards)
            {
                CurrentPlan.CurrentCards.Add(new SavedCreditCard
                {
                    Name = card.Name,
                    Balance = card.Balance,
                    InterestRate = card.InterestRate,
                    MinimumPayment = card.MinimumPayment,
                    DueDate = card.DueDate.Day
                });
            }

            _planService.SavePlan(CurrentPlan);
            ResultsText = $"Plan adjusted on {DateTime.Now:g}. New extra payment: {ExtraPayment:C}";
        }

        private void DeletePlan(DebtPlan? plan)
        {
            if (plan == null) return;

            _planService.DeletePlan(plan.Name);
            SavedPlans.Remove(plan);

            if (CurrentPlan?.Name == plan.Name)
            {
                NewPlan();
            }

            ResultsText = $"Plan '{plan.Name}' deleted.";
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
