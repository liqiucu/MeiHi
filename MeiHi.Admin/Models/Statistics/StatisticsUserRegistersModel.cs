using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Models.Statistics
{
    public class StatisticsUserRegistersModel
    {
        public int TotalRegisterCount { get; set; }

        public int TotalDownloadCount { get; set; }

        public int TodayRegisterCount { get; set; }

        //public int YesterdayRegisterCount { get; set; }

        public int TodayDownloadCount { get; set; }

        // public int YesterdayDownloadCount { get; set; }

        public List<StatisticsTypeModel> StatisticsTypes { get; set; }
        /// <summary>
        /// 1 是今日 2是昨日 3是这周 4是上周 5是这月 6是上月
        /// </summary>
        public int SearchType { get; set; }
    }

    public class StatisticsTypeModel
    {
        /// <summary>
        /// IOS ANDRIA WP
        /// </summary>
        public string Type { get; set; }

        public int RegisterCount { get; set; }

        public int DownloadCount { get; set; }
    }
}