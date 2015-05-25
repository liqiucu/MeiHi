using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MeiHi.CommonDll.Helper
{
    public class LuoSiMaoTextMessage
    {
        public static bool SendText(string mobile, string message)
        {
            message = "验证码 " + message + "【美嗨科技】";
            string username = "api";
            //string password = "key-e4ce14f0cd25bbd87a2ee45eba1a7a83";
            string password = "key-7b9bfef6b6960ee99046276d46e59206";
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

            if (response.StatusCode == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool SendShopText(string mobile)
        {
            string message = "此信息用于美嗨商户登陆系统,账号为本手机,初始密码为手机后六位,【美嗨科技】";
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

            if (response.StatusCode == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool SendBookingVerifyCode(string mobile, string message)
        {
            string username = "api";
            //string password = "key-e4ce14f0cd25bbd87a2ee45eba1a7a83";
            string password = "key-7b9bfef6b6960ee99046276d46e59206";
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

            if (response.StatusCode == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
