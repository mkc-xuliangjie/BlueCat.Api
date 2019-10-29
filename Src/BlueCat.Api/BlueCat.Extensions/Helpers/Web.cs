using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Internal;

namespace BlueCat.Extensions.Helpers
{
    /// <summary>
    /// Web操作
    /// </summary>
    public static class Web
    {

        #region 初始化

        /// <summary>
        /// 
        /// </summary>
        public static IServiceProvider ServiceProvider;

        #endregion

        #region 属性

        /// <summary>
        /// 当前Http上下文
        /// 使用前需要在【Startup.cs】文件中进行两步操作：
        /// 1：在【ConfigureServices】方法中加入【services.TryAddSingleton（尖括号）IHttpContextAccessor,HttpContextAccessor（尖括号）();】
        /// 2：【Configure】方法的参数中加入一个新的参数【IServiceProvider svp】，并且在方法中加入【BlueCat.Extensions.Helpers.Web.HttpContext.ServiceProvider = svp;】这行代码；
        /// 
        /// </summary>
        public static HttpContext HttpContext
        {
            get
            {
                object factory = ServiceProvider.GetService(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));
                HttpContext context = ((IHttpContextAccessor)factory).HttpContext;
                return context;
            }
        }

        /// <summary>
        /// 当前Http请求
        /// </summary>
        public static HttpRequest Request => HttpContext?.Request;

        /// <summary>
        /// 当前Http响应
        /// </summary>
        public static HttpResponse Response => HttpContext?.Response;

        #endregion

        #region Body(请求正文)

        /// <summary>
        /// 请求正文
        /// </summary>
        public static string Body
        {
            get
            {
                Request.EnableRewind();
                return File.ToString(Request.Body, isCloseStream: false);
            }
        }

        #endregion

        #region GetBodyAsync(获取请求正文)

        /// <summary>
        /// 获取请求正文
        /// </summary>
        public static async Task<string> GetBodyAsync()
        {
            Request.EnableRewind();
            return await File.ToStringAsync(Request.Body, isCloseStream: false);
        }

        #endregion

        #region Client( Web客户端 )

        /// <summary>
        /// Web客户端，用于发送Http请求
        /// </summary>
        public static BlueCat.Extensions.Webs.Clients.WebClient Client()
        {
            return new BlueCat.Extensions.Webs.Clients.WebClient();
        }

        /// <summary>
        /// Web客户端，用于发送Http请求
        /// </summary>
        /// <typeparam name="TResult">返回结果类型</typeparam>
        public static BlueCat.Extensions.Webs.Clients.WebClient<TResult> Client<TResult>() where TResult : class
        {
            return new BlueCat.Extensions.Webs.Clients.WebClient<TResult>();
        }

        #endregion

        #region Url(请求地址)

        /// <summary>
        /// 请求地址
        /// </summary>
        public static string Url => HttpContext?.Request?.GetDisplayUrl();

        #endregion

        #region IP操作

        #region Ip(客户端Ip地址)

        /// <summary>
        /// 客户端Ip地址
        /// </summary>
        public static string Ip
        {
            get
            {
                var list = new[] { "127.0.0.1", "::1" };
                var result = HttpContext?.Connection?.RemoteIpAddress.SafeString();
                if (string.IsNullOrWhiteSpace(result) || list.Contains(result))
                    result = GetLanIp();
                return result;
            }
        }

        /// <summary>
        /// 获取局域网IP
        /// </summary>
        private static string GetLanIp()
        {
            foreach (var hostAddress in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                    return hostAddress.ToString();
            }
            return string.Empty;
        }

        #endregion

        #region 将一个IP字符串转为Long型

        /// <summary>
        /// 将一个IP字符串转为Long型
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns></returns>
        public static long IPConvertLong(string ip)
        {
            //从大到小排列ip,与IPAddress不同
            if (string.IsNullOrEmpty(ip) || ip == "::1")
            {
                return 0;
            }
            string[] ipsegmentArray = ip.Split('.');
            if (ipsegmentArray.Length != 4)
            {
                ipsegmentArray = "127.0.0.1".Split();
            }
            long ip1 = int.Parse(ipsegmentArray[0]);
            int ip2 = int.Parse(ipsegmentArray[1]);
            int ip3 = int.Parse(ipsegmentArray[2]);
            int ip4 = int.Parse(ipsegmentArray[3]);
            long ip0 = (ip1 << 24) + (ip2 << 16) + (ip3 << 8) + ip4;
            return ip0;
        }

