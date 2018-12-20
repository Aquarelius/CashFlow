using System;

namespace CashFlow.Domain.Models
{
    public class PlanState
    {

        public int PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double PlanLimit { get; set; }
        public double DayLimit { get; set; }
        public double PlanSpent { get; set; }
        public double DaySpent { get; set; }

        public double PlannedSpend { get; set; }
        public string Currency { get; set; }
        public double DayPercentage
        {
            get
            {
                if (DayLimit == 0) return 0;
                return (DaySpent / DayLimit) * 100;
            }
        }

        public double PlanPercentage
        {
            get
            {
                if (PlanLimit == 0) return 0;
                return (PlanSpent / PlanLimit) * 100;
            }
        }
    }
}
