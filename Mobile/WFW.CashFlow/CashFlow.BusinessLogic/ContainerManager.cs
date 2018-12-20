using Autofac;
using CashFlow.BusinessLogic;

namespace CashFlow.Resolution
{
    public static class ContainerManager
    {
        public static IContainer GetContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new BusinessManagersModule());
            var container = builder.Build();
            return container;
        }
    }
}
