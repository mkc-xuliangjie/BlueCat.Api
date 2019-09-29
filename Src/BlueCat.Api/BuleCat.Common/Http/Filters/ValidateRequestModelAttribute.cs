using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace BuleCat.Common.Http.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateRequestModelAttribute : ActionFilterAttribute
    {
        private static readonly ConcurrentDictionary<Type, Type> ValidatorHub = new ConcurrentDictionary<Type, Type>();

        public virtual void ValidateCore(ActionExecutingContext context)
        {

        }
    }
}
