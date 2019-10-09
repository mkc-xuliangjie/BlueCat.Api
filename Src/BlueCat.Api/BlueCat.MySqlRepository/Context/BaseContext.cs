using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.MySqlRepository.Context
{
    /// <summary>
    /// 上下文对象
    /// </summary>
    public class BaseContext
    {
        /// <summary>
        /// 获取 DB 的连接字符串
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="name">在 ConnectionStrings 配置中下指定的节点</param>
        public BaseContext(IServiceProvider serviceProvider, string name)
        {
            var config = serviceProvider.GetService<IConfiguration>();

            ConnectionString = config.GetConnectionString(name);
        }
    }
}
