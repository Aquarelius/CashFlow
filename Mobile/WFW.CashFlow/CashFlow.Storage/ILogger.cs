using System;
using CashFlow.Domain.Enums;

namespace CashFlow.Storage
{
    public interface ILogger
    {
        void Write(Severities severity, string text);
        void Write(Severities severity, string text, Exception ex);
    }
}
