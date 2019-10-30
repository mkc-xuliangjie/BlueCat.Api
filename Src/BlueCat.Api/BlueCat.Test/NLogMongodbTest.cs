using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using MongoDB.Driver;
using System.Linq;

namespace BlueCat.Test
{
    [TestClass]
    public class NLogMongodbTest
    {

        [TestMethod]
        public async Task Insert()
        {

            UserRepAsync userRep = new UserRepAsync();

            var a = await userRep.InsertAsync(new User() { Name = "ggg" });
            var b = await userRep.InsertAsync(new User() { Name = "BBB" });
            var c = await userRep.InsertAsync(new User() { Name = "CCC" });

            APILogRepository apilog = new APILogRepository();

            var d = await apilog.InsertAsync(new APILog() { APIName = "sdfsafd", CreateTime = DateTime.Now });

            var list = await userRep.GetListAsync(x => x.Name == "ggg");

            UserRepAsync up = new UserRepAsync();

            list = await up.GetListAsync(x => x.Name == "ggg");

            Assert.AreNotEqual(list.Count, 0);
        }
    }
}
