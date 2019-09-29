using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuleCat.Common.Http.Filters
{
    /// <summary>
    /// 异常筛选器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExceptionAttribute : ExceptionFilterAttribute
    {
       // private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public override void OnException(ExceptionContext context)
        {
            //_logger.Error("ExceptionAttribute Error,Exception:{0},Message:{1}", context.Exception, context.Exception.Message);
            ExceptionHandle(context);
        }

        /// <summary>
        /// 处理异常
        /// </summary>
        public virtual void ExceptionHandle(ExceptionContext context)
        {

            context.Result = new OkObjectResult(
                      ResponseModel.Failure("4001", "服务器错误"));
        }
    }
}
