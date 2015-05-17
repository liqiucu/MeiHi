using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiHi.Admin.Helper
{
    public sealed class IPAddr
    {
        // Fields
        private static string _dataPath;
        private static string _ip;
        private static string country;
        private static int countryFlag;
        private static long endIp;
        private static long endIpOff;
        private static string errMsg;
        private static long firstStartIp;
        private static long lastStartIp;
        private static string local;
        private static System.IO.FileStream objfs;
        private static long startIp;

        // Methods
        static IPAddr()
        {
            _dataPath = "";
            _ip = "0.0.0.0";
            firstStartIp = 0;
            lastStartIp = 0;
            objfs = null;
            startIp = 0;
            endIp = 0;
            countryFlag = 0;
            endIpOff = 0;
            errMsg = null;
        }


        public IPAddr()
        {
        }

        private static string GetCountry()
        {
            switch (countryFlag)
            {
                case 1:
                case 2:
                    country = GetFlagStr(endIpOff + 4);
                    local = (1 == countryFlag) ? " " : GetFlagStr(endIpOff + 8);
                    break;

                default:
                    country = GetFlagStr(endIpOff + 4);
                    local = GetFlagStr(objfs.Position);
                    break;
            }
            return " ";
        }

        private static long GetEndIp()
        {
            objfs.Position = endIpOff;
            byte[] buffer = new byte[5];
            objfs.Read(buffer, 0, 5);
            endIp = ((Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[2].ToString()) * 0x100L) * 0x100L)) + (((Convert.ToInt64(buffer[3].ToString()) * 0x100L) * 0x100L) * 0x100L);
            countryFlag = buffer[4];
            return endIp;
        }

        private static string GetFlagStr(long offSet)
        {
            int num = 0;
            byte[] buffer = new byte[3];
            while (true)
            {
                objfs.Position = offSet;
                num = objfs.ReadByte();
                if ((num != 1) && (num != 2))
                {
                    break;
                }
                objfs.Read(buffer, 0, 3);
                if (num == 2)
                {
                    countryFlag = 2;
                    endIpOff = offSet - 4;
                }
                offSet = (Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[2].ToString()) * 0x100L) * 0x100L);
            }
            if (offSet < 12)
            {
                return " ";
            }
            objfs.Position = offSet;
            return GetStr();
        }

        private static long GetStartIp(long recNO)
        {
            long num = firstStartIp + (recNO * 7);
            objfs.Position = num;
            byte[] buffer = new byte[7];
            objfs.Read(buffer, 0, 7);
            endIpOff = (Convert.ToInt64(buffer[4].ToString()) + (Convert.ToInt64(buffer[5].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[6].ToString()) * 0x100L) * 0x100L);
            startIp = ((Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[2].ToString()) * 0x100L) * 0x100L)) + (((Convert.ToInt64(buffer[3].ToString()) * 0x100L) * 0x100L) * 0x100L);
            return startIp;
        }

        private static string GetStr()
        {
            byte num = 0;
            byte num2 = 0;
            string str = "";
            byte[] bytes = new byte[2];
            while (true)
            {
                num = (byte)objfs.ReadByte();
                if (num == 0)
                {
                    return str;
                }
                if (num > 0x7f)
                {
                    num2 = (byte)objfs.ReadByte();
                    bytes[0] = num;
                    bytes[1] = num2;
                    Encoding encoding = Encoding.GetEncoding("GB2312");
                    if (num2 == 0)
                    {
                        return str;
                    }
                    str = str + encoding.GetString(bytes);
                }
                else
                {
                    str = str + ((char)num);
                }
            }
        }

        private static string IntToIP(long ip_Int)
        {
            long num = (long)((ip_Int & 0xff000000L) >> 0x18);
            if (num < 0L)
            {
                num += 0x100L;
            }
            long num2 = (ip_Int & 0xff0000L) >> 0x10;
            if (num2 < 0L)
            {
                num2 += 0x100L;
            }
            long num3 = (ip_Int & 0xff00L) >> 8;
            if (num3 < 0L)
            {
                num3 += 0x100L;
            }
            long num4 = ip_Int & 0xffL;
            if (num4 < 0L)
            {
                num4 += 0x100L;
            }
            return (num.ToString() + "." + num2.ToString() + "." + num3.ToString() + "." + num4.ToString());
        }

        public static string IPLocate(string dataPath, string ip)
        {
            _dataPath = dataPath;
            _ip = ip;
            QQwry();
            return (country + local).Replace("CZ88.NET", "");
        }

        private static long IpToInt(string ip)
        {
            char[] separator = new char[] { '.' };
            if (ip.Split(separator).Length == 3)
            {
                ip = ip + ".0";
            }
            string[] strArray = ip.Split(separator);
            long num2 = ((long.Parse(strArray[0]) * 0x100L) * 0x100L) * 0x100L;
            long num3 = (long.Parse(strArray[1]) * 0x100L) * 0x100L;
            long num4 = long.Parse(strArray[2]) * 0x100L;
            long num5 = long.Parse(strArray[3]);
            return (((num2 + num3) + num4) + num5);
        }

        private static int QQwry()
        {
            string pattern = @"(((\d{1,2})|(1\d{2})|(2[0-4]\d)|(25[0-5]))\.){3}((\d{1,2})|(1\d{2})|(2[0-4]\d)|(25[0-5]))";
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
            if (!regex.Match(_ip).Success)
            {
                errMsg = "IP格式错误";
                return 4;
            }
            long num = IpToInt(_ip);
            int num2 = 0;
            if ((num >= IpToInt("127.0.0.0")) && (num <= IpToInt("127.255.255.255")))
            {
                country = "本机内部环回地址";
                local = "";
                num2 = 1;
            }
            else if ((((num >= IpToInt("0.0.0.0")) && (num <= IpToInt("2.255.255.255"))) || ((num >= IpToInt("64.0.0.0")) && (num <= IpToInt("126.255.255.255")))) || ((num >= IpToInt("58.0.0.0")) && (num <= IpToInt("60.255.255.255"))))
            {
                country = "网络保留地址";
                num2 = 1;
            }
            try
            {
                objfs = new System.IO.FileStream(_dataPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            }
            catch
            {
            }
            try
            {
                objfs.Position = 0L;
                byte[] buffer = new byte[8];
                objfs.Read(buffer, 0, 8);
                firstStartIp = ((buffer[0] + (buffer[1] * 0x100)) + ((buffer[2] * 0x100) * 0x100)) + (((buffer[3] * 0x100) * 0x100) * 0x100);
                lastStartIp = ((buffer[4] + (buffer[5] * 0x100)) + ((buffer[6] * 0x100) * 0x100)) + (((buffer[7] * 0x100) * 0x100) * 0x100);
                long num3 = Convert.ToInt64((double)(((double)(lastStartIp - firstStartIp)) / 7.0));
                if (num3 <= 1L)
                {
                    country = "FileDataError";
                    objfs.Close();
                    return 2;
                }
                long num4 = num3;
                long recNO = 0L;
                long num6 = 0L;
                while (recNO < (num4 - 1L))
                {
                    num6 = (num4 + recNO) / 2L;
                    GetStartIp(num6);
                    if (num == startIp)
                    {
                        recNO = num6;
                        break;
                    }
                    if (num > startIp)
                    {
                        recNO = num6;
                    }
                    else
                    {
                        num4 = num6;
                    }
                }
                GetStartIp(recNO);
                GetEndIp();
                if ((startIp <= num) && (endIp >= num))
                {
                    GetCountry();
                    local = local.Replace("（我们一定要解放台湾！！！）", "");
                }
                else
                {
                    num2 = 3;
                    country = "未知";
                    local = "";
                }
                objfs.Close();
                return num2;
            }
            catch (Exception exception)
            {
                country = exception.Message;
                return 1;
            }
        }
    }
}
