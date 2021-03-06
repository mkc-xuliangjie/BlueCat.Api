﻿using BlueCat.Contract.Test;
using BlueCat.GlobalCore;
using BlueCat.MySqlRepository.Interface;
using BlueCat.Repository;
using BlueCat.Service.Interface;
using BuleCat.Common;
using BuleCat.Common.Http;
using NLog;
using System.Threading.Tasks;

namespace BlueCat.Service.Impl
{
    public class TestService : ITestServices
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly IUserRepository userRepository;

        private readonly ITestRepository testRepository;
        public TestService(IUserRepository iuserRepository, ITestRepository itestRepository)
        {
            userRepository = iuserRepository;
            testRepository = itestRepository;
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
            //logger.Info("GetCityServicePreInfoAsync_Repository[Begin],requestId:{requestId},account:{account},param:{@param}", requestId, account, param);
            //_logger.Info("funtion name:GetUserCountAsync,TraceId:{requestId}", HttpContextGlobal.CurrentTraceId);

            _logger.Info("funtion name:GetUserCountAsync");

            _logger.Info(testRepository.Get());

            return await userRepository.GetUserCountAsync();
        }
    }
}
