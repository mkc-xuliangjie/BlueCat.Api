
using System.Threading.Tasks;
using BlueCat.Entity.Mongodb;



namespace BlueCat.Repository
{
    public class UserRepository : MongodbBaseRepository<User, long>, IUserRepository
    {
        public UserRepository(TelsafeScaffoldMongodbContextSample context) :base(context)
        { }

        public async Task<long> GetUserCountAsync()
        {
            return await this.CountAsync(null);
        }
    }
}
