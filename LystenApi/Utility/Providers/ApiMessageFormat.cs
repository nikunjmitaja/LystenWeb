using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LystenApi.Utility.Providers
{
    public class ApiMessageFormat
    {
        public string Success = "Success.";
        public string LoginUserRight = "User has no rights.";
        public string LoginUserIsNotActive = "User Is Inactive.";
        public string LoginUserInvalid = "Username and Password is Invalid";
        public string CurrentPaasswordNotSame = "Current Password Is not Match";


        public string ForgotMessage = "Your password sent sucessfully to your registered email Address.";
        public string ForgotMessageNotExist = "Email Address is not exists.";
        public string UserNotExist = "User is not exists.";

        public string TokenMessageNull = "Token Validation Failed.";

        public string GlobalError = "Something Went Wrong.";
        public string PartyNoData = "No party available.";
        public string PartyIsNotActive = "Party is not active.you have to activate first.";
        public string PartyNotExist = "Party is not exists.";

        public string PartyNameExist = "Party name is already exists.";
        public string BrokerNameExist = "Broker name is already exists.";

        public string PartySaved = "Party has been added successfully.";
        public string BrokerSaved = "Broker has been added successfully.";

        public string BrokerUpdate = "Broker has been updated successfully.";
        public string PartyUpdate = "Party has been updated successfully.";

        public string UnitSaveSuccess = "Unit has been added successfully.";
        public string UnitUpdateSuccess = "Unit has been updated successfully.";


        public string BrokerNoData = "No broker available.";
        public string BrokerIsNotActive = "Broker is not active.you have to activate first.";
        public string BrokerNotExist = "Broker is not exists.";

        public string MachineNoData = "No Machine Available.";
        public string MachineNameExists = "Machine name is already exists.";

        public string MachineIsNotActive = "Machine Is Not Active.You have to activate first.";

        public string CategoryNoData = "No Category Available.";
        public string CategoryIsExists = "Category name is already exists.";
        public string CategoryIsNotActive = "Category Is Not Active.You have to activate first.";

        public string UnitNoData = "No Unit Available.";
        public string UnitIsNotActive = "Unit Is Not Active.You have to activate first.";

        public string PolicyNoData = "No Policy Available.";
        public string PolicIsNotActive = "Policy Is Not Active.You have to activate first.";

        public string PolicyAddSuccess = "Policy has been added successfully.";
        public string PolicyUpdateSuccess = "Policy has been updated successfully.";

        public string PolicyDateMsg = "Policy date Must be grater than From your previous policy datee.";

        public string UserEMailExist = "Email Is Already Exists.Please Enter Diffrent Email";

        public string ItemNoData = "No Item Available.";

        public string PhysicalNoData = "No Physical Available.";

        public string ProductionNoData = "No Production Available.";

        public string DeliveryNoData = "No delivery available.";
        public string InvoiceNoData = "No invoice available.";
        public string CountryNoData= "No country available.";

        public string StateNoData = "No state available.";

        public string CityNoData = "No city available.";

        public string UserNameExist = "Username already exists.";

        public string AlreadyFavourite = "Already in favourite.";

        public string MessageNoData = "No messages found.";

        public string UserNodata = "No User found.";

        public string GroupNodata  ="No Group found.";

        public string EventNoData = "No Event found.";

        public string RequestCallNoData = "No data found.";

        public string AnswerNoData = "No answer found.";

        public string QuestionNodata = "No question found.";
    }
}