using Microsoft.Web.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LystenApi
{
    /// <summary>
    /// Summary description for testing
    /// </summary>
    public class testing : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
            if (context.IsWebSocketRequest)
                context.AcceptWebSocketRequest(new MyWebSocketHandle());
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