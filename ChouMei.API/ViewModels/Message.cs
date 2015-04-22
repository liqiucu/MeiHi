using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChouMei.API.ViewModels
{
    public class SystemMessage
    {
        public int MessageId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string DatePublished {get;set;}
    }

    public class NoticeBoard
    {
        public int BoardId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string DatePublished { get; set; }
    }

    public class FeedbackModel
    {
        public string Description { get; set; }
    }
}