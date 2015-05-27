using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.API.ViewModels
{
    public class UserFavoritesModel
    {
        public long UserId { get; set; }

        public List<ShopModel> FavoritieShops { get; set; }

        public List<ServiceModel> FavoritieSerivces { get; set; }
    }
}