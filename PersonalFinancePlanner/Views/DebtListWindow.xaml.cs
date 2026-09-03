using System.Windows;
using PersonalFinancePlanner.ViewModels;

namespace PersonalFinancePlanner.Views
{
    public partial class DebtListWindow : Window
    {
        public DebtListWindow(DebtListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
