using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Backendt1.Startup))]

namespace Backendt1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}