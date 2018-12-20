using System;
using System.Linq;
using Autofac;
using CashFlow.Domain.Models;
using CashFlow.Resolution;
using CashFlow.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CashFlow.BusinessLogic.Helpers;
using CashFlow.Domain.Enums;

namespace WFW.CashFlow.Tests
{
    [TestClass]
    public class UnitTest1
    {

        private IContainer _container;
        private IDataStorage _storage;
        private ProvidersManager _providers;
        private ISettingsStorage _settings;
        private Guid userId;


        [TestInitialize]
        public void Start()
        {
            _container = ContainerManager.GetContainer();
            _storage = _container.Resolve<IDataStorage>();
            _providers = _container.Resolve<ProvidersManager>();
            _settings = _container.Resolve<ISettingsStorage>();
            userId = Guid.Parse("bd0b506b-1bf4-449b-975c-15964d1dca6e");
        }

        [TestMethod]
        public void CheckCurrencyUpdate()
        {
            var cur = new Currency
            {
                Code = "USD",
                IsBaseCurrency = true,
                Rate = 1
            };
            Assert.IsTrue(_storage.UpdateCurrency(cur));
        }

        [TestMethod]
        public void CheckCurrenciesList()
        {
            
            var list = _storage.CurrenciesList;
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void CheckCurrencyClient()
        {
            var client = _container.Resolve<ICurrencyClient>();
            Assert.IsTrue(client.GetRateAndUpdate("MYR"));
        }

        [TestMethod]
        public void CheckRegisterСonsumption()
        {
             //Assert.IsTrue(_storage.RegisterСonsumption(0, "USD", "Test consumption"));
            Assert.IsTrue(_storage.RegisterСonsumption(20.75, "USD", "Продукты", userId, new DateTime (2017,8,9,7,0,0)));


        }

        [TestMethod]
        public void CheckPlansManiplation()
        {

            var plan = new Plan
            {
                StartDate = new DateTime(2017, 8, 2),
                EndDate = new DateTime(2017, 8, 8,23,59,59),
                Amount = 210
            };
            var resPlan = _storage.CreatePlan(plan);
            Assert.AreNotEqual(resPlan.Id, 0);
            var list = _storage.GetPlans(new DateTime(2017, 7, 1), new DateTime(2017, 7, 5));
            Assert.IsTrue(list.Any(p => p.Id == resPlan.Id));
            Assert.IsNotNull(_storage.GetActualPlan(new DateTime(2017, 7, 1)));
            Assert.IsTrue(_storage.DeletePlan(resPlan.Id));
        }

        [TestMethod]
        public void CheckPlanState()
        {
            var list = _storage.GetPlans(new DateTime(2017, 6, 6), new DateTime(2017, 6, 11));
            if (!list.Any())
            {
                var plan = new Plan
                {
                    StartDate = new DateTime(2017, 6, 5),
                    EndDate = new DateTime(2017, 6, 12),
                    Amount = 350
                };
                _storage.CreatePlan(plan);
                Assert.IsTrue(_storage.RegisterСonsumption(500, "THB", "Test consumption",userId, new DateTime(2017, 6, 6, 9, 0, 0)));
                Assert.IsTrue(_storage.RegisterСonsumption(50, "USD", "Test consumption", userId, new DateTime(2017, 6, 5, 15, 0, 0)));
            }

            var helper = _container.Resolve<FinanceHelper>();
            var ps = helper.GetCurrentPlanState(new DateTime(2017, 6, 6, 10, 15, 0));
            Assert.IsNotNull(ps);
            Assert.AreNotEqual(ps.DayPercentage, 0);
        }

        [TestMethod]
        public void CheckProviders()
        {
            var lst = _providers.GetCalendarProviders();
            Assert.IsTrue(lst.Count > 0);
            foreach (var provider in lst)
            {
                var rs = provider.GetAmountForDate(new DateTime(2017, 7, 1));
                provider.SetAmountToDate(new DateTime(2017, 7, 1), rs);
            }
        }
        [TestMethod]
        public void CheckSyncPlanToCalendar()
        {
            var helper = _container.Resolve<FinanceHelper>();
            helper.UpdateCalendarsFromPlan();
        }

        [TestMethod]
        public void CheckSettings()
        {
            var testValue = Guid.NewGuid().ToString();
            _settings.WriteSettings(SettingsNames.Test, testValue);
            var val = _settings.ReadSetting(SettingsNames.Test);
            Assert.AreEqual(testValue, val);
            _settings.WriteSettings(SettingsNames.SecondCurrency, "THB");
        }
    }
}
