//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChouMei.Model
{
    using System;
    
    public partial class wechat_job_search_service_Result
    {
        public long JobId { get; set; }
        public int JobTypeId { get; set; }
        public string JobType { get; set; }
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public string PositionName { get; set; }
        public string CompanyName { get; set; }
        public bool IncludeDinner { get; set; }
        public string Term { get; set; }
        public bool IncludeRoom { get; set; }
        public bool IncludeCommission { get; set; }
        public int Gender { get; set; }
        public decimal Wage { get; set; }
        public string WageUnit { get; set; }
        public string Region { get; set; }
        public string DatePublished { get; set; }
        public int ViewTimes { get; set; }
        public bool Verified { get; set; }
        public Nullable<bool> Deposit { get; set; }
        public bool Reptile { get; set; }
        public bool IgnoreTimeSheet { get; set; }
        public bool Sticky { get; set; }
    }
}
