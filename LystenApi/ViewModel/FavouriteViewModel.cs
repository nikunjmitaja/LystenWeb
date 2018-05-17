using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LystenApi.ViewModel
{
    public class FavouriteViewModel
    {
        public int UserId { get;  set; }
        public int FavoriteUserId { get; set; }
        public string IsAdded { get;  set; }
    }
}