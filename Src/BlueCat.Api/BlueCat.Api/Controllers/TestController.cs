using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueCat.Contract.Test;
using BlueCat.Service.Interface;
using BuleCat.Common;
using BuleCat.Common.DependencyInjection;
using BuleCat.Common.Http;
using BuleCat.Common.Http.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BlueCat.Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        private readonly AppSettings appSettings;

        private readonly ITestServices testServices;

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
            await testServices.GetUserCountAsync();
            return HttpContextGlobal.CurrentTraceId.ToString();
        }
    }
}