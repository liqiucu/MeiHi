using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MeiHi.FrameWork.Helper
{
    public static class Extensions
    {
        public static int? ToInt(this string str)
        {
            int val;
            if (int.TryParse(str, out val))
                return val;
            return null;
        }

        public static long? ToLong(this string str)
        {
            long val;
            if (long.TryParse(str, out val))
                return val;
            return null;
        }
        public static decimal? ToDecimal(this string str)
        {
            decimal val;
            if (decimal.TryParse(str, out val))
                return val;
            return null;
        }

        public static double? ToDouble(this string str)
        {
            double val;
            if (double.TryParse(str, out val))
                return val;
            return null;
        }

        public static bool? ToBool(this string value)
        {
            bool val;
            if (value == null) return null;
            if (value == "on") return true;
            if (value == "off") return false;
            if (value.ToLower().Trim().StartsWith("y")) return true;
            if (value.ToLower().Trim().StartsWith("n")) return false;
            if (value.ToLower().Trim().StartsWith("t")) return true;
            if (value.ToLower().Trim().StartsWith("f")) return false;
            if (value.ToLower().Trim().StartsWith("1")) return true;
            if (value.ToLower().Trim().StartsWith("0")) return false;
            if (bool.TryParse(value, out val))
                return val;
            return null;
        }

        public static DateTime? ToDateTime(this string value)
        {
            if (value == null) return null;
            DateTime val;
            if (DateTime.TryParse(value, out val))
                return val;
            return null;
        }

        public static string Left(this string value, int Number)
        {
            if (value.Length > Number)
                return value.Substring(0, Number);
            else
                return value;
        }

        public static int ToInt(this Enum enumValue)
        {
            return Convert.ToInt32(enumValue);
        }

        public static string ToBase64String(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                var bytes = Encoding.UTF8.GetBytes(str);
                return Convert.ToBase64String(bytes);
            }
            return string.Empty;
        }

        public static string FromBase64String(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                byte[] data = Convert.FromBase64String(str);
                return Encoding.UTF8.GetString(data);
            }
            return string.Empty;
        }

        public static List<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> text, Func<T, string> value)//, string defaultOption)
        {
            var items = enumerable.Select(f => new SelectListItem()
            {
                Text = text(f),
                Value = value(f)
            }).ToList();
            //items.Insert(0, new SelectListItem()
            //{
            //    Text = defaultOption,
            //    Value = "-1"
            //});
            return items;
        }

        public static List<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> text, Func<T, string> value, string selectedValue)
        {
            var items = enumerable.Select(f => new SelectListItem()
            {
                Text = text(f),
                Value = value(f),
                Selected = value(f) == selectedValue
            }).ToList();
            return items;
        }

        /// 判断是否被选中。
        /// </summary>
        /// <param name="form">路由实例对象。</param>
        /// <param name="key">路由键。</param>
        /// <param name="defaultValue">返回默认值。</param>
        /// <returns>返回是否被选中。</returns>
        public static bool IsChecked(this FormCollection form, string key, bool defaultValue = false)
        {
            var values = form.GetValues(key);
            if (values != null)
                return values[0].ToBool().Value;
            return defaultValue;
        }

        private readonly static string reservedCharacters = "!*'();:@&=+$,/?%#[]";
        public static string UrlEncode(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return String.Empty;

            var sb = new StringBuilder();

            foreach (char @char in str)
            {
                if (reservedCharacters.IndexOf(@char) == -1)
                    sb.Append(@char);
                else
                    sb.AppendFormat("%{0:X2}", (int)@char);
            }
            return sb.ToString();
        }

        public static string UrlDecode(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return HttpUtility.UrlDecode(str);
            }
            return string.Empty;
        }

        public static bool Contains(this string[] source, string value, bool ignoreCase)
        {
            bool result = false;
            foreach (var s in source)
            {
                if (ignoreCase && s.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                    result = true;
                else if (s.Equals(value))
                    result = true;
            }
            return result;
        }

        public static string StripHtml(this string str)
        {
            if (!string.IsNullOrEmpty(str))
                return Regex.Replace(str, @"<[^>]*>", String.Empty);
            return string.Empty;
        }

        public static string ToTimePassed(this DateTime dt)
        {
            if (DateTime.Now > dt)
            {
                if ((DateTime.Now - dt).TotalMinutes < 60)
                    return (int)(DateTime.Now - dt).TotalMinutes + "分钟前";
                else if ((DateTime.Now - dt).TotalHours < 24)
                    return (int)(DateTime.Now - dt).TotalHours + "小时前";
                else if (DateTime.Now.MonthDifference(dt) < 1)
                    return (int)(DateTime.Now - dt).TotalDays + "天前";
                else 
                    return DateTime.Now.MonthDifference(dt) + "月前";
            }
            else
            {
                if ((dt - DateTime.Now).TotalMinutes < 60)
                    return (int)(dt - DateTime.Now).TotalMinutes + "分钟后";
                else if ((dt - DateTime.Now).TotalHours < 24)
                    return (int)(dt - DateTime.Now).TotalHours + "小时后";
                else if (dt.MonthDifference(DateTime.Now) < 1)
                    return (int)(dt - DateTime.Now).TotalDays + "天后";
                else
                    return dt.MonthDifference(DateTime.Now) + "月后";
            }
        }

        public static int MonthDifference(this DateTime lValue, DateTime rValue)
        {
            return (lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year);
        }
    }
}