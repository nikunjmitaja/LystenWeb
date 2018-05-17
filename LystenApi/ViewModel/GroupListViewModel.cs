using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LystenApi.ViewModel
{
    public class GroupListViewModel
    {
        public int UserId { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public List<GroupViewModel> GroupList { get; set; }
    }
}