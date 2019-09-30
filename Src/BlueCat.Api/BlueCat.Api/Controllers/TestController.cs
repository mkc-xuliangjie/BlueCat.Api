using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueCat.Contract.Test;
using BuleCat.Common;
using BuleCat.Common.Http.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlueCat.Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public TestController()
        { }

        [HttpGet("v1/test")]
        [ValidateRequestModel]
        public ResponseModel<TestResponse> GetTestResponse([FromQuery]RequestModel<TestRequest> requestModel)
        {
            ResponseModel<TestResponse> responseModel = new ResponseModel<TestResponse>();

            TestResponse testResponse = new TestResponse();

            testResponse.ResponseContent = requestModel.BusinessData.RequestContent;

            responseModel.ResultData = testResponse;

            return responseModel;  
        }


        [HttpGet("v1/test1")]
        //[ValidateRequestModel]
        public string GetTest1Response()
        {
            return "11";
        }
    }
}