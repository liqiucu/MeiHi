using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using MeiHi.API.Models;

namespace MeiHi.API.Helper
{
    /// <summary>
    /// 提供 Http 相关方法。
    /// </summary>
    public class HttpUtils
    {
        /// <summary>
        /// 地名必须详细，不然百度地图返回null
        /// </summary>
        /// <returns></returns>
        public static MeiHiDistanceModel RequestApi(
            List<string> destinations,
            string ak = "yAcXaLCAD2OIEeoSRMwEyvNU",
            string origin = "上海长宁区福泉路99号携程旅游")
        {
            //string destination1 = "上海虹桥火车站",
            //string destination2 = "上海东方明珠",
            //string destination3 = "上海交通大学闵行校区",
            //string destination4 = "上海闵行区爱博家园四村",
            //string destination5 = "上海闵行区爱博家园二村"
            //yAcXaLCAD2OIEeoSRMwEyvNU        AK
            //http://api.map.baidu.com/direction/v1/routematrix

            string destinationsUrl = string.Join("|", destinations);
            string apiUrl = "http://api.map.baidu.com/direction/v1/routematrix?output=json&ak="
                + ak + "&origins="
                + origin + "&destinations=" +
                destinationsUrl;

            var info = new MeiHiDistanceModel();
            try
            {
                //以 Get 形式请求 Api 地址
                string result = HttpUtils.DoGet(apiUrl);
                info = JsonConvert.DeserializeObject<MeiHiDistanceModel>(result);
            }
            catch (Exception)
            {
                throw;
            }

            return info;
        }

        /// <summary>
        /// 执行HTTP GET请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>HTTP响应</returns>
        public static string DoGet(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.Expect100Continue = false;
            req.Method = "GET";
            req.KeepAlive = true;
            req.UserAgent = "MeiHi";
            req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

            HttpWebResponse rsp = null;
            try
            {
                rsp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException webEx)
            {
                if (webEx.Status == WebExceptionStatus.Timeout)
                {
                    rsp = null;
                }
            }

            if (rsp != null)
            {
                if (rsp.CharacterSet != null)
                {
                    Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                    return GetResponseAsString(rsp, encoding);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        private static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            StringBuilder result = new StringBuilder();
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);

                // 每次读取不大于256个字符，并写入字符串
                char[] buffer = new char[256];
                int readBytes = 0;
                while ((readBytes = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    result.Append(buffer, 0, readBytes);
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Status == WebExceptionStatus.Timeout)
                {
                    result = new StringBuilder();
                }
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }

            return result.ToString();
        }

        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典。</param>
        /// <returns>URL编码后的请求数据。</returns>
        private static string BuildPostData(IDictionary<string, string> parameters)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }

                    postData.Append(name);
                    postData.Append("=");
                    postData.Append(Uri.EscapeDataString(value));
                    hasParam = true;
                }
            }

            return postData.ToString();
        }

    }
}