using Braintree;
using LystenApi.ActionFilters;
using LystenApi.Db;
using LystenApi.Utility;
using LystenApi.Utility.ApiServices;
using LystenApi.Utility.Providers;
using LystenApi.ViewModel;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Script.Serialization;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using PushSharp.Apple;
using PushSharp.Core;
using Newtonsoft.Json.Linq;
using Stripe;
using LystenApi.Models;

namespace LystenApi.Controllers.Api
{


    [RoutePrefix("api/v1")]
    public class MasterController : BaseApiController
    {
        CommonServices cs = new CommonServices();
        MasterServices MS = new MasterServices();
        ApiMasterServices ApiMaster = new ApiMasterServices();
        ApiMessageFormat ap = new ApiMessageFormat();
        ApiException ApiEx = new ApiException();
        ApiUserServices ApiUser = new ApiUserServices();
        //public void sendMsgEventssssss(bool IsAccept,int Id, string devicetocken)
        //{
        //    string ImagePath = "";
        //    string name = "";
        //    string baseURL = HttpContext.Current.Request.Url.Authority;
        //    baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
        //    using (LystenEntities db = new LystenEntities())
        //    {
        //        ImagePath = ApiUser.GetFavouriteImage(baseURL, Id);
        //        name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
        //    }

        //    int port = 2195;
        //    String hostname = (WebConfigurationManager.AppSettings["ApnsEnvironment"]);
        //    //String hostname = "gateway.push.apple.com";

        //    string certificatePath = System.Web.HttpContext.Current.Server.MapPath("~/Lysten-DevB.p12");

        //    string certificatePassword = "";

        //    X509Certificate2 clientCertificate = new X509Certificate2(certificatePath, certificatePassword, X509KeyStorageFlags.MachineKeySet);
        //    X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);


        //    TcpClient client = new TcpClient(hostname, port);

        //    SslStream sslStream = new SslStream(
        //                    client.GetStream(),
        //                    false,
        //                    new RemoteCertificateValidationCallback(ValidateServerCertificate),
        //                    null
        //    );
        //    try
        //    {
        //        sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Tls, false);
        //    }
        //    catch (AuthenticationException ex)
        //    {
        //        client.Close();
        //        Exception Eccsssas12 = new Exception("Athentication Failed");
        //        CommonServices.ErrorLogging(Eccsssas12);
        //        System.Web.HttpContext.Current.Server.MapPath("~/Authenticationfailed.txt");
        //        return;
        //    }

        //    //// Encode a test message into a byte array.
        //    MemoryStream memoryStream = new MemoryStream();
        //    BinaryWriter writer = new BinaryWriter(memoryStream);
        //    writer.Write((byte)0);  //The command
        //    writer.Write((byte)0);  //The first byte of the deviceId length (big-endian first byte)
        //    writer.Write((byte)32); //The deviceId length (big-endian second byte)
        //    byte[] b0 = HexString2Bytes(devicetocken);
        //    WriteMultiLineByteArray(b0);
        //    writer.Write(b0);
        //    String payload;
        //    string strmsgbody = "";
        //    string Checkbool = "";
        //    int totunreadmsg = 20;
        //    if (IsAccept)
        //    {
        //        Checkbool = "Accrpted";
        //    }
        //    else
        //    {
        //        Checkbool = "Rejected";
        //    }
        //    strmsgbody = "Call request has been" + Checkbool;
        //    payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"call\":{\"CallType\":\"" + IsAccept.ToString() == "1" ? "1" : "2" + "\",\"name\":\"" + name + "\"},\"acme1\":\"bar\",\"acme2\":42}";

        //    writer.Write((byte)0); //First byte of payload length; (big-endian first byte)
        //    writer.Write((byte)payload.Length);     //payload length (big-endian second byte)
        //    byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
        //    writer.Write(b1);
        //    writer.Flush();

        //    byte[] array = memoryStream.ToArray();
        //    try
        //    {
        //        sslStream.Write(array);
        //        sslStream.Flush();
        //    }
        //    catch
        //    {
        //    }
        //    client.Close();
        //}


        [AuthorizationRequired]
        [HttpGet]
        [Route("SendDisclaimer")]
        // POST api/<controller>
        public async Task<IHttpActionResult> SendDisclaimer(int UserId, int IsAdded)
        {
            ResultClassToken result = new ResultClassToken();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiMaster.SendDisclaimer(UserId, IsAdded);

                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.CategoryNoData;
                }
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }


