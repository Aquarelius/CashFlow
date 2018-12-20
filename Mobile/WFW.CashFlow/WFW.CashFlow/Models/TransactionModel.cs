using System;
using System.Collections.Generic;

namespace WFW.CashFlow.Models
{
    public class TransactionModel
    {
        public int Id { get; set; }
        public DateTime TransactionTime { get; set; }
        public double Amount { get; set; }
        public double BaseCurrencyAmount { get; set; }
        public string Currency { get; set; }
        public string Comment { get; set; }
        public string Time => TransactionTime.ToString("HH:mm");
    }

    public class TransactionPageModel
    {
        public Dictionary<DateTime, List<TransactionModel>> SortedTransactions { get; set; }
        public string BaseCurrency { get; set; }
    }
}