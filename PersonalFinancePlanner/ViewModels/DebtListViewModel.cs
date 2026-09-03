using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using PersonalFinancePlanner.Models;

namespace PersonalFinancePlanner.ViewModels
{
    public class DebtListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CreditCard> CreditCards { get; set; }

        public int TotalDebts => CreditCards?.Count ?? 0;
        public decimal TotalBalance => CreditCards?.Sum(c => c.Balance) ?? 0;

        public DebtListViewModel(ObservableCollection<CreditCard> creditCards)
        {
            CreditCards = creditCards;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
