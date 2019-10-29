using MongoDB.Driver;

namespace MongoDB.Repository
{
    /// <summary>
    /// MogoSession
    /// </summary>
    public class MongoSession
    {
        /// <summary>
        ///写入策略(WriteConcern),https://docs.mongodb.com/manual/reference/write-concern/
        ///Unacknowledged
        ///客户端发出请求丢到socket的时候就收到相应，这个时候客户端不需要等地服务器的应答，但是的本地的驱动还是尽可能的通知客户端网络的异常，这和客户端操作系统的配置有关。就像现实中邮寄信件一样，丢到信箱就结束了，但是如果信箱坏了，发信人还是能知道的。
        ///Acknowledged
        ///这种方式客户端发送接口会等待服务器给的确认，这种方式一定能确保服务器收到了客户端的请求，并且服务器能够异常时，响应客户端。
        ///Journaled
        ///Journaled方式相比Acknowledged的方式是要保证服务器端已经写入到硬盘文件了。对于Acknowledged的方式有可能服务收到请求数据相应客户端后的一瞬间当机了，这个数据就丢失了，但是对于Journaled方式，服务器保证写入到磁盘后再相应客户端，即使当机了，也不会导致数据丢失。
        ///Replica Acknowledged
        ///这个方式和Acknowledged是一样的意思，适用于Replica sets模式。Acknowledged模式下只有一台机器收到了请求就返回了，对于复制集模式有多台机器的情况，可以要求有多台机器收到写入请求后再相应客户端。这种更安全，但是导致了客户端耗时增加，所以要结合自己的场景设置合适的策略。
        /// </summary>
        private WriteConcern _writeConcern;

        /// <summary>
        /// 写入连接
        /// </summary>
        public WriteConcern WriteConcern { get { return _writeConcern; } }

        /// <summary>
        /// 读优先级设置,类型如下：
        /// </summary>
        private ReadPreference _readPreference;

        /// <summary>
        /// 读优先级设置，  
        /// //primary
        ///主节点，默认模式，读操作只在主节点，如果主节点不可用，报错或者抛出异常。
        ///primaryPreferred
        ///首选主节点，大多情况下读操作在主节点，如果主节点不可用，如故障转移，读操作在从节点。
        ///secondary
        ///从节点，读操作只在从节点， 如果从节点不可用，报错或者抛出异常。
        ///secondaryPreferred
        ///首选从节点，大多情况下读操作在从节点，特殊情况（如单主节点架构）读操作在主节点。
        ///nearest
        ///最邻近节点，读操作在最邻近的成员，可能是主节点或者从节点
        /// </summary>
        public ReadPreference ReadPreference { get { return _readPreference; } }

        /// <summary>
        /// MongoClient
        /// </summary>
        private MongoClient _mongoClient;


        /// <summary>
        /// MongoDatabase
        /// </summary>
        public IMongoDatabase Database { get; set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mongoClient">MongoClient</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="writeConcern">WriteConcern选项</param>
        /// <param name="isSlaveOK"></param>
        /// <param name="readPreference"></param>
        public MongoSession(MongoClient mongoClient, string dbName, WriteConcern writeConcern = null, bool isSlaveOK = false, ReadPreference readPreference = null)
        {
            this._writeConcern = writeConcern ?? WriteConcern.Unacknowledged;

            this._readPreference = readPreference ?? ReadPreference.SecondaryPreferred;

            var databaseSettings = new MongoDatabaseSettings();

            databaseSettings.WriteConcern = this._writeConcern;

            databaseSettings.ReadPreference = this._readPreference;

            _mongoClient = mongoClient;

            //连接时间以及

            //if (_mongoClient.Settings.SocketTimeout == TimeSpan.Zero)
            //{
            //    _mongoClient.Settings.SocketTimeout = TimeSpan.FromSeconds(10);
            //}

            //if (_mongoClient.Settings.WaitQueueTimeout == TimeSpan.Zero)
            //{
            //    _mongoClient.Settings.WaitQueueTimeout = TimeSpan.FromSeconds(30);
            //}

            Database = _mongoClient.GetDatabase(dbName, databaseSettings);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connString">数据库链接字符串</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="writeConcern">WriteConcern选项</param>
        /// <param name="isSlaveOK"></param>
        /// <param name="readPreference"></param>
        public MongoSession(string connString, string dbName, WriteConcern writeConcern = null, bool isSlaveOK = false, ReadPreference readPreference = null)
            : this(new MongoClient(connString), dbName, writeConcern, isSlaveOK, readPreference)
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mongoClientSettings">The settings for a MongoDB client</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="writeConcern">WriteConcern选项</param>
        /// <param name="isSlaveOK"></param>
        /// <param name="readPreference"></param>
        public MongoSession(MongoClientSettings mongoClientSettings, string dbName, WriteConcern writeConcern = null, bool isSlaveOK = false, ReadPreference readPreference = null)
            : this(new MongoClient(mongoClientSettings), dbName, writeConcern, isSlaveOK, readPreference)
        { }
    }
}
