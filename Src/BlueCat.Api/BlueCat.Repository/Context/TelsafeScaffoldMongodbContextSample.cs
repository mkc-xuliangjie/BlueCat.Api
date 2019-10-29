using BlueCat.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.Repository
{
    public class TelsafeScaffoldMongodbContextSample : MongodbContext
    {
        public TelsafeScaffoldMongodbContextSample(IServiceProvider provider) : base(provider, "MongoDB:RepositoryTest")
        {

        }
    }
}
