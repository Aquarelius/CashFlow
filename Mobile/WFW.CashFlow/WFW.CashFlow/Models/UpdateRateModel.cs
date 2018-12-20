namespace WFW.CashFlow.Models
{
    public class UpdateRateModel
    {
        public string CurrencyCode { get; set; }

        public bool? Done { get; set; }

        public string RateString { get; set; }
    }
}