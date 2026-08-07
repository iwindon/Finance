using System.Windows;
using PersonalFinancePlanner.ViewModels;

namespace PersonalFinancePlanner.Views
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
