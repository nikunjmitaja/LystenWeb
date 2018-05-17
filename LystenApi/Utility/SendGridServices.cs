using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using SendGrid;
//using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace LystenApi.Utility
{
    public static class SendGridServices
    {

        private static void Main(string SendGrid, string body,string From,string To,string Subject)
        {
            Execute(SendGrid, body,From,To,Subject).Wait();
        }

        public static async Task Execute(string SendGrid, string body, string From, string To, string Subject)
        {
            //"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"
            //  var apiKey = Environment.GetEnvironmentVariable(SendGrid);
              
              //var client = new SendGridClient(SendGrid);
              //var from = new EmailAddress(From);
              //var subject = Subject;
              //var to = new EmailAddress(To);
              //var plainTextContent = "";
              //var htmlContent = body;
              //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
              //var response = await client.SendEmailAsync(msg);

            
           
        }

    }
}