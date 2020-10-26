using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;



namespace JD_seckill
{
    public class WebUtil
    {

        public static string DoMain = "https://www.jd.com/";

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref System.UInt32 pcchCookieData, int dwFlags, IntPtr lpReserved);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int InternetSetCookieEx(string lpszURL, string lpszCookieName, string lpszCookieData, int dwFlags, IntPtr dwReserved);

        public static string GetCookies(string url)
        {
            uint datasize = 256;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x2000, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;


                cookieData = new StringBuilder((int)datasize);
                if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00002000, IntPtr.Zero))
                    return null;
            }
            return cookieData.ToString();
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public static string GetCookies()
        {
            return GetCookies(DoMain);
        }

        public string Get(string url, string jdcoock)
        {
            ///如果接口不需要参数，这URL最后无需“？” 符号
            string resultdata = string.Empty;
            string errormsg = string.Empty;
            int? error_code = null;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {

                request.CookieContainer = new CookieContainer();
                request.CookieContainer.PerDomainCapacity = 40;
                string[] cookie = jdcoock.Split(';');
                foreach (var val in cookie)
                {
                    string[] item = val.Split('=');
                    var cook = new Cookie(item[0].Trim(), item[1].Trim(), "/", ".jd.com");
                    request.CookieContainer.Add(cook);
                }
                request.Headers.Add("accept-encoding", "gzip, deflate, br");
                request.Headers.Add("accept-language", "zh-CN,zh;q=0.9,sn;q=0.8,en;q=0.7");
                request.Headers.Add("cache-control", "max-age=0");
                request.Headers.Add("upgrade-insecure-requests", "1");
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return getResponseBody(response);
            }
            catch (System.Net.WebException ex)
            {
                HttpWebResponse res = (ex.Response as HttpWebResponse);
                if (res != null)
                {
                    error_code = (int)res.StatusCode;
                    switch (error_code)
                    {
                        case 401:
                            errormsg = "签名错误";
                            break;
                        default:
                            errormsg = $"服务器请求错误  {error_code}：" + ex.Message;
                            break;
                    }
                }
                else
                {
                    errormsg = "服务器请求错误：" + ex.Message;
                }
                log.Error("获取服务器数据的接口出错：" + url + Environment.NewLine + ex.ToString());
                resultdata = "{\"ret\":false,\"error_msg\":\"" + errormsg + "\",\"error_code\":\"" + error_code?.ToString() + "\"}";
                log.Info($"result:{resultdata}");
            }
            catch (Exception ex)
            {
                errormsg = "服务器请求错误：" + ex.Message;
                log.Error("获取服务器数据的接口出错：" + url + Environment.NewLine + ex.Message);
                resultdata = "{\"ret\":false,\"error_msg\":\"" + errormsg + "\",\"error_code\":\"" + error_code?.ToString() + "\"}";
            }
            return resultdata;
        }

        /// <summary>
        /// 返回body内容
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private string getResponseBody(HttpWebResponse response)
        {
            Encoding defaultEncode = Encoding.UTF8;
            string contentType = response.ContentType;
            if (contentType != null)
            {
                if (contentType.ToLower().Contains("gb2312"))
                {
                    defaultEncode = Encoding.GetEncoding("gb2312");
                }
                else if (contentType.ToLower().Contains("gbk"))
                {
                    defaultEncode = Encoding.GetEncoding("gb2312");
                }
                else if (contentType.ToLower().Contains("zh-cn"))
                {
                    defaultEncode = Encoding.GetEncoding("zh-cn");
                }
            }

            string responseBody = string.Empty;
            if (response.ContentEncoding.ToLower().Contains("gzip"))
            {
                using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream, defaultEncode))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            else if (response.ContentEncoding.ToLower().Contains("deflate"))
            {
                using (DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream, defaultEncode))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }

            }
            else if (response.ContentEncoding.ToLower().Contains("br"))
            {
                
            }
            else
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, defaultEncode))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            return responseBody;
        }


        public static string UrlEncode(string item, Encoding code)
        {
            return System.Web.HttpUtility.UrlEncode(item.Trim('\t').Trim(), Encoding.GetEncoding("gb2312"));
        }

        public static string UrlEncodeByGB2312(string item)
        {
            return UrlEncode(item, Encoding.GetEncoding("gb2312"));
        }


        public static string UrlEncodeByUTF8(string item)
        {
            return UrlEncode(item, Encoding.GetEncoding("utf-8"));
        }

        public static string HtmlDecode(string item)
        {
            return WebUtility.HtmlDecode(item.Trim('\t').Trim());
        }


    }

}
