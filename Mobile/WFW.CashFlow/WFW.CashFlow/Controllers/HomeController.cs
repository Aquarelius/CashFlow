using System.Web.Mvc;
using CashFlow.Storage;
using WFW.CashFlow.Models;

namespace WFW.CashFlow.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataStorage _storage;

        public HomeController(IDataStorage storage)
        {
            _storage = storage;
        }

        // GET: Home
        public ActionResult Index()
        {
            return RedirectToAction("Index", "FastWidget");
            var model = new HomePageModel
            {
                Currencies = _storage.CurrenciesList
            };
            return View(model);
        }
    }
}