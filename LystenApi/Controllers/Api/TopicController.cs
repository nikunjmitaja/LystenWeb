using LystenApi.ActionFilters;
using LystenApi.Utility;
using LystenApi.Utility.ApiServices;
using LystenApi.Utility.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Nito.AspNetBackgroundTasks;
using System.Web.Http.Results;
using LystenApi.Models;
using api.ActionFilters;
using System.Web.Http.Description;
using LystenApi.ViewModel;
using System.Globalization;
using LystenApi.Db;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using System.Configuration;


namespace LystenApi.Controllers.Api
{
    [AuthorizationRequired]
    [RoutePrefix("api/v1")]
    public class TopicController : BaseApiController
    {
        CommonServices cs = new CommonServices();
        ApiMessageFormat ap = new ApiMessageFormat();
        ApiException ApiEx = new ApiException();
        ApiTopicServices ApiTopic = new ApiTopicServices();

        [Route("topic")]
        // GET api/<controller>/5
        public async Task<IHttpActionResult> GetTopic(string Slug)
        {
            ResultClass result = new ResultClass();
            try
            {
                var token = HttpContext.Current.Request.Headers["Authorization"].Replace("bearer ", "");

                var mm = ApiTopic.GetTopicBySlug(Slug);
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

    }
}
