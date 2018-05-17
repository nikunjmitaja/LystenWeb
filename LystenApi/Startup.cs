using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using SuperWebSocket;
using SuperSocket.SocketBase;
using LystenApi.Utility;

[assembly: OwinStartup(typeof(LystenApi.Startup))]

namespace LystenApi
{
    partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigurationApi(app);

          


            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
