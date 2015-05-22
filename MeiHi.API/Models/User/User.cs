using MeiHi.API.Models;
using MeiHi.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeiHi.API.ViewModels
{
    public class UpdateUserInfo
    {
        /// <summary>
        /// 美嗨号
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 移动电话
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Nullable<int> Gender { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string ProfilePhoto { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// 设备型号
        /// </summary>
        public string Device { get; set; }
        /// <summary>
        /// 型号版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 从哪里下载的
        /// </summary>
        public Nullable<int> DownloadFromApplicationId { get; set; }
        /// <summary>
        /// 头发长度 1 长发 2 中长发 3短发
        /// </summary>
        public Nullable<int> HairOfLegth { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public Nullable<System.DateTime> BirthDay { get; set; }
        /// <summary>
        /// token 用来登陆验证
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 美嗨登陆验证码
        /// </summary>
        public string VerifyCode { get; set; }

        /// <summary>
        /// 用户评论数
        /// </summary>
        public int UserCommentCount { get; set; }

        /// <summary>
        /// 美嗨券数
        /// </summary>
        public int MeiHiBookingVerifyCodeCount { get; set; }

        /// <summary>
        /// 是否已经登陆
        /// </summary>
        public bool IsLogin { get; set; }
    }

    public class UserHomeModel
    {
        /// <summary>
        /// 用户美嗨号
        /// </summary>
        public long UserId { get; set; }

        public string UserImageUrl { get; set; }

        public string MeiHiUserName { get; set; }

        public decimal Balance { get; set; }

        /// <summary>
        /// 用户评论数
        /// </summary>
        public int UserCommentCount { get; set; }

        /// <summary>
        /// 美嗨券数
        /// </summary>
        public int MeiHiBookingVerifyCodeCount { get; set; }
    }

    public class UserFavority
    {
        public long UserId { get; set; }

        public List<ShopModel> FavorityShops { get; set; }

        public List<FavorityService> FavorityServices { get; set; }
    }

    public class FavorityService
    {
        public long ShopId { get; set; }
        public long ServiceId { get; set; }

        public string ShopName { get; set; }

        public string ShopTitleImage { get; set; }

        public int MyProperty { get; set; }
    }

    public class UserDetailModel
    {
        /// <summary>
        /// 用户美嗨号
        /// </summary>
        public long UserId { get; set; }

        public string UserImageUrl { get; set; }

        public string MeiHiUserName { get; set; }

        /// <summary>
        /// 0 男 1 女
        /// </summary>
        public int? Gender { get; set; }

        /// <summary>
        /// 1 2 3
        /// </summary>
        public int? HairLenth { get; set; }

        public string Address { get; set; }

        public DateTime? BirthDay { get; set; }

    }

    public class UserComment
    {
        public long UserId { get; set; }

        public string ServiceName { get; set; }

        public string Comment { get; set; }

        public int Rate { get; set; }
    }

    public class CreateBankAccountModel
    {
        public string AccountName { get; set; }
        public string AccountNo { get; set; }
        public int BankId { get; set; }
    }
}