using BuleCat.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace BuleCat.Common.Http.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateRequestModelAttribute : ActionFilterAttribute
    {
        private static readonly ConcurrentDictionary<Type, Type> ValidatorHub = new ConcurrentDictionary<Type, Type>();


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            this.ValidateCore(context);
        }

        public virtual void ValidateCore(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.SelectMany(m => m.Value.Errors.Select(e => e.ErrorMessage));
                var errMsg = $"Invalid arguments. {string.Join("", errors)}";
                InvalidValidate(errMsg, context);
                return;
            }

            // validate the params must be not null, then expose "BIZData" and "Account"
            string userName = context.HttpContext.User.Identity.IsAuthenticated ? context.HttpContext.User.Identity.Name : null;
            var parameters = context.ActionDescriptor.Parameters.Where(p => p.ParameterType.IsClass && !p.ParameterType.IsPrimitive && typeof(RequestModel).IsAssignableFrom(p.ParameterType));
            foreach (var p in parameters)
            {
                if (!context.ActionArguments.ContainsKey(p.Name))
                {
                    var errMsg = $"Argument '{p.Name}' must be not null.";
                    InvalidValidate(errMsg, context);
                    break;
                }

                var requestModelObj = context.ActionArguments[p.Name];
                var requestModel = requestModelObj as RequestModel;
                var requestModelType = requestModelObj.GetType().GetTypeInfo();
                if (requestModelType.BaseType != null)
                {
                    var bizProperty = requestModelType.GetProperty("BusinessData");
                    if (bizProperty != null)
                    {
                        var (ok, obj) = ResolveData(requestModel.Data, bizProperty.PropertyType);
                        if (ok)
                        {
                            // 验证 BusinessData 对象
                            var (valid, err) = ValidateBizData(obj);
                            if (!valid)
                            {
                                InvalidValidate(err, context);
                                return;
                            }

                            bizProperty.SetValue(requestModelObj, obj);
                        }
                    }
                }

                requestModel.Account = userName;
            }
        }

        /// <summary>
        /// 解析数据，可重写
        /// </summary>
        /// <param name="data">要解析的字符串</param>
        /// <param name="resolveType">要解析的对象类型</param>
        /// <returns></returns>
        public virtual (bool, object) ResolveData(string data, Type resolveType)
        {
            return (true, data.AsObject(resolveType));
        }

        /// <summary>
        /// 无效的验证处理, 可重写
        /// </summary>
        /// <param name="errorMessage">错误消息描述</param>
        /// <param name="context">Action 执行时的上下文</param>
        public virtual void InvalidValidate(string errorMessage, ActionExecutingContext context)
        {
            context.Result = new OkObjectResult(ResponseModel.Failure("4401", errorMessage));
        }

        /// <summary>
        ///  验证 BusinessData 对象
        /// </summary>
        /// <param name="bizObj">要验证的业务对象</param>
        /// <returns></returns>
        public virtual (bool, string) ValidateBizData(object bizObj)
        {
            var bizType = bizObj.GetType();
            if (!ValidatorHub.TryGetValue(bizType, out Type validatorType))
            {
                validatorType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                                         .FirstOrDefault(t => t.BaseType != null && t.BaseType.IsGenericType
                                                && t.BaseType.GetGenericTypeDefinition() == typeof(FluentValidation.AbstractValidator<>)
                                                && t.BaseType.GetGenericArguments()[0] == bizType);

                if (validatorType != null)
                {
                    ValidatorHub.TryAdd(bizType, validatorType);
                }
            }

            if (validatorType != null)
            {
                var validator = (FluentValidation.IValidator)Activator.CreateInstance(validatorType);
                var validateResult = validator.Validate(bizObj);
                if (!validateResult.IsValid)
                {
                    return (false, validateResult.ToString());
                }
            }

            return (true, null);
        }
    }
}
