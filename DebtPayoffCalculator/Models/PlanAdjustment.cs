using System;

namespace DebtPayoffCalculator.Models
{
    public class PlanAdjustment
    {
        public DateTime AdjustmentDate { get; set; }
        public int EffectiveMonthNumber { get; set; }
        public string AdjustmentType { get; set; } = string.Empty; // "ExtraPaymentChange", "CardAdded", "CardRemoved", "CardBalanceAdjusted"
        public string Description { get; set; } = string.Empty;

        // For extra payment adjustments
        public decimal? OldExtraPayment { get; set; }
        public decimal? NewExtraPayment { get; set; }

        // For card-related adjustments
        public string CardName { get; set; } = string.Empty;
        public decimal? OldBalance { get; set; }
        public decimal? NewBalance { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}
