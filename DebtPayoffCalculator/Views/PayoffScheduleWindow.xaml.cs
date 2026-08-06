using System.Windows;
using DebtPayoffCalculator.ViewModels;

namespace DebtPayoffCalculator.Views
{
    public partial class PayoffScheduleWindow : Window
    {
        public PayoffScheduleWindow(PayoffScheduleViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
