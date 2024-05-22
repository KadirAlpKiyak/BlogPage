using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FİNALPROJE.Startup))]
namespace FİNALPROJE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
