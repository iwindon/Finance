using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PersonalFinancePlanner.Models
{
    public enum TransactionType
    {
        Income,
        Expense,
        Savings,
        DebtPayment
    }

    public class Transaction : INotifyPropertyChanged
    {
        private DateTime _date;
        private string _description = string.Empty;
        private decimal _amount;
        private string _category = string.Empty;
        private TransactionType _type;
        private string? _creditCardName;
        private decimal _interestPaid;
        private decimal _principalPaid;

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public decimal Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
            }
        }

        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged();
            }
        }

        public TransactionType Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        public string? CreditCardName
        {
            get => _creditCardName;
            set
            {
                _creditCardName = value;
                OnPropertyChanged();
            }
        }

        public decimal InterestPaid
        {
            get => _interestPaid;
            set
            {
                _interestPaid = value;
                OnPropertyChanged();
            }
        }

        public decimal PrincipalPaid
        {
            get => _principalPaid;
            set
            {
                _principalPaid = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
