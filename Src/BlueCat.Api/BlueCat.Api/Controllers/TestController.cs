using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueCat.Contract.Test;
using BuleCat.Common;
using BuleCat.Common.DependencyInjection;
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
        public TestController(IOptions<AppSettings> options)
        {
            appSettings = options.Value;
        }

        [HttpGet("v1/test")]
        [ValidateRequestModel]
        public async Task<ResponseModel<TestResponse>> GetTestResponseAsync([FromQuery]RequestModel<TestRequest> requestModel)
        {
            ResponseModel<TestResponse> responseModel = new ResponseModel<TestResponse>();

            TestResponse testResponse = new TestResponse();

            testResponse.ResponseContent = appSettings.Test;

            responseModel.ResultData = testResponse;

            return  responseModel;  
        }


        [HttpGet("v1/test1")]
        //[ValidateRequestModel]
        public string GetTest1Response()
        {
            return "11";
        }
    }
}