        [AuthorizationRequired]
        [HttpGet]
        [Route("getdisclaimer")]
        // POST api/<controller>
        public async Task<IHttpActionResult> getdisclaimer()
        {
            ResultClass result = new ResultClass();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiMaster.getdisclaimer();

                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.CategoryNoData;
                    result.Data = new { };
                }
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }



        [HttpGet]
        [Route("TestingPayment")]
        // POST api/<controller>
        public async Task<IHttpActionResult> TestingPayment()
        {
            ResultClassToken result = new ResultClassToken();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {

                var gateway = new BraintreeGateway
                {
                    Environment = Braintree.Environment.SANDBOX,
                    MerchantId = (WebConfigurationManager.AppSettings["BraintreeMerchantId"]),
                    PublicKey = (WebConfigurationManager.AppSettings["BraintreePublicKey"]),
                    PrivateKey = (WebConfigurationManager.AppSettings["BraintreePrivateKey"])
                };


                var request = new CustomerRequest
                {
                    FirstName = "Mark",
                    LastName = "Jones",
                    Company = "Jones Co.",
                    Email = "mark.jones@example.com",
                    Fax = "419-555-1234",
                    Phone = "614-555-1234",
                    Website = "http://example.com"
                };
                Result<Customer> resultss = gateway.Customer.Create(request);

                bool success = resultss.IsSuccess();
                // true

                string customerId = resultss.Target.Id;



                var creditCardRequest = new CreditCardRequest
                {
                    CustomerId = customerId,
                    Number = "4111111111111111",
                    ExpirationDate = "06/22",
                    CVV = "100"
                };

                CreditCard creditCard = gateway.CreditCard.Create(creditCardRequest).Target;


                Customer customer = gateway.Customer.Find(customerId);
                string cardToken = customer.PaymentMethods[0].Token;
                Result<PaymentMethodNonce> resultssss = gateway.PaymentMethodNonce.Create(cardToken);
                String nonce = resultssss.Target.Nonce;

                var requestss = new TransactionRequest
                {
                    Amount = 111M,
                    PaymentMethodNonce = nonce,
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                Result<Transaction> result123 = gateway.Transaction.Sale(requestss);



                //if (mm != null)
                //{
                //    result.Code = (int)HttpStatusCode.OK;
                //    result.Msg = ap.Success;
                //}
                //else
                //{
                //    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                //    result.Msg = ap.CategoryNoData;
                //}
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }



        public void sendMsgEventsssss(int IsAccept, int Id, string devicetocken, string fullname)
        {
            string ImagePath = "";
            string name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                //Exception Eccsssas12 = new Exception(Id.ToString());
                //CommonServices.ErrorLogging(Eccsssas12);

                //Exception devicetockenasdasd = new Exception(devicetocken);
                //CommonServices.ErrorLogging(devicetockenasdasd);

                ImagePath = ApiUser.GetFavouriteImage(baseURL, Id);
                name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
            }

            int port = 2195;
            String hostname = (WebConfigurationManager.AppSettings["ApnsEnvironment"]);
            //String hostname = "gateway.push.apple.com";

            string certificatePath = System.Web.HttpContext.Current.Server.MapPath("~/Lysten-DevB.p12");

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

            string Checkbool = "";
            //if (IsAccept==1)
            //{
            //    Checkbool = "Accrpted";
            //}
            //else
            //{
            //    Checkbool = "Rejected";
            //}
            //strmsgbody = "Call request has been" + Checkbool;
            //payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"call\":{\"CallType\":\"" + IsAccept.ToString() == "1" ? "1" : "2" + "\",\"name\":\"" + name + "\"},\"acme1\":\"bar\",\"acme2\":42}";

            if (IsAccept == 1)
            {
                Checkbool = "Accepted";
            }
            else
            {
                Checkbool = "Rejected";
            }
            strmsgbody = "Your call request to " + fullname + Checkbool + ".";

            payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"call\":{\"CallType\":\"" + IsAccept + "\"},\"acme1\":\"bar\",\"acme2\":42}";
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
        [AuthorizationRequired]
        [HttpPost]
        [Route("acceptrejectcall")]
        // POST api/<controller>
        public async Task<IHttpActionResult> acceptrejectcall(CallViewModel CM)
        {
            ResultClassToken result = new ResultClassToken();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiMaster.acceptrejectcall(CM);

                using (LystenEntities db = new LystenEntities())
                {
                    Calling_Request qs = db.Calling_Request.Where(x => x.Id == CM.Id).FirstOrDefault();
                    int i = 0;
                    if (qs.IsAccept.Value)
                    {
                        i = 1;
                    }
                    //if (qs.User_Master.DeviceToken != null && qs.User_Master.DeviceToken != "")
                    //{
                    //    if (qs.User_Master.DeviceToken == "Android")
                    //    {
                    //        Helpers.NotificationHelper.SendEventPushNotification(i, qs.ToUserId.Value, qs.User_Master.DeviceToken, qs.User_Master.FullName);
                    //    }
                    //    else
                    //    {
                    //        sendMsgEventsssss(i, qs.ToUserId.Value, qs.User_Master.DeviceToken, qs.User_Master.FullName);
                    //    }
                    //}
                    //else 
                    if (qs.User_Master1.DeviceToken != null && qs.User_Master1.DeviceToken != "")
                    {
                        if (qs.User_Master1.DeviceToken == "Android")
                        {
                            Helpers.NotificationHelper.SendEventPushNotification(i, qs.ToUserId.Value, qs.User_Master1.DeviceToken, qs.User_Master1.FullName);
                        }
                        else
                        {
                            sendMsgEventsssss(i, qs.ToUserId.Value, qs.User_Master1.DeviceToken, qs.User_Master1.FullName);
                        }
                    }
                }

                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.CategoryNoData;
                }
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }


        [AuthorizationRequired]
        [HttpGet]
        [Route("getanswerbyquestionid")]
        // POST api/<controller>
        public async Task<IHttpActionResult> getanswerbyquestionid(int QuestionId, int Count, int UserId)
        {
            ResultClass result = new ResultClass();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiUser.getanswerbyquestionid(QuestionId, Count, UserId);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.AnswerNoData;
                }
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }


        //[AuthorizationRequired]
        //[HttpGet]
        //[Route("get")]
        //// POST api/<controller>
        //public async Task<IHttpActionResult> getUsercardlist(int UserId)
        //{
        //    ResultClass result = new ResultClass();
        //    try
        //    {
        //        var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");


        //        string mm = ApiMaster.getUserBraintreeId(UserId);
        //        if (mm != null)
        //        {
        //            result.Data = mm;
        //            result.Code = (int)HttpStatusCode.OK;
        //            result.Msg = ap.Success;
        //        }
        //        else
        //        {
        //            result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
        //            result.Msg = ap.RequestCallNoData;
        //            result.Data = new { };
        //        }
        //        if (updatetoken)
        //        {
        //            token = result.AccessToken = accessToken;
        //        }
        //        else
        //        {
        //            result.AccessToken = "";
        //        }
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ApiEx.FireException(result, ex));
        //    }
        //}


        [AuthorizationRequired]
        [HttpGet]
        [Route("getUsercardlist")]
        // POST api/<controller>
        public async Task<IHttpActionResult> getUsercardlist(int UserId)
        {
            ResultClass result = new ResultClass();
            try
            {
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
                StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["StripeAPIKey"]);

                //var gateway = new BraintreeGateway
                //{
                //    Environment = Braintree.Environment.SANDBOX,
                //    MerchantId = (WebConfigurationManager.AppSettings["BraintreeMerchantId"]),
                //    PublicKey = (WebConfigurationManager.AppSettings["BraintreePublicKey"]),
                //    PrivateKey = (WebConfigurationManager.AppSettings["BraintreePrivateKey"])
                //};

                List<string> _StripCardList = ApiMaster.getUserStripList(UserId);
                List<StripCardViewModel> cardList = new List<StripCardViewModel>();
                if (_StripCardList != null)
                {
                    StripeCustomerService _customerService = new StripeCustomerService();
                    foreach (var _card in _StripCardList)
                    {
                        var customer = _customerService.Get(_card);
                        //var customer = gateway.Customer.Find(mm);
                        foreach (var _source in customer.Sources)
                        {

                            StripCardViewModel strCard = new StripCardViewModel();
                            strCard.CardId = _source.Card.Id;
                            strCard.CustomerId = _source.Card.CustomerId;
                            strCard.Last4 = _source.Card.Last4;
                            cardList.Add(strCard);


                        }
                    }


                    result.Data = cardList;
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                }
                else
                {

                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.RequestCallNoData;
                    result.Data = new { };
                }


                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        [AuthorizationRequired]
        [HttpGet]
        [Route("deleteUserCard")]
        // POST api/<controller>
        public async Task<IHttpActionResult> deleteUserCard(string CustomerToken, string CardToken)
        {
            ResultClassToken result = new ResultClassToken();
            try
            {

                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                //var gateway = new BraintreeGateway
                //{
                //    Environment = Braintree.Environment.SANDBOX,
                //    MerchantId = (WebConfigurationManager.AppSettings["BraintreeMerchantId"]),
                //    PublicKey = (WebConfigurationManager.AppSettings["BraintreePublicKey"]),
                //    PrivateKey = (WebConfigurationManager.AppSettings["BraintreePrivateKey"])
                //};
                var cardService = new StripeCardService();
                StripeDeleted card = cardService.Delete(CustomerToken, CardToken);
                ApiMaster.DeleteStripeCutomer(CustomerToken);
                //gateway.CreditCard.Delete(CardToken);

                result.Code = (int)HttpStatusCode.OK;
                result.Msg = ap.Success;

                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        public dynamic CutPayment(PaymentModel PM)
        {
            LystenEntities db = new LystenEntities();

            ResultClass result = new ResultClass();

            StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["StripeAPIKey"]);
            try
            {

                var newdata = ApiMaster.GetRequestDataByDate(PM.CallingRequestId);


                var amount = 0M;
                var remainTime = 0M;
                var obja = ApiMaster.GetcallingpricebyId(newdata.CallingPriceId.Value);
                if (obja != null)
                {
                    var GetSettingsValue = ApiMaster.getsettingsbysystemname("System.CallingPricePerSec");
                    remainTime = (Convert.ToDecimal(PM.TotalCallingTime) - (Convert.ToDecimal(obja.Time)));
                    if (remainTime > 0)
                    {
                        var amountPersec = (Convert.ToDecimal(obja.Price) / Convert.ToDecimal(obja.Time));
                        var extraAmount = remainTime * amountPersec;

                        decimal value;
                        if (Decimal.TryParse(Convert.ToString(extraAmount), out value))
                            // It's a decimal 
                            amount = Math.Round(value + 1) * 100;
                        else
                            amount = Math.Round(value) * 100;
                    }
                }


                List<string> mm = ApiMaster.getUserStripList(PM.FromUserId);


                var customerService = new StripeCustomerService();
                var customer = customerService.Get(mm.FirstOrDefault());


                var myCharge = new StripeChargeCreateOptions
                {
                    Amount = Convert.ToInt32(amount),
                    Currency = "USD",
                    Description = "Payment",
                    CustomerId = customer.Id,
                    Capture = true,
                };

                var chargeService = new StripeChargeService();
                var stripeCharge = chargeService.Create(myCharge);


                var transaction = stripeCharge;
                result.Code = (int)HttpStatusCode.OK;
                result.Msg = ap.Success;
                result.Data = transaction.Id;
                var obj = ApiMaster.UpdateCallingRequest(amount  , PM, transaction.Id);


            }
            catch (Exception ex)
            {
                result.Code = (int)HttpStatusCode.BadGateway;
                result.Msg = "Something went wrong.";
                result.Data = "";
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("ReUserPayment")]
        public async Task<IHttpActionResult> AgainUserPayment(PaymentModel PM)
        {
            ResultClass result = new ResultClass();

            try
            {
                return (CutPayment(PM));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void sendMsgEvent(int EventID, int Id, string devicetocken)
        {
            devicetocken = "14510198D28335F57BF9D80088C4445BDDD366CED008EAB336C25E54906A038D";
            string ImagePath = "";
            string name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                ImagePath = ApiUser.GetFavouriteImage(baseURL, Id);
                name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
            }

            int port = 2195;
            String hostname = (WebConfigurationManager.AppSettings["ApnsEnvironment"]);
            //String hostname = "gateway.push.apple.com";

            string certificatePath = System.Web.HttpContext.Current.Server.MapPath("~/Lysten-DevB.p12");

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
            strmsgbody = "asdasdasd!";
            payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"event\":{\"EventID\":\"" + EventID + "\"}";
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

        public void sendMsgcallAccept(int Id, string devicetocken, int IsAccept)
        {
            string ImagePath = "";
            string name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                ImagePath = ApiUser.GetFavouriteImage(baseURL, Id);
                name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
            }

            int port = 2195;
            String hostname = (WebConfigurationManager.AppSettings["ApnsEnvironment"]);
            //String hostname = "gateway.push.apple.com";

            string certificatePath = System.Web.HttpContext.Current.Server.MapPath("~/Lysten-DevB.p12");

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
            strmsgbody = "Hey There! You have a new calling request.";
            payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"call\":{\"CallType\":\"" + IsAccept.ToString() == "1" ? "1" : "2" + "\",\"name\":\"" + name + "\"},\"acme1\":\"bar\",\"acme2\":42}";
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

        public void sendMsg(int Id, string devicetocken, string requestedusername)
        {
            string ImagePath = "";
            string name = "";
            string baseURL = HttpContext.Current.Request.Url.Authority;
            baseURL += (WebConfigurationManager.AppSettings["userimagepath"]).Replace("~", "");
            using (LystenEntities db = new LystenEntities())
            {
                ImagePath = ApiUser.GetFavouriteImage(baseURL, Id);
                name = db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault() == null ? "" : db.User_Master.Where(x => x.Id == Id).Select(x => x.FullName).FirstOrDefault();
            }

            int port = 2195;
            String hostname = (WebConfigurationManager.AppSettings["ApnsEnvironment"]);
            //String hostname = "gateway.push.apple.com";

            string certificatePath = System.Web.HttpContext.Current.Server.MapPath("~/Lysten-DevB.p12");

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
            strmsgbody = requestedusername + " requested for a call.";
            payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"call\":{\"CallType\":\"3\",\"name\":\"" + name + "\"},\"acme1\":\"bar\",\"acme2\":42}";
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



        //[HttpPost]
        //[Route("UserPayment")]
        //public async Task<IHttpActionResult> Payment(PaymentModel PM)
        //{
        //    ResultClass result = new ResultClass();
        //    LystenEntities db = new LystenEntities();
        //    var amount = 0M;

        //    var obja = ApiMaster.GetcallingpricebyId(Convert.ToInt32(PM.CallingPriceId));
        //    if (obja != null)
        //    {
        //        var GetSettingsValue = ApiMaster.getsettingsbysystemname("System.CallingPricePerSec");
        //        amount = (Convert.ToDecimal(obja) * 60) * Convert.ToDecimal(GetSettingsValue);
        //    }

        //    var gateway = new BraintreeGateway
        //    {
        //        Environment = Braintree.Environment.SANDBOX,
        //        MerchantId = (WebConfigurationManager.AppSettings["BraintreeMerchantId"]),
        //        PublicKey = (WebConfigurationManager.AppSettings["BraintreePublicKey"]),
        //        PrivateKey = (WebConfigurationManager.AppSettings["BraintreePrivateKey"])
        //    };

        //    try
        //    {
        //        string mm = ApiMaster.getUserBraintreeId(PM.FromUserId);

        //        if (mm == null)
        //        {

        //            User_Master UM = ApiUser.GetUserById(PM.FromUserId);
        //            var requests = new CustomerRequest
        //            {
        //                FirstName = UM.FullName,
        //                Company = "",
        //                Email = UM.Email,
        //                Phone = UM.Phone
        //            };
        //            Result<Customer> results = gateway.Customer.Create(requests);
        //            bool success = results.IsSuccess();
        //            // true
        //            string customerId = results.Target.Id;

        //            PaymentMethodRequest requestssss = new PaymentMethodRequest
        //            {
        //                CustomerId = customerId,
        //                PaymentMethodNonce = PM.payment_method_nonce,
        //                Options = new PaymentMethodOptionsRequest
        //                {
        //                    VerifyCard = true,
        //                    MakeDefault = true,
        //                    FailOnDuplicatePaymentMethod = true
        //                }
        //            };

        //            Result<PaymentMethod> resultsssss = gateway.PaymentMethod.Create(requestssss);


        //            BraintreeCustomerMapping BCM = new BraintreeCustomerMapping()
        //            {
        //                UserId = PM.FromUserId,
        //                BraintreeCustomerId = customerId
        //            };
        //            db.BraintreeCustomerMappings.Add(BCM);
        //            db.SaveChanges();

        //            //var request = new TransactionRequest
        //            //{
        //            //    Amount = amount,
        //            //    PaymentMethodNonce = PM.payment_method_nonce,
        //            //    Options = new TransactionOptionsRequest
        //            //    {
        //            //        SubmitForSettlement = true
        //            //    }
        //            //};

        //            //Result<Transaction> resultss = gateway.Transaction.Sale(request);


        //            if (resultsssss.IsSuccess())
        //            {

        //                Calling_Request CR = new Calling_Request();

        //                Transaction transaction = resultsssss.Transaction;
        //                result.Code = (int)HttpStatusCode.OK;
        //                result.Msg = ap.Success;
        //                result.Data = transaction.AuthorizedTransactionId;

        //                if (PM.CallingDateTime2 == "" || PM.CallingDateTime2 == null)
        //                {
        //                    CR = new Calling_Request()
        //                    {
        //                        ToUserId = PM.ToUserId,
        //                        FromUserId = PM.FromUserId,
        //                        CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
        //                        IsAccept = false,
        //                        IsReject = false,
        //                        PaymentStatus = "Success",
        //                        TimeZoneId = PM.TimeZoneId,
        //                        TransactionId = transaction.Id,
        //                        TotalCallingTime = PM.TotalCallingTime,
        //                        CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
        //                        TotalAmount = amount,
        //                    };

        //                    db.Calling_Request.Add(CR);
        //                    db.SaveChanges();

        //                }
        //                else
        //                {
        //                    CR = new Calling_Request()
        //                    {
        //                        ToUserId = PM.ToUserId,
        //                        FromUserId = PM.FromUserId,
        //                        CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
        //                        CallingDateTime2 = PM.CallingDateTime2 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime2),
        //                        CallingDateTime3 = PM.CallingDateTime3 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime3),
        //                        IsAccept = false,
        //                        IsReject = false,
        //                        PaymentStatus = "Success",
        //                        TimeZoneId = PM.TimeZoneId,
        //                        TransactionId = transaction.Id,
        //                        TotalCallingTime = PM.TotalCallingTime,
        //                        CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
        //                        TotalAmount = amount,
        //                    };

        //                    db.Calling_Request.Add(CR);
        //                    db.SaveChanges();
        //                }

        //                var devicetoken = db.User_Master.Where(x => x.Id == CR.ToUserId).Select(x => x.DeviceToken).FirstOrDefault();
        //                if (devicetoken != null)
        //                {
        //                    sendMsg(CR.ToUserId.Value, devicetoken);
        //                }
        //            }
        //            else if (resultsssss.Transaction != null)
        //            {
        //                result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
        //                result.Msg = ap.GlobalError;
        //                result.Data = new { };
        //            }
        //            else
        //            {

        //                foreach (ValidationError error in resultsssss.Errors.DeepAll())
        //                {
        //                    Exception ex = new Exception("Error: " + (int)error.Code + " - " + error.Message + "\n");
        //                    CommonServices.ErrorLogging(ex);
        //                }


        //                result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
        //                result.Msg = ap.GlobalError;
        //                result.Data = new { };
        //            }
        //        }
        //        else
        //        {
        //            if (PM.CardToken == null || PM.CardToken == "")
        //            {
        //                var customer = gateway.Customer.Find(mm);

        //                //Result<PaymentMethodNonce> resultsss = gateway.PaymentMethodNonce.Create(PM.CardToken);
        //                ///Vatsal 
        //                PaymentMethodRequest requestssss = new PaymentMethodRequest
        //                {
        //                    CustomerId = mm,
        //                    PaymentMethodNonce = PM.payment_method_nonce,
        //                    Options = new PaymentMethodOptionsRequest
        //                    {
        //                        VerifyCard = true,
        //                        MakeDefault = true,
        //                        FailOnDuplicatePaymentMethod = true
        //                    }
        //                };

        //                Result<PaymentMethod> resultsssss = gateway.PaymentMethod.Create(requestssss);


        //                //BraintreeCustomerMapping BCM = new BraintreeCustomerMapping()
        //                //{
        //                //    UserId = PM.FromUserId,
        //                //    BraintreeCustomerId = mm
        //                //};
        //                //db.BraintreeCustomerMappings.Add(BCM);
        //                //db.SaveChanges();
        //                ///Vatsal 



        //                //String nonce = resultsssss.n;

        //                //var request = new TransactionRequest
        //                //{
        //                //    Amount = amount,
        //                //    PaymentMethodNonce = PM.payment_method_nonce,// nonce,
        //                //    Options = new TransactionOptionsRequest
        //                //    {
        //                //        SubmitForSettlement = true
        //                //    }
        //                //};
        //                //Result<Transaction> resultss = gateway.Transaction.Sale(request);
        //                if (resultsssss.IsSuccess())
        //                {
        //                    Calling_Request CR = new Calling_Request();
        //                    Transaction transaction = resultsssss.Transaction;
        //                    result.Code = (int)HttpStatusCode.OK;
        //                    result.Msg = ap.Success;
        //                    result.Data = transaction.Id;
        //                    if (PM.CallingDateTime2 == "" || PM.CallingDateTime2 == null)
        //                    {
        //                        CR = new Calling_Request()
        //                        {
        //                            ToUserId = PM.ToUserId,
        //                            FromUserId = PM.FromUserId,
        //                            CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
        //                            IsAccept = false,
        //                            IsReject = false,
        //                            PaymentStatus = "Success",
        //                            TimeZoneId = PM.TimeZoneId,
        //                            TransactionId = transaction.Id,
        //                            TotalCallingTime = PM.TotalCallingTime,
        //                            CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
        //                            TotalAmount = amount,
        //                        };

        //                        db.Calling_Request.Add(CR);
        //                        db.SaveChanges();

        //                    }
        //                    else
        //                    {
        //                        CR = new Calling_Request()
        //                        {
        //                            ToUserId = PM.ToUserId,
        //                            FromUserId = PM.FromUserId,
        //                            CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
        //                            CallingDateTime2 = PM.CallingDateTime2 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime2),
        //                            CallingDateTime3 = PM.CallingDateTime3 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime3),
        //                            IsAccept = false,
        //                            IsReject = false,
        //                            PaymentStatus = "Success",
        //                            TimeZoneId = PM.TimeZoneId,
        //                            TransactionId = transaction.Id,
        //                            TotalCallingTime = PM.TotalCallingTime,
        //                            CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
        //                            TotalAmount = amount,
        //                        };

        //                        db.Calling_Request.Add(CR);
        //                        db.SaveChanges();
        //                    }


        //                    var devicetoken = db.User_Master.Where(x => x.Id == CR.ToUserId).Select(x => x.DeviceToken).FirstOrDefault();
        //                    if (devicetoken != null)
        //                    {
        //                        sendMsg(CR.ToUserId.Value, devicetoken);
        //                    }


        //                }
        //                else if (resultsssss.Transaction != null)
        //                {
        //                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
        //                    result.Msg = ap.GlobalError;
        //                    result.Data = new { };
        //                }
        //                else
        //                {
        //                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
        //                    result.Msg = ap.GlobalError;
        //                    result.Data = new { };
        //                }
        //            }
        //            else
        //            {

        //                Calling_Request CR = new Calling_Request();
        //                Result<PaymentMethodNonce> resultsss = gateway.PaymentMethodNonce.Create(PM.CardToken);
        //                String nonce = resultsss.Target.Nonce;

        //                var request = new TransactionRequest
        //                {
        //                    Amount = amount,
        //                    PaymentMethodNonce = nonce,// nonce,
        //                    Options = new TransactionOptionsRequest
        //                    {
        //                        SubmitForSettlement = true
        //                    }
        //                };
        //                Result<Transaction> resultss = gateway.Transaction.Sale(request);
        //                if (resultss.IsSuccess())
        //                {
        //                    Transaction transaction = resultss.Target;
        //                    result.Code = (int)HttpStatusCode.OK;
        //                    result.Msg = ap.Success;
        //                    result.Data = transaction.Id;

        //                    if (PM.CallingDateTime2 == "" || PM.CallingDateTime2 == null)
        //                    {
        //                        CR = new Calling_Request()
        //                        {
        //                            ToUserId = PM.ToUserId,
        //                            FromUserId = PM.FromUserId,
        //                            CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
        //                            IsAccept = false,
        //                            IsReject = false,
        //                            PaymentStatus = "Success",
        //                            TimeZoneId = PM.TimeZoneId,
        //                            TransactionId = transaction.Id,
        //                            TotalCallingTime = PM.TotalCallingTime,
        //                            CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
        //                            TotalAmount = amount,
        //                        };

        //                        db.Calling_Request.Add(CR);
        //                        db.SaveChanges();

        //                    }
        //                    else
        //                    {
        //                        CR = new Calling_Request()
        //                        {
        //                            ToUserId = PM.ToUserId,
        //                            FromUserId = PM.FromUserId,
        //                            CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
        //                            CallingDateTime2 = PM.CallingDateTime2 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime2),
        //                            CallingDateTime3 = PM.CallingDateTime3 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime3),
        //                            IsAccept = false,
        //                            IsReject = false,
        //                            PaymentStatus = "Success",
        //                            TimeZoneId = PM.TimeZoneId,
        //                            TransactionId = transaction.Id,
        //                            TotalCallingTime = PM.TotalCallingTime,
        //                            CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
        //                            TotalAmount = amount,
        //                        };

        //                        db.Calling_Request.Add(CR);
        //                        db.SaveChanges();
        //                    }

        //                    var devicetoken = db.User_Master.Where(x => x.Id == CR.ToUserId).Select(x => x.DeviceToken).FirstOrDefault();
        //                    if (devicetoken != null)
        //                    {
        //                        sendMsg(CR.ToUserId.Value, devicetoken);
        //                    }

        //                }
        //                else if (resultss.Transaction != null)
        //                {
        //                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
        //                    result.Msg = ap.GlobalError;
        //                    result.Data = new { };
        //                }
        //                else
        //                {
        //                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
        //                    result.Msg = ap.GlobalError;
        //                    result.Data = new { };
        //                }
        //            }
        //        }
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonServices.ErrorLogging(ex);
        //        throw;
        //    }

        //}





        [HttpPost]
        [Route("UserPayment")]
        public async Task<IHttpActionResult> Payment(PaymentModel PM)
        {
            ResultClass result = new ResultClass();
            LystenEntities db = new LystenEntities();
            var amount = 0M;
            Exception ex1232hjh22 = new Exception(PM.CallingPriceId.ToString());
            CommonServices.ErrorLogging(ex1232hjh22);
            var obja = ApiMaster.GetcallingpricebyId(Convert.ToInt32(PM.CallingPriceId));
            if (obja != null)
            {
                var GetSettingsValue = ApiMaster.getsettingsbysystemname("System.CallingPricePerSec");
                amount = (Convert.ToDecimal(obja.Time) * 60) * Convert.ToInt32(obja.Price);
            }

            var gateway = new BraintreeGateway
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId = (WebConfigurationManager.AppSettings["BraintreeMerchantId"]),
                PublicKey = (WebConfigurationManager.AppSettings["BraintreePublicKey"]),
                PrivateKey = (WebConfigurationManager.AppSettings["BraintreePrivateKey"])
            };

            try
            {
                string mm = ApiMaster.getUserBraintreeId(PM.FromUserId);

                if (mm == null)
                {
                    User_Master UM = ApiUser.GetUserById(PM.FromUserId);
                    var requests = new CustomerRequest
                    {
                        FirstName = UM.FullName,
                        Company = "",
                        Email = UM.Email,
                        Phone = UM.Phone
                    };

                    Result<Customer> results = gateway.Customer.Create(requests);
                    bool success = results.IsSuccess();
                    // true


                    string customerId = results.Target.Id;

                    PaymentMethodRequest requestssss = new PaymentMethodRequest
                    {
                        CustomerId = customerId,
                        PaymentMethodNonce = PM.payment_method_nonce,
                        Options = new PaymentMethodOptionsRequest
                        {
                            VerifyCard = true,
                            MakeDefault = true
                        }
                    };

                    Result<PaymentMethod> resultsssss = gateway.PaymentMethod.Create(requestssss);

                    BraintreeCustomerMapping BCM = new BraintreeCustomerMapping()
                    {
                        UserId = PM.FromUserId,
                        BraintreeCustomerId = customerId
                    };
                    db.BraintreeCustomerMappings.Add(BCM);
                    db.SaveChanges();

                    //Customer customer = gateway.Customer.Find(customerId);
                    //string cardToken = customer.PaymentMethods[0].Token;



                    Result<PaymentMethodNonce> results123 = gateway.PaymentMethodNonce.Create(resultsssss.Target.Token);
                    String nonce = results123.Target.Nonce;


                    var request = new TransactionRequest
                    {
                        Amount = amount,
                        PaymentMethodNonce = nonce,
                        Options = new TransactionOptionsRequest
                        {
                            SubmitForSettlement = true
                        }
                    };

                    Result<Transaction> resultss = gateway.Transaction.Sale(request);


                    if (resultss.IsSuccess())
                    {
                        Calling_Request CR = new Calling_Request();

                        Transaction transaction = resultss.Target;
                        result.Code = (int)HttpStatusCode.OK;
                        result.Msg = ap.Success;
                        result.Data = transaction.AuthorizedTransactionId;

                        if (PM.CallingDateTime2 == "" || PM.CallingDateTime2 == null)
                        {
                            DateTime ut = DateTime.SpecifyKind(Convert.ToDateTime(PM.CallingDateTime1), DateTimeKind.Utc);

                            CR = new Calling_Request()
                            {
                                ToUserId = PM.ToUserId,
                                FromUserId = PM.FromUserId,
                                CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
                                IsAccept = false,
                                IsReject = false,
                                PaymentStatus = "Success",
                                TimeZoneId = PM.TimeZoneId,
                                TransactionId = transaction.Id,
                                TotalCallingTime = PM.TotalCallingTime,
                                CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
                                TotalAmount = amount,
                            };

                            db.Calling_Request.Add(CR);
                            db.SaveChanges();

                        }
                        else
                        {
                            CR = new Calling_Request()
                            {
                                ToUserId = PM.ToUserId,
                                FromUserId = PM.FromUserId,
                                CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
                                CallingDateTime2 = PM.CallingDateTime2 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime2),
                                CallingDateTime3 = PM.CallingDateTime3 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime3),
                                IsAccept = false,
                                IsReject = false,
                                PaymentStatus = "Success",
                                TimeZoneId = PM.TimeZoneId,
                                TransactionId = transaction.Id,
                                TotalCallingTime = PM.TotalCallingTime,
                                CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
                                TotalAmount = amount,
                            };

                            db.Calling_Request.Add(CR);
                            db.SaveChanges();
                        }

                        var devicetoken = db.User_Master.Where(x => x.Id == CR.ToUserId).Select(x => x.DeviceToken).FirstOrDefault();

                        var requestedusername = db.User_Master.Where(x => x.Id == CR.ToUserId).Select(x => x.FullName).FirstOrDefault();

                        if (devicetoken != null)
                        {
                            sendMsg(CR.ToUserId.Value, devicetoken, requestedusername);
                        }
                    }
                    else if (resultss.Transaction != null)
                    {
                        result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                        result.Msg = ap.GlobalError;
                        result.Data = new { };
                    }
                    else
                    {

                        foreach (ValidationError error in resultss.Errors.DeepAll())
                        {
                            Exception ex122 = new Exception("Error: " + (int)error.Code + " - " + error.Message + "\n");
                            CommonServices.ErrorLogging(ex122);
                        }


                        result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                        result.Msg = ap.GlobalError;
                        result.Data = new { };
                    }
                }
                else
                {
                    if (PM.CardToken == null || PM.CardToken == "")
                    {
                        var customer = gateway.Customer.Find(mm);
                        PaymentMethodRequest requestssss = new PaymentMethodRequest
                        {
                            CustomerId = mm,
                            PaymentMethodNonce = PM.payment_method_nonce,
                            Options = new PaymentMethodOptionsRequest
                            {
                                VerifyCard = true
                            }
                        };




                        Result<PaymentMethod> resultsssss = gateway.PaymentMethod.Create(requestssss);

                        Result<PaymentMethodNonce> results123 = gateway.PaymentMethodNonce.Create(resultsssss.Target.Token);
                        String nonce = results123.Target.Nonce;

                        var request = new TransactionRequest
                        {
                            Amount = amount,
                            PaymentMethodNonce = nonce,// nonce,
                            Options = new TransactionOptionsRequest
                            {
                                SubmitForSettlement = true
                            }
                        };
                        Result<Transaction> resultss = gateway.Transaction.Sale(request);
                        if (resultss.IsSuccess())
                        {
                            Calling_Request CR = new Calling_Request();
                            Transaction transaction = resultss.Target;
                            result.Code = (int)HttpStatusCode.OK;
                            result.Msg = ap.Success;
                            result.Data = transaction.Id;
                            if (PM.CallingDateTime2 == "" || PM.CallingDateTime2 == null)
                            {
                                CR = new Calling_Request()
                                {
                                    ToUserId = PM.ToUserId,
                                    FromUserId = PM.FromUserId,
                                    CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
                                    IsAccept = false,
                                    IsReject = false,
                                    PaymentStatus = "Success",
                                    TimeZoneId = PM.TimeZoneId,
                                    TransactionId = transaction.Id,
                                    TotalCallingTime = PM.TotalCallingTime,
                                    CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
                                    TotalAmount = amount,
                                };

                                db.Calling_Request.Add(CR);
                                db.SaveChanges();

                            }
                            else
                            {
                                CR = new Calling_Request()
                                {
                                    ToUserId = PM.ToUserId,
                                    FromUserId = PM.FromUserId,
                                    CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
                                    CallingDateTime2 = PM.CallingDateTime2 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime2),
                                    CallingDateTime3 = PM.CallingDateTime3 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime3),
                                    IsAccept = false,
                                    IsReject = false,
                                    PaymentStatus = "Success",
                                    TimeZoneId = PM.TimeZoneId,
                                    TransactionId = transaction.Id,
                                    TotalCallingTime = PM.TotalCallingTime,
                                    CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
                                    TotalAmount = amount,
                                };

                                db.Calling_Request.Add(CR);
                                db.SaveChanges();
                            }

                            var requestedusername = db.User_Master.Where(x => x.Id == CR.ToUserId).Select(x => x.FullName).FirstOrDefault();
                            var devicetoken = db.User_Master.Where(x => x.Id == CR.ToUserId).Select(x => x.DeviceToken).FirstOrDefault();

                            if (devicetoken != null)
                            {
                                sendMsg(CR.ToUserId.Value, devicetoken, requestedusername);
                            }
                        }
                        else if (resultss.Transaction != null)
                        {
                            result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                            result.Msg = ap.GlobalError;
                            result.Data = new { };
                        }
                        else
                        {
                            result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                            result.Msg = ap.GlobalError;
                            result.Data = new { };
                        }
                    }
                    else
                    {

                        Calling_Request CR = new Calling_Request();
                        Result<PaymentMethodNonce> resultsss = gateway.PaymentMethodNonce.Create(PM.CardToken);
                        String nonce = resultsss.Target.Nonce;

                        var request = new TransactionRequest
                        {
                            Amount = amount,
                            PaymentMethodNonce = nonce,// nonce,
                            Options = new TransactionOptionsRequest
                            {
                                SubmitForSettlement = true
                            }
                        };
                        Result<Transaction> resultss = gateway.Transaction.Sale(request);


                        if (resultss.IsSuccess())
                        {

                            Transaction transaction = resultss.Target;
                            result.Code = (int)HttpStatusCode.OK;
                            result.Msg = ap.Success;
                            result.Data = transaction.Id;

                            if (PM.CallingDateTime2 == "" || PM.CallingDateTime2 == null)
                            {


                                CR = new Calling_Request()
                                {
                                    ToUserId = PM.ToUserId,
                                    FromUserId = PM.FromUserId,
                                    CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
                                    IsAccept = false,
                                    IsReject = false,
                                    PaymentStatus = "Success",
                                    TimeZoneId = PM.TimeZoneId,
                                    TransactionId = transaction.Id,
                                    TotalCallingTime = PM.TotalCallingTime,
                                    CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
                                    TotalAmount = amount,
                                };

                                db.Calling_Request.Add(CR);
                                db.SaveChanges();

                            }
                            else
                            {

                                CR = new Calling_Request()
                                {
                                    ToUserId = PM.ToUserId,
                                    FromUserId = PM.FromUserId,
                                    CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
                                    CallingDateTime2 = PM.CallingDateTime2 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime2),
                                    CallingDateTime3 = PM.CallingDateTime3 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime3),
                                    IsAccept = false,
                                    IsReject = false,
                                    PaymentStatus = "Success",
                                    TimeZoneId = PM.TimeZoneId,
                                    TransactionId = transaction.Id,
                                    TotalCallingTime = PM.TotalCallingTime,
                                    CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
                                    TotalAmount = amount,
                                };

                                db.Calling_Request.Add(CR);
                                db.SaveChanges();
                            }


                            var requestedusername = db.User_Master.Where(x => x.Id == CR.ToUserId).Select(x => x.FullName).FirstOrDefault();
                            var devicetoken = db.User_Master.Where(x => x.Id == CR.ToUserId).Select(x => x.DeviceToken).FirstOrDefault();


                            if (devicetoken != null)
                            {
                                sendMsg(CR.ToUserId.Value, devicetoken, requestedusername);
                            }
                        }
                        else if (resultss.Transaction != null)
                        {

                            result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                            result.Msg = ap.GlobalError;
                            result.Data = new { };
                        }
                        else
                        {

                            result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                            result.Msg = ap.GlobalError;
                            result.Data = new { };
                        }
                    }
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                CommonServices.ErrorLogging(ex);
                throw;
            }

        }



        [HttpPost]
        [Route("StripeUserPayment")]
        public async Task<IHttpActionResult> StripPayment(PaymentModel PM)
        {
            ResultClass result = new ResultClass();
            LystenEntities db = new LystenEntities();
            var amount = 0M;
            //try
            //{
            //    Exception ex1232hjh22 = new Exception(PM.CallingPriceId.ToString());
            //    CommonServices.ErrorLogging(ex1232hjh22);

            //}
            //catch { }
            var obja = ApiMaster.GetcallingpricebyId(Convert.ToInt32(PM.CallingPriceId));
            if (obja != null)
            {
                var GetSettingsValue = ApiMaster.getsettingsbysystemname("System.CallingPricePerSec");
                amount = Convert.ToInt32(Convert.ToInt32(obja.Price) * 100);
            }



            try
            {
                string mm = ApiMaster.getUserBraintreeId(PM.FromUserId);

                if (!string.IsNullOrEmpty(PM.CardToken))
                {
                    User_Master UM = ApiUser.GetUserById(PM.FromUserId);
                    StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["StripeAPIKey"]);

                    var customerOptions = new StripeCustomerCreateOptions
                    {
                        SourceToken = PM.CardToken,//model.payment_method_nonce != null ? model.payment_method_nonce : "tok_1C7eUEKZZeWSkNcHIUD7C59e",
                        Email = UM.Email// userInformation.Email,
                    };

                    var customerService = new StripeCustomerService();
                    StripeCustomer customer = customerService.Create(customerOptions);

                    // true
                    string customerId = customer.Id;

                    BraintreeCustomerMapping BCM = new BraintreeCustomerMapping()
                    {
                        UserId = PM.FromUserId,
                        BraintreeCustomerId = customerId
                    };
                    db.BraintreeCustomerMappings.Add(BCM);
                    db.SaveChanges();

                    var myCharge = new StripeChargeCreateOptions
                    {
                        Amount = Convert.ToInt32(amount),
                        Currency = "USD",
                        Description = "Calling charge",
                        CustomerId = customerId,
                        Capture = true,
                    };

                    var chargeService = new StripeChargeService();
                    var stripeCharge = chargeService.Create(myCharge);

                    //Result<Transaction> resultss = gateway.Transaction.Sale(request);

                    Calling_Request CR = new Calling_Request();
                    try
                    {
                        var transaction = stripeCharge;
                        result.Code = (int)HttpStatusCode.OK;
                        result.Msg = ap.Success;
                        result.Data = new { TransactionId = transaction.Id, Status = transaction.Status, Amount = transaction.Amount };

                        if (PM.CallingDateTime2 == "" || PM.CallingDateTime2 == null)
                        {
                            DateTime ut = DateTime.SpecifyKind(Convert.ToDateTime(PM.CallingDateTime1), DateTimeKind.Utc);

                            CR = new Calling_Request()
                            {
                                ToUserId = PM.ToUserId,
                                FromUserId = PM.FromUserId,
                                CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
                                IsAccept = false,
                                IsReject = false,
                                PaymentStatus = "Success",
                                TimeZoneId = PM.TimeZoneId,
                                TransactionId = transaction.Id,
                                TotalCallingTime = PM.TotalCallingTime,
                                CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
                                TotalAmount = amount,
                            };

                            db.Calling_Request.Add(CR);
                            db.SaveChanges();

                        }
                        else
                        {
                            CR = new Calling_Request()
                            {
                                ToUserId = PM.ToUserId,
                                FromUserId = PM.FromUserId,
                                CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
                                CallingDateTime2 = PM.CallingDateTime2 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime2),
                                CallingDateTime3 = PM.CallingDateTime3 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime3),
                                IsAccept = false,
                                IsReject = false,
                                PaymentStatus = "Success",
                                TimeZoneId = PM.TimeZoneId,
                                TransactionId = transaction.Id,
                                TotalCallingTime = PM.TotalCallingTime,
                                CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
                                TotalAmount = amount,
                            };

                            db.Calling_Request.Add(CR);
                            db.SaveChanges();
                        }

                        if (CR != null)
                        {
                            var _user = db.User_Master.Where(x => x.Id == CR.ToUserId).FirstOrDefault();
                            var devicetoken = _user.DeviceToken;
                            var deviceType = _user.DeviceType;

                            var requestedusername = _user.FullName;

                            if (devicetoken != null)
                            {
                                try
                                {
                                    if (deviceType == "Android")
                                    {
                                        Helpers.NotificationHelper.SendCallingNotification(CR.ToUserId.Value, devicetoken, requestedusername);
                                    }
                                    else
                                    {
                                        sendMsg(CR.ToUserId.Value, devicetoken, requestedusername);
                                    }
                                }
                                catch
                                {

                                }


                            }
                        }
                    }
                    catch
                    {

                    }
                }
                else
                {
                    //if (PM.CardToken == null || PM.CardToken == "")
                    //{
                    StripeCustomerService _customerService = new StripeCustomerService();
                    var customer = _customerService.Get(PM.payment_method_nonce);


                    var myCharge = new StripeChargeCreateOptions
                    {
                        Amount = Convert.ToInt32(amount),
                        Currency = "USD",
                        Description = "Calling Charge",
                        //SourceCard =new SourceCard(){ PM.payment_method_nonce,
                        CustomerId = customer.Id,
                        Capture = true,
                    };

                    var chargeService = new StripeChargeService();
                    var stripeCharge = chargeService.Create(myCharge);
                    var CR = new Calling_Request();
                    try
                    {

                        var transaction = stripeCharge;
                        result.Code = (int)HttpStatusCode.OK;
                        result.Msg = ap.Success;
                        result.Data = new { TransactionId = transaction.Id, Status = transaction.Status, Amount = transaction.Amount };

                        if (PM.CallingDateTime2 == "" || PM.CallingDateTime2 == null)
                        {
                            CR.ToUserId = PM.ToUserId;
                            CR.FromUserId = PM.FromUserId;
                            CR.CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1);
                            CR.IsAccept = false;
                            CR.IsReject = false;
                            CR.PaymentStatus = "Success";
                            CR.TimeZoneId = PM.TimeZoneId;
                            CR.TransactionId = transaction.Id;
                            CR.TotalCallingTime = PM.TotalCallingTime;
                            CR.CallingPriceId = Convert.ToInt32(PM.CallingPriceId);
                            CR.TotalAmount = amount;


                            db.Calling_Request.Add(CR);
                            db.SaveChanges();

                        }
                        else if (PM.CallingDateTime3 == "" || PM.CallingDateTime3 == null)
                        {
                            CR = new Calling_Request()
                            {
                                ToUserId = PM.ToUserId,
                                FromUserId = PM.FromUserId,
                                CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
                                CallingDateTime2 = PM.CallingDateTime2 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime2),
                                //CallingDateTime3 = PM.CallingDateTime3 == null ? Convert.ToDateTime(null) : Convert.ToDateTime(PM.CallingDateTime3),
                                IsAccept = false,
                                IsReject = false,
                                PaymentStatus = "Success",
                                TimeZoneId = PM.TimeZoneId,
                                TransactionId = transaction.Id,
                                TotalCallingTime = PM.TotalCallingTime,
                                CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
                                TotalAmount = amount,
                            };

                            db.Calling_Request.Add(CR);
                            db.SaveChanges();
                        }
                        else
                        {
                            CR = new Calling_Request()
                            {
                                ToUserId = PM.ToUserId,
                                FromUserId = PM.FromUserId,
                                CallingDateTime1 = Convert.ToDateTime(PM.CallingDateTime1),
                                CallingDateTime2 = Convert.ToDateTime(PM.CallingDateTime2),
                                CallingDateTime3 = Convert.ToDateTime(PM.CallingDateTime3),
                                IsAccept = false,
                                IsReject = false,
                                PaymentStatus = "Success",
                                TimeZoneId = PM.TimeZoneId,
                                TransactionId = transaction.Id,
                                TotalCallingTime = PM.TotalCallingTime,
                                CallingPriceId = Convert.ToInt32(PM.CallingPriceId),
                                TotalAmount = amount,
                            };

                            db.Calling_Request.Add(CR);
                            db.SaveChanges();
                        }
                        var _user = db.User_Master.Where(x => x.Id == CR.ToUserId).FirstOrDefault();
                        var requestedusername = _user.FullName;
                        var devicetoken = _user.DeviceToken;
                        var deviceType = _user.DeviceType;


                        if (devicetoken != null)
                        {
                            try
                            {
                                if (deviceType == "Android")
                                {
                                    Helpers.NotificationHelper.SendCallingNotification(CR.ToUserId.Value, devicetoken, requestedusername);
                                }
                                else
                                {
                                    sendMsg(CR.ToUserId.Value, devicetoken, requestedusername);
                                }
                            }
                            catch (Exception ex)
                            {

                            }


                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Code = (int)HttpStatusCode.NotAcceptable;
                result.Msg = ex.Message;
                result.Data = new { };
                return Ok(result);
            }

        }

        [AuthorizationRequired]
        [HttpGet]
        [Route("getrequestcall")]
        // POST api/<controller>
        public async Task<IHttpActionResult> getrequestcall()
        {
            ResultClass result = new ResultClass();
            try
            {
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                var mm = ApiMaster.getrequestcall();
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = new { mm.TimeZone, mm.Minutes };
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.RequestCallNoData;
                    result.Data = mm;
                }
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        // [AuthorizationRequired]
        [HttpGet]
        [Route("calling")]
        // POST api/<controller>
        public async Task<IHttpActionResult> calling()
        {
            ResultClass result = new ResultClass();
            try
            {
                // var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                string accountSid = ConfigurationManager.AppSettings["TwilioaccountSid"];
                string authToken = ConfigurationManager.AppSettings["TwilioauthToken"];
                string pnnumber = ConfigurationManager.AppSettings["TwilioFromNumber"];

                TwilioClient.Init(accountSid, authToken);

                var to = new PhoneNumber("+918511840258");//919429628741
                var from = new PhoneNumber(pnnumber);
                var call = CallResource.Create(to,
                                               from,
                                               url: new Uri("http://demo.twilio.com/docs/voice.xml"));

                if (call != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = call;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = "";
                    result.Data = call;
                }
                if (updatetoken)
                {
                    //token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        [HttpGet]
        [Route("accessToken")]
        // POST api/<controller>
        public async Task<IHttpActionResult> GetAccesstoken(string name)
        {
            ResultClass result = new ResultClass();
            try
            {
                const string twilioAccountSid = "AC58a8f470d6f9178a7d2c82250c42d26c";// live debi"ACd500288365cba05024ef159a53bf2446";//vatsal "";//"ACba66147727f38172a069d8611ab20232";
                const string twilioApiKey = "SK0b6501b76c46dd8b11232185179428d2";//live "SKc20073e6f68255c66668054c098de0af";//vatsal "";//"SK68e953efe34ba88af56a123121b49409";
                const string twilioApiSecret = "hlNUVOmxFVfeKVgDSMFNB8w0qVXV0Fjb";//live  "FMC58Fq2aj4FaS3H7Xa2fD5EEl4pnB4l";//vatsal "";//"Q9Mz2mUKrwebFVZZ5aNaKQjceybFCl8V";

                // These are specific to Voice

                // var grant = new VideoGrant();

                // Create a Voice grant for this token
                const string outgoingApplicationSid = "AP7a09a85e33b24cd083da6534c39255e8";//live "AP5231a1efe8ff4ec2af12523138291868"; // vatsal "";
                string identity = name;

                // Create a Voice grant for this token
                var grant = new Twilio.Jwt.AccessToken.VoiceGrant();
                grant.OutgoingApplicationSid = outgoingApplicationSid;

                var grants = new HashSet<Twilio.Jwt.AccessToken.IGrant>
        {
            { grant }
        };

                // Create an Access Token generator
                var token = new Twilio.Jwt.AccessToken.Token(
                    twilioAccountSid,
                    twilioApiKey,
                    twilioApiSecret,
                    identity,
                    grants: grants);

                result.Code = (int)HttpStatusCode.OK;
                result.Msg = ap.Success;
                result.Data = token.ToJwt();
                return Ok(token.ToJwt());
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }


        [AuthorizationRequired]
        [HttpGet]
        [Route("dashboard")]
        // POST api/<controller>
        public async Task<IHttpActionResult> dashboard(string Date)
        {
            ResultClass result = new ResultClass();
            try
            {
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                var mm = ApiMaster.GetDashBoardData(Date, token);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = "";
                    result.Data = mm;
                }
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        [HttpGet]
        [Route("country")]
        // POST api/<controller>
        public async Task<IHttpActionResult> Country()
        {
            ResultClassForNonAuth result = new ResultClassForNonAuth();
            try
            {
                var mm = ApiMaster.GetCountryList();
                if (mm.Count > 0)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.CountryNoData;
                    result.Data = mm;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        [HttpGet]
        [Route("state")]
        // POST api/<controller>
        public async Task<IHttpActionResult> State(int CountryId)
        {
            ResultClassForNonAuth result = new ResultClassForNonAuth();
            try
            {
                var mm = ApiMaster.GetStatelistByCountryName(CountryId);
                if (mm.Count > 0)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.StateNoData;
                    result.Data = mm;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        [HttpGet]
        [Route("city")]
        // POST api/<controller>
        public async Task<IHttpActionResult> City(int CountryId, int StateId)
        {
            ResultClassForNonAuth result = new ResultClassForNonAuth();
            try
            {
                var mm = ApiMaster.GetCityListByCountryAndStateName(CountryId, StateId);
                if (mm.Count > 0)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.CityNoData;
                    result.Data = mm;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        #region User

        [HttpGet]
        [AuthorizationRequired]
        [Route("master/getuserlist")]
        // POST api/<controller>
        public async Task<IHttpActionResult> getuserlist(int page)
        {
            ResultClass result = new ResultClass();
            try
            {
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                //var mm = ApiMaster.getuserlist(page, token);
                //if (mm.Count > 0)
                //{
                //    result.Code = HttpStatusCode.OK.GetEnumValue();
                //    result.Msg = ap.Success;
                //    result.Data = mm;
                //}
                //else
                //{
                //    result.Code = HttpStatusCode.NoContent.GetEnumValue();
                //    result.Msg = ap.PolicyNoData;
                //    result.Data = mm;
                //}
                //if (updatetoken)
                //{
                //    token = result.AccessToken = accessToken;
                //}
                //else
                //{
                //    result.AccessToken = "";
                //}
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        [HttpGet]
        [AuthorizationRequired]
        [Route("master/getuserbyId")]
        // POST api/<controller>
        public async Task<IHttpActionResult> getuserbyId(int UserId)
        {
            ResultClass result = new ResultClass();
            try
            {
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                //var mm = ApiMaster.getuserbyId(UserId);
                //if (mm != null)
                //{
                //    result.Code = HttpStatusCode.OK.GetEnumValue();
                //    result.Msg = ap.Success;
                //    result.Data = mm;
                //}
                //else
                //{
                //    result.Code = (int)HttpStatusCode.NoContent;
                //    result.Msg = ap.PolicyNoData;
                //    result.Data = mm;
                //}
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        [HttpGet]
        [AuthorizationRequired]
        [Route("master/deactiveuser")]
        // POST api/<controller>
        public async Task<IHttpActionResult> Deactiveuser(int UserId)
        {
            ResultClass result = new ResultClass();
            try
            {
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
                var model = new { };
                //var mm = ApiMaster.Deactiveuser(UserId);
                //if (mm != null)
                //{
                //    result.Code = (int)HttpStatusCode.OK;
                //    result.Msg = ap.Success;
                //    result.Data = mm;
                //}
                //else
                //{
                //    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                //    result.Msg = ap.GlobalError;
                //    result.Data = model;
                //}
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        public class Datum
        {
            public bool Status { get; set; }
            public string Name { get; set; }
            public int Id { get; set; }
            public List<object> Rights { get; set; }
        }

        public class RootObject
        {
            public List<Datum> Data { get; set; }
        }

        [HttpPost]
        [AuthorizationRequired]
        [Route("master/saveupdateuser")]
        // POST api/<controller>
        public async Task<IHttpActionResult> SaveUpdateUser([FromBody] UserProfileViewModelSave CM)
        {
            ResultClassToken result = new ResultClassToken();
            try
            {
                JavaScriptSerializer oJS = new JavaScriptSerializer();
                var oRootObject123 = oJS.Deserialize<RootObject>(CM.Rights);
                var oRootObject = JsonConvert.DeserializeObject<RootObject>(CM.Rights);


                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                //var mm = ApiMaster.SaveUpdateUser(CM, token, oRootObject);
                //if (mm != null)
                //{
                //    if (mm.IsActive)
                //    {
                //        result.Code = HttpStatusCode.OK.GetEnumValue();
                //        result.Msg = ap.Success;
                //    }
                //    else
                //    {
                //        result.Code = HttpStatusCode.NonAuthoritativeInformation.GetEnumValue();
                //        result.Msg = "";
                //    }
                //}
                //else
                //{
                //    result.Code = HttpStatusCode.NonAuthoritativeInformation.GetEnumValue();
                //    result.Msg = ap.UserEMailExist;
                //}
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }



        [HttpGet]
        [AuthorizationRequired]
        [Route("deleteevent")]
        // POST api/<controller>
        public async Task<IHttpActionResult> deleteevent(int EventId)
        {
            ResultClassToken result = new ResultClassToken();
            try
            {
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                var mm = ApiMaster.deleteevent(EventId);
                if (mm == 1)
                {

                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.UserEMailExist;
                }
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }


        [HttpGet]
        [AuthorizationRequired]
        [Route("geteventlistbyuser")]
        // POST api/<controller>
        public async Task<IHttpActionResult> geteventlistbyuserid(int UserId, int Count)
        {
            ResultClass result = new ResultClass();
            try
            {
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                var mm = ApiMaster.geteventlistbyuserid(UserId, Count);
                if (mm.Count > 0)
                {

                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.EventNoData;
                    result.Data = new { };
                }
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }




        [HttpGet]
        [AuthorizationRequired]
        [Route("deletequestion")]
        // POST api/<controller>
        public async Task<IHttpActionResult> deletequestion(int QuestionId)
        {
            ResultClassToken result = new ResultClassToken();
            try
            {
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                var mm = ApiMaster.deletequestion(QuestionId);
                if (mm == 1)
                {

                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.UserEMailExist;
                }
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }


        [HttpGet]
        [AuthorizationRequired]
        [Route("getquestionlistbyuser")]
        // POST api/<controller>
        public async Task<IHttpActionResult> getquestionlistbyuser(int UserId, int Count)
        {
            ResultClass result = new ResultClass();
            try
            {
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                var mm = ApiMaster.getquestionlistbyuser(UserId, Count);
                if (mm.Count > 0)
                {

                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.QuestionNodata;
                    result.Data = new { };
                }
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }





        #endregion

        #region Event

        [AuthorizationRequired]
        [HttpPost]
        [Route("SaveEvents")]
        // POST api/<controller>
        public async Task<IHttpActionResult> SaveEvents()
        {
            ResultClass result = new ResultClass();

            EventViewModel UM = new EventViewModel();
            UM.Title = HttpContext.Current.Request.Params["Title"];
            UM.Description = HttpContext.Current.Request.Params["Description"];
            UM.Location = HttpContext.Current.Request.Params["Location"];
            UM.CategoryId = Convert.ToInt16(HttpContext.Current.Request.Params["CategoryId"]);
            UM.Id = Convert.ToInt16(HttpContext.Current.Request.Params["Id"]);

            UM.UserId = Convert.ToInt16(HttpContext.Current.Request.Params["UserId"]);

            UM.CreatedDate = (HttpContext.Current.Request.Params["Date"]);

            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiMaster.SaveEvents(UM);
                int iUploadedCnt = 0;

                // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["eventimagepath"]);

                bool exists = System.IO.Directory.Exists(sPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(sPath);
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

                // CHECK THE FILE COUNT.
                if (mm != null)
                {

                    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                    {
                        System.Web.HttpPostedFile hpf = hfc[iCnt];

                        if (hpf.ContentLength > 0)
                        {
                            // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                            string ImagePath = mm.Id + "_" + hpf.FileName;

                            // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                            if (!File.Exists(sPath + Path.GetFileName(ImagePath)))
                            {
                                // SAVE THE FILES IN THE FOLDER.
                                hpf.SaveAs(sPath + Path.GetFileName(ImagePath));
                                UM.Image = ImagePath;
                                iUploadedCnt = iUploadedCnt + 1;

                            }
                            else
                            {
                                File.Delete(sPath + Path.GetFileName(ImagePath));
                                hpf.SaveAs(sPath + Path.GetFileName(ImagePath));
                                UM.Image = ImagePath;
                                iUploadedCnt = iUploadedCnt + 1;
                            }


                            string baseURL = HttpContext.Current.Request.Url.Authority;
                            baseURL += (WebConfigurationManager.AppSettings["eventimagepath"]).Replace("~", "");


                            //WebClient wc = new WebClient();
                            //string str ="http://"+baseURL + ImagePath;

                            //byte[] bytes = wc.DownloadData(str);
                            //MemoryStream ms = new MemoryStream(bytes);
                            //System.Drawing.Image img = System.Drawing.Image.FromStream(ms);


                            //var obj = ToByteArray(FixedSize(img, 400, 200, true));
                            //File.WriteAllBytes(str, obj);


                            UM.Id = mm.Id;
                            var resultdata = ApiMaster.SaveEvents(UM);

                            if (resultdata != null)
                            {
                                result.Code = (int)HttpStatusCode.OK;
                                result.Msg = ap.Success;
                                result.Data = mm;
                            }
                            else
                            {
                                result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                                result.Msg = ap.EventNoData;
                                result.Data = new { };
                            }
                            if (updatetoken)
                            {
                                token = result.AccessToken = accessToken;
                            }
                            else
                            {
                                result.AccessToken = "";
                            }
                            return Ok(result);

                        }
                    }
                }

                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.EventNoData;
                    result.Data = new { };
                }
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }



        public System.Drawing.Image FixedSize(Image image, int Width, int Height, bool needToFill)
        {
            #region calculations
            int sourceWidth = image.Width;
            int sourceHeight = image.Height;
            int sourceX = 0;
            int sourceY = 0;
            double destX = 0;
            double destY = 0;

            double nScale = 0;
            double nScaleW = 0;
            double nScaleH = 0;

            nScaleW = ((double)Width / (double)sourceWidth);
            nScaleH = ((double)Height / (double)sourceHeight);
            if (!needToFill)
            {
                nScale = Math.Min(nScaleH, nScaleW);
            }
            else
            {
                nScale = Math.Max(nScaleH, nScaleW);
                destY = (Height - sourceHeight * nScale) / 2;
                destX = (Width - sourceWidth * nScale) / 2;
            }

            if (nScale > 1)
                nScale = 1;

            int destWidth = (int)Math.Round(sourceWidth * nScale);
            int destHeight = (int)Math.Round(sourceHeight * nScale);
            #endregion

            System.Drawing.Bitmap bmPhoto = null;
            try
            {
                bmPhoto = new System.Drawing.Bitmap(destWidth + (int)Math.Round(2 * destX), destHeight + (int)Math.Round(2 * destY));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("destWidth:{0}, destX:{1}, destHeight:{2}, desxtY:{3}, Width:{4}, Height:{5}",
                    destWidth, destX, destHeight, destY, Width, Height), ex);
            }
            using (System.Drawing.Graphics grPhoto = System.Drawing.Graphics.FromImage(bmPhoto))
            {
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                grPhoto.CompositingQuality = CompositingQuality.HighQuality;
                grPhoto.SmoothingMode = SmoothingMode.HighQuality;

                Rectangle to = new System.Drawing.Rectangle((int)Math.Round(destX), (int)Math.Round(destY), destWidth, destHeight);
                Rectangle from = new System.Drawing.Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);
                //Console.WriteLine("From: " + from.ToString());
                //Console.WriteLine("To: " + to.ToString());
                grPhoto.DrawImage(image, to, from, System.Drawing.GraphicsUnit.Pixel);

                using (Stream imgStream = new MemoryStream())
                {
                    bmPhoto.Save(imgStream, ImageFormat.Bmp);
                    bmPhoto = new Bitmap(imgStream);

                    //convert it to a byte array to fire at a printer.
                    ImageConverter converter = new ImageConverter();
                    byte[] test = (byte[])converter.ConvertTo(bmPhoto, typeof(byte[]));
                }
                return bmPhoto;
            }


        }


        public byte[] ToByteArray(Image imageIn)
        {
            var ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        //Convert byte[] array to Image:
        public Image ToImage(byte[] byteArrayIn)
        {
            var ms = new MemoryStream(byteArrayIn);
            var returnImage = Image.FromStream(ms);
            return returnImage;
        }


        [AuthorizationRequired]
        [HttpGet]
        [Route("getallevents")]
        // POST api/<controller>
        public async Task<IHttpActionResult> getallevents(int Count, string Search)
        {
            ResultClass result = new ResultClass();
            try
            {
                string baseURL = HttpContext.Current.Request.Url.Authority;
                baseURL += (WebConfigurationManager.AppSettings["eventimagepath"]).Replace("~", "");

                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                var mm = ApiMaster.getallevents(Count, Search);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.EventNoData;
                    result.Data = mm; result.Data = mm;
                }
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }


        [AuthorizationRequired]
        [HttpGet]
        [Route("geteventsbyid")]
        // POST api/<controller>
        public async Task<IHttpActionResult> geteventsbyid(int EventId, int UserId)
        {
            ResultClass result = new ResultClass();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiMaster.geteventsbyid(EventId, UserId);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.EventNoData;
                    result.Data = mm; result.Data = mm;
                }
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }


        [AuthorizationRequired]
        [HttpGet]
        [Route("getalleventsbyCategoryId")]
        // POST api/<controller>
        public async Task<IHttpActionResult> getalleventsbyCategoryId(int CategoryId, int Count, string Search, string Location)
        {
            ResultClass result = new ResultClass();
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            try
            {
                var mm = ApiMaster.getalleventsbyCategoryId(CategoryId, Count, Search, Location);
                if (mm != null)
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.Msg = ap.Success;
                    result.Data = mm;
                }
                else
                {
                    result.Code = (int)HttpStatusCode.NonAuthoritativeInformation;
                    result.Msg = ap.EventNoData;
                    result.Data = mm; result.Data = mm;
                }
                if (updatetoken)
                {
                    token = result.AccessToken = accessToken;
                }
                else
                {
                    result.AccessToken = "";
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ApiEx.FireException(result, ex));
            }
        }

        public class IdModel
        {
            public IdModel()
            {
            }
            public int Id { get; set; }
        }



        #endregion
    }


    public class CallViewModel
    {
        public int Id { get; set; }
        public int AcceptDatetime { get; set; }
        public string RejectedNote { get; set; }
        public int IsAccept { get; set; }

    }


    public class PaymentModel
    {
        public string amount { get; set; }
        public string payment_method_nonce { get; set; }
        public int ToUserId { get; set; }
        public int FromUserId { get; set; }
        public string CallingPriceId { get; set; }
        public string CallingDateTime1 { get; set; }
        public string CallingDateTime2 { get; set; }
        public string CallingDateTime3 { get; set; }
        public string RejectedNote { get; set; }
        public string CardToken { get; set; }
        public string TimeZoneId { get; set; }
        public string TotalCallingTime { get; set; }
        public int CallingRequestId { get; set; }
    }
}