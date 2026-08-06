# Personal Finance Planner

A comprehensive Windows desktop application for managing your personal finances, built with WPF and .NET 8.

## 🌟 Features

### 📊 Dashboard
- **Summary Cards**: Quick overview of monthly income, expenses, budget remaining, savings, and total debt
- **Spending by Category**: Interactive pie chart showing where your money goes
- **Income vs Expenses Trend**: Line chart tracking the last 6 months

### 💳 Transactions
- **Manual Entry**: Add transactions with date, description, amount, and category
- **CSV Import**: Bulk import transactions from bank exports
- **Category Filtering**: View transactions by specific categories
- **Income/Expense Tracking**: Separate income and expense transactions

### 📈 Budget Management
- **Pre-defined Categories**: Housing, Transportation, Food, Utilities, Entertainment, Healthcare, Personal Care, Debt Payments, Savings, Other
- **Custom Categories**: Add your own budget categories
- **Budget vs Actual**: Real-time comparison showing how much you've spent vs budgeted
- **Progress Tracking**: See percentage used for each category

### 💰 Debt Payoff Calculator
- **Credit Card Management**: Track multiple credit cards with balances, interest rates, and minimum payments
- **Payoff Methods**:
  - **Avalanche Method**: Pay highest interest rate first (saves most money)
  - **Snowball Method**: Pay smallest balance first (quick wins for motivation)
- **Extra Payment Support**: Factor in additional monthly payments
- **Detailed Results**: See total payoff time, interest paid, and average monthly payment

### 🏦 Savings Tracker
- **Balance History**: Track your savings balance over time
- **Visual Trends**: Line chart showing savings growth
- **Notes**: Add context to each savings entry

## 🎨 Design
- Modern blue-themed UI throughout
- Sidebar navigation for easy access to all sections
- Responsive layouts that adapt to window size
- Professional dashboard-style interface

## 💾 Data Persistence
All data is automatically saved to:
```
%LocalAppData%\DebtPayoffCalculator\
```

Files:
- `transactions.json` - All your transactions
- `budget.json` - Budget categories and amounts
- `creditcards.json` - Credit card debt information
- `savings.json` - Savings history

## 🚀 How to Run

### From Command Line:
```powershell
dotnet run --project DebtPayoffCalculator/DebtPayoffCalculator.csproj
```

### From Visual Studio:
1. Open `Debt.slnx` in Visual Studio
2. Set `DebtPayoffCalculator` as the startup project
3. Press F5 to run

## 📝 CSV Import Format

To import transactions, create a CSV file with these columns:
```csv
Date,Description,Amount,Category
2024-01-15,Grocery Store,-125.50,Food & Dining
2024-01-16,Paycheck,2500.00,Income
```

- **Date**: Format: YYYY-MM-DD or MM/DD/YYYY
- **Description**: Any text
- **Amount**: Positive for income, negative for expenses (or just use the absolute value)
- **Category**: Optional - will default to "Other" if not specified

## 🎯 Usage Tips

1. **Start with Budget**: Set up your monthly budget in the Budget section first
2. **Enter Transactions**: Add your income and expenses regularly
3. **Track Debt**: Enter all credit cards in the Debt Payoff section
4. **Monitor Progress**: Check the Dashboard to see your financial health
5. **Save Regularly**: Update your savings balance when it changes

## 🛠️ Technologies Used

- **.NET 8.0** - Modern cross-platform framework
- **WPF** - Windows Presentation Foundation for rich UI
- **LiveCharts2** - Beautiful interactive charts
- **CsvHelper** - CSV file parsing
- **MVVM Pattern** - Clean separation of concerns

## 📦 Dependencies

- LiveChartsCore.SkiaSharpView.WPF (2.0.0-rc2)
- CsvHelper (30.0.1)

## 🔄 Data Flow

1. User enters data in any section
2. ViewModel processes and validates the data
3. Service layer saves to JSON files in AppData
4. Dashboard automatically refreshes on navigation
5. Data persists between sessions

## 🎨 Color Scheme

- Primary Blue: #1E88E5
- Dark Blue: #1565C0
- Light Blue: #42A5F5
- Background: #F5F9FC
- Success Green: #4CAF50
- Warning Orange: #FF9800
- Danger Red: #F44336

## 📄 License

This is a personal finance management tool. Use responsibly and always back up your financial data.

---

**Note**: This application stores data locally on your computer. No data is sent to external servers or cloud services.
