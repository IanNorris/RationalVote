using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RationalVote.Startup))]
namespace RationalVote
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
