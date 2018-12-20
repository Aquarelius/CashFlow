using System;
using CashFlow.Domain.Enums;
using CashFlow.Storage.Properties;
using Elmah;

namespace CashFlow.Storage.Log
{
    public class ElmahLogger : SqlErrorLog, ILogger
    {
        public ElmahLogger() : base(Settings.Default.DBConnectionString)
        {
            ApplicationName = "CashFlow";
        }

        public void Write(Severities severity, string text)
        {
            var err = new Error()
            {
                Time = DateTime.UtcNow,
                Message = $"[{severity}] {text}"
            };
            Log(err);
        }

        public void Write(Severities severity, string text, Exception ex)
        {
            var err = new Error(ex)
            {
                Message = $"[{severity}] {text}"
            };
            Log(err);
        }
    }
}
