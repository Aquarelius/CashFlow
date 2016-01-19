using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CashFlow.Startup))]
namespace CashFlow
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
