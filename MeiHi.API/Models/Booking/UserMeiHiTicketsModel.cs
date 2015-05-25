using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.API.ViewModels
{
    public class UserMeiHiTicketsModel
    {
        public long UserId { get; set; }

        public List<MeiHiTicketModel> NotUsedConsumeMeiHiTickets { get; set; }

        public List<MeiHiTicketModel> CalceledConsumeMeiHiTickets { get; set; }

        public List<MeiHiTicketModel> UsedConsumeMeiHiTickets { get; set; }
    }

    public class MeiHiTicketModel
    {
        public long BookingId { get; set; }
        public string Mobile { get; set; }
        public int Count { get; set; }
        public string ServiceName { get; set; }
        public string ShopName { get; set; }
        public decimal Cost { get; set; }

        public decimal MeiHiUnitCost { get; set; }

        public decimal OriginalUnitCost { get; set; }

        public int BuyCounts { get; set; }

        public DateTime DataCreate { get; set; }

        public string Desinger { get; set; }

        /// <summary>
        /// 订单状态(是否已经打回给店铺)
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 是否已消费
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        /// 是否已支付
        /// </summary>
        public bool IsBilling { get; set; }

        /// <summary>
        /// 是否申请退款（退款）
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// 退款成功
        /// </summary>
        public bool CancelSuccess { get; set; }

        public string Comment { get; set; }

        /// <summary>
        /// 美嗨券
        /// </summary>
        public string VerifyCode { get; set; }

        public DateTime DateModified { get; set; }
    }

    public class NotPayedBookingModel
    {
        public long BookingId { get; set; }

        public string ShopName { get; set; }

        public string ServiceName { get; set; }

        public decimal TotalCost { get; set; }

        public int Count { get; set; }
    }

    public class NotPayedBookingsModel
    {
        public long UserId { get; set; }

        public List<NotPayedBookingModel> NotPayedBookings { get; set; }
    }
}