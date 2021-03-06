﻿using System;
using System.Collections.Generic;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;
using BlueCat.Extensions;

namespace StackExchangeRedis
{
    /// <summary>
    /// REDIS代理类
    /// </summary>
    public class Redis : IRedisCache
    {

        #region redis连接
        /// <summary>
        /// 得到一个可用的Redis客户端
        /// </summary>
        public IDatabase Client
        {
            get
            {
                return StackExchangeRedisHelper.Instance().GetDatabase();
            }
        }
    

        #endregion

        #region Key处理

        /// <summary>
        /// 删除指定Key
        /// </summary>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return Client.KeyDelete(key);
        }
        /// <summary>
        /// 删除指定key
        /// </summary>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public Task<bool> RemoveAsync(string key)
        {
           

            return Client.KeyDeleteAsync(key);
        }
        /// <summary>
        /// 删除指定的key（一个或多个）
        /// </summary>
        /// <param name="keys">密钥集合</param>
        /// <returns></returns>
        public long Remove(string[] keys)
        {
            var tasks = new Task<bool>[keys.Length];
            var removed = 0L;

            for (var i = 0; i < keys.Length; i++)
                tasks[i] = RemoveAsync(keys[i]);

            for (var i = 0; i < keys.Length; i++)
                if (Client.Wait(tasks[i]))
                    removed++;

            return removed;
        }
        /// <summary>
        /// 删除指定的key（一个或多个）
        /// </summary>
        /// <param name="keys">密钥集合</param>
        /// <returns></returns>
        public async Task<long> RemoveAsync(string[] keys)
        {
            var removed = 0L;

            foreach (var t in keys)
            {
                if (await Client.KeyDeleteAsync(t))
                    removed++;
            }
            return removed;
        }
        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return Client.KeyExists(key);
        }
        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public Task<bool> ContainsAsync(string key)
        {
            return Client.KeyExistsAsync(key);
        }
        /// <summary>
        ///为给定 key 设置过期时间。超时结束后，key将自动被删除
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="expiry">时间</param>
        /// <returns></returns>
        public bool KeyExpire(string key, DateTime? expiry)
        {
            return Client.KeyExpire(key, expiry);
        }
        /// <summary>
        ///为给定 key 设置过期时间。超时结束后，key将自动被删除
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="expiry">时间</param>
        /// <returns></returns>
        public Task<bool> KeyExpireAsync(string key, DateTime? expiry)
        {
            return Client.KeyExpireAsync(key, expiry);
        }
        /// <summary>
        /// 获取 key的有效时间（毫秒）
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public TimeSpan? KeyTimeToLive(string key)
        {
            return Client.KeyTimeToLive(key);
        }
        /// <summary>
        /// 获取 key的有效时间（毫秒）
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public Task<TimeSpan?> KeyTimeToLiveAsync(string key)
        {
            return Client.KeyTimeToLiveAsync(key);
        }
        /// <summary>
        /// 返回 key 所储存的值的类型
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public RedisItemType KeyType(string key)
        {
            return Client.KeyType(key).ToType();
        }
        /// <summary>
        /// 返回 key 所储存的值的类型
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public async Task<RedisItemType> KeyTypeAsync(string key)
        {
            return (await Client.KeyTypeAsync(key)).ToType();
        }

        #endregion

        #region 字符串

        #region 赋值

        /// <summary>
        /// 设置指定key的值，过期时间，若key存在则是修改，否则新增
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="seconds">时间</param>
        /// <returns></returns>
        public bool Set(string key, string value, int seconds = 0)
        {
            return Client.StringSet(key, value, seconds.ToRedisTimeSpan());
        }
        /// <summary>
        /// 设置指定key的值，过期时间，若key存在则是修改，否则新增
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="key">key</param>
        /// <param name="t">实体</param>
        /// <param name="seconds">过期时间</param>
        /// <returns></returns>
        public bool Set<T>(string key, T t, int seconds = 0) where T : class, new()
        {
            return Set(key, t.ToJson(), seconds);
        }
        /// <summary>
        /// 设置指定key的值，过期时间，若key存在则是修改，否则新增
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="seconds">时间</param>
        /// <returns></returns>
        public Task<bool> SetAsync(string key, string value, int seconds = 0)
        {
            return Client.StringSetAsync(key, value, seconds.ToRedisTimeSpan());
        }
        /// <summary>
        /// 设置指定key的值，过期时间，若key存在则是修改，否则新增
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="key">key</param>
        /// <param name="t">类</param>
        /// <param name="seconds">过期时间</param>
        /// <returns></returns>
        public Task<bool> SetAsync<T>(string key, T t, int seconds = 0) where T : class, new()
        {
            return SetAsync(key, t.ToJson(), seconds);
        }
        /// <summary>
        /// 保存多个key- value，若key存在则是修改，否则新增
        /// </summary>
        /// <param name="kvs">键值对</param>
        /// <returns></returns>
        public bool Set(IDictionary<string, string> kvs)
        {
            return Client.StringSet(kvs.ToKeyValuePairArray());
        }
        /// <summary>
        /// 保存多个key- value，若key存在则是修改，否则新增
        /// </summary>
        /// <param name="kvs">键值对</param>
        /// <returns></returns>
        public Task<bool> SetAsync(IDictionary<string, string> kvs)
        {
            return Client.StringSetAsync(kvs.ToKeyValuePairArray());
        }

        #endregion

        #region 添加值
        /// <summary>
        /// 添加指定key的值，过期时间，若key不存在则新增，否则不会执行新增和修改
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="seconds">时间</param>
        /// <returns></returns>
        public bool Add(string key, string value, int seconds = 0)
        {
            return Client.StringSet(key, value, seconds.ToRedisTimeSpan(), When.NotExists);
        }
        /// <summary>
        /// 添加指定key的值，过期时间，若key不存在则新增，否则不会执行新增和修改
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="key">key</param>
        /// <param name="t">类</param>
        /// <param name="seconds">过期时间</param>
        /// <returns></returns>
        public bool Add<T>(string key, T t, int seconds = 0) where T : class, new()
        {
            return Client.StringSet(key, t.ToJson(), seconds.ToRedisTimeSpan(), When.NotExists);
        }
        /// <summary>
        /// 添加指定key的值，过期时间，若key不存在则新增，否则不会执行新增和修改
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="seconds">时间</param>
        /// <returns></returns>
        public Task<bool> AddAsync(string key, string value, int seconds = 0)
        {
            return Client.StringSetAsync(key, value, seconds.ToRedisTimeSpan(), When.NotExists);
        }
        /// <summary>
        /// 保存多个key- value，若key不存在则新增，否则不会执行新增和修改
        /// </summary>
        /// <param name="kvs">键值对</param>
        /// <returns></returns>
        public bool Add(IDictionary<string, string> kvs)
        {
            return Client.StringSet(kvs.ToKeyValuePairArray(), When.NotExists);
        }
        /// <summary>
        /// 异步保存多个key- value，若key不存在则新增，否则不会执行新增和修改
        /// </summary>
        /// <param name="kvs">键值对</param>
        /// <returns></returns>
        public Task<bool> AddAsync(IDictionary<string, string> kvs)
        {
            return Client.StringSetAsync(kvs.ToKeyValuePairArray(), When.NotExists);
        }

        #endregion

        #region 获取值
        /// <summary>
        /// 获取一个值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string Get(string key)
        {
            return Client.StringGet(key);
        }
        /// <summary>
        /// 获取一个实体类
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="key">key</param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            return Get(key).FromJson<T>();

           
        }
        /// <summary>
        /// 获取多个key的值
        /// </summary>
        /// <param name="keys">key集合</param>
        /// <returns></returns>
        public string[] Get(string[] keys)
        {
            return Client.StringGet(keys.ToRedisKeyArray()).ToStringArray();
        }
        /// <summary>
        /// 获取一个key的值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string key)
        {
            return await Client.StringGetAsync(key);
        }
        /// <summary>
        /// 获取多个key的值
        /// </summary>
        /// <param name="keys">key集合</param>
        /// <returns></returns>
        public async Task<string[]> GetAsync(string[] keys)
        {
            return (await Client.StringGetAsync(keys.ToRedisKeyArray())).ToStringArray();
        }

        #endregion

        #region 自增
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment）。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">增量值(默认=1)</param>
        /// <returns></returns>
        public long Increment(string key, long value = 1)
        {
            return Client.StringIncrement(key, value);
        }
        /// <summary>
        ///如果key不存在，那么key的值会先被初始化为0，然后再执行增量命令。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">增量值(默认=1)</param>
        /// <returns></returns>
        public Task<long> IncrementAsync(string key, long value = 1)
        {
            return Client.StringIncrementAsync(key, value);
        }

        #endregion

        #region 自减
        /// <summary>
        /// 如果key不存在，那么key的值会先被初始化为0，然后再执行减量命令。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">减量默认为1</param>
        /// <returns></returns>
        public long Decrement(string key, long value = 1)
        {
            return Client.StringDecrement(key, value);
        }
        /// <summary>
        /// 如果key不存在，那么key的值会先被初始化为0，然后再执行减量命令。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">减量默认为1</param>
        /// <returns></returns>
        public Task<long> DecrementAsync(string key, long value = 1)
        {
            return Client.StringDecrementAsync(key, value);
        }

        #endregion

        #region 获取旧值赋上新值
        /// <summary>
        ///  将给定 key 的值设为 value ，并返回 key 的旧值(old value)
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public string GetSet(string key, string value)
        {
            return Client.StringGetSet(key, value);
        }
        /// <summary>
        ///  将给定 key 的值设为 value ，并返回 key 的旧值(old value)
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<string> GetSetAsync(string key, string value)
        {
            return await Client.StringGetSetAsync(key, value);
        }

        #endregion

        #endregion

        #region 哈希

        #region 哈希赋值
        /// <summary>
        /// 如果hashId集合中存在key/value则不添加返回false，如果不存在在添加key/value,返回true
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool HashSet(string hashId, string key, string value)
        {
            return Client.HashSet(hashId, key, value);
        }
        /// <summary>
        /// 如果hashId集合中存在key/value则不添加返回false，如果不存在在添加key/value,返回true
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Task<bool> HashSetAsync(string hashId, string key, string value)
        {
            return Client.HashSetAsync(hashId, key, value);
        }
        /// <summary>
        /// 存储对象T t到hash集合中
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="key">key</param>
        /// <param name="entity">对象</param>
        public void HashSet<T>(string key, T entity) where T : class, new()
        {
            var hashEntry = entity.GetHashEntry(typeof(T).GetEntityProperties());
            Client.HashSet(key, hashEntry);
        }
        /// <summary>
        /// 存储对象T t到hash集合中
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="key">key</param>
        /// <param name="entity">对象</param>
        public void HashSet<T>(string key, Expression<Func<T>> expression) where T : class, new()
        {
            var hashEntry = RedisExpression<T>.HashEntry(expression);
            Client.HashSet(key, hashEntry);
        }

        #endregion

        #region 哈希自增
        /// <summary>
        ///  给hashid数据集key的value加自增量，返回相加后的数据
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public long HashIncrement(string hashId, string key, long value = 1)
        {
            return Client.HashIncrement(hashId, key, value);
        }
        /// <summary>
        ///  给hashid数据集key的value加自增量，返回相加后的数据
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Task<long> HashIncrementAsync(string hashId, string key, long value = 1)
        {
            return Client.HashIncrementAsync(hashId, key, value);
        }

        #endregion

        #region 哈希自减
        /// <summary>
        ///  给hashid数据集key的value减去自减量，返回相加后的数据
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public long HashDecrement(string hashId, string key, long value = 1)
        {
            return Client.HashDecrement(hashId, key, value);
        }
        /// <summary>
        ///  给hashid数据集key的value减去自减量，返回相加后的数据
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Task<long> HashDecrementAsync(string hashId, string key, long value = 1)
        {
            return Client.HashDecrementAsync(hashId, key, value);
        }

        #endregion

        #region 查询数据是否存在
        /// <summary>
        /// 判断hashid数据集中是否存在key的数据
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public bool HashExists(string hashId, string key)
        {
            return Client.HashExists(hashId, key);
        }
        /// <summary>
        /// 判断hashid数据集中是否存在key的数据
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public Task<bool> HashExistsAsync(string hashId, string key)
        {
            return Client.HashExistsAsync(hashId, key);
        }

        #endregion

        #region 删除
        /// <summary>
        /// 删除hashid数据集中的key数据
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public bool HashDelete(string hashId, string key)
        {
            return Client.HashDelete(hashId, key);
        }
        /// <summary>
        /// 删除hashid数据集中的key数据
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public Task<bool> HashDeleteAsync(string hashId, string key)
        {
            return Client.HashDeleteAsync(hashId, key);
        }
        /// <summary>
        ///  删除hashid数据集中的key的集合数据
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <param name="keys">keys</param>
        /// <returns></returns>
        public long HashDelete(string hashId, string[] keys)
        {
            return Client.HashDelete(hashId, keys.ToRedisValueArray());
        }
        /// <summary>
        ///  删除hashid数据集中的key的集合数据
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <param name="keys">keys</param>
        /// <returns></returns>
        public Task<long> HashDeleteAsync(string hashId, string[] keys)
        {
            return Client.HashDeleteAsync(hashId, keys.ToRedisValueArray());
        }

        #endregion

        #region 获取单条
        /// <summary>
        /// 获取hashid数据集中，key的value数据
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string HashGet(string hashId, string key)
        {
            return Client.HashGet(hashId, key);
        }
        /// <summary>
        /// 获取hashid数据集中，key的value数据
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public Task<RedisValue> HashGetAsync(string hashId, string key)
        {
            return Client.HashGetAsync(hashId, key);
        }

        #endregion

        #region 获取全部
        /// <summary>
        /// 获取所有hashid数据集的key/value数据集合
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <returns></returns>
        public KeyValuePair<string, string>[] HashGetAll(string hashId)
        {
            return Client.HashGetAll(hashId).ToHashPairs();
        }
        /// <summary>
        /// 获取所有hashid数据集的key/value数据集合
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <returns></returns>
        public async Task<KeyValuePair<string, string>[]> HashGetAllAsync(string hashId)
        {
            return (await Client.HashGetAllAsync(hashId)).ToHashPairs();
        }

        #endregion

        #region 获取key集合
        /// <summary>
        /// 获取hashid数据集中所有key的集合
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public string[] HashKeys(string hashId)
        {
            return Client.HashKeys(hashId).ToStringArray();
        }
        /// <summary>
        /// 获取hashid数据集中所有key的集合
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public async Task<string[]> HashKeysAsync(string hashId)
        {
            return (await Client.HashKeysAsync(hashId)).ToStringArray();
        }

        #endregion

        #region 获取值集合
        /// <summary>
        /// 获取hashid数据集中的所有value集合
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public string[] HashValues(string hashId)
        {
            return Client.HashValues(hashId).ToStringArray();
        }
        /// <summary>
        /// 获取hashid数据集中的所有value集合
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public async Task<string[]> HashValuesAsync(string hashId)
        {
            return (await Client.HashValuesAsync(hashId)).ToStringArray();
        }

        #endregion

        #region 获取数据总数
        /// <summary>
        /// 获取hashid数据集中的数据总数
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <returns></returns>
        public long HashLength(string hashId)
        {
            return Client.HashLength(hashId);
        }
        /// <summary>
        /// 获取hashid数据集中的数据总数
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <returns></returns>
        public Task<long> HashLengthAsync(string hashId)
        {
            return Client.HashLengthAsync(hashId);
        }

        #endregion



        #endregion

        #region 列表

        #region 左侧赋值
        /// <summary>
        /// 从左侧向list中添加值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public long ListLeftPush(string key, string value)
        {
            return Client.ListLeftPush(key, value);
        }
        /// <summary>
        /// 从左侧向list中添加值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值集合</param>
        /// <returns></returns>
        public long ListLeftPush(string key, string[] values)
        {
            return Client.ListLeftPush(key, values.ToRedisValueArray());
        }
        /// <summary>
        /// 从左侧向list中添加值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<long> ListLeftPushAsync(string key, string value)
        {
            return await Client.ListLeftPushAsync(key, value);
        }
        /// <summary>
        /// 从左侧向list中添加值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值集合</param>
        /// <returns></returns>
        public async Task<long> ListLeftPushAsync(string key, string[] values)
        {
            return await Client.ListLeftPushAsync(key, values.ToRedisValueArray());
        }

        #endregion

        #region ListRightPush
        /// <summary>
        /// 从右侧向list中添加值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public long ListRightPush(string key, string value)
        {
            return Client.ListRightPush(key, value);
        }
        /// <summary>
        /// 从右侧向list中添加值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值集合</param>
        /// <returns></returns>
        public long ListRightPush(string key, string[] values)
        {
            return Client.ListRightPush(key, values.ToRedisValueArray());
        }
        /// <summary>
        /// 从右侧向list中添加值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<long> ListRightPushAsync(string key, string value)
        {
            return await Client.ListRightPushAsync(key, value);
        }
        /// <summary>
        /// 从右侧向list中添加值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值集合</param>
        /// <returns></returns>
        public async Task<long> ListRightPushAsync(string key, string[] values)
        {
            return await Client.ListRightPushAsync(key, values.ToRedisValueArray());
        }

        #endregion

        #region ListLeftPop
        /// <summary>
        /// 从list的头部移除一个数据，返回移除的值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string ListLeftPop(string key)
        {
            return Client.ListLeftPop(key);
        }
        /// <summary>
        /// 从list的头部移除一个数据，返回移除的值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public async Task<string> ListLeftPopAsync(string key)
        {
            return await Client.ListLeftPopAsync(key);
        }

        #endregion

        #region ListRightPop
        /// <summary>
        /// 从list的尾部移除一个数据，返回移除的数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string ListRightPop(string key)
        {
            return Client.ListRightPop(key);
        }
        /// <summary>
        /// 从list的尾部移除一个数据，返回移除的数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public async Task<string> ListRightPopAsync(string key)
        {
            return await Client.ListRightPopAsync(key);
        }

        #endregion

        #region ListLength
        /// <summary>
        /// 获取list的长度
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public long ListLength(string key)
        {
            return Client.ListLength(key);
        }
        /// <summary>
        /// 获取list的长度
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public Task<long> ListLengthAsync(string key)
        {
            return Client.ListLengthAsync(key);
        }

        #endregion

        #region ListRange
        /// <summary>
        /// 获取某个范围数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始范围</param>
        /// <param name="stop">结束范围</param>
        /// <returns></returns>
        public string[] ListRange(string key, long start = 0, long stop = -1)
        {
            return Client.ListRange(key, start, stop).ToStringArray();
        }
        /// <summary>
        /// 获取某个范围数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始范围</param>
        /// <param name="stop">结束范围</param>
        /// <returns></returns>
        public async Task<string[]> ListRangeAsync(string key, long start = 0, long stop = -1)
        {
            return (await Client.ListRangeAsync(key, start, stop)).ToStringArray();
        }

        #endregion

        #region ListRemove
        /// <summary>
        /// 删除,返回删除成功个数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">要从列表中删除的值</param>
        /// <param name="count">计数</param>
        /// <returns></returns>
        public long ListRemove(string key, string value, long count = 0)
        {
            return Client.ListRemove(key, value, count);
        }
        /// <summary>
        /// 删除,返回删除成功个数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">要从列表中删除的值</param>
        /// <param name="count">计数</param>
        /// <returns></returns>
        public Task<long> ListRemoveAsync(string key, string value, long count = 0)
        {
            return Client.ListRemoveAsync(key, value, count);
        }

        #endregion

        #region ListTrim
        /// <summary>
        /// 获取列表指定范围内的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public void ListTrim(string key, long start, long stop)
        {
            Client.ListTrim(key, start, stop);
        }
        /// <summary>
        /// 获取列表指定范围内的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public Task ListTrimAsync(string key, long start, long stop)
        {
            return Client.ListTrimAsync(key, start, stop);
        }

        #endregion

        #region ListInsertAfter
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pivot"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long ListInsertAfter(string key, string pivot, string value)
        {
            return Client.ListInsertAfter(key, pivot, value);
        }
        /// <summary>
        /// 在pivot之后插入值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pivot">要在其后插入的值</param>
        /// <param name="value">要插入的值</param>
        /// <returns></returns>
        public Task<long> ListInsertAfterAsync(string key, string pivot, string value)
        {
            return Client.ListInsertAfterAsync(key, pivot, value);
        }

        #endregion

        #region ListInsertBefore
        /// <summary>
        /// 在pivot之前插入值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pivot">要在其前插入的值</param>
        /// <param name="value">要插入的值</param>
        /// <returns></returns>
        public long ListInsertBefore(string key, string pivot, string value)
        {
            return Client.ListInsertBefore(key, pivot, value);
        }
        /// <summary>
        /// 在pivot之前插入值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pivot">要在其前插入的值</param>
        /// <param name="value">要插入的值</param>
        /// <returns></returns>
        public Task<long> ListInsertBeforeAsync(string key, string pivot, string value)
        {
            return Client.ListInsertBeforeAsync(key, pivot, value);
        }

        #endregion

        #region ListGetByIndex
        /// <summary>
        /// 通过索引获取列表中的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public string ListGetByIndex(string key, long index)
        {
            return Client.ListGetByIndex(key, index);
        }
        /// <summary>
        /// 通过索引获取列表中的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public async Task<string> ListGetByIndexAsync(string key, long index)
        {
            return await Client.ListGetByIndexAsync(key, index);
        }

        #endregion

        #region ListSetByIndex
        /// <summary>
        ///  通过索引设置列表元素的值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        public void ListSetByIndex(string key, long index, string value)
        {
            Client.ListSetByIndex(key, index, value);
        }
        /// <summary>
        ///  通过索引设置列表元素的值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        public Task ListSetByIndexAsync(string key, long index, string value)
        {
            return Client.ListSetByIndexAsync(key, index, value);
        }

        #endregion

        #endregion

        #region 集合

        #region SetAdd
        /// <summary>
        /// 向集合添加一个值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool SetAdd(string key, string value)
        {
            return Client.SetAdd(key, value);
        }
        /// <summary>
        /// 向集合添加一个或多个值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">一个或多个值</param>
        /// <returns></returns>
        public long SetAdd(string key, string[] values)
        {
            return Client.SetAdd(key, values.ToRedisValueArray());
        }
        /// <summary>
        /// 向集合添加一个值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Task<bool> SetAddAsync(string key, string value)
        {
            return Client.SetAddAsync(key, value);
        }
        /// <summary>
        /// 向集合添加一个或多个值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">一个或多个值</param>
        /// <returns></returns>
        public Task<long> SetAddAsync(string key, string[] values)
        {
            return Client.SetAddAsync(key, values.ToRedisValueArray());
        }

        #endregion

        #region SetCombine
        /// <summary>
        /// 返回由对给定集合的指定操作产生的集合成员
        /// </summary>
        /// <param name="operation">操作类型</param>
        /// <param name="first">第一组集合Key</param>
        /// <param name="second">第二组集合Key</param>
        /// <returns></returns>
        public string[] SetCombine(RedisSetOperation operation, string first, string second)
        {
            return Client.SetCombine(operation.ToSetOperation(), first, second).ToStringArray();
        }
        /// <summary>
        /// 返回由对给定集合的指定操作产生的集合成员
        /// </summary>
        /// <param name="operation">操作类型</param>
        /// <param name="first">第一组集合Key</param>
        /// <param name="second">第二组集合Key</param>
        /// <returns></returns>
        public async Task<string[]> SetCombineAsync(RedisSetOperation operation, string first, string second)
        {
            return (await Client.SetCombineAsync(operation.ToSetOperation(), first, second)).ToStringArray();
        }
        /// <summary>
        /// 返回由对给定集合的指定操作产生的集合成员
        /// </summary>
        /// <param name="operation">操作类型</param>
        /// <param name="keys">要操作的集合key集合</param>
        /// <returns></returns>
        public string[] SetCombine(RedisSetOperation operation, string[] keys)
        {
            return Client.SetCombine(operation.ToSetOperation(), keys.ToRedisKeyArray()).ToStringArray();
        }
        /// <summary>
        /// 返回由对给定集合的指定操作产生的集合成员
        /// </summary>
        /// <param name="operation">操作类型</param>
        /// <param name="keys">要操作的集合key集合</param>
        /// <returns></returns>
        public async Task<string[]> SetCombineAsync(RedisSetOperation operation, string[] keys)
        {
            return (await Client.SetCombineAsync(operation.ToSetOperation(), keys.ToRedisKeyArray())).ToStringArray();
        }

        #endregion

        #region SetCombineAndStore
        /// <summary>
        /// 此命令等于setcombine，但不返回结果集，而是存储在desctination中。如果desctination已经存在，它将被覆盖。
        /// </summary>
        /// <param name="operation">操作类型</param>
        /// <param name="desctination">存储key</param>
        /// <param name="first">第一组集合</param>
        /// <param name="second">第二组集合</param>
        /// <returns></returns>
        public long SetCombineAndStore(RedisSetOperation operation, string desctination, string first, string second)
        {
            return Client.SetCombineAndStore(operation.ToSetOperation(), desctination, first, second);
        }
        /// <summary>
        /// 此命令等于setcombine，但不返回结果集，而是存储在desctination中。如果desctination已经存在，它将被覆盖。
        /// </summary>
        /// <param name="operation">操作类型</param>
        /// <param name="desctination">存储key</param>
        /// <param name="first">第一组集合</param>
        /// <param name="second">第二组集合</param>
        /// <returns></returns>
        public Task<long> SetCombineAndStoreAsync(RedisSetOperation operation, string desctination, string first, string second)
        {
            return Client.SetCombineAndStoreAsync(operation.ToSetOperation(), desctination, first, second);
        }
        /// <summary>
        /// 此命令等于setcombine，但不返回结果集，而是存储在desctination中。如果desctination已经存在，它将被覆盖。
        /// </summary>
        /// <param name="operation">操作类型</param>
        /// <param name="desctination">存储key</param>
        /// <param name="keys">要操作的集合key集合</param>
        /// <returns></returns>
        public long SetCombineAndStore(RedisSetOperation operation, string desctination, string[] keys)
        {
            return Client.SetCombineAndStore(operation.ToSetOperation(), desctination, keys.ToRedisKeyArray());
        }
        /// <summary>
        /// 此命令等于setcombine，但不返回结果集，而是存储在desctination中。如果desctination已经存在，它将被覆盖。
        /// </summary>
        /// <param name="operation">操作类型</param>
        /// <param name="desctination">存储key</param>
        /// <param name="keys">要操作的集合key集合</param>
        /// <returns></returns>
        public Task<long> SetCombineAndStoreAsync(RedisSetOperation operation, string desctination, string[] keys)
        {
            return Client.SetCombineAndStoreAsync(operation.ToSetOperation(), desctination, keys.ToRedisKeyArray());
        }

        #endregion

        #region SetContains
        /// <summary>
        /// 判断 value 元素是否是集合 key 的成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool SetContains(string key, string value)
        {
            return Client.SetContains(key, value);
        }
        /// <summary>
        /// 判断 value 元素是否是集合 key 的成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Task<bool> SetContainsAsync(string key, string value)
        {
            return Client.SetContainsAsync(key, value);
        }

        #endregion

        #region SetLength
        /// <summary>
        /// 查询集合的长度
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public long SetLength(string key)
        {
            return Client.SetLength(key);
        }
        /// <summary>
        /// 查询集合的长度
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public Task<long> SetLengthAsync(string key)
        {
            return Client.SetLengthAsync(key);
        }

        #endregion

        #region SetMembers
        /// <summary>
        /// 查询集合下的所有成员
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string[] SetMembers(string key)
        {
            return Client.SetMembers(key).ToStringArray();
        }
        /// <summary>
        /// 查询集合下的所有成员
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public async Task<string[]> SetMembersAsync(string key)
        {
            return (await Client.SetMembersAsync(key)).ToStringArray();
        }

        #endregion

        #region SetMove
        /// <summary>
        /// 从源集合种移除一个值到目标集合中，如果目标集合已存在，那么只从源集合中删除，目标集合依然会存在
        /// </summary>
        /// <param name="source">源集合</param>
        /// <param name="desctination">目标集合</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool SetMove(string source, string desctination, string value)
        {
            return Client.SetMove(source, desctination, value);
        }
        /// <summary>
        /// 从源集合种移除一个值到目标集合中，如果目标集合已存在，那么只从源集合中删除，目标集合依然会存在
        /// </summary>
        /// <param name="source">源集合</param>
        /// <param name="desctination">目标集合</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Task<bool> SetMoveAsync(string source, string desctination, string value)
        {
            return Client.SetMoveAsync(source, desctination, value);
        }

        #endregion

        #region SetPop
        /// <summary>
        ///  移除并返回集合中的一个随机元素
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string SetPop(string key)
        {
            return Client.SetPop(key);
        }
        /// <summary>
        ///  移除并返回集合中的一个随机元素
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public async Task<string> SetPopAsync(string key)
        {
            return await Client.SetPopAsync(key);
        }

        #endregion

        #region SetRandomMember
        /// <summary>
        ///  返回集合中的一个随机元素
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string SetRandomMember(string key)
        {
            return Client.SetRandomMember(key);
        }
        /// <summary>
        ///  返回集合中的一个随机元素
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public async Task<string> SetRandomMemberAsync(string key)
        {
            return await Client.SetRandomMemberAsync(key);
        }

        #endregion

        #region SetRandomMembers
        /// <summary>
        /// 返回集合中一个或多个随机数的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="count">返回个数</param>
        /// <returns></returns>
        public string[] SetRandomMembers(string key, long count)
        {
            return Client.SetRandomMembers(key, count).ToStringArray();
        }
        /// <summary>
        /// 返回集合中一个或多个随机数的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="count">返回个数</param>
        /// <returns></returns>
        public async Task<string[]> SetRandomMembersAsync(string key, long count)
        {
            return (await Client.SetRandomMembersAsync(key, count)).ToStringArray();
        }

        #endregion

        #region SetRemove
        /// <summary>
        /// 移除集合中一个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool SetRemove(string key, string value)
        {
            return Client.SetRemove(key, value);
        }
        /// <summary>
        /// 移除集合中一个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Task<bool> SetRemoveAsync(string key, string value)
        {
            return Client.SetRemoveAsync(key, value);
        }
        /// <summary>
        /// 移除集合中一个或多个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值集合</param>
        /// <returns></returns>
        public long SetRemove(string key, string[] values)
        {
            return Client.SetRemove(key, values.ToRedisValueArray());
        }
        /// <summary>
        /// 移除集合中一个或多个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值集合</param>
        /// <returns></returns>
        public Task<long> SetRemoveAsync(string key, string[] values)
        {
            return Client.SetRemoveAsync(key, values.ToRedisValueArray());
        }

        #endregion

        #region SetScan
        /// <summary>
        /// 迭代集合中的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pattern">模式</param>
        /// <param name="pageSize">每页多少条</param>
        /// <param name="cursor">位置</param>
        /// <param name="pageOffset">第几页</param>
        /// <returns></returns>
        public string[] SetScan(string key, string pattern = null, int pageSize = 0, long cursor = 0, int pageOffset = 0)
        {
            return Client.SetScan(key, pattern, pageSize, cursor, pageOffset).ToArray().ToStringArray();
        }

        #endregion

        #endregion

        #region 有序集合

        #region SortedSetAdd
        /// <summary>
        /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <param name="score">一个或多个成员分数</param>
        /// <returns></returns>
        public bool SortedSetAdd(string key, string member, double score)
        {
            return Client.SortedSetAdd(key, member, score);
        }
        /// <summary>
        /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <param name="score">一个或多个成员分数</param>
        /// <returns></returns>
        public Task<bool> SortedSetAddAsync(string key, string member, double score)
        {
            return Client.SortedSetAddAsync(key, member, score);
        }
        /// <summary>
        /// 将具有指定分数的所有指定成员添加到存储在键处的排序集。如果指定的成员已经是排序集的成员，则分数更新并在正确位置重新插入元件，以确保正确排序。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="members">成员集合</param>
        /// <returns></returns>
        public long SortedSetAdd(string key, IDictionary<string, double> members)
        {
            return Client.SortedSetAdd(key, members.ToSortedSetEntry());
        }
        /// <summary>
        /// 将具有指定分数的所有指定成员添加到存储在键处的排序集。如果指定的成员已经是排序集的成员，则分数更新并在正确位置重新插入元件，以确保正确排序。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="members">成员集合</param>
        /// <returns></returns>
        public Task<long> SortedSetAddAsync(string key, IDictionary<string, double> members)
        {
            return Client.SortedSetAddAsync(key, members.ToSortedSetEntry());
        }

        #endregion

        #region SortedSetCombineAndStore
        /// <summary>
        /// 对两个排序集计算集合操作，并将结果存储在desctination中，可以选择执行特定聚合（默认值为SUM）。
        /// </summary>
        /// <param name="operation">操作类型</param>
        /// <param name="desctination">目标集合</param>
        /// <param name="first">第一组集合</param>
        /// <param name="second">第二组集合</param>
        /// <param name="aggregate">特定聚合（默认值为SUM）</param>
        /// <returns></returns>
        public long SortedSetCombineAndStore(RedisSetOperation operation, string desctination, string first, string second, RedisAggregate aggregate = RedisAggregate.Sum)
        {
            return Client.SortedSetCombineAndStore(operation.ToSetOperation(), desctination, first, second, aggregate.ToAggregate());
        }
        /// <summary>
        /// 对两个排序集计算集合操作，并将结果存储在desctination中，可以选择执行特定聚合（默认值为SUM）。
        /// </summary>
        /// <param name="operation">操作类型</param>
        /// <param name="desctination">目标集合</param>
        /// <param name="first">第一组集合</param>
        /// <param name="second">第二组集合</param>
        /// <param name="aggregate">特定聚合（默认值为SUM）</param>
        /// <returns></returns>
        public Task<long> SortedSetCombineAndStoreAsync(RedisSetOperation operation, string desctination, string first, string second, RedisAggregate aggregate = RedisAggregate.Sum)
        {
            return Client.SortedSetCombineAndStoreAsync(operation.ToSetOperation(), desctination, first, second, aggregate.ToAggregate());
        }
        /// <summary>
        /// 对多个排序集计算集合操作，并将结果存储在desctination中，可以选择执行特定聚合（默认值为SUM）。
        /// </summary>
        /// <param name="operation">操作类型</param>
        /// <param name="desctination">目标集合</param>
        /// <param name="keys">要操作的集合</param>
        /// <param name="weights">权重</param>
        /// <param name="aggregate">特定聚合（默认值为SUM）</param>
        /// <returns></returns>
        public long SortedSetCombineAndStore(RedisSetOperation operation, string desctination, string[] keys, double[] weights = null, RedisAggregate aggregate = RedisAggregate.Sum)
        {
            return Client.SortedSetCombineAndStore(operation.ToSetOperation(), desctination, keys.ToRedisKeyArray(), weights, aggregate.ToAggregate());
        }
        /// <summary>
        /// 对多个排序集计算集合操作，并将结果存储在desctination中，可以选择执行特定聚合（默认值为SUM）。
        /// </summary>
        /// <param name="operation">操作类型</param>
        /// <param name="desctination">目标集合</param>
        /// <param name="keys">要操作的集合</param>
        /// <param name="weights">权重</param>
        /// <param name="aggregate">特定聚合（默认值为SUM）</param>
        /// <returns></returns>
        public Task<long> SortedSetCombineAndStoreAsync(RedisSetOperation operation, string desctination, string[] keys, double[] weights = null, RedisAggregate aggregate = RedisAggregate.Sum)
        {
            return Client.SortedSetCombineAndStoreAsync(operation.ToSetOperation(), desctination, keys.ToRedisKeyArray(), weights, aggregate.ToAggregate());
        }

        #endregion

        #region SortedSetDecrement
        /// <summary>
        /// 有序集合中对指定成员的分数减去减量
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <param name="value">减量</param>
        /// <returns></returns>
        public double SortedSetDecrement(string key, string member, double value)
        {
            return Client.SortedSetDecrement(key, member, value);
        }
        /// <summary>
        /// 有序集合中对指定成员的分数减去减量
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <param name="value">减量</param>
        /// <returns></returns>
        public Task<double> SortedSetDecrementAsync(string key, string member, double value)
        {
            return Client.SortedSetDecrementAsync(key, member, value);
        }

        #endregion

        #region SortedSetIncrement
        /// <summary>
        /// 有序集合中对指定成员的分数加上增量
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <param name="value">增量</param>
        /// <returns></returns>
        public double SortedSetIncrement(string key, string member, double value)
        {
            return Client.SortedSetIncrement(key, member, value);
        }
        /// <summary>
        /// 有序集合中对指定成员的分数加上增量
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <param name="value">增量</param>
        /// <returns></returns>
        public Task<double> SortedSetIncrementAsync(string key, string member, double value)
        {
            return Client.SortedSetIncrementAsync(key, member, value);
        }

        #endregion

        #region SortedSetLength
        /// <summary>
        /// 返回有序集中指定分数区间内的成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">分数最小（默认无穷负）</param>
        /// <param name="max">分数最大（默认无穷大）</param>
        /// <param name="exclude">是否从范围检查中排除最小值和最大值（默认为包含这两个值）</param>
        /// <returns></returns>
        public long SortedSetLength(string key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, RedisExclude exclude = RedisExclude.None)
        {
            return Client.SortedSetLength(key, min, max, exclude.ToExclude());
        }
        /// <summary>
        /// 返回有序集中指定分数区间内的成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">分数最小（默认无穷负）</param>
        /// <param name="max">分数最大（默认无穷大）</param>
        /// <param name="exclude">是否从范围检查中排除最小值和最大值（默认为包含这两个值）</param>
        /// <returns></returns>
        public Task<long> SortedSetLengthAsync(string key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, RedisExclude exclude = RedisExclude.None)
        {
            return Client.SortedSetLengthAsync(key, min, max, exclude.ToExclude());
        }

        #endregion

        #region SortedSetLengthByValue
        /// <summary>
        /// 返回有序集中指定分数区间内的成员个数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="exclude">是否从范围检查中排除最小值和最大值（默认为包含这两个值）</param>
        /// <returns></returns>
        public long SortedSetLengthByValue(string key, string min, string max, RedisExclude exclude = RedisExclude.None)
        {
            return Client.SortedSetLengthByValue(key, min, max, exclude.ToExclude());
        }
        /// <summary>
        /// 返回有序集中指定分数区间内的成员个数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="exclude">是否从范围检查中排除最小值和最大值（默认为包含这两个值）</param>
        /// <returns></returns>
        public Task<long> SortedSetLengthByValueAsync(string key, string min, string max, RedisExclude exclude = RedisExclude.None)
        {
            return Client.SortedSetLengthByValueAsync(key, min, max, exclude.ToExclude());
        }

        #endregion

        #region SortedSetRangeByRank
        /// <summary>
        ///  返回有序集中指定区间内的成员和分数，通过索引
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="order">排序正序或倒序（默认正序）</param>
        /// <returns></returns>
        public string[] SortedSetRangeByRank(string key, long start = 0, long stop = -1, RedisOrder order = RedisOrder.Ascending)
        {
            return Client.SortedSetRangeByRank(key, start, stop, order.ToOrder()).ToStringArray();
        }
        /// <summary>
        ///  返回有序集中指定区间内的成员列表，通过索引
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="order">排序正序或倒序（默认正序）</param>
        /// <returns></returns>
        public async Task<string[]> SortedSetRangeByRankAsync(string key, long start = 0, long stop = -1, RedisOrder order = RedisOrder.Ascending)
        {
            return (await Client.SortedSetRangeByRankAsync(key, start, stop, order.ToOrder())).ToStringArray();
        }

        #endregion

        #region SortedSetRangeByRankWithScores
        /// <summary>
        /// 返回有序集中指定区间内的成员和分数，通过索引
        /// </summary> 
        /// <param name="key">key</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="order">排序正序或倒序（默认正序）</param>
        /// <returns></returns>
        public KeyValuePair<string, double>[] SortedSetRangeByRankWithScores(string key, long start = 0, long stop = -1, RedisOrder order = RedisOrder.Ascending)
        {
            return Client.SortedSetRangeByRankWithScores(key, start, stop, order.ToOrder()).ToSortedPairs();
        }
        /// <summary>
        /// 返回有序集中指定区间内的成员和分数，通过索引
        /// </summary> 
        /// <param name="key">key</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="order">排序正序或倒序（默认正序）</param>
        /// <returns></returns>
        public async Task<KeyValuePair<string, double>[]> SortedSetRangeByRankWithScoresAsync(string key, long start = 0, long stop = -1, RedisOrder order = RedisOrder.Ascending)
        {
            return (await Client.SortedSetRangeByRankWithScoresAsync(key, start, stop, order.ToOrder())).ToSortedPairs();
        }

        #endregion

        #region SortedSetRangeByScore
        /// <summary>
        /// 返回有序集中指定区间内的成员，通过索引
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="exclude">是否从范围检查中排除最小值和最大值（默认为包含这两个值）</param>
        /// <param name="order"></param>
        /// <param name="skip">第多少条开始</param>
        /// <param name="take">显示多少条</param>
        /// <returns></returns>
        public string[] SortedSetRangeByScore(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, RedisExclude exclude = RedisExclude.None, RedisOrder order = RedisOrder.Ascending, long skip = 0, long take = -1)
        {
            return Client.SortedSetRangeByScore(key, start, stop, exclude.ToExclude(), order.ToOrder(), skip, take).ToStringArray();
        }
        /// <summary>
        /// 返回有序集中指定区间内的成员，通过索引
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="exclude">是否从范围检查中排除最小值和最大值（默认为包含这两个值）</param>
        /// <param name="order"></param>
        /// <param name="skip">第多少条开始</param>
        /// <param name="take">显示多少条</param>
        /// <returns></returns>
        public async Task<string[]> SortedSetRangeByScoreAsync(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, RedisExclude exclude = RedisExclude.None, RedisOrder order = RedisOrder.Ascending, long skip = 0, long take = -1)
        {
            return (await Client.SortedSetRangeByScoreAsync(key, start, stop, exclude.ToExclude(), order.ToOrder(), skip, take)).ToStringArray();
        }

        #endregion

        #region SortedSetRangeByScoreWithScores
        /// <summary>
        /// 返回有序集中指定区间内的成员和分数，通过索引
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="exclude">是否从范围检查中排除最小值和最大值（默认为包含这两个值）</param>
        /// <param name="order"></param>
        /// <param name="skip">第多少条开始</param>
        /// <param name="take">显示多少条</param>
        /// <returns></returns>
        public KeyValuePair<string, double>[] SortedSetRangeByScoreWithScores(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, RedisExclude exclude = RedisExclude.None, RedisOrder order = RedisOrder.Ascending, long skip = 0, long take = -1)
        {
            return Client.SortedSetRangeByScoreWithScores(key, start, stop, exclude.ToExclude(), order.ToOrder(), skip, take).ToSortedPairs();
        }
        /// <summary>
        /// 返回有序集中指定区间内的成员和分数，通过索引
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="exclude">是否从范围检查中排除最小值和最大值（默认为包含这两个值）</param>
        /// <param name="order"></param>
        /// <param name="skip">第多少条开始</param>
        /// <param name="take">显示多少条</param>
        /// <returns></returns>
        public async Task<KeyValuePair<string, double>[]> SortedSetRangeByScoreWithScoresAsync(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, RedisExclude exclude = RedisExclude.None, RedisOrder order = RedisOrder.Ascending, long skip = 0, long take = -1)
        {
            return (await Client.SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude.ToExclude(), order.ToOrder(), skip, take)).ToSortedPairs();
        }

        #endregion

        #region SortedSetRangeByValue
        /// <summary>
        /// 返回有序集中指定分数区间内的成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="exclude">是否从范围检查中排除最小值和最大值（默认为包含这两个值）</param>
        /// <param name="skip">第多少条开始</param>
        /// <param name="take">显示多少条</param>
        /// <returns></returns>

        public string[] SortedSetRangeByValue(string key, string min = null, string max = null, RedisExclude exclude = RedisExclude.None, long skip = 0, long take = -1)
        {
            return Client.SortedSetRangeByValue(key, min, max, exclude.ToExclude(), skip, take).ToStringArray();
        }
        /// <summary>
        /// 返回有序集中指定分数区间内的成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="exclude">是否从范围检查中排除最小值和最大值（默认为包含这两个值）</param>
        /// <param name="skip">第多少条开始</param>
        /// <param name="take">显示多少条</param>
        /// <returns></returns>
        public async Task<string[]> SortedSetRangeByValueAsync(string key, string min = null, string max = null, RedisExclude exclude = RedisExclude.None, long skip = 0, long take = -1)
        {
            return (await Client.SortedSetRangeByValueAsync(key, min, max, exclude.ToExclude(), skip, take)).ToStringArray();
        }

        #endregion

        #region SortedSetRank
        /// <summary>
        /// 返回存储在键上的已排序集中成员的排名，默认情况下，分数从低到高排序。排名（或指数）以0为基础，这意味着得分最低的成员的排名为0。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <param name="order">排序顺序（默认正序）</param>
        /// <returns></returns>
        public long? SortedSetRank(string key, string member, RedisOrder order = RedisOrder.Ascending)
        {
            return Client.SortedSetRank(key, member, order.ToOrder());
        }
        /// <summary>
        /// 返回存储在键上的已排序集中成员的排名，默认情况下，分数从低到高排序。排名（或指数）以0为基础，这意味着得分最低的成员的排名为0。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <param name="order">排序顺序（默认正序）</param>
        /// <returns></returns>
        public Task<long?> SortedSetRankAsync(string key, string member, RedisOrder order = RedisOrder.Ascending)
        {
            return Client.SortedSetRankAsync(key, member, order.ToOrder());
        }

        #endregion

        #region SortedSetRemove
        /// <summary>
        /// 移除有序集合中的一个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public bool SortedSetRemove(string key, string member)
        {
            return Client.SortedSetRemove(key, member);
        }
        /// <summary>
        /// 移除有序集合中的一个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public long SortedSetRemove(string key, string[] members)
        {
            return Client.SortedSetRemove(key, members.ToRedisValueArray());
        }
        /// <summary>
        /// 移除有序集合中的一个或多个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public Task<bool> SortedSetRemoveAsync(string key, string member)
        {
            return Client.SortedSetRemoveAsync(key, member);
        }
        /// <summary>
        /// 移除有序集合中的一个或多个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public Task<long> SortedSetRemoveAsync(string key, string[] members)
        {
            return Client.SortedSetRemoveAsync(key, members.ToRedisValueArray());
        }

        #endregion

        #region SortedSetRemoveRangeByRank
        /// <summary>
        /// 删除存储在键处的排序集中的所有元素，其排名介于开始和停止之间。
        /// 开始和停止都是基于0的索引，0是得分最低的元素。
        /// 这些指数可以是负数，表示从得分最高的元素开始的偏移量。
        /// 例如，-1是得分最高的元素，-2是得分第二的元素，以此类推。
        /// 返回删除的元素数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public long SortedSetRemoveRangeByRank(string key, long start, long stop)
        {
            return Client.SortedSetRemoveRangeByRank(key, start, stop);
        }
        /// <summary>
        /// 删除存储在键处的排序集中的所有元素，其排名介于开始和停止之间。
        /// 开始和停止都是基于0的索引，0是得分最低的元素。
        /// 这些指数可以是负数，表示从得分最高的元素开始的偏移量。
        /// 例如，-1是得分最高的元素，-2是得分第二的元素，以此类推。
        /// 返回删除的元素数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public Task<long> SortedSetRemoveRangeByRankAsync(string key, long start, long stop)
        {
            return Client.SortedSetRemoveRangeByRankAsync(key, start, stop);
        }

        #endregion

        #region SortedSetRemoveRangeByScore
        /// <summary>
        /// 删除存储在键处的排序集中的所有元素，其排名介于开始和停止之间。
        /// 开始和停止都是基于0的索引，0是得分最低的元素。
        /// 这些指数可以是负数，表示从得分最高的元素开始的偏移量。
        /// 例如，-1是得分最高的元素，-2是得分第二的元素，以此类推。
        /// 返回删除的元素数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="exclude">是否从范围检查中排除最小值和最大值（默认为包含这两个值）</param>
        /// <returns></returns>
        public long SortedSetRemoveRangeByScore(string key, double start, double stop, RedisExclude exclude = RedisExclude.None)
        {
            return Client.SortedSetRemoveRangeByScore(key, start, stop, exclude.ToExclude());
        }
        /// <summary>
        /// 删除存储在键处的排序集中的所有元素，其排名介于开始和停止之间。
        /// 开始和停止都是基于0的索引，0是得分最低的元素。
        /// 这些指数可以是负数，表示从得分最高的元素开始的偏移量。
        /// 例如，-1是得分最高的元素，-2是得分第二的元素，以此类推。
        /// 返回删除的元素数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="exclude">是否从范围检查中排除最小值和最大值（默认为包含这两个值）</param>
        /// <returns></returns>

        public Task<long> SortedSetRemoveRangeByScoreAsync(string key, double start, double stop, RedisExclude exclude = RedisExclude.None)
        {
            return Client.SortedSetRemoveRangeByScoreAsync(key, start, stop, exclude.ToExclude());
        }

        #endregion

        #region SortedSetRemoveRangeByValue
        /// <summary>
        /// 删除此排序区间的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小分数</param>
        /// <param name="max">最大分数</param>
        /// <param name="exclude">是否从范围检查中排除最小值和最大值（默认为包含这两个值）</param>
        /// <returns></returns>
        public long SortedSetRemoveRangeByValue(string key, string min, string max, RedisExclude exclude = RedisExclude.None)
        {
            return Client.SortedSetRemoveRangeByValue(key, min, max, exclude.ToExclude());
        }
        /// <summary>
        /// 删除此排序区间的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小分数</param>
        /// <param name="max">最大分数</param>
        /// <param name="exclude">是否从范围检查中排除最小值和最大值（默认为包含这两个值）</param>
        /// <returns></returns>
        public Task<long> SortedSetRemoveRangeByValueAsync(string key, string min, string max, RedisExclude exclude = RedisExclude.None)
        {
            return Client.SortedSetRemoveRangeByValueAsync(key, min, max, exclude.ToExclude());
        }

        #endregion

        #region SortedSetScan
        /// <summary>
        /// 迭代有序集合中的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pattern">模式</param>
        /// <param name="pageSize">每页多少条</param>
        /// <param name="cursor">位置</param>
        /// <param name="pageOffset">第几页</param>
        /// <returns></returns>
        public KeyValuePair<string, double>[] SortedSetScan(string key, string pattern = null, int pageSize = 10, int cursor = 0, int pageOffset = 0)
        {
            return Client.SortedSetScan(key, pattern, pageSize, cursor, pageOffset).ToSortedPairs();
        }

        #endregion

        #region SortedSetScore
        /// <summary>
        /// 返回成员分数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public double? SortedSetScore(string key, string member)
        {
            return Client.SortedSetScore(key, member);
        }
        /// <summary>
        /// 返回成员分数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public Task<double?> SortedSetScoreAsync(string key, string member)
        {
            return Client.SortedSetScoreAsync(key, member);
        }

        #endregion

        #endregion
        
        #region Lua脚本
        /// <summary>
        /// 通过keys进行模糊查询后的批量删除
        /// </summary>
        /// <param name="key"></param>
        public void ScriptEvaluateDelete(string key)
        {
            Client.ScriptEvaluate(LuaScript.Prepare(
               " local ks = redis.call('KEYS', @keypattern) " + //local ks为定义一个局部变量，其中用于存储获取到的keys
               " for i=1,#ks,5000 do " +    //#ks为ks集合的个数, 语句的意思： for(int i = 1; i <= ks.Count; i+=5000)
               "     redis.call('del', unpack(ks, i, math.min(i+4999, #ks))) " + //Lua集合索引值从1为起始，unpack为解包，获取ks集合中的数据，每次5000，然后执行删除
               " end " +
               " return true "
               ),
               new { keypattern = $"{key}*" });

        }
        #endregion

        //public void SetNX(string key)
        //{
        //    Client.setnx
        //}
    }
}
