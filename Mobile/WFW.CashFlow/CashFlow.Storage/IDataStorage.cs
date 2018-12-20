using System;
using System.Collections.Generic;
using CashFlow.Domain.Models;

namespace CashFlow.Storage
{
    public interface IDataStorage
    {
        bool UpdateCurrency(Currency currency);
        IList<string> CurrenciesList { get; }
        Currency GetCurrency(string currencyCode);
        Currency GetCurrency(int id);
        Currency GetBaseCurrency();
        bool RegisterСonsumption(double amount, string currency, string description, Guid userId,  DateTime? consumptionTime = null);
        Plan CreatePlan(Plan plan);
        bool DeletePlan(int planId);
        IList<Plan> GetPlans(DateTime startDate, DateTime endDate);
        Plan GetActualPlan(DateTime? date = null);
        IList<Transaction> GetTransactions(DateTime startDate, DateTime endDate);
        bool DeleteTransaction(int id, Guid userId);
    }
}
