namespace CashFlow.BusinessLogic.Helpers
{
    public interface ICurrencyClient
    {
        double GetRate(string currency);
        bool GetRateAndUpdate(string currency);
    }
}