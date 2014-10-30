using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BugHound.Startup))]
namespace BugHound
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
