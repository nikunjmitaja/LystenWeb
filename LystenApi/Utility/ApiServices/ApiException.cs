using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using LystenApi.Db;
using LystenApi.ViewModel;
using System.Data;
using static LystenApi.Controllers.Api.MessageController;
using LystenApi.Controllers.Api;
using System.Net;

namespace LystenApi.Utility.ApiServices
{
    public class ApiException
    {
        public dynamic FireException(ResultClassForNonAuth result, Exception ex)
        {
            result.Code = (int)HttpStatusCode.InternalServerError;
            result.Msg = ex.Message;
            return result;
        }
        public dynamic FireException(ResultClass result, Exception ex)
        {
            result.Code = (int)HttpStatusCode.InternalServerError;
            result.Msg = ex.Message;
            return result;
        }
        public dynamic FireException(ResultClassToken result, Exception ex)
        {
            result.Code = (int)HttpStatusCode.InternalServerError;
            result.Msg = ex.Message;
            return result;
        }

        public dynamic FireException(ResultClassCommon result, Exception ex)
        {
            result.Code = (int)HttpStatusCode.InternalServerError;
            result.Msg = ex.Message;
            return result;
        }
    }
}