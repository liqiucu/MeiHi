using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PagedList;
using MeiHi.Shop.Models.UserComments;
using MeiHi.Shop.Models.Booking;

namespace MeiHi.Shop.Models.User
{
   
    public class UsersModel
    {
        public StaticPagedList<UserModel> Users { get; set; }
    }

    public class UserCommentsListModel
    {
        public string ShopName { get; set; }
        public long ShopId { get; set; }

        public string UserFullName { get; set; }

        public long UserId { get; set; }

        public StaticPagedList<UserCommentsModel> UserCommentsList { get; set; }
    }

    public class UserBookingsModel
    {
        public decimal TotalCost { get; set; }
        public int TotalCount { get; set; }

        public long ShopId { get; set; }

        public string AliAccount { get; set; }

        public string WeiXinAccount { get; set; }
        public StaticPagedList<BookingModel> UserBookings { get; set; }
    }

    public class UserCommentsReplyModel
    {
        public long ReplyId { get; set; }

        public long UserId { get; set; }

        public long ShopId { get; set; }

        public string UserName { get; set; }

        public long UserCommentId { get; set; }

        public string UserContent { get; set; }

        public string MeiHiContent { get; set; }

        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
    }

    public class UserModel
    {
        [Display(Name = "美嗨ID")]
        public long UserId { get; set; }

        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Display(Name = "移动电话")]
        public string Mobile { get; set; }

        [Display(Name = "阿里账号")]
        public string AliPayAccount { get; set; }

        [Display(Name = "微信账号")]
        public string WeiXinPayAccount { get; set; }

        [Display(Name="美嗨余额")]
        public decimal Blance { get; set; }

        [Display(Name = "订单数")]
        public int HaveBillingCount { get; set; }

        [Display(Name = "消费总额")]
        public decimal HaveBillingMoney { get; set; }


        [Display(Name = "正在申请退款")]
        public bool HaveCancelBooking { get; set; }
    }
}