        #endregion

        #region 将一个Long型的ip转成字符串

        /// <summary>
        /// 将一个Long型的ip转成字符串
        /// </summary>
        /// <returns></returns>
        public static string LongConvertIP(long ip)
        {
            long ip1 = ip >> 24 & 0xFF;
            long ip2 = ip >> 16 & 0xFF;
            long ip3 = ip >> 8 & 0xFF;
            long ip4 = ip & 0xFF;
            var ipstr = ip1 + "." + ip2 + "." + ip3 + "." + ip4;
            return ipstr;
        }

        #endregion 

        #region 判断是否是IP地址格式 0.0.0.0  

        /// <summary>  
        /// 判断是否是IP地址格式 0.0.0.0  
        /// </summary>  
        /// <param name="str1">待判断的IP地址</param>  
        /// <returns>true or false</returns>  
        public static bool IsIPAddress(string str1)
        {
            if (str1 == null || str1 == string.Empty || str1.Length < 7 || str1.Length > 15) return false;
            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str1);
        }

        #endregion 

        #endregion

        #region Host(主机)

        /// <summary>
        /// 主机
        /// </summary>
        public static string Host => HttpContext == null ? Dns.GetHostName() : GetClientHostName();

        /// <summary>
        /// 获取Web客户端主机名
        /// </summary>
        private static string GetClientHostName()
        {
            var address = GetRemoteAddress();
            if (string.IsNullOrWhiteSpace(address))
                return Dns.GetHostName();
            var result = Dns.GetHostEntry(IPAddress.Parse(address)).HostName;
            if (result == "localhost.localdomain")
                result = Dns.GetHostName();
            return result;
        }

        /// <summary>
        /// 获取远程地址
        /// </summary>
        private static string GetRemoteAddress()
        {
            return HttpContext?.Request?.Headers["HTTP_X_FORWARDED_FOR"] ?? HttpContext?.Request?.Headers["REMOTE_ADDR"];
        }

        #endregion

        #region Browser(浏览器)

        /// <summary>
        /// 浏览器
        /// </summary>
        public static string Browser => HttpContext?.Request?.Headers["User-Agent"];

        #endregion

        #region GetFiles(获取客户端文件集合)

        /// <summary>
        /// 获取客户端文件集合
        /// </summary>
        public static List<IFormFile> GetFiles()
        {
            var result = new List<IFormFile>();
            var files = HttpContext.Request.Form.Files;
            if (files == null || files.Count == 0)
                return result;
            result.AddRange(files.Where(file => file?.Length > 0));
            return result;
        }

        #endregion

        #region GetFile(获取客户端文件)

        /// <summary>
        /// 获取客户端文件
        /// </summary>
        public static IFormFile GetFile()
        {
            var files = GetFiles();
            return files.Count == 0 ? null : files[0];
        }

        #endregion

        #region URL操作

        #region URL添加参数

        /// <summary>
        /// URL添加参数
        /// </summary>
        /// <param name="originUrl"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string AddUrlParams(string originUrl, IDictionary<string, string> data)
        {
            var baseUrl = string.Empty;
            var paramsStr = string.Empty;
            var baseData = GetUrlParams(originUrl, out baseUrl);

            foreach (var d in data)
            {
                baseData[d.Key] = d.Value;
            }

            baseUrl += "?";
            foreach (var d in baseData)
            {
                baseUrl += string.Format("{0}={1}&", d.Key, d.Value);
            }
            return baseUrl.Substring(0, baseUrl.Length - 1);
        }

        #endregion

        #region 获取Url参数

        /// <summary>
        /// 获取Url参数
        /// </summary>
        /// <param name="originUrl"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public static IDictionary<string, string> GetUrlParams(string originUrl, out string baseUrl)
        {
            baseUrl = string.Empty;
            var paramsStr = string.Empty;
            if (originUrl.IndexOf("?") >= 0)
            {
                var temp = originUrl.Split('?');
                if (temp.Length > 1)
                {
                    paramsStr = temp[1];
                }
                baseUrl = temp[0];
            }
            else
            {
                baseUrl = originUrl;
            }

            var _data = new Dictionary<string, string>();
            string[] paramsArray;
            if (!string.IsNullOrWhiteSpace(paramsStr))
            {
                paramsArray = paramsStr.Split('&');
                for (var i = 0; i < paramsArray.Length; i++)
                {
                    var _temp = paramsArray[i].Split('=');
                    if (_temp.Length == 2)
                    {
                        _data[_temp[0]] = _temp[1];
                    }
                }
            }

            return _data;
        }

        #endregion

        #endregion

        #region UrlEncode(Url编码)

        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="isUpper">编码字符是否转成大写,范例,"http://"转成"http%3A%2F%2F"</param>
        public static string UrlEncode(string url, bool isUpper = false)
        {
            return UrlEncode(url, Encoding.UTF8, isUpper);
        }

        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="isUpper">编码字符是否转成大写,范例,"http://"转成"http%3A%2F%2F"</param>
        public static string UrlEncode(string url, string encoding, bool isUpper = false)
        {
            encoding = string.IsNullOrWhiteSpace(encoding) ? "UTF-8" : encoding;
            return UrlEncode(url, Encoding.GetEncoding(encoding), isUpper);
        }

        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="isUpper">编码字符是否转成大写,范例,"http://"转成"http%3A%2F%2F"</param>
        public static string UrlEncode(string url, Encoding encoding, bool isUpper = false)
        {
            var result = HttpUtility.UrlEncode(url, encoding);
            if (isUpper == false)
                return result;
            return GetUpperEncode(result);
        }

        /// <summary>
        /// 获取大写编码字符串
        /// </summary>
        private static string GetUpperEncode(string encode)
        {
            var result = new StringBuilder();
            int index = int.MinValue;
            for (int i = 0; i < encode.Length; i++)
            {
                string character = encode[i].ToString();
                if (character == "%")
                    index = i;
                if (i - index == 1 || i - index == 2)
                    character = character.ToUpper();
                result.Append(character);
            }
            return result.ToString();
        }

        #endregion

        #region UrlDecode(Url解码)

        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="url">url</param>
        public static string UrlDecode(string url)
        {
            return HttpUtility.UrlDecode(url);
        }

        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">字符编码</param>
        public static string UrlDecode(string url, Encoding encoding)
        {
            return HttpUtility.UrlDecode(url, encoding);
        }

        #endregion

        #region DownloadAsync(下载)

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <param name="fileName">文件名,包含扩展名</param>
        public static async Task DownloadFileAsync(string filePath, string fileName)
        {
            await DownloadFileAsync(filePath, fileName, Encoding.UTF8);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <param name="fileName">文件名,包含扩展名</param>
        /// <param name="encoding">字符编码</param>
        public static async Task DownloadFileAsync(string filePath, string fileName, Encoding encoding)
        {
            var bytes = File.Read(filePath);
            await DownloadAsync(bytes, fileName, encoding);
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="fileName">文件名,包含扩展名</param>
        public static async Task DownloadAsync(Stream stream, string fileName)
        {
            await DownloadAsync(stream, fileName, Encoding.UTF8);
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="fileName">文件名,包含扩展名</param>
        /// <param name="encoding">字符编码</param>
        public static async Task DownloadAsync(Stream stream, string fileName, Encoding encoding)
        {
            await DownloadAsync(File.ToBytes(stream), fileName, encoding);
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="bytes">字节流</param>
        /// <param name="fileName">文件名,包含扩展名</param>
        public static async Task DownloadAsync(byte[] bytes, string fileName)
        {
            await DownloadAsync(bytes, fileName, Encoding.UTF8);
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="bytes">字节流</param>
        /// <param name="fileName">文件名,包含扩展名</param>
        /// <param name="encoding">字符编码</param>
        public static async Task DownloadAsync(byte[] bytes, string fileName, Encoding encoding)
        {
            if (bytes == null || bytes.Length == 0)
                return;
            fileName = fileName.Replace(" ", "");
            fileName = UrlEncode(fileName, encoding);
            Response.ContentType = "application/octet-stream";
            Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
            await Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        #endregion
    }
}
