using System;

namespace CashFlow.Storage
{
    public interface ICalendarProvider
    {
        double GetAmountForDate(DateTime date);
        void SetAmountToDate(DateTime date, double amount);
    }
}
