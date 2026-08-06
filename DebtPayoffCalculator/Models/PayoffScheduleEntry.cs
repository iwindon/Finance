using System;

namespace DebtPayoffCalculator.Models
{
    public class PayoffScheduleEntry
    {
        public int MonthNumber { get; set; }
        public DateTime Date { get; set; }
        public string CardName { get; set; } = string.Empty;
        public decimal StartingBalance { get; set; }
        public decimal Payment { get; set; }
        public decimal InterestCharged { get; set; }
        public decimal PrincipalPaid { get; set; }
        public decimal EndingBalance { get; set; }
        public bool IsPaidOff { get; set; }
        public bool IsRolloverMonth { get; set; }
        public decimal RolloverAmount { get; set; }
        public bool IsPriorityCard { get; set; }
        public bool IsHistorical { get; set; } // Indicates this is actual historical data vs projection
    }
}
