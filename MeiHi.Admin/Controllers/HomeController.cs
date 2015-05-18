using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Text;

namespace MeiHi.Admin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
            //  return RedirectToAction("ShopManege", "Shop");
        }

        public ActionResult About()
        {
            //MD5Cng
            //MD5CryptoServiceProvider
            MD5 md = MD5.Create();
            var temp = md.ComputeHash(System.Text.Encoding.UTF8.GetBytes("12345678"));

            var dadad = BitConverter.ToString(temp).Replace("-", "");


            var MD5info1 = "Amount=100.01&BillNo=1688888888&MerNo=168885&ReturnURL=http://mystore.com/payresult&" + dadad.ToUpper();
            byte[] bytes = md.ComputeHash(System.Text.Encoding.UTF8.GetBytes(MD5info1));

            var addadada = BitConverter.ToString(bytes).Replace("-", "").ToUpper();


            var MerNo = "168885";
            var MD5key="12345678";
            var Amount = "100.01";
            var BillNo = "888888888888888";
            var ReturnURL = "http://mystore.com/payResult";
            var NotifyURL = "http://mystore.com/NotifyResult";
            var PayType = "CSPAY";
            var PaymentType = "ICBC";

            byte[] byteArray = new byte[2000];
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri("https://www.95epay.cn/sslpayment"));

            webRequest.Headers.Add("MD5info", addadada);
            webRequest.Headers.Add("MerNo", MerNo);
            webRequest.Headers.Add("MD5key", MD5key);
            webRequest.Headers.Add("Amount", Amount);
            webRequest.Headers.Add("BillNo", BillNo);
            webRequest.Headers.Add("ReturnURL", ReturnURL);
            webRequest.Headers.Add("NotifyURL", NotifyURL);
            webRequest.Headers.Add("PayType", PayType);
            webRequest.Headers.Add("PaymentType", PaymentType);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = byteArray.Length;

            Stream newStream = webRequest.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}