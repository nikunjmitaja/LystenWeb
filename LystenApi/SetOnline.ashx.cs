using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Web.WebSockets;
using LystenApi.Utility;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.WebSockets;

namespace LystenApi
{
    /// <summary>
    /// Summary description for SetOnline
    /// </summary>
    public class SetOnline : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                if (context.IsWebSocketRequest)
                {
                    context.AcceptWebSocketRequest(new SetOnlineHandler());
                }
                context = null;
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}