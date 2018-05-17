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
    
    public partial class Message
    {
        public Message()
        {
            this.MessageRecipients = new HashSet<MessageRecipient>();
        }
    
        public int Id { get; set; }
        public string Subject { get; set; }
        public Nullable<int> CreatorId { get; set; }
        public string Body { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ParentMessageId { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public Nullable<bool> IsReminder { get; set; }
        public Nullable<System.DateTime> NextReminder { get; set; }
        public Nullable<int> ReminderFrequencyId { get; set; }
    
        public virtual Message Message1 { get; set; }
        public virtual Message Message2 { get; set; }
        public virtual User_Master User_Master { get; set; }
        public virtual ICollection<MessageRecipient> MessageRecipients { get; set; }
    }
}
