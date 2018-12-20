using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CashFlow.BusinessLogic.Helpers;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Extensions;
using CashFlow.Domain.Models;
using CashFlow.Storage;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WFW.CashFlow.Models;

namespace WFW.CashFlow.Controllers
{
    [Authorize]
    public class FastWidgetController : Controller
    {
        private readonly IDataStorage _storage;
        private readonly ISettingsStorage _settings;
        private readonly FinanceHelper _financeHelper;
        private readonly ConvertHelper _converter;
        private readonly ICurrencyClient _currency;
        private readonly UsersHelper _usersHelper;
        public FastWidgetController(IDataStorage storage, ISettingsStorage settings, FinanceHelper financeHelper, ConvertHelper converter, ICurrencyClient currency, UsersHelper usersHelper)
        {
            _storage = storage;
            _settings = settings;
            _financeHelper = financeHelper;
            _converter = converter;
            _currency = currency;
            _usersHelper = usersHelper;
        }

        // GET: FastWidget
       
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult Register()
        {
            var back = Request.QueryString["Return"];
            if (string.IsNullOrEmpty(back)) back = "Index";
            ViewBag.BackLink = back;
            ViewBag.Currencies = _storage.CurrenciesList;
            return View(GetEmptyRegisterModel());
        }

        [HttpPost]
        public ActionResult Register(RegisterСonsumptionModel model)
        {

            var strAmount = model.Amount.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                .Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            double am;
            if (double.TryParse(strAmount, out am))
            {
                ViewBag.Done = _storage.RegisterСonsumption(am, model.Currency, model.Description ?? "",_usersHelper.GetCurrentUserId());
                _settings.WriteSettings(SettingsNames.LastUsedCurrency, model.Currency);
                ModelState.Clear();
                model = GetEmptyRegisterModel();
            }
            else
            {
                ViewBag.Done = false;
            }
            var back = Request.QueryString["Return"];
            if (string.IsNullOrEmpty(back)) back = "Index";
            ViewBag.BackLink = back;
            ViewBag.Currencies = _storage.CurrenciesList;
            return View(model);
        }

        public ActionResult Calendars()
        {
            var model = new CalendarsModel
            {
                CurrentPlan = 1,
                Date = DateTime.UtcNow.ToString("d")
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Calendars(CalendarsModel model)
        {
            DateTime? date = null;
            if (model.CurrentPlan == 0)
            {
                DateTime dt;
                if (DateTime.TryParseExact(model.Date, "dd.MM.yyyy", null, DateTimeStyles.None, out dt)) date = dt;
            }
            if (model.CurrentPlan == 2)
            {
                date = DateTime.UtcNow.AddDays(-1);
            }
            ViewBag.Done = _financeHelper.UpdateCalendarsFromPlan(date);
            return View();
        }

        public ActionResult Transactions()
        {
            var plan = _storage.GetActualPlan();
            var trns = _storage.GetTransactions(plan.StartDate, plan.EndDate);
            var bCurrency = _storage.GetBaseCurrency();

            var model = new TransactionPageModel
            {
                SortedTransactions = new Dictionary<DateTime, List<TransactionModel>>(),
                BaseCurrency = bCurrency.Code
            };
            foreach (var tr in trns.OrderBy(z => z.TransactionDate))
            {
                if (!model.SortedTransactions.ContainsKey(tr.TransactionDate.Date))
                {
                    model.SortedTransactions.Add(tr.TransactionDate.Date, new List<TransactionModel>());
                }
                model.SortedTransactions[tr.TransactionDate.Date].Add(new TransactionModel()
                {
                    Id = tr.Id,
                    Currency = tr.Currency.Code,
                    Amount = tr.Amount,
                    BaseCurrencyAmount = tr.BaseCurrencyAmount,
                    TransactionTime = _converter.GetActualDateTime(tr.TransactionDate),
                    Comment = tr.Description
                });
            }
            return View(model);
        }

        public ActionResult DeleteTransaction(int id)
        {
            _storage.DeleteTransaction(id, _usersHelper.GetCurrentUserId() );
            return RedirectToAction("Transactions");
        }

        public ActionResult CreatePlan()
        {
            var model = new CreatePlanModel
            {
                TimeZone = _converter.GetCurrenTimeZone(),
                BaseCurrency = _storage.GetBaseCurrency()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePlan(CreatePlanModel model)
        {
            DateTime? st = null, et = null;
            double? am = null;
            DateTime lst, let;
            if (DateTime.TryParseExact(model.StartDate, "dd.MM.yyyy", null, DateTimeStyles.None, out lst)) st = _converter.GetUtcDateTime(lst);
            if (DateTime.TryParseExact(model.EndDate, "dd.MM.yyyy", null, DateTimeStyles.None, out let))
            {
                et = _converter.GetUtcDateTime(new DateTime(let.Year, let.Month, let.Day, 23, 59, 59));
            }
            double lam;
            if (double.TryParse(model.Amount, out lam)) am = lam;
            if (st.HasValue && et.HasValue && am.HasValue)
            {
                var plan = _storage.CreatePlan(new Plan()
                {
                    Amount = am.Value,
                    StartDate = st.Value,
                    EndDate = et.Value
                });
                ViewBag.Success = $"Создан план № {plan.Id}. C {_converter.GetActualDateTime(plan.StartDate):M} по {_converter.GetActualDateTime(plan.EndDate):M} ";
                ModelState.Clear();
                return View(new CreatePlanModel
                {

                    TimeZone = _converter.GetCurrenTimeZone(),
                    BaseCurrency = _storage.GetBaseCurrency()
                });
            }
            ViewBag.Error = "Can't parse values";
            model.TimeZone = _converter.GetCurrenTimeZone();
            model.BaseCurrency = _storage.GetBaseCurrency();
            return View(model);
        }

        public ActionResult UpdateRate()
        {
            var lcur = _settings.ReadSetting(SettingsNames.LastUsedCurrency);
            if (string.IsNullOrEmpty(lcur)) lcur = _storage.GetBaseCurrency().Code;
            return View(new UpdateRateModel
            {
                CurrencyCode = lcur
            });
        }

        [HttpPost]
        public ActionResult UpdateRate(UpdateRateModel model)
        {
            model.Done = _currency.GetRateAndUpdate(model.CurrencyCode);
            if (model.Done.Value)
            {
                var cur = _storage.GetCurrency(model.CurrencyCode);
                var bCur = _storage.GetBaseCurrency();
                model.RateString = $"1 {bCur.Code} = {cur.Rate.ToMoneyString(cur.Code)}";
            }
            else
            {
                ViewBag.Error = $"Не удалось обновить курс для {model.CurrencyCode}";
            }
            return View(model);
        }

        private RegisterСonsumptionModel GetEmptyRegisterModel()
        {
            var cur = _settings.ReadSetting(SettingsNames.LastUsedCurrency);
            if (string.IsNullOrEmpty(cur))
            {
                var bCur = _storage.GetBaseCurrency();
                if (bCur != null) cur = bCur.Code;
            }
            var model = new RegisterСonsumptionModel
            {
                Amount = "",
                Currency = cur,
                Description = ""
            };
            return model;
        }
    }
}