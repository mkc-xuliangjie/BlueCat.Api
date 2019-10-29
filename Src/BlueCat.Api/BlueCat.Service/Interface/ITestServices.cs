using BlueCat.Contract.Test;
using BuleCat.Common;
using System.Threading.Tasks;

namespace BlueCat.Service.Interface
{
    public interface ITestServices
    {
        Task<ResponseModel<TestResponse>> GetTestResponseAsync(TestRequest requestModel);

        Task<long> GetUserCountAsync();
    }
}
