﻿using BlueCat.GlobalCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
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
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public override void OnException(ExceptionContext context)
        {
            //_logger.Error("ExceptionAttribute Error,TraceId:{0},Exception:{1},Message:{2}", HttpContextGlobal.CurrentTraceId, context.Exception, context.Exception.Message);
            _logger.Error($"ExceptionAttribute Error,Exception:{context.Exception},Message:{context.Exception.Message}");
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
