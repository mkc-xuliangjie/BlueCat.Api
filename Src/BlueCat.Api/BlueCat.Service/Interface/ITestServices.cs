using BlueCat.Contract.Test;
using BuleCat.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlueCat.Service.Interface
{
    public interface ITestServices
    {

        Task<ResponseModel<TestResponse>> GetTestResponseAsync(TestRequest requestModel);
    }
}
