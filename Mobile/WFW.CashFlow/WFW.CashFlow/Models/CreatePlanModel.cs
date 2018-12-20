using System;
using CashFlow.Domain.Models;

namespace WFW.CashFlow.Models
{
    public class CreatePlanModel
    {
        public TimeZoneInfo TimeZone { get; set; }

        public Currency BaseCurrency { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string Amount { get; set; }

    }
}