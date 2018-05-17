using LystenApi.Controllers.Api;
using LystenApi.Db;
using LystenApi.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Timers;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LystenApi
{
    public class MvcApplication : System.Web.HttpApplication
    {
        static Timer timer;

        protected void Application_Start()
        {


            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register); // NEW way

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            try
            {
                schedule_Timer();

            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
            }
        }

        public void schedule_Timer()
        {
            Console.WriteLine("### Timer Started ###");

            DateTime nowTime = DateTime.UtcNow;
            DateTime scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, nowTime.Minute, 0, 0);
            if (nowTime > scheduledTime)
            {
                scheduledTime = scheduledTime.AddMinutes(15);
            }


            //Exception asd = new Exception(DateTime.UtcNow.ToString());
            //CommonServices.ErrorLogging(asd);




            double tickTime = (double)(scheduledTime - DateTime.UtcNow).TotalMilliseconds;
            timer = new Timer(tickTime);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }


        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                //Console.WriteLine("### Timer Stopped ### \n");

                timer.Stop();
                var NowUTCdate = DateTime.UtcNow;
                var NowUTCdate15Min = DateTime.UtcNow.AddMinutes(-15);

                using (LystenEntities db = new LystenEntities())
                {
                    var callingdata = (from dd in db.Calling_Request
                                       where dd.AcceptDatetimeUTC >= NowUTCdate15Min && dd.AcceptDatetimeUTC <= NowUTCdate
                                       select dd).ToList();

                    foreach (var item in callingdata)
                    {
                        //if (item.User_Master1.DeviceToken != null || item.User_Master.DeviceToken != "")
                        //{
                        //    sendMsgPreCalling(item.User_Master.Id, item.User_Master.DeviceToken,"TO",item.User_Master1.FullName);
                        //}

                        if (item.User_Master1.DeviceToken != null || item.User_Master1.DeviceToken != "")
                        {
                            if (item.User_Master1.DeviceType == "Android")
                            {
                                Helpers.NotificationHelper.sendMsgPreCalling(item.User_Master1.Id, item.User_Master1.DeviceToken, "FROM", item.User_Master1.FullName);
                            }
                            else
                            {
                                sendMsgPreCalling(item.User_Master1.Id, item.User_Master1.DeviceToken, "FROM", item.User_Master1.FullName);
                            }
                        }
                    }
                }
                //Console.WriteLine("### Scheduled Task Started ### \n\n");
                //Console.WriteLine("Hello World!!! - Performing scheduled task\n");
                //Console.WriteLine("### Task Finished ### \n\n");
                schedule_Timer();
            }
            catch (Exception ex)
            {
                ///throw ex;
            }
            finally
            {
                schedule_Timer();
            }
        }



        public void sendMsgPreCalling(int Id, string devicetocken,string Status,string Fullname)
        {
            string ImagePath = "";
            string name = "";
            var certificatePath = HostingEnvironment.MapPath("~/Lysten-DevB.p12");


            int port = 2195;
            String hostname = (WebConfigurationManager.AppSettings["ApnsEnvironment"]);
            //String hostname = "gateway.push.apple.com";

            string certificatePassword = "";

            X509Certificate2 clientCertificate = new X509Certificate2(certificatePath, certificatePassword, X509KeyStorageFlags.MachineKeySet);
            X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);


            TcpClient client = new TcpClient(hostname, port);

            SslStream sslStream = new SslStream(
                            client.GetStream(),
                            false,
                            new RemoteCertificateValidationCallback(ValidateServerCertificate),
                            null
            );
            try
            {
                sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Tls, false);
            }
            catch (AuthenticationException ex)
            {
                client.Close();
                Exception Eccsssas12 = new Exception("Athentication Failed");
                CommonServices.ErrorLogging(Eccsssas12);
                System.Web.HttpContext.Current.Server.MapPath("~/Authenticationfailed.txt");
                return;
            }

            //// Encode a test message into a byte array.
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);
            writer.Write((byte)0);  //The command
            writer.Write((byte)0);  //The first byte of the deviceId length (big-endian first byte)
            writer.Write((byte)32); //The deviceId length (big-endian second byte)
            byte[] b0 = HexString2Bytes(devicetocken);
            WriteMultiLineByteArray(b0);
            writer.Write(b0);
            String payload;
            string strmsgbody = "";
            int totunreadmsg = 20;
            if (Status == "TO")
            {
                strmsgbody = "You have a call from " +Fullname + " in 15 minutes.be ready!";
            }
            else
            {
                strmsgbody = "Your calling time is in 15 minutes.be ready!";
            }
            payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"acme1\":\"bar\",\"acme2\":42}";
            writer.Write((byte)0); //First byte of payload length; (big-endian first byte)
            writer.Write((byte)payload.Length);     //payload length (big-endian second byte)
            byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
            writer.Write(b1);
            writer.Flush();

            byte[] array = memoryStream.ToArray();
            try
            {
                sslStream.Write(array);
                sslStream.Flush();
            }
            catch
            {
            }
            client.Close();
        }

        private byte[] HexString2Bytes(string hexString)
        {
            //check for null
            if (hexString == null) return null;
            //get length
            int len = hexString.Length;
            if (len % 2 == 1) return null;
            int len_half = len / 2;
            //create a byte array
            byte[] bs = new byte[len_half];
            try
            {
                //convert the hexstring to bytes
                for (int i = 0; i != len_half; i++)
                {
                    bs[i] = (byte)Int32.Parse(hexString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Exception : " + ex.Message);
            }
            //return the byte array
            return bs;
        }
        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
        public static void WriteMultiLineByteArray(byte[] bytes)
        {
            const int rowSize = 20;
            int iter;

            Console.WriteLine("initial byte array");
            Console.WriteLine("------------------");

            for (iter = 0; iter < bytes.Length - rowSize; iter += rowSize)
            {
                Console.Write(
                    BitConverter.ToString(bytes, iter, rowSize));
                Console.WriteLine("-");
            }

            Console.WriteLine(BitConverter.ToString(bytes, iter));
            Console.WriteLine();
        }


    }
}
