using CashFlow.Domain.Models;

namespace WFW.CashFlow.Models
{
    public class PlaneStateModel
    {
        public PlanState PlanState { get; set; }
        public SecondCurrencyState SecondCurrency { get; set; }
    }

    public class SecondCurrencyState
    {
        public string Code { get; set; }
        public double MainState { get; set; }
        public double LeftPlan { get; set; }
        public double LeftDay { get; set; }
    }
}