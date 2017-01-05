using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Clear.Web.Startup))]
namespace Clear.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
