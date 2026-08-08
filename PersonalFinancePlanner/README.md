# Personal Finance Planner

A comprehensive WPF desktop application for managing personal finances, tracking expenses, budgeting, debt management, and savings goals.

## Features

### 📊 Dashboard
- Real-time financial overview and summary statistics
- Visual representation of financial health
- Quick access to key metrics

### 💰 Transaction Management
- Track multiple transaction types:
  - **Expenses** - Daily spending and bills
  - **Income** - Salary, wages, and other income sources
  - **Savings** - Money set aside for savings goals
  - **Debt Payments** - Credit card and loan payments
- Categorize transactions for better organization
- Date-based transaction tracking
- Add, edit, and delete transactions
- Filter and search transaction history

### 📈 Budget Tracking
- Create and manage budget categories
- Set spending limits for each category
- Monitor budget vs. actual spending
- Visual indicators for budget adherence

### 💳 Debt Management
- Track multiple credit cards and debts
- Monitor balances, interest rates, and minimum payments
- Create debt payoff plans
- **Payoff Schedule Window** - Visualize and plan debt reduction strategies
- Payment history tracking
- Plan adjustments and progress monitoring

### 🎯 Savings Goals
- Set and track savings goals
- Monitor progress toward financial targets
- Record savings contributions
- Track savings over time

## Technical Architecture

### Design Pattern
- **MVVM (Model-View-ViewModel)** - Clean separation of concerns
- Command pattern for user interactions
- Data binding for reactive UI updates

### Technology Stack
- **Framework**: .NET with WPF (Windows Presentation Foundation)
- **Language**: C#
- **UI**: XAML with modern blue-themed design system

### Project Structure
```
PersonalFinancePlanner/
├── Models/                    # Data models
│   ├── Transaction.cs
│   ├── BudgetCategory.cs
│   ├── CreditCard.cs
│   ├── DebtPlan.cs
│   ├── SavingsEntry.cs
│   ├── PaymentHistory.cs
│   ├── PayoffScheduleEntry.cs
│   └── PlanAdjustment.cs
├── ViewModels/                # View models (business logic)
│   ├── DashboardViewModel.cs
│   ├── TransactionsViewModel.cs
│   ├── BudgetViewModel.cs
│   ├── DebtViewModel.cs
│   ├── SavingsViewModel.cs
│   ├── PayoffScheduleViewModel.cs
│   └── NavigationViewModel.cs
├── Views/                     # UI views
│   ├── DashboardView.xaml
│   ├── TransactionsView.xaml
│   ├── BudgetView.xaml
│   ├── DebtView.xaml
│   ├── SavingsView.xaml
│   └── PayoffScheduleWindow.xaml
├── Services/                  # Business logic services
│   ├── DataService.cs
│   ├── TransactionService.cs
│   ├── BudgetService.cs
│   ├── DebtPlanService.cs
│   ├── SavingsService.cs
│   └── CreditCardPaymentService.cs
├── App.xaml                   # Application resources and styles
└── MainWindow.xaml            # Main application window
```

## Design System

### Color Palette
The application uses a professional blue-themed color scheme:
- **Primary Blue**: #1E88E5
- **Dark Blue**: #1565C0
- **Light Blue**: #42A5F5
- **Accent Blue**: #2196F3
- **Background**: #F5F9FC
- **Card Background**: #FFFFFF

### UI Components
- Modern rounded corners (5-10px radius)
- Consistent spacing and padding
- Custom styled buttons, text boxes, and form controls
- Hover effects for interactive elements
- Disabled state styling

## Getting Started

### Prerequisites
- Windows OS
- .NET Framework or .NET 6+ SDK
- Visual Studio 2019 or later (recommended)

### Installation
1. Clone the repository
2. Open `PersonalFinancePlanner.sln` in Visual Studio
3. Restore NuGet packages
4. Build the solution (Ctrl+Shift+B)
5. Run the application (F5)

### First-Time Setup
1. Launch the application
2. Navigate through the sidebar to access different modules
3. Start by adding transactions or setting up your budget
4. Add credit cards in the Debt section if needed
5. Set savings goals in the Savings section

## Usage

### Adding a Transaction
1. Go to the **Transactions** view
2. Fill in the transaction details:
   - Date
   - Description
   - Amount
   - Category
   - Type (Expense/Income/Savings/Debt Payment)
3. For Debt Payments, select the credit card
4. Click "Add Transaction"

### Managing Debt
1. Navigate to the **Debt** view
2. Add your credit cards with balance and interest rate information
3. Create a debt payoff plan
4. Use the **Payoff Schedule** window to visualize your repayment strategy
5. Record payments as you make them

### Tracking Budget
1. Go to the **Budget** view
2. Create budget categories
3. Set spending limits
4. Monitor your spending against the budget

### Setting Savings Goals
1. Open the **Savings** view
2. Define your savings goals with target amounts
3. Record contributions regularly
4. Track progress toward your goals

## Data Persistence
The application uses a service-based architecture for data management. All financial data is managed through dedicated services:
- TransactionService for transaction operations
- BudgetService for budget management
- DebtPlanService for debt tracking
- SavingsService for savings goals
- CreditCardPaymentService for payment processing

## Future Enhancements
- Data export to CSV/Excel
- Reporting and analytics
- Multi-currency support
- Recurring transaction templates
- Mobile companion app
- Cloud sync capabilities

## Contributing
Contributions are welcome! Please ensure code follows the existing MVVM pattern and maintains the design system consistency.

## Support
For issues, questions, or feature requests, please open an issue in the repository.

---

**Version**: 1.0  
**Last Updated**: 2026  
**Platform**: Windows Desktop (WPF)
