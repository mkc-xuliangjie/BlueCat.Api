using Microsoft.AspNetCore.Http;

namespace BuleCat.Common.Http
{
    /// <summary>
    /// <see cref="HttpContext"/> 全局对象
    /// </summary>
    public static class HttpContextGlobal
    {
        static IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// 配置 HttpContext 访问者对象
        /// </summary>
        /// <param name="accessor"></param>
        public static void Configure(IHttpContextAccessor accessor)
        {
            _httpContextAccessor = accessor;
        }

        /// <summary>
        /// 获取当前的 <see cref="HttpContext"/> 对象
        /// </summary>
        public static HttpContext Current => _httpContextAccessor?.HttpContext;

        /// <summary>
        /// 获取当前请求的 TraceID 标识
        /// </summary>
        public static string CurrentTraceId
        {
            get
            {
                if (Current != null)
                {
                    if (Current.Request.Headers.TryGetValue(Constant.RESTfulTraceId, out Microsoft.Extensions.Primitives.StringValues value))
                    {
                        return value.ToString();
                    }
                }

                return null;
            }
        }
    }
}
