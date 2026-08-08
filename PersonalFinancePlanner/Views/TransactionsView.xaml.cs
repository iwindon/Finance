using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PersonalFinancePlanner.Models;

namespace PersonalFinancePlanner.Views
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
            HideCreditCardSelector();
        }

        private void IncomeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.TransactionsViewModel viewModel)
            {
                viewModel.TransactionType = TransactionType.Income;
            }
            HideCreditCardSelector();
        }

        private void SavingsRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.TransactionsViewModel viewModel)
            {
                viewModel.TransactionType = TransactionType.Savings;
            }
            HideCreditCardSelector();
        }

        private void DebtPaymentRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.TransactionsViewModel viewModel)
            {
                viewModel.TransactionType = TransactionType.DebtPayment;
            }
            ShowCreditCardSelector();
        }

        private void ShowCreditCardSelector()
        {
            if (CreditCardSelectorPanel != null)
            {
                CreditCardSelectorPanel.Visibility = Visibility.Visible;
            }
        }

        private void HideCreditCardSelector()
        {
            if (CreditCardSelectorPanel != null)
            {
                CreditCardSelectorPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}
