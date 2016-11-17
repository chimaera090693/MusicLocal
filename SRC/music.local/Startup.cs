using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(music.local.Startup))]
namespace music.local
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
