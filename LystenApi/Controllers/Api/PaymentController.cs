using LystenApi.ActionFilters;
using LystenApi.Db;
using LystenApi.Utility;
using LystenApi.Utility.Providers;
using Stripe;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace LystenApi.Controllers.Api
{
    public class PaymentController : BaseApiController
    {
        private ApiMessageFormat ap;


        public PaymentController()
        {
            ap = new ApiMessageFormat();

        }
        [AuthorizationRequired]
        [HttpPost]
        public async Task<IHttpActionResult> BuySign([FromBody]PaymentModel model)
        {
            var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");
            ResultClass result = new ResultClass();
            if (model == null)
            {
                result.Code = (int)HttpStatusCode.NoContent;
                result.Msg = ap.AnswerNoData;
                result.Data = "";
                return Ok(result);
            }
            var errorMessage = string.Empty;
            var validationError = string.Empty;
            var chargeId = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    //var tokenId = model.CardToken;//await GetTokenId(model);
                    chargeId = await ChargeCustomer(model);
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                }
                result.Code = (int)HttpStatusCode.OK;
                result.Msg = ap.Success;
                result.Data = "";
            }

            return Ok(result);
            //...rest of the code omitted for clarity
        }


        private static async Task<string> ChargeCustomer(PaymentModel model)
        {
            StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["StripeAPIKey"]); 
            using (LystenEntities db = new LystenEntities())
            {
                var userInformation = db.User_Master.Where(x => x.Id == model.FromUserId).FirstOrDefault();
                StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["StripeAPIKey"]);



                var myCharge1 = new StripeChargeCreateOptions
                         {

                             Amount = 50,
                             Currency = "USD",
                             Description = "Charge for property sign and postage",
                             //SourceTokenOrExistingSourceId = model.CardToken,
                             Capture = true,
                             CustomerId= "cus_CX5YshCWHyKVPB",
                         };

                var chargeService1 = new StripeChargeService();
                var stripeCharge1 = chargeService1.Create(myCharge1);

                //var customerOptions = new StripeCustomerCreateOptions
                //{
                //   SourceToken = model.CardToken,//model.payment_method_nonce != null ? model.payment_method_nonce : "tok_1C7eUEKZZeWSkNcHIUD7C59e",
                //    Email = userInformation.Email// userInformation.Email,
                //};
                ////var cardDeatil = new StripeCardService();
                ////cardDeatil.Get()
                //var customerService = new StripeCustomerService();
                //StripeCustomer customer = customerService.Create(customerOptions);
                var myCharge = new StripeChargeCreateOptions
                {
                    Amount = 50,
                    Currency = "USD",
                    Description = "Charge for property sign and postage",
                    SourceTokenOrExistingSourceId = model.CardToken,
                    Capture=true,
            };

                var chargeService = new StripeChargeService();
                var stripeCharge = chargeService.Create(myCharge);

                return stripeCharge.Id;
            }


        }
        //private static async Task<string> GetTokenId(PaymentModel model)
        //{
        //    using (LystenEntities db = new LystenEntities())
        //    {
        //        var userInformation = db.User_Master.Where(x => x.Id == model.ToUserId).FirstOrDefault();
        //        StripeConfiguration.SetApiKey("sk_test_RY6tukGzc8DgthGZm7Zx9yIG");//sk_test_RY6tukGzc8DgthGZm7Zx9yIG");


        //        var customerOptions = new StripeCustomerCreateOptions
        //        {
        //            SourceToken = "tok_mastercard",//model.payment_method_nonce != null ? model.payment_method_nonce : "tok_1C7eUEKZZeWSkNcHIUD7C59e",
        //            Email = "alexander.johnson.76@example.com"// userInformation.Email,
        //        };
        //        var customerService = new StripeCustomerService();
        //        //StripeCustomer customer = customerService.List(new StripeCustomerListOptions(){  sti customerOptions });
        //        var myToken = new StripeTokenCreateOptions();

        //        //        myToken.CustomerId = customer.Id;
        //        //myToken.BankAccount = new BankAccountOptions()
        //        //{
        //        //    AccountHolderName = customer.CustomerBankAccounts != null ? customer.CustomerBankAccounts.FirstOrDefault().AccountHolderName : "",
        //        //    AccountHolderType = customer.CustomerBankAccounts != null ? customer.CustomerBankAccounts.FirstOrDefault().AccountHolderType : "",
        //        //    AccountNumber = customer.CustomerBankAccounts != null ? customer.CustomerBankAccounts.FirstOrDefault().AccountId : "",
        //        //    Country = customer.CustomerBankAccounts != null ? customer.CustomerBankAccounts.FirstOrDefault().Country : "",
        //        //    Currency = customer.CustomerBankAccounts != null ? customer.CustomerBankAccounts.FirstOrDefault().Currency : "",
        //        //    RoutingNumber = customer.CustomerBankAccounts != null ? customer.CustomerBankAccounts.FirstOrDefault().RoutingNumber : "",
        //        //    //TokenId=customer.CustomerBankAccounts.FirstOrDefault().Stri
        //        //};
        //        myToken.CustomerId = customer.Id;
        //        myToken.Card = new StripeCreditCardOptions()
        //        {
        //            ExpirationMonth = customer.Sources != null ? customer.Sources.FirstOrDefault().Card.ExpirationMonth : 0,
        //            ExpirationYear = customer.Sources != null ? customer.Sources.FirstOrDefault().Card.ExpirationYear : 0,
        //            AddressCity = customer.Sources != null ? customer.Sources.FirstOrDefault().Card.AddressCity : "",
        //            Name = customer.Sources != null ? customer.Sources.FirstOrDefault().Card.Name : "",

        //        };


        //        var tokenService = new StripeTokenService();
        //        var stripeToken = tokenService.Create(myToken);

        //        return stripeToken.Id;

        //    }
        //}

    }
}
