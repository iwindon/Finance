using System;
using System.Collections.Generic;
using System.Linq;
using PersonalFinancePlanner.Models;

namespace PersonalFinancePlanner.Services
{
    public class CreditCardPaymentService
    {
        private readonly DataService _dataService;

        public CreditCardPaymentService()
        {
            _dataService = new DataService();
        }

        /// <summary>
        /// Calculates the interest accrued since the last payment (or for the full month if no previous payment)
        /// </summary>
        public decimal CalculateAccruedInterest(CreditCard card, DateTime paymentDate)
        {
            if (card.Balance <= 0)
                return 0;

            // Calculate daily interest rate
            decimal annualRate = card.InterestRate / 100m; // Convert percentage to decimal
            decimal dailyRate = annualRate / 365m;

            // Calculate days since last payment or use 30 days as default
            int daysSinceLastPayment = 30;
            if (card.LastPaymentDate.HasValue)
            {
                daysSinceLastPayment = (paymentDate - card.LastPaymentDate.Value).Days;
                if (daysSinceLastPayment <= 0)
                    daysSinceLastPayment = 1; // At least 1 day
            }

            // Calculate interest: Balance × Daily Rate × Days
            decimal interest = card.Balance * dailyRate * daysSinceLastPayment;
            return Math.Round(interest, 2);
        }

        /// <summary>
        /// Applies a payment to a credit card, splitting it between interest and principal
        /// </summary>
        public PaymentBreakdown ApplyPayment(CreditCard card, decimal paymentAmount, DateTime paymentDate, string notes = "")
        {
            var breakdown = new PaymentBreakdown();

            // Calculate accrued interest
            breakdown.InterestPaid = CalculateAccruedInterest(card, paymentDate);

            // Ensure interest doesn't exceed payment amount
            if (breakdown.InterestPaid > paymentAmount)
            {
                breakdown.InterestPaid = paymentAmount;
                breakdown.PrincipalPaid = 0;
            }
            else
            {
                breakdown.PrincipalPaid = paymentAmount - breakdown.InterestPaid;
            }

            // Ensure principal doesn't exceed current balance
            if (breakdown.PrincipalPaid > card.Balance)
            {
                breakdown.PrincipalPaid = card.Balance;
            }

            // Update card balance
            decimal oldBalance = card.Balance;
            card.Balance = Math.Max(0, card.Balance - breakdown.PrincipalPaid);
            card.LastPaymentDate = paymentDate;

            // Create payment history entry
            var historyEntry = new PaymentHistory
            {
                PaymentDate = paymentDate,
                PaymentAmount = paymentAmount,
                InterestPaid = breakdown.InterestPaid,
                PrincipalPaid = breakdown.PrincipalPaid,
                RemainingBalance = card.Balance,
                Notes = notes
            };

            // Add to payment history
            if (card.PaymentHistory == null)
            {
                card.PaymentHistory = new List<PaymentHistory>();
            }
            card.PaymentHistory.Add(historyEntry);

            breakdown.NewBalance = card.Balance;
            breakdown.OldBalance = oldBalance;

            return breakdown;
        }

        /// <summary>
        /// Saves updated credit cards to storage
        /// </summary>
        public void SaveCreditCards(IEnumerable<CreditCard> cards)
        {
            _dataService.SaveCreditCards(cards);
        }

        /// <summary>
        /// Loads credit cards from storage
        /// </summary>
        public List<CreditCard> LoadCreditCards()
        {
            return _dataService.LoadCreditCards();
        }
    }

    /// <summary>
    /// Details about how a payment was applied
    /// </summary>
    public class PaymentBreakdown
    {
        public decimal InterestPaid { get; set; }
        public decimal PrincipalPaid { get; set; }
        public decimal OldBalance { get; set; }
        public decimal NewBalance { get; set; }
    }
}
