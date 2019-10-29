using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using MongoDB.Driver;
using System.Linq;

namespace BlueCat.Test
{
    [TestClass]
    public class MongoRepositoryTest
    {


        /// <summary>
        /// 插入
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Insert1()
        {
            UserRepAsync userRep = new UserRepAsync();

            User user = new User();
            user.Name = "aa";
            await userRep.InsertAsync(user);

            user = new User();
            user.Name = "bb";
            await userRep.InsertAsync(user);

            user = new User();
            user.Name = "cc";
            await userRep.InsertAsync(user);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <returns></returns>

        [TestMethod]
        public async Task InsertBatch()
        {
            UserRepAsync userRep = new UserRepAsync();

            List<User> userList = new List<User>();
            for (var i = 0; i < 5; i++)
            {
                User user = new User();
                user.Name = new Random().Next().ToString();
                userList.Add(user);
            }

            await userRep.InsertBatchAsync(userList);
        }

        /// <summary>
        /// 修改单条数据
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UpdateOne()
        {
            UserRepAsync userRep = new UserRepAsync();

            await userRep.UpdateOneAsync(x => x.ID == 4, UserRepAsync.Update.Set(nameof(User.CreateTime), DateTime.Now));

            await userRep.UpdateOneAsync(x => x.Name == "bb", UserRepAsync.Update.Set(nameof(User.CreateTime), DateTime.Now));

            //获取当前自增ID
            long id = await userRep.CreateIncIDAsync();

            var update = UserRepAsync.Update.Set(nameof(User.Name), "xyz");

            update = update.SetOnInsert(x => x.ID, id).SetOnInsert(x => x.CreateTime, DateTime.Now);

            await userRep.UpdateOneAsync(x => x.Name == "abc", update, true);

            var res = await userRep.UpdateOneAsync(x => x.Name == "xyz", update, true, WriteConcern.Acknowledged);

            Assert.AreEqual(res.IsAcknowledged, true);
        }

        /// <summary>
        /// 查找对象，然后更新对象
        /// </summary>
        /// <returns></returns>

        [TestMethod]
        public async Task UpdateOne_updateEntity()
        {
            User user = new User();
            UserRepAsync userRep = new UserRepAsync();

            //新增
            user = await userRep.GetAsync(x => x.ID == 1);
            user.Age += 1;
            user.CreateTime = DateTime.Now;
            user.Name = "axxxx";

            await userRep.UpdateOneAsync(x => x.Name == "axxxx", user, false);



            //根据条件查询，然后修改
            user = await userRep.GetAsync(x => x.Name == "axxxx");
            user.Age = 12;
            await userRep.UpdateOneAsync(x => x.ID == user.ID, user, true);

        }

        /// <summary>
        /// 更新一个对象
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task FindOneAndUpdate()
        {
            User user;
            UserRepAsync userRep = new UserRepAsync();
            user = await userRep.GetAsync(2);
            user.Age += 1;
            user.CreateTime = DateTime.Now;

            user = await userRep.FindOneAndUpdateAsync(filterExp: x => x.ID == user.ID, updateEntity: user, isUpsert: false);
        }


        /// <summary>
        /// 修改多条数据
        /// </summary>
        /// <returns></returns>

        [TestMethod]
        public async Task UpdateMany()
        {
            UserRepAsync userRep = new UserRepAsync();

            var update = UserRepAsync.Update.Set(nameof(User.CreateTime), DateTime.Now);

            await userRep.UpdateManyAsync(x => x.Name == "cc", update);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Get()
        {
            UserRepAsync userRep = new UserRepAsync();
            List<User> list = await userRep.GetListAsync(limit: 10);

            User user = list.First();
            long id = user.ID;

            user = await userRep.GetAsync(user.ID);
            Assert.AreEqual(user.ID, id);

            user = await userRep.GetAsync(x => x.Name == "aa");
            Assert.AreNotEqual(user, null);
            user = await userRep.GetAsync(x => x.Name == "aa", x => new { x.Name });

            Assert.AreNotEqual(user, null);
            user = await userRep.GetAsync(x => x.Name == "aa", x => new { x.CreateTime });
            Assert.AreNotEqual(user, null);

            user = await userRep.GetAsync(x => x.Name == "aa" && x.CreateTime > DateTime.Parse("2015/10/20"));
            Assert.AreNotEqual(user, null);
            Builders<User>.Filter.Eq("Name", "aa");

            var filter = UserRepAsync.Filter.Eq(x => x.Name, "aa") & UserRepAsync.Filter.Eq(x => x.ID, 123);
            UserRepAsync.Sort.Descending("_id");

            user = await userRep.GetAsync(Builders<User>.Filter.Eq("Name", "aa"), null, Builders<User>.Sort.Descending("_id"));
            Assert.AreNotEqual(user, null);

            user = await userRep.GetAsync(filter: Builders<User>.Filter.Eq("Name", "aa"), projection: Builders<User>.Projection.Include(x => x.Name));
            Assert.AreNotEqual(user, null);

        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Paging()
        {
            UserRepAsync userRep = new UserRepAsync();
            List<User> list = await userRep.GetListAsync(limit: 10, skip: 10);
            User user = list.First();
            long id = user.ID;
            user = await userRep.GetAsync(user.ID);
            Assert.AreEqual(user.ID, id);

        }



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
