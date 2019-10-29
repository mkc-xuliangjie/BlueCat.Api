using BlueCat.Repository;
using MongoDB.Repository;
using MongoDB.Repository.IEntity;

namespace BlueCat.Repository
{
    //public class BaseRepositoryShadow<TEntity, TKey> : MongoRepositoryAsync<TEntity, TKey> where TEntity : class, IEntity<TKey>, new()
    //{
    //    public BaseRepositoryShadow(MongodbContext context) :base(context.ConnectionString, nameof(MongoDB.Shadow))
    //    {

    //    }
    //}

    public class MongodbBaseRepository<TEntity, TKey> : MongoRepositoryAsync<TEntity, TKey> where TEntity : class, IEntity<TKey>, new()
    {
        public MongodbBaseRepository(MongodbContext context) : base(context.ConnectionString, nameof(MongoDB.RepositoryTest))
        {

        }
    }

}
