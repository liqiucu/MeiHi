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
    
    public partial class agent_get_campus_job_Result
    {
        public string Title { get; set; }
        public Nullable<long> CompanyId { get; set; }
        public string CompanyName { get; set; }
        public Nullable<int> PeopleRequired { get; set; }
        public int Gender { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> WorkFrom { get; set; }
        public Nullable<System.DateTime> WorkEnd { get; set; }
        public bool MonMorning { get; set; }
        public bool TueMorning { get; set; }
        public bool WedMorning { get; set; }
        public bool ThuMorning { get; set; }
        public bool FriMorning { get; set; }
        public bool SatMorning { get; set; }
        public bool SunMorning { get; set; }
        public bool MonAfternoon { get; set; }
        public bool TueAfternoon { get; set; }
        public bool WedAfternoon { get; set; }
        public bool ThuAfternoon { get; set; }
        public bool FriAfternoon { get; set; }
        public bool SatAfternoon { get; set; }
        public bool SunAfternoon { get; set; }
        public bool MonNight { get; set; }
        public bool TueNight { get; set; }
        public bool WedNight { get; set; }
        public bool ThuNight { get; set; }
        public bool FriNight { get; set; }
        public bool SatNight { get; set; }
        public bool SunNight { get; set; }
        public string Description { get; set; }
        public string Contact { get; set; }
        public string ContactPhone { get; set; }
        public bool Sticky { get; set; }
        public Nullable<System.DateTime> StickyExpired { get; set; }
        public bool Recommended { get; set; }
        public string JobTypeName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public long JobId { get; set; }
        public int JobTypeId { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public string TermName { get; set; }
    }
}
