using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TheStudyList.Startup))]
namespace TheStudyList
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
