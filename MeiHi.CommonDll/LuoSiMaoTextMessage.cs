using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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


            ////将要加密的字符串转化为byte类型
            //byte[] data = System.Text.Encoding.Unicode.GetBytes("daishengpu");
            ////创建一个Md5对象
            //System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            ////加密Byte[]数组
            //byte[] result = md5.ComputeHash(data);
            ////将加密后的数组转化为字段
            //string sResult = System.Text.Encoding.Unicode.GetString(result);
            //var url1 = "http://api.cnsms.cn/?ac=send&uid=100860&pwd=" + sResult+"&mobile=13167226393&content=123";
            ////http://api.cnsms.cn/?ac=send&uid=100860&pwd=fa246d0262c3925617b0c72bb20eeb1d&mobile=15901997305&content=%D6%D0%B9%FA%B6%CC%D0%C5%CD%F8%B7%A2%CB%CD%B2%E2%CA%D4
        }
        public static bool SendShopText(string mobile, string comment)
        {
            string message = comment + " 此信息用于美嗨商户登陆系统,账号为本手机,初始密码为手机后六位,【美嗨科技】";
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
