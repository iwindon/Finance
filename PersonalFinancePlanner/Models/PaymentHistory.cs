using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PersonalFinancePlanner.Models
{
    public class PaymentHistory : INotifyPropertyChanged
    {
        private DateTime _paymentDate;
        private decimal _paymentAmount;
        private decimal _interestPaid;
        private decimal _principalPaid;
        private decimal _remainingBalance;
        private string _notes = string.Empty;

        public DateTime PaymentDate
        {
            get => _paymentDate;
            set
            {
                _paymentDate = value;
                OnPropertyChanged();
            }
        }

        public decimal PaymentAmount
        {
            get => _paymentAmount;
            set
            {
                _paymentAmount = value;
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

        public decimal RemainingBalance
        {
            get => _remainingBalance;
            set
            {
                _remainingBalance = value;
                OnPropertyChanged();
            }
        }

        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
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
