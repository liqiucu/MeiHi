using ChouMei.API.ViewModels;
using ChouMei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChouMei.API.Controllers
{
    [RoutePrefix("message")]
    public class MessageController : ApiController
    {
        private SiTuXiaoYuanEntities db = new SiTuXiaoYuanEntities();

        [Route("system")]
        [HttpGet]
        public object System()
        {
            var messages = db.SystemMessages;
            List<object> results = new List<object>();
            foreach (var m in messages)
            {
                results.Add(new
                {
                    messageId = m.SystemMessageId,
                    title = m.Title,
                    datePublished = m.DateCreated.ToString("MM-dd")
                });
            }
            return new
            {
                jsonStatus = 1,
                results = results
            };
        }

        [Route("board")]
        [HttpGet]
        public object Board()
        {
            var messages = db.NoticeBoards;
            List<object> results = new List<object>();
            foreach (var m in messages)
            {
                results.Add(new
                {
                    boardId = m.BoardId,
                    title = m.Title,
                    datePublished = m.DatePublished.Value.ToString("MM-dd")
                });
            }
            return new
            {
                jsonStatus = 1,
                results = results
            };
        }

        [Route("system/{id}")]
        [HttpGet]
        public object System(int id)
        {
            var m = db.SystemMessages.SingleOrDefault(r => r.SystemMessageId == id);
            return new
            {
                jsonStatus = 1,
                result = new
                {
                    //messageId = m.MessageId,
                    //title = m.Title,
                    //content = m.Content,
                    //datePublished = m.DatePublished.Value.ToString("MM-dd")
                }
            };
        }

        [Route("board/{id}")]
        [HttpGet]
        public object Board(int id)
        {
            var m = db.NoticeBoards.SingleOrDefault(r => r.BoardId == id);
            return new
            {
                jsonStatus = 1,
                result = new
                {
                    messageId = m.BoardId,
                    title = m.Title,
                    content = m.Content,
                    datePublished = m.DatePublished.Value.ToString("MM-dd")
                }
            };
        }

        [Route("feedback")]
        [HttpPost]
        [ApiAuthorize]
        public object Feedback([FromBody] FeedbackModel feedbackModel)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.UserLogons.SingleOrDefault(r => r.Token == token);
            if (login != null)
            {
                var feedback = new Feedback()
                {
                    UserId = login.UserId,
                    Description = feedbackModel.Description,
                    DateCreated = DateTime.Now
                };
                db.Feedbacks.Add(feedback);
            }
            else
            {
                var comLogin = db.CompanyLogons.SingleOrDefault(r => r.Token == token);
                if (comLogin != null)
                {
                    var compFeedback = new Feedback()
                    {
                        CompanyId = comLogin.CompanyId,
                        Description = feedbackModel.Description,
                        DateCreated = DateTime.Now
                    };
                    db.Feedbacks.Add(compFeedback);
                }
            }
            db.SaveChanges();

            return new
            {
                jsonStatus = 1
            };
        }
    }
}
