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
    
    public partial class UserAccount
    {
        public UserAccount()
        {
            this.UserCashOuts = new HashSet<UserCashOut>();
        }
    
        public long AccountId { get; set; }
        public long UserId { get; set; }
        public int BankId { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public bool Default { get; set; }
        public Nullable<System.DateTime> DateDeleted { get; set; }
        public System.DateTime DateCreated { get; set; }
    
        public virtual Bank Bank { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<UserCashOut> UserCashOuts { get; set; }
    }
}
