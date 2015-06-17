using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PagedList;
using MeiHi.Admin.Models.UserComments;
using MeiHi.Admin.Models.Booking;

namespace MeiHi.Admin.Models.User
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

    public class UserInfoModel
    {
        [Display(Name="用户ID")]
        public long UserId { get; set; }
        [Display(Name = "手机")]
        public string Mobile { get; set; }
        [Display(Name = "昵称")]
        public string FullName { get; set; }
        [Display(Name = "性别")]
        public string Gender { get; set; }
        [Display(Name = "头像")]
        public string ProfilePhoto { get; set; }
        [Display(Name = "美嗨余额")]
        public decimal Balance { get; set; }
        [Display(Name = "设备")]
        public string Device { get; set; }
        [Display(Name = "版本号")]
        public string Version { get; set; }
        [Display(Name = "从哪下的")]
        public Nullable<int> DownloadFromApplicationId { get; set; }
        [Display(Name = "发长")]
        public string HairOfLegth { get; set; }
        [Display(Name = "详细地址")]
        public string Address { get; set; }
        [Display(Name = "生日")]
        public Nullable<System.DateTime> BirthDay { get; set; }
        [Display(Name = "注册时间")]
        public System.DateTime DateCreated { get; set; }
    }
}