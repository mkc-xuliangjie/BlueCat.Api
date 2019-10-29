using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BuleCat.Common.Http.Filters
{
    /// <summary>
    /// 日志记录，用于记录 Action 执行前输入和执行后输出的数据
    /// </summary>
    [AttributeUsage((AttributeTargets.Method | AttributeTargets.Class), AllowMultiple = true)]
    public class LogAttribute : ActionFilterAttribute
    {
        private readonly DateTime _startTime = DateTime.Now;

        /// <summary>
        /// 是否记录输出的日志, 默认为 true.
        /// </summary>
        public bool IsLogOutPut { get; set; } = true;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller.GetType().GetCustomAttribute<NoLogAttribute>() != null)
            {
                return;
            }

            if (context.ActionArguments != null)
            {
                // think: how to exclude the "IFormFile" argument.
                HttpLogManager.LogInfo<LogAttribute>(context.ActionArguments);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Controller.GetType().GetCustomAttribute<NoLogAttribute>() != null)
            {
                return;
            }

            if (IsLogOutPut)
            {
                var timeSpan = (long)(DateTime.Now - _startTime).TotalMilliseconds / 1000.0;
                var requestRoute = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString.ToString();

                if (context.Result is JsonResult jsonResult)
                {
                    HttpLogManager.LogInfo<LogAttribute>(new { ElapsedTime = timeSpan, jsonResult.Value });
                }
                else if (context.Result is ObjectResult objectResult)
                {
                    HttpLogManager.LogInfo<LogAttribute>(new { ElapsedTime = timeSpan, objectResult.Value });
                }
                else if (context.Result is StatusCodeResult okResult)
                {
                    HttpLogManager.LogInfo<LogAttribute>(new { ElapsedTime = timeSpan, okResult.StatusCode });
                }
                else if (context.Result is EmptyResult)
                {
                    HttpLogManager.LogInfo<LogAttribute>(new { ElapsedTime = timeSpan });
                }
            }

            base.OnActionExecuted(context);
        }
    }



}
