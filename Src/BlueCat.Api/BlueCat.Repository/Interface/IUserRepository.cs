using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlueCat.Repository
{
    public interface IUserRepository
    {
       Task<long> GetUserCountAsync();
    }
}
