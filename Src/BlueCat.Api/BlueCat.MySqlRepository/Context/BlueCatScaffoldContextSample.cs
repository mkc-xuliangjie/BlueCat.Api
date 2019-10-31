using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.MySqlRepository.Context
{
    public class BlueCatScaffoldContextSample: BaseContext
    {
        public BlueCatScaffoldContextSample(IServiceProvider provider) : base(provider, "Mysql:DBString", "Mysql:DBType")
        {

        }
    }
}
