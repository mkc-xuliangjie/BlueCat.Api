using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using BlueCat.ORM;

namespace BlueCat.ORM.DbProviders
{
    /// <summary>
    /// The db provider factory.
    /// </summary>
    public sealed class DbProviderFactory
    {
        #region Private Members

        private static Dictionary<string, DbProvider> providerCache = new Dictionary<string, DbProvider>();

        private DbProviderFactory()
		{
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Creates the db provider.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="connStr">The conn STR.</param>
        /// <returns>The db provider.</returns>
        public static DbProvider CreateDbProvider(string assemblyName, string className, string connStr)
        {
            //Check.Require(!string.IsNullOrEmpty(className), "className could not be null.");
            Check.Require(!string.IsNullOrEmpty(connStr), "connStr could not be null.");

            if (connStr.ToLower().Contains("microsoft.jet.oledb") || connStr.ToLower().Contains(".db3"))
            {
                string mdbPath = connStr.Substring(connStr.ToLower().IndexOf("data source") + "data source".Length + 1).TrimStart(' ', '=');
                if (mdbPath.ToLower().StartsWith("|datadirectory|"))
                {
                    mdbPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\App_Data" + mdbPath.Substring("|datadirectory|".Length);
                }
                else if (mdbPath.StartsWith("~/") || mdbPath.StartsWith("~\\"))
                {
                    mdbPath = mdbPath.Replace("/", "\\").Replace("~\\", AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\");
                }
                else if (mdbPath.StartsWith("./") || mdbPath.StartsWith(".\\"))
                {
                    mdbPath = mdbPath.Replace("/", "\\").Replace(".\\", AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\");
                }

                connStr = connStr.Substring(0, connStr.ToLower().IndexOf("data source")) + "Data Source=" + mdbPath;
            }

            //by default, using sqlserver db provider
            if (string.IsNullOrEmpty(className))
            {
                className = typeof(SqlServer.SqlDbProvider).ToString();
            }
            else if (className.ToLower().IndexOf("System.Data.SqlClient".ToLower()) >= 0 || className.ToLower().Trim() == "sql" || className.ToLower().Trim() == "sqlserver")
            {
                className = typeof(SqlServer.SqlDbProvider).ToString();
            }
            else if (className.ToLower().Trim() == "sql9" || className.ToLower().Trim() == "sqlserver9" || className.ToLower().Trim() == "sqlserver2005" || className.ToLower().Trim() == "sql2005")
            {
                className = typeof(SqlServer.SqlDbProvider9).ToString();
            }
            else if (className.ToLower().IndexOf("oracle".ToLower()) >= 0)
            {
                //className = typeof(Oracle.OracleDbProvider).ToString();
            }
            else if (className.ToLower().IndexOf("access".ToLower()) >= 0)
            {
                //className = typeof(MsAccess.AccessDbProvider).ToString();
            }
            else if (className.ToLower().IndexOf("mysql".ToLower()) >= 0)
            {
                className = typeof(MySql.MySqlDbProvider).ToString();
            }
            else if (className.ToLower().IndexOf("sqlite".ToLower()) >= 0)
            {
                assemblyName = "BlueCat.ORM.Core.AdditionalDbProviders";
                className = "BlueCat.ORM.DbProviders.Sqlite.SqliteDbProvider";
            }
            else if (className.ToLower().IndexOf("postgresql".ToLower()) >= 0)
            {
                className = typeof(PostgreSql.PostgreSqlDbProvider).ToString();
            }

            string cacheKey = string.Concat(assemblyName, className, connStr);
            lock (providerCache)
            {
                if (providerCache.ContainsKey(cacheKey))
                {
                    return providerCache[cacheKey];
                }
                else
                {
                    System.Reflection.Assembly ass;

                    if (assemblyName == null)
                    {
                        ass = typeof(DbProvider).Assembly;
                    }
                    else
                    {
                        ass = System.Reflection.Assembly.Load(assemblyName);
                    }

                    DbProvider retProvider = ass.CreateInstance(className, false, System.Reflection.BindingFlags.Default, null, new object[] { connStr }, null, null) as DbProvider;
                    providerCache.Add(cacheKey, retProvider);
                    return retProvider;
                }
            }
        }

        /// <summary>
        /// Gets the default db provider.
        /// </summary>
        /// <value>The default.</value>
        
        /// <summary>
        /// Creates the db provider.
        /// </summary>
        /// <param name="connStrName">Name of the conn STR.</param>
        /// <returns>The db provider.</returns>
        //public static DbProvider CreateDbProvider(string connStrName)
        //{
        //    Check.Require(!string.IsNullOrEmpty(connStrName), "connStrName could not be null or empty.");



        //    ConnectionStringSettings connStrSetting = ConfigurationManager.ConnectionStrings[connStrName];
        //    Check.Invariant(connStrSetting != null, null, new ConfigurationErrorsException(string.Concat("Cannot find specified connection string setting named as ", connStrName, " in application config file's ConnectionString section.")));
        //    string[] assAndClass = connStrSetting.ProviderName.Split(',');
        //    if (assAndClass.Length > 1)
        //    {
        //        return CreateDbProvider(assAndClass[0].Trim(), assAndClass[1].Trim(), connStrSetting.ConnectionString);
        //    }
        //    else
        //    {
        //        return CreateDbProvider(null, assAndClass[0].Trim(), connStrSetting.ConnectionString);
        //    }
        //}

        #endregion
    }
}
