using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DebtPayoffCalculator.Views
{
    public partial class DebtView : UserControl
    {
        public DebtView()
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

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && DataContext is ViewModels.DebtViewModel viewModel)
            {
                if (radioButton.Content?.ToString()?.Contains("Avalanche") == true)
                {
                    viewModel.PayoffMethod = "Avalanche";
                }
                else if (radioButton.Content?.ToString()?.Contains("Snowball") == true)
                {
                    viewModel.PayoffMethod = "Snowball";
                }
            }
        }
    }
}
