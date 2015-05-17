using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MeiHi.Admin.Helper
{
    public class DodgeTextMessage
    {
        public static void SendText(string mobile, string message)
        {
            string endPoint = "http://sms.tourerp.cn/smsSending.do";
            Dictionary<string, string> paramters = new Dictionary<string, string>();
            paramters.Add("userId", "sf");
            paramters.Add("pwd", "1bbd886460827015e5d605ed44252251");
            paramters.Add("mobiles", mobile);
            paramters.Add("message", "【思途校园兼职实习平台】验证码 " + message);

            var populatedEndPoint = CreateFormattedPostRequest(paramters);
            byte[] bytes = Encoding.UTF8.GetBytes(populatedEndPoint);

            HttpWebRequest request = CreateWebRequest(endPoint, bytes.Length);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException(message);
                }
            }
        }

        private static HttpWebRequest CreateWebRequest(string endPoint, Int32 contentLength)
        {
            var request = (HttpWebRequest)WebRequest.Create(endPoint);

            request.Method = "POST";
            request.ContentLength = contentLength;
            request.ContentType = "application/x-www-form-urlencoded";

            return request;
        }

        private static string CreateFormattedPostRequest(Dictionary<string, string> values)
        {
            var paramterBuilder = new StringBuilder();
            var counter = 0;
            foreach (var value in values)
            {
                paramterBuilder.AppendFormat("{0}={1}", value.Key, HttpUtility.UrlEncode(value.Value));

                if (counter != values.Count - 1)
                {
                    paramterBuilder.Append("&");
                }

                counter++;
            }

            return paramterBuilder.ToString();
        }
    }
}
