using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using LystenApi.Db;
using LystenApi.Models;

namespace LystenApi.Utility
{
    public class EmailServices
    {
        //TPEntities db = new TPEntities();
        public void SendUserForgotPassword(string EmailId, string obj)
        {
            using (LystenEntities db = new LystenEntities())
            {
                try
                {
                    var template = db.EmailTemplates.Where(x => x.SystemName == "system.forgot.password").FirstOrDefault();
                    var emailaccount = db.EmailAccounts.FirstOrDefault();
                    MailMessage mailMsg = new MailMessage();
                    mailMsg.To.Add(new MailAddress(EmailId, ""));
                    mailMsg.From = new MailAddress(emailaccount.EmailId, "Lysten");
                    var _password = SecutiryServices.DecodeFrom64(obj);
                    //mailMsg.To.Add(new MailAddress(Email, "Mitaja Corp."));
                    //mailMsg.From = new MailAddress("noreply@mitajacorp.com", "Mitaja Corp.");
                    mailMsg.Subject = template.Subject;
                    string body = template.Body;
                    body = body.Replace("{Username}", EmailId);
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
        }

        public void SendToUser(User_Master obj)
        {
            using (LystenEntities db = new LystenEntities())
            {
                try
                {
                    var template = db.EmailTemplates.Where(x => x.SystemName == "system.user.Password").FirstOrDefault();
                    var emailaccount = db.Settings.Where(x => x.Name == "SendGrid.Key").FirstOrDefault();
                    var Emailfrom = db.Settings.Where(x => x.Name == "SendGrid.FromMail").FirstOrDefault();
                    string body = template.Body;
                    body = body.Replace("{Fullname}", obj.Displayname);
                    body = body.Replace("{Username}", obj.Email);
                    body = body.Replace("{Password}", obj.Password);
                    var res = SendGridServices.Execute(emailaccount.Value, body, Emailfrom.Value, obj.Email, template.Subject);
                }
                catch (Exception ex)
                {
                    CommonServices.ErrorLogging(ex);
                    throw ex;
                }
            }
        }
    }
}