﻿using MeiHi.API.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeiHi.API.ViewModels
{
    public class ServiceModel
    {
        public long ServiceId { get; set; }
        public long ShopId { get; set; }
        public string TitleUrl { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public decimal CMUnitCost { get; set; }
        public decimal OriginalUnitCost { get; set; }
        public ShopModel Shop { get; set; }
        public string PurchaseNotes { get; set; }
        public string Designer { get; set; }
        public bool? IfSupportRealTimeRefund { get; set; }
        [Display(Name="是否已收藏")]
        public bool HaveAddedToFavorite { get; set; }
        public List<UserCommentsModel> UserComments { get; set; }
    }
}