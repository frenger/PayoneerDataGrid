using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PayoneerExercise.Startup))]
namespace PayoneerExercise
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
