using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CBLSummer09052016Budgeter.Startup))]
namespace CBLSummer09052016Budgeter
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
