using System;
using System.Collections.Generic;
using System.Text;

namespace BuleCat.Common
{
    /// <summary>
    /// 基础返回结果(返回数据)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseModel<T> : ResponseModel
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public T ResultData { get; set; }

        public static ResponseModel<T> Success(T data)
        {
            return new ResponseModel<T>
            {
                ResultData = data
            };
        }

        public new static ResponseModel<T> Failure(string code, string desc)
        {
            return new ResponseModel<T>
            {
                ResultCode = code,
                ResultDesc = desc
            };
        }
    }


    /// <summary>
    /// 基础返回结果(不返回数据)
    /// </summary>
    public class ResponseModel
    {
        /// <summary>
        /// 请求id，每个请求唯一
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// 返回状态编码
        /// </summary>
        public string ResultCode { get; set; }

        /// <summary>
        /// 返回描述
        /// </summary>
        public string ResultDesc { get; set; }

        /// <summary>
        /// 服务器时间
        /// </summary>
        public long ServerTime { get; set; }

        /// <summary>
        /// 返回提示
        /// </summary>
        //public string Message { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ResponseModel()
        {
            RequestId = HttpContextGlobal.CurrentTraceId;
            ResultCode = "0000";
            ResultDesc = "success";
            ServerTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }

        public static ResponseModel Failure(string code, string desc)
        {
            return new ResponseModel
            {
                ResultCode = code,
                ResultDesc = desc
            };
        }
    }
}
