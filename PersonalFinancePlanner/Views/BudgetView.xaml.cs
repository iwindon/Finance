using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalFinancePlanner.Views
{
    public partial class BudgetView : UserControl
    {
        public BudgetView()
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
    }
}
