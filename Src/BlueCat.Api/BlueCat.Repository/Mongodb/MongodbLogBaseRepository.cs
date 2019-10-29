using MongoDB.Repository;
using MongoDB.Repository.IEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.Repository.Mongodb
{
    public class MongodbLogBaseRepository<TEntity, TKey> : MongoRepositoryAsync<TEntity, TKey> where TEntity : class, IEntity<TKey>, new()
    {
        public MongodbLogBaseRepository(MongodbContext context) : base(context.ConnectionString, nameof(MongoDB.APILogs))
        {

        }
    }
}
