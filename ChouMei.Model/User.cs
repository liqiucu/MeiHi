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
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.ActivityJobApplies = new HashSet<ActivityJobApply>();
            this.ActivityJobComplains = new HashSet<ActivityJobComplain>();
            this.CampusJobApplies = new HashSet<CampusJobApply>();
            this.CampusJobComplains = new HashSet<CampusJobComplain>();
            this.InternJobApplies = new HashSet<InternJobApply>();
            this.InternJobComplains = new HashSet<InternJobComplain>();
            this.ServiceJobApplies = new HashSet<ServiceJobApply>();
            this.ServiceJobComplains = new HashSet<ServiceJobComplain>();
            this.TutorJobApplies = new HashSet<TutorJobApply>();
            this.TutorJobComplains = new HashSet<TutorJobComplain>();
            this.UserAccounts = new HashSet<UserAccount>();
            this.UserCashIns = new HashSet<UserCashIn>();
            this.UserCashOuts = new HashSet<UserCashOut>();
            this.UserLogons = new HashSet<UserLogon>();
        }
    
        public long UserId { get; set; }
        public string Mobile { get; set; }
        public int SchoolId { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public Nullable<System.DateTime> DateofBirth { get; set; }
        public Nullable<int> Gender { get; set; }
        public Nullable<int> Tall { get; set; }
        public Nullable<int> RegionId { get; set; }
        public string ProfilePhoto { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string QQ { get; set; }
        public string Email { get; set; }
        public string Major { get; set; }
        public string WorkExperience { get; set; }
        public decimal Balance { get; set; }
        public decimal FrozenBalance { get; set; }
        public string Device { get; set; }
        public string DeviceToken { get; set; }
        public string BaiduUserId { get; set; }
        public string Version { get; set; }
        public Nullable<int> DownloadFromApplicationId { get; set; }
        public bool VoiceAlert { get; set; }
        public bool Vibrate { get; set; }
        public bool IsLogin { get; set; }
        public bool Manual { get; set; }
        public int NotificationCount { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
    
        public virtual ICollection<ActivityJobApply> ActivityJobApplies { get; set; }
        public virtual ICollection<ActivityJobComplain> ActivityJobComplains { get; set; }
        public virtual ICollection<CampusJobApply> CampusJobApplies { get; set; }
        public virtual ICollection<CampusJobComplain> CampusJobComplains { get; set; }
        public virtual ICollection<InternJobApply> InternJobApplies { get; set; }
        public virtual ICollection<InternJobComplain> InternJobComplains { get; set; }
        public virtual Region Region { get; set; }
        public virtual School School { get; set; }
        public virtual ICollection<ServiceJobApply> ServiceJobApplies { get; set; }
        public virtual ICollection<ServiceJobComplain> ServiceJobComplains { get; set; }
        public virtual ICollection<TutorJobApply> TutorJobApplies { get; set; }
        public virtual ICollection<TutorJobComplain> TutorJobComplains { get; set; }
        public virtual ICollection<UserAccount> UserAccounts { get; set; }
        public virtual ICollection<UserCashIn> UserCashIns { get; set; }
        public virtual ICollection<UserCashOut> UserCashOuts { get; set; }
        public virtual ICollection<UserLogon> UserLogons { get; set; }
    }
}
