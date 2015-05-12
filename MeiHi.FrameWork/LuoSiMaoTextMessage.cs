using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MeiHi.FrameWork.Helper
{
    public class LuoSiMaoTextMessage
    {
        public static void SendText(string mobile, string message)
        {
            message = "验证码 " + message + "【思途校园】";
            string username = "api";
            string password = "key-e4ce14f0cd25bbd87a2ee45eba1a7a83";
            string url = "https://sms-api.luosimao.com/v1/send.json";

            byte[] byteArray = Encoding.UTF8.GetBytes("mobile=" + mobile + "&message=" + message);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
            string auth = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(username + ":" + password));
            webRequest.Headers.Add("Authorization", auth);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = byteArray.Length;

            Stream newStream = webRequest.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            StreamReader php = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string Message = php.ReadToEnd();

            System.Console.Write(Message);
            System.Console.Read();
        }
    }
}
