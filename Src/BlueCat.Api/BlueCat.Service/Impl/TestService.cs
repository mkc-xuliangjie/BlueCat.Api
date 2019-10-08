using BlueCat.Contract.Test;
using BlueCat.Service.Interface;
using BuleCat.Common;
using BuleCat.Common.Http;
using System.Threading.Tasks;

namespace BlueCat.Service.Impl
{
    public class TestService : ITestServices
    {
        public async Task<ResponseModel<TestResponse>> GetTestResponseAsync(TestRequest requestModel)
        {
            string requestId = HttpContextGlobal.CurrentTraceId;
            var response = new ResponseModel<TestResponse>();

            TestResponse testResponse = new TestResponse();
            testResponse.ResponseContent = "111";

            response.ResultData = testResponse;

            return response;
        }
    }
}
