namespace BuleCat.Common.Http
{
    /// <summary>
    /// 用于常量标记
    /// </summary>
    public static class Constant
    {
        /// <summary>
        /// 用于 HTTP 请求日志跟踪记录
        /// </summary>
        public const string RESTfulTraceId = "TraceId";

        /// <summary>
        /// 用于记录请求时间
        /// </summary>
        public const string RESTfulTraceTime = "X-Trace-Time";
    }
}
