using LystenApi.Db;
using LystenApi.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace LystenApi.Helpers
{
    public static class EmailHelper
    {

        public static bool SendEmail(string Email ,string UserName,string Password)
        {


            using (LystenEntities db = new LystenEntities())
            {
                try
                {
                    var template = db.EmailTemplates.Where(x => x.SystemName == "system.account.verification").FirstOrDefault();
                    var emailaccount = db.EmailAccounts.FirstOrDefault();
                    var _password =  SecutiryServices.DecodeFrom64(Password);
                    MailMessage mailMsg = new MailMessage();
                    mailMsg.To.Add(new MailAddress(Email, ""));
                    mailMsg.From = new MailAddress(emailaccount.EmailId, "Lysten");

                    //mailMsg.To.Add(new MailAddress(Email, "Mitaja Corp."));
                    //mailMsg.From = new MailAddress("noreply@mitajacorp.com", "Mitaja Corp.");
                    mailMsg.Subject = template.Subject;
                    string body = template.Body;
                    body = body.Replace("{Username}", UserName);
                    body = body.Replace("{Password}", _password);
                    mailMsg.IsBodyHtml = true;
                    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html));
                    SmtpClient smtpClient = new SmtpClient(emailaccount.SMTPRelay, Convert.ToInt32(emailaccount.Port));
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(emailaccount.EmailId, emailaccount.Password);
                    smtpClient.Credentials = credentials;
                    smtpClient.EnableSsl = Convert.ToBoolean(emailaccount.EnableSSL);
                    ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate,
                         X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };
                    smtpClient.Send(mailMsg);



                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            //try
            //{
               
            //    MailMessage mailMsg = new MailMessage();
            //    mailMsg.To.Add(new MailAddress(Email, UserName));
            //    mailMsg.From = new MailAddress("testing.mitaja@gmail.com", "Lysten");

            //    //mailMsg.To.Add(new MailAddress(Email, "Mitaja Corp."));
            //    //mailMsg.From = new MailAddress("noreply@mitajacorp.com", "Mitaja Corp.");
            //    mailMsg.Subject = "Account Verified";
            //    string body = "";

            //    body = "--> Thank you for registering as Service Provider with Lysten. Please login with your below credentials: <br/><br/>";
            //    body=body+ "<br/>UserName: " + UserName;
            //    body = body + "<br/>Password: " + Password;
            //    body = body + "<br/>If you have any questions, please contact us xxxx @xx.com";
            //    body = body + "<br/>Registration confirmation email should be sent to the service provider once admin will approve the user from the admin panel.";
            //    body = body + "<br/>Random password for the service provider should be generate and will pass in the email.";
              
            //    mailMsg.IsBodyHtml = true;
            //    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html));
            //    SmtpClient smtpClient = new SmtpClient("testing.mitaja@gmail.com",  465);
            //    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("testing.mitaja@gmail.com", "Testing.Mitaja1");
            //    smtpClient.Credentials = credentials;
            //    smtpClient.EnableSsl = true;
            //    ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            //    { return true; };
            //    smtpClient.Send(mailMsg);
            //}
            //catch
            //{

            //}
            return true;
        }
    }
}