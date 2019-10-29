using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MongoDB.Repository
{
    /// <summary>
    /// 容器
    /// </summary>
    public class RepositoryContainer
    {
        private static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;


        private static ConcurrentDictionary<string, Lazy<object>> Instances;

        static RepositoryContainer()
        {
            Instances = new ConcurrentDictionary<string, Lazy<object>>();
        }

        #region 注册
        /// <summary>
        /// 注册实例（添加或替换原有对象）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        public static void Register<T>(T service)
            where T : IMongoBaseRepository
        {
            var t = typeof(T);
            var lazy = new Lazy<object>(() => service);

            Instances.AddOrUpdate(GetKey(t), lazy, (x, y) => lazy);
        }

        /// <summary>
        /// 注册实例（添加或替换原有对象）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Register<T>()
            where T : IMongoBaseRepository, new()
        {
            var t = typeof(T);
            var lazy = new Lazy<object>(() => new T());

            Instances.AddOrUpdate(GetKey(t), lazy, (x, y) => lazy);
        }

        /// <summary>
        /// 注册实例（添加或替换原有对象）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        public static void Register<T>(Func<object> function)
            where T : IMongoBaseRepository
        {
            var t = typeof(T);
            var lazy = new Lazy<object>(function);

            Instances.AddOrUpdate(GetKey(t), lazy, (x, y) => lazy);
        }

        /// <summary>
        /// 注册实例（添加或替换原有对象）
        /// </summary>
        /// <param name="t">typeof(T)</param>
        public static void Register(Type t)
        {
            var instance = Activator.CreateInstance(t, true);
            var lazy = new Lazy<object>(() => instance);
            Instances.AddOrUpdate(GetKey(t), lazy, (x, y) => lazy);
        }

        /// <summary>
        /// 注册实例（添加或替换原有对象）
        /// </summary>
        /// <param name="assemblyNames"></param>
        public static void RegisterAll(params string[] assemblyNames)
        {
            LoadAll((t, repo) =>
            {
                var lazy = new Lazy<object>(() => repo);
                Instances.AddOrUpdate(GetKey(t), lazy, (x, y) => lazy);
            }, assemblyNames);
        }
        #endregion

        /// <summary>
        /// 加载所有类型
        /// </summary>
        /// <param name="a">对象</param>
        /// <param name="assemblyNames">类名</param>
        private static void LoadAll(Action<Type, IMongoBaseRepository> a, params string[] assemblyNames)
        {
            foreach (var assemblyName in assemblyNames)
            {
                var classList = Util.GetAllClassByInterface(typeof(IMongoBaseRepository), assemblyName);
                foreach (var c in classList)
                {
                    IMongoBaseRepository repos = null;
                    try
                    {
                        var method = c.GetMethod(Util.CREATE_INSTANCE_METHOD, bindingFlags);
                        if (method != null)
                        {
                            repos = (IMongoBaseRepository)method.Invoke(null, null);
                        }
                    }
                    catch { }
                    if (repos == null)
                    {
                        var instance = Activator.CreateInstance(c, true);
                        repos = (IMongoBaseRepository)instance;
                    }
                    if (repos == null)
                    {
                        throw new MongoModuleException($"this repository({c.Namespace}.{c.Name}) is not create");
                    }

                    a?.Invoke(c, repos);
                }
            }
        }

        /// <summary>
        /// 取出指定对象
        /// </summary>
        /// <typeparam name="T">指定对象类型</typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
            where T : IMongoBaseRepository
        {
            var t = typeof(T);
            var k = GetKey(t);

            if (Instances.TryGetValue(k, out Lazy<object> repository))
            {
                return (T)repository.Value;
            }
            else
            {
                throw new MongoModuleException($"this repository({k}) is not register");
            }
        }

        /// <summary>
        ///  获取该类型的完全限定名称，包括其命名空间，但不包括程序集。
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static string GetKey(Type t)
        {
            return t.FullName;
        }

    }
}
