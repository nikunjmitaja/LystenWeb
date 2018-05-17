using LystenApi.Utility;
using Microsoft.Web.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;

namespace LystenApi
{
    /// <summary>
    /// Summary description for testing
    /// </summary>
    public class testing12 : IHttpHandler
    {
        private static WebSocketCollection clients = new WebSocketCollection();

        public void ProcessRequest(HttpContext context)
        {
            Exception Ex = new Exception("1123123");
            CommonServices.ErrorLogging(Ex);
            if (context.IsWebSocketRequest)
                context.AcceptWebSocketRequest(ProcessWSChat);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
            

        private async Task ProcessWSChat(AspNetWebSocketContext context)
        {
            Exception Ex = new Exception("1");
            CommonServices.ErrorLogging(Ex);
         
            Exception Ex1 = new Exception("2");
            CommonServices.ErrorLogging(Ex1);
            WebSocket socket = context.WebSocket;
            Exception ex = new Exception(Convert.ToString("123"));
            CommonServices.ErrorLogging(ex);
            while (true)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                if (socket.State == WebSocketState.Open)
                {
                    string userMessage = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                    userMessage = "You sent: " + userMessage + " at " + DateTime.Now.ToLongTimeString();
                    buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(userMessage));
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    break;
                }
            }

        }
    }
}