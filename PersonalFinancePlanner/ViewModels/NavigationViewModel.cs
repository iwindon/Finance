using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PersonalFinancePlanner.ViewModels
{
    public class NavigationViewModel : INotifyPropertyChanged
    {
        private object? _currentView;
        private DashboardViewModel? _dashboardViewModel;
        private TransactionsViewModel? _transactionsViewModel;
        private BudgetViewModel? _budgetViewModel;
        private DebtViewModel? _debtViewModel;
        private SavingsViewModel? _savingsViewModel;

        public object? CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateToDashboardCommand { get; }
        public ICommand NavigateToTransactionsCommand { get; }
        public ICommand NavigateToBudgetCommand { get; }
        public ICommand NavigateToDebtCommand { get; }
        public ICommand NavigateToSavingsCommand { get; }

        public NavigationViewModel()
        {
            NavigateToDashboardCommand = new RelayCommand(NavigateToDashboard);
            NavigateToTransactionsCommand = new RelayCommand(NavigateToTransactions);
            NavigateToBudgetCommand = new RelayCommand(NavigateToBudget);
            NavigateToDebtCommand = new RelayCommand(NavigateToDebt);
            NavigateToSavingsCommand = new RelayCommand(NavigateToSavings);

            // Start on dashboard
            NavigateToDashboard();
        }

        private void NavigateToDashboard()
        {
            if (_dashboardViewModel == null)
            {
                _dashboardViewModel = new DashboardViewModel(this);
            }
            else
            {
                _dashboardViewModel.RefreshData();
            }
            CurrentView = _dashboardViewModel;
        }

        private void NavigateToTransactions()
        {
            _transactionsViewModel ??= new TransactionsViewModel();
            CurrentView = _transactionsViewModel;
        }

        private void NavigateToBudget()
        {
            _budgetViewModel ??= new BudgetViewModel();
            CurrentView = _budgetViewModel;
        }

        private void NavigateToDebt()
        {
            _debtViewModel ??= new DebtViewModel();
            CurrentView = _debtViewModel;
        }

        private void NavigateToSavings()
        {
            _savingsViewModel ??= new SavingsViewModel();
            CurrentView = _savingsViewModel;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
