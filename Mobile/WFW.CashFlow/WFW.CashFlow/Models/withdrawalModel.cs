namespace WFW.CashFlow.Models
{
    public class WithdrawalModel
    {
        public string AccountCurrency { get; set; }
        public double FromAmount { get; set; }
        public string CashCurrency { get; set; }
        public double CashAmount { get; set; }
        public double FeeAmount { get; set; }
        public bool UpdateCurrencyRate { get; set; }
    }
}