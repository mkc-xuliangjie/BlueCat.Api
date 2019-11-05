using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueCat.Contract.Test;
using BlueCat.GlobalCore;
using BlueCat.Service.Interface;
using BuleCat.Common;
using BuleCat.Common.DependencyInjection;
using BuleCat.Common.Http;
using BuleCat.Common.Http.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StackExchangeRedis;


namespace BlueCat.Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        private readonly AppSettings appSettings;

        private readonly ITestServices testServices;

        //private readonly IDistributedCache distributedCache;

        //public TestController(ITestServices itestServices, IOptions<AppSettings> options,  IDistributedCache distributedCache)
        //{
        //    appSettings = options.Value;

        //    testServices = itestServices;

        //    this.distributedCache = distributedCache;
        //}

        //private readonly IRedisCache redisCache;

        // public TestController(ITestServices itestServices, IOptions<AppSettings> options, IRedisCache iredisCache)
        // {
        //     appSettings = options.Value;

        //     testServices = itestServices;

        //     redisCache = iredisCache;
        // }

        public TestController(ITestServices itestServices, IOptions<AppSettings> options)
        {
            appSettings = options.Value;

            testServices = itestServices;
        }


        [HttpGet("v1/test")]
        [ValidateRequestModel]
        public async Task<ResponseModel<TestResponse>> GetTestResponseAsync([FromQuery]RequestModel<TestRequest> requestModel)
        {

            return await testServices.GetTestResponseAsync(requestModel.BusinessData);
        }


        [HttpGet("v1/test1")]
        //[ValidateRequestModel]
        public async Task<string> GetTest1Response()
        {
            //var value = distributedCache.Get("name-key");
            //string valStr = string.Empty;
            //if (value == null)
            //{
            //    valStr = "孙悟空三打白骨精！";
            //    // 存储的数据必须为字节，所以需要转换一下
            //    var encoded = Encoding.UTF8.GetBytes(valStr);
            //    // 配置类：30秒过时
            //    var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
            //    distributedCache.Set("name-key", encoded, options);

            //    distributedCache.SetString("test", "111111");
            //}

            
            var value = RedisHelper.Get("name-key");
            string valStr = string.Empty;
            if (value == null)
            {
                valStr = "孙悟空三打白骨精！";

                RedisHelper.Set("name-key", valStr, 30);
            }


            await testServices.GetUserCountAsync();
            return HttpContextGlobal.CurrentTraceId.ToString();
        }
    }
}