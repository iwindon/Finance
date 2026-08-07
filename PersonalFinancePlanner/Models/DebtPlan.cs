using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PersonalFinancePlanner.Models
{
    public class DebtPlan : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private DateTime _createdDate;
        private DateTime _startDate;
        private DateTime _lastModifiedDate;
        private string _payoffMethod = string.Empty;
        private decimal _originalExtraPayment;
        private decimal _currentExtraPayment;
        private int _currentMonthNumber;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set { _createdDate = value; OnPropertyChanged(); }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(); }
        }

        public DateTime LastModifiedDate
        {
            get => _lastModifiedDate;
            set { _lastModifiedDate = value; OnPropertyChanged(); }
        }

        public string PayoffMethod
        {
            get => _payoffMethod;
            set { _payoffMethod = value; OnPropertyChanged(); }
        }

        public decimal OriginalExtraPayment
        {
            get => _originalExtraPayment;
            set { _originalExtraPayment = value; OnPropertyChanged(); }
        }

        public decimal CurrentExtraPayment
        {
            get => _currentExtraPayment;
            set { _currentExtraPayment = value; OnPropertyChanged(); }
        }

        // Tracks which month number we're currently at (1-based)
        public int CurrentMonthNumber
        {
            get => _currentMonthNumber;
            set { _currentMonthNumber = value; OnPropertyChanged(); }
        }

        // Snapshot of the original credit cards when plan was created
        public List<SavedCreditCard> OriginalCards { get; set; } = new List<SavedCreditCard>();

        // Current state of cards (may have been adjusted)
        public List<SavedCreditCard> CurrentCards { get; set; } = new List<SavedCreditCard>();

        // Historical months that have been completed
        public List<CompletedMonth> CompletedMonths { get; set; } = new List<CompletedMonth>();

        // History of adjustments made to the plan
        public List<PlanAdjustment> Adjustments { get; set; } = new List<PlanAdjustment>();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Simplified credit card data for serialization
    public class SavedCreditCard
    {
        public string Name { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MinimumPayment { get; set; }
        public int DueDate { get; set; }
    }

    // Represents a completed month with actual payment data
    public class CompletedMonth
    {
        public int MonthNumber { get; set; }
        public DateTime Date { get; set; }
        public List<MonthlyCardPayment> Payments { get; set; } = new List<MonthlyCardPayment>();
        public string Notes { get; set; } = string.Empty;
    }

    // Actual payment made to a specific card in a completed month
    public class MonthlyCardPayment
    {
        public string CardName { get; set; } = string.Empty;
        public decimal StartingBalance { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal InterestCharged { get; set; }
        public decimal EndingBalance { get; set; }
        public bool PaidOff { get; set; }
    }
}
