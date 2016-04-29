using Microsoft.Owin.Cors;
using Owin;

namespace SignalRHub
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            var x=  app.MapSignalR();
        }
    }
}
