using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace BlueCat.Repository
{
    /// <summary>
    /// 上下文对象
    /// </summary>
    public class MongodbContext
    {
        /// <summary>
        /// 获取 DB 的连接字符串
        /// </summary>
        public string ConnectionString { get; }


        /// <summary>
        /// 获取Mongodb配置项目
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="name">在 ConnectionStrings 配置中下指定的节点</param>

        public MongodbContext(IServiceProvider serviceProvider, string name)
        {
            var config = serviceProvider.GetService<IConfiguration>();

            ConnectionString = config.GetConnectionString(name);
        }
    }
}
