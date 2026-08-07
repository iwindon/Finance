using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PersonalFinancePlanner.Models
{
    public class CreditCard : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private decimal _balance;
        private decimal _interestRate;
        private decimal _minimumPayment;
        private DateTime _dueDate;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public decimal Balance
        {
            get => _balance;
            set
            {
                _balance = value;
                OnPropertyChanged();
            }
        }

        public decimal InterestRate
        {
            get => _interestRate;
            set
            {
                _interestRate = value;
                OnPropertyChanged();
            }
        }

        public decimal MinimumPayment
        {
            get => _minimumPayment;
            set
            {
                _minimumPayment = value;
                OnPropertyChanged();
            }
        }

        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
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
