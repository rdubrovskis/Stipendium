using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Stipendium.Startup))]
namespace Stipendium
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
