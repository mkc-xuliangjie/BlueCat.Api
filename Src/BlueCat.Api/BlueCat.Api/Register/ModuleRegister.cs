using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace BlueCat.Api.Register
{
    public class ModuleRegister
    {
        /// <summary>
        /// 获取所有MongoDB项目程序集名称
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllAssembliesName()
        {
            var list = new List<string>();
            var deps = DependencyContext.Default;
            //排除所有的系统程序集(Microsoft.***、System.***等)、Nuget下载包
            var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package");
            foreach (var lib in libs)
            {
                try
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    if (assembly.GetName().ToString().Contains("MongoDB"))
                        list.Add(assembly.GetName().ToString());
                }
                catch (System.Exception)
                {

                }
            }
            return list.ToArray();
        }
    }
}
