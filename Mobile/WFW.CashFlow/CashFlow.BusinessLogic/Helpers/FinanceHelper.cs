using System;
using System.Linq;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Models;
using CashFlow.Storage;

namespace CashFlow.BusinessLogic.Helpers
{
    public class FinanceHelper
    {
        private readonly IDataStorage _storage;
        private readonly ILogger _logger;
        private readonly ProvidersManager _providers;
        private readonly ConvertHelper _convertor;

        public FinanceHelper(IDataStorage storage, ILogger logger, ProvidersManager providers, ConvertHelper convertor)
        {
            _storage = storage;
            _logger = logger;
            _providers = providers;
            _convertor = convertor;
        }

        public PlanState GetCurrentPlanState(DateTime? date = null)
        {
            var dt = date ?? DateTime.UtcNow;
            PlanState res = null;

            try
            {
                var plan = _storage.GetActualPlan(dt);
                if (plan != null)
                {
                    var st = plan.StartDate;
                    var en = plan.EndDate;

                    var bc = _storage.GetBaseCurrency();
                    var transactions = _storage.GetTransactions(st, en);
                    var days = Math.Ceiling(plan.EndDate.Subtract(st).TotalDays);

                    var alreadySpent = transactions.Where(t => t.Amount < 0 && t.TransactionDate < dt.Date).Sum(t => -1 * t.BaseCurrencyAmount);
                    var todaySpent = transactions.Where(t => t.Amount < 0 && t.TransactionDate >= dt.Date && t.TransactionDate <= dt).Sum(t => -1 * t.BaseCurrencyAmount);

                    var curDayNumber = Math.Floor(dt.Subtract(st).TotalDays);
                    var dailyBudget = (plan.Amount - alreadySpent) / (days - curDayNumber);

                    var planSeconds = plan.EndDate.Subtract(st).TotalSeconds;

                    //number of seconds that will be at end of day
                    var currentSeconds = _convertor.GetUtcDateTime(new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59)).Subtract(st).TotalSeconds;

                    var plannedSpent = plan.Amount * (currentSeconds / planSeconds);

                    res = new PlanState
                    {
                        PlanId = plan.Id,
                        StartDate = plan.StartDate,
                        EndDate = plan.EndDate,
                        DayLimit = dailyBudget,
                        PlanLimit = plan.Amount,
                        PlanSpent = alreadySpent + todaySpent,
                        DaySpent = todaySpent,
                        PlannedSpend = plannedSpent,
                        Currency = bc.Code
                    };

                }
            }
            catch (Exception e)
            {
                _logger.Write(Severities.Error, "Cannot calculate plan status", e);
            }
            return res;
        }

        public bool UpdateCalendarsFromPlan(DateTime? date = null)
        {
            var res = false;
            var dt = date ?? _convertor.GetActualDateTime( DateTime.UtcNow);
            try
            {
                var plan = _storage.GetActualPlan(dt);
                if (plan != null)
                {
                    var transactions = _storage.GetTransactions(plan.StartDate, plan.EndDate);
                    foreach (var provider in _providers.GetCalendarProviders())
                    {
                        try
                        {
                            var ct = _convertor.GetActualDateTime(  plan.StartDate);
                            while (ct.Date <= dt.Date)
                            {
                                var trs = transactions
                                    .Where(z => z.TransactionDate.Date == ct.Date && z.BaseCurrencyAmount < 0)
                                    .Sum(z => -1 * z.BaseCurrencyAmount);
                                provider.SetAmountToDate(ct, trs);
                                ct = ct.AddDays(1);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Write(Severities.Error, $"Cannont update calendar throw {provider.GetType()} provider", ex);
                        }
                    }
                    res = true;
                }
            }
            catch (Exception e)
            {
                _logger.Write(Severities.Error, "Cannot update calendars", e);
            }
            return res;
        }
    }
}
