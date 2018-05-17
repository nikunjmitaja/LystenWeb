using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LystenApi.ViewModel
{
    public class ForgotViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Displayname { get; set; }
        public string CompanyName { get; set; }
        public string CompanyId { get; set; }
    }
}