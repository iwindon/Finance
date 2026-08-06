using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DebtPayoffCalculator.Models
{
    public class BudgetCategory : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private decimal _budgetedAmount;
        private bool _isCustom;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public decimal BudgetedAmount
        {
            get => _budgetedAmount;
            set
            {
                _budgetedAmount = value;
                OnPropertyChanged();
            }
        }

        public bool IsCustom
        {
            get => _isCustom;
            set
            {
                _isCustom = value;
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
