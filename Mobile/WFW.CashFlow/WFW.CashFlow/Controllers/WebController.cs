using System;
using System.Web.Http;
using CashFlow.BusinessLogic.Helpers;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Models;
using CashFlow.Storage;
using WFW.CashFlow.Models;

namespace WFW.CashFlow.Controllers
{
    public class WebController : ApiController
    {
        private readonly FinanceHelper _financeHelper;
        private readonly ISettingsStorage _settings;
        private readonly IDataStorage _storage;

        public WebController(FinanceHelper financeHelper, ISettingsStorage settings, IDataStorage storage)
        {
            _financeHelper = financeHelper;
            _settings = settings;
            _storage = storage;
        }


        [HttpGet]
        public PlaneStateModel PlanState()
        {
            var model = new PlaneStateModel
            {
                PlanState = _financeHelper.GetCurrentPlanState()
            };
            var secondCur = _settings.ReadSetting(SettingsNames.SecondCurrency);
            if (!string.IsNullOrEmpty(secondCur) && model.PlanState != null)
            {
                var cur = _storage.GetCurrency(secondCur);
                model.SecondCurrency = new SecondCurrencyState
                {
                    Code = secondCur,
                    MainState = (model.PlanState.PlannedSpend - model.PlanState.PlanSpent) * cur.Rate,
                    LeftPlan = (model.PlanState.PlanLimit - model.PlanState.PlanSpent) >=0?
                        (model.PlanState.PlanLimit - model.PlanState.PlanSpent) * cur.Rate:0,
                    LeftDay = (model.PlanState.DayLimit - model.PlanState.DaySpent) >0?
                        (model.PlanState.DayLimit - model.PlanState.DaySpent) * cur.Rate:0
                };
            }
            return model;
        }
    }
}
