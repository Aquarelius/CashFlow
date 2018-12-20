using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using CashFlow.Resolution;

namespace CashFlow.Storage
{
    public class ProvidersManager
    {
        public IList<ICalendarProvider> GetCalendarProviders()
        {

            var list = new List<ICalendarProvider>();
            var providers = Assembly.GetExecutingAssembly().GetTypes();
            if (providers.Any())
            {
                var container = ContainerManager.GetContainer();
                foreach (var provider in providers)
                {
                    if (provider.GetInterfaces().Contains(typeof(ICalendarProvider)) && provider.IsClass)
                    {
                        list.Add((ICalendarProvider)container.Resolve(provider));
                    }
                }
            }
            return list;
        }
    }
}
