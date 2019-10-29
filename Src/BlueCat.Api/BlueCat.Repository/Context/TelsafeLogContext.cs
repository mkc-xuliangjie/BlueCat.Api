using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.Repository.Context
{
    public class TelsafeLogContext : MongodbContext
    {
        public TelsafeLogContext(IServiceProvider provider) : base(provider, "MongoDB:APILogs")
        {

        }
    }
}
