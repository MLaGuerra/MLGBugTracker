using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MLGBugTracker.Startup))]
namespace MLGBugTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
