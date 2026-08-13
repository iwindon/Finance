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
            if (_budgetViewModel == null)
            {
                _budgetViewModel = new BudgetViewModel();
            }
            else
            {
                _budgetViewModel.RefreshData();
            }
            CurrentView = _budgetViewModel;
        }

        private void NavigateToDebt()
        {
            if (_debtViewModel == null)
            {
                _debtViewModel = new DebtViewModel();
            }
            else
            {
                _debtViewModel.RefreshData();
            }
            CurrentView = _debtViewModel;
        }

        private void NavigateToSavings()
        {
            if (_savingsViewModel == null)
            {
                _savingsViewModel = new SavingsViewModel();
            }
            else
            {
                _savingsViewModel.RefreshData();
            }
            CurrentView = _savingsViewModel;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
