using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DebtPayoffCalculator.Models;

namespace DebtPayoffCalculator.Views
{
    public partial class TransactionsView : UserControl
    {
        public TransactionsView()
        {
            InitializeComponent();
        }

        private void NumericTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void NumericTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (!textBox.IsKeyboardFocusWithin)
                {
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private void ExpenseRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.TransactionsViewModel viewModel)
            {
                viewModel.TransactionType = TransactionType.Expense;
            }
        }

        private void IncomeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.TransactionsViewModel viewModel)
            {
                viewModel.TransactionType = TransactionType.Income;
            }
        }

        private void SavingsRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.TransactionsViewModel viewModel)
            {
                viewModel.TransactionType = TransactionType.Savings;
            }
        }
    }
}
