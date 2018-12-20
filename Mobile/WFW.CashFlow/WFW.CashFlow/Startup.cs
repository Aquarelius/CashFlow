using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WFW.CashFlow.Startup))]
namespace WFW.CashFlow
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
