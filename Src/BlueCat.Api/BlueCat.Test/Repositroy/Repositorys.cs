
using MongoDB.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.Test
{
    public class Repositorys
    {
        public static string dbName = "RepositoryTest";
        public static string connString = "mongodb://127.0.0.1:27017/";
    }

    public class UserRepAsync : MongoRepositoryAsync<User, long>
    {
        public UserRepAsync()
            : base(Repositorys.connString, Repositorys.dbName, null, null)
        {

        }
    }

    public class APILogRepository : MongoRepositoryAsync<APILog, string>
    {
        public APILogRepository() :
            base(Repositorys.connString, Repositorys.dbName, null, null)
        {

        }
    }
}
