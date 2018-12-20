using Autofac;
using CashFlow.BusinessLogic.Helpers;
using CashFlow.Storage;
using CashFlow.Storage.Log;
using CashFlow.Storage.SQLStorage;

namespace CashFlow.Resolution
{
    public class BusinessManagersModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StorageContext>().AsSelf();
            builder.RegisterType<SqlDataStorage>().As<IDataStorage>();
            builder.RegisterType<ElmahLogger>().As<ILogger>();
            builder.RegisterType<CurrencyClient>().As<ICurrencyClient>();
            builder.RegisterType<FinanceHelper>().AsSelf();
            builder.RegisterType<SqlSettingsStorage>().As<ISettingsStorage>();
            builder.RegisterType<ProvidersManager>().AsSelf();
        }
    }
}
