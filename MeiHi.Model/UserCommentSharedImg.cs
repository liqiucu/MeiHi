//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MeiHi.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserCommentSharedImg
    {
        public long UserCommentSharedImgId { get; set; }
        public string ImgUrl { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
        public long UserCommentId { get; set; }
    
        public virtual UserComments UserComments { get; set; }
    }
}
