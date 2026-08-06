using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DebtPayoffCalculator.Models
{
    public enum TransactionType
    {
        Income,
        Expense,
        Savings
    }

    public class Transaction : INotifyPropertyChanged
    {
        private DateTime _date;
        private string _description = string.Empty;
        private decimal _amount;
        private string _category = string.Empty;
        private TransactionType _type;

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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
