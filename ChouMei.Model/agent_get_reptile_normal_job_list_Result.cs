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
    
    public partial class agent_get_reptile_normal_job_list_Result
    {
        public System.DateTime datefrom { get; set; }
        public System.DateTime dateexpiry { get; set; }
        public long JobId { get; set; }
        public Nullable<long> CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyMobile { get; set; }
        public string Title { get; set; }
        public int JobTypeId { get; set; }
        public string JobTypeName { get; set; }
        public int StatusId { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public Nullable<System.DateTime> DatePublished { get; set; }
        public Nullable<System.DateTime> DateDeleted { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
        public bool Recommended { get; set; }
        public bool Sticky { get; set; }
        public Nullable<int> AgentUserId { get; set; }
        public Nullable<int> AdminId { get; set; }
        public Nullable<int> TotalCount { get; set; }
        public Nullable<long> RowNumber { get; set; }
    }
}
