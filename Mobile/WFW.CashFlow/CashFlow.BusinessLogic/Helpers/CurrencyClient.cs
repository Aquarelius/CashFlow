using System;
using System.Collections.Generic;
using System.ServiceModel;
using CashFlow.BusinessLogic.Properties;
using CashFlow.Domain.Enums;
using CashFlow.Storage;

namespace CashFlow.BusinessLogic.Helpers
{
    public class CurrencyClient : ICurrencyClient
    {
        private readonly IDataStorage _storage;
        private readonly ILogger _logger;
        public CurrencyClient(IDataStorage storage, ILogger logger)
        {
            _storage = storage;
            _logger = logger;
        }

        public double GetRate(string currency)
        {
            double res = 0;
            try
            {
                var baseCur = _storage.GetBaseCurrency();
                if (baseCur == null)
                {
                    _logger.Write(Severities.Warning, "Not found base currency");
                    return 0;
                }
                if (baseCur.Code == currency)
                {
                    _logger.Write(Severities.Warning, "Impossible get rate for base currency");
                    return 0;
                }
                //https://openexchangerates.org
                //https://github.com/eliefaart/OpenExchangeRates
                var client = new OpenExchangeRates.OpenExchangeRatesClient(Settings.Default.CurrencyServiceKey);
                var lst = client.GetExchangeRates(baseCur.Code, new List<string>() { currency }).Rates;
                if (lst.ContainsKey(currency))
                {
                    res = (double) lst[currency];
                }
                else
                {
                    _logger.Write(Severities.Warning, $"Rate for currency {currency} not found");
                }
            }
            catch (Exception e)
            {
                _logger.Write(Severities.Error, "Error getting currency rate", e);
            }
            return res;
        }

        public bool GetRateAndUpdate(string currency)
        {
            var rate = GetRate(currency);
            if (rate <= 0)
            {
                _logger.Write(Severities.Warning, $"Cannot gate rate for currency {currency}");
                return false;
            }
            var res = _storage.UpdateCurrency(
                new Domain.Models.Currency { Code = currency, IsBaseCurrency = false, Rate = rate });

            return res;
        }
    }
}
