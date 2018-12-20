using Autofac;
using CashFlow.BusinessLogic.Helpers;
using CashFlow.BusinessLogic.Providers;
using CashFlow.Storage;
using CashFlow.Storage.Log;
using CashFlow.Storage.SQLStorage;
using Google.Apis.Util.Store;

namespace CashFlow.BusinessLogic
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
            builder.RegisterType<GoogleTokensStorage>().As<IDataStore>();
            builder.RegisterType<GoogleSheetProvider>().AsSelf();
            builder.RegisterType<ConvertHelper>().AsSelf();
            builder.RegisterType<UsersHelper>().AsSelf();
            builder.RegisterType<UsersStorage>().As<IUsersStorage>();
        }
    }
}
