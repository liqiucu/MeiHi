using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.API.Models.UserComments
{
    public class UserCommentsModel
    {
        public long UserCommentId { get; set; }
        public long ShopId { get; set; }
        public long UserId { get; set; }
        public string UserFullName { get; set; }
        public string ServiceName { get; set; }
        public int Rate { get; set; }
        public string Comment { get; set; }
        
        public System.DateTime DateCreated { get; set; }
        public List<string> UserSharedImgaeList { get; set; }

        public List<string> MeiHiReply { get; set; }
    }
}