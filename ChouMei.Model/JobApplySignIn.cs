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
    
    public partial class JobApplySignIn
    {
        public long SignInId { get; set; }
        public System.DateTime Date { get; set; }
        public long JobId { get; set; }
        public int JobTypeId { get; set; }
        public long UserId { get; set; }
        public Nullable<System.DateTime> DateConfirmedSignedIn { get; set; }
        public Nullable<System.DateTime> DateConfirmedSignedOut { get; set; }
        public Nullable<System.DateTime> DateUpdatedSignedIn { get; set; }
        public Nullable<System.DateTime> DateUpdatedSignedOut { get; set; }
        public Nullable<decimal> WageAdjusted { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> DateAbsent { get; set; }
        public Nullable<int> AbsentComplainId { get; set; }
        public System.DateTime DateCreated { get; set; }
    }
}
