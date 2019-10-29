using BlueCat.Contract.Test;
using BlueCat.Repository;
using BlueCat.Service.Interface;
using BuleCat.Common;
using BuleCat.Common.Http;
using System.Threading.Tasks;

namespace BlueCat.Service.Impl
{
    public class TestService : ITestServices
    {
        private readonly IUserRepository userRepository;
        public TestService(IUserRepository iuserRepository)
        {
            userRepository = iuserRepository;
        }

        public async Task<ResponseModel<TestResponse>> GetTestResponseAsync(TestRequest requestModel)
        {
            string requestId = HttpContextGlobal.CurrentTraceId;
            var response = new ResponseModel<TestResponse>();

            TestResponse testResponse = new TestResponse();
            testResponse.ResponseContent = "111";

            long count = await userRepository.GetUserCountAsync();

            testResponse.ResponseContent = count.ToString();

            response.ResultData = testResponse;

            return response;
        }

        public async Task<long> GetUserCountAsync()
        {
            return await userRepository.GetUserCountAsync();
        }
    }
}
