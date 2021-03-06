//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LystenApi.Db
{
    using System;
    using System.Collections.Generic;
    
    public partial class Calling_Request
    {
        public int Id { get; set; }
        public Nullable<int> ToUserId { get; set; }
        public Nullable<int> FromUserId { get; set; }
        public Nullable<int> CallingPriceId { get; set; }
        public Nullable<System.DateTime> CallingDateTime1 { get; set; }
        public Nullable<System.DateTime> CallingDateTime2 { get; set; }
        public Nullable<System.DateTime> CallingDateTime3 { get; set; }
        public Nullable<bool> IsAccept { get; set; }
        public Nullable<bool> IsReject { get; set; }
        public string RejectedNote { get; set; }
        public string PaymentStatus { get; set; }
        public string TransactionId { get; set; }
        public Nullable<System.DateTime> AcceptDatetime { get; set; }
        public string TimeZoneId { get; set; }
        public string TotalCallingTime { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<System.DateTime> AcceptDatetimeUTC { get; set; }
    
        public virtual CallingPriceMaster CallingPriceMaster { get; set; }
        public virtual User_Master User_Master { get; set; }
        public virtual User_Master User_Master1 { get; set; }
    }
}
