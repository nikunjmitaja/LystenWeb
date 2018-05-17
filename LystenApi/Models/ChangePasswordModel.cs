using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LystenApi.Models
{
    public class ChangePasswordModel
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}