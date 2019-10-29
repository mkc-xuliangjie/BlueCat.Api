using MongoDB.Repository;

namespace BlueCat.Test.Repositroy
{
    public class User2RepositoryAsync : MongoRepositoryAsync<User2, string>
    {
        private User2RepositoryAsync() :
            base(Repositorys.connString, Repositorys.dbName)
        {

        }
    }
}
