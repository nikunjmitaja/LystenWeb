using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LystenApi.Models
{
    public class StripCardViewModel
    {
        public string CardId { get; set; }

        public string CustomerId { get; set; }

        public string Last4 { get; set; }
     
    }
}