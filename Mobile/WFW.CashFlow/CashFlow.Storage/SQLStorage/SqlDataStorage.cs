using System;
using System.Collections.Generic;
using System.Linq;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Models;

namespace CashFlow.Storage.SQLStorage
{
    public class SqlDataStorage : IDataStorage
    {
        private readonly StorageContext _context;
        private readonly ILogger _logger;

        public SqlDataStorage(StorageContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public bool UpdateCurrency(Currency currency)
        {
            if (currency == null) return false;
            var res = false;
            try
            {
                var cur = _context.Currencies.FirstOrDefault(c => c.Code == currency.Code);
                if (cur == null)
                {
                    currency.LastUpdate = DateTime.UtcNow;
                    _context.Currencies.Add(currency);
                    _logger.Write(Severities.Information,
                        $"Currency {currency.Code} added with rate {currency.Rate}");
                }
                else
                {
                    cur.IsBaseCurrency = currency.IsBaseCurrency;
                    cur.Rate = currency.Rate;
                    cur.LastUpdate = DateTime.UtcNow;
                    _logger.Write(Severities.Information,
                        $"Currency {currency.Code} updated with rate {currency.Rate}");
                }
                _context.SaveChanges();

                if (currency.IsBaseCurrency)//Change base currency
                {
                    var oldBase = _context.Currencies.Where(c => c.IsBaseCurrency && c.Code != currency.Code);
                    if (oldBase.Any())
                    {

                        foreach (var cur0 in oldBase)
                        {
                            cur0.IsBaseCurrency = false;
                        }
                        _context.SaveChanges();
                    }
                }
                res = true;
            }
            catch (Exception e)
            {
                _logger.Write(Severities.Error, "Error update currency", e);
                res = false;
            }

            return res;
        }

        public IList<string> CurrenciesList
        {
            get
            {
                var list = new List<string>();
                try
                {
                    list = _context.Currencies.Select(c => c.Code).ToList();
                }
                catch (Exception e)
                {
                    _logger.Write(Severities.Error, "Error getting currencies list", e);
                }
                return list;
            }
        }

        public Currency GetCurrency(string currencyCode)
        {
            Currency res = null;
            try
            {
                res = _context.Currencies.FirstOrDefault(c => c.Code == currencyCode);
            }
            catch (Exception e)
            {
                _logger.Write(Severities.Error, "Error getting currency", e);
            }
            return res;
        }

        public Currency GetCurrency(int id)
        {
            Currency res = null;
            try
            {
                res = _context.Currencies.FirstOrDefault(c => c.Id == id);
            }
            catch (Exception e)
            {
                _logger.Write(Severities.Error, "Error getting currency", e);
            }
            return res;
        }

        public Currency GetBaseCurrency()
        {
            Currency res = null;
            try
            {
                res = _context.Currencies.SingleOrDefault(c => c.IsBaseCurrency);
            }
            catch (Exception e)
            {
                _logger.Write(Severities.Error, "Error getting currency", e);
            }
            return res;
        }

        public bool RegisterСonsumption(double amount, string currency, string description, Guid userId, DateTime? consumptionTime = null)
        {
            var res = false;
            var dt = consumptionTime ?? DateTime.UtcNow;
            try
            {
                if (!_context.Users.Any(z => z.Id.ToLower() == userId.ToString().ToLower()))
                {
                    _logger.Write(Severities.Warning, $"Unknown user id '{userId}'");
                    return false;
                }
                var cur = _context.Currencies.FirstOrDefault(c => c.Code == currency);
                if (cur == null)
                {
                    _logger.Write(Severities.Error, $"Unknown currency '{currency}'");
                }
                else
                {
                    var transaction = new Transaction
                    {
                        Amount = -1 * amount,
                        BaseCurrencyAmount = -1 * (amount / cur.Rate),
                        CurrencyId = cur.Id,
                        Description = description,
                        TransactionDate = dt,
                        UserId = userId
                    };
                    _context.Transactions.Add(transaction);
                    _context.SaveChanges();
                    res = true;
                }
            }
            catch (Exception e)
            {
                _logger.Write(Severities.Error, "Error register transaction", e);
            }

            return res;
        }

        public Plan CreatePlan(Plan plan)
        {
            Plan res = null;
            try
            {
                _context.Plans.Add(plan);
                _context.SaveChanges();
                res = plan;
            }
            catch (Exception e)
            {
                _logger.Write(Severities.Error, "Error creating plan", e);
                res = null;
            }
            return res;
        }

        public bool DeletePlan(int planId)
        {
            var res = false;

            try
            {
                var itms = _context.Plans.Where(c => c.Id == planId);
                if (itms.Any())
                {
                    _context.Plans.RemoveRange(itms);
                    _context.SaveChanges();
                    res = true;
                }
            }
            catch (Exception e)
            {
                _logger.Write(Severities.Error, $"Error deleting plan: {planId}", e);
                res = false;
            }

            return res;
        }

        public IList<Plan> GetPlans(DateTime startDate, DateTime endDate)
        {
            var list = new List<Plan>();

            try
            {
                list = _context.Plans.Where(p => p.EndDate >= startDate && p.StartDate <= endDate).ToList();
            }
            catch (Exception e)
            {
                _logger.Write(Severities.Error, $"Error getting plans list", e);
            }
            return list;
        }

        public Plan GetActualPlan(DateTime? date = null)
        {
            Plan res = null;
            var dt = date ?? DateTime.UtcNow;

            try
            {
                var itms = _context.Plans.Where(p => p.StartDate <= dt && p.EndDate >= dt);
                if (itms.Any())
                {
                    res = itms.First();
                }
            }
            catch (Exception e)
            {
                _logger.Write(Severities.Error, $"Error getting plan", e);
            }
            return res;
        }

        public IList<Transaction> GetTransactions(DateTime startDate, DateTime endDate)
        {
            var list = new List<Transaction>();

            try
            {
                list = _context.Transactions.Where(p => p.TransactionDate >= startDate && p.TransactionDate <= endDate && !p.IsDeleted).ToList();
            }
            catch (Exception e)
            {
                _logger.Write(Severities.Error, $"Error getting transactions list", e);
            }
            return list;
        }

        public bool DeleteTransaction(int id, Guid userId)
        {
            var res = false;

            try
            {
                if (!_context.Users.Any(z => z.Id == userId.ToString()))
                {
                    _logger.Write(Severities.Warning, $"Unknown user id '{userId}'");
                    return false;
                }
                var itms = _context.Transactions.Where(t => t.Id == id);
                if (itms.Any())
                {
                    foreach (var itm in itms)
                    {
                        itm.IsDeleted = true;
                        itm.DeletedAt = DateTime.UtcNow;
                        itm.DeletedBy = userId;

                    }
                  //  _context.Transactions.RemoveRange(itms);
                    _context.SaveChanges();
                    res = true;
                }
            }
            catch (Exception e)
            {
                _logger.Write(Severities.Error, $"Cannot delete transaction {id}", e);
            }

            return res;
        }
    }
}
